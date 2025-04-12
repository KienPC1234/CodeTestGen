using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin.Controls;

namespace CodeTestGenV1
{
    public partial class FormCode : MaterialForm
    {
        public string Code;
        private FormMain _mMain;
        
        public FormCode(string oldCode, FormMain fm)
        {
            _mMain = fm;
            Code = oldCode;
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
            string htmlPath = Path.Combine(Hotro.AppPath, "editor.html");
            if (!File.Exists(htmlPath))
            {
                MessageBox.Show("Không tìm thấy file editor.html!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            webView21.Source = new Uri($"file:///{htmlPath.Replace("\\", "/")}");
            webView21.CoreWebView2.NavigationCompleted += async (sender, e) =>
            {
                await SetTheme();
                await webView21.CoreWebView2.ExecuteScriptAsync($"setText({JsonSerializer.Serialize(Code)});");
            };
        }

        private  async void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            string EditorDataRaw = await webView21.ExecuteScriptAsync("WebViewGetCode()");
            Code = JsonSerializer.Deserialize<string>(EditorDataRaw);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
