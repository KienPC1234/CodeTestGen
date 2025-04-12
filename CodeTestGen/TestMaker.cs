using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeTestGenV1;
using MaterialSkin.Controls;

namespace CodeTestGen
{
    public partial class TestMaker : MaterialForm
    {
        private FormMain _mMain;
        public TestMaker(FormMain fm)
        {
            _mMain = fm;
            InitializeComponent();
            InitializeWebViewAsync();
        }


        private async Task SetTheme()
        {
            if (_mMain != null)
            {
                string Mode = _mMain.appSettings.Mode;
                if (Mode == "Dark")
                {
                    await webView21.CoreWebView2.ExecuteScriptAsync($"toggleDarkMode(true);");
                }
                else // Light mode
                {
                    await webView21.CoreWebView2.ExecuteScriptAsync($"toggleDarkMode(false);");
                }

            }
        }
        private async void InitializeWebViewAsync()
        {
            await webView21.EnsureCoreWebView2Async(null);
            //Editor
            string htmlPath = Path.Combine(Hotro.AppPath, "Blocky.html");
            if (!File.Exists(htmlPath))
            {
                MessageBox.Show("Không tìm thấy file Blocky.html!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            webView21.Source = new Uri($"file:///{htmlPath.Replace("\\", "/")}");
            webView21.CoreWebView2.NavigationCompleted += async (sender, e) =>
            {
                await SetTheme();
            };
        }


        private  void materialRaisedButton2_Click(object sender, EventArgs e)
        {
            string htmlPath = Path.Combine(Hotro.AppPath, "editor.html");
            if (!File.Exists(htmlPath))
            {
                MessageBox.Show("Không tìm thấy file editor.html!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            webView21.Source = new Uri($"file:///{htmlPath.Replace("\\", "/")}");
        }

        private  void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            string htmlPath = Path.Combine(Hotro.AppPath, "Blocky.html");
            if (!File.Exists(htmlPath))
            {
                MessageBox.Show("Không tìm thấy file Blocky.html!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            webView21.Source = new Uri($"file:///{htmlPath.Replace("\\", "/")}");
        }

        private async void materialRaisedButton3_Click(object sender, EventArgs e)
        {
            string EditorDataRaw = await webView21.ExecuteScriptAsync("WebViewGetCode()");
            string EditorData = JsonSerializer.Deserialize<string>(EditorDataRaw);
            _mMain.fastColoredTextBox1.Text = "NoAIFlag\n" +EditorData;
            Close();
        }
    }
}
