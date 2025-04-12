using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeTestGenV1;
using MaterialSkin.Controls;
using static GenerativeAI.VertexAIModels;


namespace CodeTestGen
{
    public partial class TestcaseViewer : MaterialForm
    {
        private FormMain _mMain;
        public string Xml;
        public TestcaseViewer(string XmlCode,FormMain _main)
        {
           _mMain = _main;
            Xml = XmlCode;
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
            string htmlPath = Path.Combine(Hotro.AppPath, "TestCaseViewer.html");
            if (!File.Exists(htmlPath))
            {
                MessageBox.Show("Không tìm thấy file TestCaseViewer.html!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            webView21.Source = new Uri($"file:///{htmlPath.Replace("\\", "/")}");
            webView21.CoreWebView2.NavigationCompleted += async (sender, e) =>
            {
                await SetTheme();
                await webView21.CoreWebView2.ExecuteScriptAsync($"loadXMLData({JsonConvert.SerializeObject(Xml)});");
            };
        }

        private async void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            string XmlRaw = await webView21.ExecuteScriptAsync("GetXML()");
            Xml = JsonConvert.DeserializeObject<string>(XmlRaw);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
