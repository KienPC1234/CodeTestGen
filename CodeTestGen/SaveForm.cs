using CodeTestGenV1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CodeTestGen.TescaseSaver;

namespace CodeTestGen
{
    public partial class SaveForm : MaterialSkin.Controls.MaterialForm
    {
        private readonly string TestCasesPath;
        private TestCaseFormat _tsf;
        private List<TestCase> _ts;
        private FormMain _fm;
        public SaveForm(TestCaseFormat tsf, List<TestCase> ts, string tcpath, FormMain fm)
        {
            InitializeComponent();
            _tsf = tsf;
            _fm = fm;
            _ts = ts;
            TestCasesPath = tcpath;
            SetTheme();
        }
        private void SetTheme()
        {
            if (_fm != null)
            {
                string Mode = _fm.appSettings.Mode;
                if (Mode == "Dark")
                {
                    panel1.BackColor= Color.FromArgb(29, 35, 44);
                }
                else // Light mode
                {
                    panel1.BackColor = Color.WhiteSmoke;
                }

            }
        }

        private async void materialRaisedButton2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(materialSingleLineTextField2.Text))
            {
                MessageBox.Show("Vui lòng nhập tên test case!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(materialSingleLineTextField1.Text))
            {
                MessageBox.Show("Vui lòng cung cấp thư mục lưu Test!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string testCaseName = materialSingleLineTextField2.Text.Trim();
            string testCasePath = materialSingleLineTextField1.Text.Trim();
            toolStripStatusLabel1.Text = "Đang lưu test case...";
            toolStripProgressBar1.Visible = true;
            switch (_tsf) {
                case TestCaseFormat.XmlCodeTestGen:
                    File.Copy(Path.Combine(testCasePath,testCaseName+".xml"), TestCasesPath);
                    break;
                case TestCaseFormat.Themis:
                    await SaveThesmisTestCasesAsync(_ts, testCasePath, testCaseName);
                    break;
                case TestCaseFormat.OnlineJudge:
                    await SaveOJTestCasesAsync(_ts, testCasePath, testCaseName);
                    break;
                case TestCaseFormat.LegacyTxt:
                    await SaveLegacyTestCasesAsync(_ts, testCasePath, testCaseName);
                    break;
                case TestCaseFormat.DotTest:
                    await SavePolygonYandexTestCasesAsync(_ts, testCasePath, testCaseName);
                    break;
                case TestCaseFormat.JsonWebApi:
                    await SaveJsonTestCasesAsync(_ts, testCasePath, testCaseName);
                    break;
                case TestCaseFormat.ZipGrouped:
                    await SaveZipTestCasesAsync(_ts, testCasePath, testCaseName);
                    break;
                default:
                    return;
            }
            MessageBox.Show("Lưu test case thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Chọn thư mục để lưu test case";
                folderDialog.ShowNewFolderButton = true;

                DialogResult result = folderDialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
                {
                    materialSingleLineTextField1.Text = folderDialog.SelectedPath;
                }
            }
        }

    }
}
