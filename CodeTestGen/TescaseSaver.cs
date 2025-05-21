using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO.Compression;
using System.Windows.Forms;
using CodeTestGenV1;

namespace CodeTestGen
{
    public class TescaseSaver
    {


        public enum TestCaseFormat
        {
            [Description("Themis")]
            Themis,

            [Description("Online Judge (VNOJ, DMOJ,...)")]
            OnlineJudge,

            [Description("Legacy inputX.txt/outputX.txt")]
            LegacyTxt,

            [Description("XML (CodeTestGen)")]
            XmlCodeTestGen,

            [Description("(Yandex / Polygon)")]
            DotTest,

            [Description("JSON (Web/API)")]
            JsonWebApi,

            [Description("ZIP (Gộp test)")]
            ZipGrouped
        }

        

        public class TestCase : IComparable<TestCase>
        {
            public int TestCaseIndex { get; set; }
            public string TestCaseInput { get; set; }
            public string TestCaseOutput { get; set; }

            public TestCase(int index, string input, string output)
            {
                TestCaseIndex = index;
                TestCaseInput = input;
                TestCaseOutput = output;
            }

            public int CompareTo(TestCase other)
            {
                if (other == null) return 1;
                return this.TestCaseIndex.CompareTo(other.TestCaseIndex);
            }

            public override string ToString()
            {
                return $"Index: {TestCaseIndex}, Input: {TestCaseInput}, Output: {TestCaseOutput}";
            }
        }

        public static List<TestCase> LoadTestCases(string xmlpath)
        {
            XDocument xmlDoc;
            try
            {
                xmlDoc = XDocument.Load(xmlpath);
            }
            catch
            {
                MessageBox.Show("Không thể đọc file XML từ TestCasesPath!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new IOException("Không thể đọc được file");
            }

            var testCases = xmlDoc.Descendants("testcase").Select(t => new
            {
                Case = t.Attribute("case")?.Value,
                Input = t.Value
            }).ToList();

            var testCaseOuts = xmlDoc.Descendants("testcaseout").Select(t => new
            {
                Case = t.Attribute("case")?.Value,
                Output = t.Value
            }).ToList();

            var validTestCases = testCases.Join(testCaseOuts,
                input => input.Case,
                output => output.Case,
                (input, output) => new TestCase(
                    int.Parse(input.Case),
                    input.Input,
                    output.Output)
                )
                .Where(t => !string.IsNullOrEmpty(t.TestCaseInput) && !string.IsNullOrEmpty(t.TestCaseOutput))
                .ToList();

            if (validTestCases.Count == 0)
            {
                MessageBox.Show("Không có testcase hợp lệ (cần cả input và output)!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<TestCase>();  
            }

            return validTestCases;
        }

        public static async Task SaveThesmisTestCasesAsync(List<TestCase> testCases, string savePath, string problemName)
        {
            try
            {
                string problemPath = Path.Combine(savePath, problemName);
                Directory.CreateDirectory(problemPath);

                var sortedTestCases = testCases.OrderBy(tc => tc.TestCaseIndex).ToList();

                foreach (var testCase in sortedTestCases)
                {
                    string testFolderName = $"TEST{testCase.TestCaseIndex:D2}";
                    string testPath = Path.Combine(problemPath, testFolderName);
                    Directory.CreateDirectory(testPath);

                    string inputFilePath = Path.Combine(testPath, $"{problemName}.INP");
                    await Task.Run(() => File.WriteAllText(inputFilePath, testCase.TestCaseInput));

                    string outputFilePath = Path.Combine(testPath, $"{problemName}.OUT");
                    await Task.Run(() => File.WriteAllText(outputFilePath, testCase.TestCaseOutput));
                }

                MessageBox.Show($"Successfully saved {sortedTestCases.Count} test cases for problem '{problemName}' at '{savePath}'.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving test cases: {ex.Message}");
            }
        }

        public static async Task SaveOJTestCasesAsync(List<TestCase> testCases, string savePath, string problemName)
        {
            try
            {
                string problemPath = Path.Combine(savePath, problemName);
                Directory.CreateDirectory(problemPath);

                var sortedTestCases = testCases.OrderBy(tc => tc.TestCaseIndex).ToList();
                int totalPoints = sortedTestCases.Count * 10;
                int totalWeight = sortedTestCases.Sum(tc => tc.TestCaseInput.Length);
                totalWeight = totalWeight == 0 ? 1 : totalWeight; 

                var yamlBuilder = new StringBuilder();
                yamlBuilder.AppendLine("test_cases:");

                for (int i = 0; i < sortedTestCases.Count; i++)
                {
                    var testCase = sortedTestCases[i];
                    string inputFileName = $"{problemName}.{testCase.TestCaseIndex}.in";
                    string outputFileName = $"{problemName}.{testCase.TestCaseIndex}.out";
                    string inputFilePath = Path.Combine(problemPath, inputFileName);
                    string outputFilePath = Path.Combine(problemPath, outputFileName);

                    await Task.Run(() => File.WriteAllText(inputFilePath, testCase.TestCaseInput));
                    await Task.Run(() => File.WriteAllText(outputFilePath, testCase.TestCaseOutput));

                    int points = totalWeight > 0 ? (int)Math.Round((double)testCase.TestCaseInput.Length * totalPoints / totalWeight) : totalPoints / sortedTestCases.Count;
                    points = Math.Max(1, points); 

                    yamlBuilder.AppendLine($"- {{points: {points}, in: {inputFileName}, out: {outputFileName}}}");
                }

                string yamlPath = Path.Combine(problemPath, "init.yml");
                await Task.Run(() => File.WriteAllText(yamlPath, yamlBuilder.ToString()));

                MessageBox.Show($"Successfully saved {sortedTestCases.Count} test cases for problem '{problemName}' at '{savePath}'.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving test cases: {ex.Message}");
            }
        }
        public static async Task SaveLegacyTestCasesAsync(List<TestCase> testCases, string savePath, string problemName)
        {
            try
            {
                string problemPath = Path.Combine(savePath, problemName);
                Directory.CreateDirectory(problemPath);

                var sortedTestCases = testCases.OrderBy(tc => tc.TestCaseIndex).ToList();
                int totalPoints = sortedTestCases.Count * 10;
                int totalWeight = sortedTestCases.Sum(tc => tc.TestCaseInput.Length);
                totalWeight = totalWeight == 0 ? 1 : totalWeight;

                foreach (var testCase in sortedTestCases)
                {
                    string inputFileName = $"input{testCase.TestCaseIndex}.txt";
                    string outputFileName = $"output{testCase.TestCaseIndex}.txt";
                    string inputFilePath = Path.Combine(problemPath, inputFileName);
                    string outputFilePath = Path.Combine(problemPath, outputFileName);

                    string inputContent = testCase.TestCaseInput.EndsWith("\n") ? testCase.TestCaseInput : testCase.TestCaseInput + "\n";
                    string outputContent = testCase.TestCaseOutput.EndsWith("\n") ? testCase.TestCaseOutput : testCase.TestCaseOutput + "\n";

                    await Task.Run(() => File.WriteAllText(inputFilePath, inputContent));
                    await Task.Run(() => File.WriteAllText(outputFilePath, outputContent));
                }

                MessageBox.Show($"Successfully saved {sortedTestCases.Count} test cases for problem '{problemName}' at '{savePath}'.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving test cases: {ex.Message}");
            }
        }

        public static async Task SavePolygonYandexTestCasesAsync(List<TestCase> testCases, string savePath, string problemName)
        {
            try
            {
                string problemPath = Path.Combine(savePath, problemName);
                string testsPath = Path.Combine(problemPath, "tests");
                Directory.CreateDirectory(testsPath);

                var sortedTestCases = testCases.OrderBy(tc => tc.TestCaseIndex).ToList();

                var problem = new XDocument(
                    new XDeclaration("1.0", "UTF-8", null),
                    new XElement("problem",
                        new XAttribute("short-name", problemName),
                        new XAttribute("name", problemName),
                        new XAttribute("type", "acm"),
                        new XAttribute("interactive", "false"),
                        new XElement("names",
                            new XElement("name",
                                new XAttribute("lang", "en"),
                                new XAttribute("value", problemName))),
                        new XElement("judging",
                            new XElement("testset",
                                new XAttribute("name", "tests"),
                                new XAttribute("time-limit", "1000"),
                                new XAttribute("memory-limit", "256"),
                                new XAttribute("output-limit", "65536"),
                                sortedTestCases.Select(tc => new XElement("test",
                                    new XAttribute("name", tc.TestCaseIndex),
                                    new XAttribute("method", "manual"),
                                    new XAttribute("input", $"tests/{tc.TestCaseIndex:D3}"),
                                    new XAttribute("answer", $"tests/{tc.TestCaseIndex:D3}.a")))
                            )
                        )
                    )
                );

                foreach (var testCase in sortedTestCases)
                {
                    string inputFileName = $"{testCase.TestCaseIndex:D3}";
                    string outputFileName = $"{testCase.TestCaseIndex:D3}.a";
                    string inputFilePath = Path.Combine(testsPath, inputFileName);
                    string outputFilePath = Path.Combine(testsPath, outputFileName);

                    string inputContent = testCase.TestCaseInput.EndsWith("\n") ? testCase.TestCaseInput : testCase.TestCaseInput + "\n";
                    string outputContent = testCase.TestCaseOutput.EndsWith("\n") ? testCase.TestCaseOutput : testCase.TestCaseOutput + "\n";

                    await Task.Run(() => File.WriteAllText(inputFilePath, inputContent));
                    await Task.Run(() => File.WriteAllText(outputFilePath, outputContent));
                }

                string xmlPath = Path.Combine(problemPath, "problem.xml");
                await Task.Run(() => problem.Save(xmlPath));

                MessageBox.Show($"Successfully saved {sortedTestCases.Count} test cases for problem '{problemName}' at '{savePath}'.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving test cases: {ex.Message}");
            }
        }

        public static async Task SaveJsonTestCasesAsync(List<TestCase> testCases, string savePath, string problemName)
        {
            try
            {
                string problemPath = Path.Combine(savePath, problemName);
                string testsPath = Path.Combine(problemPath, "tests");
                Directory.CreateDirectory(testsPath);

                var sortedTestCases = testCases.OrderBy(tc => tc.TestCaseIndex).ToList();
                var testPaths = new Dictionary<int, string>();

                foreach (var testCase in sortedTestCases)
                {
                    var outputNumber = long.Parse(testCase.TestCaseOutput.Trim());
                    var jsonPayload = new
                    {
                        input = new
                        {
                            body = new { input = testCase.TestCaseInput }
                        },
                        output = new
                        {
                            body = new { output = outputNumber }
                        }
                    };

                    string jsonFileName = $"{testCase.TestCaseIndex:D2}.json";
                    string jsonFilePath = Path.Combine(testsPath, jsonFileName);
                    string jsonContent = JsonConvert.SerializeObject(jsonPayload, Formatting.Indented);
                    await Task.Run(() => File.WriteAllText(jsonFilePath, jsonContent));

                    testPaths[testCase.TestCaseIndex] = Path.Combine("tests", jsonFileName).Replace("\\", "/");
                }

                var cfgPayload = new
                {
                    problemName,
                    tests = testPaths.OrderBy(kvp => kvp.Key).Select(kvp => new { index = kvp.Key, path = kvp.Value })
                };
                string cfgPath = Path.Combine(problemPath, "cfg.json");
                string cfgContent = JsonConvert.SerializeObject(cfgPayload, Formatting.Indented);
                await Task.Run(() => File.WriteAllText(cfgPath, cfgContent));

                MessageBox.Show($"Successfully saved {sortedTestCases.Count} test cases for problem '{problemName}' at '{savePath}'.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving test cases: {ex.Message}");
            }
        }
        public static async Task SaveZipTestCasesAsync(List<TestCase> testCases, string savePath, string problemName)
        {
            if (testCases == null || testCases.Count == 0)
                throw new ArgumentException("Danh sách test case không được rỗng");

            testCases.Sort();

            string zipFilePath = Path.Combine(savePath, $"{problemName}.zip");

            using (var zipStream = new FileStream(zipFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create))
            {
                foreach (var testCase in testCases)
                {
                    string inputFileName = $"{testCase.TestCaseIndex:D2}.in";
                    string outputFileName = $"{testCase.TestCaseIndex:D2}.ans";

                    var inputEntry = archive.CreateEntry(inputFileName);
                    using (var entryStream = inputEntry.Open())
                    using (var writer = new StreamWriter(entryStream))
                    {
                        await writer.WriteAsync(testCase.TestCaseInput);
                        await writer.FlushAsync();
                    }

                    var outputEntry = archive.CreateEntry(outputFileName);
                    using (var entryStream = outputEntry.Open())
                    using (var writer = new StreamWriter(entryStream))
                    {
                        await writer.WriteAsync(testCase.TestCaseOutput);
                        await writer.FlushAsync();
                    }
                }
            }
        }
    }
}
