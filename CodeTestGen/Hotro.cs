using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Web.WebView2.Core;
using GenerativeAI;
using GenerativeAI.Types;
using System.Collections.Generic;


namespace CodeTestGenV1
{
    internal class Hotro
    {
        public static readonly string AppPath = Directory.GetCurrentDirectory();
        public static readonly string version = "1.0 Beta";
    }
    public class CSVHelper
    {
        /// <summary>
        /// Lưu dữ liệu từ DataGridView vào file CSV.
        /// </summary>
        public static void SaveToCSV(DataGridView dgv, string filePath)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < dgv.Columns.Count; i++)
                {
                    sb.Append(dgv.Columns[i].HeaderText + (i < dgv.Columns.Count - 1 ? "," : ""));
                }
                sb.AppendLine();

                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        for (int i = 0; i < dgv.Columns.Count; i++)
                        {
                            string value = row.Cells[i].Value?.ToString().Replace(",", " ") ?? "";
                            sb.Append(value + (i < dgv.Columns.Count - 1 ? "," : ""));
                        }
                        sb.AppendLine();
                    }
                }

                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
                MessageBox.Show("Lưu CSV thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu CSV: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Tải dữ liệu từ file CSV vào DataGridView.
        /// </summary>
        public static void LoadFromCSV(DataGridView dgv, string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("File không tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string[] lines = File.ReadAllLines(filePath);
                if (lines.Length == 0) return;

                dgv.Rows.Clear();
                dgv.Columns.Clear();

                string[] headers = lines[0].Split(',');
                foreach (string header in headers)
                {
                    dgv.Columns.Add(header, header);
                }

                for (int i = 1; i < lines.Length; i++)
                {
                    string[] data = lines[i].Split(',');
                    dgv.Rows.Add(data);
                }

                MessageBox.Show("Tải CSV thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải CSV: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public class GeminiClient
    {
        private readonly string _apiKey;
        private readonly string _modelType;
        private readonly GenerativeModel _model;

        /// <summary>
        /// Khởi tạo GeminiClient với API Key và loại model
        /// </summary>
        /// <param name="apiKey">Google API Key</param>
        /// <param name="modelType">Loại model (ví dụ: "models/gemini-2.0-flash")</param>
        public GeminiClient(string apiKey, string modelType = "models/gemini-2.0-flash")
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _modelType = modelType ?? throw new ArgumentNullException(nameof(modelType));

            _model = new GenerativeModel(_apiKey, _modelType);
        }

        /// <summary>
        /// Tạo text từ text input
        /// </summary>
        /// <param name="inputText">Text đầu vào</param>
        /// <returns>Text được sinh ra</returns>
        public async Task<string> GenerateTextFromTextAsync(string inputText)
        {
            if (string.IsNullOrEmpty(inputText))
                throw new ArgumentNullException(nameof(inputText));

            try
            {
                var request = new GenerateContentRequest
                {
                    Contents = new List<Content>
                    {
                        new Content
                        {
                            Role = "user",
                            Parts = new List<Part>
                            {
                                new Part { Text = inputText }
                            }
                        }
                    }
                };

                var response = await _model.GenerateContentAsync(request);
                return response.Candidates[0].Content.Parts[0].Text;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo text: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tạo text từ ảnh và text input
        /// </summary>
        /// <param name="inputText">Text đầu vào</param>
        /// <param name="imagePath">Đường dẫn đến file ảnh cục bộ</param>
        /// <returns>Text được sinh ra</returns>
        public async Task<string> GenerateTextFromImageAndTextAsync(string inputText, string imagePath)
        {
            if (string.IsNullOrEmpty(inputText))
                throw new ArgumentNullException(nameof(inputText));
            if (string.IsNullOrEmpty(imagePath))
                throw new ArgumentNullException(nameof(imagePath));

            try
            {
                var request = new GenerateContentRequest();
                request.AddText(inputText);
                request.AddInlineFile(imagePath);

                var response = await _model.GenerateContentAsync(request);
                return response.Candidates[0].Content.Parts[0].Text;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo text từ ảnh và text: {ex.Message}", ex);
            }
        }
    }
}
