using System.IO;
using System.Windows.Forms;

namespace ProjectRPL2_
{
    public partial class FormLogin : Form
    {
        public string penggunaAktif { get; private set; } = string.Empty;

        public FormLogin()
        {
            InitializeComponent();
            buatTataLetakLogin();
            AcceptButton = tombolLogin;
            CancelButton = null;
        }

        private void buatTataLetakLogin()
        {
            Text = "SMKMART | Masuk";
            BackColor = TampilanModern.Latar;
            ForeColor = TampilanModern.Teks;
            Font = new Font("Segoe UI", 10F);
            Rectangle areaKerja = Screen.FromPoint(Cursor.Position).WorkingArea;
            int lebarMaks = Math.Max(320, areaKerja.Width - 32);
            int tinggiMaks = Math.Max(320, areaKerja.Height - 48);
            ClientSize = new Size(Math.Min(980, lebarMaks), Math.Min(620, tinggiMaks));
            MinimumSize = new Size(Math.Min(560, lebarMaks), Math.Min(440, tinggiMaks));
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;

            Controls.Clear();
            inputUsername.Dock = DockStyle.Fill;
            inputUsername.Tag = "permukaan";
            inputUsername.Font = new Font("Segoe UI", 12F);
            inputUsername.Margin = Padding.Empty;
            inputUsername.Height = 42;
            inputPassword.Dock = DockStyle.Fill;
            inputPassword.Tag = "permukaan";
            inputPassword.Font = new Font("Segoe UI", 12F);
            inputPassword.Margin = Padding.Empty;
            inputPassword.Height = 42;
            tombolLogin.Dock = DockStyle.Fill;
            tombolLogin.Tag = "aksen";
            tombolLogin.Margin = Padding.Empty;
            tombolLogin.BackColor = TampilanModern.Aksen;
            tombolLogin.ForeColor = Color.White;
            tombolLogin.FlatStyle = FlatStyle.Flat;
            tombolLogin.FlatAppearance.BorderSize = 0;
            tombolLogin.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            tombolBelumPunyaAkun.Text = "Belum punya akun? Hubungi admin";
            tombolBelumPunyaAkun.Dock = DockStyle.Fill;
            tombolBelumPunyaAkun.Tag = "tautan";
            tombolBelumPunyaAkun.FlatStyle = FlatStyle.Flat;
            tombolBelumPunyaAkun.FlatAppearance.BorderSize = 0;
            tombolBelumPunyaAkun.BackColor = Color.Transparent;
            tombolBelumPunyaAkun.ForeColor = TampilanModern.Aksen;
            tombolBelumPunyaAkun.Margin = Padding.Empty;

            var panelKiri = new Panel { Name = "panelFormLogin", Dock = DockStyle.Fill, BackColor = TampilanModern.Permukaan, Tag = "permukaan", Padding = new Padding(52, 60, 52, 40) };
            var tataLetakForm = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 7, Margin = Padding.Empty,
                BackColor = TampilanModern.Permukaan, Tag = "permukaan"
            };
            tataLetakForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
            tataLetakForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
            tataLetakForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            tataLetakForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            tataLetakForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
            tataLetakForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            tataLetakForm.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            labelJudulLogin.Text = "Masuk ke SMKMART";
            labelJudulLogin.Font = new Font("Segoe UI", 23F, FontStyle.Bold);
            labelJudulLogin.AutoSize = false;
            labelJudulLogin.AutoEllipsis = true;
            labelJudulLogin.TextAlign = ContentAlignment.MiddleLeft;
            labelJudulLogin.ForeColor = TampilanModern.Teks;
            labelJudulLogin.Dock = DockStyle.Fill;
            labelJudulLogin.Margin = Padding.Empty;
            labelPanduanLogin.Text = "Masukkan username dan password akun Admin atau Kasir. Ketik 0 pada keduanya untuk keluar.";
            labelPanduanLogin.Font = new Font("Segoe UI", 10F);
            labelPanduanLogin.AutoSize = false;
            labelPanduanLogin.AutoEllipsis = false;
            labelPanduanLogin.TextAlign = ContentAlignment.TopLeft;
            labelPanduanLogin.ForeColor = TampilanModern.TeksSekunder;
            labelPanduanLogin.Tag = "sekunder";
            labelPanduanLogin.Dock = DockStyle.Fill;
            labelPanduanLogin.Margin = Padding.Empty;
            tataLetakForm.Controls.Add(labelJudulLogin, 0, 0);
            tataLetakForm.Controls.Add(labelPanduanLogin, 0, 1);
            tataLetakForm.Controls.Add(inputUsername, 0, 2);
            tataLetakForm.Controls.Add(inputPassword, 0, 3);
            tataLetakForm.Controls.Add(tombolLogin, 0, 4);
            tataLetakForm.Controls.Add(tombolBelumPunyaAkun, 0, 5);
            panelKiri.Controls.Add(tataLetakForm);
            var tombolTemaLogin = TampilanModern.buatTombolTema("tombolTemaLogin");
            tombolTemaLogin.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            tombolTemaLogin.Location = new Point(panelKiri.ClientSize.Width - tombolTemaLogin.Width - 18, 18);
            panelKiri.Resize += (_, _) => tombolTemaLogin.Location = new Point(panelKiri.ClientSize.Width - tombolTemaLogin.Width - 18, 18);
            panelKiri.Controls.Add(tombolTemaLogin);

            panelInformasiLogin.Controls.Clear();
            panelInformasiLogin.Dock = DockStyle.Fill;
            panelInformasiLogin.Location = Point.Empty;
            panelInformasiLogin.BackColor = TampilanModern.Aksen;
            panelInformasiLogin.Tag = "panelAksen";
            panelInformasiLogin.Padding = new Padding(42);
            labelSelamatDatang.Text = "Selamat datang";
            labelSelamatDatang.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            labelSelamatDatang.AutoSize = false;
            labelSelamatDatang.ForeColor = Color.White;
            labelSelamatDatang.Tag = "teksPutih";
            labelSelamatDatang.Dock = DockStyle.Top;
            labelSelamatDatang.Height = 74;
            labelInformasiLogin.Text = "Sistem pengelolaan gudang dan penjualan SMKMART.\r\n\r\nMasuk untuk melanjutkan ke panel sesuai peran akun.";
            labelInformasiLogin.Font = new Font("Segoe UI", 11F);
            labelInformasiLogin.AutoSize = false;
            labelInformasiLogin.ForeColor = Color.FromArgb(235, 237, 255);
            labelInformasiLogin.Tag = "teksPutih";
            labelInformasiLogin.Dock = DockStyle.Top;
            labelInformasiLogin.Height = 180;
            labelInformasiLogin.TextAlign = ContentAlignment.TopLeft;
            panelInformasiLogin.Controls.Add(labelInformasiLogin);
            panelInformasiLogin.Controls.Add(labelSelamatDatang);

            var tataLetakUtama = new TableLayoutPanel { Name = "tataLetakLogin", Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, BackColor = TampilanModern.Latar };
            tataLetakUtama.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));
            tataLetakUtama.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));
            tataLetakUtama.Controls.Add(panelKiri, 0, 0);
            tataLetakUtama.Controls.Add(panelInformasiLogin, 1, 0);
            Controls.Add(tataLetakUtama);
            void sesuaikanKolomLogin()
            {
                bool satuKolom = ClientSize.Width < 900;
                tataLetakUtama.ColumnStyles[0].SizeType = SizeType.Percent;
                tataLetakUtama.ColumnStyles[0].Width = satuKolom ? 100 : 58;
                tataLetakUtama.ColumnStyles[1].SizeType = SizeType.Percent;
                tataLetakUtama.ColumnStyles[1].Width = satuKolom ? 0 : 42;
                panelInformasiLogin.Visible = !satuKolom;
                panelKiri.Padding = satuKolom ? new Padding(28, 58, 28, 28) : new Padding(52, 60, 52, 40);
            }
            SizeChanged += (_, _) => sesuaikanKolomLogin();
            sesuaikanKolomLogin();
            TampilanModern.aturPaletForm(this);
            inputUsername.Focus();
        }

        private void tombolLogin_Click(object sender, EventArgs e)
        {
            string namaPengguna = inputUsername.Text.Trim();
            string kataSandi = inputPassword.Text;
            if (namaPengguna == "0" && kataSandi == "0")
            {
                Application.Exit();
                return;
            }
            if (namaPengguna.Length == 0 || kataSandi.Length == 0)
            {
                MessageBox.Show("Username dan password wajib diisi.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PengelolaAkun.Pengguna? pengguna;
            try
            {
                pengguna = PengelolaAkun.validasiLogin(namaPengguna, kataSandi);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show($"Berkas User.txt tidak ditemukan di folder data:\n{PenyimpananGudang.folderData}", "Data akun", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Berkas User.txt tidak dapat dibaca.\n{ex.Message}", "Data akun", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Aplikasi tidak memiliki izin untuk membaca User.txt.", "Data akun", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (pengguna is null)
            {
                MessageBox.Show("Username atau password salah atau peran akun tidak valid.", "Login gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                inputPassword.Clear();
                inputPassword.Focus();
                return;
            }

            penggunaAktif = pengguna.NamaPengguna;
            Form formTujuan = pengguna.Peran switch
            {
                "admin" => new FormAdmin(penggunaAktif),
                "kasir" => new FormKasir(penggunaAktif),
                _ => throw new InvalidOperationException("Peran akun tidak didukung.")
            };

            formTujuan.FormClosed += (_, _) =>
            {
                inputUsername.Clear();
                inputPassword.Clear();
                Show();
                Activate();
            };
            // Do not set the hidden login window as owner. Some Windows configurations
            // hide owned windows along with their owner, which makes the role form look stuck.
            Hide();
            formTujuan.Show();
        }

        private void tombolBelumPunyaAkun_Click(object sender, EventArgs e) =>
            MessageBox.Show("Silakan hubungi admin untuk mendapatkan akun aplikasi.", "Informasi akun", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}




