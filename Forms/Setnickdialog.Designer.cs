namespace opentuner.Forms
{
    partial class Setnickdialog
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
            this.txtNick = new System.Windows.Forms.TextBox();
            this.btnSetNick = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtNick
            // 
            this.txtNick.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNick.Location = new System.Drawing.Point(12, 12);
            this.txtNick.Name = "txtNick";
            this.txtNick.Size = new System.Drawing.Size(429, 26);
            this.txtNick.TabIndex = 0;
            // 
            // btnSetNick
            // 
            this.btnSetNick.Location = new System.Drawing.Point(366, 44);
            this.btnSetNick.Name = "btnSetNick";
            this.btnSetNick.Size = new System.Drawing.Size(75, 23);
            this.btnSetNick.TabIndex = 1;
            this.btnSetNick.Text = "Set Nick";
            this.btnSetNick.UseVisualStyleBackColor = true;
            this.btnSetNick.Click += new System.EventHandler(this.btnSetNick_Click);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(285, 44);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // setnickdialog
            // 
            this.AcceptButton = this.btnSetNick;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(456, 81);
            this.ControlBox = false;
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnSetNick);
            this.Controls.Add(this.txtNick);
            this.Name = "Setnickdialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Wideband Chat Nickname";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnSetNick;
        private System.Windows.Forms.Button button2;
        public System.Windows.Forms.TextBox txtNick;
    }
}