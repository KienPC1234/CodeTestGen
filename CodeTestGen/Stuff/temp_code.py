from SinhTest import *

for test_index in range(1, 31):
    testcase(test_index)
    n = 4
    a = random_range_list(0, 10**9, n)
    testcase_print(a)
    endtestcase()

for test_index in range(31, 61):
    testcase(test_index)
    n = random_number(5, 6)
    a = random_range_list(0, 10**9, n)
    testcase_print(a)
    endtestcase()

for test_index in range(61, 101):
    testcase(test_index)
    n = random_number(7, 8)
    a = random_range_list(0, 10**9, n)
    testcase_print(a)
    endtestcase()

SaveTestCases()