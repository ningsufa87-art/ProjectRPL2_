using System.IO;
using System.Windows.Forms;

namespace ProjectRPL2_
{
    public partial class Form1 : Form
    {
        public string Currentuser { get; private set; } = string.Empty;

        public Form1() => InitializeComponent();

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text.Trim();
            string password = usernme.Text;
            if (username.Length == 0 || password.Length == 0)
            {
                MessageBox.Show("Username dan password wajib diisi.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UserStore.User? user;
            try
            {
                user = UserStore.Authenticate(username, password);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show($"Berkas User.txt tidak ditemukan di folder data:\n{WarehouseStore.Root}", "Data akun", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            if (user is null)
            {
                MessageBox.Show("Username atau password salah, akun ganda, atau peran akun tidak valid.", "Login gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                usernme.Clear();
                usernme.Focus();
                return;
            }

            Currentuser = user.Username;
            Form destination = user.Role switch
            {
                "admin" => new AdminForm(Currentuser),
                "kasir" => new KasirForm(Currentuser),
                _ => throw new InvalidOperationException("Peran akun tidak didukung.")
            };

            Hide();
            destination.FormClosed += (_, _) =>
            {
                textBox1.Clear();
                usernme.Clear();
                Show();
                Activate();
            };
            destination.Show(this);
        }

        private void button2_Click(object sender, EventArgs e) =>
            MessageBox.Show("Silakan hubungi admin untuk mendapatkan akun aplikasi.", "Informasi akun", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
