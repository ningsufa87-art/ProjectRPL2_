namespace ProjectRPL2_
{
    internal static class RoleDashboard
    {
        internal static void AddWelcome(Form form, string role, string username)
        {
            form.TopMost = false;
            form.ControlBox = true;
            form.MinimizeBox = true;
            form.MaximizeBox = false;
            form.StartPosition = FormStartPosition.CenterScreen;

            var welcome = new Label
            {
                Text = $"Selamat datang, {username}!\r\nAnda masuk sebagai {role}.",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 16, FontStyle.Regular)
            };
            var close = new Button
            {
                Text = "Keluar / ganti akun",
                Dock = DockStyle.Bottom,
                Height = 44
            };
            close.Click += (_, _) => form.Close();
            form.Controls.Add(welcome);
            form.Controls.Add(close);
        }
    }
}
