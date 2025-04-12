using System;
using System.Text;
using Args;

namespace TestMaker
{
    public class CompileArgs
    {
        public string CompilerPath { get; set; }
        public string CompilerOption { get; set; }
        public string ScriptData { get; set; }
        public bool IsExeFlag { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            //var parsed = Args.Configuration.Configure<CompileArgs>().CreateAndBind(args);
            using (var executor = new BienDich.CompilerExecutor())
            {
                Console.InputEncoding = Encoding.UTF8;
                Console.OutputEncoding = Encoding.UTF8;

                Console.WriteLine("=== BẮT ĐẦU TEST CÁC TRƯỜNG HỢP ===");
                Console.WriteLine();

                // Test 1: Code dùng freopen (xóa được, trả null)
                TestCase(executor, "Test 1: Code dùng freopen (C++)", @"
                    #include <stdio.h>
                    // test.in comment
                    int main() {
                        freopen(""test.inp"", ""r"", stdin);
                        freopen(""test.out"", ""w"", stdout);
                        int x; scanf(""%d"", &x); printf(""%d"", x);
                        return 0;
                    }");

                // Test 2: Code dùng file I/O không xóa được (tìm thấy file)
                TestCase(executor, "Test 2: Code dùng file I/O (C++)", @"
                    #include <stdio.h>
                    // test.in comment
                    int main() {
                        FILE *fin = fopen(""test.inp"", ""r"");
                        FILE *fout = fopen(""test.out"", ""w"");
                        int x; fscanf(fin, ""%d"", &x); fprintf(fout, ""%d"", x);
                        fclose(fin); fclose(fout);
                        return 0;
                    }");

                // Test 3: Code có .in/.out nhưng không tìm thấy file (ép nhập thủ công)
                TestCase(executor, "Test 3: Code không tìm thấy file (C++)", @"#include <bits/stdc++.h>
#define FileNM ""QuanTrong""
using namespace std;

int main() {
    ios_base::sync_with_stdio(0);
    cin.tie(0);
    freopen(FileNM "".inp"", ""r"", stdin);
    freopen(FileNM "".out"", ""w"", stdout);
    
    long long n;
    cin >> n;
    vector<long long> a(n);
    unordered_map<long long, long long> umap;
    long long ans = 0, prefix_sum = 0;
    
    for (long long i = 0; i < n; i++) {
        cin >> a[i];
    }
    
    for (long long i = 0; i < n; i++) {
        prefix_sum += a[i];
        if (umap.count(a[i])) {
            ans = max(ans, prefix_sum - umap[a[i]]);
        } else {
            umap[a[i]] = prefix_sum - a[i];
        }
    }
    
    cout << ans;
    return 0;
}
");

                // Test 4: Code không dùng file I/O (trả null)
                TestCase(executor, "Test 4: Code không dùng file I/O (C++)", @"
                    #include <stdio.h>
                    // test.in comment
                    int main() {
                        int x; scanf(""%d"", &x); printf(""%d"", x);
                        return 0;
                    }");

                Console.WriteLine("=== KẾT THÚC TEST ===");
            }
        }
        static void TestCase(BienDich.CompilerExecutor executor, string testName, string scriptData)
        {
            Console.WriteLine(testName);
            Console.WriteLine("----------------------------------------");

            var args = new BienDich.CompilerArguments
            {
                CompilerPath = "g++.exe",
                ScriptData = scriptData
            };

            var result = args.ExtractIOFiles();
            Console.WriteLine("Nội dung script sau khi xử lý:");
            Console.WriteLine(result.Item3);
            Console.WriteLine();

            if (result.Item1 == null && result.Item2 == null)
            {
                Console.WriteLine("Kết quả: Trả về null (không có file I/O hoặc đã xóa hết)");
            }
            else
            {
                Console.WriteLine("Kết quả:");
                Console.WriteLine("  Input File: {0}", result.Item1);
                Console.WriteLine("  Output File: {0}", result.Item2);
            }

            var compileResult = executor.ProcessAndCompile(args, "-O2", result);
            if (compileResult != null)
            {
                Console.WriteLine("Biên dịch thành công:");
                Console.WriteLine("  Compiled Path: {0}", compileResult.CompiledPath);
            }

            Console.WriteLine("----------------------------------------");
            Console.WriteLine();
        }
    }
}

