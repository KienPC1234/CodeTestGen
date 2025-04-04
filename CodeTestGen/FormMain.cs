﻿using System;
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
            await VideoPlayer.EnsureCoreWebView2Async(null);
            string VideoPlayerhtmlPath = Path.Combine(Hotro.AppPath, "VideoPlayer.html");
            //HuongDan
            if (!File.Exists(VideoPlayerhtmlPath))
            {
                MessageBox.Show("Không tìm thấy file VideoPlayer.html!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            VideoPlayer.Source = new Uri($"file:///{VideoPlayerhtmlPath.Replace("\\", "/")}");
            //Editor
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
                if (!File.Exists(Path.Combine(Hotro.AppPath, "settings.json")))
                {
                    appSettings = new Settings(materialSkinManager,this);
                    appSettings.ApplyToForm();
                    appSettings.SaveSettings();
                }
                else
                {
                    appSettings = Settings.LoadSettings(materialSkinManager, this);
                    appSettings.ApplyToForm();
                }
            }
            else
            {
                MessageBox.Show("Editor không thể tải thành công.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //csv Loader




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

        private void materialCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (materialCheckBox1.Checked) {
                if (!Directory.Exists(Path.Combine(Hotro.AppPath, "BienDich")))
                {
                    MessageBox.Show("Không tìm thấy thư mục biên dịch của app!","Lỗi",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    materialCheckBox1.Checked = false;
                }
                else
                {
                    materialSingleLineTextField5.Text = Path.Combine(Hotro.AppPath, "python", "python.exe");
                    materialSingleLineTextField4.Text = Path.Combine(Hotro.AppPath, "mingw64", "bin", "g++.exe");
                }
            }
            else
            {
                materialSingleLineTextField5.Text = "python";
                materialSingleLineTextField4.Text = "g++";
            }
        }


        private void materialFlatButton2_Click_1(object sender, EventArgs e)
        {
            var apikey = materialSingleLineTextField1.Text;
            var modeltype = materialSingleLineTextField6.Text;
            var pypath = materialSingleLineTextField5.Text;
            var cpppath = materialSingleLineTextField4.Text;
            if (string.IsNullOrEmpty(apikey)|| string.IsNullOrEmpty(modeltype) || string.IsNullOrEmpty(pypath) || string.IsNullOrEmpty(cpppath))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                appSettings.UpdateFromForm();
                appSettings.SaveSettings();
                MessageBox.Show("Cài đặt đã được lưu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        
        private void materialFlatButton3_Click(object sender, EventArgs e)
        {
            appSettings.RefreshSettings();
            MessageBox.Show("Đã tải lại cài đặt từ file!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private async void materialRaisedButton9_Click(object sender, EventArgs e)
        {
            string clipboardText = Clipboard.GetText();
            await webView21.CoreWebView2.ExecuteScriptAsync($"setText({JsonSerializer.Serialize(clipboardText)});");
        }

        private async void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Code Test (*.py;*.cpp)|*.py;*.cpp";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                string fileContent = File.ReadAllText(filePath);

                await webView21.ExecuteScriptAsync($"setText({JsonSerializer.Serialize(fileContent)})");

                string language = "";
                if (filePath.EndsWith(".py"))
                {
                    language = "1";
                }
                else if (filePath.EndsWith(".cpp"))
                {
                    language = "2";
                }

                await webView21.ExecuteScriptAsync($"changeLanguage({JsonSerializer.Serialize(language)})");
            }
        }

        private void materialRaisedButton7_Click(object sender, EventArgs e)
        {

        }
        #endregion

        private void materialRaisedButton10_Click(object sender, EventArgs e)
        {

        }

        private void materialRaisedButton5_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất.");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
            saveFileDialog.Title = "Lưu dữ liệu CSV";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                StringBuilder csvContent = new StringBuilder();

                csvContent.AppendLine($"{materialLabel11.Text},{crownNumeric1.Value}");

                var header1 = dataGridView1.Columns[0].HeaderText;
                var header2 = dataGridView1.Columns[1].HeaderText;
                csvContent.AppendLine($"{header1},{header2}");

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        var val1 = row.Cells[0].Value?.ToString() ?? "";
                        var val2 = row.Cells[1].Value?.ToString() ?? "";
                        csvContent.AppendLine($"{val1},{val2}");
                    }
                }

                File.WriteAllText(saveFileDialog.FileName, csvContent.ToString(), Encoding.UTF8);
                MessageBox.Show("Đã lưu thành công!");
            }
        }

        private void materialRaisedButton8_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv";
            openFileDialog.Title = "Mở file CSV";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var lines = File.ReadAllLines(openFileDialog.FileName, Encoding.UTF8);

                if (lines.Length < 2)
                {
                    MessageBox.Show("File không hợp lệ.");
                    return;
                }

                var firstLine = lines[0].Split(',');
                materialLabel11.Text = firstLine[0];
                if (decimal.TryParse(firstLine[1], out decimal value))
                {
                    crownNumeric1.Value = value;
                }

                var headerLine = lines[1].Split(',');
                if (dataGridView1.Columns.Count < 2)
                {
                    dataGridView1.Columns.Clear();
                    dataGridView1.Columns.Add("col1", headerLine[0]);
                    dataGridView1.Columns.Add("col2", headerLine[1]);
                }
                else
                {
                    dataGridView1.Columns[0].HeaderText = headerLine[0];
                    dataGridView1.Columns[1].HeaderText = headerLine[1];
                }

                dataGridView1.Rows.Clear();
                for (int i = 2; i < lines.Length; i++)
                {
                    var data = lines[i].Split(',');
                    if (data.Length >= 2)
                    {
                        dataGridView1.Rows.Add(data[0], data[1]);
                    }
                }

                MessageBox.Show("Đã tải dữ liệu thành công.");
            }
        }
    }
}