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

        static void ExitWithPrompt(bool isSuccess)
        {
            Console.WriteLine("Chờ 3 giây hoặc nhấn phím bất kỳ để thoát...");
            if (!Console.KeyAvailable)
            {
                Thread.Sleep(3000);
            }
            else
            {
                Console.ReadKey(true);
            }
            Environment.Exit(isSuccess ? 0 : -1);
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = new UTF8Encoding(false);
            Console.InputEncoding = new UTF8Encoding(false);

            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                ConsoleUtils.LogError("Ứng dụng bị dừng bởi người dùng.");
                ExitWithPrompt(false);
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                ConsoleUtils.LogError(string.Format("Ứng dụng gặp sự cố nghiêm trọng: {0}", e.ExceptionObject.ToString()));
                ExitWithPrompt(false);
            };

            bool isSuccess = false;
            try
            {
                Parser.Default.ParseArguments<Options>(args)
                    .WithParsed(RunWithOptions)
                    .WithNotParsed(errors =>
                    {
                        ConsoleUtils.LogError("Không thể phân tích tham số. Sử dụng --help để xem hướng dẫn.");
                        Environment.Exit(-1);
                    });
                isSuccess = true;
            }
            finally
            {
                ExitWithPrompt(isSuccess);
            }
        }

        static void RunWithOptions(Options opts)
        {
            Console.Clear();
            ConsoleUtils.AnimateBanner();

            string xmlData = null;
            if (string.IsNullOrEmpty(opts.XmlData))
            {
                ConsoleUtils.LogError("Cần cung cấp XmlData hoặc đường dẫn file XML.");
                Environment.Exit(-1);
            }

            if (File.Exists(opts.XmlData))
            {
                try
                {
                    byte[] xmlBytes = File.ReadAllBytes(opts.XmlData);
                    xmlData = Encoding.UTF8.GetString(xmlBytes).TrimStart('\uFEFF').Trim();
                    if (!xmlData.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase))
                    {
                        ConsoleUtils.LogError(string.Format("XML không hợp lệ: '{0}'", xmlData.Substring(0, Math.Min(50, xmlData.Length))));
                        Environment.Exit(-1);
                    }
                    ConsoleUtils.LogSuccess("Đọc file XML thành công.");
                }
                catch (Exception ex)
                {
                    ConsoleUtils.LogError(string.Format("Lỗi khi đọc file XML: {0}", ex.Message));
                    Environment.Exit(-1);
                }
            }
            else
            {
                xmlData = opts.XmlData.TrimStart('\uFEFF').Trim();
                if (!xmlData.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase))
                {
                    ConsoleUtils.LogError(string.Format("XML không hợp lệ: '{0}'", xmlData.Substring(0, Math.Min(50, xmlData.Length))));
                    Environment.Exit(-1);
                }
                ConsoleUtils.LogSuccess("Nhận chuỗi XML hợp lệ.");
            }

            using (var executor = new BienDich.CompilerExecutor())
            {
                BienDich.CompileResult compileResult = null;
                if (opts.IsExeFlag)
                {
                    if (string.IsNullOrEmpty(opts.ExePath))
                    {
                        ConsoleUtils.LogError("Cần cung cấp ExePath khi IsExeFlag là true.");
                        Environment.Exit(-1);
                    }
                    if (!File.Exists(opts.ExePath))
                    {
                        ConsoleUtils.LogError(string.Format("File thực thi '{0}' không tồn tại.", opts.ExePath));
                        Environment.Exit(-1);
                    }
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("📥 Nhập thông tin file I/O (để trống nếu dùng stdio):");
                    Console.ResetColor();
                    Console.Write("  Tên file input: ");
                    string inputFile = Console.ReadLine().Trim();
                    if (string.IsNullOrEmpty(inputFile)) inputFile = null;
                    Console.Write("  Tên file output: ");
                    string outputFile = Console.ReadLine().Trim();
                    if (string.IsNullOrEmpty(outputFile)) outputFile = null;
                    string tempFileName = string.Format("script_{0}{1}", Guid.NewGuid().ToString("N"), Path.GetExtension(opts.ExePath));
                    string tempFolder = executor.GetType().GetField("_tempFolder", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(executor) as string;
                    string tempPath = Path.Combine(tempFolder, tempFileName);
                    try
                    {
                        File.Copy(opts.ExePath, tempPath, true);
                    }
                    catch (Exception ex)
                    {
                        ConsoleUtils.LogError(string.Format("Lỗi khi sao chép file thực thi: {0}", ex.Message));
                        Environment.Exit(-1);
                    }
                    compileResult = new BienDich.CompileResult
                    {
                        InputFile = inputFile,
                        OutputFile = outputFile,
                        CompiledPath = tempPath,
                        CompileDir = tempFolder
                    };
                    ConsoleUtils.LogSuccess("Chuẩn bị file thực thi hoàn tất.");
                }
                else
                {
                    if (string.IsNullOrEmpty(opts.ScriptData) || string.IsNullOrEmpty(opts.CompilerPath))
                    {
                        ConsoleUtils.LogError("Cần cung cấp ScriptData và CompilerPath khi IsExeFlag là false.");
                        Environment.Exit(-1);
                    }
                    string cleanedScriptData = null;
                    try
                    {
                        byte[] scriptBytes = Convert.FromBase64String(opts.ScriptData);
                        cleanedScriptData = Encoding.UTF8.GetString(scriptBytes).TrimStart('\uFEFF');
                    }
                    catch (Exception ex)
                    {
                        ConsoleUtils.LogError(string.Format("Lỗi giải mã Base64: {0}", ex.Message));
                        Environment.Exit(-1);
                    }
                    var compilerArgs = new BienDich.CompilerArguments
                    {
                        CompilerPath = opts.CompilerPath,
                        ScriptData = cleanedScriptData
                    };
                    ConsoleUtils.LogInfo("🔄 Đang xử lý mã nguồn...");
                    ConsoleUtils.ShowProgressAnimation("Xử lý", 5);
                    var extractResult = compilerArgs.ExtractIOFiles();
                    if (extractResult == null)
                    {
                        ConsoleUtils.LogError("Không thể xử lý mã nguồn.");
                        Environment.Exit(-1);
                    }
                    ConsoleUtils.LogInfo("⚙️ Đang biên dịch...");
                    ConsoleUtils.ShowProgressAnimation("Biên dịch", 5);
                    compileResult = executor.ProcessAndCompile(compilerArgs, opts.CompilerOption, extractResult);
                    if (compileResult == null)
                    {
                        ConsoleUtils.LogError("Biên dịch thất bại.");
                        Environment.Exit(-1);
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("✅ Kết quả biên dịch:");
                    Console.ResetColor();
                    ConsoleUtils.LogCompileResult(compileResult);
                }
                Console.Clear();
                ConsoleUtils.AnimateBanner();
                var testGen = new TestGenComp(compileResult, xmlData);
                ConsoleUtils.LogInfo("⏳ Đang chạy testcase...");
                ConsoleUtils.ShowProgressAnimation("Chạy test", 5);
                testGen.ProcessTestCases();
                string xmlOutputPath = Path.Combine(compileResult.CompileDir, "testcases.xml");
                if (File.Exists(xmlOutputPath))
                {
                    byte[] xmlBytes = File.ReadAllBytes(xmlOutputPath);
                    string xmlContent = Encoding.UTF8.GetString(xmlBytes).TrimStart('\uFEFF');
                    string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "fulltestcases.xml");
                    string finalContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine + xmlContent;
                    File.WriteAllText(outputPath, finalContent, new UTF8Encoding(false));
                    ConsoleUtils.LogSuccess(string.Format("🎉 Sinh testcase hoàn tất! Kết quả lưu tại '{0}'.", outputPath));
                }
                else
                {
                    ConsoleUtils.LogError("Không tìm thấy file testcase đầu ra.");
                    Environment.Exit(-1);
                }
            }
        }
    }
}