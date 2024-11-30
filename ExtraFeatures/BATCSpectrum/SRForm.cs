using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace opentuner
{
    internal class SRForm : Form
    {
        private Button button25;
        private Button button33;
        private Button button66;
        private Button button125;
        private Button button250;
        private Button button333;
        private Button button500;
        private Button button1000;
        private Button button1500;
        private Label label1;
        private Label label2;
        private Label label3;
        uint qrg = 0;
        uint SR = 0;

        public SRForm(uint _qrg)
        {
            qrg = _qrg;
            InitializeComponent();
            label3.Text = (qrg / 1000.0f).ToString() + " MHz";

        }

        private void InitializeComponent()
        {
            this.button25 = new System.Windows.Forms.Button();
            this.button33 = new System.Windows.Forms.Button();
            this.button66 = new System.Windows.Forms.Button();
            this.button125 = new System.Windows.Forms.Button();
            this.button250 = new System.Windows.Forms.Button();
            this.button333 = new System.Windows.Forms.Button();
            this.button500 = new System.Windows.Forms.Button();
            this.button1000 = new System.Windows.Forms.Button();
            this.button1500 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button25
            // 
            this.button25.Enabled = false;
            this.button25.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button25.Location = new System.Drawing.Point(13, 43);
            this.button25.Name = "button25";
            this.button25.Size = new System.Drawing.Size(50, 23);
            this.button25.TabIndex = 1;
            this.button25.Text = "25";
            this.button25.UseVisualStyleBackColor = true;
            this.button25.Visible = false;
            this.button25.Click += new System.EventHandler(this.button2_Click);
            // 
            // button33
            // 
            this.button33.Enabled = false;
            this.button33.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button33.Location = new System.Drawing.Point(69, 42);
            this.button33.Name = "button33";
            this.button33.Size = new System.Drawing.Size(50, 23);
            this.button33.TabIndex = 2;
            this.button33.Text = "33";
            this.button33.UseVisualStyleBackColor = true;
            this.button33.Visible = false;
            this.button33.Click += new System.EventHandler(this.button3_Click);
            // 
            // button66
            // 
            this.button66.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button66.Location = new System.Drawing.Point(125, 42);
            this.button66.Name = "button66";
            this.button66.Size = new System.Drawing.Size(50, 23);
            this.button66.TabIndex = 3;
            this.button66.Text = "66";
            this.button66.UseVisualStyleBackColor = true;
            this.button66.Click += new System.EventHandler(this.button4_Click);
            // 
            // button125
            // 
            this.button125.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button125.Location = new System.Drawing.Point(13, 72);
            this.button125.Name = "button125";
            this.button125.Size = new System.Drawing.Size(50, 23);
            this.button125.TabIndex = 4;
            this.button125.Text = "125";
            this.button125.UseVisualStyleBackColor = true;
            this.button125.Click += new System.EventHandler(this.button5_Click);
            // 
            // button250
            // 
            this.button250.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button250.Location = new System.Drawing.Point(69, 71);
            this.button250.Name = "button250";
            this.button250.Size = new System.Drawing.Size(50, 23);
            this.button250.TabIndex = 5;
            this.button250.Text = "250";
            this.button250.UseVisualStyleBackColor = true;
            this.button250.Click += new System.EventHandler(this.button6_Click);
            // 
            // button333
            // 
            this.button333.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button333.Location = new System.Drawing.Point(125, 71);
            this.button333.Name = "button333";
            this.button333.Size = new System.Drawing.Size(50, 23);
            this.button333.TabIndex = 6;
            this.button333.Text = "333";
            this.button333.UseVisualStyleBackColor = true;
            this.button333.Click += new System.EventHandler(this.button7_Click);
            // 
            // button500
            // 
            this.button500.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button500.Location = new System.Drawing.Point(13, 101);
            this.button500.Name = "button500";
            this.button500.Size = new System.Drawing.Size(50, 23);
            this.button500.TabIndex = 7;
            this.button500.Text = "500";
            this.button500.UseVisualStyleBackColor = true;
            this.button500.Click += new System.EventHandler(this.button8_Click);
            // 
            // button1000
            // 
            this.button1000.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1000.Location = new System.Drawing.Point(69, 100);
            this.button1000.Name = "button1000";
            this.button1000.Size = new System.Drawing.Size(50, 23);
            this.button1000.TabIndex = 8;
            this.button1000.Text = "1000";
            this.button1000.UseVisualStyleBackColor = true;
            this.button1000.Click += new System.EventHandler(this.button9_Click);
            // 
            // button1500
            // 
            this.button1500.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1500.Location = new System.Drawing.Point(125, 100);
            this.button1500.Name = "button1500";
            this.button1500.Size = new System.Drawing.Size(50, 23);
            this.button1500.TabIndex = 9;
            this.button1500.Text = "1500";
            this.button1500.UseVisualStyleBackColor = true;
            this.button1500.Click += new System.EventHandler(this.button10_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(29, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 20);
            this.label1.TabIndex = 10;
            this.label1.Text = "Select SR";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(13, 137);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 20);
            this.label2.TabIndex = 11;
            this.label2.Text = "QRG:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(65, 137);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 20);
            this.label3.TabIndex = 12;
            // 
            // SRForm
            // 
            this.ClientSize = new System.Drawing.Size(188, 175);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button25);
            this.Controls.Add(this.button33);
            this.Controls.Add(this.button66);
            this.Controls.Add(this.button125);
            this.Controls.Add(this.button250);
            this.Controls.Add(this.button333);
            this.Controls.Add(this.button500);
            this.Controls.Add(this.button1000);
            this.Controls.Add(this.button1500);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SRForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        public uint getsr()
        {
            return SR;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SR = 0;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SR = Convert.ToUInt16((sender as Button).Text);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SR = Convert.ToUInt16((sender as Button).Text);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SR = Convert.ToUInt16((sender as Button).Text);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SR = Convert.ToUInt16((sender as Button).Text);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SR = Convert.ToUInt16((sender as Button).Text);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SR = Convert.ToUInt16((sender as Button).Text);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            SR = Convert.ToUInt16((sender as Button).Text);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            SR = Convert.ToUInt16((sender as Button).Text);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            SR = Convert.ToUInt16((sender as Button).Text);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
