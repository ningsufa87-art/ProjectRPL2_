using System.IO;
using System.Windows.Forms;

namespace ProjectRPL2_
{
    public partial class FormLogin : Form
    {
        public string penggunaAktif { get; private set; } = string.Empty;

        public FormLogin() => InitializeComponent();

        private void tombolLogin_Click(object sender, EventArgs e)
        {
            string namaPengguna = inputUsername.Text.Trim();
            string kataSandi = inputPassword.Text;
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
                MessageBox.Show("Username atau password salah, akun ganda, atau peran akun tidak valid.", "Login gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            Hide();
            formTujuan.FormClosed += (_, _) =>
            {
                inputUsername.Clear();
                inputPassword.Clear();
                Show();
                Activate();
            };
            formTujuan.Show(this);
        }

        private void tombolBelumPunyaAkun_Click(object sender, EventArgs e) =>
            MessageBox.Show("Silakan hubungi admin untuk mendapatkan akun aplikasi.", "Informasi akun", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}




