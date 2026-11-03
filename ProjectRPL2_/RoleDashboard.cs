using System.Windows.Forms;

namespace ProjectRPL2_
{
    internal static class RoleDashboard
    {
        internal static string FormatProduct(WarehouseStore.Product product) =>
            $"{product.Id} | {product.Name} | {product.Category} | Stok {product.Stock} | " +
            $"{WarehouseStore.Money(product.Price)} | {product.Status}";

        internal static void ShowHistory(IWin32Window owner, string title, bool warehouse)
        {
            var viewer = new Form
            {
                Text = title,
                Width = 760,
                Height = 520,
                StartPosition = FormStartPosition.CenterParent
            };
            var text = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                WordWrap = false,
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10),
                Text = WarehouseStore.ReadHistory(warehouse)
            };
            viewer.Controls.Add(text);
            viewer.ShowDialog(owner);
        }

        internal static string? Ask(string prompt, string title)
        {
            using var dialog = new Form
            {
                Text = title,
                Width = 390,
                Height = 165,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MinimizeBox = false,
                MaximizeBox = false
            };
            var label = new Label { Text = prompt, Left = 12, Top = 12, AutoSize = true };
            var input = new TextBox { Left = 12, Top = 42, Width = 345 };
            var ok = new Button { Text = "OK", Left = 275, Top = 78, DialogResult = DialogResult.OK };
            var cancel = new Button { Text = "Batal", Left = 190, Top = 78, DialogResult = DialogResult.Cancel };
            dialog.Controls.AddRange([label, input, ok, cancel]);
            dialog.AcceptButton = ok;
            dialog.CancelButton = cancel;
            return dialog.ShowDialog() == DialogResult.OK ? input.Text : null;
        }

        internal static void ShowDataError(IWin32Window owner, Exception exception)
        {
            MessageBox.Show(owner,
                $"Data gudang tidak dapat diproses. Pastikan folder berikut tersedia dan dapat ditulis:\r\n{WarehouseStore.Root}\r\n\r\n{exception.Message}",
                "Kesalahan data", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
