using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeTestGenV1
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Hotro.StuffFolder = Path.Combine(Hotro.AppPath, "Stuff");
            if (!Directory.Exists(Hotro.StuffFolder)) {
                MessageBox.Show("KHông Tìm Thấy Thư Mục Chứa File Quan Trọng, Vui Lòng Cài Lại App!","Lỗi Nghiêm Trọng",MessageBoxButtons.OK,MessageBoxIcon.Stop);
                Environment.Exit(-1);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
            //cleanUp
            if (Directory.Exists(Path.Combine(Path.GetTempPath(), "ctgPDF")))
            {
                Directory.Delete(Path.Combine(Path.GetTempPath(), "ctgPDF"));
            }
        }
    }
}
