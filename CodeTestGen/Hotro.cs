using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Web.WebView2.Core;

namespace CodeTestGenV1
{
    internal class Hotro
    {
        public static readonly string AppPath = Directory.GetCurrentDirectory();
        public const float version = 1F;
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

                // Tạo cột từ dòng tiêu đề
                string[] headers = lines[0].Split(',');
                foreach (string header in headers)
                {
                    dgv.Columns.Add(header, header);
                }

                // Thêm dữ liệu vào DataGridView
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

}
