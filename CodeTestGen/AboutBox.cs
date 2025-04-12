using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using MaterialSkin.Controls;
using MaterialSkin;

namespace CodeTestGenV1
{
    partial class AboutBox : MaterialForm
    {
        private readonly TextStyle hyperlinkStyle = new TextStyle(Brushes.Blue, null, FontStyle.Underline);
        private readonly TextStyle numberStyle = new TextStyle(Brushes.Green, null, FontStyle.Bold);

        public AboutBox()
        {
            InitializeComponent();
            this.Text = $"About {AssemblyTitle}";
            labelProductName.Text = AssemblyProduct;
            labelVersion.Text = $"Version {AssemblyVersion}";
            labelCopyright.Text = AssemblyCopyright+ " © " + AssemblyCompany;
            fastColoredTextBox1.Text = AssemblyDescription;

            fastColoredTextBox1.Language = Language.Custom;
            fastColoredTextBox1.DescriptionFile = null;
            fastColoredTextBox1.OnTextChanged();
            HighlightText();
            
            fastColoredTextBox1.MouseClick += FastColoredTextBox1_MouseClick;
        }

        private void HighlightText()
        {
            string urlPattern = @"https?:\/\/[^\s]+";
            string numberPattern = @"\b\d+\b";

            var range = fastColoredTextBox1.Range;
            range.ClearStyle(hyperlinkStyle, numberStyle);
            range.SetStyle(hyperlinkStyle, urlPattern);
            range.SetStyle(numberStyle, numberPattern);
        }

        private void FastColoredTextBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Place place = fastColoredTextBox1.PointToPlace(e.Location);
            var line = fastColoredTextBox1.GetLineText(place.iLine);

            string urlPattern = @"(https?:\/\/[^\s]+)";
            Match match = Regex.Match(line, urlPattern);
            if (match.Success)
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = match.Value,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không thể mở liên kết: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #region Assembly Attribute Accessors
        public string AssemblyTitle => GetAssemblyAttribute<AssemblyTitleAttribute>()?.Title ?? "Ứng dụng";
        public string AssemblyVersion => Hotro.version;
        public string AssemblyDescription =>
@"App Được Làm Bởi KCD Dev (Kien TensorFlow)

Copyright 2025 KCD Dev

Licensed under the Apache License, Version 2.0 (the 'License');
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

You can contribute or fork the project at:
https://github.com/KienPC1234/CodeTestGen";
        public string AssemblyProduct => GetAssemblyAttribute<AssemblyProductAttribute>()?.Product ?? "Sản phẩm";
        public string AssemblyCopyright => "Apache License 2.0";
        public string AssemblyCompany => GetAssemblyAttribute<AssemblyCompanyAttribute>()?.Company ?? "Công ty";
        #endregion

        private void materialRaisedButton1_Click(object sender, EventArgs e) => Close();

        private T GetAssemblyAttribute<T>() where T : Attribute
        {
            return (T)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(T));
        }

    }
}
