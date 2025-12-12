namespace CCd.Wins.UI
{
    partial class ProgressForm
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

                // === 사용자 관리 리소스 ===
                DisposeManagedResources();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressForm));
            fastColoredTextBox1 = new FastColoredTextBoxNS.FastColoredTextBox();
            backgroundWorker_DisplayLog = new System.ComponentModel.BackgroundWorker();
            progressBar1 = new ProgressBar();
            label_FailureCount = new Label();
            label_Index = new Label();
            label_InstantMsg = new Label();
            button_Cancel = new Button();
            check_writeInstantMsg = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)fastColoredTextBox1).BeginInit();
            SuspendLayout();
            // 
            // fastColoredTextBox1
            // 
            fastColoredTextBox1.AllowDrop = false;
            fastColoredTextBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            fastColoredTextBox1.AutoIndentCharsPatterns = "\r\n^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;=]+);\r\n^\\s*(case|default)\\s*[^:]*(?<range>:)\\s*(?<range>[^;]+);\r\n";
            fastColoredTextBox1.BracketsHighlightStrategy = FastColoredTextBoxNS.BracketsHighlightStrategy.Strategy2;
            fastColoredTextBox1.Cursor = Cursors.IBeam;
            fastColoredTextBox1.DefaultMarkerSize = 8;
            fastColoredTextBox1.DisabledColor = Color.FromArgb(100, 180, 180, 180);
            fastColoredTextBox1.Hotkeys = resources.GetString("fastColoredTextBox1.Hotkeys");
            fastColoredTextBox1.Language = FastColoredTextBoxNS.Language.CSharp;
            fastColoredTextBox1.LeftBracket = '(';
            fastColoredTextBox1.LeftBracket2 = '{';
            fastColoredTextBox1.Location = new Point(12, 78);
            fastColoredTextBox1.Margin = new Padding(2, 4, 2, 4);
            fastColoredTextBox1.Name = "fastColoredTextBox1";
            fastColoredTextBox1.ReadOnly = true;
            fastColoredTextBox1.RightBracket = ')';
            fastColoredTextBox1.RightBracket2 = '}';
            fastColoredTextBox1.SelectionColor = Color.FromArgb(60, 0, 0, 255);
            fastColoredTextBox1.Size = new Size(625, 136);
            fastColoredTextBox1.TabIndex = 60;
            // 
            // backgroundWorker_DisplayLog
            // 
            backgroundWorker_DisplayLog.DoWork += backgroundWorker_DisplayLog_DoWork;
            // 
            // progressBar1
            // 
            progressBar1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            progressBar1.Location = new Point(12, 32);
            progressBar1.Margin = new Padding(2, 4, 2, 4);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(625, 12);
            progressBar1.Step = 1;
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.TabIndex = 0;
            // 
            // label_FailureCount
            // 
            label_FailureCount.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label_FailureCount.AutoSize = true;
            label_FailureCount.Location = new Point(531, 13);
            label_FailureCount.Margin = new Padding(2, 0, 2, 0);
            label_FailureCount.Name = "label_FailureCount";
            label_FailureCount.Size = new Size(60, 15);
            label_FailureCount.TabIndex = 61;
            label_FailureCount.Text = "Failure : 0";
            // 
            // label_Index
            // 
            label_Index.AutoSize = true;
            label_Index.Location = new Point(12, 14);
            label_Index.Margin = new Padding(2, 0, 2, 0);
            label_Index.Name = "label_Index";
            label_Index.Size = new Size(34, 15);
            label_Index.TabIndex = 1;
            label_Index.Text = "0 / 1";
            // 
            // label_InstantMsg
            // 
            label_InstantMsg.AutoSize = true;
            label_InstantMsg.Location = new Point(12, 48);
            label_InstantMsg.Margin = new Padding(2, 0, 2, 0);
            label_InstantMsg.Name = "label_InstantMsg";
            label_InstantMsg.Size = new Size(37, 15);
            label_InstantMsg.TabIndex = 62;
            label_InstantMsg.Text = "msg :";
            // 
            // button_Cancel
            // 
            button_Cancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button_Cancel.Location = new Point(511, 222);
            button_Cancel.Margin = new Padding(2, 4, 2, 4);
            button_Cancel.Name = "button_Cancel";
            button_Cancel.Size = new Size(126, 29);
            button_Cancel.TabIndex = 2;
            button_Cancel.Text = "Cancel";
            button_Cancel.UseVisualStyleBackColor = true;
            button_Cancel.Click += button_Cancel_Click;
            // 
            // check_writeInstantMsg
            // 
            check_writeInstantMsg.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            check_writeInstantMsg.AutoSize = true;
            check_writeInstantMsg.Location = new Point(12, 233);
            check_writeInstantMsg.Name = "check_writeInstantMsg";
            check_writeInstantMsg.Size = new Size(142, 19);
            check_writeInstantMsg.TabIndex = 63;
            check_writeInstantMsg.Text = "write instant message";
            check_writeInstantMsg.UseVisualStyleBackColor = true;
            check_writeInstantMsg.CheckedChanged += check_writeInstantMsg_CheckedChanged;
            // 
            // ProgressForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(652, 264);
            Controls.Add(check_writeInstantMsg);
            Controls.Add(button_Cancel);
            Controls.Add(label_InstantMsg);
            Controls.Add(label_Index);
            Controls.Add(label_FailureCount);
            Controls.Add(progressBar1);
            Controls.Add(fastColoredTextBox1);
            Name = "ProgressForm";
            Text = "ProgressForm";
            FormClosing += ProgressForm_FormClosing;
            Load += ProgressForm_Load;
            Shown += ProgressForm_Shown;
            ((System.ComponentModel.ISupportInitialize)fastColoredTextBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private FastColoredTextBoxNS.FastColoredTextBox fastColoredTextBox1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label_FailureCount;
        private System.Windows.Forms.Label label_Index;
        private System.ComponentModel.BackgroundWorker backgroundWorker_DisplayLog;
        private System.Windows.Forms.Label label_InstantMsg;
        private System.Windows.Forms.Button button_Cancel;
        private System.Windows.Forms.CheckBox check_writeInstantMsg;
    }
}