namespace ProjectRPL2_
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            panel1 = new Panel();
            label4 = new Label();
            button2 = new Button();
            label3 = new Label();
            label1 = new Label();
            label2 = new Label();
            usernme = new TextBox();
            button1 = new Button();
            textBox1 = new TextBox();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.Cyan;
            panel1.Controls.Add(label4);
            panel1.Controls.Add(button2);
            panel1.Controls.Add(label3);
            panel1.Location = new Point(391, -80);
            panel1.Name = "panel1";
            panel1.Size = new Size(407, 588);
            panel1.TabIndex = 0;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.None;
            label4.Font = new Font("Segoe UI", 8F);
            label4.ForeColor = Color.Black;
            label4.Location = new Point(3, 196);
            label4.Name = "label4";
            label4.Size = new Size(407, 227);
            label4.TabIndex = 4;
            label4.Text = resources.GetString("label4.Text");
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // button2
            // 
            button2.BackColor = Color.Cyan;
            button2.ForeColor = SystemColors.Control;
            button2.Location = new Point(94, 426);
            button2.Name = "button2";
            button2.Size = new Size(219, 36);
            button2.TabIndex = 3;
            button2.Tag = "";
            button2.Text = "Belum Punya Akun";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Sitka Subheading", 14F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label3.Location = new Point(94, 156);
            label3.Name = "label3";
            label3.Size = new Size(208, 40);
            label3.TabIndex = 2;
            label3.Text = "Selamat Datang";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Showcard Gothic", 20F, FontStyle.Bold);
            label1.Location = new Point(110, 79);
            label1.Name = "label1";
            label1.Size = new Size(142, 50);
            label1.TabIndex = 1;
            label1.Text = "LOGIN";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label2.Location = new Point(32, 333);
            label2.Name = "label2";
            label2.Size = new Size(326, 25);
            label2.TabIndex = 0;
            label2.Text = "Login dengan akun yang sudah tersedia";
            
            // 
            // usernme
            // 
            usernme.Location = new Point(88, 195);
            usernme.Name = "usernme";
            usernme.PasswordChar = '*';
            usernme.PlaceholderText = "Password";
            usernme.RightToLeft = RightToLeft.No;
            usernme.Size = new Size(190, 31);
            usernme.TabIndex = 2;
            // 
            // button1
            // 
            button1.BackColor = Color.Aqua;
            button1.ForeColor = SystemColors.InfoText;
            button1.Location = new Point(88, 248);
            button1.Name = "button1";
            button1.Size = new Size(190, 47);
            button1.TabIndex = 4;
            button1.Text = "Login";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(88, 149);
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "Username";
            textBox1.Size = new Size(190, 31);
            textBox1.TabIndex = 1;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Azure;
            ClientSize = new Size(800, 450);
            AcceptButton = button1;
            ControlBox = true;
            Controls.Add(textBox1);
            Controls.Add(button1);
            Controls.Add(usernme);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(panel1);
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Login kasis SMKMART";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private Label label2;
        private Label label1;
        private Label label3;
        private TextBox usernme;
        private Button button2;
        private Button button1;
        private TextBox textBox1;
        private Label label4;
    }
}
