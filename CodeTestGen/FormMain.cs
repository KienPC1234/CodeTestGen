using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeTestGen;
using FastColoredTextBoxNS;
using MaterialSkin;
using MaterialSkin.Controls;
using ReaLTaiizor.Child.Crown;
using ReaLTaiizor.Controls;
using static CodeTestGen.TescaseSaver;

namespace CodeTestGenV1
{
    public partial class FormMain : MaterialForm
    {
        #region Fields
        private readonly TextStyle _hyperlinkStyle = new TextStyle(Brushes.Blue, null, FontStyle.Underline);
        private readonly TextStyle _numberStyle = new TextStyle(Brushes.Green, null, FontStyle.Bold);
        private readonly MaterialSkinManager _materialSkinManager;
        private readonly string _testCasesPath = Path.Combine(Hotro.StuffFolder, "testcases.xml");
        private readonly string _defaultFastTextBoxContent =
            "Bạn Thêm Yêu Cầu Sinh Test Vào Đây\r\nHoặc Paste Đề Bài Vào\r\nVí dụ:\r\n" +
            "• Có 25% số test có m, n ≤ 10;\r\n" +
            "• Có 25% số test khác có m, n ≤ 50;\r\n" +
            "• Có 25% số test khác có m, n ≤ 300;\r\n" +
            "• Có 25% số test còn lại có m × n ≤ 10^6\r\n";

        public Settings appSettings;
        private CancellationTokenSource _zoneWatcherCts;
        private Task _zoneWatcherTask;
        #endregion

        #region Constructor
        public FormMain()
        {
            _materialSkinManager = MaterialSkinManager.Instance;
            InitializeComponent();
            InitializeMaterialSkin();
            InitializeSettings();
            InitializeDropDowns();
            ConfigureFormProperties();
            Task.Factory.StartNew(StartZoneWatcher, TaskCreationOptions.LongRunning);
        }
        #endregion

        #region Initialization Methods
        private void InitializeMaterialSkin()
        {
            
            _materialSkinManager.AddFormToManage(this);
        }

        private void InitializeSettings()
        {
            string settingsPath = Path.Combine(Hotro.AppPath, "settings.json");
            appSettings = File.Exists(settingsPath)
                ? Settings.LoadSettings(_materialSkinManager, this)
                : new Settings(_materialSkinManager, this);

            appSettings.ApplyToForm();
            appSettings.SaveSettings();
        }

        private void InitializeDropDowns()
        {
            dropDownControl1.Items.Add(new CrownDropDownItem { Text = "Dark" });
            dropDownControl1.Items.Add(new CrownDropDownItem { Text = "Light" });
            dropDownControl1.SelectedItem = appSettings.Mode == "Dark"
                ? dropDownControl1.Items[0]
                : dropDownControl1.Items[1];

            foreach (TestCaseFormat format in Enum.GetValues(typeof(TestCaseFormat)))
            {
                var fieldInfo = format.GetType().GetField(format.ToString());
                var description = fieldInfo.GetCustomAttribute<DescriptionAttribute>()?.Description ?? format.ToString();
                crownDropDownList1.Items.Add(new CrownDropDownItem { Text = description });
            }
        }

        private void ConfigureFormProperties()
        {
            materialFlatButton3.Visible = false;
            MinimumSize = new Size(1000, 600);
            InitializeWebViewAsync();
        }

        private void InitializeWebViewAsync()
        {
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    await webView21.EnsureCoreWebView2Async(null);
                    await VideoPlayer.EnsureCoreWebView2Async(null);

                    if (!LoadWebViewContent(VideoPlayer, "VideoPlayer.html") ||
                        !LoadWebViewContent(webView21, "editor.html"))
                    {
                        return;
                    }

                    webView21.CoreWebView2.NavigationCompleted += (sender, e) => appSettings.WebViewApply();
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)(() =>
                        MessageBox.Show($"Khởi tạo WebView thất bại: {ex.Message}", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)));
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private bool LoadWebViewContent(Microsoft.Web.WebView2.WinForms.WebView2 webView, string htmlFileName)
        {
            string htmlPath = Path.Combine(Hotro.AppPath, htmlFileName);
            if (!File.Exists(htmlPath))
            {
                this.Invoke((MethodInvoker)(() =>
                    MessageBox.Show($"Không tìm thấy file {htmlFileName}!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error)));
                return false;
            }
            webView.Source = new Uri($"file:///{htmlPath.Replace("\\", "/")}");
            return true;
        }
        #endregion

        #region Event Handlers
        private void materialFlatButton1_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog();
        }

        private void hopeTabPage1_SelectedIndexChanged(object sender, EventArgs e)
        {
            materialFlatButton3.Visible = hopeTabPage1.SelectedTab == tabPage2;
        }

        private void materialCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            string bienDichPath = Path.Combine(Hotro.AppPath, "BienDich");
            if (materialCheckBox1.Checked)
            {
                if (!Directory.Exists(bienDichPath))
                {
                    MessageBox.Show("Không tìm thấy thư mục biên dịch!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    materialCheckBox1.Checked = false;
                    return;
                }
                materialSingleLineTextField5.Text = Path.Combine(bienDichPath, "python", "python.exe");
                materialSingleLineTextField4.Text = Path.Combine(bienDichPath, "mingw64", "bin", "g++.exe");
                materialSingleLineTextField8.Text = Path.Combine(bienDichPath, "FPC", "bin", "i386-win32", "fpc.exe");
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
            if (!ValidateSettingsInput())
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin cài đặt!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            appSettings.UpdateFromForm();
            appSettings.SaveSettings();
            MessageBox.Show("Cài đặt đã được lưu!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool ValidateSettingsInput()
        {
            return !string.IsNullOrEmpty(materialSingleLineTextField1.Text) &&
                   !string.IsNullOrEmpty(materialSingleLineTextField6.Text) &&
                   !string.IsNullOrEmpty(materialSingleLineTextField5.Text) &&
                   !string.IsNullOrEmpty(materialSingleLineTextField4.Text);
        }

        private void materialFlatButton3_Click(object sender, EventArgs e)
        {
            appSettings.RefreshSettings();
            MessageBox.Show("Đã tải lại cài đặt từ file!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void materialRaisedButton9_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    string clipboardText = Clipboard.GetText();
                    await webView21.CoreWebView2.ExecuteScriptAsync(
                        $"setText({JsonSerializer.Serialize(clipboardText)})");
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)(() =>
                        MessageBox.Show($"Lỗi khi dán code từ clipboard: {ex.Message}", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)));
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Filter = "Code Test (*.py;*.cpp;*.pas)|*.py;*.cpp;*.pas"
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            Task.Factory.StartNew(async () =>
            {
                try
                {
                    string filePath = openFileDialog.FileName;
                    string fileContent = File.ReadAllText(filePath);
                    await webView21.ExecuteScriptAsync($"setText({JsonSerializer.Serialize(fileContent)})");
                    await webView21.ExecuteScriptAsync($"changeLanguage({JsonSerializer.Serialize(GetLanguageCode(filePath))})");
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)(() =>
                        MessageBox.Show($"Lỗi khi mở file: {ex.Message}", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)));
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private string GetLanguageCode(string filePath)
        {
            return Path.GetExtension(filePath).ToLower() switch
            {
                ".py" => "1",
                ".cpp" => "2",
                ".pas" => "3",
                _ => string.Empty
            };
        }

        private void materialRaisedButton10_Click(object sender, EventArgs e)
        {
            try
            {
                string testCasesPath = Path.Combine(Hotro.StuffFolder, "testcases.xml");
                if (File.Exists(testCasesPath))
                {
                    File.Delete(testCasesPath);
                    MessageBox.Show("Đã xóa test cases!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa test cases: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void materialRaisedButton6_Click(object sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Image and PDF Files (*.png, *.jpg, *.jpeg, *.pdf)|*.png;*.jpg;*.jpeg;*.pdf|All Files (*.*)|*.*",
                Title = "Chọn file ảnh hoặc PDF"
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            Task.Factory.StartNew(async () =>
            {
                try
                {
                    appSettings = Settings.LoadSettings(_materialSkinManager, this);
                    var ai = appSettings.GeminiAI();
                    string originalButtonText = materialRaisedButton6.Text;
                    this.Invoke((MethodInvoker)(() => materialRaisedButton6.Text = "AI đang phân tích và tạo yêu cầu..."));

                    string editorData = await GetEditorCodeAsync();
                    if (string.IsNullOrEmpty(editorData) || editorData == "#Bạn Có Thể Dán Code Vào Đây!")
                    {
                        this.Invoke((MethodInvoker)(() =>
                            MessageBox.Show("Vui lòng dán code vào Editor!", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)));
                        return;
                    }

                    string requirement = await ai.GenerateTextFromImageAndTextAsync(
                        GenerateTestRequirementPrompt(editorData),
                        openFileDialog.FileNames);

                    this.Invoke((MethodInvoker)(() => fastColoredTextBox1.Text = requirement));
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)(() =>
                        MessageBox.Show($"Lỗi khi tạo yêu cầu: {ex.Message}", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)));
                }
                finally
                {
                    this.Invoke((MethodInvoker)(() => materialRaisedButton6.Text = "Dùng PDF, Ảnh Của Đề Bài Để Ai Tự Tạo Yêu Cầu (Có Thể Chọn Nhiều File)"));
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task<string> GetEditorCodeAsync()
        {
            string rawData = await webView21.ExecuteScriptAsync("WebViewGetCode()");
            return JsonSerializer.Deserialize<string>(rawData);
        }

        private string GenerateTestRequirementPrompt(string editorData)
        {
            return "Bạn là trợ lý AI chuyên phân tích đề bài và code giải. Nhiệm vụ của bạn là tạo yêu cầu sinh test chính xác, bám sát thông tin từ đề bài và áp dụng tỷ lệ phần trăm cụ thể cho các trường hợp test.\r\n" +
                   "1. **Xác định bài phù hợp:** Đọc kỹ đề bài và code trong EditorData. Tìm bài tương ứng với code dựa trên tên hàm, biến đầu vào, hoặc đặc điểm nổi bật trong code.\r\n" +
                   "2. **Phân tích thông tin từ đề bài:** Trích xuất thông tin về biến đầu vào (tên, kiểu dữ liệu), đầu ra, và các ràng buộc. Ưu tiên sử dụng giới hạn từ đề bài.\r\n" +
                   "3. **Tạo yêu cầu sinh test:** Dựa hoàn toàn trên đề bài để xây dựng yêu cầu sinh test. Nếu không có thông tin cụ thể, áp dụng mẫu chia test mặc định: 20% test nhỏ (m, n ≤ 10), 30% test trung bình (m, n ≤ 100), 50% test lớn (m, n > 100 hoặc m × n ≤ 10^6).\r\n" +
                   "4. **Đầu ra:** Trả về theo định dạng:\r\n" +
                   "--------------------------------------------------\r\n" +
                   "### Bài: [Tên bài hoặc mô tả ngắn]\r\n" +
                   "- Input: [Tên và kiểu biến đầu vào]\r\n" +
                   "- Yêu cầu sinh test:\r\n" +
                   "    • [Tỷ lệ %] test: [Điều kiện dữ liệu nhỏ]\r\n" +
                   "    • [Tỷ lệ %] test: [Điều kiện dữ liệu trung bình]\r\n" +
                   "    • [Tỷ lệ %] test: [Điều kiện dữ liệu lớn]\r\n" +
                   "    • [Tỷ lệ %] test: [Điều kiện bổ sung nếu có]\r\n" +
                   "--------------------------------------------------\r\n" +
                   $"Code giải: {editorData}";
        }

        private void fastColoredTextBox1_Load(object sender, EventArgs e)
        {
            if (fastColoredTextBox1.Text == _defaultFastTextBoxContent)
            {
                fastColoredTextBox1.Text = string.Empty;
            }
            fastColoredTextBox1.Focus();
        }

        private void fastColoredTextBox1_MouseLeave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(fastColoredTextBox1.Text))
            {
                fastColoredTextBox1.Text = _defaultFastTextBoxContent;
            }
        }

        private void fastColoredTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            var range = fastColoredTextBox1.Range;
            range.ClearStyle(_hyperlinkStyle, _numberStyle);
            range.SetStyle(_hyperlinkStyle, @"https?:\/\/[^\s]+");
            range.SetStyle(_numberStyle, @"\d+");
        }

        private void materialRaisedButton4_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    appSettings = Settings.LoadSettings(_materialSkinManager, this);
                    string pythonCode = await GenerateTestCaseCodeAsync();
                    if (string.IsNullOrEmpty(pythonCode))
                        return;

                    if (await ReviewAndRunTestCaseCodeAsync(pythonCode))
                    {
                        await ProcessTestCasesAsync();
                    }
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)(() =>
                        MessageBox.Show($"Lỗi khi tạo test case: {ex.Message}", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)));
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task<string> GenerateTestCaseCodeAsync()
        {
            string requirement = fastColoredTextBox1.Text;
            if (requirement.Contains("NoAIFlag"))
            {
                string pythonCode = requirement.Substring(requirement.IndexOf("NoAIFlag") + "NoAIFlag".Length).Trim();
                if (string.IsNullOrEmpty(pythonCode))
                {
                    this.Invoke((MethodInvoker)(() =>
                        MessageBox.Show("Vui lòng cung cấp code sau NoAIFlag!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)));
                    return null;
                }
                return pythonCode;
            }

            if ((long)crownNumeric1.Value <= 0)
            {
                this.Invoke((MethodInvoker)(() =>
                    MessageBox.Show("Vui lòng cung cấp số lượng test!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error)));
                return null;
            }

            string editorData = await GetEditorCodeAsync();
            if (string.IsNullOrEmpty(requirement) || requirement == _defaultFastTextBoxContent ||
                string.IsNullOrEmpty(editorData) || editorData == "#Bạn Có Thể Dán Code Vào Đây!")
            {
                this.Invoke((MethodInvoker)(() =>
                    MessageBox.Show("Vui lòng cung cấp đầy đủ code hoặc yêu cầu!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error)));
                return null;
            }

            string prompt = Hotro.promptGenTesCaseCode(((long)crownNumeric1.Value).ToString(),
                editorData, requirement, File.ReadAllText(Path.Combine(Hotro.StuffFolder, "SinhTest.py")));

            string code = await appSettings.GeminiAI().GenerateTextFromTextAsync(prompt);
            int startIndex = code.IndexOf("<MakeByAIFlag>") + "<MakeByAIFlag>".Length;
            int endIndex = code.IndexOf("</MakeByAIFlag>");

            if (startIndex == -1 || endIndex == -1 || endIndex <= startIndex)
            {
                this.Invoke((MethodInvoker)(() =>
                    MessageBox.Show("Không tìm thấy code Python hợp lệ!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error)));
                return null;
            }

            return code.Substring(startIndex, endIndex - startIndex).Trim();
        }

        private async Task<bool> ReviewAndRunTestCaseCodeAsync(string pythonCode)
        {
            bool shouldContinue = false;
            this.Invoke((MethodInvoker)(() =>
            {
                if (MessageBox.Show("Code đã tạo xong. Bạn có muốn sửa hoặc xem lại?", "Hỏi",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    using var formCode = new FormCode(pythonCode, this);
                    if (formCode.ShowDialog() == DialogResult.OK)
                    {
                        pythonCode = formCode.Code;
                    }
                    shouldContinue = true;
                }
            }));

            if (!shouldContinue)
                return false;

            if (File.Exists(_testCasesPath))
            {
                File.Delete(_testCasesPath);
            }

            int executionResult = await TestGen.RunPython(pythonCode, appSettings.PythonCompilerPath);
            if (executionResult != 0)
            {
                bool retry = false;
                this.Invoke((MethodInvoker)(() =>
                {
                    if (MessageBox.Show("Code bị lỗi. Bạn có muốn sửa lại?", "Lỗi",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK)
                    {
                        using var formCode = new FormCode(pythonCode, this);
                        if (formCode.ShowDialog() == DialogResult.OK)
                        {
                            Task.Factory.StartNew(async () =>
                            {
                                await TestGen.RunPython(formCode.Code, appSettings.PythonCompilerPath);
                            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
                        }
                        retry = true;
                    }
                }));
                return !retry;
            }
            return true;
        }

        private async Task ProcessTestCasesAsync()
        {
            string xmlData = File.ReadAllText(_testCasesPath);
            bool shouldSave = false;
            this.Invoke((MethodInvoker)(() =>
            {
                if (MessageBox.Show("Input của Testcase đã tạo xong. Bạn có muốn xem lại hoặc tùy chỉnh?",
                    "Hỏi", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    using var testcaseViewer = new TestcaseViewer(xmlData, this);
                    if (testcaseViewer.ShowDialog() == DialogResult.OK)
                    {
                        xmlData = testcaseViewer.Xml;
                    }
                    shouldSave = true;
                }
            }));

            if (shouldSave)
            {
                File.WriteAllText(_testCasesPath, xmlData, new UTF8Encoding(false));
            }

            var (compilerPath, compilerOption) = await TestGen.GetEditorLanguageAsync(webView21, appSettings);
            var (testMakerResult, newXmlData) = await TestGen.RunTestMaker(_testCasesPath, await GetEditorCodeAsync(),
                compilerPath, compilerOption);

            this.Invoke((MethodInvoker)(() =>
            {
                if (testMakerResult == 0)
                {
                    File.WriteAllText(_testCasesPath, newXmlData, new UTF8Encoding(false));
                    MessageBox.Show("Tạo TestCase hoàn tất!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Code bị lỗi. Vui lòng kiểm tra lại!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }));
        }

        private void StopZoneWatcher()
        {
            if (_zoneWatcherCts == null)
                return;

            _zoneWatcherCts.Cancel();
            try
            {
                _zoneWatcherTask.Wait();
            }
            catch (AggregateException) { }
            finally
            {
                _zoneWatcherCts.Dispose();
                _zoneWatcherCts = null;
                _zoneWatcherTask = null;
            }
        }

        private void StartZoneWatcher()
        {
            StopZoneWatcher();
            _zoneWatcherCts = new CancellationTokenSource();
            _zoneWatcherTask = Task.Factory.StartNew(async () =>
            {
                try
                {
                    while (!_zoneWatcherCts.Token.IsCancellationRequested)
                    {
                        bool testCasesExist = File.Exists(_testCasesPath);
                        this.Invoke((MethodInvoker)(() =>
                        {
                            materialRaisedButton3.Enabled = testCasesExist;
                            materialRaisedButton10.Enabled = testCasesExist;
                            materialRaisedButton11.Enabled = testCasesExist;
                        }));
                        await Task.Delay(300, _zoneWatcherCts.Token);
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    Console.WriteLine($"ZoneWatcher error: {ex}");
                }
            }, _zoneWatcherCts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private void materialRaisedButton5_Click(object sender, EventArgs e)
        {
            appSettings = Settings.LoadSettings(_materialSkinManager, this);
            new TestMaker(this).ShowDialog();
        }

        private void materialRaisedButton7_Click(object sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Filter = "Executable Files (*.exe)|*.exe",
                Title = "Chọn file thực thi (.exe)"
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            Task.Factory.StartNew(async () =>
            {
                try
                {
                    string exePath = openFileDialog.FileName;
                    string requirement = fastColoredTextBox1.Text;

                    if (!requirement.Contains("NoAIFlag"))
                    {
                        this.Invoke((MethodInvoker)(() =>
                        {
                            MessageBox.Show("Vui lòng cung cấp code sinh test case!", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            appSettings = Settings.LoadSettings(_materialSkinManager, this);
                            new TestMaker(this).ShowDialog();
                            materialRaisedButton7_Click(sender, e);
                        }));
                        return;
                    }

                    string pythonCode = requirement.Substring(requirement.IndexOf("NoAIFlag") + "NoAIFlag".Length).Trim();
                    if (string.IsNullOrEmpty(pythonCode))
                    {
                        this.Invoke((MethodInvoker)(() =>
                            MessageBox.Show("Vui lòng cung cấp code sau NoAIFlag!", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error)));
                        return;
                    }

                    int executionResult = await TestGen.RunPython(pythonCode, appSettings.PythonCompilerPath);
                    if (executionResult != 0)
                    {
                        this.Invoke((MethodInvoker)(() =>
                        {
                            if (MessageBox.Show("Code bị lỗi. Bạn có muốn sửa lại?", "Lỗi",
                                MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK)
                            {
                                using var formCode = new FormCode(pythonCode, this);
                                if (formCode.ShowDialog() == DialogResult.OK)
                                {
                                    Task.Factory.StartNew(async () =>
                                    {
                                        await TestGen.RunPython(formCode.Code, appSettings.PythonCompilerPath);
                                    }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
                                }
                            }
                        }));
                        return;
                    }

                    string xmlData = File.ReadAllText(_testCasesPath);
                    bool shouldSave = false;
                    this.Invoke((MethodInvoker)(() =>
                    {
                        if (MessageBox.Show("Input của Testcase đã tạo xong. Bạn có muốn xem lại hoặc tùy chỉnh?",
                            "Hỏi", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            using var testcaseViewer = new TestcaseViewer(xmlData, this);
                            if (testcaseViewer.ShowDialog() == DialogResult.OK)
                            {
                                xmlData = testcaseViewer.Xml;
                            }
                            shouldSave = true;
                        }
                    }));

                    if (shouldSave)
                    {
                        File.WriteAllText(_testCasesPath, xmlData, new UTF8Encoding(false));
                    }

                    var (testMakerResult, newXmlData) = await TestGen.RunTestMakerExeMode(_testCasesPath, exePath);
                    this.Invoke((MethodInvoker)(() =>
                    {
                        if (testMakerResult == 0)
                        {
                            File.WriteAllText(_testCasesPath, newXmlData, new UTF8Encoding(false));
                            MessageBox.Show("Tạo TestCase hoàn tất!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Code bị lỗi. Vui lòng kiểm tra lại!", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }));
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)(() =>
                        MessageBox.Show($"Lỗi khi xử lý file thực thi: {ex.Message}", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)));
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void materialRaisedButton11_Click(object sender, EventArgs e)
        {
            try
            {
                string testCasesPath = Path.Combine(Hotro.StuffFolder, "testcases.xml");
                if (!File.Exists(testCasesPath))
                {
                    MessageBox.Show("Không tìm thấy file test case!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string tcData = File.ReadAllText(testCasesPath);
                using var testcaseViewer = new TestcaseViewer(tcData, this);
                if (testcaseViewer.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(testCasesPath, testcaseViewer.Xml);
                    MessageBox.Show("Đã chỉnh sửa và lưu test cases!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chỉnh sửa test cases: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void materialRaisedButton8_Click(object sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Filter = "XML File (*.xml)|*.xml",
                Title = "Chọn file XML test cases"
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                File.Copy(openFileDialog.FileName, _testCasesPath, true);
                MessageBox.Show("Đã nhập file test cases!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi nhập file: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void materialRaisedButton3_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedItem = crownDropDownList1.SelectedItem as CrownDropDownItem;
                if (selectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn một định dạng!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                TestCaseFormat format = selectedItem.Text switch
                {
                    "Themis" => TestCaseFormat.Themis,
                    "Online Judge (VNOJ, DMOJ,...)" => TestCaseFormat.OnlineJudge,
                    "Legacy inputX.txt/outputX.txt" => TestCaseFormat.LegacyTxt,
                    "XML (CodeTestGen)" => TestCaseFormat.XmlCodeTestGen,
                    "(Yandex / Polygon)" => TestCaseFormat.DotTest,
                    "JSON (Web/API)" => TestCaseFormat.JsonWebApi,
                    "ZIP (Gộp test)" => TestCaseFormat.ZipGrouped,
                    _ => throw new ArgumentException("Định dạng không được hỗ trợ!")
                };

                var validTestCases = LoadTestCases(_testCasesPath);
                new SaveForm(format, validTestCases, _testCasesPath, this).Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu file: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopZoneWatcher();
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {
            // Empty event handler as per original code
        }
        #endregion
    }
}