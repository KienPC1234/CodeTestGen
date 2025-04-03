using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using System.IO;
using System.Text.Json;
using ReaLTaiizor.Controls;
using ReaLTaiizor.Child.Crown;

namespace CodeTestGenV1
{
    public partial class FormMain :  MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;
        private Settings appSettings;

        public FormMain()
        {
            InitializeComponent();
            materialFlatButton3.Visible = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.MinimumSize = new Size(1000, 600);
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);

            dropDownControl1.Items.Add(new CrownDropDownItem { Text = "Light" });
            dropDownControl1.Items.Add(new CrownDropDownItem { Text = "Dark" });
            
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

                appSettings = Settings.LoadSettings(materialSkinManager, this);
                appSettings.ApplyToForm();
            }
            else
            {
                MessageBox.Show("Editor không thể tải thành công.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        #region event

        private void materialFlatButton1_Click(object sender, EventArgs e) => new AboutBox().ShowDialog();


        private void hopeTabPage1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(hopeTabPage1.SelectedTab != tabPage2)
            {
                materialFlatButton3.Visible = false;
            }
            else
            {
                materialFlatButton3.Visible = true;
            }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void metroLabel1_Click(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void materialLabel3_Click(object sender, EventArgs e)
        {

        }

        private void materialCheckBox1_CheckedChanged(object sender, EventArgs e)
        {

        }


        private void materialFlatButton2_Click_1(object sender, EventArgs e)
        {
            appSettings.UpdateFromForm();
            appSettings.SaveSettings();
            MessageBox.Show("Cài đặt đã được lưu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion
        private void materialFlatButton3_Click(object sender, EventArgs e)
        {
            appSettings.RefreshSettings();
            MessageBox.Show("Đã tải lại cài đặt từ file!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void lostBorderPanel1_Click(object sender, EventArgs e)
        {

        }

        private void materialLabel3_Click_1(object sender, EventArgs e)
        {

        }

        private void materialRaisedButton5_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private async void materialRaisedButton9_Click(object sender, EventArgs e)
        {
            string clipboardText = Clipboard.GetText();
            await webView21.CoreWebView2.ExecuteScriptAsync($"setText({JsonSerializer.Serialize(clipboardText)});");
        }
    }
}