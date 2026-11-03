using System.Globalization;
using System.Windows.Forms;

namespace ProjectRPL2_
{
    public partial class FormAdmin : Form
    {
        private readonly ListView daftarBarang = TampilanPeran.buatDaftarBarang();
        private readonly Label labelPetunjukBarang = new() { Name = "labelPetunjukBarang", Dock = DockStyle.Top, Height = 38, ForeColor = TampilanModern.TeksSekunder, Tag = "sekunder" };
        private List<PenyimpananGudang.Barang> dataBarang = [];

        public FormAdmin(string namaAdmin)
        {
            InitializeComponent();
            TampilanModern.aturForm(this);
            Text = $"SMKMART - Admin ({namaAdmin})";

            var panelMenuAdmin = new FlowLayoutPanel
            {
                Name = "panelMenuAdmin", Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown,
                WrapContents = false, AutoScroll = true, Padding = new Padding(8), BackColor = TampilanModern.Permukaan, Tag = "permukaan"
            };
            panelMenuAdmin.Controls.Add(labelBagianMenu("GUDANG"));
            tambahTombolMenu(panelMenuAdmin, "tombolMuatUlang", "Muat ulang", muatDaftarBarang);
            tambahTombolMenu(panelMenuAdmin, "tombolTambahBarang", "Tambah barang", tambahBarang);
            tambahTombolMenu(panelMenuAdmin, "tombolTambahKategori", "Tambah kategori", tambahKategori);
            tambahTombolMenu(panelMenuAdmin, "tombolRestokBarang", "Restok barang", restokBarangTerpilih);
            tambahTombolMenu(panelMenuAdmin, "tombolUbahBarang", "Ubah barang", ubahBarangTerpilih);
            tambahTombolMenu(panelMenuAdmin, "tombolHapusBarang", "Hapus barang", hapusBarangTerpilih);
            panelMenuAdmin.Controls.Add(labelBagianMenu("PENCARIAN & LAPORAN"));
            tambahTombolMenu(panelMenuAdmin, "tombolCariBarang", "Cari barang", cariBarangLinear);
            tambahTombolMenu(panelMenuAdmin, "tombolRecycleBin", "Recycle Bin", tampilkanRecycleBin);
            tambahTombolMenu(panelMenuAdmin, "tombolRiwayatGudang", "Riwayat gudang", () => TampilanPeran.tampilkanRiwayat(this, "Riwayat Gudang", riwayatGudang: true));
            tambahTombolMenu(panelMenuAdmin, "tombolRiwayatTransaksi", "Riwayat transaksi", () => TampilanPeran.tampilkanRiwayat(this, "Riwayat Transaksi", riwayatGudang: false));
            tambahTombolMenu(panelMenuAdmin, "tombolSummaryPenjualan", "Summary penjualan", tampilkanSummaryPenjualan);
            var panelSamping = new Panel { Name = "panelNavigasiAdmin", Dock = DockStyle.Fill, Padding = new Padding(8), BackColor = TampilanModern.Permukaan, Tag = "permukaan" };
            panelSamping.Controls.Add(panelMenuAdmin);
            var panelKonten = new Panel { Name = "panelKontenAdmin", Dock = DockStyle.Fill, Padding = new Padding(24), BackColor = TampilanModern.Latar };
            var labelDaftar = new Label { Name = "labelDaftarBarang", Text = "Persediaan barang", Dock = DockStyle.Top, Height = 42, Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = TampilanModern.Teks };
            panelKonten.Controls.Add(daftarBarang);
            panelKonten.Controls.Add(labelPetunjukBarang);
            panelKonten.Controls.Add(labelDaftar);
            var tataLetakIsi = new TableLayoutPanel { Name = "tataLetakIsiAdmin", Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, BackColor = TampilanModern.Latar };
            tataLetakIsi.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250));
            tataLetakIsi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tataLetakIsi.Controls.Add(panelSamping, 0, 0);
            tataLetakIsi.Controls.Add(panelKonten, 1, 0);
            var tataLetakAdmin = new TableLayoutPanel { Name = "tataLetakAdmin", Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
            tataLetakAdmin.RowStyles.Add(new RowStyle(SizeType.Absolute, 84));
            tataLetakAdmin.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            tataLetakAdmin.Controls.Add(TampilanModern.buatHeader("Panel Admin", "Kelola katalog, stok, dan laporan SMKMART", namaAdmin, Close), 0, 0);
            tataLetakAdmin.Controls.Add(tataLetakIsi, 0, 1);
            Controls.Clear();
            Controls.Add(tataLetakAdmin);
            TampilanModern.aturPaletForm(this);
            muatDaftarBarang();
        }

        private static Label labelBagianMenu(string teks) => new()
        {
            Text = teks, AutoSize = false, Width = 190, Height = 34,
            TextAlign = ContentAlignment.MiddleLeft, ForeColor = TampilanModern.TeksSekunder,
            Font = new Font("Segoe UI", 8.5F, FontStyle.Bold), Margin = new Padding(5, 5, 5, 2), Tag = "sekunder"
        };

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
                    ? "Belum ada barang aktif. Gunakan Tambah barang untuk mulai mengisi katalog."
                    : "Daftar dikelompokkan per kategori. Stok di bawah 5 unit diberi tanda peringatan.";
            }
            catch (Exception ex) { TampilanPeran.tampilkanKesalahanData(this, ex); }
        }

        private void tambahKategori()
        {
            string? nama = TampilanPeran.tanyaPengguna("Nama kategori baru:", "Tambah Kategori");
            if (nama is null) return;
            try
            {
                int idBarang = PenyimpananGudang.tambahKategori(nama);
                MessageBox.Show(this, $"Kategori berhasil ditambahkan dengan ID {idBarang}.", "Berhasil");
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Tambah kategori gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void tambahBarang()
        {
            string? nama = TampilanPeran.tanyaPengguna("Nama barang:", "Tambah Barang");
            if (nama is null) return;
            if (!pilihKategori(null, out int idKategori, out string? namaKategoriBaru)) return;
            if (!mintaStokDanHarga(null, out int stok, out decimal harga)) return;
            try
            {
                if (namaKategoriBaru is not null) idKategori = PenyimpananGudang.tambahKategori(namaKategoriBaru);
                PenyimpananGudang.Barang barang = PenyimpananGudang.tambahBarang(nama, idKategori, stok, harga);
                MessageBox.Show(this, $"Barang ditambahkan dengan ID {barang.IdBarang}.", "Berhasil");
                muatDaftarBarang();
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Tambah barang gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private bool pilihKategori(int? idKategoriSaatIni, out int idKategori, out string? namaKategoriBaru)
        {
            idKategori = 0;
            namaKategoriBaru = null;
            var dataKategori = PenyimpananGudang.muatKategori();
            string pilihanKategori = string.Join(Environment.NewLine, dataKategori.OrderBy(pair => pair.Key).Select(pair => $"{pair.Key}. {pair.Value}"));
            string pertanyaan = $"Kategori yang tersedia:\r\n{pilihanKategori}\r\n\r\nMasukkan ID kategori" +
                (idKategoriSaatIni.HasValue ? $" (kosongkan untuk tetap {idKategoriSaatIni.Value})" : string.Empty) + "; masukkan 0 untuk kategori baru:";
            string? masukan = TampilanPeran.tanyaPengguna(pertanyaan, "Pilih Kategori");
            if (masukan is null) return false;
            if (idKategoriSaatIni.HasValue && string.IsNullOrWhiteSpace(masukan)) { idKategori = idKategoriSaatIni.Value; return true; }
            if (!int.TryParse(masukan, out int barangTerpilih) || barangTerpilih < 0)
            { MessageBox.Show(this, "ID kategori tidak valid."); return false; }
            if (barangTerpilih == 0)
            {
                namaKategoriBaru = TampilanPeran.tanyaPengguna("Nama kategori baru:", "Tambah Kategori");
                if (string.IsNullOrWhiteSpace(namaKategoriBaru)) return false;
                string categoryCandidate = namaKategoriBaru.Trim();
                var existing = dataKategori.FirstOrDefault(pair => pair.Value.Equals(categoryCandidate, StringComparison.OrdinalIgnoreCase));
                if (!existing.Equals(default(KeyValuePair<int, string>)))
                {
                    MessageBox.Show(this, $"Kategori '{categoryCandidate}' sudah ada dengan ID {existing.Key}.", "Kategori sudah ada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                return true;
            }
            if (!dataKategori.ContainsKey(barangTerpilih)) { MessageBox.Show(this, "Kategori tidak ditemukan."); return false; }
            idKategori = barangTerpilih;
            return true;
        }

        private bool mintaStokDanHarga(PenyimpananGudang.Barang? current, out int stok, out decimal harga)
        {
            stok = current?.Stok ?? 0;
            harga = current?.Harga ?? 0;
            string? teksStok = TampilanPeran.tanyaPengguna(current is null ? "Stok awal:" : $"Stok baru (saat ini {stok}; kosongkan untuk tetap):", "Stok");
            if (teksStok is null) return false;
            if (!string.IsNullOrWhiteSpace(teksStok) && (!int.TryParse(teksStok, out stok) || stok < 0))
            { MessageBox.Show(this, "Stok harus bilangan bulat nol atau lebih."); return false; }
            if (current is null && string.IsNullOrWhiteSpace(teksStok))
            { MessageBox.Show(this, "Stok awal wajib diisi."); return false; }

            string? teksHarga = TampilanPeran.tanyaPengguna(current is null ? "Harga satuan (angka, boleh memakai titik ribuan):" : $"Harga baru (saat ini {harga:N0}; kosongkan untuk tetap):", "Harga");
            if (teksHarga is null) return false;
            if (!string.IsNullOrWhiteSpace(teksHarga) &&
                (!decimal.TryParse(teksHarga, NumberStyles.Number, CultureInfo.GetCultureInfo("id-ID"), out harga) || harga < 0))
            { MessageBox.Show(this, "Harga tidak valid."); return false; }
            if (current is null && string.IsNullOrWhiteSpace(teksHarga))
            { MessageBox.Show(this, "Harga wajib diisi."); return false; }
            return true;
        }

        private void restokBarangTerpilih()
        {
            PenyimpananGudang.Barang? barangDipilih = TampilanPeran.barangTerpilih(daftarBarang);
            if (barangDipilih is null) { MessageBox.Show(this, "Pilih barang untuk direstok."); return; }
            string? masukan = TampilanPeran.tanyaPengguna($"Jumlah tambahan untuk {barangDipilih.NamaBarang}:", "Restok");
            if (masukan is null) return;
            if (!int.TryParse(masukan, out int amount) || amount <= 0)
            { MessageBox.Show(this, "Jumlah restok harus bilangan bulat lebih dari nol."); return; }
            try
            {
                bool success = PenyimpananGudang.restokBarang(barangDipilih.IdBarang, amount, out string pesan);
                MessageBox.Show(this, pesan, success ? "Berhasil" : "Restok gagal");
                if (success) muatDaftarBarang();
            }
            catch (Exception ex) { TampilanPeran.tampilkanKesalahanData(this, ex); }
        }

        private void ubahBarangTerpilih()
        {
            PenyimpananGudang.Barang? barangDipilih = TampilanPeran.barangTerpilih(daftarBarang);
            if (barangDipilih is null || barangDipilih.StatusBarang.Equals("terhapus", StringComparison.OrdinalIgnoreCase))
            { MessageBox.Show(this, "Pilih barang aktif untuk diubah."); return; }
            string? nama = TampilanPeran.tanyaPengguna($"Nama baru (kosongkan untuk tetap '{barangDipilih.NamaBarang}'):", "Ubah Barang");
            if (nama is null) return;
            if (string.IsNullOrWhiteSpace(nama)) nama = barangDipilih.NamaBarang;
            if (!pilihKategori(barangDipilih.IdBarang / 100, out int idKategori, out string? namaKategoriBaru)) return;
            if (!mintaStokDanHarga(barangDipilih, out int stok, out decimal harga)) return;
            try
            {
                if (namaKategoriBaru is not null) idKategori = PenyimpananGudang.tambahKategori(namaKategoriBaru);
                PenyimpananGudang.Barang barangDiperbarui = PenyimpananGudang.ubahBarang(barangDipilih.IdBarang, nama, idKategori, stok, harga);
                MessageBox.Show(this, $"Barang berhasil diperbarui. ID baru: {barangDiperbarui.IdBarang}.", "Berhasil");
                muatDaftarBarang();
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Ubah barang gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void hapusBarangTerpilih()
        {
            PenyimpananGudang.Barang? barangDipilih = TampilanPeran.barangTerpilih(daftarBarang);
            if (barangDipilih is null || barangDipilih.StatusBarang.Equals("terhapus", StringComparison.OrdinalIgnoreCase))
            { MessageBox.Show(this, "Pilih barang aktif untuk dihapus."); return; }
            if (MessageBox.Show(this, $"Pindahkan {barangDipilih.NamaBarang} ke Recycle Bin?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                if (PenyimpananGudang.hapusBarang(barangDipilih.IdBarang))
                {
                    MessageBox.Show(this, $"Barang '{barangDipilih.NamaBarang}' berhasil dipindahkan ke Recycle Bin.", "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    muatDaftarBarang();
                }
                else MessageBox.Show(this, "Barang tidak ditemukan.");
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

        private void tampilkanRecycleBin()
        {
            var barangTerhapus = PenyimpananGudang.muatData().Where(barang => barang.StatusBarang.Equals("terhapus", StringComparison.OrdinalIgnoreCase)).ToList();
            if (barangTerhapus.Count == 0) { MessageBox.Show(this, "Recycle Bin kosong."); return; }
            using var formRecycleBin = new Form { Name = "formRecycleBin", Text = "Recycle Bin", Width = 980, Height = 600, MinimumSize = new Size(760, 440), StartPosition = FormStartPosition.CenterParent, BackColor = TampilanModern.Latar };
            var daftarBarangTerhapus = TampilanPeran.buatDaftarBarang("daftarBarangTerhapus");
            TampilanPeran.muatBarangKeDaftar(daftarBarangTerhapus, barangTerhapus);
            var panelMenuAdmin = new FlowLayoutPanel { Name = "panelMenuRecycleBin", Dock = DockStyle.Bottom, Height = 76, Padding = new Padding(12, 8, 12, 8), FlowDirection = FlowDirection.RightToLeft, WrapContents = false, BackColor = TampilanModern.Latar };
            var tombolRestore = TampilanModern.buatTombol("tombolRestoreBarang", "Restore");
            var tombolHapusPermanen = TampilanModern.buatTombol("tombolHapusPermanen", "Hapus permanen", sekunder: true);
            tombolRestore.Click += (_, _) =>
            {
                var barang = TampilanPeran.barangTerpilih(daftarBarangTerhapus);
                if (barang is null) { MessageBox.Show(formRecycleBin, "Pilih barang yang akan dipulihkan."); return; }
                try
                {
                    if (PenyimpananGudang.restoreBarang(barang.IdBarang))
                    {
                        MessageBox.Show(formRecycleBin, $"Barang '{barang.NamaBarang}' berhasil di-restore.", "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        formRecycleBin.DialogResult = DialogResult.OK; formRecycleBin.Close();
                    }
                }
                catch (Exception ex) { TampilanPeran.tampilkanKesalahanData(formRecycleBin, ex); }
            };
            tombolHapusPermanen.Click += (_, _) =>
            {
                var barang = TampilanPeran.barangTerpilih(daftarBarangTerhapus);
                if (barang is null) { MessageBox.Show(formRecycleBin, "Pilih barang yang akan dihapus permanen."); return; }
                if (MessageBox.Show(formRecycleBin, $"Apakah Anda yakin ingin menghapus permanen '{barang.NamaBarang}'?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
                try
                {
                    if (PenyimpananGudang.hapusPermanenBarang(barang.IdBarang))
                    {
                        MessageBox.Show(formRecycleBin, $"Barang '{barang.NamaBarang}' berhasil dihapus permanen.", "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        formRecycleBin.DialogResult = DialogResult.OK; formRecycleBin.Close();
                    }
                }
                catch (Exception ex) { TampilanPeran.tampilkanKesalahanData(formRecycleBin, ex); }
            };
            panelMenuAdmin.Controls.AddRange([tombolRestore, tombolHapusPermanen]);
            formRecycleBin.Controls.Add(daftarBarangTerhapus);
            formRecycleBin.Controls.Add(panelMenuAdmin);
            TampilanModern.aturUkuranDialog(formRecycleBin, new Size(980, 600), new Size(760, 440));
            TampilanModern.aturPaletForm(formRecycleBin);
            if (formRecycleBin.ShowDialog(this) == DialogResult.OK) muatDaftarBarang();
        }

        private void tampilkanSummaryPenjualan()
            => TampilanPeran.tampilkanSummaryPenjualan(this);
    }
}





