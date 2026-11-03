namespace ProjectRPL2_
{
    partial class FormLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLogin));
            panelInformasiLogin = new Panel();
            labelInformasiLogin = new Label();
            tombolBelumPunyaAkun = new Button();
            labelSelamatDatang = new Label();
            labelJudulLogin = new Label();
            labelPanduanLogin = new Label();
            inputPassword = new TextBox();
            tombolLogin = new Button();
            inputUsername = new TextBox();
            panelInformasiLogin.SuspendLayout();
            SuspendLayout();
            // 
            // panelInformasiLogin
            // 
            panelInformasiLogin.BackColor = Color.Cyan;
            panelInformasiLogin.Controls.Add(labelInformasiLogin);
            panelInformasiLogin.Controls.Add(tombolBelumPunyaAkun);
            panelInformasiLogin.Controls.Add(labelSelamatDatang);
            panelInformasiLogin.Location = new Point(391, -80);
            panelInformasiLogin.Name = "panelInformasiLogin";
            panelInformasiLogin.Size = new Size(407, 588);
            panelInformasiLogin.TabIndex = 0;
            // 
            // labelInformasiLogin
            // 
            labelInformasiLogin.Anchor = AnchorStyles.None;
            labelInformasiLogin.Font = new Font("Segoe UI", 8F);
            labelInformasiLogin.ForeColor = Color.Black;
            labelInformasiLogin.Location = new Point(3, 196);
            labelInformasiLogin.Name = "labelInformasiLogin";
            labelInformasiLogin.Size = new Size(407, 227);
            labelInformasiLogin.TabIndex = 4;
            labelInformasiLogin.Text = resources.GetString("label4.Text");
            labelInformasiLogin.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tombolBelumPunyaAkun
            // 
            tombolBelumPunyaAkun.BackColor = Color.Cyan;
            tombolBelumPunyaAkun.ForeColor = SystemColors.Control;
            tombolBelumPunyaAkun.Location = new Point(94, 426);
            tombolBelumPunyaAkun.Name = "tombolBelumPunyaAkun";
            tombolBelumPunyaAkun.Size = new Size(219, 36);
            tombolBelumPunyaAkun.TabIndex = 3;
            tombolBelumPunyaAkun.Tag = "";
            tombolBelumPunyaAkun.Text = "Belum Punya Akun";
            tombolBelumPunyaAkun.UseVisualStyleBackColor = false;
            tombolBelumPunyaAkun.Click += tombolBelumPunyaAkun_Click;
            // 
            // labelSelamatDatang
            // 
            labelSelamatDatang.AutoSize = true;
            labelSelamatDatang.Font = new Font("Sitka Subheading", 14F, FontStyle.Italic, GraphicsUnit.Point, 0);
            labelSelamatDatang.Location = new Point(94, 156);
            labelSelamatDatang.Name = "labelSelamatDatang";
            labelSelamatDatang.Size = new Size(208, 40);
            labelSelamatDatang.TabIndex = 2;
            labelSelamatDatang.Text = "Selamat Datang";
            // 
            // labelJudulLogin
            // 
            labelJudulLogin.AutoSize = true;
            labelJudulLogin.Font = new Font("Showcard Gothic", 20F, FontStyle.Bold);
            labelJudulLogin.Location = new Point(110, 79);
            labelJudulLogin.Name = "labelJudulLogin";
            labelJudulLogin.Size = new Size(142, 50);
            labelJudulLogin.TabIndex = 1;
            labelJudulLogin.Text = "LOGIN";
            // 
            // labelPanduanLogin
            // 
            labelPanduanLogin.AutoSize = true;
            labelPanduanLogin.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point, 0);
            labelPanduanLogin.Location = new Point(32, 333);
            labelPanduanLogin.Name = "labelPanduanLogin";
            labelPanduanLogin.Size = new Size(326, 25);
            labelPanduanLogin.TabIndex = 0;
            labelPanduanLogin.Text = "Login dengan akun yang sudah tersedia";
            
            // 
            // inputPassword
            // 
            inputPassword.Location = new Point(88, 195);
            inputPassword.Name = "inputPassword";
            inputPassword.PasswordChar = '*';
            inputPassword.PlaceholderText = "Password";
            inputPassword.RightToLeft = RightToLeft.No;
            inputPassword.Size = new Size(190, 31);
            inputPassword.TabIndex = 2;
            // 
            // tombolLogin
            // 
            tombolLogin.BackColor = Color.Aqua;
            tombolLogin.ForeColor = SystemColors.InfoText;
            tombolLogin.Location = new Point(88, 248);
            tombolLogin.Name = "tombolLogin";
            tombolLogin.Size = new Size(190, 47);
            tombolLogin.TabIndex = 4;
            tombolLogin.Text = "Login";
            tombolLogin.UseVisualStyleBackColor = false;
            tombolLogin.Click += tombolLogin_Click;
            // 
            // inputUsername
            // 
            inputUsername.Location = new Point(88, 149);
            inputUsername.Name = "inputUsername";
            inputUsername.PlaceholderText = "Username";
            inputUsername.Size = new Size(190, 31);
            inputUsername.TabIndex = 1;
            // 
            // FormLogin
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Azure;
            ClientSize = new Size(800, 450);
            AcceptButton = tombolLogin;
            ControlBox = true;
            Controls.Add(inputUsername);
            Controls.Add(tombolLogin);
            Controls.Add(inputPassword);
            Controls.Add(labelPanduanLogin);
            Controls.Add(labelJudulLogin);
            Controls.Add(panelInformasiLogin);
            MaximizeBox = false;
            Name = "FormLogin";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Login SMKMART";
            panelInformasiLogin.ResumeLayout(false);
            panelInformasiLogin.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panelInformasiLogin;
        private Label labelPanduanLogin;
        private Label labelJudulLogin;
        private Label labelSelamatDatang;
        private TextBox inputPassword;
        private Button tombolBelumPunyaAkun;
        private Button tombolLogin;
        private TextBox inputUsername;
        private Label labelInformasiLogin;
    }
}




