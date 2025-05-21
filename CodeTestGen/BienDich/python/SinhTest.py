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
from typing import List
import itertools

RED = "\033[31m"
RESET = "\033[0m"

def my_excepthook(exctype, value, traceback):
    
    print(f"\n{RED}Đã xảy ra lỗi: {exctype.__name__} - {value}{RESET}")
    import traceback as tb
    print("Traceback:")
    tb.print_tb(traceback)
    
    print("Nhấn phím bất kỳ để thoát, hoặc đợi 5 giây...")
    start_time = time.time()
    while time.time() - start_time < 5:
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


def _print_animated_banner() -> None:
    banner: List[str] = [
        r"╔════════════════════════════════════════════════════════════════════════════════╗",
        r"║ █████   ████   █████████  ██████████      ██████████   ██████████ █████   █████║",
        r"║░░███   ███░   ███░░░░░███░░███░░░░███    ░░███░░░░███ ░░███░░░░░█░░███   ░░███ ║",
        r"║ ░███  ███    ███     ░░░  ░███   ░░███    ░███   ░░███ ░███  █ ░  ░███    ░███ ║",
        r"║ ░███████    ░███          ░███    ░███    ░███    ░███ ░██████    ░███    ░███ ║",
        r"║ ░███░░███   ░███          ░███    ░███    ░███    ░███ ░███░░█    ░░███   ███  ║",
        r"║ ░███ ░░███  ░░███     ███ ░███    ███     ░███    ███  ░███ ░   █  ░░░█████░   ║",
        r"║ █████ ░░████ ░░█████████  ██████████      ██████████   ██████████    ░░███     ║",
        r"║░░░░░   ░░░░   ░░░░░░░░░  ░░░░░░░░░░      ░░░░░░░░░░   ░░░░░░░░░░      ░░░      ║",
        r"╚═════════════════════════════════════════════════ v1.0 ═════════════════════════╝"
    ]

    color: str = '\033[38;5;46m' 
    reset: str = '\033[0m'

    print("\033c", end="")  # Clear screen

    for line in banner:
        print(color + line + reset)
        time.sleep(0.05)  # Delay giữa các dòng (tùy chỉnh được)

    # Hiệu ứng nhấp nháy dòng cuối
    for _ in range(2):
        sys.stdout.write("\033[1A")  # Move cursor up
        sys.stdout.write(color + banner[-1] + reset + "\n")
        sys.stdout.flush()
        time.sleep(0.1)
        sys.stdout.write("\033[1A")
        sys.stdout.write("\033[38;5;82m" + banner[-1] + reset + "\n")
        sys.stdout.flush()
        time.sleep(0.1)

_print_animated_banner()

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
    - Nếu min_val > 0 và negative_ratio > 0, số âm được tạo đối xứng trong [-max_val, -min_val].
    - Nếu min_val < 0, số âm được tạo trong [min_val, 0].
    - Số dương được tạo trong [0, max_val] hoặc [min_val, max_val] tùy trường hợp.
    """
    if not isinstance(count, int) or count < 1:
        raise ValueError("Số lượng phải là số nguyên dương")
    if min_val > max_val:
        raise ValueError("min_val phải nhỏ hơn hoặc bằng max_val")
    if not (0 <= negative_ratio <= 100):
        raise ValueError("negative_ratio phải nằm trong [0, 100]")

    res = []
    for _ in range(count):
        is_negative = random.random() < (negative_ratio / 100)
        if min_val > 0 and is_negative:
            val = random.randint(min_val, max_val)
            res.append(-val)
        elif min_val < 0 and is_negative:
            val = random.randint(min_val, 0)
            res.append(val)
        else:
            val = random.randint(max(0, min_val), max_val)
            res.append(val)
    return ' '.join(map(str, res))

def random_matrix(rows, cols, min_val, max_val, negative_ratio):
    """
    Tạo ma trận ngẫu nhiên với tỷ lệ số âm nhất định.
    - Nếu min_val > 0, số âm được tạo đối xứng trong [-max_val, -min_val].
    - Nếu min_val < 0, số âm được tạo trong [min_val, 0].
    - Số dương được tạo trong [max(0, min_val), max_val].
    :param rows: Số hàng của ma trận.
    :param cols: Số cột của ma trận.
    :param min_val: Giá trị nhỏ nhất của phần tử.
    :param max_val: Giá trị lớn nhất của phần tử.
    :param negative_ratio: Tỷ lệ số âm (%).
    :return: Ma trận dưới dạng chuỗi, các hàng cách nhau bởi '\n', phần tử trong hàng cách nhau bởi ' '.
    """
    if not isinstance(rows, int) or not isinstance(cols, int) or rows < 1 or cols < 1:
        raise ValueError("Số hàng và cột phải là số nguyên dương")
    if min_val > max_val:
        raise ValueError("min_val phải nhỏ hơn hoặc bằng max_val")
    if not (0 <= negative_ratio <= 100):
        raise ValueError("negative_ratio phải nằm trong [0, 100]")

    matrix = []
    for _ in range(rows):
        row = []
        for _ in range(cols):
            is_negative = random.random() < (negative_ratio / 100)
            if min_val > 0 and is_negative:
                val = random.randint(min_val, max_val)
                row.append(-val)
            elif min_val < 0 and is_negative:
                val = random.randint(min_val, 0)
                row.append(val)
            else:
                val = random.randint(max(0, min_val), max_val)
                row.append(val)
        matrix.append([str(val) for val in row])
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
    Tạo danh sách số thực ngẫu nhiên trong phạm vi min_val đến max_val.
    """
    if not isinstance(count, int) or count < 1:
        raise ValueError("Số lượng phải là số nguyên dương")
    if not isinstance(decimals, int) or decimals < 0:
        raise ValueError("Số chữ số thập phân phải là số nguyên không âm")
    if not isinstance(min_val, (int, float)) or not isinstance(max_val, (int, float)):
        raise ValueError("min_val và max_val phải là số")
    if min_val > max_val:
        raise ValueError("min_val phải nhỏ hơn hoặc bằng max_val")
    return ' '.join(f'{random.uniform(min_val, max_val):.{decimals}f}' for _ in range(count))

def random_tree(n, min_val, max_val, rooted=False):
    """
    Tạo cây ngẫu nhiên với n đỉnh nằm trong [min_val, max_val].
    Nếu rooted=True, gốc là đỉnh đầu tiên.
    """
    if not isinstance(n, int) or n < 2:
        raise ValueError("Số đỉnh phải là số nguyên >= 2")
    if min_val > max_val:
        raise ValueError("min_val phải nhỏ hơn hoặc bằng max_val")

    vertices = list(range(min_val, min_val + n))
    edges = []
    for i in range(1, n):
        parent = vertices[0] if rooted else random.choice(vertices[:i])
        edges.append((parent, vertices[i]))
    return '\n'.join(f'{u} {v}' for u, v in edges)

def random_graph(n, m, min_val, max_val, directed=False, connected=False):
    """
    Tạo đồ thị với n đỉnh và m cạnh trong phạm vi min_val đến max_val.
    :param connected: Nếu True, đảm bảo đồ thị liên thông (yêu cầu m >= n-1).
    """
    if not isinstance(n, int) or n < 1:
        raise ValueError("Số đỉnh phải là số nguyên dương")
    if not isinstance(m, int) or m < 0:
        raise ValueError("Số cạnh phải là số nguyên không âm")
    if min_val > max_val:
        raise ValueError("min_val phải nhỏ hơn hoặc bằng max_val")
    if connected and m < n - 1:
        raise ValueError("Số cạnh phải ít nhất n-1 để đảm bảo liên thông")

    vertices = list(range(min_val, min_val + n))
    max_edges = n * (n - 1) if directed else n * (n - 1) // 2
    m = min(m, max_edges)

    edges = set()
    if connected:
        # Tạo cây khung để đảm bảo liên thông
        for i in range(1, n):
            parent = random.choice(vertices[:i])
            edge = (parent, vertices[i]) if directed else tuple(sorted((parent, vertices[i])))
            edges.add(edge)
    
    # Tạo danh sách cạnh có thể
    if directed:
        possible_edges = [(u, v) for u in vertices for v in vertices if u != v and (u, v) not in edges]
    else:
        possible_edges = [tuple(sorted((u, v))) for u, v in itertools.combinations(vertices, 2) if tuple(sorted((u, v))) not in edges]
    
    # Chọn ngẫu nhiên các cạnh còn lại
    remaining_edges = min(m - len(edges), len(possible_edges))
    if remaining_edges > 0:
        edges.update(random.sample(possible_edges, remaining_edges))

    return '\n'.join(f'{u} {v}' for u, v in edges)

def random_permutation(n, min_val, max_val):
    """
    Tạo hoán vị ngẫu nhiên của các số trong phạm vi từ min_val đến max_val.
    """
    if not isinstance(n, int) or n < 1:
        raise ValueError("Số phải là số nguyên dương")
    if min_val > max_val:
        raise ValueError("min_val phải nhỏ hơn hoặc bằng max_val")
    lst = list(range(min_val, min_val + n))
    random.shuffle(lst)
    return ' '.join(map(str, lst))

def random_graph_weighted(n, m, min_val, max_val, weight_min=1, weight_max=100, directed=False, connected=False):
    """
    Tạo đồ thị có trọng số với n đỉnh và m cạnh.
    :param connected: Nếu True, đảm bảo đồ thị liên thông (yêu cầu m >= n-1).
    """
    if not isinstance(n, int) or n < 1:
        raise ValueError("Số đỉnh phải là số nguyên dương")
    if not isinstance(m, int) or m < 0:
        raise ValueError("Số cạnh phải là số nguyên không âm")
    if min_val > max_val:
        raise ValueError("min_val phải nhỏ hơn hoặc bằng max_val")
    if connected and m < n - 1:
        raise ValueError("Số cạnh phải ít nhất n-1 để đảm bảo liên thông")
    if weight_min > weight_max:
        raise ValueError("weight_min phải nhỏ hơn hoặc bằng weight_max")

    vertices = list(range(min_val, min_val + n))
    max_edges = n * (n - 1) if directed else n * (n - 1) // 2
    m = min(m, max_edges)

    edges = set()
    result = []
    if connected:
        # Tạo cây khung để đảm bảo liên thông
        for i in range(1, n):
            parent = random.choice(vertices[:i])
            edge = (parent, vertices[i]) if directed else tuple(sorted((parent, vertices[i])))
            edges.add(edge)
            w = random.randint(weight_min, weight_max)
            result.append(f'{edge[0]} {edge[1]} {w}')

    # Tạo danh sách cạnh có thể
    if directed:
        possible_edges = [(u, v) for u in vertices for v in vertices if u != v and (u, v) not in edges]
    else:
        possible_edges = [tuple(sorted((u, v))) for u, v in itertools.combinations(vertices, 2) if tuple(sorted((u, v))) not in edges]
    
    # Chọn ngẫu nhiên các cạnh còn lại
    remaining_edges = min(m - len(edges), len(possible_edges))
    if remaining_edges > 0:
        extra_edges = random.sample(possible_edges, remaining_edges)
        for u, v in extra_edges:
            w = random.randint(weight_min, weight_max)
            result.append(f'{u} {v} {w}')

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

