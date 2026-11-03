using System.Windows.Forms;

namespace ProjectRPL2_
{
    public partial class AdminForm : Form
    {
        private readonly ListBox productList = new() { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        private List<WarehouseStore.Product> products = [];

        public AdminForm(string username)
        {
            InitializeComponent();
            Text = $"SMKMART - Admin ({username})";

            var actions = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 46, Padding = new Padding(6) };
            var refresh = new Button { Text = "Muat ulang", AutoSize = true };
            var restock = new Button { Text = "Restok produk terpilih", AutoSize = true };
            var warehouseLog = new Button { Text = "Riwayat gudang", AutoSize = true };
            var salesLog = new Button { Text = "Riwayat transaksi", AutoSize = true };
            refresh.Click += (_, _) => LoadProducts();
            restock.Click += (_, _) => RestockSelected();
            warehouseLog.Click += (_, _) => RoleDashboard.ShowHistory(this, "Riwayat Gudang", warehouse: true);
            salesLog.Click += (_, _) => RoleDashboard.ShowHistory(this, "Riwayat Transaksi", warehouse: false);
            actions.Controls.AddRange([refresh, restock, warehouseLog, salesLog]);
            Controls.Add(productList);
            Controls.Add(actions);
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                products = WarehouseStore.GetProducts();
                productList.Items.Clear();
                foreach (var product in products)
                    productList.Items.Add(RoleDashboard.FormatProduct(product));
            }
            catch (Exception ex)
            {
                RoleDashboard.ShowDataError(this, ex);
            }
        }

        private void RestockSelected()
        {
            int index = productList.SelectedIndex;
            if (index < 0 || index >= products.Count)
            {
                MessageBox.Show(this, "Pilih produk yang akan direstok.", "Restok", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var product = products[index];
            string? input = RoleDashboard.Ask($"Jumlah restok untuk {product.Name}:", "Restok Produk");
            if (input is null) return;
            if (!int.TryParse(input, out int amount) || amount <= 0)
            {
                MessageBox.Show(this, "Masukkan jumlah bilangan bulat lebih dari nol.", "Input tidak valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool success = WarehouseStore.Restock(product.Id, amount, out string message);
                MessageBox.Show(this, message, success ? "Berhasil" : "Restok gagal", MessageBoxButtons.OK,
                    success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                if (success) LoadProducts();
            }
            catch (Exception ex)
            {
                RoleDashboard.ShowDataError(this, ex);
            }
        }
    }
}
