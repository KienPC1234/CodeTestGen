from SinhTest import *

n = None
m = None


for test_index in range(1, 20 + 1):
      testcase(test_index)
      n = random_number(1, 100)
      m = random_number(1, 100)
      testcase_print((str(n) + str(' ') + str(m)))
      xuong_dong()
      testcase_print((random_matrix(n, m, 1, 100, 0)))
      endtestcase()

SaveTestCases()