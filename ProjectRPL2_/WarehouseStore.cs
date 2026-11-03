using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ProjectRPL2_
{
    internal static class WarehouseStore
    {
        internal static readonly string Root = FindDataRoot();

        internal static readonly string UserFile = Path.Combine(Root, "User.txt");
        private static string ProductsFile => Path.Combine(Root, "data.txt");
        private static string CategoriesFile => Path.Combine(Root, "kategori.txt");
        private static string CartFile => Path.Combine(Root, "keranjang.txt");
        private static string SalesHistoryFile => Path.Combine(Root, "riwayat_transaksi.txt");
        private static string WarehouseHistoryFile => Path.Combine(Root, "riwayat_gudang.txt");
        private static readonly Encoding Utf8 = new UTF8Encoding(false);
        private static readonly CultureInfo Indonesian = CultureInfo.GetCultureInfo("id-ID");
        private static readonly object FileLock = new();

        private static string FindDataRoot()
        {
            string fallback = AppContext.BaseDirectory;
            for (DirectoryInfo? folder = new DirectoryInfo(AppContext.BaseDirectory);
                 folder is not null;
                 folder = folder.Parent)
            {
                string projectFile = Path.Combine(folder.FullName, "ProjectRPL2_.csproj");
                string userFile = Path.Combine(folder.FullName, "User.txt");
                string dataFile = Path.Combine(folder.FullName, "data.txt");
                if (File.Exists(projectFile) && File.Exists(userFile) && File.Exists(dataFile))
                    return folder.FullName;
            }

            return fallback;
        }

        internal sealed record Product(int Id, string Name, int Stock, decimal Price, string Status, string Category);

        internal static List<Product> GetProducts(bool includeDeleted = true)
        {
            var categories = GetCategories();
            var products = new List<Product>();
            foreach (string line in File.ReadLines(ProductsFile, Encoding.UTF8))
            {
                if (TryParseProduct(line, categories, out Product product) &&
                    (includeDeleted || !product.Status.Equals("terhapus", StringComparison.OrdinalIgnoreCase)))
                    products.Add(product);
            }
            return products.OrderBy(product => product.Id).ToList();
        }

        internal static IReadOnlyDictionary<int, string> GetCategories()
        {
            var categories = new Dictionary<int, string>();
            foreach (string line in File.ReadLines(CategoriesFile, Encoding.UTF8))
            {
                string[] fields = line.Split(',', 2);
                if (fields.Length == 2 && int.TryParse(fields[0].Trim(), out int id) && id > 0 &&
                    !string.IsNullOrWhiteSpace(fields[1]))
                    categories[id] = fields[1].Trim();
            }
            return categories;
        }

        internal static bool Restock(int productId, int amount, out string message)
        {
            if (amount <= 0) { message = "Jumlah restok harus lebih dari nol."; return false; }
            lock (FileLock)
            {
                string original = File.ReadAllText(ProductsFile, Encoding.UTF8);
                string[] lines = File.ReadAllLines(ProductsFile, Encoding.UTF8);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (!TryParseProduct(lines[i], GetCategories(), out Product product) || product.Id != productId)
                        continue;
                    if (product.Status.Equals("terhapus", StringComparison.OrdinalIgnoreCase))
                    { message = "Produk sudah dihapus dari katalog."; return false; }
                    int newStock;
                    try { newStock = checked(product.Stock + amount); }
                    catch (OverflowException) { message = "Jumlah stok melebihi batas."; return false; }

                    string[] fields = lines[i].Split(',');
                    fields[2] = newStock.ToString(CultureInfo.InvariantCulture);
                    if (fields.Length == 4) fields = [.. fields, "aktif"];
                    lines[i] = string.Join(',', fields);
                    WriteProductLines(lines);
                    try
                    {
                        File.AppendAllText(WarehouseHistoryFile,
                            $"[RESTOK] {product.Name} +{amount} (Total Stok: {newStock}){Environment.NewLine}", Utf8);
                    }
                    catch
                    {
                        File.WriteAllText(ProductsFile, original, Utf8);
                        throw;
                    }
                    message = $"Stok {product.Name} berhasil ditambah menjadi {newStock}.";
                    return true;
                }
            }
            message = "Produk tidak ditemukan.";
            return false;
        }

        internal static bool Checkout(int productId, int quantity, string cashier, string method,
            decimal amountPaid, out string message)
        {
            if (quantity <= 0) { message = "Jumlah pembelian harus lebih dari nol."; return false; }
            if (!(method.Equals("Cash", StringComparison.OrdinalIgnoreCase) ||
                  method.Equals("QRIS", StringComparison.OrdinalIgnoreCase) ||
                  method.Equals("Debit", StringComparison.OrdinalIgnoreCase)))
            { message = "Metode pembayaran tidak dikenal."; return false; }

            lock (FileLock)
            {
                string original = File.ReadAllText(ProductsFile, Encoding.UTF8);
                string[] lines = File.ReadAllLines(ProductsFile, Encoding.UTF8);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (!TryParseProduct(lines[i], GetCategories(), out Product product) || product.Id != productId)
                        continue;
                    if (product.Status.Equals("terhapus", StringComparison.OrdinalIgnoreCase))
                    { message = "Produk sudah tidak aktif."; return false; }
                    if (quantity > product.Stock)
                    { message = $"Stok tidak cukup. Tersedia {product.Stock} unit."; return false; }

                    decimal total = product.Price * quantity;
                    if (method.Equals("Cash", StringComparison.OrdinalIgnoreCase) && amountPaid < total)
                    { message = $"Uang pembayaran kurang. Total belanja {Money(total)}."; return false; }
                    if (!method.Equals("Cash", StringComparison.OrdinalIgnoreCase)) amountPaid = total;

                    string[] fields = lines[i].Split(',');
                    fields[2] = (product.Stock - quantity).ToString(CultureInfo.InvariantCulture);
                    if (fields.Length == 4) fields = [.. fields, "aktif"];
                    lines[i] = string.Join(',', fields);
                    WriteProductLines(lines);

                    try
                    {
                        AppendSale(product, quantity, cashier, method, total, amountPaid);
                    }
                    catch
                    {
                        File.WriteAllText(ProductsFile, original, Utf8);
                        throw;
                    }
                    decimal change = amountPaid - total;
                    message = $"Transaksi berhasil. Total {Money(total)}; kembalian {Money(change)}.";
                    return true;
                }
            }
            message = "Produk tidak ditemukan.";
            return false;
        }

        internal static string ReadHistory(bool warehouse)
        {
            string path = warehouse ? WarehouseHistoryFile : SalesHistoryFile;
            return File.Exists(path) ? File.ReadAllText(path, Encoding.UTF8) : "Belum ada riwayat.";
        }

        internal static string Money(decimal amount) => amount.ToString("C0", Indonesian);

        private static bool TryParseProduct(string line, IReadOnlyDictionary<int, string> categories, out Product product)
        {
            product = null!;
            if (string.IsNullOrWhiteSpace(line)) return false;
            string[] fields = line.Split(',');
            if (fields.Length is not (4 or 5) ||
                !int.TryParse(fields[0].Trim(), out int id) || id <= 0 ||
                string.IsNullOrWhiteSpace(fields[1]) ||
                !int.TryParse(fields[2].Trim(), out int stock) || stock < 0 ||
                !decimal.TryParse(fields[3].Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal price) || price < 0)
                return false;

            string status = fields.Length == 5 ? fields[4].Trim() : "aktif";
            int categoryId = id / 100;
            string category = categories.TryGetValue(categoryId, out string? name) ? name : "Lainnya";
            product = new Product(id, fields[1].Trim(), stock, price, status, category);
            return true;
        }

        private static void WriteProductLines(string[] lines)
        {
            string temp = ProductsFile + ".csharp.tmp";
            File.WriteAllLines(temp, lines, Utf8);
            File.Move(temp, ProductsFile, true);
        }

        private static void AppendSale(Product product, int quantity, string cashier, string method,
            decimal total, decimal paid)
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            int count = File.ReadLines(SalesHistoryFile, Encoding.UTF8)
                .Count(line => Regex.IsMatch(line, @"^Tanggal\s*:\s*" + Regex.Escape(today), RegexOptions.CultureInvariant));
            DateTime now = DateTime.Now;
            decimal change = paid - total;
            string transaction = string.Join(Environment.NewLine,
                "=== TRANSAKSI ===",
                $"ID Transaksi: SMK{count + 1:000}{now:yyyyMMddHHmmss}",
                $"Tanggal : {now:yyyy-MM-dd HH:mm}",
                $"Kasir   : {cashier}",
                $"Metode  : {method}",
                "----------------------------------------",
                $"- [{product.Id}] {product.Name} x{quantity} = {Money(product.Price * quantity)}",
                $"TOTAL: {Money(total)}",
                $"Dibayar: {Money(paid)}",
                $"Kembalian: {Money(change)}",
                "----------------------------------------", "", "");
            File.AppendAllText(SalesHistoryFile, transaction, Utf8);
        }
    }
}
