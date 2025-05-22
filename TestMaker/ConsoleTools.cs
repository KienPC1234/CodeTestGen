using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestMaker
{

    public static class ConsoleUtils
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
        public static void LogError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Lỗi: {message}");
            Console.ResetColor();
        }

        public static void LogSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✅ {message}");
            Console.ResetColor();
        }
        static int lastMessageLength = 0;

        public static void LogSuccessInline(string message)
        {
            
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', lastMessageLength));
            Console.SetCursorPosition(0, Console.CursorTop); 

            Console.ForegroundColor = ConsoleColor.Green;
            string fullMessage = $"✅ {message}";
            Console.Write(fullMessage);
            Console.Out.Flush();
            Console.ResetColor();

            lastMessageLength = fullMessage.Length;
        }


        public static void LogInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void LogWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠️ Cảnh báo: {message}");
            Console.ResetColor();
        }

        public static void AnimateBanner()
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

        public static void ShowProgressAnimation(string task, int cycles)
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
                    Thread.Sleep(50); // Fast animation
                }
            }
            finally
            {
                Console.SetCursorPosition(cursorLeft, cursorTop);
                Console.Write(new string(' ', task.Length + 2));
                Console.SetCursorPosition(cursorLeft, cursorTop);
            }
        }
        public static void LogCompileResult(BienDich.CompileResult result)
        {
            Console.WriteLine(string.Format("  📄 Input File: {0}", result.InputFile ?? "none"));
            Console.WriteLine(string.Format("  📄 Output File: {0}", result.OutputFile ?? "none"));
            Console.WriteLine(string.Format("  🛠️ Compiled Path: {0}", result.CompiledPath));
            Console.WriteLine(string.Format("  📁 Compile Directory: {0}", result.CompileDir));
        }
    }
}
