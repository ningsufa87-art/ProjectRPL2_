using System.Globalization;
using System.Windows.Forms;

namespace ProjectRPL2_
{
    public partial class FormKasir : Form
    {
        private readonly string namaKasir;
        private readonly ListView daftarBarang = TampilanPeran.buatDaftarBarang();
        private readonly Label labelPetunjukBarang = new() { Name = "labelPetunjukBarang", Dock = DockStyle.Top, Height = 38, ForeColor = TampilanModern.TeksSekunder, Tag = "sekunder" };
        private List<PenyimpananGudang.Barang> dataBarang = [];

        public FormKasir(string namaKasir)
        {
            this.namaKasir = namaKasir;
            InitializeComponent();
            TampilanModern.aturForm(this);
            Text = $"SMKMART - Kasir ({namaKasir})";

            var panelMenuKasir = new FlowLayoutPanel
            {
                Name = "panelMenuKasir", Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown,
                WrapContents = false, AutoScroll = true, Padding = new Padding(8), BackColor = TampilanModern.Permukaan, Tag = "permukaan"
            };
            panelMenuKasir.Controls.Add(new Label { Text = "TRANSAKSI", AutoSize = false, Width = 190, Height = 34, TextAlign = ContentAlignment.MiddleLeft, ForeColor = TampilanModern.TeksSekunder, Font = new Font("Segoe UI", 8.5F, FontStyle.Bold), Margin = new Padding(5), Tag = "sekunder" });
            tambahTombolMenu(panelMenuKasir, "tombolMuatUlang", "Muat ulang", muatDaftarBarang);
            tambahTombolMenu(panelMenuKasir, "tombolTambahKeKeranjang", "Tambah ke keranjang", tambahBarangKeKeranjang);
            tambahTombolMenu(panelMenuKasir, "tombolLihatKeranjang", "Lihat keranjang", () => tampilkanKeranjang(modeHapus: false));
            tambahTombolMenu(panelMenuKasir, "tombolHapusDariKeranjang", "Hapus dari keranjang", () => tampilkanKeranjang(modeHapus: true));
            tambahTombolMenu(panelMenuKasir, "tombolCheckout", "Checkout", checkout);
            tambahTombolMenu(panelMenuKasir, "tombolRiwayatTransaksi", "Riwayat transaksi", () => TampilanPeran.tampilkanRiwayat(this, "Riwayat Transaksi", riwayatGudang: false));
            tambahTombolMenu(panelMenuKasir, "tombolCariBarang", "Cari barang", cariBarangLinear);
            tambahTombolMenu(panelMenuKasir, "tombolSummaryPenjualan", "Summary penjualan", () => TampilanPeran.tampilkanSummaryPenjualan(this));
            foreach (Button kontrol in panelMenuKasir.Controls.OfType<Button>())
            {
                kontrol.Width = 190;
                kontrol.Height = 42;
                kontrol.TextAlign = ContentAlignment.MiddleLeft;
            }
            var panelSamping = new Panel { Name = "panelNavigasiKasir", Dock = DockStyle.Fill, Padding = new Padding(8), BackColor = TampilanModern.Permukaan, Tag = "permukaan" };
            panelSamping.Controls.Add(panelMenuKasir);
            var panelKonten = new Panel { Name = "panelKontenKasir", Dock = DockStyle.Fill, Padding = new Padding(24), BackColor = TampilanModern.Latar };
            var labelDaftar = new Label { Name = "labelDaftarBarang", Text = "Barang siap dijual", Dock = DockStyle.Top, Height = 42, Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = TampilanModern.Teks };
            panelKonten.Controls.Add(daftarBarang);
            panelKonten.Controls.Add(labelPetunjukBarang);
            panelKonten.Controls.Add(labelDaftar);
            var tataLetakIsi = new TableLayoutPanel { Name = "tataLetakIsiKasir", Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, BackColor = TampilanModern.Latar };
            tataLetakIsi.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250));
            tataLetakIsi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tataLetakIsi.Controls.Add(panelSamping, 0, 0);
            tataLetakIsi.Controls.Add(panelKonten, 1, 0);
            var tataLetakKasir = new TableLayoutPanel { Name = "tataLetakKasir", Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
            tataLetakKasir.RowStyles.Add(new RowStyle(SizeType.Absolute, 84));
            tataLetakKasir.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            tataLetakKasir.Controls.Add(TampilanModern.buatHeader("Panel Kasir", "Pilih barang, siapkan keranjang, dan proses pembayaran", namaKasir, Close), 0, 0);
            tataLetakKasir.Controls.Add(tataLetakIsi, 0, 1);
            Controls.Clear();
            Controls.Add(tataLetakKasir);
            TampilanModern.aturPaletForm(this);
            muatDaftarBarang();
        }

        private static void tambahTombolMenu(FlowLayoutPanel panelTombol, string namaTombol, string teksTombol, Action aksiTombol)
        {
            var tombolMenu = TampilanModern.buatTombol(namaTombol, teksTombol, sekunder: true);
            tombolMenu.AutoSize = false;
            tombolMenu.Width = 190;
            tombolMenu.Height = 42;
            tombolMenu.TextAlign = ContentAlignment.MiddleLeft;
            tombolMenu.Click += (_, _) => aksiTombol();
            panelTombol.Controls.Add(tombolMenu);
        }

        private void muatDaftarBarang()
        {
            try
            {
                dataBarang = PenyimpananGudang.muatData(includeDeleted: false)
                    .Where(barang => barang.StatusBarang.Equals("aktif", StringComparison.OrdinalIgnoreCase)).ToList();
                TampilanPeran.muatBarangKeDaftar(daftarBarang, dataBarang);
                labelPetunjukBarang.Text = dataBarang.Count == 0
                    ? "Belum ada barang aktif yang dapat dijual. Hubungi Admin untuk mengisi katalog."
                    : "Pilih barang lalu tambahkan ke keranjang. Stok di bawah 5 unit ditandai.";
            }
            catch (Exception ex) { TampilanPeran.tampilkanKesalahanData(this, ex); }
        }

        private PenyimpananGudang.Barang? barangTerpilih()
        {
            return TampilanPeran.barangTerpilih(daftarBarang);
        }

        private void tambahBarangKeKeranjang()
        {
            PenyimpananGudang.Barang? barang = barangTerpilih();
            if (barang is null) { MessageBox.Show(this, "Pilih barang yang ingin dimasukkan ke keranjang."); return; }
            string? masukan = TampilanPeran.tanyaPengguna($"Jumlah {barang.NamaBarang} (stok {barang.Stok}):", "Tambah ke Keranjang");
            if (masukan is null) return;
            if (!int.TryParse(masukan, out int jumlah) || jumlah <= 0)
            { MessageBox.Show(this, "Jumlah harus bilangan bulat lebih dari nol."); return; }
            try
            {
                bool berhasil = PenyimpananGudang.tambahKeKeranjang(barang.IdBarang, jumlah, out string pesan);
                MessageBox.Show(this, pesan, berhasil ? "Keranjang" : "Gagal", MessageBoxButtons.OK,
                    berhasil ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            }
            catch (Exception ex) { TampilanPeran.tampilkanKesalahanData(this, ex); }
        }

        private void tampilkanKeranjang(bool modeHapus)
        {
            try
            {
                List<PenyimpananGudang.BarangKeranjang> keranjangBelanja = PenyimpananGudang.muatKeranjang();
                if (keranjangBelanja.Count == 0) { MessageBox.Show(this, "Keranjang belanja kosong."); return; }
                using var formKeranjang = new Form
                {
                    Name = "formKeranjangBelanja", Text = modeHapus ? "Hapus Barang dari Keranjang" : "Keranjang Belanja",
                    Width = 920, Height = 540, MinimumSize = new Size(720, 420),
                    StartPosition = FormStartPosition.CenterParent, BackColor = TampilanModern.Latar, Font = new Font("Segoe UI", 10F)
                };
                var panelIsi = new Panel { Dock = DockStyle.Fill, Padding = new Padding(22), BackColor = TampilanModern.Latar };
                var daftarIsiKeranjang = TampilanPeran.buatDaftarKeranjang();
                TampilanPeran.muatKeranjangKeDaftar(daftarIsiKeranjang, keranjangBelanja);
                var labelJudul = new Label { Text = "KERANJANG BELANJA", Dock = DockStyle.Top, Height = 46, Font = new Font("Segoe UI", 15, FontStyle.Bold), ForeColor = TampilanModern.Teks };
                var panelBawah = new FlowLayoutPanel { Name = "panelBawahKeranjang", Dock = DockStyle.Bottom, Height = 68, Padding = new Padding(4), FlowDirection = FlowDirection.RightToLeft, BackColor = TampilanModern.Latar };
                var totalHarga = keranjangBelanja.Sum(item => item.HargaSatuan * item.Jumlah);
                var labelTotal = new Label { Text = $"TOTAL  {PenyimpananGudang.formatRupiah(totalHarga)}", AutoSize = true, Padding = new Padding(12), Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = TampilanModern.Aksen, TextAlign = ContentAlignment.MiddleRight };
                panelBawah.Controls.Add(labelTotal);
                if (modeHapus)
                {
                    var tombolHapusDariKeranjang = TampilanModern.buatTombol("tombolHapusDariKeranjang", "Hapus barang terpilih", sekunder: true);
                    tombolHapusDariKeranjang.Click += (_, _) =>
                    {
                        var itemDipilih = daftarIsiKeranjang.SelectedItems.Count > 0
                            ? daftarIsiKeranjang.SelectedItems[0].Tag as PenyimpananGudang.BarangKeranjang : null;
                        if (itemDipilih is null) { MessageBox.Show(formKeranjang, "Pilih barang yang ingin dihapus."); return; }
                        try
                        {
                            if (!PenyimpananGudang.hapusDariKeranjang(itemDipilih.IdBarang))
                            {
                                MessageBox.Show(formKeranjang, "ID barang tidak ditemukan di keranjang.", "Keranjang", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            MessageBox.Show(formKeranjang, $"'{itemDipilih.NamaBarang}' (ID: {itemDipilih.IdBarang}) berhasil dihapus dari keranjang.", "Keranjang", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            formKeranjang.DialogResult = DialogResult.OK;
                            formKeranjang.Close();
                        }
                        catch (Exception ex) { TampilanPeran.tampilkanKesalahanData(formKeranjang, ex); }
                    };
                    panelBawah.Controls.Add(tombolHapusDariKeranjang);
                }
                var tombolTutup = TampilanModern.buatTombol("tombolTutupKeranjang", "Tutup", sekunder: true);
                tombolTutup.DialogResult = DialogResult.Cancel;
                panelBawah.Controls.Add(tombolTutup);
                panelIsi.Controls.Add(daftarIsiKeranjang);
                panelIsi.Controls.Add(panelBawah);
                panelIsi.Controls.Add(labelJudul);
                formKeranjang.Controls.Add(panelIsi);
                formKeranjang.CancelButton = tombolTutup;
                TampilanModern.aturUkuranDialog(formKeranjang, new Size(920, 540), new Size(720, 420));
                TampilanModern.aturPaletForm(formKeranjang);
                formKeranjang.ShowDialog(this);
            }
            catch (Exception ex) { TampilanPeran.tampilkanKesalahanData(this, ex); }
        }

        private void checkout()
        {
            try
            {
                List<PenyimpananGudang.BarangKeranjang> keranjangBelanja = PenyimpananGudang.muatKeranjang();
                if (keranjangBelanja.Count == 0) { MessageBox.Show(this, "Keranjang belanja kosong."); return; }
                decimal totalHarga = keranjangBelanja.Sum(item => item.HargaSatuan * item.Jumlah);
                DateTime waktuTransaksi = DateTime.Now;
                string isiStrukAwal = string.Join(Environment.NewLine,
                    "STRUK PEMBELIAN",
                    "==========================================================================",
                    $"Tanggal     : {waktuTransaksi:yyyy-MM-dd HH:mm}",
                    $"Kasir       : {namaKasir}",
                    "ID     Nama Barang                       Jumlah       Subtotal",
                    "--------------------------------------------------------------------------",
                    string.Join(Environment.NewLine, keranjangBelanja.Select(item =>
                        $"{item.IdBarang,-6}  {item.NamaBarang,-32} {item.Jumlah,-8} {PenyimpananGudang.formatRupiah(item.HargaSatuan * item.Jumlah),14}")),
                    "==========================================================================",
                    $"TOTAL HARGA: {PenyimpananGudang.formatRupiah(totalHarga)}");
                TampilanPeran.tampilkanTeks(this, "Struk Pembelian", isiStrukAwal);
                string? pilihanMetode = TampilanPeran.tanyaPengguna(
                    $"Total belanja {PenyimpananGudang.formatRupiah(totalHarga)}\r\n\r\n1. Cash\r\n2. Debit\r\n3. QRIS\r\n0. Batal", "Pilih Metode Pembayaran");
                string? metodePembayaran = pilihanMetode?.Trim() switch
                {
                    "1" => "Cash", "2" => "Debit", "3" => "QRIS", _ => null
                };
                if (pilihanMetode?.Trim() == "0") return;
                if (metodePembayaran is null)
                {
                    MessageBox.Show(this, "Metode pembayaran tidak valid. Pilih 1, 2, atau 3.", "Pembayaran", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                decimal uangDibayarkan = totalHarga;
                if (metodePembayaran.Equals("Cash", StringComparison.OrdinalIgnoreCase))
                {
                    string? teksUangDibayar = TampilanPeran.tanyaPengguna($"Total {PenyimpananGudang.formatRupiah(totalHarga)}. Uang diterima:", "Pembayaran Tunai");
                    if (teksUangDibayar is null) return;
                    if (!decimal.TryParse(teksUangDibayar, NumberStyles.Number, CultureInfo.GetCultureInfo("id-ID"), out uangDibayarkan) || uangDibayarkan < 0)
                    { MessageBox.Show(this, "Nominal pembayaran tidak valid."); return; }
                }
                bool berhasil = PenyimpananGudang.checkout(namaKasir, metodePembayaran, uangDibayarkan, out string pesan);
                if (berhasil)
                {
                    muatDaftarBarang();
                    TampilanPeran.tampilkanTeks(this, "Struk Pembayaran", string.Join(Environment.NewLine,
                        "PEMBAYARAN BERHASIL",
                        "==========================================================================",
                        pesan,
                        "==========================================================================",
                        "Terima kasih telah berbelanja di SMKMART!"));
                }
                else MessageBox.Show(this, pesan, "Transaksi gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex) { TampilanPeran.tampilkanKesalahanData(this, ex); }
        }

        private void cariBarangLinear()
        {
            if (PenyimpananGudang.muatData().Count == 0) { MessageBox.Show(this, "Data barang masih kosong."); return; }
            string? teksModeCari = TampilanPeran.tanyaPengguna("PENCARIAN BARANG (LINEAR SEARCH)\r\n1. Cari berdasarkan ID barang\r\n2. Cari berdasarkan nama barang\r\n3. Cari berdasarkan ID kategori\r\n0. Batal\r\n\r\nMasukkan pilihan pencarian (0-3):", "Cari Barang");
            if (!int.TryParse(teksModeCari, out int pilihanCari)) { MessageBox.Show(this, "Pilihan harus berupa angka."); return; }
            if (pilihanCari == 0) return;
            if (pilihanCari is < 1 or > 3) { MessageBox.Show(this, "Pilihan tidak valid."); return; }
            string? kataKunci = TampilanPeran.tanyaPengguna(pilihanCari == 1 ? "ID barang:" : pilihanCari == 2 ? "Nama atau potongan nama:" : "ID kategori:", "Cari Barang");
            if (kataKunci is null) return;
            if (string.IsNullOrWhiteSpace(kataKunci)) { MessageBox.Show(this, pilihanCari == 2 ? "Nama barang tidak boleh kosong!" : "Masukan pencarian tidak boleh kosong."); return; }
            if (pilihanCari is 1 or 3 && !int.TryParse(kataKunci, out _)) { MessageBox.Show(this, pilihanCari == 1 ? "ID barang harus berupa angka!" : "ID kategori harus berupa angka!"); return; }
            try
            {
                if (pilihanCari == 3 && int.TryParse(kataKunci, out int idKategori) && !PenyimpananGudang.muatKategori().ContainsKey(idKategori))
                { MessageBox.Show(this, $"Kategori dengan ID {idKategori} tidak ditemukan."); return; }
                var hasilPencarian = PenyimpananGudang.cariBarangLinear(pilihanCari, kataKunci.Trim());
                TampilanPeran.tampilkanDaftarBarang(this, "Hasil Pencarian", hasilPencarian);
            }
            catch (Exception ex) { TampilanPeran.tampilkanKesalahanData(this, ex); }
        }
    }
}





