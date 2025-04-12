using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
                // Kiểm tra XML
                if (string.IsNullOrWhiteSpace(_xmlData))
                {
                    LogError("Dữ liệu XML rỗng.");
                    return;
                }

                // Parse XML
                XDocument doc;
                try
                {
                    LogInfo("🔍 Đang phân tích XML...");
                    ShowProgressAnimation("Phân tích", 5);
                    doc = XDocument.Parse(_xmlData);
                    LogSuccess("Phân tích XML thành công.");
                }
                catch (Exception ex)
                {
                    LogError($"Lỗi khi parse XML: {ex.Message}");
                    LogError("Vui lòng kiểm tra định dạng XML.");
                    return;
                }

                var testcases = doc.Element("testcases")?.Elements("testcase");
                if (testcases == null)
                {
                    LogError("Không tìm thấy thẻ <testcases> trong XML.");
                    return;
                }

                var results = new List<(string Case, string Output)>();
                int testIndex = 1;

                foreach (var testcase in testcases)
                {
                    string caseNumber = testcase.Attribute("case")?.Value ?? "#" + testIndex;
                    string inputData = testcase.Value?.Trim();

                    if (string.IsNullOrEmpty(inputData))
                    {
                        LogError($"Testcase {caseNumber} không có dữ liệu đầu vào.");
                        continue;
                    }

                    inputData = inputData.Trim().TrimStart('\uFEFF');
                    inputData = new string(inputData.Where(c => !char.IsControl(c) || c == '\n' || c == '\r').ToArray());


                    string output1 = RunTestCase(inputData);
                    string output2 = RunTestCase(inputData);

                    if (output1 == null)
                    {
                        LogError($"Không chạy được testcase {caseNumber}.");
                        continue;
                    }

                    if (output1 != output2)
                    {
                        LogError($"Kết quả testcase {caseNumber} không nhất quán. Output1: {output1}, Output2: {output2}");
                        continue;
                    }

                    results.Add((caseNumber, output1));
                    LogSuccess($"Hoàn tất testcase {caseNumber}!");
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
                    LogInfo("💾 Đang lưu kết quả testcase...");
                    ShowProgressAnimation("Lưu", 3);
                    byte[] xmlBytes = Encoding.ASCII.GetBytes(doc.ToString());
                    File.WriteAllBytes(outputXmlPath, xmlBytes);
                    LogSuccess("Lưu file XML thành công.");
                }
                catch (Exception ex)
                {
                    LogError($"Lỗi khi lưu file XML: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                LogError($"Lỗi chung khi xử lý testcases: {ex.Message}");
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
                LogError($"Lỗi khi chạy testcase: {ex.Message}");
                return null;
            }
        }

        private string RunWithStdio(string inputData)
        {
            Process process = null;
            try
            {
                if (File.Exists("output.txt"))
                {
                    File.Delete("output.txt");
                }
                if (File.Exists("error.txt"))
                {
                    File.Delete("error.txt");
                }

                File.WriteAllText("input.txt", inputData, new UTF8Encoding(false));
                string cmdCommand = $"type input.txt | \"{_compileResult.CompiledPath}\" > output.txt 2> error.txt";

                process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/C {cmdCommand}",
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
                    ? File.ReadAllText("output.txt", new UTF8Encoding(false)).Trim()
                    : null;
                if (!string.IsNullOrWhiteSpace(error))
                {
                    LogError("Lỗi từ chương trình:\n" + error.Trim());
                    return null;
                }
                return output;
            }
            catch (Exception ex)
            {
                LogError($"Lỗi khi chạy với stdio: {ex.Message}");
                return null;
            }
            finally
            {
                process?.Dispose();
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
                LogError($"Lỗi khi ghi file input {inputFilePath}: {ex.Message}");
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
                    LogError($"Lỗi khi chạy chương trình: {error}");
                    return null;
                }

                try
                {
                    byte[] outputBytes = File.ReadAllBytes(outputFilePath);
                    return encoding.GetString(outputBytes);
                }
                catch (Exception ex)
                {
                    LogError($"Lỗi khi đọc file output {outputFilePath}: {ex.Message}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogError($"Lỗi khi chạy với file: {ex.Message}");
                return null;
            }
            finally
            {
                process?.Dispose();
            }
        }

        private void LogError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Lỗi: {message}");
            Console.ResetColor();
        }

        private void LogSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✅ {message}");
            Console.ResetColor();
        }

        private void LogInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private void ShowProgressAnimation(string task, int cycles)
        {
            string[] frames = { "⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏" };
            int cursorLeft = Console.CursorLeft;
            int cursorTop = Console.CursorTop;
            try
            {
                for (int i = 0; i < cycles * frames.Length; i++)
                {
                    Console.SetCursorPosition(cursorLeft, cursorTop);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"{task} {frames[i % frames.Length]}");
                    Console.ResetColor();
                    Console.Out.Flush();
                    Thread.Sleep(50); 
                }
            }
            finally
            {
                Console.SetCursorPosition(cursorLeft, cursorTop);
                Console.Write(new string(' ', task.Length + 2));
                Console.SetCursorPosition(cursorLeft, cursorTop);
            }
        }
    }
}