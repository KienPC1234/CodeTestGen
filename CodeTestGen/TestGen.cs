using CodeTestGenV1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

internal class TestGen
{
    public static async Task<int> RunPython(string code, string pypath)
    {
        return await Task.Run(() =>
        {
            string tempFile = Path.Combine(Hotro.StuffFolder, "temp_code.py");
            File.WriteAllText(tempFile, code);
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = pypath,
                    Arguments = $"\"{tempFile}\"",
                    WorkingDirectory = Hotro.StuffFolder,
                }
            };

            process.Start();
            process.WaitForExit();
            int exitCode = process.ExitCode;
            process.Close();
            return exitCode;
        });
    }
    public static async Task<(int ExitCode, string XmlData)> RunTestMaker(string xmlDataPath, string scriptData, string compilerPath, string compilerOption)
    {
        string appPath = Path.Combine(Hotro.AppPath, "TestMaker.exe");
        string fullTestCasesPath = Path.Combine(Hotro.AppPath, "fulltestcases.xml");

        if (!File.Exists(appPath))
        {
            throw new FileNotFoundException("TestMaker.exe not found at: " + appPath);
        }

        string base64ScriptData = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(scriptData));

        var startInfo = new ProcessStartInfo
        {
            FileName = appPath,
            Arguments = $"-x \"{xmlDataPath}\" -s \"{base64ScriptData}\" -c \"{compilerPath}\" -o \"{compilerOption}\"",
            UseShellExecute = false,
            RedirectStandardOutput = false,
            CreateNoWindow = false
        };

        using (var process = new Process { StartInfo = startInfo })
        {
            process.Start();
            await Task.Run(() => process.WaitForExit());

            string xmlData = File.Exists(fullTestCasesPath) ? File.ReadAllText(fullTestCasesPath) : string.Empty;

            return (process.ExitCode, xmlData);
        }
    }
    public static async Task<(int ExitCode, string XmlData)> RunTestMakerExeMode(string xmlDataPath, string ExePath)
    {
        string appPath = Path.Combine(Hotro.AppPath, "TestMaker.exe");
        string fullTestCasesPath = Path.Combine(Hotro.AppPath, "fulltestcases.xml");

        if (!File.Exists(appPath))
        {
            throw new FileNotFoundException("TestMaker.exe not found at: " + appPath);
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = appPath,
            Arguments = $"-x \"{xmlDataPath}\" -e -p \"{ExePath}\"",
            UseShellExecute = false,
            RedirectStandardOutput = false,
            CreateNoWindow = false
        };

        using (var process = new Process { StartInfo = startInfo })
        {
            process.Start();
            await Task.Run(() => process.WaitForExit());

            string xmlData = File.Exists(fullTestCasesPath) ? File.ReadAllText(fullTestCasesPath) : string.Empty;

            return (process.ExitCode, xmlData);
        }
    }
    public static async Task<(string compPath, string compOption)> GetEditorLanguageAsync(Microsoft.Web.WebView2.WinForms.WebView2 webView, Settings appSetting)
    {
        try
        {
            string language = await webView.ExecuteScriptAsync("getCurrentLanguage()");

            language = JsonSerializer.Deserialize<string>(language);

            switch (language)
            {
                case "python":
                    return (appSetting.PythonCompilerPath, "");
                case "cpp":
                    return (appSetting.CppCompilerPath, appSetting.CppCompilerOptions);
                case "pascal":
                    return (appSetting.PascalCompilerPath, appSetting.PascalCompilerOptions);
                default:
                    return (appSetting.PythonCompilerPath, "");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi lấy ngôn ngữ: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return (null, null);
        }
    }
}
