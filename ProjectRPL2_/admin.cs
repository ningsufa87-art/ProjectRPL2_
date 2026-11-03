namespace ProjectRPL2_
{
    public partial class AdminForm : Form
    {
        public AdminForm(string username)
        {
            InitializeComponent();
            Text = "Dashboard Admin";
            RoleDashboard.AddWelcome(this, "Admin", username);
        }
    }
}
