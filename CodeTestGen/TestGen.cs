using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeTestGenV1
{
    internal static class TestGen
    {
        private static readonly string TestMakerPath = Path.Combine(Hotro.AppPath, "TestMaker.exe");
        private static readonly string FullTestCasesPath = Path.Combine(Hotro.AppPath, "fulltestcases.xml");
        private static readonly string TempPythonFile = Path.Combine(Hotro.StuffFolder, "temp_code.py");

        public static async Task<int> RunPython(string code, string pythonPath)
        {
            File.WriteAllText(TempPythonFile, code);

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = pythonPath,
                    Arguments = $"\"{TempPythonFile}\"",
                    WorkingDirectory = Hotro.StuffFolder,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            await Task.Run(() => process.WaitForExit());
            return process.ExitCode;
        }

        public static async Task<(int ExitCode, string XmlData)> RunTestMaker(string xmlDataPath, string scriptData, string compilerPath, string compilerOption)
        {
            if (!File.Exists(TestMakerPath))
            {
                throw new FileNotFoundException($"TestMaker.exe not found at: {TestMakerPath}");
            }

            string base64ScriptData = Convert.ToBase64String(Encoding.UTF8.GetBytes(scriptData));

            using var process = CreateProcess($"-x \"{xmlDataPath}\" -s \"{base64ScriptData}\" -c \"{compilerPath}\" -o \"{compilerOption}\"");
            process.Start();
            await Task.Run(() => process.WaitForExit());

            string xmlData = File.Exists(FullTestCasesPath) ? File.ReadAllText(FullTestCasesPath) : string.Empty;
            return (process.ExitCode, xmlData);
        }

        public static async Task<(int ExitCode, string XmlData)> RunTestMakerExeMode(string xmlDataPath, string exePath)
        {
            if (!File.Exists(TestMakerPath))
            {
                throw new FileNotFoundException($"TestMaker.exe not found at: {TestMakerPath}");
            }

            using var process = CreateProcess($"-x \"{xmlDataPath}\" -e -p \"{exePath}\"");
            process.Start();
            await Task.Run(() => process.WaitForExit());

            string xmlData = File.Exists(FullTestCasesPath) ? File.ReadAllText(FullTestCasesPath) : string.Empty;
            return (process.ExitCode, xmlData);
        }

        public static async Task<(string CompilerPath, string CompilerOption)> GetEditorLanguageAsync(Microsoft.Web.WebView2.WinForms.WebView2 webView, Settings appSettings)
        {
            try
            {
                string languageRaw = await webView.ExecuteScriptAsync("getCurrentLanguage()");
                string language = JsonSerializer.Deserialize<string>(languageRaw);

                return language switch
                {
                    "python" => (appSettings.PythonCompilerPath, ""),
                    "cpp" => (appSettings.CppCompilerPath, appSettings.CppCompilerOptions),
                    "pascal" => (appSettings.PascalCompilerPath, appSettings.PascalCompilerOptions),
                    _ => (appSettings.PythonCompilerPath, "")
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lấy ngôn ngữ: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (null, null);
            }
        }

        private static Process CreateProcess(string arguments)
        {
            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = TestMakerPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    CreateNoWindow = false
                }
            };
        }
    }
}