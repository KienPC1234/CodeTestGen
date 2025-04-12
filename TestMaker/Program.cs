using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using CommandLine;

namespace TestMaker
{
    class Program
    {
        private const string banner = @"
╔════════════════════════════════════════════════════════════════════════════════╗
║ █████   ████   █████████  ██████████      ██████████   ██████████ █████   █████║
║░░███   ███░   ███░░░░░███░░███░░░░███    ░░███░░░░███ ░░███░░░░░█░░███   ░░███ ║
║ ░███  ███    ███     ░░░  ░███   ░░███    ░███   ░░███ ░███  █ ░  ░███    ░███ ║
║ ░███████    ░███          ░███    ░███    ░███    ░███ ░██████    ░███    ░███ ║
║ ░███░░███   ░███          ░███    ░███    ░███    ░███ ░███░░█    ░░███   ███  ║
║ ░███ ░░███  ░░███     ███ ░███    ███     ░███    ███  ░███ ░   █  ░░░█████░   ║
║ █████ ░░████ ░░█████████  ██████████      ██████████   ██████████    ░░███     ║
║░░░░░   ░░░░   ░░░░░░░░░  ░░░░░░░░░░      ░░░░░░░░░░   ░░░░░░░░░░      ░░░      ║
╚═════════════════════════════════════════════════ v1.0 ═════════════════════════╝
Make by KCD DEV (KienTensorFlow) - https://github.com/KienPC1234/CodeTestGen
Chú ý: Vui lòng không thoát khi đang sinh test!

";

        class Options
        {
            [Option('s', "scriptdata", Required = false, HelpText = "Mã nguồn chương trình (Base64).")]
            public string ScriptData { get; set; }

            [Option('c', "compilerpath", Required = false, HelpText = "Đường dẫn đến compiler.")]
            public string CompilerPath { get; set; }

            [Option('o', "compileroption", Required = false, HelpText = "Tùy chọn compiler.")]
            public string CompilerOption { get; set; }

            [Option('e', "isexeflag", Required = false, Default = false, HelpText = "Cờ chỉ định dùng file thực thi có sẵn.")]
            public bool IsExeFlag { get; set; }

            [Option('p', "exepath", Required = false, HelpText = "Đường dẫn file thực thi nếu IsExeFlag là true.")]
            public string ExePath { get; set; }

            [Option('x', "xmldata", Required = false, HelpText = "Dữ liệu XML chứa testcase hoặc đường dẫn file XML.")]
            public string XmlData { get; set; }
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunWithOptions)
                .WithNotParsed(errors =>
                {
                    LogError("Không thể phân tích tham số. Sử dụng --help để xem hướng dẫn.");
                });
        }

        static void RunWithOptions(Options opts)
        {
            Console.Clear();
            AnimateBanner();

            string xmlData;
            if (string.IsNullOrEmpty(opts.XmlData))
            {
                LogError("Cần cung cấp XmlData hoặc đường dẫn file XML.");
                return;
            }

            if (File.Exists(opts.XmlData))
            {
                try
                {
                    byte[] xmlBytes = File.ReadAllBytes(opts.XmlData);
                    xmlData = Encoding.UTF8.GetString(xmlBytes).TrimStart('\uFEFF').Trim();

                    if (!xmlData.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase))
                    {
                        LogError($"XML không hợp lệ: '{xmlData.Substring(0, Math.Min(50, xmlData.Length))}'");
                        return;
                    }

                    LogSuccess("Đọc file XML thành công.");
                }
                catch (Exception ex)
                {
                    LogError($"Lỗi khi đọc file XML: {ex.Message}");
                    return;
                }
            }
            else
            {
                xmlData = opts.XmlData.TrimStart('\uFEFF').Trim();
                if (!xmlData.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase))
                {
                    LogError($"XML không hợp lệ: '{xmlData.Substring(0, Math.Min(50, xmlData.Length))}'");
                    return;
                }

                LogSuccess("Nhận chuỗi XML hợp lệ.");
            }

            using (var executor = new BienDich.CompilerExecutor())
            {
                BienDich.CompileResult compileResult = null;

                if (opts.IsExeFlag)
                {
                    if (string.IsNullOrEmpty(opts.ExePath))
                    {
                        LogError("Cần cung cấp ExePath khi IsExeFlag là true.");
                        return;
                    }

                    if (!File.Exists(opts.ExePath))
                    {
                        LogError($"File thực thi '{opts.ExePath}' không tồn tại.");
                        return;
                    }

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("📥 Nhập thông tin file I/O (để trống nếu dùng stdio):");
                    Console.ResetColor();
                    Console.Write("  Tên file input: ");
                    string inputFile = Console.ReadLine()?.Trim() ?? null;
                    Console.Write("  Tên file output: ");
                    string outputFile = Console.ReadLine()?.Trim() ?? null;

                    string tempFileName = $"script_{Guid.NewGuid():N}{Path.GetExtension(opts.ExePath)}";
                    string tempFolder = executor.GetType().GetField("_tempFolder", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(executor) as string;
                    string tempPath = Path.Combine(tempFolder, tempFileName);

                    try
                    {
                        File.Copy(opts.ExePath, tempPath, true);
                    }
                    catch (Exception ex)
                    {
                        LogError($"Lỗi khi sao chép file thực thi: {ex.Message}");
                        return;
                    }

                    compileResult = new BienDich.CompileResult
                    {
                        InputFile = inputFile,
                        OutputFile = outputFile,
                        CompiledPath = tempPath,
                        CompileDir = tempFolder
                    };

                    LogSuccess("Chuẩn bị file thực thi hoàn tất.");
                }
                else
                {
                    if (string.IsNullOrEmpty(opts.ScriptData) || string.IsNullOrEmpty(opts.CompilerPath))
                    {
                        LogError("Cần cung cấp ScriptData và CompilerPath khi IsExeFlag là false.");
                        return;
                    }

                    string cleanedScriptData;
                    try
                    {
                        cleanedScriptData = Encoding.UTF8.GetString(Convert.FromBase64String(opts.ScriptData));
                    }
                    catch (Exception ex)
                    {
                        LogError($"Lỗi giải mã Base64: {ex.Message}");
                        return;
                    }

                    var compilerArgs = new BienDich.CompilerArguments
                    {
                        CompilerPath = opts.CompilerPath,
                        ScriptData = cleanedScriptData
                    };

                    LogInfo("🔄 Đang xử lý mã nguồn...");
                    ShowProgressAnimation("Xử lý", 5);
                    var extractResult = compilerArgs.ExtractIOFiles();
                    if (extractResult == null)
                    {
                        LogError("Không thể xử lý mã nguồn.");
                        return;
                    }

                    LogInfo("⚙️ Đang biên dịch...");
                    ShowProgressAnimation("Biên dịch", 5);
                    compileResult = executor.ProcessAndCompile(compilerArgs, opts.CompilerOption, extractResult);
                    if (compileResult == null)
                    {
                        LogError("Biên dịch thất bại.");
                        return;
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("✅ Kết quả biên dịch:");
                    Console.ResetColor();
                    LogCompileResult(compileResult);
                }

                Console.Clear();
                AnimateBanner();

                var testGen = new TestGenComp(compileResult, xmlData);
                LogInfo("⏳ Đang chạy testcase...");
                ShowProgressAnimation("Chạy test", 5);
                testGen.ProcessTestCases();

                string xmlOutputPath = Path.Combine(compileResult.CompileDir, "testcases.xml");
                if (File.Exists(xmlOutputPath))
                {
                    byte[] xmlBytes = File.ReadAllBytes(xmlOutputPath);
                    File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "fulltestcases.xml"), xmlBytes);
                    LogSuccess("🎉 Sinh testcase hoàn tất! Kết quả lưu tại 'fulltestcases.xml'.");
                }
                else
                {
                    LogError("Không tìm thấy file testcase đầu ra.");
                }
            }
        }

        static void LogError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Lỗi: {message}");
            Console.ResetColor();
            Environment.Exit(-1);
        }

        static void LogSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✅ {message}");
            Console.ResetColor();
        }

        static void LogInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        static void LogCompileResult(BienDich.CompileResult result)
        {
            Console.WriteLine($"  📄 Input File: {(result.InputFile ?? "none")}");
            Console.WriteLine($"  📄 Output File: {(result.OutputFile ?? "none")}");
            Console.WriteLine($"  🛠️ Compiled Path: {result.CompiledPath}");
            Console.WriteLine($"  📁 Compile Directory: {result.CompileDir}");
        }

        static void AnimateBanner()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            string[] lines = banner.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                Console.WriteLine(lines[i]);
                Thread.Sleep(50);
            }
            Console.ResetColor();
            Thread.Sleep(200);
        }

        static void ShowProgressAnimation(string task, int cycles)
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