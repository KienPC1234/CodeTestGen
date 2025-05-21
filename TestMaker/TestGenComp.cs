using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TestMaker
{
    internal class TestGenComp
    {
        private readonly BienDich.CompileResult _compileResult;
        private readonly string _xmlData;

        public TestGenComp(BienDich.CompileResult compileResult, string xmlData)
        {
            _compileResult = compileResult ?? throw new ArgumentNullException("compileResult");
            _xmlData = xmlData ?? throw new ArgumentNullException("xmlData");
        }

        public void ProcessTestCases()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_xmlData))
                {
                    ConsoleUtils.LogError("Dữ liệu XML rỗng.");
                    return;
                }

                XDocument doc;
                try
                {
                    ConsoleUtils.LogInfo("🔍 Đang phân tích XML...");
                    ConsoleUtils.ShowProgressAnimation("Phân tích", 5);
                    doc = XDocument.Parse(_xmlData);
                    ConsoleUtils.LogSuccess("Phân tích XML thành công.");
                }
                catch (Exception ex)
                {
                    ConsoleUtils.LogError(string.Format("Lỗi khi parse XML: {0}", ex.Message));
                    ConsoleUtils.LogError("Vui lòng kiểm tra định dạng XML.");
                    return;
                }

                var testcases = doc.Element("testcases")?.Elements("testcase");
                if (testcases == null)
                {
                    ConsoleUtils.LogError("Không tìm thấy thẻ <testcases> trong XML.");
                    return;
                }

                Console.Write("Có muốn tự động làm sạch (bỏ khoảng trắng đầu/cuối) output testcase không? (Nhấn y/n hoặc Enter để đồng ý): ");
                string trimInput = Console.ReadLine()?.Trim()?.ToLower();
                bool shouldTrim = string.IsNullOrEmpty(trimInput) || trimInput == "y";
                if (trimInput != "y" && trimInput != "n" && !string.IsNullOrEmpty(trimInput))
                {
                    ConsoleUtils.LogInfo("Đầu vào không hợp lệ, mặc định sẽ trim output.");
                }
                ConsoleUtils.LogInfo(shouldTrim ? "Sẽ trim output của testcase." : "Sẽ không trim output của testcase.");

                var results = new List<(string Case, string Output)>();
                int testIndex = 1;

                foreach (var testcase in testcases)
                {
                    string caseNumber = testcase.Attribute("case")?.Value ?? "#" + testIndex;
                    string inputData = testcase.Value?.Trim();

                    if (string.IsNullOrEmpty(inputData))
                    {
                        ConsoleUtils.LogError(string.Format("Testcase {0} không có dữ liệu đầu vào.", caseNumber));
                        continue;
                    }

                    inputData = inputData.Trim().TrimStart('\uFEFF');
                    inputData = new string(inputData.Where(c => !char.IsControl(c) || c == '\n' || c == '\r').ToArray());

                    string output1 = RunTestCase(inputData);
                    string output2 = RunTestCase(inputData);

                    if (output1 == null)
                    {
                        ConsoleUtils.LogError(string.Format("Không chạy được testcase {0}.", caseNumber));
                        continue;
                    }

                    if (shouldTrim)
                    {
                        output1 = output1.Trim();
                        output2 = output2.Trim();
                    }

                    if (output1 != output2)
                    {
                        ConsoleUtils.LogError(string.Format("Kết quả testcase {0} không nhất quán. Output1: {1}, Output2: {2}", caseNumber, output1, output2));
                        continue;
                    }

                    results.Add((caseNumber, output1));
                    ConsoleUtils.LogSuccess(string.Format("Hoàn tất testcase {0}!", caseNumber));
                    testIndex++;
                }

                var testcaseOuts = new XElement("testcaseouts");
                foreach (var result in results)
                {
                    testcaseOuts.Add(new XElement("testcaseout",
                        new XAttribute("case", result.Case),
                        result.Output));
                }

                var root = doc.Root;
                if (root != null)
                {
                    root.Element("testcaseouts")?.Remove();
                    root.Add(testcaseOuts);
                }

                string outputXmlPath = Path.Combine(_compileResult.CompileDir, "testcases.xml");
                try
                {
                    ConsoleUtils.LogInfo("💾 Đang lưu kết quả testcase...");
                    ConsoleUtils.ShowProgressAnimation("Lưu", 3);
                    byte[] xmlBytes = Encoding.ASCII.GetBytes(doc.ToString());
                    File.WriteAllBytes(outputXmlPath, xmlBytes);
                    ConsoleUtils.LogSuccess("Lưu file XML thành công.");
                }
                catch (Exception ex)
                {
                    ConsoleUtils.LogError(string.Format("Lỗi khi lưu file XML: {0}", ex.Message));
                }
            }
            catch (Exception ex)
            {
                ConsoleUtils.LogError(string.Format("Lỗi chung khi xử lý testcases: {0}", ex.Message));
            }
        }

        private string RunTestCase(string inputData)
        {
            try
            {
                return _compileResult.InputFile == null && _compileResult.OutputFile == null
                    ? RunWithStdio(inputData)
                    : RunWithFiles(inputData);
            }
            catch (Exception ex)
            {
                ConsoleUtils.LogError(string.Format("Lỗi khi chạy testcase: {0}", ex.Message));
                return null;
            }
        }

        private string RunWithStdio(string inputData)
        {
            Process process = null;
            try
            {
                File.WriteAllText("input.txt", inputData, new UTF8Encoding(false));
                string cmdCommand = string.Format("type input.txt | \"{0}\" > output.txt 2> error.txt", _compileResult.CompiledPath);

                process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = string.Format("/C {0}", cmdCommand),
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                process.WaitForExit();
                string error = File.Exists("error.txt")
                    ? File.ReadAllText("error.txt", new UTF8Encoding(false)).Trim()
                    : null;
                string output = File.Exists("output.txt")
                    ? File.ReadAllText("output.txt", new UTF8Encoding(false))
                    : null;
                if (!string.IsNullOrWhiteSpace(error))
                {
                    ConsoleUtils.LogError("Lỗi từ chương trình:\n" + error.Trim());
                    return null;
                }

                return output;
            }
            catch (Exception ex)
            {
                ConsoleUtils.LogError(string.Format("Lỗi khi chạy với stdio: {0}", ex.Message));
                return null;
            }
            finally
            {
                process?.Dispose();
                if (File.Exists("output.txt"))
                {
                    File.Delete("output.txt");
                }
                if (File.Exists("error.txt"))
                {
                    File.Delete("error.txt");
                }
                if (File.Exists("input.txt"))
                {
                    File.Delete("input.txt");
                }
            }
        }

        private string RunWithFiles(string inputData)
        {
            string inputFilePath = Path.Combine(_compileResult.CompileDir, _compileResult.InputFile);
            string outputFilePath = Path.Combine(_compileResult.CompileDir, _compileResult.OutputFile);
            var encoding = new UTF8Encoding(false);
            try
            {
                byte[] inputBytes = encoding.GetBytes(inputData + Environment.NewLine);
                File.WriteAllBytes(inputFilePath, inputBytes);
            }
            catch (Exception ex)
            {
                ConsoleUtils.LogError(string.Format("Lỗi khi ghi file input {0}: {1}", inputFilePath, ex.Message));
                return null;
            }

            Process process = null;
            try
            {
                process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = _compileResult.CompiledPath,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0 || !string.IsNullOrWhiteSpace(error))
                {
                    ConsoleUtils.LogError(string.Format("Lỗi khi chạy chương trình: {0}", error));
                    return null;
                }

                try
                {
                    byte[] outputBytes = File.ReadAllBytes(outputFilePath);
                    return encoding.GetString(outputBytes);
                }
                catch (Exception ex)
                {
                    ConsoleUtils.LogError(string.Format("Lỗi khi đọc file output {0}: {1}", outputFilePath, ex.Message));
                    return null;
                }
            }
            catch (Exception ex)
            {
                ConsoleUtils.LogError(string.Format("Lỗi khi chạy với file: {0}", ex.Message));
                return null;
            }
            finally
            {
                process?.Dispose();
            }
        }
    }
}