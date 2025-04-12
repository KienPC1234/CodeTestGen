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
using FastColoredTextBoxNS;
using CodeTestGen;
namespace CodeTestGenV1
{
    public partial class FormMain : MaterialForm
    {
        private readonly TextStyle hyperlinkStyle = new TextStyle(Brushes.Blue, null, FontStyle.Underline);
        private readonly TextStyle numberStyle = new TextStyle(Brushes.Green, null, FontStyle.Bold);
        private readonly MaterialSkinManager materialSkinManager;
        public Settings appSettings;

        public FormMain()
        {
            InitializeComponent();
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            if (!File.Exists(Path.Combine(Hotro.AppPath, "settings.json")))
            {
                appSettings = new Settings(materialSkinManager, this);
                appSettings.ApplyToForm();
                appSettings.SaveSettings();
            }
            else
            {
                appSettings = Settings.LoadSettings(materialSkinManager, this);
                appSettings.ApplyToForm();
            }
            materialFlatButton3.Visible = false;
            this.MinimumSize = new Size(1000, 600);

            dropDownControl1.Items.Add(new CrownDropDownItem { Text = "Dark" });
            dropDownControl1.Items.Add(new CrownDropDownItem { Text = "Light" });

            if (appSettings.Mode == "Dark")
            {
                dropDownControl1.SelectedItem = dropDownControl1.Items[0];
            }
            else
            {
                dropDownControl1.SelectedItem = dropDownControl1.Items[1];
            }

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

            webView21.CoreWebView2.NavigationCompleted += (sender, e) =>
            {
                appSettings.WebViewApply();
            };

        }


        #region event

        private void materialFlatButton1_Click(object sender, EventArgs e) => new AboutBox().ShowDialog();


        private void hopeTabPage1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (hopeTabPage1.SelectedTab != tabPage2)
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
            string bienDichPath = Path.Combine(Hotro.AppPath, "BienDich");
            if (materialCheckBox1.Checked)
            {
                if (!Directory.Exists(bienDichPath))
                {
                    MessageBox.Show("Không tìm thấy thư mục biên dịch của app!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    materialCheckBox1.Checked = false;
                }
                else
                {
                    materialSingleLineTextField5.Text = Path.Combine(bienDichPath, "python", "python.exe");
                    materialSingleLineTextField4.Text = Path.Combine(bienDichPath, "mingw64", "bin", "g++.exe");
                    materialSingleLineTextField8.Text = Path.Combine(bienDichPath, "FPC", "bin", "i386-win32", "fpc.exe");
                }
            }
            else
            {
                materialSingleLineTextField5.Text = "python.exe";
                materialSingleLineTextField4.Text = "g++.exe";
                materialSingleLineTextField8.Text = "fpc.exe";
            }
        }


        private void materialFlatButton2_Click_1(object sender, EventArgs e)
        {
            var apikey = materialSingleLineTextField1.Text;
            var modeltype = materialSingleLineTextField6.Text;
            var pypath = materialSingleLineTextField5.Text;
            var cpppath = materialSingleLineTextField4.Text;
            if (string.IsNullOrEmpty(apikey) || string.IsNullOrEmpty(modeltype) || string.IsNullOrEmpty(pypath) || string.IsNullOrEmpty(cpppath))
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
            openFileDialog.Filter = "Code Test (*.py;*.cpp;*.pas)|*.py;*.cpp;*.pas";

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
                else if (filePath.EndsWith(".pas"))
                {
                    language = "3";
                }

                await webView21.ExecuteScriptAsync($"changeLanguage({JsonSerializer.Serialize(language)})");
            }
        }
        #endregion

        private void materialRaisedButton10_Click(object sender, EventArgs e)
        {

        }

        private async void materialRaisedButton6_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Image and PDF Files (*.png, *.jpg, *.jpeg, *.pdf)|*.png;*.jpg;*.jpeg;*.pdf|All Files (*.*)|*.*",
                Title = "Chọn file ảnh hoặc PDF"
            };

            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string[] Paths = openFileDialog.FileNames;
                appSettings = Settings.LoadSettings(materialSkinManager, this);
                var AI = appSettings.GeminiAI();
                string _old = materialRaisedButton6.Text;
                materialRaisedButton6.Text = "AI đang phân tích và tạo yêu cầu...";

                string EditorDataRaw = await webView21.ExecuteScriptAsync("WebViewGetCode()");
                string EditorData = JsonSerializer.Deserialize<string>(EditorDataRaw);

                if (string.IsNullOrEmpty(EditorData) || EditorData == "#Bạn Có Thể Dán Code Vào Đây!")
                {
                    MessageBox.Show("Vui Lòng dán code của bạn vào Editor để AI tạo yêu cầu!");
                    materialRaisedButton6.Text = _old;
                    return;
                }

                string YeuCau = await AI.GenerateTextFromImageAndTextAsync(
                    "Bạn là trợ lý AI chuyên phân tích đề bài và code giải được gửi cùng nhau. Nhiệm vụ của bạn là tạo yêu cầu sinh test chính xác, bám sát chặt chẽ thông tin từ đề bài và áp dụng tỷ lệ phần trăm cụ thể cho các trường hợp test. Đề bài có thể chứa nhiều bài, vì vậy hãy thực hiện các bước sau:\r\n\r\n" +
                    "1. **Xác định bài phù hợp:**\r\n" +
                    "   - Đọc kỹ đề bài và code trong EditorData.\r\n" +
                    "   - Tìm bài tương ứng với code dựa trên các dấu hiệu như tên hàm, biến đầu vào, hoặc đặc điểm nổi bật trong code. Nếu không rõ ràng, chọn bài hợp lý nhất nhưng phải dựa trên thông tin từ đề bài.\r\n\r\n" +
                    "2. **Phân tích thông tin từ đề bài:**\r\n" +
                    "   - Trích xuất thông tin từ đề bài về biến đầu vào (tên, kiểu dữ liệu), đầu ra, và các ràng buộc (constraints) được nêu rõ.\r\n" +
                    "   - Ưu tiên sử dụng các giới hạn, ví dụ input/output, và yêu cầu cụ thể trong đề bài. Chỉ suy ra từ code nếu đề bài không cung cấp đủ thông tin.\r\n" +
                    "   - Nếu đề bài có ví dụ input/output, dùng chúng làm cơ sở chính để hiểu yêu cầu bài toán.\r\n\r\n" +
                    "3. **Tạo yêu cầu sinh test:**\r\n" +
                    "   - Dựa hoàn toàn trên thông tin từ đề bài để xây dựng yêu cầu sinh test cho bài đã chọn.\r\n" +
                    "   - Nếu đề bài nêu rõ ràng các giới hạn (constraints), sử dụng chúng để định hình các loại test và phân bổ tỷ lệ phần trăm phù hợp.\r\n" +
                    "   - Nếu đề bài không nêu rõ ràng, áp dụng mẫu chia test mặc định dưới đây với tỷ lệ phần trăm cụ thể (điều chỉnh theo ngữ cảnh đề bài nếu cần):\r\n" +
                    "       • 20% test: Dữ liệu nhỏ, đơn giản (ví dụ: m, n ≤ 10).\r\n" +
                    "       • 30% test: Dữ liệu trung bình (ví dụ: m, n ≤ 100).\r\n" +
                    "       • 50% test: Dữ liệu lớn (ví dụ: m, n > 100 hoặc m × n ≤ 10^6).\r\n" +
                    "   - Nếu đề bài yêu cầu đặc biệt (ví dụ: test trường hợp biên, giá trị âm), thay đổi tỷ lệ và thêm các loại test đó dựa trên đề.\r\n\r\n" +
                    "4. **Đầu ra:**\r\n" +
                    "   Trả về thông tin theo định dạng sau (nếu không xác định được tên bài, dùng số thứ tự hoặc mô tả ngắn từ đề bài):\r\n" +
                    "--------------------------------------------------\r\n" +
                    "### Bài: [Tên bài hoặc mô tả ngắn từ đề bài]\r\n" +
                    "- Input: [Tên và kiểu biến đầu vào từ đề bài]\r\n" +
                    "- Yêu cầu sinh test:\r\n" +
                    "    • [Tỷ lệ %] test: [Điều kiện dữ liệu nhỏ dựa trên đề bài]\r\n" +
                    "    • [Tỷ lệ %] test: [Điều kiện dữ liệu trung bình dựa trên đề bài]\r\n" +
                    "    • [Tỷ lệ %] test: [Điều kiện dữ liệu lớn dựa trên đề bài]\r\n" +
                    "    • [Tỷ lệ %] test: [Các điều kiện bổ sung nếu đề bài yêu cầu]\r\n" +
                    "--------------------------------------------------\r\n\r\n" +
                    "Hãy bám sát đề bài, áp dụng tỷ lệ phần trăm rõ ràng cho từng loại test (mặc định 50:30:20 nếu không có thông tin cụ thể), chỉ sử dụng code để hỗ trợ xác định bài và bổ sung thông tin khi đề bài thiếu. Code giải: " + EditorData,
                    Paths
                );
                fastColoredTextBox1.Text = YeuCau;
                materialRaisedButton6.Text = _old;
            }
        }
        private string _dfg = "Bạn Thêm Yêu Cầu Sinh Test Vào Đây\r\nHoặc Paste Đề Bài Vào\r\nVí dụ:\r\n• Có 25% số test có m, n ≤ 10;\r\n• Có 25% số test khác có m, n ≤ 50;\r\n• Có 25% số test khác có m, n ≤ 300;\r\n• Có 25% số test còn lại có m × n ≤ 10^6\r\n";
        private void fastColoredTextBox1_Load(object sender, EventArgs e)
        {
            if (fastColoredTextBox1.Text == _dfg)
            {
                fastColoredTextBox1.Text = "";
            }
        }

        private void fastColoredTextBox1_MouseLeave(object sender, EventArgs e)
        {
            if (fastColoredTextBox1.Text == "")
            {
                fastColoredTextBox1.Text = _dfg;
            }
        }
        private void fastColoredTextBox1_Load_1(object sender, EventArgs e)
        {
            fastColoredTextBox1.Focus();
        }

        private void fastColoredTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            string urlPattern = @"https?:\/\/[^\s]+";
            string numberPattern = @"\d+";

            var range = fastColoredTextBox1.Range;
            range.ClearStyle(hyperlinkStyle, numberStyle);
            range.SetStyle(hyperlinkStyle, urlPattern);
            range.SetStyle(numberStyle, numberPattern);
        }


        private async void materialRaisedButton4_Click(object sender, EventArgs e)
        {
            appSettings = Settings.LoadSettings(materialSkinManager, this);
            var AI = appSettings.GeminiAI();
            string EditorDataRaw = await webView21.ExecuteScriptAsync("WebViewGetCode()");
            string EditorData = JsonSerializer.Deserialize<string>(EditorDataRaw);
            string YeuCau = fastColoredTextBox1.Text;
            string SinhTestModule = File.ReadAllText(Path.Combine(Hotro.StuffFolder, "SinhTest.py"));
            string sotesst = "100";
            string pythonCode = "";
     
            if (YeuCau.Contains("NoAIFlag"))
            {
                pythonCode = YeuCau.Substring(YeuCau.IndexOf("NoAIFlag") + "NoAIFlag".Length).Trim();
                if (string.IsNullOrEmpty(pythonCode))
                {
                    MessageBox.Show("Vui lòng cung cấp code sau NoAIFlag!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            }
            else
            {
                if ((long)crownNumeric1.Value <= 0)
                {
                    MessageBox.Show("Vui Lòng Cung Cấp Số Lượng Test!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    sotesst = ((long)crownNumeric1.Value).ToString();
                }

                if (string.IsNullOrEmpty(YeuCau) || YeuCau == _dfg || string.IsNullOrEmpty(EditorData) || EditorData == "#Bạn Có Thể Dán Code Vào Đây!")
                {
                    MessageBox.Show("Vui Lòng Cung Cấp Đầy Đủ Code Hoặc Yêu Cầu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string prompt = Hotro.promptGenTesCaseCode(sotesst,EditorData,YeuCau,SinhTestModule);

                string code = await AI.GenerateTextFromTextAsync(prompt);

                int startIndex = code.IndexOf("<MakeByAIFlag>") + "<MakeByAIFlag>".Length;
                int endIndex = code.IndexOf("</MakeByAIFlag>");
                if (startIndex == -1 || endIndex == -1 || endIndex <= startIndex)
                {
                    MessageBox.Show("Không tìm thấy code Python hợp lệ trong thẻ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                pythonCode = code.Substring(startIndex, endIndex - startIndex).Trim();
            }

            var Result2  = MessageBox.Show("Code Đã Tạo Xong Bạn Có Muốn Sửa Hay Xem Lại Không?", "Hỏi", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (Result2 == DialogResult.OK)
            {
                using (var formCode = new FormCode(pythonCode, this))
                {
                    if (formCode.ShowDialog() == DialogResult.OK)
                    {
                         pythonCode = formCode.Code;
                    }
                }
            }
            string TestCasesPath = Path.Combine(Hotro.StuffFolder, "testcases.xml");
            if (File.Exists(TestCasesPath)){
                File.Delete(TestCasesPath);
            }
            int Pyec = await TestGen.RunPython(pythonCode, appSettings.PythonCompilerPath);
            if (Pyec == 0)
            {
                string XMLData = File.ReadAllText(TestCasesPath);
                var Result3 = MessageBox.Show("Input Testcase đã tạo xong, Bạn có muốn xem lại hay tùy chỉnh gì không?", "Hỏi", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (Result3 == DialogResult.OK)
                {
                    using (var TestcaseViewer = new TestcaseViewer(XMLData, this))
                    {
                        if (TestcaseViewer.ShowDialog() == DialogResult.OK)
                        {
                            XMLData = TestcaseViewer.Xml;
                        }
                    }
                    File.WriteAllText(TestCasesPath, XMLData, new UTF8Encoding(false));
                }

                var (compPath, compOption) = await TestGen.GetEditorLanguageAsync(webView21, appSettings);

                var (TestMaker,XmlDataNew) = await TestGen.RunTestMaker(TestCasesPath, EditorData,compPath,compOption);
                if (TestMaker == 0) {
                    MessageBox.Show("Tạo TestCase Hoàn Tất, Bạn có thể lưu lại testcase", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    File.WriteAllText(TestCasesPath, XmlDataNew, new UTF8Encoding(false));
                    return;
                }
                else
                {
                    MessageBox.Show("Code Bị Lỗi, Vui Lòng Tạo Lại Hoặc Xem Lại Code Của Bạn", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {

                var Result = MessageBox.Show("Code Bị Lỗi, Bạn Có Muốn Tự Sửa Lại Để Tiếp Tục Tạo Test?", "Lỗi", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                if (Result == DialogResult.OK)
                {
                    using (var formCode = new FormCode(pythonCode, this))
                    {
                        if (formCode.ShowDialog() == DialogResult.OK)
                        {
                            string userCode = formCode.Code;
                            await TestGen.RunPython(userCode, appSettings.PythonCompilerPath);
                            return;
                        }
                    }
                }
            }

        }


        private void materialRaisedButton5_Click(object sender, EventArgs e)
        {
            appSettings = Settings.LoadSettings(materialSkinManager, this);
            new TestMaker(this).ShowDialog();
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void materialRaisedButton7_Click(object sender, EventArgs e)
        {

        }
    }
}