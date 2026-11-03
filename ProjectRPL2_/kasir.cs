using System.Globalization;
using System.Windows.Forms;

namespace ProjectRPL2_
{
    public partial class KasirForm : Form
    {
        private readonly string username;
        private readonly ListBox productList = new() { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        private List<WarehouseStore.Product> products = [];

        public KasirForm(string username)
        {
            this.username = username;
            InitializeComponent();
            Text = $"SMKMART - Kasir ({username})";

            var actions = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 46, Padding = new Padding(6) };
            var refresh = new Button { Text = "Muat ulang produk", AutoSize = true };
            var checkout = new Button { Text = "Proses penjualan", AutoSize = true };
            refresh.Click += (_, _) => LoadProducts();
            checkout.Click += (_, _) => CheckoutSelected();
            actions.Controls.AddRange([refresh, checkout]);
            Controls.Add(productList);
            Controls.Add(actions);
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                products = WarehouseStore.GetProducts(includeDeleted: false)
                    .Where(product => product.Status.Equals("aktif", StringComparison.OrdinalIgnoreCase))
                    .ToList();
                productList.Items.Clear();
                foreach (var product in products)
                    productList.Items.Add(RoleDashboard.FormatProduct(product));
            }
            catch (Exception ex)
            {
                RoleDashboard.ShowDataError(this, ex);
            }
        }

        private void CheckoutSelected()
        {
            int index = productList.SelectedIndex;
            if (index < 0 || index >= products.Count)
            {
                MessageBox.Show(this, "Pilih produk yang akan dijual.", "Penjualan", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var product = products[index];
            string? quantityText = RoleDashboard.Ask($"Jumlah {product.Name} (stok {product.Stock}):", "Jumlah Produk");
            if (quantityText is null) return;
            if (!int.TryParse(quantityText, out int quantity) || quantity <= 0)
            {
                MessageBox.Show(this, "Jumlah harus berupa bilangan bulat lebih dari nol.", "Input tidak valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string? method = RoleDashboard.Ask("Metode pembayaran: Cash, QRIS, atau Debit", "Pembayaran");
            if (string.IsNullOrWhiteSpace(method)) return;
            decimal paid = product.Price * quantity;
            if (method.Equals("Cash", StringComparison.OrdinalIgnoreCase))
            {
                string? paidText = RoleDashboard.Ask($"Total {WarehouseStore.Money(paid)}. Uang diterima:", "Pembayaran Tunai");
                if (paidText is null) return;
                if (!decimal.TryParse(paidText, NumberStyles.Number, CultureInfo.GetCultureInfo("id-ID"), out paid) || paid < 0)
                {
                    MessageBox.Show(this, "Nominal pembayaran tidak valid.", "Input tidak valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            try
            {
                bool success = WarehouseStore.Checkout(product.Id, quantity, username, method.Trim(), paid, out string message);
                MessageBox.Show(this, message, success ? "Transaksi berhasil" : "Transaksi gagal", MessageBoxButtons.OK,
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
