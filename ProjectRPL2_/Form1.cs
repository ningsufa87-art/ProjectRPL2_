using System;
using System.IO;
using System.Windows.Forms;

namespace ProjectRPL2_
{
    public partial class Form1 : Form
    {

        public static string CurrentDirectory = "";
        private object roleLogin;

        public Form1()
        {
            InitializeComponent();
        }

        public string Currentuser { get; private set; }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = usernme.Text.Trim();
            string password = textBox1.Text.Trim();

            if (username == "0" && password == "0")
            {
                Application.Exit();
                return;
            }

            if (!File.Exists("users.txt"))
            {
                MessageBox.Show("File users.txt tidak ditemukan!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool loginSuccess = false;  
            string rolelogin = "";

            foreach (var line in File.ReadLines("users.txt"))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split(',');
                if (parts.Length == 3)
                {
                    string fileUsername = parts[0].Trim();
                    string filePassword = parts[1].Trim();
                    string fileRole = parts[2].Trim();
                    if (username == fileUsername && password == filePassword)
                    {
                        loginSuccess = true;
                        rolelogin = fileRole;
                        break;
                    }
                }
            }
            if (!loginSuccess)
            {
                if (rolelogin == "admin")
                {
                    Admin adminForm = new Admin();
                    adminForm.Show();
                    this.Hide();
                }
                else if (rolelogin == "kasir")
                {
                    kasir kasirForm = new kasir();
                    kasirForm.Show();
                    this.Hide();
                }

            }

            Currentuser = username;
            MessageBox.Show($"Login berhasil! Selamat datang, {username} ({rolelogin})", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);



        }

    }
}
