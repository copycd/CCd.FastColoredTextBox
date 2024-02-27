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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressForm));
            fastColoredTextBox1 = new FastColoredTextBoxNS.FastColoredTextBox();
            backgroundWorker_DisplayLog = new System.ComponentModel.BackgroundWorker();
            progressBar1 = new System.Windows.Forms.ProgressBar();
            label_FailureCount = new System.Windows.Forms.Label();
            label_Index = new System.Windows.Forms.Label();
            label_InstantMsg = new System.Windows.Forms.Label();
            button_Cancel = new System.Windows.Forms.Button();
            check_writeInstantMsg = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)fastColoredTextBox1).BeginInit();
            SuspendLayout();
            // 
            // fastColoredTextBox1
            // 
            fastColoredTextBox1.AllowDrop = false;
            fastColoredTextBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            fastColoredTextBox1.AutoCompleteBracketsList = (new char[] { '(', ')', '{', '}', '[', ']', '"', '"', '\'', '\'' });
            fastColoredTextBox1.AutoIndentCharsPatterns = "\r\n^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;=]+);\r\n^\\s*(case|default)\\s*[^:]*(?<range>:)\\s*(?<range>[^;]+);\r\n";
            fastColoredTextBox1.AutoScrollMinSize = new System.Drawing.Size(27, 14);
            fastColoredTextBox1.BackBrush = null;
            fastColoredTextBox1.BracketsHighlightStrategy = FastColoredTextBoxNS.BracketsHighlightStrategy.Strategy2;
            fastColoredTextBox1.CharHeight = 14;
            fastColoredTextBox1.CharWidth = 8;
            fastColoredTextBox1.Cursor = System.Windows.Forms.Cursors.IBeam;
            fastColoredTextBox1.DisabledColor = System.Drawing.Color.FromArgb(100, 180, 180, 180);
            fastColoredTextBox1.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            fastColoredTextBox1.IsReplaceMode = false;
            fastColoredTextBox1.Language = FastColoredTextBoxNS.Language.CSharp;
            fastColoredTextBox1.LeftBracket = '(';
            fastColoredTextBox1.LeftBracket2 = '{';
            fastColoredTextBox1.Location = new System.Drawing.Point(12, 71);
            fastColoredTextBox1.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            fastColoredTextBox1.Name = "fastColoredTextBox1";
            fastColoredTextBox1.Paddings = new System.Windows.Forms.Padding(0);
            fastColoredTextBox1.ReadOnly = true;
            fastColoredTextBox1.RightBracket = ')';
            fastColoredTextBox1.RightBracket2 = '}';
            fastColoredTextBox1.SelectionColor = System.Drawing.Color.FromArgb(60, 0, 0, 255);
            fastColoredTextBox1.Size = new System.Drawing.Size(625, 205);
            fastColoredTextBox1.TabIndex = 60;
            fastColoredTextBox1.Zoom = 100;
            // 
            // backgroundWorker_DisplayLog
            // 
            backgroundWorker_DisplayLog.DoWork += backgroundWorker_DisplayLog_DoWork;
            // 
            // progressBar1
            // 
            progressBar1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            progressBar1.Location = new System.Drawing.Point(12, 32);
            progressBar1.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new System.Drawing.Size(625, 12);
            progressBar1.Step = 1;
            progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            progressBar1.TabIndex = 0;
            // 
            // label_FailureCount
            // 
            label_FailureCount.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label_FailureCount.AutoSize = true;
            label_FailureCount.Location = new System.Drawing.Point(531, 13);
            label_FailureCount.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label_FailureCount.Name = "label_FailureCount";
            label_FailureCount.Size = new System.Drawing.Size(60, 15);
            label_FailureCount.TabIndex = 61;
            label_FailureCount.Text = "Failure : 0";
            // 
            // label_Index
            // 
            label_Index.AutoSize = true;
            label_Index.Location = new System.Drawing.Point(12, 14);
            label_Index.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label_Index.Name = "label_Index";
            label_Index.Size = new System.Drawing.Size(34, 15);
            label_Index.TabIndex = 1;
            label_Index.Text = "0 / 1";
            // 
            // label_InstantMsg
            // 
            label_InstantMsg.AutoSize = true;
            label_InstantMsg.Location = new System.Drawing.Point(12, 48);
            label_InstantMsg.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label_InstantMsg.Name = "label_InstantMsg";
            label_InstantMsg.Size = new System.Drawing.Size(37, 15);
            label_InstantMsg.TabIndex = 62;
            label_InstantMsg.Text = "msg :";
            // 
            // button_Cancel
            // 
            button_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            button_Cancel.Location = new System.Drawing.Point(512, 284);
            button_Cancel.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            button_Cancel.Name = "button_Cancel";
            button_Cancel.Size = new System.Drawing.Size(126, 29);
            button_Cancel.TabIndex = 2;
            button_Cancel.Text = "Cancel";
            button_Cancel.UseVisualStyleBackColor = true;
            button_Cancel.Click += button_Cancel_Click;
            // 
            // check_writeInstantMsg
            // 
            check_writeInstantMsg.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            check_writeInstantMsg.AutoSize = true;
            check_writeInstantMsg.Location = new System.Drawing.Point(30, 290);
            check_writeInstantMsg.Name = "check_writeInstantMsg";
            check_writeInstantMsg.Size = new System.Drawing.Size(142, 19);
            check_writeInstantMsg.TabIndex = 63;
            check_writeInstantMsg.Text = "write instant message";
            check_writeInstantMsg.UseVisualStyleBackColor = true;
            check_writeInstantMsg.CheckedChanged += check_writeInstantMsg_CheckedChanged;
            // 
            // ProgressForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(652, 326);
            Controls.Add(check_writeInstantMsg);
            Controls.Add(button_Cancel);
            Controls.Add(label_InstantMsg);
            Controls.Add(label_Index);
            Controls.Add(label_FailureCount);
            Controls.Add(progressBar1);
            Controls.Add(fastColoredTextBox1);
            Name = "ProgressForm";
            Text = "ProgressForm";
            Load += ProgressForm_Load;
            FormClosing += ProgressForm_FormClosing;
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