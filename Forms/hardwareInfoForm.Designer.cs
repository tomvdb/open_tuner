namespace opentuner
{
    partial class hardwareInfoForm
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
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblDeviceDescription = new System.Windows.Forms.Label();
            this.lblDeviceIndex = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblDeviceSerialNumber = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(15, 22);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(303, 368);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(15, 396);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 9;
            this.button1.Text = "Refresh";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblDeviceSerialNumber);
            this.groupBox1.Controls.Add(this.lblDeviceDescription);
            this.groupBox1.Controls.Add(this.lblDeviceIndex);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(324, 22);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(353, 368);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Device Properties";
            // 
            // lblDeviceDescription
            // 
            this.lblDeviceDescription.AutoSize = true;
            this.lblDeviceDescription.Location = new System.Drawing.Point(150, 88);
            this.lblDeviceDescription.Name = "lblDeviceDescription";
            this.lblDeviceDescription.Size = new System.Drawing.Size(35, 13);
            this.lblDeviceDescription.TabIndex = 14;
            this.lblDeviceDescription.Text = "label8";
            // 
            // lblDeviceIndex
            // 
            this.lblDeviceIndex.AutoSize = true;
            this.lblDeviceIndex.Location = new System.Drawing.Point(150, 35);
            this.lblDeviceIndex.Name = "lblDeviceIndex";
            this.lblDeviceIndex.Size = new System.Drawing.Size(35, 13);
            this.lblDeviceIndex.TabIndex = 12;
            this.lblDeviceIndex.Text = "label6";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(26, 88);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Device Description:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(26, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Device Serial Number:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Device Index:";
            // 
            // lblDeviceSerialNumber
            // 
            this.lblDeviceSerialNumber.Location = new System.Drawing.Point(153, 58);
            this.lblDeviceSerialNumber.Name = "lblDeviceSerialNumber";
            this.lblDeviceSerialNumber.ReadOnly = true;
            this.lblDeviceSerialNumber.Size = new System.Drawing.Size(100, 20);
            this.lblDeviceSerialNumber.TabIndex = 15;
            // 
            // hardwareInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(689, 438);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "hardwareInfoForm";
            this.Text = "Detected Hardware Info";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblDeviceDescription;
        private System.Windows.Forms.Label lblDeviceIndex;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox lblDeviceSerialNumber;
    }
}