namespace ProjectRPL2_
{
    public partial class KasirForm : Form
    {
        public KasirForm(string username)
        {
            InitializeComponent();
            Text = "Dashboard Kasir";
            RoleDashboard.AddWelcome(this, "Kasir", username);
        }
    }
}
