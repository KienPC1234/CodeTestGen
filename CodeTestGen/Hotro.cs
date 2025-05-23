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
using Aspose.Pdf;
using Aspose.Pdf.Devices;
using System.Runtime.CompilerServices;
namespace CodeTestGenV1
{
    internal class Hotro
    {
        public static readonly string AppPath = Directory.GetCurrentDirectory();
        public static readonly string version = "1.0R";
        public static string StuffFolder = null;

        //AI
        public static string promptGenTesCaseCode(string Sotest, string EditorData, string YeuCau, string SinhTestModule)
        {
            string prompt = $@"
Hãy viết code Python để sinh {Sotest} test theo yêu cầu của tôi.
- Yêu Cầu Quan Trọng: Bắt Buộc phải bọc toàn bộ code của bạn tạo ra trong thẻ <MakeByAIFlag> và </MakeByAIFlag>
- Dữ liệu có sẵn trong biến EditorData.
- Yêu cầu người dùng có trong biến YeuCau.
- Thư Viện 'SinhTest' đã được import sẵn, có các hàm hỗ trợ sinh test, hãy tận dụng triệt để Thư Viên Này!
- Không Được Phép Phản Hồi Lại Bằng Markdown, Nội dung thông thường thôi!
- Bắt Buộc Sinh Đủ {Sotest} Testcase.

Yêu cầu bắt buộc:
- Chỉ in ra code Python.
- Phải bọc toàn bộ code trong <MakeByAIFlag> và </MakeByAIFlag>.
- Không được viết mô tả, giải thích hay in gì ngoài thẻ.
- Code phải chạy được, rõ ràng, trực tiếp sinh test từ EditorData hoặc theo yêu cầu.
- Hãy ưu tiên dùng EditorData làm mẫu đầu vào để sinh test phù hợp với code!

Biến:
EditorData: {EditorData}
YeuCau: {YeuCau}
Thư viện 'SinhTest': {SinhTestModule}

Ví dụ:

EditorData: Không có gì cụ thể
YeuCau: Sinh 20 test là ma trận ngẫu nhiên kích thước (1-100), giá trị 1-100.

Trả lời:
<MakeByAIFlag>
from SinhTest import *

for test_index in range(1, 21):
    testcase(test_index)
    n = random_number(1, 100)
    m = random_number(1, 100)
    testcase_print(n + ' ' + m)
    xuong_dong()
    testcase_print(random_matrix(n, m, 1, 100, 0))
    endtestcase()

SaveTestCases()
</MakeByAIFlag>
";
            return prompt;
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
        private string[] Pdf2Image(string filepath, string baseTempDir)
        {
            string uuid = Guid.NewGuid().ToString();
            string tempDir = Path.Combine(baseTempDir, uuid);
            Directory.CreateDirectory(tempDir);

            List<string> imagePaths = new List<string>();
            using (var pdfDocument = new Aspose.Pdf.Document(filepath))
            {
                var resolution = new Resolution(300);
                for (int pageNumber = 1; pageNumber <= pdfDocument.Pages.Count; pageNumber++)
                {
                    string outputPath = Path.Combine(tempDir, $"page_{pageNumber}.png");
                    using (var imageStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        var pngDevice = new PngDevice(new PageSize(595, 842), resolution);
                        pngDevice.Process(pdfDocument.Pages[pageNumber], imageStream);
                    }
                    imagePaths.Add(outputPath);
                }
            }
            return imagePaths.ToArray();
        }

        public async Task<string> GenerateTextFromImageAndTextAsync(string inputText, string[] imageFolder)
        {
            if (string.IsNullOrEmpty(inputText))
                throw new ArgumentNullException(nameof(inputText));

            string baseTempDir = Path.Combine(Path.GetTempPath(), "ctgPDF");
            Directory.CreateDirectory(baseTempDir);

            try
            {
                var request = new GenerateContentRequest();
                request.AddText(inputText);

                foreach (var file in imageFolder)
                {
                    if (File.Exists(file))
                    {
                        if (Path.GetExtension(file).ToLower() == ".pdf")
                        {
                            string[] imagePaths = Pdf2Image(file, baseTempDir);
                            foreach (var imagePath in imagePaths)
                            {
                                request.AddInlineFile(imagePath);
                            }
                        }
                        else
                        {
                            request.AddInlineFile(file);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Không Tìm Thấy File Ở: {file}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

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