using System.Globalization;
using System.Windows.Forms;

namespace ProjectRPL2_
{
    public partial class FormAdmin : Form
    {
        private readonly ListBox daftarBarang = new() { Name = "daftarBarang", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        private List<PenyimpananGudang.Barang> dataBarang = [];

        public FormAdmin(string namaAdmin)
        {
            InitializeComponent();
            Text = $"SMKMART - Admin ({namaAdmin})";

            var panelMenuAdmin = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 88, Padding = new Padding(6), WrapContents = true };
            tambahTombolMenu(panelMenuAdmin, "tombolMuatUlang", "Muat ulang", muatDaftarBarang);
            tambahTombolMenu(panelMenuAdmin, "tombolTambahBarang", "Tambah barang", tambahBarang);
            tambahTombolMenu(panelMenuAdmin, "tombolTambahKategori", "Tambah kategori", tambahKategori);
            tambahTombolMenu(panelMenuAdmin, "tombolRestokBarang", "Restok", restokBarangTerpilih);
            tambahTombolMenu(panelMenuAdmin, "tombolUbahBarang", "Ubah barang", ubahBarangTerpilih);
            tambahTombolMenu(panelMenuAdmin, "tombolHapusBarang", "Hapus barang", hapusBarangTerpilih);
            tambahTombolMenu(panelMenuAdmin, "tombolCariBarang", "Cari barang", cariBarangLinear);
            tambahTombolMenu(panelMenuAdmin, "tombolRecycleBin", "Recycle Bin", tampilkanRecycleBin);
            tambahTombolMenu(panelMenuAdmin, "tombolRiwayatGudang", "Riwayat gudang", () => TampilanPeran.tampilkanRiwayat(this, "Riwayat Gudang", riwayatGudang: true));
            tambahTombolMenu(panelMenuAdmin, "tombolRiwayatTransaksi", "Riwayat transaksi", () => TampilanPeran.tampilkanRiwayat(this, "Riwayat Transaksi", riwayatGudang: false));
            tambahTombolMenu(panelMenuAdmin, "tombolSummaryPenjualan", "Summary penjualan", tampilkanSummaryPenjualan);
            Controls.Add(daftarBarang);
            Controls.Add(panelMenuAdmin);
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
                dataBarang = PenyimpananGudang.muatData();
                daftarBarang.Items.Clear();
                foreach (var barang in dataBarang)
                    daftarBarang.Items.Add(TampilanPeran.formatBarang(barang));
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
                    idKategori = existing.Key;
                    namaKategoriBaru = null;
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
            PenyimpananGudang.Barang? barangDipilih = barangTerpilih();
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
            PenyimpananGudang.Barang? barangDipilih = barangTerpilih();
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
            PenyimpananGudang.Barang? barangDipilih = barangTerpilih();
            if (barangDipilih is null || barangDipilih.StatusBarang.Equals("terhapus", StringComparison.OrdinalIgnoreCase))
            { MessageBox.Show(this, "Pilih barang aktif untuk dihapus."); return; }
            if (MessageBox.Show(this, $"Pindahkan {barangDipilih.NamaBarang} ke Recycle Bin?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                if (PenyimpananGudang.hapusBarang(barangDipilih.IdBarang)) muatDaftarBarang();
                else MessageBox.Show(this, "Barang tidak ditemukan.");
            }
            catch (Exception ex) { TampilanPeran.tampilkanKesalahanData(this, ex); }
        }

        private PenyimpananGudang.Barang? barangTerpilih()
        {
            int indeks = daftarBarang.SelectedIndex;
            return indeks >= 0 && indeks < dataBarang.Count ? dataBarang[indeks] : null;
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

        private void tampilkanRecycleBin()
        {
            var barangTerhapus = PenyimpananGudang.muatData().Where(barang => barang.StatusBarang.Equals("terhapus", StringComparison.OrdinalIgnoreCase)).ToList();
            if (barangTerhapus.Count == 0) { MessageBox.Show(this, "Recycle Bin kosong."); return; }
            using var formRecycleBin = new Form { Text = "Recycle Bin", Width = 760, Height = 430, StartPosition = FormStartPosition.CenterParent };
            var daftarBarangTerhapus = new ListBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
            foreach (var barang in barangTerhapus) daftarBarangTerhapus.Items.Add(TampilanPeran.formatBarang(barang));
            var panelMenuAdmin = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 44 };
            var tombolRestore = new Button { Text = "Restore", AutoSize = true };
            var tombolHapusPermanen = new Button { Text = "Hapus permanen", AutoSize = true };
            tombolRestore.Click += (_, _) =>
            {
                int indeks = daftarBarangTerhapus.SelectedIndex;
                if (indeks < 0) return;
                try { if (PenyimpananGudang.restoreBarang(barangTerhapus[indeks].IdBarang)) { formRecycleBin.DialogResult = DialogResult.OK; formRecycleBin.Close(); } }
                catch (Exception ex) { TampilanPeran.tampilkanKesalahanData(formRecycleBin, ex); }
            };
            tombolHapusPermanen.Click += (_, _) =>
            {
                int indeks = daftarBarangTerhapus.SelectedIndex;
                if (indeks < 0) return;
                if (MessageBox.Show(formRecycleBin, $"Hapus {barangTerhapus[indeks].NamaBarang} permanen?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
                try { if (PenyimpananGudang.hapusPermanenBarang(barangTerhapus[indeks].IdBarang)) { formRecycleBin.DialogResult = DialogResult.OK; formRecycleBin.Close(); } }
                catch (Exception ex) { TampilanPeran.tampilkanKesalahanData(formRecycleBin, ex); }
            };
            panelMenuAdmin.Controls.AddRange([tombolRestore, tombolHapusPermanen]);
            formRecycleBin.Controls.Add(daftarBarangTerhapus);
            formRecycleBin.Controls.Add(panelMenuAdmin);
            if (formRecycleBin.ShowDialog(this) == DialogResult.OK) muatDaftarBarang();
        }

        private void tampilkanSummaryPenjualan()
            => TampilanPeran.tampilkanSummaryPenjualan(this);
    }
}





