namespace DataCenterUnpack
{
    partial class UnpackForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Go = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.Key = new System.Windows.Forms.TextBox();
            this.IV = new System.Windows.Forms.TextBox();
            this.InputFile = new System.Windows.Forms.TextBox();
            this.BrowseInput = new System.Windows.Forms.Button();
            this.FindKeyButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Key";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "IV";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Input";
            // 
            // Go
            // 
            this.Go.Location = new System.Drawing.Point(15, 90);
            this.Go.Name = "Go";
            this.Go.Size = new System.Drawing.Size(75, 23);
            this.Go.TabIndex = 4;
            this.Go.Text = "go";
            this.Go.UseVisualStyleBackColor = true;
            this.Go.Click += new System.EventHandler(this.Go_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "Date Center|DataCenter_Final_*.dat|All Files|*.*";
            // 
            // Key
            // 
            this.Key.Location = new System.Drawing.Point(58, 19);
            this.Key.Name = "Key";
            this.Key.Size = new System.Drawing.Size(329, 20);
            this.Key.TabIndex = 5;
            // 
            // IV
            // 
            this.IV.Location = new System.Drawing.Point(58, 42);
            this.IV.Name = "IV";
            this.IV.Size = new System.Drawing.Size(329, 20);
            this.IV.TabIndex = 6;
            // 
            // InputFile
            // 
            this.InputFile.Location = new System.Drawing.Point(58, 64);
            this.InputFile.Name = "InputFile";
            this.InputFile.Size = new System.Drawing.Size(296, 20);
            this.InputFile.TabIndex = 7;
            // 
            // BrowseInput
            // 
            this.BrowseInput.Location = new System.Drawing.Point(360, 64);
            this.BrowseInput.Name = "BrowseInput";
            this.BrowseInput.Size = new System.Drawing.Size(27, 22);
            this.BrowseInput.TabIndex = 9;
            this.BrowseInput.Text = "...";
            this.BrowseInput.UseVisualStyleBackColor = true;
            this.BrowseInput.Click += new System.EventHandler(this.BrowseInput_Click);
            // 
            // FindKeyButton
            // 
            this.FindKeyButton.Location = new System.Drawing.Point(96, 90);
            this.FindKeyButton.Name = "FindKeyButton";
            this.FindKeyButton.Size = new System.Drawing.Size(75, 23);
            this.FindKeyButton.TabIndex = 11;
            this.FindKeyButton.Text = "Find Key";
            this.FindKeyButton.UseVisualStyleBackColor = true;
            this.FindKeyButton.Click += new System.EventHandler(this.FindKeyButton_Click);
            // 
            // UnpackForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(399, 121);
            this.Controls.Add(this.FindKeyButton);
            this.Controls.Add(this.BrowseInput);
            this.Controls.Add(this.InputFile);
            this.Controls.Add(this.IV);
            this.Controls.Add(this.Key);
            this.Controls.Add(this.Go);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "UnpackForm";
            this.Text = "DateCenter Unpacker";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button Go;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.TextBox Key;
        private System.Windows.Forms.TextBox IV;
        private System.Windows.Forms.TextBox InputFile;
        private System.Windows.Forms.Button BrowseInput;
        private System.Windows.Forms.Button FindKeyButton;
    }
}

