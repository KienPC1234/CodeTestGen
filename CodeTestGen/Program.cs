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
        #region Constants
        private const string StuffFolderName = "Stuff";
        private const string TempPdfFolder = "ctgPDF";
        private const string TempCodeFile = "temp_code.py";
        private const string TestCasesFile = "testcases.xml";
        #endregion

        #region Main Entry Point
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                InitializeApplication();
                PerformCleanup();
                RunApplication();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khởi động ứng dụng: {ex.Message}", "Lỗi Nghiêm Trọng",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Environment.Exit(-1);
            }
        }
        #endregion

        #region Initialization Methods
        private static void InitializeApplication()
        {
            Hotro.StuffFolder = Path.Combine(Hotro.AppPath, StuffFolderName);
            if (!Directory.Exists(Hotro.StuffFolder))
            {
                throw new DirectoryNotFoundException("Không tìm thấy thư mục chứa file quan trọng. Vui lòng cài lại ứng dụng!");
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }
        #endregion

        #region Cleanup Methods
        private static void PerformCleanup()
        {
            try
            {
                // Clean up temporary PDF folder
                string tempPdfPath = Path.Combine(Path.GetTempPath(), TempPdfFolder);
                if (Directory.Exists(tempPdfPath))
                {
                    Directory.Delete(tempPdfPath, true);
                }

                // Clean up temporary code file
                string tempCodePath = Path.Combine(Hotro.StuffFolder, TempCodeFile);
                if (File.Exists(tempCodePath))
                {
                    File.Delete(tempCodePath);
                }

                // Clean up test cases file
                string testCasesPath = Path.Combine(Hotro.StuffFolder, TestCasesFile);
                if (File.Exists(testCasesPath))
                {
                    File.Delete(testCasesPath);
                }
            }
            catch (Exception ex)
            {
                throw new IOException("Lỗi khi dọn dẹp file tạm thời: " + ex.Message);
            }
        }
        #endregion

        #region Application Execution
        private static void RunApplication()
        {
            Application.Run(new FormMain());
        }
        #endregion
    }
}