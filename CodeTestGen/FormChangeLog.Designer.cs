namespace CodeTestGen
{
    partial class FormChangeLog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormChangeLog));
            this.foreverListBox1 = new ReaLTaiizor.Controls.ForeverListBox();
            this.parrotSplashScreen1 = new ReaLTaiizor.Controls.ParrotSplashScreen();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.foreverButton1 = new ReaLTaiizor.Controls.ForeverButton();
            this.webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).BeginInit();
            this.SuspendLayout();
            // 
            // foreverListBox1
            // 
            this.foreverListBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(47)))), ((int)(((byte)(49)))));
            this.foreverListBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.foreverListBox1.Items = new string[] {
        "kien ",
        "pc"};
            this.foreverListBox1.Location = new System.Drawing.Point(0, 0);
            this.foreverListBox1.Name = "foreverListBox1";
            this.foreverListBox1.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(168)))), ((int)(((byte)(109)))));
            this.foreverListBox1.SelectedIndex = 0;
            this.foreverListBox1.SelectedItem = "kien ";
            this.foreverListBox1.Size = new System.Drawing.Size(187, 350);
            this.foreverListBox1.TabIndex = 0;
            this.foreverListBox1.Text = "foreverListBox1";
            // 
            // parrotSplashScreen1
            // 
            this.parrotSplashScreen1.AllowDragging = true;
            this.parrotSplashScreen1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.parrotSplashScreen1.BottomText = "ReaLTaizor Special Edition";
            this.parrotSplashScreen1.BottomTextColor = System.Drawing.Color.White;
            this.parrotSplashScreen1.BottomTextLocation = new System.Drawing.Point(51, 125);
            this.parrotSplashScreen1.BottomTextSize = 16;
            this.parrotSplashScreen1.EllipseCornerRadius = 15;
            this.parrotSplashScreen1.IsEllipse = false;
            this.parrotSplashScreen1.LoadedColor = System.Drawing.Color.DodgerBlue;
            this.parrotSplashScreen1.ProgressBarBorder = false;
            this.parrotSplashScreen1.ProgressBarLocation = new System.Drawing.Point(0, 224);
            this.parrotSplashScreen1.ProgressBarStyle = ReaLTaiizor.Controls.ParrotFlatProgressBar.Style.Material;
            this.parrotSplashScreen1.SecondsDisplayed = 3000;
            this.parrotSplashScreen1.ShowProgressBar = true;
            this.parrotSplashScreen1.SplashIcon = ((System.Drawing.Icon)(resources.GetObject("parrotSplashScreen1.SplashIcon")));
            this.parrotSplashScreen1.SplashSize = new System.Drawing.Size(450, 280);
            this.parrotSplashScreen1.TopText = "Visual Studio";
            this.parrotSplashScreen1.TopTextColor = System.Drawing.Color.White;
            this.parrotSplashScreen1.TopTextLocation = new System.Drawing.Point(0, 70);
            this.parrotSplashScreen1.TopTextSize = 36;
            this.parrotSplashScreen1.UnloadedColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(-1, 64);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.foreverListBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.foreverButton1);
            this.splitContainer1.Panel2.Controls.Add(this.webView21);
            this.splitContainer1.Size = new System.Drawing.Size(563, 350);
            this.splitContainer1.SplitterDistance = 187;
            this.splitContainer1.TabIndex = 1;
            // 
            // foreverButton1
            // 
            this.foreverButton1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.foreverButton1.BackColor = System.Drawing.Color.Transparent;
            this.foreverButton1.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(168)))), ((int)(((byte)(109)))));
            this.foreverButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.foreverButton1.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.foreverButton1.Location = new System.Drawing.Point(16, 298);
            this.foreverButton1.Name = "foreverButton1";
            this.foreverButton1.Rounded = false;
            this.foreverButton1.Size = new System.Drawing.Size(332, 40);
            this.foreverButton1.TabIndex = 1;
            this.foreverButton1.Text = "Cài Đặt Phiên Bản: ";
            this.foreverButton1.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.foreverButton1.Click += new System.EventHandler(this.foreverButton1_Click);
            // 
            // webView21
            // 
            this.webView21.AllowExternalDrop = true;
            this.webView21.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webView21.CreationProperties = null;
            this.webView21.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView21.Location = new System.Drawing.Point(3, 3);
            this.webView21.Name = "webView21";
            this.webView21.Size = new System.Drawing.Size(366, 279);
            this.webView21.TabIndex = 0;
            this.webView21.ZoomFactor = 1D;
            // 
            // FormChangeLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(561, 414);
            this.Controls.Add(this.splitContainer1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormChangeLog";
            this.Text = "Update";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ReaLTaiizor.Controls.ForeverListBox foreverListBox1;
        private ReaLTaiizor.Controls.ParrotSplashScreen parrotSplashScreen1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private ReaLTaiizor.Controls.ForeverButton foreverButton1;
    }
}