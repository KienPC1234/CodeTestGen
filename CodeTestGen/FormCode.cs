using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin.Controls;

namespace CodeTestGenV1
{
    public partial class FormCode : MaterialForm
    {
        public FormCode(string Code)
        {
            InitializeComponent();
            InitializeWebViewAsync();
        }
        private async void InitializeWebViewAsync()
        {
            await webView21.EnsureCoreWebView2Async(null);

            string htmlPath = Path.Combine(Hotro.AppPath, "editor.html");
            if (!File.Exists(htmlPath))
            {
                MessageBox.Show("Không tìm thấy file editor.html!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            webView21.Source = new Uri($"file:///{htmlPath.Replace("\\", "/")}");
            var tcs = new TaskCompletionSource<bool>();

            webView21.CoreWebView2.NavigationCompleted += (sender, e) =>
            {
                if (e.IsSuccess)
                {
                    tcs.SetResult(true);
                }
                else
                {
                    tcs.SetResult(false);
                }
            };

            bool isLoaded = await tcs.Task;

            if (isLoaded)
            {
                await webView21.CoreWebView2.ExecuteScriptAsync($"toggleDarkMode(true);");
                await webView21.CoreWebView2.ExecuteScriptAsync($"toggleDarkMode(true);");
            }
            else
            {
                MessageBox.Show("Editor không thể tải thành công.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
