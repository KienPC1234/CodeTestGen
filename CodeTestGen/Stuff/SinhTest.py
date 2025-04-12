# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

import random
import rstr
import msvcrt
import time
import sys

RED = "\033[31m"
RESET = "\033[0m"

def my_excepthook(exctype, value, traceback):
    
    print(f"\n{RED}Đã xảy ra lỗi: {exctype.__name__} - {value}{RESET}")
    import traceback as tb
    print("Traceback:")
    tb.print_tb(traceback)
    
    print("Nhấn phím bất kỳ để thoát, hoặc đợi 2 giây...")
    start_time = time.time()
    while time.time() - start_time < 2:
        if msvcrt.kbhit():  
            key = msvcrt.getch()  
            print(f"Đã nhấn phím: {key}")
            break
        time.sleep(0.1)
    
    print("Kết thúc.")
    sys.exit(-1)

sys.excepthook = my_excepthook

def _print(text: str) -> None:
    color = '\033[38;5;46m'  
    reset = '\033[0m'
    print(color + text + reset,end="")


_print(r"""
╔════════════════════════════════════════════════════════════════════════════════╗
║ █████   ████   █████████  ██████████      ██████████   ██████████ █████   █████║
║░░███   ███░   ███░░░░░███░░███░░░░███    ░░███░░░░███ ░░███░░░░░█░░███   ░░███ ║
║ ░███  ███    ███     ░░░  ░███   ░░███    ░███   ░░███ ░███  █ ░  ░███    ░███ ║
║ ░███████    ░███          ░███    ░███    ░███    ░███ ░██████    ░███    ░███ ║
║ ░███░░███   ░███          ░███    ░███    ░███    ░███ ░███░░█    ░░███   ███  ║
║ ░███ ░░███  ░░███     ███ ░███    ███     ░███    ███  ░███ ░   █  ░░░█████░   ║
║ █████ ░░████ ░░█████████  ██████████      ██████████   ██████████    ░░███     ║
║░░░░░   ░░░░   ░░░░░░░░░  ░░░░░░░░░░      ░░░░░░░░░░   ░░░░░░░░░░      ░░░      ║
╚════════════════════════════════════════════════════════════════════════════════╝
               
""")
_print("Make by KCD DEV (KienTensorFlow) - https://github.com/KienPC1234/CodeTestGen\n")
_print("Chú ý: Vui lòng không thoát khi đang sinh test!\n\n")

output = []
is_creating_testcase = False

output.append("<?xml version='1.0' encoding='utf-8'?>")
output.append('<testcases>')

def random_number(min_val, max_val):
    if not isinstance(min_val, int) or not isinstance(max_val, int):
        raise ValueError("min_val và max_val phải là số nguyên")
    if min_val >= max_val:
        raise ValueError("min_val phải nhỏ hơn max_val")
    return random.randint(min_val, max_val)

def random_string(length, chars):
    """
    Hàm này tạo ra một chuỗi ngẫu nhiên có độ dài nhất định với các ký tự được chỉ định.
    :param length: Độ dài của chuỗi.
    :param chars: Các ký tự có thể xuất hiện trong chuỗi.
    :return: Chuỗi ngẫu nhiên.
    """
    return ''.join(random.choice(chars) for _ in range(length))

def random_list(count, min_val, max_val, negative_ratio):
    """
    Tạo danh sách số nguyên ngẫu nhiên, một phần có thể là số âm.
    - Nếu tỷ lệ số âm là X%, thì khoảng X% số sẽ là số âm đối xứng trong [-max_val, -min_val].
    """
    if not isinstance(count, int) or count < 1:
        raise ValueError("Số lượng phải là số nguyên dương")
    if min_val <= 0 or max_val <= 0:
        raise ValueError("min_val và max_val phải > 0 để đối xứng âm/dương")
    if min_val > max_val:
        raise ValueError("min_val phải nhỏ hơn hoặc bằng max_val")
    if not (0 <= negative_ratio <= 100):
        raise ValueError("negative_ratio phải nằm trong [0, 100]")

    res = []
    for _ in range(count):
        is_negative = random.random() < (negative_ratio / 100)
        val = random.randint(min_val, max_val)
        res.append(-val if is_negative else val)
    return ' '.join(map(str, res))

def random_matrix(rows, cols, min_val, max_val, negative_ratio):
    """
    Hàm này tạo ra một ma trận ngẫu nhiên trong phạm vi min_val đến max_val.
    :param rows: Số hàng của ma trận.
    :param cols: Số cột của ma trận.
    :param min_val: Giá trị nhỏ nhất của phần tử.
    :param max_val: Giá trị lớn nhất của phần tử.
    :param negative_ratio: Tỷ lệ số âm trong ma trận.
    :return: Ma trận dưới dạng chuỗi.
    """
    if min_val >= max_val:
        raise ValueError("min_val phải nhỏ hơn max_val")
    matrix = [[str(random.randint(-max_val if random.random() < negative_ratio / 100 else min_val, max_val)) for _ in range(cols)] for _ in range(rows)]
    return '\n'.join(' '.join(row) for row in matrix)

def random_regex(regex, length):
    """
    Hàm này tạo ra chuỗi ngẫu nhiên theo biểu thức chính quy (regex).
    :param regex: Biểu thức chính quy.
    :param length: Số lượng chuỗi cần tạo.
    :return: Chuỗi kết hợp từ các kết quả ngẫu nhiên từ regex.
    """
    results = []
    for _ in range(length):
        results.append(rstr.xeger(regex))
    return ''.join(results)

def random_uppercase(length):
    """
    Hàm này tạo ra chuỗi ngẫu nhiên với các ký tự viết hoa.
    :param length: Độ dài chuỗi.
    :return: Chuỗi ngẫu nhiên gồm các ký tự viết hoa.
    """
    return random_string(length, "ABCDEFGHIJKLMNOPQRSTUVWXYZ")

def random_lowercase(length):
    """
    Hàm này tạo ra chuỗi ngẫu nhiên với các ký tự viết thường.
    :param length: Độ dài chuỗi.
    :return: Chuỗi ngẫu nhiên gồm các ký tự viết thường.
    """
    return random_string(length, "abcdefghijklmnopqrstuvwxyz")

def custom_function(code, *args):
    """
    Hàm này thực thi mã nguồn Python tùy chỉnh và trả về kết quả.
    :param code: Mã nguồn Python dưới dạng chuỗi.
    :param args: Các tham số truyền vào cho mã nguồn Python.
    :return: Kết quả của mã nguồn thực thi.
    """
    local_vars = {'args': args}
    exec(code, globals(), local_vars)
    return local_vars.get('result', None)

def random_range_list(min_val, max_val, count):
    """
    Hàm này tạo ra một danh sách các số ngẫu nhiên trong phạm vi min_val đến max_val.
    :param min_val: Giá trị nhỏ nhất của phạm vi.
    :param max_val: Giá trị lớn nhất của phạm vi.
    :param count: Số lượng phần tử trong danh sách.
    :return: Một chuỗi các số ngẫu nhiên trong phạm vi, ngăn cách bằng dấu cách.
    """
    if min_val >= max_val:
        raise ValueError("min_val phải nhỏ hơn max_val")
    return ' '.join(str(random.randint(min_val, max_val)) for _ in range(count))

def random_choice(values):
    """
    Hàm này trả về một giá trị ngẫu nhiên từ một chuỗi các giá trị.
    :param values: Chuỗi các giá trị có thể chọn.
    :return: Giá trị ngẫu nhiên.
    """
    return random.choice(values.split())

def shuffle_list(lst):
    """
    Hàm này xáo trộn một danh sách các giá trị.
    :param lst: Chuỗi các giá trị cần xáo trộn.
    :return: Chuỗi các giá trị đã xáo trộn.
    """
    lst = lst.split()
    random.shuffle(lst)
    return ' '.join(lst)

def set_seed(seed):
    """
    Hàm này đặt lại seed cho các hàm random để đảm bảo tính ngẫu nhiên có thể tái tạo.
    :param seed: Giá trị seed để khởi tạo random.
    """
    random.seed(seed)

def random_float_list(count, min_val, max_val, decimals):
    """
    Hàm này tạo ra một danh sách các số thực ngẫu nhiên trong phạm vi min_val đến max_val.
    :param count: Số lượng phần tử trong danh sách.
    :param min_val: Giá trị nhỏ nhất của số thực.
    :param max_val: Giá trị lớn nhất của số thực.
    :param decimals: Số chữ số thập phân.
    :return: Một chuỗi các số thực ngẫu nhiên, ngăn cách bằng dấu cách.
    """
    if not isinstance(count, int) or count < 1:
        raise ValueError("Số lượng phải là số nguyên dương")
    if not isinstance(decimals, int) or decimals < 0:
        raise ValueError("Số chữ số thập phân phải là số nguyên không âm")
    if min_val >= max_val:
        raise ValueError("min_val phải nhỏ hơn max_val")
    return ' '.join(f'{random.uniform(min_val, max_val):.{decimals}f}' for _ in range(count))

def random_tree(n, min_val, max_val, rooted=False):
    """
    Tạo cây ngẫu nhiên với n đỉnh nằm trong [min_val, max_val].
    Nếu rooted=True, gốc là đỉnh đầu tiên.
    """
    if not isinstance(n, int) or n < 2:
        raise ValueError("Số đỉnh phải là số nguyên >= 2")
    if min_val >= max_val:
        raise ValueError("min_val phải nhỏ hơn max_val")
    if n > max_val - min_val + 1:
        raise ValueError("Số đỉnh vượt quá phạm vi min_val đến max_val")

    vertices = list(range(min_val, min_val + n))
    edges = []
    for i in range(1, n):
        parent = vertices[0] if rooted else random.choice(vertices[:i])
        edges.append((parent, vertices[i]))
    return '\n'.join(f'{u} {v}' for u, v in edges)

def random_graph(n, m, min_val, max_val, directed=False):
    """
    Hàm này tạo ra một đồ thị với số đỉnh và số cạnh nhất định trong phạm vi min_val đến max_val.
    :param n: Số lượng đỉnh trong đồ thị.
    :param m: Số lượng cạnh trong đồ thị.
    :param min_val: Giá trị nhỏ nhất của đỉnh.
    :param max_val: Giá trị lớn nhất của đỉnh.
    :param directed: Đồ thị có hướng hay không.
    :return: Đồ thị dưới dạng chuỗi.
    """
    if not isinstance(n, int) or n < 1:
        raise ValueError("Số đỉnh phải là số nguyên dương")
    if not isinstance(m, int) or m < 0:
        raise ValueError("Số cạnh phải là số nguyên không âm")
    if min_val >= max_val:
        raise ValueError("min_val phải nhỏ hơn max_val")
    if n > max_val - min_val + 1:
        raise ValueError("Số đỉnh vượt quá phạm vi min_val đến max_val")
    vertices = list(range(min_val, min_val + n))
    max_edges = n * (n - 1) if directed else n * (n - 1) // 2
    if m > max_edges:
        m = max_edges
    edges = set()
    while len(edges) < m:
        u = random.choice(vertices)
        v = random.choice(vertices)
        if u != v:
            edge = (u, v) if u < v or directed else (v, u)
            edges.add(edge)
    return '\n'.join(f'{u} {v}' for u, v in edges)

def random_permutation(n, min_val, max_val):
    """
    Hàm này tạo ra một hoán vị ngẫu nhiên của các số trong phạm vi từ min_val đến max_val.
    :param n: Số lượng phần tử trong hoán vị.
    :param min_val: Giá trị nhỏ nhất của phần tử.
    :param max_val: Giá trị lớn nhất của phần tử.
    :return: Hoán vị ngẫu nhiên của các số trong phạm vi.
    """
    if not isinstance(n, int) or n < 1:
        raise ValueError("Số phải là số nguyên dương")
    if min_val >= max_val:
        raise ValueError("min_val phải nhỏ hơn max_val")
    if n > max_val - min_val + 1:
        raise ValueError("Số phần tử vượt quá phạm vi min_val đến max_val")
    lst = list(range(min_val, min_val + n))
    random.shuffle(lst)
    return ' '.join(map(str, lst))

def random_graph_weighted(n, m, min_val, max_val, weight_min=1, weight_max=100, directed=False):
    """
    Tạo đồ thị có trọng số với n đỉnh và m cạnh.
    Các đỉnh nằm trong khoảng [min_val, max_val], trọng số nằm trong [weight_min, weight_max].
    """
    if not isinstance(n, int) or n < 1:
        raise ValueError("Số đỉnh phải là số nguyên dương")
    if not isinstance(m, int) or m < 0:
        raise ValueError("Số cạnh phải là số nguyên không âm")
    if min_val >= max_val:
        raise ValueError("min_val phải nhỏ hơn max_val")
    if n > max_val - min_val + 1:
        raise ValueError("Số đỉnh vượt quá phạm vi")
    if weight_min > weight_max:
        raise ValueError("weight_min phải <= weight_max")

    vertices = list(range(min_val, min_val + n))
    max_edges = n * (n - 1) if directed else n * (n - 1) // 2
    m = min(m, max_edges)

    edges = set()
    result = []
    while len(edges) < m:
        u = random.choice(vertices)
        v = random.choice(vertices)
        if u != v:
            edge = (u, v) if directed else tuple(sorted((u, v)))
            if edge not in edges:
                edges.add(edge)
                w = random.randint(weight_min, weight_max)
                result.append(f'{edge[0]} {edge[1]} {w}')

    return '\n'.join(result)

GREEN = '\033[38;5;82m'
RESET = '\033[0m'

def testcase(caseNum):
    global is_creating_testcase
    if is_creating_testcase:
        raise RuntimeError(f"{RED}Lỗi: Test case trước đó chưa được đóng bằng endtestcase(){RESET}")
    
    is_creating_testcase = True
    animations = ['.', '..', '...']
    
    for anim in animations:
        sys.stdout.write(f"\r{GREEN}Đang Tạo Test Case: {caseNum} {anim}      {RESET}")
        sys.stdout.flush()
    
    sys.stdout.write(f"\r{GREEN}Đang Tạo Test Case: {caseNum}      {RESET}")
    sys.stdout.flush()
    
    output.append(f'<testcase case="{caseNum}">')

def endtestcase():
    global is_creating_testcase
    if not is_creating_testcase:
        raise RuntimeError(f"{RED}Lỗi: Không có test case nào đang được tạo{RESET}")
    
    is_creating_testcase = False
    output.append(f'</testcase>')
    

def testcase_print(result):
    try:
        output.append(str(result))
    except Exception as e:
        raise Exception(f"{RED}Lỗi khi in kết quả: {str(e)}{RESET}")

def xuong_dong():
    try:
        output.append("\n")
    except Exception as e:
        raise Exception(f"{RED}Lỗi khi xuống dòng: {str(e)}{RESET}")

def tao_khoang_trang(so):
    try:
        if not isinstance(so, int):
            raise ValueError("Số khoảng trắng phải là số nguyên")
        if so < 0:
            raise ValueError("Số khoảng trắng không được âm")
        output.append(" " * so)
    except Exception as e:
        raise Exception(f"{RED}Lỗi khi tạo khoảng trắng: {str(e)}{RESET}")
    
def SaveTestCases():
    """
    Hàm này lưu các test cases vào file XML 'testcases.xml' tại thư mục hiện tại.
    """
    output.append('</testcases>')
    with open("testcases.xml", "w") as f:
        for case in output:
            f.write(case)
    _print("\nĐã Tạo Xong Test Case!\n")
    print("Nhấn phím bất kỳ để thoát, hoặc đợi 2 giây...")
    start_time = time.time()
    while time.time() - start_time < 2:
        if msvcrt.kbhit():
            key = msvcrt.getch()
            print(f"Đã nhấn phím: {key}")
            break
        time.sleep(0.1)

    print("Kết thúc.")

