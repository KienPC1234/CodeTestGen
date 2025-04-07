// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

let editor;

function setText(content) {
  editor.setValue(content);
}

function clearText() {
  editor.setValue("");
}

function getText() {
  return editor.getValue();
}

function toggleDarkMode(enable) {
  document.getElementById("themeToggle").checked = enable;
  document.getElementById("themeToggle").dispatchEvent(new Event("change"));
}

function changeLanguage(language) {
  let lang = "";
  if (language === "1") {
    lang = "python";
    document.getElementById("language").value = "python";
  } else if (language === "2") {
    lang = "cpp";
    document.getElementById("language").value = "cpp";
  }
  monaco.editor.setModelLanguage(editor.getModel(), lang);
}

require(["vs/editor/editor.main"], function () {
  const cppSuggestions = [
    {
      label: "if",
      kind: monaco.languages.CompletionItemKind.Keyword,
      insertText: "if (${1:condition}) {\n\t$0\n}",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "else",
      kind: monaco.languages.CompletionItemKind.Keyword,
      insertText: "else {\n\t$0\n}",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "for",
      kind: monaco.languages.CompletionItemKind.Keyword,
      insertText: "for (${1:init}; ${2:condition}; ${3:increment}) {\n\t$0\n}",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "while",
      kind: monaco.languages.CompletionItemKind.Keyword,
      insertText: "while (${1:condition}) {\n\t$0\n}",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "do",
      kind: monaco.languages.CompletionItemKind.Keyword,
      insertText: "do {\n\t$0\n} while (${1:condition});",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "switch",
      kind: monaco.languages.CompletionItemKind.Keyword,
      insertText:
        "switch (${1:variable}) {\n\tcase ${2:value}:\n\t\t$0\n\t\tbreak;\n\tdefault:\n\t\tbreak;\n}",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "cout",
      kind: monaco.languages.CompletionItemKind.Function,
      insertText: "std::cout << ${1:message} << std::endl;",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "cin",
      kind: monaco.languages.CompletionItemKind.Function,
      insertText: "std::cin >> ${1:variable};",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "main",
      kind: monaco.languages.CompletionItemKind.Function,
      insertText: "int main() {\n\t$0\n\treturn 0;\n}",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "include bits/stdc++.h",
      kind: monaco.languages.CompletionItemKind.Module,
      insertText: "#include <bits/stdc++.h>",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "include iostream",
      kind: monaco.languages.CompletionItemKind.Module,
      insertText: "#include <iostream>",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "include vector",
      kind: monaco.languages.CompletionItemKind.Module,
      insertText: "#include <vector>",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "include string",
      kind: monaco.languages.CompletionItemKind.Module,
      insertText: "#include <string>",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "include algorithm",
      kind: monaco.languages.CompletionItemKind.Module,
      insertText: "#include <algorithm>",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "include cmath",
      kind: monaco.languages.CompletionItemKind.Module,
      insertText: "#include <cmath>",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "include cstdlib",
      kind: monaco.languages.CompletionItemKind.Module,
      insertText: "#include <cstdlib>",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "include cstdio",
      kind: monaco.languages.CompletionItemKind.Module,
      insertText: "#include <cstdio>",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "include cstring",
      kind: monaco.languages.CompletionItemKind.Module,
      insertText: "#include <cstring>",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "using namespace std",
      kind: monaco.languages.CompletionItemKind.Snippet,
      insertText: "using namespace std;",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
  ];

  const pythonSuggestions = [
    {
      label: "if",
      kind: monaco.languages.CompletionItemKind.Keyword,
      insertText: "if ${1:condition}:\n\t$0",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "elif",
      kind: monaco.languages.CompletionItemKind.Keyword,
      insertText: "elif ${1:condition}:\n\t$0",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "else",
      kind: monaco.languages.CompletionItemKind.Keyword,
      insertText: "else:\n\t$0",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "for",
      kind: monaco.languages.CompletionItemKind.Keyword,
      insertText: "for ${1:item} in ${2:iterable}:\n\t$0",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "while",
      kind: monaco.languages.CompletionItemKind.Keyword,
      insertText: "while ${1:condition}:\n\t$0",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "def",
      kind: monaco.languages.CompletionItemKind.Keyword,
      insertText: "def ${1:functionName}(${2:parameters}):\n\t$0",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "class",
      kind: monaco.languages.CompletionItemKind.Keyword,
      insertText:
        "class ${1:ClassName}:\n\tdef __init__(self${2:, parameters}):\n\t\t$0",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "print",
      kind: monaco.languages.CompletionItemKind.Function,
      insertText: "print(${1:message})",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "len",
      kind: monaco.languages.CompletionItemKind.Function,
      insertText: "len(${1:iterable})",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "range",
      kind: monaco.languages.CompletionItemKind.Function,
      insertText: "range(${1:start}, ${2:stop}${3:, step})",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "import math",
      kind: monaco.languages.CompletionItemKind.Module,
      insertText: "import math",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "import sys",
      kind: monaco.languages.CompletionItemKind.Module,
      insertText: "import sys",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "import os",
      kind: monaco.languages.CompletionItemKind.Module,
      insertText: "import os",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "import random",
      kind: monaco.languages.CompletionItemKind.Module,
      insertText: "import random",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "from datetime import datetime",
      kind: monaco.languages.CompletionItemKind.Module,
      insertText: "from datetime import datetime",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
    },
    {
      label: "from SinhTest import *",
      kind: monaco.languages.CompletionItemKind.Module,
      insertText: "from SinhTest import *",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
      detail: "Import all functions from SinhTest module",
    },
    {
      label: "random_number",
      kind: monaco.languages.CompletionItemKind.Function,
      insertText: "random_number(${1:min_val}, ${2:max_val})",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
      detail: "Generate a random integer between min_val and max_val",
    },
    {
      label: "random_string",
      kind: monaco.languages.CompletionItemKind.Function,
      insertText: "random_string(${1:length}, ${2:chars})",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
      detail:
        "Generate a random string of specified length with given characters",
    },
    {
      label: "random_list",
      kind: monaco.languages.CompletionItemKind.Function,
      insertText:
        "random_list(${1:count}, ${2:min_val}, ${3:max_val}, ${4:negative_ratio})",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
      detail:
        "Generate a list of random integers with a specified negative ratio",
    },
    {
      label: "random_matrix",
      kind: monaco.languages.CompletionItemKind.Function,
      insertText:
        "random_matrix(${1:rows}, ${2:cols}, ${3:min_val}, ${4:max_val}, ${5:negative_ratio})",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
      detail:
        "Generate a random matrix with specified dimensions and negative ratio",
    },
    {
      label: "random_regex",
      kind: monaco.languages.CompletionItemKind.Function,
      insertText: "random_regex(${1:regex}, ${2:length})",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
      detail: "Generate a random string matching a regex pattern",
    },
    {
      label: "random_uppercase",
      kind: monaco.languages.CompletionItemKind.Function,
      insertText: "random_uppercase(${1:length})",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
      detail: "Generate a random uppercase string of specified length",
    },
    {
      label: "random_lowercase",
      kind: monaco.languages.CompletionItemKind.Function,
      insertText: "random_lowercase(${1:length})",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
      detail: "Generate a random lowercase string of specified length",
    },
    {
      label: "custom_function",
      kind: monaco.languages.CompletionItemKind.Function,
      insertText: "custom_function(${1:code}, ${2:*args})",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
      detail: "Execute custom Python code with variable arguments",
    },
    {
      label: "random_range_list",
      kind: monaco.languages.CompletionItemKind.Function,
      insertText: "random_range_list(${1:min_val}, ${2:max_val}, ${3:count})",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
      detail: "Generate a list of random integers within a range",
    },
    {
      label: "random_choice",
      kind: monaco.languages.CompletionItemKind.Function,
      insertText: "random_choice(${1:values})",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
      detail: "Pick a random value from a space-separated string",
    },
    {
      label: "shuffle_list",
      kind: monaco.languages.CompletionItemKind.Function,
      insertText: "shuffle_list(${1:lst})",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
      detail: "Shuffle a space-separated list of values",
    },
    {
      label: "set_seed",
      kind: monaco.languages.CompletionItemKind.Function,
      insertText: "set_seed(${1:seed})",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
      detail: "Set the random seed for reproducible results",
    },
    {
      label: "random_float_list",
      kind: monaco.languages.CompletionItemKind.Function,
      insertText:
        "random_float_list(${1:count}, ${2:min_val}, ${3:max_val}, ${4:decimals})",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
      detail: "Generate a list of random floats with specified decimals",
    },
    {
      label: "random_tree",
      kind: monaco.languages.CompletionItemKind.Function,
      insertText:
        "random_tree(${1:n}, ${2:min_val}, ${3:max_val}, ${4:rooted=False})",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
      detail: "Generate a random tree with n vertices",
    },
    {
      label: "random_graph",
      kind: monaco.languages.CompletionItemKind.Function,
      insertText:
        "random_graph(${1:n}, ${2:m}, ${3:min_val}, ${4:max_val}, ${5:directed=False})",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
      detail: "Generate a random graph with n vertices and m edges",
    },
    {
      label: "random_permutation",
      kind: monaco.languages.CompletionItemKind.Function,
      insertText: "random_permutation(${1:n}, ${2:min_val}, ${3:max_val})",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
      detail: "Generate a random permutation of n numbers",
    },
    {
      label: "random_graph_weighted",
      kind: monaco.languages.CompletionItemKind.Function,
      insertText:
        "random_graph_weighted(${1:n}, ${2:m}, ${3:min_val}, ${4:max_val}, ${5:weight_min}, ${6:weight_max}, ${7:directed=False})",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
      detail: "Generate a weighted random graph",
    },
    {
      label: "testcase",
      kind: monaco.languages.CompletionItemKind.Function,
      insertText: "testcase(${1:caseNum})",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
      detail: "Start a new test case with a given number",
    },
    {
      label: "endtestcase",
      kind: monaco.languages.CompletionItemKind.Function,
      insertText: "endtestcase()",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
      detail: "End the current test case",
    },
    {
      label: "testcase_print",
      kind: monaco.languages.CompletionItemKind.Function,
      insertText: "testcase_print(${1:result})",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
      detail: "Print a result to the current test case",
    },
    {
      label: "xuong_dong",
      kind: monaco.languages.CompletionItemKind.Function,
      insertText: "xuong_dong()",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
      detail: "Insert a newline in the test case output",
    },
    {
      label: "tao_khoang_trang",
      kind: monaco.languages.CompletionItemKind.Function,
      insertText: "tao_khoang_trang(${1:so})",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
      detail: "Insert a specified number of spaces in the test case output",
    },
    {
      label: "SaveTestCases",
      kind: monaco.languages.CompletionItemKind.Function,
      insertText: "SaveTestCases()",
      insertTextRules:
        monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
      detail: "Save all test cases to testcases.xml",
    },
  ];

  editor = monaco.editor.create(document.getElementById("editor"), {
    value: `#Bạn Có Thể Dán Code Bài Test Vào Đây!`,
    language: "python",
    theme: "vs",
    automaticLayout: true,
  });

  monaco.languages.registerCompletionItemProvider("cpp", {
    provideCompletionItems: (model, position) => {
      const word = model.getWordUntilPosition(position);
      const range = {
        startLineNumber: position.lineNumber,
        endLineNumber: position.lineNumber,
        startColumn: word.startColumn,
        endColumn: word.endColumn,
      };
      const suggestionsWithRange = cppSuggestions.map((suggestion) => ({
        ...suggestion,
        range: range,
      }));
      return { suggestions: suggestionsWithRange };
    },
  });

  monaco.languages.registerCompletionItemProvider("python", {
    provideCompletionItems: (model, position) => {
      const word = model.getWordUntilPosition(position);
      const range = {
        startLineNumber: position.lineNumber,
        endLineNumber: position.lineNumber,
        startColumn: word.startColumn,
        endColumn: word.endColumn,
      };
      const suggestionsWithRange = pythonSuggestions.map((suggestion) => ({
        ...suggestion,
        range: range,
      }));
      return { suggestions: suggestionsWithRange };
    },
  });

  document.getElementById("language").addEventListener("change", function () {
    monaco.editor.setModelLanguage(editor.getModel(), this.value);
  });

  document
    .getElementById("themeToggle")
    .addEventListener("change", function () {
      if (this.checked) {
        document.body.classList.add("dark-mode");
        monaco.editor.setTheme("vs-dark");
      } else {
        document.body.classList.remove("dark-mode");
        monaco.editor.setTheme("vs");
      }
    });
});
