using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace TestMaker
{
    internal class BienDich
    {
        public enum CompilerType
        {
            Cpp,
            Python,
            Pascal,
            Unknown
        }

        public class CompilerArguments
        {
            public string CompilerPath { get; set; }
            public string ScriptData { get; set; }

            public CompilerType DetectCompilerType()
            {
                if (string.IsNullOrWhiteSpace(CompilerPath))
                    return CompilerType.Unknown;

                string file = CompilerPath.Trim().ToLower();
                if (file.EndsWith("g++.exe") || file.Contains("g++"))
                    return CompilerType.Cpp;
                if (file.EndsWith("python.exe") || file.Contains("python"))
                    return CompilerType.Python;
                if (file.EndsWith("fpc.exe") || file.Contains("fpc"))
                    return CompilerType.Pascal;

                return CompilerType.Unknown;
            }

            public Tuple<string, string, string> ExtractIOFiles()
            {
                if (string.IsNullOrWhiteSpace(ScriptData))
                {
                    return Tuple.Create<string, string, string>(null, null, null);
                }

                var type = DetectCompilerType();
                string input = null;
                string output = null;
                string code = ScriptData;

                // Bước 1: Xóa comment và lệnh file I/O có thể xóa
                switch (type)
                {
                    case CompilerType.Cpp:
                        code = Regex.Replace(code, @"(?<![""'])//.*?$|(?<![""'])/\*[\s\S]*?\*/", "", RegexOptions.Multiline);
                        code = Regex.Replace(code,
                            @"\bfreopen\s*\(\s*['""][^'""]*\.(in|out|inp)['""]\s*,[^)]*\)\s*;",
                            "// Removed freopen\n",
                            RegexOptions.Singleline);
                        break;

                    case CompilerType.Python:
                        code = Regex.Replace(code, @"(?<![""'])#.*?$", "", RegexOptions.Multiline);
                        code = Regex.Replace(code,
                            @"\bsys\.(stdin|stdout)\s*=\s*open\s*\([^)]*\)\s*(#.*)?$",
                            "# Removed sys.open\n",
                            RegexOptions.Multiline);
                        break;

                    case CompilerType.Pascal:
                        code = Regex.Replace(code, @"(?<!['""])\{[^}]*\}|(?<!['""])\(\*.*?\*\)|(?<!['""])//.*?$", "", RegexOptions.Singleline | RegexOptions.Multiline);
                        code = Regex.Replace(code,
                            @"\b(assign|reset|rewrite)\s*\([^;]*\.(in|out|inp)[^;]*\)\s*;",
                            "// Removed file operation\n",
                            RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        break;

                    default:
                        return Tuple.Create<string, string, string>(null, null, null);
                }

                // Bước 2: Kiểm tra sự hiện diện của .in, .out, .inp
                bool hasIn = Regex.IsMatch(code, @"\.in\b", RegexOptions.IgnoreCase);
                bool hasOut = Regex.IsMatch(code, @"\.out\b", RegexOptions.IgnoreCase);
                bool hasInp = Regex.IsMatch(code, @"\.inp\b", RegexOptions.IgnoreCase);

                // Nếu không có file I/O, trả null
                if (!hasIn && !hasOut && !hasInp)
                {
                    return Tuple.Create<string, string, string>(null, null, code);
                }

                // Bước 3: Trích xuất tên file
                string filePattern = @"(['""])([^'""]*\.(in|out|inp))\1(?!\s*;)";
                var matches = Regex.Matches(code, filePattern, RegexOptions.IgnoreCase);
                bool hasSpecialChars = Regex.IsMatch(code, @"['""][^'""]*\.(in|out|inp)[^'""]*[{}][^'""]*['""]");
                bool hasVariable = Regex.IsMatch(code, @"['""][^'""]*\s*\+\s*[^'""]*\.(in|out|inp)['""]");

                foreach (Match match in matches)
                {
                    string fileName = match.Groups[2].Value;
                    if ((fileName.EndsWith(".in", StringComparison.OrdinalIgnoreCase) ||
                         fileName.EndsWith(".inp", StringComparison.OrdinalIgnoreCase)) && input == null)
                        input = fileName;
                    else if (fileName.EndsWith(".out", StringComparison.OrdinalIgnoreCase) && output == null)
                        output = fileName;
                }

                // Bước 4: Xử lý trường hợp không rõ ràng
                if (input == null || output == null || hasSpecialChars || hasVariable)
                {
                    Console.WriteLine("Lỗi: Tên file .in, .inp hoặc .out không rõ ràng trong mã nguồn.");
                    if (input == null || hasSpecialChars || hasVariable)
                    {
                        Console.WriteLine("Vui lòng nhập tên file input (.in hoặc .inp):");
                        input = Console.ReadLine()?.Trim();
                    }
                    if (output == null || hasSpecialChars || hasVariable)
                    {
                        Console.WriteLine("Vui lòng nhập tên file output (.out):");
                        output = Console.ReadLine()?.Trim();
                    }

                    if (string.IsNullOrWhiteSpace(input) || string.IsNullOrWhiteSpace(output))
                    {
                        Console.WriteLine("Lỗi: Tên file input/output không được để trống.");
                        return Tuple.Create<string, string, string>(null, null, code);
                    }
                }

                // Bước 5: Xác nhận từ người dùng
                bool confirmed = false;
                while (!confirmed)
                {
                    Console.WriteLine($"Input File: {(input ?? "none")}, Output File: {(output ?? "none")}");
                    Console.WriteLine("Nhấn Enter để chấp nhận, hoặc nhập 'no' để từ chối:");
                    string response = Console.ReadLine()?.Trim().ToLower();

                    if (string.IsNullOrEmpty(response))
                    {
                        confirmed = true; // Enter để chấp nhận
                    }
                    else if (response == "no")
                    {
                        Console.WriteLine("Chọn chế độ:");
                        Console.WriteLine("1. Dùng stdio (không dùng file)");
                        Console.WriteLine("2. Nhập tên file mới");
                        Console.Write("Nhập lựa chọn (1 hoặc 2): ");
                        string choice = Console.ReadLine()?.Trim();

                        if (choice == "1")
                        {
                            return Tuple.Create<string, string, string>(null, null, code);
                        }
                        else if (choice == "2")
                        {
                            Console.WriteLine("Nhập tên file input (bất kỳ extension):");
                            input = Console.ReadLine()?.Trim();
                            Console.WriteLine("Nhập tên file output (bất kỳ extension):");
                            output = Console.ReadLine()?.Trim();

                            if (string.IsNullOrWhiteSpace(input) || string.IsNullOrWhiteSpace(output))
                            {
                                Console.WriteLine("Lỗi: Tên file không được để trống.");
                                return Tuple.Create<string, string, string>(null, null, code);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Lựa chọn không hợp lệ, thử lại.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Đầu vào không hợp lệ, nhấn Enter hoặc 'no'.");
                    }
                }

                return Tuple.Create(input, output, code);
            }
        }

        public class CompileResult
        {
            public string InputFile { get; set; }
            public string OutputFile { get; set; }
            public string CompiledPath { get; set; }
            public string CompileDir { get; set; } // Thêm trường mới
        }

        public class CompilerExecutor : IDisposable
        {
            private readonly string _tempFolder;
            private bool _disposed;

            public CompilerExecutor()
            {
                _tempFolder = Path.Combine(Path.GetTempPath(), string.Format("TM_SandBox_{0}", Guid.NewGuid().ToString("N")));
                Directory.CreateDirectory(_tempFolder);
            }

            public CompileResult ProcessAndCompile(CompilerArguments args, string compilerOption, Tuple<string, string, string> extractResult)
            {
                if (string.IsNullOrWhiteSpace(args.CompilerPath))
                {
                    Console.WriteLine("Lỗi: Đường dẫn trình biên dịch không được để trống.");
                    return null;
                }

                string fullPath = args.CompilerPath;
                if (!File.Exists(fullPath))
                {
                    string pathEnv = Environment.GetEnvironmentVariable("PATH");
                    if (pathEnv != null)
                    {
                        string[] paths = pathEnv.Split(';');
                        foreach (string path in paths)
                        {
                            string testPath = Path.Combine(path, args.CompilerPath);
                            if (File.Exists(testPath))
                            {
                                fullPath = testPath;
                                break;
                            }
                        }
                    }
                }

                if (!File.Exists(fullPath))
                {
                    Console.WriteLine("Lỗi: Không tìm thấy trình biên dịch trong PATH hoặc đường dẫn không hợp lệ.");
                    return null;
                }

                if (string.IsNullOrWhiteSpace(args.ScriptData))
                {
                    Console.WriteLine("Lỗi: Mã nguồn rỗng, không thể biên dịch.");
                    return null;
                }

                var result = extractResult;

                var type = args.DetectCompilerType();
                string ext;
                switch (type)
                {
                    case CompilerType.Cpp:
                        ext = ".cpp";
                        break;
                    case CompilerType.Pascal:
                        ext = ".pas";
                        break;
                    case CompilerType.Python:
                        ext = ".py";
                        break;
                    default:
                        throw new NotSupportedException("Loại trình biên dịch không được hỗ trợ.");
                }

                string scriptPath = Path.Combine(_tempFolder, string.Format("script_{0}{1}", Guid.NewGuid().ToString("N"), ext));
                try
                {
                    File.WriteAllText(scriptPath, result.Item3);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi khi ghi file mã nguồn: {0}", ex.Message);
                    return null;
                }

                string compiledPath = scriptPath;

                if (type != CompilerType.Python)
                {
                    string exePath = Path.Combine(_tempFolder, string.Format("script_{0}.exe", Guid.NewGuid().ToString("N")));
                    string arguments;
                    switch (type)
                    {
                        case CompilerType.Cpp:
                            arguments = string.Format("{0} \"{1}\" -o \"{2}\"", compilerOption != null ? compilerOption.Trim() : "", scriptPath, exePath);
                            break;
                        case CompilerType.Pascal:
                            arguments = string.Format("{0} \"{1}\" -o\"{2}\"", compilerOption != null ? compilerOption.Trim() : "", scriptPath, exePath);
                            break;
                        default:
                            arguments = "";
                            break;
                    }

                    Process process = null;
                    try
                    {
                        process = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = fullPath,
                                Arguments = arguments,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                UseShellExecute = false,
                                CreateNoWindow = true
                            }
                        };

                        process.Start();
                        string stdout = process.StandardOutput.ReadToEnd();
                        string stderr = process.StandardError.ReadToEnd();
                        process.WaitForExit();

                        if (process.ExitCode != 0)
                        {
                            Console.WriteLine("Lỗi biên dịch:");
                            Console.WriteLine("Chi tiết lỗi: {0}", stderr);
                            Console.WriteLine("Đầu ra: {0}", stdout);
                            return null;
                        }

                        Console.WriteLine("Biên dịch thành công.");
                        compiledPath = exePath;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Lỗi khi chạy trình biên dịch: {0}", ex.Message);
                        return null;
                    }
                    finally
                    {
                        if (process != null)
                        {
                            process.Dispose();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Script Python đã được xử lý và lưu.");
                }

                return new CompileResult
                {
                    InputFile = result.Item1,
                    OutputFile = result.Item2,
                    CompiledPath = compiledPath,
                    CompileDir = _tempFolder // Lưu đường dẫn thư mục
                };
            }

            public void Dispose()
            {
                if (_disposed)
                    return;

                int retries = 3;
                for (int i = 0; i < retries; i++)
                {
                    try
                    {
                        if (Directory.Exists(_tempFolder))
                        {
                            Directory.Delete(_tempFolder, true);
                            Console.WriteLine("Đã xóa thư mục tạm: {0}", _tempFolder);
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Lỗi khi xóa thư mục tạm (lần {0}/{1}): {2}", i + 1, retries, ex.Message);
                        if (i < retries - 1)
                            Thread.Sleep(500);
                    }
                }

                _disposed = true;
            }
        }
    }
}