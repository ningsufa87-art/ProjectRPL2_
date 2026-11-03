using System.Globalization;
using System.Windows.Forms;

namespace ProjectRPL2_
{
    internal static class TampilanPeran
    {
        internal static string formatBarang(PenyimpananGudang.Barang barang) =>
            $"{barang.IdBarang} | {barang.NamaBarang} | {barang.NamaKategori} | Stok {barang.Stok} | " +
            $"{PenyimpananGudang.formatRupiah(barang.Harga)} | {barang.StatusBarang}" +
            (barang.StatusBarang.Equals("aktif", StringComparison.OrdinalIgnoreCase) && barang.Stok < 5 ? " | STOK RENDAH" : string.Empty);

        internal static void tampilkanRiwayat(IWin32Window pemilikForm, string judul, bool riwayatGudang)
        {
            tampilkanTeks(pemilikForm, judul, PenyimpananGudang.bacaRiwayat(riwayatGudang));
        }

        internal static void tampilkanTeks(IWin32Window pemilikForm, string judul, string isi)
        {
            var formRiwayat = new Form
            {
                Name = "formTampilanRiwayat",
                Text = judul,
                Width = 760,
                Height = 520,
                StartPosition = FormStartPosition.CenterParent
            };
            var kotakTeksRiwayat = new TextBox
            {
                Name = "kotakTeksRiwayat",
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                WordWrap = false,
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10),
                Text = isi
            };
            formRiwayat.Controls.Add(kotakTeksRiwayat);
            formRiwayat.ShowDialog(pemilikForm);
        }

        internal static void tampilkanSummaryPenjualan(IWin32Window pemilikForm)
        {
            try
            {
                var dataPenjualan = PenyimpananGudang.summaryPenjualan();
                if (dataPenjualan.Count == 0)
                {
                    MessageBox.Show(pemilikForm, "Belum ada transaksi dengan format yang dapat diringkas.");
                    return;
                }

                var daftarBulan = dataPenjualan.Select(penjualan => penjualan.Tanggal.ToString("yyyy-MM")).Distinct().ToList();
                string? bulanDipilih = tanyaPengguna(
                    $"Bulan tersedia: {string.Join(", ", daftarBulan)}\r\nMasukkan bulan yyyy-MM (kosongkan untuk semua):",
                    "Summary Penjualan");
                if (bulanDipilih is null) return;

                var penjualanTerpilih = string.IsNullOrWhiteSpace(bulanDipilih)
                    ? dataPenjualan
                    : dataPenjualan.Where(penjualan => penjualan.Tanggal.ToString("yyyy-MM") == bulanDipilih.Trim()).ToList();
                if (penjualanTerpilih.Count == 0)
                {
                    MessageBox.Show(pemilikForm, "Bulan tidak ditemukan.");
                    return;
                }

                string isiSummary = string.Join(Environment.NewLine, penjualanTerpilih.Select(penjualan =>
                    $"{penjualan.Tanggal.ToString("dd MMMM yyyy", CultureInfo.GetCultureInfo("id-ID"))} | {PenyimpananGudang.formatRupiah(penjualan.TotalPenjualan)}")) +
                    Environment.NewLine + $"TOTAL: {PenyimpananGudang.formatRupiah(penjualanTerpilih.Sum(penjualan => penjualan.TotalPenjualan))}";
                tampilkanTeks(pemilikForm, "Summary Penjualan", isiSummary);
            }
            catch (Exception ex)
            {
                tampilkanKesalahanData(pemilikForm, ex);
            }
        }

        internal static string? tanyaPengguna(string pertanyaan, string judul)
        {
            using var formInput = new Form
            {
                Name = "formInput",
                Text = judul,
                Width = 390,
                Height = 165,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MinimizeBox = false,
                MaximizeBox = false
            };
            var labelPertanyaan = new Label { Name = "labelPertanyaan", Text = pertanyaan, Left = 12, Top = 12, AutoSize = true };
            var inputJawaban = new TextBox { Name = "inputJawaban", Left = 12, Top = 42, Width = 345 };
            var tombolSetuju = new Button { Name = "tombolSetuju", Text = "OK", Left = 275, Top = 78, DialogResult = DialogResult.OK };
            var tombolBatal = new Button { Name = "tombolBatal", Text = "Batal", Left = 190, Top = 78, DialogResult = DialogResult.Cancel };
            formInput.Controls.AddRange([labelPertanyaan, inputJawaban, tombolSetuju, tombolBatal]);
            formInput.AcceptButton = tombolSetuju;
            formInput.CancelButton = tombolBatal;
            return formInput.ShowDialog() == DialogResult.OK ? inputJawaban.Text : null;
        }

        internal static void tampilkanKesalahanData(IWin32Window pemilikForm, Exception kesalahan)
        {
            MessageBox.Show(pemilikForm,
                $"Data gudang tidak dapat diproses. Pastikan folder berikut tersedia dan dapat ditulis:\r\n{PenyimpananGudang.folderData}\r\n\r\n{kesalahan.Message}",
                "Kesalahan data", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

