from SinhTest import *

for test_index in range(1, 26):
    testcase(test_index)
    m = random_number(1, 10)
    n = random_number(1, 10)
    testcase_print(str(m) + ' ' + str(n))
    xuong_dong()
    matrix_str = random_matrix(m, n, 0, 1000000, 0)
    testcase_print(matrix_str)
    endtestcase()

for test_index in range(26, 51):
    testcase(test_index)
    m = random_number(1, 50)
    n = random_number(1, 50)
    testcase_print(str(m) + ' ' + str(n))
    xuong_dong()
    matrix_str = random_matrix(m, n, 0, 1000000, 0)
    testcase_print(matrix_str)
    endtestcase()

for test_index in range(51, 76):
    testcase(test_index)
    m = random_number(1, 300)
    n = random_number(1, 300)
    testcase_print(str(m) + ' ' + str(n))
    xuong_dong()
    matrix_str = random_matrix(m, n, 0, 1000000, 0)
    testcase_print(matrix_str)
    endtestcase()

for test_index in range(76, 101):
    testcase(test_index)
    product = random_number(1, 1000000)
    m = random_number(1, min(1000, product))
    n = product // m
    testcase_print(str(m) + ' ' + str(n))
    xuong_dong()
    matrix_str = random_matrix(m, n, 0, 1000000, 0)
    testcase_print(matrix_str)
    endtestcase()

SaveTestCases()