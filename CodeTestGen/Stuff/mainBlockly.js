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

const workspace = Blockly.inject("blocklyDiv", {
  toolbox: `
        <xml id="toolbox" style="display: none">
            <category name="Test Containers" colour="160">
                <block type="variables_get">
                    <field name="VAR">test_index</field>
                </block>
                <block type="test_container">
                    <value name="NUM_TESTS"><block type="math_number"><field name="NUM">1</field></block></value>
                    <statement name="CONTENT">
                        <block type="testcase_start">
                            <next>
                                <block type="testcase_end"></block>
                            </next>
                        </block>
                    </statement>
                </block>
                <block type="test_subcontainer">
                    <value name="START"><block type="math_number"><field name="NUM">1</field></block></value>
                    <value name="END"><block type="math_number"><field name="NUM">1</field></block></value>
                </block>
                <block type="testcase_start"></block>
                <block type="testcase_end"></block>
            </category>
            <category name="Inputs" colour="290">
                <block type="math_number"></block>
                <block type="text"></block>
                <block type="logic_boolean"></block>
            </category>
            <category name="Math" colour="260">
                <block type="math_arithmetic"></block>
                <block type="custom_power">
                    <value name="BASE"><block type="math_number"><field name="NUM">2</field></block></value>
                    <value name="EXP"><block type="math_number"><field name="NUM">3</field></block></value>
                </block>
                <block type="math_sin">
                    <value name="ANGLE"><block type="math_number"><field name="NUM">0</field></block></value>
                </block>
                <block type="math_cos">
                    <value name="ANGLE"><block type="math_number"><field name="NUM">0</field></block></value>
                </block>
                <block type="math_tan">
                    <value name="ANGLE"><block type="math_number"><field name="NUM">0</field></block></value>
                </block>
                <block type="math_log">
                    <value name="VALUE"><block type="math_number"><field name="NUM">1</field></block></value>
                </block>
                <block type="math_sqrt">
                    <value name="VALUE"><block type="math_number"><field name="NUM">4</field></block></value>
                </block>
                <block type="math_exp">
                    <value name="VALUE"><block type="math_number"><field name="NUM">1</field></block></value>
                </block>
                <block type="math_abs">
                    <value name="VALUE"><block type="math_number"><field name="NUM">-5</field></block></value>
                </block>
                <block type="math_round">
                    <value name="VALUE"><block type="math_number"><field name="NUM">3.7</field></block></value>
                </block>
            </category>
            <category name="Strings" colour="180">
                <block type="string_concat">
                    <value name="VALUE0"><block type="text"><field name="TEXT">he</field></block></value>
                    <value name="VALUE1"><block type="math_number"><field name="NUM">123</field></block></value>
                </block>
                <block type="string_join">
                    <value name="SEPARATOR"><block type="text"><field name="TEXT">,</field></block></value>
                </block>
                <block type="string_split">
                    <value name="STRING"><block type="text"><field name="TEXT">a,b,c</field></block></value>
                    <value name="SEPARATOR"><block type="text"><field name="TEXT">,</field></block></value>
                </block>
                <block type="string_length">
                    <value name="STRING"><block type="text"><field name="TEXT">hello</field></block></value>
                </block>
                <block type="string_substring">
                    <value name="STRING"><block type="text"><field name="TEXT">hello</field></block></value>
                    <value name="START"><block type="math_number"><field name="NUM">0</field></block></value>
                    <value name="END"><block type="math_number"><field name="NUM">2</field></block></value>
                </block>
                <block type="string_to_upper">
                    <value name="STRING"><block type="text"><field name="TEXT">hello</field></block></value>
                </block>
                <block type="string_to_lower">
                    <value name="STRING"><block type="text"><field name="TEXT">HELLO</field></block></value>
                </block>
                <block type="string_char_at">
                    <value name="STRING"><block type="text"><field name="TEXT">hello</field></block></value>
                    <value name="INDEX"><block type="math_number"><field name="NUM">1</field></block></value>
                </block>
            </category>
            <category name="Logic" colour="210">
                <block type="logic_compare"></block>
                <block type="logic_operation"></block>
                <block type="logic_negate">
                    <value name="BOOL"><block type="logic_boolean"><field name="BOOL">TRUE</field></block></value>
                </block>
                <block type="custom_logic_and">
                    <value name="A"><block type="logic_boolean"><field name="BOOL">TRUE</field></block></value>
                    <value name="B"><block type="logic_boolean"><field name="BOOL">FALSE</field></block></value>
                </block>
                <block type="custom_logic_or">
                    <value name="A"><block type="logic_boolean"><field name="BOOL">TRUE</field></block></value>
                    <value name="B"><block type="logic_boolean"><field name="BOOL">FALSE</field></block></value>
                </block>
                <block type="custom_logic_not">
                    <value name="VALUE"><block type="logic_boolean"><field name="BOOL">TRUE</field></block></value>
                </block>
                <block type="custom_logic_xor">
                    <value name="A"><block type="logic_boolean"><field name="BOOL">TRUE</field></block></value>
                    <value name="B"><block type="logic_boolean"><field name="BOOL">FALSE</field></block></value>
                </block>
                <block type="custom_logic_imply">
                    <value name="A"><block type="logic_boolean"><field name="BOOL">TRUE</field></block></value>
                    <value name="B"><block type="logic_boolean"><field name="BOOL">FALSE</field></block></value>
                </block>
                <block type="custom_logic_equiv">
                    <value name="A"><block type="logic_boolean"><field name="BOOL">TRUE</field></block></value>
                    <value name="B"><block type="logic_boolean"><field name="BOOL">TRUE</field></block></value>
                </block>
            </category>
            <category name="Control" colour="120">
                <block type="controls_if"></block>
                <block type="controls_whileUntil"></block>
                <block type="controls_for">
                    <value name="FROM"><block type="math_number"><field name="NUM">1</field></block></value>
                    <value name="TO"><block type="math_number"><field name="NUM">10</field></block></value>
                    <value name="BY"><block type="math_number"><field name="NUM">1</field></block></value>
                </block>
                <block type="controls_repeat_ext">
                    <value name="TIMES"><block type="math_number"><field name="NUM">10</field></block></value>
                </block>
            </category>
            <category name="Variables" colour="330" custom="VARIABLE">
               
            </category>
            <category name="Functions" colour="230">
                <block type="print">
                    <value name="VALUE"><block type="text"><field name="TEXT">Hello</field></block></value>
                </block>
                <block type="space">
                    <value name="COUNT"><block type="math_number"><field name="NUM">1</field></block></value>
                </block>
                <block type="newline"></block>
                <block type="random_number">
                    <value name="MIN"><block type="math_number"><field name="NUM">1</field></block></value>
                    <value name="MAX"><block type="math_number"><field name="NUM">100</field></block></value>
                </block>
                <block type="random_float_list">
                    <value name="COUNT"><block type="math_number"><field name="NUM">5</field></block></value>
                    <value name="MIN"><block type="math_number"><field name="NUM">0</field></block></value>
                    <value name="MAX"><block type="math_number"><field name="NUM">100</field></block></value>
                    <value name="DECIMALS"><block type="math_number"><field name="NUM">2</field></block></value>
                </block>
                <block type="random_string">
                    <value name="LENGTH"><block type="math_number"><field name="NUM">5</field></block></value>
                    <value name="CHARS"><block type="text"><field name="TEXT">abc</field></block></value>
                </block>
                <block type="random_list">
                    <value name="COUNT"><block type="math_number"><field name="NUM">5</field></block></value>
                    <value name="MIN"><block type="math_number"><field name="NUM">1</field></block></value>
                    <value name="MAX"><block type="math_number"><field name="NUM">100</field></block></value>
                    <value name="NEGATIVE_RATIO"><block type="math_number"><field name="NUM">20</field></block></value>
                </block>
                <block type="random_matrix">
                    <value name="ROWS"><block type="math_number"><field name="NUM">2</field></block></value>
                    <value name="COLS"><block type="math_number"><field name="NUM">3</field></block></value>
                    <value name="MIN"><block type="math_number"><field name="NUM">1</field></block></value>
                    <value name="MAX"><block type="math_number"><field name="NUM">100</field></block></value>
                    <value name="NEGATIVE_RATIO"><block type="math_number"><field name="NUM">30</field></block></value>
                </block>
                <block type="random_regex">
                    <value name="REGEX"><block type="text"><field name="TEXT">[a-z]{3}</field></block></value>
                    <value name="LENGTH"><block type="math_number"><field name="NUM">1</field></block></value>
                </block>
                <block type="random_uppercase">
                    <value name="LENGTH"><block type="math_number"><field name="NUM">4</field></block></value>
                </block>
                <block type="random_lowercase">
                    <value name="LENGTH"><block type="math_number"><field name="NUM">4</field></block></value>
                </block>
                <block type="random_range_list">
                    <value name="START"><block type="math_number"><field name="NUM">1</field></block></value>
                    <value name="END"><block type="math_number"><field name="NUM">10</field></block></value>
                    <value name="COUNT"><block type="math_number"><field name="NUM">5</field></block></value>
                </block>
                <block type="random_choice">
                    <value name="VALUES"><block type="text"><field name="TEXT">1 3 5 7</field></block></value>
                </block>
                <block type="shuffle_list">
                    <value name="LIST"><block type="text"><field name="TEXT">1 2 3 4 5</field></block></value>
                </block>
                <block type="set_seed">
                    <value name="SEED"><block type="math_number"><field name="NUM">42</field></block></value>
                </block>
                <block type="random_tree">
                    <value name="N"><block type="math_number"><field name="NUM">5</field></block></value>
                    <value name="MIN"><block type="math_number"><field name="NUM">1</field></block></value>
                    <value name="MAX"><block type="math_number"><field name="NUM">100</field></block></value>
                    <value name="ROOTED"><block type="logic_boolean"><field name="BOOL">FALSE</field></block></value>
                </block>
                <block type="random_graph">
                    <value name="N"><block type="math_number"><field name="NUM">4</field></block></value>
                    <value name="M"><block type="math_number"><field name="NUM">4</field></block></value>
                    <value name="MIN"><block type="math_number"><field name="NUM">1</field></block></value>
                    <value name="MAX"><block type="math_number"><field name="NUM">100</field></block></value>
                    <value name="DIRECTED"><block type="logic_boolean"><field name="BOOL">FALSE</field></block></value>
                </block>
                <block type="random_graph_weighted">
                    <value name="N"><block type="math_number"><field name="NUM">4</field></block></value>
                    <value name="M"><block type="math_number"><field name="NUM">4</field></block></value>
                    <value name="MIN"><block type="math_number"><field name="NUM">1</field></block></value>
                    <value name="MAX"><block type="math_number"><field name="NUM">100</field></block></value>
                    <value name="WMIN"><block type="math_number"><field name="NUM">1</field></block></value>
                    <value name="WMAX"><block type="math_number"><field name="NUM">100</field></block></value>
                    <value name="DIRECTED"><block type="logic_boolean"><field name="BOOL">FALSE</field></block></value>
                </block>
                <block type="random_permutation">
                    <value name="N"><block type="math_number"><field name="NUM">5</field></block></value>
                    <value name="MIN"><block type="math_number"><field name="NUM">1</field></block></value>
                    <value name="MAX"><block type="math_number"><field name="NUM">100</field></block></value>
                </block>
                <block type="custom_function">
                    <value name="CODE"><block type="text"><field name="TEXT">result = sum(args)</field></block></value>
                    <value name="ARGS0"><block type="text"><field name="TEXT">Input 1</field></block></value>
                </block>
            </category>
        </xml>
    `,
  scrollbars: true,
  trashcan: true,
});

// Custom Block Definitions
Blockly.Blocks["random_number"] = {
  init: function () {
    this.appendValueInput("MIN")
      .setCheck("Number")
      .appendField("Số ngẫu nhiên từ");
    this.appendValueInput("MAX").setCheck("Number").appendField("đến");
    this.setOutput(true, "Number");
    this.setColour(230);
    this.setTooltip("Tạo số ngẫu nhiên trong khoảng");
  },
};

Blockly.Python["random_number"] = function (block) {
  const min =
    Blockly.Python.valueToCode(block, "MIN", Blockly.Python.ORDER_ATOMIC) ||
    "1";
  const max =
    Blockly.Python.valueToCode(block, "MAX", Blockly.Python.ORDER_ATOMIC) ||
    "100";
  return [`random_number(${min}, ${max})`, Blockly.Python.ORDER_FUNCTION_CALL];
};

Blockly.Blocks["random_float_list"] = {
  init: function () {
    this.appendValueInput("COUNT")
      .setCheck("Number")
      .appendField(" Ngẫu nhiên danh sách số thực, số lượng");
    this.appendValueInput("MIN").setCheck("Number").appendField("từ");
    this.appendValueInput("MAX").setCheck("Number").appendField("đến");
    this.appendValueInput("DECIMALS")
      .setCheck("Number")
      .appendField("số chữ số thập phân");
    this.setOutput(true, "String");
    this.setColour(230);
    this.setTooltip("Tạo danh sách số thực ngẫu nhiên");
  },
};

Blockly.Python["random_float_list"] = function (block) {
  const count =
    Blockly.Python.valueToCode(block, "COUNT", Blockly.Python.ORDER_ATOMIC) ||
    "5";
  const min =
    Blockly.Python.valueToCode(block, "MIN", Blockly.Python.ORDER_ATOMIC) ||
    "0";
  const max =
    Blockly.Python.valueToCode(block, "MAX", Blockly.Python.ORDER_ATOMIC) ||
    "100";
  const decimals =
    Blockly.Python.valueToCode(
      block,
      "DECIMALS",
      Blockly.Python.ORDER_ATOMIC
    ) || "2";
  return [
    `random_float_list(${count}, ${min}, ${max}, ${decimals})`,
    Blockly.Python.ORDER_FUNCTION_CALL,
  ];
};

Blockly.Blocks["random_string"] = {
  init: function () {
    this.appendValueInput("LENGTH")
      .setCheck("Number")
      .appendField("Chuỗi ngẫu nhiên, độ dài");
    this.appendValueInput("CHARS").setCheck("String").appendField("từ ký tự");
    this.setOutput(true, "String");
    this.setColour(230);
    this.setTooltip("Tạo chuỗi ngẫu nhiên từ tập ký tự");
  },
};

Blockly.Python["random_string"] = function (block) {
  const length =
    Blockly.Python.valueToCode(block, "LENGTH", Blockly.Python.ORDER_ATOMIC) ||
    "5";
  const chars =
    Blockly.Python.valueToCode(block, "CHARS", Blockly.Python.ORDER_ATOMIC) ||
    '"abc"';
  return [
    `random_string(${length}, ${chars})`,
    Blockly.Python.ORDER_FUNCTION_CALL,
  ];
};

Blockly.Blocks["random_list"] = {
  init: function () {
    this.appendValueInput("COUNT")
      .setCheck("Number")
      .appendField("Danh sách ngẫu nhiên, số lượng");
    this.appendValueInput("MIN").setCheck("Number").appendField("từ");
    this.appendValueInput("MAX").setCheck("Number").appendField("đến");
    this.appendValueInput("NEGATIVE_RATIO")
      .setCheck("Number")
      .appendField("tỷ lệ âm (%)");
    this.setOutput(true, "String");
    this.setColour(230);
    this.setTooltip("Tạo danh sách số ngẫu nhiên");
  },
};

Blockly.Python["random_list"] = function (block) {
  const count =
    Blockly.Python.valueToCode(block, "COUNT", Blockly.Python.ORDER_ATOMIC) ||
    "5";
  const min =
    Blockly.Python.valueToCode(block, "MIN", Blockly.Python.ORDER_ATOMIC) ||
    "1";
  const max =
    Blockly.Python.valueToCode(block, "MAX", Blockly.Python.ORDER_ATOMIC) ||
    "100";
  const negativeRatio =
    Blockly.Python.valueToCode(
      block,
      "NEGATIVE_RATIO",
      Blockly.Python.ORDER_ATOMIC
    ) || "20";
  return [
    `random_list(${count}, ${min}, ${max}, ${negativeRatio})`,
    Blockly.Python.ORDER_FUNCTION_CALL,
  ];
};

Blockly.Blocks["random_matrix"] = {
  init: function () {
    this.appendValueInput("ROWS")
      .setCheck("Number")
      .appendField("Ma trận ngẫu nhiên, số hàng");
    this.appendValueInput("COLS").setCheck("Number").appendField("số cột");
    this.appendValueInput("MIN").setCheck("Number").appendField("từ");
    this.appendValueInput("MAX").setCheck("Number").appendField("đến");
    this.appendValueInput("NEGATIVE_RATIO")
      .setCheck("Number")
      .appendField("tỷ lệ âm (%)");
    this.setOutput(true, "String");
    this.setColour(230);
    this.setTooltip("Tạo ma trận ngẫu nhiên");
  },
};

Blockly.Python["random_matrix"] = function (block) {
  const rows =
    Blockly.Python.valueToCode(block, "ROWS", Blockly.Python.ORDER_ATOMIC) ||
    "2";
  const cols =
    Blockly.Python.valueToCode(block, "COLS", Blockly.Python.ORDER_ATOMIC) ||
    "3";
  const min =
    Blockly.Python.valueToCode(block, "MIN", Blockly.Python.ORDER_ATOMIC) ||
    "1";
  const max =
    Blockly.Python.valueToCode(block, "MAX", Blockly.Python.ORDER_ATOMIC) ||
    "100";
  const negativeRatio =
    Blockly.Python.valueToCode(
      block,
      "NEGATIVE_RATIO",
      Blockly.Python.ORDER_ATOMIC
    ) || "30";
  return [
    `random_matrix(${rows}, ${cols}, ${min}, ${max}, ${negativeRatio})`,
    Blockly.Python.ORDER_FUNCTION_CALL,
  ];
};

Blockly.Blocks["random_regex"] = {
  init: function () {
    this.appendValueInput("REGEX")
      .setCheck("String")
      .appendField("Chuỗi ngẫu nhiên theo regex");
    this.appendValueInput("LENGTH")
      .setCheck("Number")
      .appendField("số lần lặp");
    this.setOutput(true, "String");
    this.setColour(230);
    this.setTooltip("Tạo chuỗi ngẫu nhiên theo regex");
  },
};

Blockly.Python["random_regex"] = function (block) {
  const regex =
    Blockly.Python.valueToCode(block, "REGEX", Blockly.Python.ORDER_ATOMIC) ||
    '"[a-z]{3}"';
  const length =
    Blockly.Python.valueToCode(block, "LENGTH", Blockly.Python.ORDER_ATOMIC) ||
    "1";
  return [
    `random_regex(${regex}, ${length})`,
    Blockly.Python.ORDER_FUNCTION_CALL,
  ];
};

Blockly.Blocks["random_uppercase"] = {
  init: function () {
    this.appendValueInput("LENGTH")
      .setCheck("Number")
      .appendField("Chuỗi in hoa ngẫu nhiên độ dài");
    this.setOutput(true, "String");
    this.setColour(230);
    this.setTooltip("Tạo chuỗi ngẫu nhiên chữ in hoa");
  },
};

Blockly.Python["random_uppercase"] = function (block) {
  const length =
    Blockly.Python.valueToCode(block, "LENGTH", Blockly.Python.ORDER_ATOMIC) ||
    "4";
  return [`random_uppercase(${length})`, Blockly.Python.ORDER_FUNCTION_CALL];
};

Blockly.Blocks["random_lowercase"] = {
  init: function () {
    this.appendValueInput("LENGTH")
      .setCheck("Number")
      .appendField("Chuỗi thường ngẫu nhiên độ dài");
    this.setOutput(true, "String");
    this.setColour(230);
    this.setTooltip("Tạo chuỗi ngẫu nhiên chữ thường");
  },
};

Blockly.Python["random_lowercase"] = function (block) {
  const length =
    Blockly.Python.valueToCode(block, "LENGTH", Blockly.Python.ORDER_ATOMIC) ||
    "4";
  return [`random_lowercase(${length})`, Blockly.Python.ORDER_FUNCTION_CALL];
};

Blockly.Blocks["random_range_list"] = {
  init: function () {
    this.appendValueInput("START")
      .setCheck("Number")
      .appendField("Danh sách ngẫu nhiên từ");
    this.appendValueInput("END").setCheck("Number").appendField("đến");
    this.appendValueInput("COUNT").setCheck("Number").appendField("số phần tử");
    this.setOutput(true, "String");
    this.setColour(230);
    this.setTooltip("Tạo danh sách số ngẫu nhiên trong khoảng");
  },
};

Blockly.Python["random_range_list"] = function (block) {
  const start =
    Blockly.Python.valueToCode(block, "START", Blockly.Python.ORDER_ATOMIC) ||
    "1";
  const end =
    Blockly.Python.valueToCode(block, "END", Blockly.Python.ORDER_ATOMIC) ||
    "10";
  const count =
    Blockly.Python.valueToCode(block, "COUNT", Blockly.Python.ORDER_ATOMIC) ||
    "5";
  return [
    `random_range_list(${start}, ${end}, ${count})`,
    Blockly.Python.ORDER_FUNCTION_CALL,
  ];
};

Blockly.Blocks["random_choice"] = {
  init: function () {
    this.appendValueInput("VALUES")
      .setCheck("String")
      .appendField("Chọn ngẫu nhiên từ");
    this.setOutput(true, "String");
    this.setColour(230);
    this.setTooltip("Chọn ngẫu nhiên một giá trị từ danh sách");
  },
};

Blockly.Python["random_choice"] = function (block) {
  const values =
    Blockly.Python.valueToCode(block, "VALUES", Blockly.Python.ORDER_ATOMIC) ||
    '"1 3 5 7"';
  return [`random_choice(${values})`, Blockly.Python.ORDER_FUNCTION_CALL];
};

Blockly.Blocks["shuffle_list"] = {
  init: function () {
    this.appendValueInput("LIST")
      .setCheck("String")
      .appendField("Xáo trộn danh sách");
    this.setOutput(true, "String");
    this.setColour(230);
    this.setTooltip("Xáo trộn các phần tử trong danh sách");
  },
};

Blockly.Python["shuffle_list"] = function (block) {
  const list =
    Blockly.Python.valueToCode(block, "LIST", Blockly.Python.ORDER_ATOMIC) ||
    '"1 2 3 4 5"';
  return [`shuffle_list(${list})`, Blockly.Python.ORDER_FUNCTION_CALL];
};

Blockly.Blocks["set_seed"] = {
  init: function () {
    this.appendValueInput("SEED")
      .setCheck("Number")
      .appendField("Đặt seed ngẫu nhiên");
    this.setPreviousStatement(true, null);
    this.setNextStatement(true, null);
    this.setColour(230);
    this.setTooltip("Đặt seed để tái lập kết quả ngẫu nhiên");
  },
};

Blockly.Python["set_seed"] = function (block) {
  const seed =
    Blockly.Python.valueToCode(block, "SEED", Blockly.Python.ORDER_ATOMIC) ||
    "42";
  return `set_seed(${seed})\n`;
};

Blockly.Blocks["random_tree"] = {
  init: function () {
    this.appendValueInput("N")
      .setCheck("Number")
      .appendField("Cây ngẫu nhiên, số đỉnh");
    this.appendValueInput("MIN").setCheck("Number").appendField("từ");
    this.appendValueInput("MAX").setCheck("Number").appendField("đến");
    this.appendValueInput("ROOTED").setCheck("Boolean").appendField("có gốc");
    this.setOutput(true, "String");
    this.setColour(230);
    this.setTooltip("Tạo cây ngẫu nhiên với n đỉnh");
  },
};

Blockly.Python["random_tree"] = function (block) {
  const n =
    Blockly.Python.valueToCode(block, "N", Blockly.Python.ORDER_ATOMIC) || "5";
  const min =
    Blockly.Python.valueToCode(block, "MIN", Blockly.Python.ORDER_ATOMIC) ||
    "1";
  const max =
    Blockly.Python.valueToCode(block, "MAX", Blockly.Python.ORDER_ATOMIC) ||
    "100";
  const rooted =
    Blockly.Python.valueToCode(block, "ROOTED", Blockly.Python.ORDER_ATOMIC) ||
    "False";
  return [
    `random_tree(${n}, ${min}, ${max}, ${rooted})`,
    Blockly.Python.ORDER_FUNCTION_CALL,
  ];
};

Blockly.Blocks["random_graph"] = {
  init: function () {
    this.appendValueInput("N")
      .setCheck("Number")
      .appendField("Đồ thị ngẫu nhiên, số đỉnh");
    this.appendValueInput("M").setCheck("Number").appendField("số cạnh");
    this.appendValueInput("MIN").setCheck("Number").appendField("từ");
    this.appendValueInput("MAX").setCheck("Number").appendField("đến");
    this.appendValueInput("DIRECTED")
      .setCheck("Boolean")
      .appendField("có hướng");
    this.setOutput(true, "String");
    this.setColour(230);
    this.setTooltip("Tạo đồ thị ngẫu nhiên với n đỉnh và m cạnh");
  },
};

Blockly.Blocks["random_graph_weighted"] = {
  init: function () {
    this.appendValueInput("N")
      .setCheck("Number")
      .appendField("Đồ thị có trọng số ngẫu nhiên, số đỉnh");
    this.appendValueInput("M").setCheck("Number").appendField("số cạnh");
    this.appendValueInput("MIN").setCheck("Number").appendField("từ");
    this.appendValueInput("MAX").setCheck("Number").appendField("đến");
    this.appendValueInput("DIRECTED")
      .setCheck("Boolean")
      .appendField("có hướng");
    this.appendValueInput("WMIN").setCheck("Number").appendField("trọng số từ");
    this.appendValueInput("WMAX").setCheck("Number").appendField("đến");
    this.setOutput(true, "String");
    this.setColour(230);
    this.setTooltip("Tạo đồ thị có trọng số với n đỉnh và m cạnh.");
  },
};

Blockly.Python["random_graph"] = function (block) {
  const n =
    Blockly.Python.valueToCode(block, "N", Blockly.Python.ORDER_ATOMIC) || "4";
  const m =
    Blockly.Python.valueToCode(block, "M", Blockly.Python.ORDER_ATOMIC) || "4";
  const min =
    Blockly.Python.valueToCode(block, "MIN", Blockly.Python.ORDER_ATOMIC) ||
    "1";
  const max =
    Blockly.Python.valueToCode(block, "MAX", Blockly.Python.ORDER_ATOMIC) ||
    "100";
  const directed =
    Blockly.Python.valueToCode(
      block,
      "DIRECTED",
      Blockly.Python.ORDER_ATOMIC
    ) || "False";
  return [
    `random_graph(${n}, ${m}, ${min}, ${max}, ${directed})`,
    Blockly.Python.ORDER_FUNCTION_CALL,
  ];
};

Blockly.Python["random_graph_weighted"] = function (block) {
  const n =
    Blockly.Python.valueToCode(block, "N", Blockly.Python.ORDER_ATOMIC) || "4";
  const m =
    Blockly.Python.valueToCode(block, "M", Blockly.Python.ORDER_ATOMIC) || "4";
  const min =
    Blockly.Python.valueToCode(block, "MIN", Blockly.Python.ORDER_ATOMIC) ||
    "1";
  const max =
    Blockly.Python.valueToCode(block, "MAX", Blockly.Python.ORDER_ATOMIC) ||
    "100";
  const wmin =
    Blockly.Python.valueToCode(block, "WMIN", Blockly.Python.ORDER_ATOMIC) ||
    "1";
  const wmax =
    Blockly.Python.valueToCode(block, "WMAX", Blockly.Python.ORDER_ATOMIC) ||
    "100";
  const directed =
    Blockly.Python.valueToCode(
      block,
      "DIRECTED",
      Blockly.Python.ORDER_ATOMIC
    ) || "False";
  return [
    `random_graph_weighted(${n}, ${m}, ${min}, ${max}, ${wmin}, ${wmax}, ${directed})`,
    Blockly.Python.ORDER_FUNCTION_CALL,
  ];
};

Blockly.Blocks["random_permutation"] = {
  init: function () {
    this.appendValueInput("N")
      .setCheck("Number")
      .appendField("Hoán vị ngẫu nhiên, số lượng");
    this.appendValueInput("MIN").setCheck("Number").appendField("từ");
    this.appendValueInput("MAX").setCheck("Number").appendField("đến");
    this.setOutput(true, "String");
    this.setColour(230);
    this.setTooltip("Tạo hoán vị ngẫu nhiên của n số");
  },
};

Blockly.Python["random_permutation"] = function (block) {
  const n =
    Blockly.Python.valueToCode(block, "N", Blockly.Python.ORDER_ATOMIC) || "5";
  const min =
    Blockly.Python.valueToCode(block, "MIN", Blockly.Python.ORDER_ATOMIC) ||
    "1";
  const max =
    Blockly.Python.valueToCode(block, "MAX", Blockly.Python.ORDER_ATOMIC) ||
    "100";
  return [
    `random_permutation(${n}, ${min}, ${max})`,
    Blockly.Python.ORDER_FUNCTION_CALL,
  ];
};

Blockly.Blocks["custom_function"] = {
  init: function () {
    this.appendValueInput("CODE")
      .setCheck("String")
      .appendField("Hàm tùy chỉnh với code");
    this.appendDummyInput().appendField("và đối số (*args)");
    this.setMutator(new Blockly.Mutator(["custom_function_arg"]));
    this.setOutput(true, null);
    this.setColour(230);
    this.setTooltip("Hàm tùy chỉnh nhận mã Python và nhiều đối số (*args)");
    this.argCount_ = 1;
    this.updateShape_();
  },
  mutationToDom: function () {
    const container = Blockly.utils.xml.createElement("mutation");
    container.setAttribute("args", this.argCount_);
    return container;
  },
  domToMutation: function (xmlElement) {
    this.argCount_ = parseInt(xmlElement.getAttribute("args"), 10) || 1;
    this.updateShape_();
  },
  decompose: function (workspace) {
    const containerBlock = workspace.newBlock("custom_function_container");
    containerBlock.initSvg();
    let connection = containerBlock.getInput("STACK").connection;
    for (let i = 0; i < this.argCount_; i++) {
      const argBlock = workspace.newBlock("custom_function_arg");
      argBlock.initSvg();
      connection.connect(argBlock.previousConnection);
      connection = argBlock.nextConnection;
    }
    return containerBlock;
  },
  compose: function (containerBlock) {
    let clauseBlock = containerBlock.getInputTargetBlock("STACK");
    this.argCount_ = 0;
    const valueConnections = [];
    while (clauseBlock) {
      valueConnections.push(clauseBlock.valueConnection_);
      clauseBlock =
        clauseBlock.nextConnection && clauseBlock.nextConnection.targetBlock();
      this.argCount_++;
    }
    this.updateShape_();
    for (let i = 0; i < this.argCount_; i++) {
      if (valueConnections[i]) {
        this.getInput("ARG" + i).connection.connect(valueConnections[i]);
      }
    }
  },
  updateShape_: function () {
    for (let i = 0; i < this.argCount_; i++) {
      if (!this.getInput("ARG" + i)) {
        this.appendValueInput("ARG" + i)
          .setCheck(null)
          .appendField("Đối số " + (i + 1));
      }
    }
    let i = this.argCount_;
    while (this.getInput("ARG" + i)) {
      this.removeInput("ARG" + i);
      i++;
    }
  },
};

Blockly.Python["custom_function"] = function (block) {
  const code =
    Blockly.Python.valueToCode(block, "CODE", Blockly.Python.ORDER_ATOMIC) ||
    '""';
  const args = [];
  for (let i = 0; i < block.argCount_; i++) {
    args.push(
      Blockly.Python.valueToCode(
        block,
        "ARG" + i,
        Blockly.Python.ORDER_ATOMIC
      ) || "None"
    );
  }
  return [
    `custom_function(${code}, ${args.join(", ")})`,
    Blockly.Python.ORDER_FUNCTION_CALL,
  ];
};

Blockly.Blocks["custom_function_container"] = {
  init: function () {
    this.appendDummyInput().appendField("Đối số");
    this.appendStatementInput("STACK");
    this.setColour(230);
    this.contextMenu = false;
  },
};

Blockly.Blocks["custom_function_arg"] = {
  init: function () {
    this.appendDummyInput().appendField("Thêm đối số");
    this.setPreviousStatement(true);
    this.setNextStatement(true);
    this.setColour(230);
    this.contextMenu = false;
  },
};

Blockly.Blocks["testcase_start"] = {
  init: function () {
    this.appendDummyInput().appendField("Bắt đầu testcase");
    this.setPreviousStatement(true, null);
    this.setNextStatement(true, null);
    this.setColour(160);
  },
};

Blockly.Python["testcase_start"] = function (block) {
  let parent = block.getParent();
  while (parent) {
    if (parent.type === "test_container") {
      return `testcase(test_index)\n`;
    }
    parent = parent.getParent();
  }
  return `testcase(1)\n`;
};

Blockly.Blocks["testcase_end"] = {
  init: function () {
    this.appendDummyInput().appendField("Kết thúc testcase");
    this.setPreviousStatement(true, null);
    this.setNextStatement(true, null);
    this.setColour(160);
  },
};

Blockly.Python["testcase_end"] = function (block) {
  return `endtestcase()\n`;
};

Blockly.Blocks["test_container"] = {
  init: function () {
    this.appendValueInput("NUM_TESTS")
      .setCheck("Number")
      .appendField("Test (số lượng)");
    this.appendStatementInput("CONTENT").setCheck(null);
    this.setColour(160);
  },
};

Blockly.Python["test_container"] = function (block) {
  const numTests =
    Blockly.Python.valueToCode(
      block,
      "NUM_TESTS",
      Blockly.Python.ORDER_ATOMIC
    ) || "1";
  const content = Blockly.Python.statementToCode(block, "CONTENT") || "";
  const indentedContent = content.replace(/^/gm, "    "); // Thụt lề 4 khoảng trắng
  return `for test_index in range(1, ${numTests} + 1):\n${indentedContent}`;
};

Blockly.Blocks["test_subcontainer"] = {
  init: function () {
    this.appendValueInput("START").setCheck("Number").appendField("Test (từ)");
    this.appendValueInput("END").setCheck("Number").appendField("đến");
    this.appendStatementInput("CONTENT").setCheck(null);
    this.setPreviousStatement(true, null);
    this.setNextStatement(true, null);
    this.setColour(160);
  },
};

Blockly.Python["test_subcontainer"] = function (block) {
  const start =
    Blockly.Python.valueToCode(block, "START", Blockly.Python.ORDER_ATOMIC) ||
    "1";
  const end =
    Blockly.Python.valueToCode(block, "END", Blockly.Python.ORDER_ATOMIC) ||
    "1";
  const content = Blockly.Python.statementToCode(block, "CONTENT") || "";
  const indentedContent = content.replace(/^/gm, "    ");
  return `if test_index >= ${start} and test_index <= ${end}:\n${indentedContent}\n`;
};

Blockly.Python["controls_if"] = function (block) {
  const condition =
    Blockly.Python.valueToCode(block, "IF0", Blockly.Python.ORDER_NONE) ||
    "False";
  const statements = Blockly.Python.statementToCode(block, "DO0") || "";
  const indentedStatements = statements.replace(/^/gm, "    ");
  return `if ${condition}:\n${indentedStatements}\n`;
};

Blockly.Blocks["print"] = {
  init: function () {
    this.appendValueInput("VALUE")
      .setCheck(["Number", "String", "Array"])
      .appendField("In ra");
    this.setPreviousStatement(true, null);
    this.setNextStatement(true, null);
    this.setColour(230);
  },
};

Blockly.Python["print"] = function (block) {
  const value =
    Blockly.Python.valueToCode(block, "VALUE", Blockly.Python.ORDER_ATOMIC) ||
    '""';
  return `testcase_print(${value})\n`;
};

Blockly.Blocks["space"] = {
  init: function () {
    this.appendValueInput("COUNT")
      .setCheck("Number")
      .appendField("In khoảng trắng");
    this.setPreviousStatement(true, null);
    this.setNextStatement(true, null);
    this.setColour(230);
  },
};

Blockly.Python["space"] = function (block) {
  const count =
    Blockly.Python.valueToCode(block, "COUNT", Blockly.Python.ORDER_ATOMIC) ||
    "1";
  return `tao_khoang_trang(${count})\n`;
};

Blockly.Blocks["newline"] = {
  init: function () {
    this.appendDummyInput().appendField("Xuống dòng");
    this.setPreviousStatement(true, null);
    this.setNextStatement(true, null);
    this.setColour(230);
  },
};

Blockly.Python["newline"] = function (block) {
  return `xuong_dong()\n`;
};

Blockly.Blocks["string_concat"] = {
  init: function () {
    this.appendValueInput("VALUE0").setCheck(null).appendField("Nối");
    this.appendValueInput("VALUE1").setCheck(null).appendField("+");
    this.setMutator(new Blockly.Mutator(["string_concat_arg"]));
    this.setOutput(true, "String");
    this.setColour(180);
    this.argCount_ = 2; // Bắt đầu với 2 giá trị
    this.updateShape_();
  },
  mutationToDom: function () {
    const container = Blockly.utils.xml.createElement("mutation");
    container.setAttribute("args", this.argCount_);
    return container;
  },
  domToMutation: function (xmlElement) {
    this.argCount_ = parseInt(xmlElement.getAttribute("args"), 10) || 2;
    this.updateShape_();
  },
  decompose: function (workspace) {
    const containerBlock = workspace.newBlock("string_concat_container");
    containerBlock.initSvg();
    let connection = containerBlock.getInput("STACK").connection;
    for (let i = 0; i < this.argCount_; i++) {
      const argBlock = workspace.newBlock("string_concat_arg");
      argBlock.initSvg();
      connection.connect(argBlock.previousConnection);
      connection = argBlock.nextConnection;
    }
    return containerBlock;
  },
  compose: function (containerBlock) {
    let clauseBlock = containerBlock.getInputTargetBlock("STACK");
    this.argCount_ = 0;
    const valueConnections = [];
    while (clauseBlock) {
      valueConnections.push(clauseBlock.valueConnection_);
      clauseBlock =
        clauseBlock.nextConnection && clauseBlock.nextConnection.targetBlock();
      this.argCount_++;
    }
    this.updateShape_();
    for (let i = 0; i < this.argCount_; i++) {
      if (valueConnections[i]) {
        this.getInput("VALUE" + i).connection.connect(valueConnections[i]);
      }
    }
  },
  updateShape_: function () {
    for (let i = 0; i < this.argCount_; i++) {
      if (!this.getInput("VALUE" + i)) {
        this.appendValueInput("VALUE" + i)
          .setCheck(null)
          .appendField(i === 0 ? "Nối" : "+");
      }
    }
    let i = this.argCount_;
    while (this.getInput("VALUE" + i)) {
      this.removeInput("VALUE" + i);
      i++;
    }
  },
};

Blockly.Blocks["string_concat_container"] = {
  init: function () {
    this.appendDummyInput().appendField("Giá trị");
    this.appendStatementInput("STACK");
    this.setColour(180);
    this.contextMenu = false;
  },
};

Blockly.Blocks["string_concat_arg"] = {
  init: function () {
    this.appendDummyInput().appendField("Thêm giá trị");
    this.setPreviousStatement(true);
    this.setNextStatement(true);
    this.setColour(180);
    this.contextMenu = false;
  },
};

Blockly.Python["string_concat"] = function (block) {
  const values = [];
  for (let i = 0; i < block.argCount_; i++) {
    const value =
      Blockly.Python.valueToCode(
        block,
        "VALUE" + i,
        Blockly.Python.ORDER_ATOMIC
      ) || '""';
    values.push(`str(${value})`);
  }
  return [values.join(" + "), Blockly.Python.ORDER_FUNCTION_CALL];
};

Blockly.Blocks["string_join"] = {
  init: function () {
    this.appendValueInput("ARRAY").setCheck("Array").appendField("Nối mảng");
    this.appendValueInput("SEPARATOR")
      .setCheck("String")
      .appendField("bằng dấu phân cách");
    this.setOutput(true, "String");
    this.setColour(180);
  },
};

Blockly.Python["string_join"] = function (block) {
  const array =
    Blockly.Python.valueToCode(block, "ARRAY", Blockly.Python.ORDER_ATOMIC) ||
    "[]";
  const separator =
    Blockly.Python.valueToCode(
      block,
      "SEPARATOR",
      Blockly.Python.ORDER_ATOMIC
    ) || '","';
  return [`${separator}.join(${array})`, Blockly.Python.ORDER_FUNCTION_CALL];
};

Blockly.Blocks["string_split"] = {
  init: function () {
    this.appendValueInput("STRING")
      .setCheck("String")
      .appendField("Chia chuỗi");
    this.appendValueInput("SEPARATOR")
      .setCheck("String")
      .appendField("bằng dấu phân cách");
    this.setOutput(true, "Array");
    this.setColour(180);
  },
};

Blockly.Python["string_split"] = function (block) {
  const string =
    Blockly.Python.valueToCode(block, "STRING", Blockly.Python.ORDER_ATOMIC) ||
    '""';
  const separator =
    Blockly.Python.valueToCode(
      block,
      "SEPARATOR",
      Blockly.Python.ORDER_ATOMIC
    ) || '","';
  return [`${string}.split(${separator})`, Blockly.Python.ORDER_FUNCTION_CALL];
};

Blockly.Blocks["string_length"] = {
  init: function () {
    this.appendValueInput("STRING")
      .setCheck("String")
      .appendField("Độ dài chuỗi");
    this.setOutput(true, "Number");
    this.setColour(180);
  },
};

Blockly.Python["string_length"] = function (block) {
  const string =
    Blockly.Python.valueToCode(block, "STRING", Blockly.Python.ORDER_ATOMIC) ||
    '""';
  return [`len(${string})`, Blockly.Python.ORDER_FUNCTION_CALL];
};

Blockly.Blocks["string_substring"] = {
  init: function () {
    this.appendValueInput("STRING").setCheck("String").appendField("Cắt chuỗi");
    this.appendValueInput("START").setCheck("Number").appendField("từ vị trí");
    this.appendValueInput("END").setCheck("Number").appendField("đến");
    this.setOutput(true, "String");
    this.setColour(180);
  },
};

Blockly.Python["string_substring"] = function (block) {
  const string =
    Blockly.Python.valueToCode(block, "STRING", Blockly.Python.ORDER_ATOMIC) ||
    '""';
  const start =
    Blockly.Python.valueToCode(block, "START", Blockly.Python.ORDER_ATOMIC) ||
    "0";
  const end =
    Blockly.Python.valueToCode(block, "END", Blockly.Python.ORDER_ATOMIC) ||
    "0";
  return [`${string}[${start}:${end}]`, Blockly.Python.ORDER_FUNCTION_CALL];
};

Blockly.Blocks["string_to_upper"] = {
  init: function () {
    this.appendValueInput("STRING")
      .setCheck("String")
      .appendField("Chuyển thành chữ in hoa");
    this.setOutput(true, "String");
    this.setColour(180);
  },
};

Blockly.Python["string_to_upper"] = function (block) {
  const string =
    Blockly.Python.valueToCode(block, "STRING", Blockly.Python.ORDER_ATOMIC) ||
    '""';
  return [`${string}.upper()`, Blockly.Python.ORDER_FUNCTION_CALL];
};

Blockly.Blocks["string_to_lower"] = {
  init: function () {
    this.appendValueInput("STRING")
      .setCheck("String")
      .appendField("Chuyển thành chữ thường");
    this.setOutput(true, "String");
    this.setColour(180);
  },
};

Blockly.Python["string_to_lower"] = function (block) {
  const string =
    Blockly.Python.valueToCode(block, "STRING", Blockly.Python.ORDER_ATOMIC) ||
    '""';
  return [`${string}.lower()`, Blockly.Python.ORDER_FUNCTION_CALL];
};

Blockly.Blocks["string_char_at"] = {
  init: function () {
    this.appendValueInput("STRING")
      .setCheck("String")
      .appendField("Lấy ký tự tại vị trí");
    this.appendValueInput("INDEX").setCheck("Number").appendField("vị trí");
    this.setOutput(true, "String");
    this.setColour(180);
  },
};

Blockly.Python["string_char_at"] = function (block) {
  const string =
    Blockly.Python.valueToCode(block, "STRING", Blockly.Python.ORDER_ATOMIC) ||
    '""';
  const index =
    Blockly.Python.valueToCode(block, "INDEX", Blockly.Python.ORDER_ATOMIC) ||
    "0";
  return [`${string}[${index}]`, Blockly.Python.ORDER_FUNCTION_CALL];
};

Blockly.Blocks["custom_power"] = {
  init: function () {
    this.appendValueInput("BASE")
      .setCheck("Number")
      .appendField("Lũy thừa cơ số");
    this.appendValueInput("EXP").setCheck("Number").appendField("mũ");
    this.setOutput(true, "Number");
    this.setColour(260);
  },
};

Blockly.Python["custom_power"] = function (block) {
  const base =
    Blockly.Python.valueToCode(block, "BASE", Blockly.Python.ORDER_ATOMIC) ||
    "2";
  const exp =
    Blockly.Python.valueToCode(block, "EXP", Blockly.Python.ORDER_ATOMIC) ||
    "3";
  return [`${base} ** ${exp}`, Blockly.Python.ORDER_EXPONENTIATION];
};

Blockly.Blocks["math_sin"] = {
  init: function () {
    this.appendValueInput("ANGLE").setCheck("Number").appendField("Sin");
    this.setOutput(true, "Number");
    this.setColour(260);
  },
};

Blockly.Python["math_sin"] = function (block) {
  const angle =
    Blockly.Python.valueToCode(block, "ANGLE", Blockly.Python.ORDER_ATOMIC) ||
    "0";
  return [`math.sin(${angle})`, Blockly.Python.ORDER_FUNCTION_CALL];
};

Blockly.Blocks["math_cos"] = {
  init: function () {
    this.appendValueInput("ANGLE").setCheck("Number").appendField("Cos");
    this.setOutput(true, "Number");
    this.setColour(260);
  },
};

Blockly.Python["math_cos"] = function (block) {
  const angle =
    Blockly.Python.valueToCode(block, "ANGLE", Blockly.Python.ORDER_ATOMIC) ||
    "0";
  return [`math.cos(${angle})`, Blockly.Python.ORDER_FUNCTION_CALL];
};

Blockly.Blocks["math_tan"] = {
  init: function () {
    this.appendValueInput("ANGLE").setCheck("Number").appendField("Tan");
    this.setOutput(true, "Number");
    this.setColour(260);
  },
};

Blockly.Python["math_tan"] = function (block) {
  const angle =
    Blockly.Python.valueToCode(block, "ANGLE", Blockly.Python.ORDER_ATOMIC) ||
    "0";
  return [`math.tan(${angle})`, Blockly.Python.ORDER_FUNCTION_CALL];
};

Blockly.Blocks["math_log"] = {
  init: function () {
    this.appendValueInput("VALUE").setCheck("Number").appendField("Log");
    this.setOutput(true, "Number");
    this.setColour(260);
  },
};

Blockly.Python["math_log"] = function (block) {
  const value =
    Blockly.Python.valueToCode(block, "VALUE", Blockly.Python.ORDER_ATOMIC) ||
    "1";
  return [`math.log(${value})`, Blockly.Python.ORDER_FUNCTION_CALL];
};

Blockly.Blocks["math_sqrt"] = {
  init: function () {
    this.appendValueInput("VALUE").setCheck("Number").appendField("Sqrt");
    this.setOutput(true, "Number");
    this.setColour(260);
  },
};

Blockly.Python["math_sqrt"] = function (block) {
  const value =
    Blockly.Python.valueToCode(block, "VALUE", Blockly.Python.ORDER_ATOMIC) ||
    "4";
  return [`math.sqrt(${value})`, Blockly.Python.ORDER_FUNCTION_CALL];
};

Blockly.Blocks["math_exp"] = {
  init: function () {
    this.appendValueInput("VALUE").setCheck("Number").appendField("Exp");
    this.setOutput(true, "Number");
    this.setColour(260);
  },
};

Blockly.Python["math_exp"] = function (block) {
  const value =
    Blockly.Python.valueToCode(block, "VALUE", Blockly.Python.ORDER_ATOMIC) ||
    "1";
  return [`math.exp(${value})`, Blockly.Python.ORDER_FUNCTION_CALL];
};

Blockly.Blocks["math_abs"] = {
  init: function () {
    this.appendValueInput("VALUE").setCheck("Number").appendField("Abs");
    this.setOutput(true, "Number");
    this.setColour(260);
  },
};

Blockly.Python["math_abs"] = function (block) {
  const value =
    Blockly.Python.valueToCode(block, "VALUE", Blockly.Python.ORDER_ATOMIC) ||
    "-5";
  return [`abs(${value})`, Blockly.Python.ORDER_FUNCTION_CALL];
};

Blockly.Blocks["math_round"] = {
  init: function () {
    this.appendValueInput("VALUE").setCheck("Number").appendField("Round");
    this.setOutput(true, "Number");
    this.setColour(260);
  },
};

Blockly.Python["math_round"] = function (block) {
  const value =
    Blockly.Python.valueToCode(block, "VALUE", Blockly.Python.ORDER_ATOMIC) ||
    "3.7";
  return [`round(${value})`, Blockly.Python.ORDER_FUNCTION_CALL];
};

Blockly.Blocks["custom_logic_and"] = {
  init: function () {
    this.appendValueInput("A").setCheck("Boolean").appendField("AND");
    this.appendValueInput("B").setCheck("Boolean");
    this.setOutput(true, "Boolean");
    this.setColour(210);
  },
};

Blockly.Python["custom_logic_and"] = function (block) {
  const a =
    Blockly.Python.valueToCode(block, "A", Blockly.Python.ORDER_ATOMIC) ||
    "True";
  const b =
    Blockly.Python.valueToCode(block, "B", Blockly.Python.ORDER_ATOMIC) ||
    "False";
  return [`${a} and ${b}`, Blockly.Python.ORDER_LOGICAL_AND];
};

Blockly.Blocks["custom_logic_or"] = {
  init: function () {
    this.appendValueInput("A").setCheck("Boolean").appendField("OR");
    this.appendValueInput("B").setCheck("Boolean");
    this.setOutput(true, "Boolean");
    this.setColour(210);
  },
};

Blockly.Python["custom_logic_or"] = function (block) {
  const a =
    Blockly.Python.valueToCode(block, "A", Blockly.Python.ORDER_ATOMIC) ||
    "True";
  const b =
    Blockly.Python.valueToCode(block, "B", Blockly.Python.ORDER_ATOMIC) ||
    "False";
  return [`${a} or ${b}`, Blockly.Python.ORDER_LOGICAL_OR];
};

Blockly.Blocks["custom_logic_not"] = {
  init: function () {
    this.appendValueInput("VALUE").setCheck("Boolean").appendField("NOT");
    this.setOutput(true, "Boolean");
    this.setColour(210);
  },
};

Blockly.Python["custom_logic_not"] = function (block) {
  const value =
    Blockly.Python.valueToCode(block, "VALUE", Blockly.Python.ORDER_ATOMIC) ||
    "True";
  return [`not ${value}`, Blockly.Python.ORDER_LOGICAL_NOT];
};

Blockly.Blocks["custom_logic_xor"] = {
  init: function () {
    this.appendValueInput("A").setCheck("Boolean").appendField("XOR");
    this.appendValueInput("B").setCheck("Boolean");
    this.setOutput(true, "Boolean");
    this.setColour(210);
  },
};

Blockly.Python["custom_logic_xor"] = function (block) {
  const a =
    Blockly.Python.valueToCode(block, "A", Blockly.Python.ORDER_ATOMIC) ||
    "True";
  const b =
    Blockly.Python.valueToCode(block, "B", Blockly.Python.ORDER_ATOMIC) ||
    "False";
  return [`(${a} != ${b})`, Blockly.Python.ORDER_EQUALITY];
};

Blockly.Blocks["custom_logic_imply"] = {
  init: function () {
    this.appendValueInput("A").setCheck("Boolean").appendField("IMPLY");
    this.appendValueInput("B").setCheck("Boolean");
    this.setOutput(true, "Boolean");
    this.setColour(210);
  },
};

Blockly.Python["custom_logic_imply"] = function (block) {
  const a =
    Blockly.Python.valueToCode(block, "A", Blockly.Python.ORDER_ATOMIC) ||
    "True";
  const b =
    Blockly.Python.valueToCode(block, "B", Blockly.Python.ORDER_ATOMIC) ||
    "False";
  return [`(not ${a} or ${b})`, Blockly.Python.ORDER_LOGICAL_OR];
};

Blockly.Blocks["custom_logic_equiv"] = {
  init: function () {
    this.appendValueInput("A").setCheck("Boolean").appendField("EQUIV");
    this.appendValueInput("B").setCheck("Boolean");
    this.setOutput(true, "Boolean");
    this.setColour(210);
  },
};

Blockly.Python["custom_logic_equiv"] = function (block) {
  const a =
    Blockly.Python.valueToCode(block, "A", Blockly.Python.ORDER_ATOMIC) ||
    "True";
  const b =
    Blockly.Python.valueToCode(block, "B", Blockly.Python.ORDER_ATOMIC) ||
    "True";
  return [`(${a} == ${b})`, Blockly.Python.ORDER_EQUALITY];
};

// Utility Functions
function getCode() {
  return Blockly.Python.workspaceToCode(workspace).trim();
}

function updatePythonCode() {
  const code = `from SinhTest import *\n\n${getCode()}\n\nSaveTestCases()`;
  document.getElementById("pythonCode").value = code;
}

function WebViewGetCode() {
  const code = `from SinhTest import *\n\n${getCode()}\n\nSaveTestCases()`;
  return code;
}

async function exportToJson() {
  try {
    const json = JSON.stringify(
      Blockly.serialization.workspaces.save(workspace)
    );
    const blob = new Blob([json], { type: "application/json" });

    const handle = await window.showSaveFilePicker({
      suggestedName: "blockly_workspace.json",
      types: [
        {
          description: "JSON File",
          accept: { "application/json": [".json"] },
        },
      ],
    });

    const writable = await handle.createWritable();
    await writable.write(blob);
    await writable.close();
  } catch (err) {
    console.error("Lỗi khi lưu tệp:", err);
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = "blockly_workspace.json";
    a.click();
    URL.revokeObjectURL(url);
  }
}
function loadFromJson(event) {
  const file = event.target.files[0];
  if (file) {
    const reader = new FileReader();
    reader.onload = function (e) {
      try {
        const json = e.target.result;
        const workspaceData = JSON.parse(json);
        workspace.clear();
        Blockly.serialization.workspaces.load(workspaceData, workspace);
        updatePythonCode();
      } catch (err) {
        alert("Lỗi khi tải JSON: " + err.message);
      }
    };
    reader.readAsText(file);
  }
}

const container = workspace.newBlock('test_container');
container.initSvg();
container.render();
container.moveBy(100, 100);

const numberBlock = workspace.newBlock('math_number');
numberBlock.setFieldValue('1', 'NUM');
numberBlock.initSvg();
numberBlock.render();

container.getInput('NUM_TESTS').connection.connect(numberBlock.outputConnection);

const startBlock = workspace.newBlock('testcase_start');
startBlock.initSvg();
startBlock.render();

const endBlock = workspace.newBlock('testcase_end');
endBlock.initSvg();
endBlock.render();

startBlock.nextConnection.connect(endBlock.previousConnection);

container.getInput('CONTENT').connection.connect(startBlock.previousConnection);

workspace.addChangeListener(function () {
  updatePythonCode();
});

updatePythonCode();
