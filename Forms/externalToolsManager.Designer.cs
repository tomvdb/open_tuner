namespace opentuner
{
    partial class externalToolsManager
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
            this.listTools = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblToolName = new System.Windows.Forms.Label();
            this.lblToolPath = new System.Windows.Forms.Label();
            this.lblToolParameters = new System.Windows.Forms.Label();
            this.lblEnableUDP1 = new System.Windows.Forms.Label();
            this.lblUDP2 = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listTools
            // 
            this.listTools.FormattingEnabled = true;
            this.listTools.Location = new System.Drawing.Point(12, 12);
            this.listTools.Name = "listTools";
            this.listTools.Size = new System.Drawing.Size(293, 420);
            this.listTools.TabIndex = 0;
            this.listTools.SelectedIndexChanged += new System.EventHandler(this.listTools_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblUDP2);
            this.groupBox1.Controls.Add(this.lblEnableUDP1);
            this.groupBox1.Controls.Add(this.lblToolParameters);
            this.groupBox1.Controls.Add(this.lblToolPath);
            this.groupBox1.Controls.Add(this.lblToolName);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(311, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(333, 183);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "External Tool Properties";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Tool Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Tool Path:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Tool Parameters:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 112);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Enable UDP Tuner 1:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(22, 141);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(109, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Enable UDP Tuner 2:";
            // 
            // lblToolName
            // 
            this.lblToolName.AutoSize = true;
            this.lblToolName.Location = new System.Drawing.Point(149, 33);
            this.lblToolName.Name = "lblToolName";
            this.lblToolName.Size = new System.Drawing.Size(10, 13);
            this.lblToolName.TabIndex = 5;
            this.lblToolName.Text = " ";
            // 
            // lblToolPath
            // 
            this.lblToolPath.AutoSize = true;
            this.lblToolPath.Location = new System.Drawing.Point(149, 58);
            this.lblToolPath.Name = "lblToolPath";
            this.lblToolPath.Size = new System.Drawing.Size(10, 13);
            this.lblToolPath.TabIndex = 6;
            this.lblToolPath.Text = " ";
            // 
            // lblToolParameters
            // 
            this.lblToolParameters.AutoSize = true;
            this.lblToolParameters.Location = new System.Drawing.Point(149, 84);
            this.lblToolParameters.Name = "lblToolParameters";
            this.lblToolParameters.Size = new System.Drawing.Size(10, 13);
            this.lblToolParameters.TabIndex = 7;
            this.lblToolParameters.Text = " ";
            // 
            // lblEnableUDP1
            // 
            this.lblEnableUDP1.AutoSize = true;
            this.lblEnableUDP1.Location = new System.Drawing.Point(149, 112);
            this.lblEnableUDP1.Name = "lblEnableUDP1";
            this.lblEnableUDP1.Size = new System.Drawing.Size(10, 13);
            this.lblEnableUDP1.TabIndex = 8;
            this.lblEnableUDP1.Text = " ";
            // 
            // lblUDP2
            // 
            this.lblUDP2.AutoSize = true;
            this.lblUDP2.Location = new System.Drawing.Point(149, 141);
            this.lblUDP2.Name = "lblUDP2";
            this.lblUDP2.Size = new System.Drawing.Size(10, 13);
            this.lblUDP2.TabIndex = 9;
            this.lblUDP2.Text = " ";
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(569, 201);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(488, 201);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 3;
            this.btnEdit.Text = "&Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(407, 201);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 4;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(569, 409);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "&Close";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // externalToolsManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(656, 450);
            this.ControlBox = false;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.listTools);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "externalToolsManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "External Tools Manager";
            this.Load += new System.EventHandler(this.externalToolsManager_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listTools;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblUDP2;
        private System.Windows.Forms.Label lblEnableUDP1;
        private System.Windows.Forms.Label lblToolParameters;
        private System.Windows.Forms.Label lblToolPath;
        private System.Windows.Forms.Label lblToolName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button button1;
    }
}