using System;
using System.IO;
using System.Text.Json;
using MaterialSkin;
using ReaLTaiizor.Child.Crown;

namespace CodeTestGenV1
{
    // Lớp để lưu trữ dữ liệu JSON (không có BasePath)
    public class SettingsData
    {
        public string ApiKey { get; set; }
        public string Mode { get; set; }
        public bool UseAppCompiler { get; set; }
        public string PythonCompilerOptions { get; set; }
        public string CppCompilerOptions { get; set; }
    }

    public class Settings
    {
        // Thuộc tính cài đặt
        public string ApiKey { get; set; }
        public string Mode { get; set; }
        public bool UseAppCompiler { get; set; }
        public string PythonCompilerOptions { get; set; }
        public string CppCompilerOptions { get; set; }
        public string BasePath { get; set; } // Vẫn giữ trong lớp nhưng không lưu vào JSON

        private static readonly string SettingsFilePath = Path.Combine(Hotro.AppPath, "settings.json");
        private readonly MaterialSkinManager skinManager;

        // Constructor mặc định với giá trị khởi tạo
        public Settings(MaterialSkinManager skinManager)
        {
            this.skinManager = skinManager;
            ApiKey = "";
            Mode = "Light"; // Mặc định là Light
            UseAppCompiler = true; // Mặc định bật
            PythonCompilerOptions = "";
            CppCompilerOptions = "";
            BasePath = Hotro.AppPath;

            // Kiểm tra thư mục BasePath/BienDich
            string bienDichPath = Path.Combine(BasePath, "BienDich");
            if (!Directory.Exists(bienDichPath))
            {
                UseAppCompiler = false; // Tắt nếu thư mục không tồn tại
            }
        }

        // Tải cài đặt từ file và trả về SettingsData
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
                Console.WriteLine($"Error loading settings: {ex.Message}");
            }
            return new SettingsData { Mode = "Light", UseAppCompiler = true };
        }

        // Tải cài đặt vào lớp Settings
        public static Settings LoadSettings(MaterialSkinManager skinManager)
        {
            var data = LoadSettingsData();
            var settings = new Settings(skinManager)
            {
                ApiKey = data.ApiKey,
                Mode = data.Mode,
                UseAppCompiler = data.UseAppCompiler,
                PythonCompilerOptions = data.PythonCompilerOptions,
                CppCompilerOptions = data.CppCompilerOptions,
                BasePath = Hotro.AppPath // BasePath lấy từ Hotro.AppPath, không từ JSON
            };
            settings.ApplyTheme();
            return settings;
        }

        // Lưu cài đặt vào file (không lưu BasePath)
        public void SaveSettings()
        {
            try
            {
                var data = new SettingsData
                {
                    ApiKey = this.ApiKey,
                    Mode = this.Mode,
                    UseAppCompiler = this.UseAppCompiler,
                    PythonCompilerOptions = this.PythonCompilerOptions,
                    CppCompilerOptions = this.CppCompilerOptions
                };
                string jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsFilePath, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving settings: {ex.Message}");
            }
        }

        // Cập nhật cài đặt từ Form
        public void UpdateFromForm(FormMain form)
        {
            ApiKey = form.materialSingleLineTextField1.Text;
            Mode = form.dropDownControl1.SelectedItem != null ? form.dropDownControl1.SelectedItem.Text : "Light";
            UseAppCompiler = form.materialCheckBox1.Checked;
            PythonCompilerOptions = form.materialSingleLineTextField2.Text;
            CppCompilerOptions = form.materialSingleLineTextField3.Text;
            BasePath = Hotro.AppPath; // BasePath giữ nguyên từ Hotro.AppPath
            ApplyTheme();
        }

        // Áp dụng cài đặt vào Form
        public void ApplyToForm(FormMain form)
        {
            form.materialSingleLineTextField1.Text = ApiKey;
            foreach (CrownDropDownItem item in form.dropDownControl1.Items)
            {
                if (item.Text == Mode)
                {
                    form.dropDownControl1.SelectedItem = item;
                    break;
                }
            }
            form.materialCheckBox1.Checked = UseAppCompiler;
            form.materialSingleLineTextField2.Text = PythonCompilerOptions;
            form.materialSingleLineTextField3.Text = CppCompilerOptions;
            ApplyTheme();
        }

        // Tải lại dữ liệu từ JSON vào Form
        public void RefreshSettings(FormMain form)
        {
            var data = LoadSettingsData();
            ApiKey = data.ApiKey;
            Mode = data.Mode;
            UseAppCompiler = data.UseAppCompiler;
            PythonCompilerOptions = data.PythonCompilerOptions;
            CppCompilerOptions = data.CppCompilerOptions;
            BasePath = Hotro.AppPath; // BasePath lấy từ Hotro.AppPath
            ApplyToForm(form);
        }

        // Áp dụng theme MaterialSkin dựa trên Mode
        private void ApplyTheme()
        {
            if (Mode == "Dark")
            {
                skinManager.Theme = MaterialSkinManager.Themes.DARK;
                skinManager.ColorScheme = new ColorScheme(
                    Primary.BlueGrey800, Primary.BlueGrey900,
                    Primary.BlueGrey500, Accent.LightBlue200,
                    TextShade.WHITE
                );
            }
            else // Light mode
            {
                skinManager.Theme = MaterialSkinManager.Themes.LIGHT;
                skinManager.ColorScheme = new ColorScheme(
                    Primary.Indigo500, Primary.Indigo700,
                    Primary.Indigo100, Accent.Pink200,
                    TextShade.BLACK
                );
            }
        }
    }
}