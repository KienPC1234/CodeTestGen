using CodeTestGenV1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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

}
