using System;
using System.IO;
using System.Text.Json;
using MaterialSkin;
using MaterialSkin.Controls;
using ReaLTaiizor.Controls;
using ReaLTaiizor.Child.Crown;
using System.Windows.Forms;
using System.Drawing;

namespace CodeTestGenV1
{
    public class SettingsData
    {
        public string ApiKey { get; set; }
        public string ModelType { get; set; }
        public string Mode { get; set; }
        public bool UseAppCompiler { get; set; }
        public string CppCompilerOptions { get; set; }
        public string PascalCompilerOptions { get; set; }
        public string PythonCompilerPath { get; set; }
        public string CppCompilerPath { get; set; }
        public string PascalCompilerPath { get; set; }
    }

    public class Settings
    {
        public string ApiKey { get; set; }
        public string ModelType { get; set; }
        public string Mode { get; set; }
        public bool UseAppCompiler { get; set; }
        public string CppCompilerOptions { get; set; }
        public string PascalCompilerOptions { get; set; }
        public string BasePath { get; set; }
        public string PythonCompilerPath { get; set; }
        public string CppCompilerPath { get; set; }
        public string PascalCompilerPath { get; set; }
        private FormMain form;
        private static readonly string SettingsFilePath = Path.Combine(Hotro.AppPath, "settings.json");
        private readonly MaterialSkinManager skinManager;

        public GeminiClient GeminiAI()
        {
            var GeminiClient = new GeminiClient(ApiKey, ModelType);
            return GeminiClient;
        }

        public Settings(MaterialSkinManager skinManager,FormMain formMain)
        {
            form = formMain;
            this.skinManager = skinManager;
            ApiKey = "AIzaSyDar-WvC-WReSGkb6AAPCm7q-KW9b3LdT4";
            ModelType = "models/gemini-2.0-flash";
            Mode = "Dark"; 
            UseAppCompiler = true;
            CppCompilerOptions = "";
            BasePath = Hotro.AppPath;
            string bienDichPath = Path.Combine(BasePath, "BienDich");
            PythonCompilerPath = Path.Combine(bienDichPath, "python", "python.exe");
            CppCompilerPath = Path.Combine(bienDichPath, "mingw64", "bin", "g++.exe");
            PascalCompilerPath = Path.Combine(bienDichPath, "FPC", "bin", "i386-win32", "fpc.exe");
            if (!Directory.Exists(bienDichPath))
            {
                UseAppCompiler = false;
                PythonCompilerPath = "python.exe";
                CppCompilerPath = "g++.exe";
                PascalCompilerPath = "fpc.exe";
            }
        }

        public static SettingsData LoadSettingsData()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    string jsonString = File.ReadAllText(SettingsFilePath);
                    return JsonSerializer.Deserialize<SettingsData>(jsonString) ?? new SettingsData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading settings: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return new SettingsData { Mode = "Light", UseAppCompiler = true, ApiKey = "AIzaSyDar-WvC-WReSGkb6AAPCm7q-KW9b3LdT4" };
        }

        public static Settings LoadSettings(MaterialSkinManager skinManager,FormMain formM)
        {
            string BasePath = Hotro.AppPath;
            string bienDichPath = Path.Combine(BasePath, "BienDich");
            var data = LoadSettingsData();
            if (data.UseAppCompiler && Directory.Exists(bienDichPath))
            {
                data.PythonCompilerPath = Path.Combine(bienDichPath, "python", "python.exe");
                data.CppCompilerPath = Path.Combine(bienDichPath, "mingw64", "bin", "g++.exe");
                data.PascalCompilerPath = Path.Combine(bienDichPath, "FPC", "bin", "i386-win32", "fpc.exe");
            }
            var settings = new Settings(skinManager,formM)
            {
                ApiKey = data.ApiKey,
                ModelType = data.ModelType,
                Mode = data.Mode,
                UseAppCompiler = data.UseAppCompiler,
                CppCompilerOptions = data.CppCompilerOptions,
                PascalCompilerOptions = data.PascalCompilerOptions,
                PythonCompilerPath = data.PythonCompilerPath,
                CppCompilerPath = data.CppCompilerPath,
                PascalCompilerPath = data.PascalCompilerPath,
                BasePath = Hotro.AppPath
            };
            
            settings.colorSet();
            return settings;
        }

        public void SaveSettings()
        {
            try
            {
                var data = new SettingsData
                {
                    ApiKey = this.ApiKey,
                    ModelType = this.ModelType,
                    Mode = this.Mode,
                    UseAppCompiler = this.UseAppCompiler,
                    CppCompilerOptions = this.CppCompilerOptions,
                    PascalCompilerOptions = this.PascalCompilerOptions,
                    PythonCompilerPath= this.PythonCompilerPath,
                    CppCompilerPath = this.CppCompilerPath,
                    PascalCompilerPath = this.PascalCompilerPath,
                };
                string jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsFilePath, jsonString);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void UpdateFromForm()
        {
            ApiKey = form.materialSingleLineTextField1.Text;
            ModelType = form.materialSingleLineTextField6.Text;
            Mode = form.dropDownControl1.SelectedItem != null ? form.dropDownControl1.SelectedItem.Text : "Light";
            UseAppCompiler = form.materialCheckBox1.Checked;
            CppCompilerOptions = form.materialSingleLineTextField3.Text;
            PascalCompilerOptions = form.materialSingleLineTextField7.Text;
            PythonCompilerPath = form.materialSingleLineTextField5.Text;
            CppCompilerPath = form.materialSingleLineTextField4.Text;
            PascalCompilerPath = form.materialSingleLineTextField8.Text;
            BasePath = Hotro.AppPath;
            colorSet();
            WebViewApply();
        }

        public void ApplyToForm()
        {
            form.materialSingleLineTextField1.Text = ApiKey;
            form.materialSingleLineTextField6.Text = ModelType;
            foreach (CrownDropDownItem item in form.dropDownControl1.Items)
            {
                if (item.Text == Mode)
                {
                    form.dropDownControl1.SelectedItem = item;
                    break;
                }
            }
            form.materialCheckBox1.Checked = UseAppCompiler;
            form.materialSingleLineTextField3.Text = CppCompilerOptions;
            form.materialSingleLineTextField7.Text = PascalCompilerOptions;
            form.materialSingleLineTextField5.Text = PythonCompilerPath;
            form.materialSingleLineTextField4.Text = CppCompilerPath;
            form.materialSingleLineTextField8.Text = PascalCompilerPath;
            colorSet();
        }

        public void RefreshSettings()  
        {
            var data = LoadSettingsData();
            ApiKey = data.ApiKey;
            ModelType = data.ModelType;
            Mode = data.Mode;
            UseAppCompiler = data.UseAppCompiler;
            CppCompilerOptions = data.CppCompilerOptions;
            PascalCompilerOptions = data.PascalCompilerOptions;
            PythonCompilerPath = data.PythonCompilerPath;
            CppCompilerPath = data.CppCompilerPath;
            PascalCompilerPath = data.PascalCompilerPath;
            BasePath = Hotro.AppPath; 
            ApplyToForm();
        }
        public void colorSet()
        {
            if (Mode == "Dark")
            {
                skinManager.Theme = MaterialSkinManager.Themes.DARK;
                skinManager.ColorScheme = new ColorScheme(
                    Primary.BlueGrey800, Primary.BlueGrey900,
                    Primary.BlueGrey500, Accent.LightBlue200,
                    TextShade.WHITE
                );
                form.ForeColor = Color.White;
                form.tabPage1.BackColor = Color.FromArgb(29, 35, 44);
                form.tabPage2.BackColor = Color.FromArgb(29, 35, 44);
                form.hopeTabPage1.BaseColor = Color.FromArgb(44, 55, 66);
                form.lostBorderPanel1.BorderColor = Color.FromArgb(67, 84, 102);
                form.lostBorderPanel2.BorderColor = Color.FromArgb(67, 84, 102);
                form.crownDockPanel1.BackColor = Color.FromArgb(67, 84, 102);
                form.lostBorderPanel1.BackColor = Color.FromArgb(29, 35, 44);
                form.lostBorderPanel2.BackColor = Color.FromArgb(29, 35, 44);
                form.materialLabel3.ForeColor = Color.White;
                form.materialLabel6.ForeColor = Color.White;
                form.fastColoredTextBox1.BackColor = Color.FromArgb(54, 65, 82);
                form.fastColoredTextBox1.ForeColor = Color.White;
                form.lostBorderPanel4.BorderColor = Color.FromArgb(67, 84, 102);
                form.lostBorderPanel4.BackColor = Color.FromArgb(67, 84, 102);
            }
            else // Light mode
            {
                skinManager.Theme = MaterialSkinManager.Themes.LIGHT;
                skinManager.ColorScheme = new ColorScheme(
                    Primary.Indigo500, Primary.Indigo700,
                    Primary.Indigo100, Accent.Pink200,
                    TextShade.WHITE
                );
                form.ForeColor = Color.Black;
                form.BackColor = Color.WhiteSmoke;
                form.tabPage1.BackColor = Color.WhiteSmoke;
                form.tabPage2.BackColor = Color.WhiteSmoke;
                form.hopeTabPage1.BaseColor = Color.FromArgb(48, 63, 159);
                form.lostBorderPanel1.BorderColor = Color.FromArgb(48, 63, 159);
                form.lostBorderPanel2.BorderColor = Color.FromArgb(48, 63, 159);
                form.crownDockPanel1.BackColor = Color.FromArgb(67, 84, 102);
                form.lostBorderPanel1.BackColor = Color.WhiteSmoke;
                form.lostBorderPanel2.BackColor = Color.WhiteSmoke;
                form.materialLabel3.ForeColor = Color.Black;
                form.materialLabel6.ForeColor = Color.Black;
                form.fastColoredTextBox1.BackColor = Color.WhiteSmoke;
                form.fastColoredTextBox1.ForeColor = Color.Black;
                form.lostBorderPanel4.BorderColor = Color.FromArgb(48, 63, 159);
                form.lostBorderPanel4.BackColor = Color.FromArgb(48, 63, 159);
            }
        }
        public async void WebViewApply()
        {
            if (Mode == "Dark")
            {
                await form.webView21.CoreWebView2.ExecuteScriptAsync($"toggleDarkMode(true);");
                await form.webView21.CoreWebView2.ExecuteScriptAsync($"toggleDarkMode(true);");
                await form.VideoPlayer.CoreWebView2.ExecuteScriptAsync($"toggleDarkMode(true);");
            }
            else // Light mode
            {
                await form.webView21.CoreWebView2.ExecuteScriptAsync($"toggleDarkMode(false);");
                await form.VideoPlayer.CoreWebView2.ExecuteScriptAsync($"toggleDarkMode(false);");
            }
            
        }
    }
}