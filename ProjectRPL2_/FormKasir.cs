using System.Globalization;
using System.Windows.Forms;

namespace ProjectRPL2_
{
    public partial class FormKasir : Form
    {
        private readonly string namaKasir;
        private readonly ListBox daftarBarang = new() { Name = "daftarBarang", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        private List<PenyimpananGudang.Barang> dataBarang = [];

        public FormKasir(string namaKasir)
        {
            this.namaKasir = namaKasir;
            InitializeComponent();
            Text = $"SMKMART - Kasir ({namaKasir})";

            var panelMenuKasir = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 88, Padding = new Padding(6), WrapContents = true };
            tambahTombolMenu(panelMenuKasir, "tombolMuatUlang", "Muat ulang", muatDaftarBarang);
            tambahTombolMenu(panelMenuKasir, "tombolTambahKeKeranjang", "Tambah ke keranjang", tambahBarangKeKeranjang);
            tambahTombolMenu(panelMenuKasir, "tombolLihatKeranjang", "Lihat keranjang", () => tampilkanKeranjang(modeHapus: false));
            tambahTombolMenu(panelMenuKasir, "tombolHapusDariKeranjang", "Hapus dari keranjang", () => tampilkanKeranjang(modeHapus: true));
            tambahTombolMenu(panelMenuKasir, "tombolCheckout", "Checkout", checkout);
            tambahTombolMenu(panelMenuKasir, "tombolRiwayatTransaksi", "Riwayat transaksi", () => TampilanPeran.tampilkanRiwayat(this, "Riwayat Transaksi", riwayatGudang: false));
            tambahTombolMenu(panelMenuKasir, "tombolCariBarang", "Cari barang", cariBarangLinear);
            tambahTombolMenu(panelMenuKasir, "tombolSummaryPenjualan", "Summary penjualan", () => TampilanPeran.tampilkanSummaryPenjualan(this));
            Controls.Add(daftarBarang);
            Controls.Add(panelMenuKasir);
            muatDaftarBarang();
        }

        private static void tambahTombolMenu(FlowLayoutPanel panelTombol, string namaTombol, string teksTombol, Action aksiTombol)
        {
            var tombolMenu = new Button { Name = namaTombol, Text = teksTombol, AutoSize = true, Height = 32 };
            tombolMenu.Click += (_, _) => aksiTombol();
            panelTombol.Controls.Add(tombolMenu);
        }

        private void muatDaftarBarang()
        {
            try
            {
                dataBarang = PenyimpananGudang.muatData(includeDeleted: false)
                    .Where(barang => barang.StatusBarang.Equals("aktif", StringComparison.OrdinalIgnoreCase)).ToList();
                daftarBarang.Items.Clear();
                foreach (var barang in dataBarang)
                    daftarBarang.Items.Add(TampilanPeran.formatBarang(barang));
            }
            catch (Exception ex) { TampilanPeran.tampilkanKesalahanData(this, ex); }
        }

        private PenyimpananGudang.Barang? barangTerpilih()
        {
            int indeks = daftarBarang.SelectedIndex;
            return indeks >= 0 && indeks < dataBarang.Count ? dataBarang[indeks] : null;
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
                using var formKeranjang = new Form { Text = modeHapus ? "Hapus Barang dari Keranjang" : "Keranjang Belanja", Width = 760, Height = 420, StartPosition = FormStartPosition.CenterParent };
                var daftarIsiKeranjang = new ListBox { Name = "daftarIsiKeranjang", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
                foreach (var item in keranjangBelanja)
                    daftarIsiKeranjang.Items.Add($"{item.IdBarang} | {item.NamaBarang} | Jumlah {item.Jumlah} | {PenyimpananGudang.formatRupiah(item.HargaSatuan)} | Subtotal {PenyimpananGudang.formatRupiah(item.HargaSatuan * item.Jumlah)}");
                var panelBawah = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 46, Padding = new Padding(6) };
                var totalHarga = keranjangBelanja.Sum(item => item.HargaSatuan * item.Jumlah);
                panelBawah.Controls.Add(new Label { Text = $"Total: {PenyimpananGudang.formatRupiah(totalHarga)}", AutoSize = true, Padding = new Padding(8) });
                if (modeHapus)
                {
                    var tombolHapusDariKeranjang = new Button { Name = "tombolHapusDariKeranjang", Text = "Hapus barang terpilih", AutoSize = true };
                    tombolHapusDariKeranjang.Click += (_, _) =>
                    {
                        int indeks = daftarIsiKeranjang.SelectedIndex;
                        if (indeks < 0) return;
                        try
                        {
                            PenyimpananGudang.hapusDariKeranjang(keranjangBelanja[indeks].IdBarang);
                            formKeranjang.DialogResult = DialogResult.OK;
                            formKeranjang.Close();
                        }
                        catch (Exception ex) { TampilanPeran.tampilkanKesalahanData(formKeranjang, ex); }
                    };
                    panelBawah.Controls.Add(tombolHapusDariKeranjang);
                }
                var tombolTutup = new Button { Text = "Tutup", AutoSize = true, DialogResult = DialogResult.Cancel };
                panelBawah.Controls.Add(tombolTutup);
                formKeranjang.Controls.Add(daftarIsiKeranjang);
                formKeranjang.Controls.Add(panelBawah);
                formKeranjang.CancelButton = tombolTutup;
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
                string? metodePembayaran = TampilanPeran.tanyaPengguna(
                    $"Total belanja {PenyimpananGudang.formatRupiah(totalHarga)}. Masukkan metode: Cash, Debit, atau QRIS.", "checkout");
                if (string.IsNullOrWhiteSpace(metodePembayaran)) return;
                decimal uangDibayarkan = totalHarga;
                if (metodePembayaran.Equals("Cash", StringComparison.OrdinalIgnoreCase))
                {
                    string? teksUangDibayar = TampilanPeran.tanyaPengguna($"Total {PenyimpananGudang.formatRupiah(totalHarga)}. Uang diterima:", "Pembayaran Tunai");
                    if (teksUangDibayar is null) return;
                if (!decimal.TryParse(teksUangDibayar, NumberStyles.Number, CultureInfo.GetCultureInfo("id-ID"), out uangDibayarkan) || uangDibayarkan < 0)
                    { MessageBox.Show(this, "Nominal pembayaran tidak valid."); return; }
                }
                bool berhasil = PenyimpananGudang.checkout(namaKasir, metodePembayaran.Trim(), uangDibayarkan, out string pesan);
                MessageBox.Show(this, pesan, berhasil ? "Transaksi berhasil" : "Transaksi gagal", MessageBoxButtons.OK,
                    berhasil ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                if (berhasil) muatDaftarBarang();
            }
            catch (Exception ex) { TampilanPeran.tampilkanKesalahanData(this, ex); }
        }

        private void cariBarangLinear()
        {
            string? teksModeCari = TampilanPeran.tanyaPengguna("Cari dengan: 1 = ID, 2 = nama, 3 = ID kategori", "Cari Barang");
            if (!int.TryParse(teksModeCari, out int pilihanCari) || pilihanCari is < 1 or > 3) return;
            string? kataKunci = TampilanPeran.tanyaPengguna(pilihanCari == 1 ? "ID barang:" : pilihanCari == 2 ? "Nama atau potongan nama:" : "ID kategori:", "Cari Barang");
            if (kataKunci is null) return;
            try
            {
                var hasilPencarian = PenyimpananGudang.cariBarangLinear(pilihanCari, kataKunci.Trim());
                string isiHasil = hasilPencarian.Count == 0 ? "Barang tidak ditemukan." : string.Join(Environment.NewLine, hasilPencarian.Select(TampilanPeran.formatBarang));
                TampilanPeran.tampilkanTeks(this, "Hasil Pencarian", isiHasil);
            }
            catch (Exception ex) { TampilanPeran.tampilkanKesalahanData(this, ex); }
        }
    }
}





