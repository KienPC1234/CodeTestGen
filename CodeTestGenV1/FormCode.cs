using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin.Controls;

namespace CodeTestGenV1
{
    public partial class FormCode : MaterialForm
    {
        public FormCode(string Code)
        {
            InitializeComponent();
            fastColoredTextBox1.Text = Code;
        }
    }
}
