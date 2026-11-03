using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ProjectRPL2_
{
    internal static class PenyimpananGudang
    {
        internal static readonly string folderData = cariFolderData();

        internal static readonly string filePengguna = Path.Combine(folderData, "User.txt");
        private static string fileDataBarang => Path.Combine(folderData, "data.txt");
        private static string fileKategori => Path.Combine(folderData, "kategori.txt");
        private static string fileKeranjang => Path.Combine(folderData, "keranjang.txt");
        private static string fileRiwayatTransaksi => Path.Combine(folderData, "riwayat_transaksi.txt");
        private static string fileRiwayatGudang => Path.Combine(folderData, "riwayat_gudang.txt");
        private static readonly Encoding pengkodeanUtf8 = new UTF8Encoding(false);
        private static readonly CultureInfo budayaIndonesia = CultureInfo.GetCultureInfo("id-ID");
        private static readonly object kunciBerkas = new();

        private static string cariFolderData()
        {
            string folderDefault = AppContext.BaseDirectory;
            for (DirectoryInfo? folderSaatIni = new DirectoryInfo(AppContext.BaseDirectory);
                 folderSaatIni is not null;
                 folderSaatIni = folderSaatIni.Parent)
            {
                string fileProyek = Path.Combine(folderSaatIni.FullName, "ProjectRPL2_.csproj");
                string filePengguna = Path.Combine(folderSaatIni.FullName, "User.txt");
                string fileDataBarang = Path.Combine(folderSaatIni.FullName, "data.txt");
                if (File.Exists(fileProyek) && File.Exists(filePengguna) && File.Exists(fileDataBarang))
                    return folderSaatIni.FullName;
            }

            return folderDefault;
        }

        internal sealed record Barang(int IdBarang, string NamaBarang, int Stok, decimal Harga, string StatusBarang, string NamaKategori);
        internal sealed record BarangKeranjang(int IdBarang, string NamaBarang, int Jumlah, decimal HargaSatuan);
        internal sealed record PenjualanHarian(DateTime Tanggal, decimal TotalPenjualan);

        // Port of the Python insertion_sort / merge / tim_sort helpers.
        // The sort is stable and is used for the category-grouped inventory view.
        internal static List<T> timSort<T>(List<T> daftar, Comparison<T> bandingkan, bool urutTerbalik = false)
        {
            const int ukuranRun = 32;
            int jumlahData = daftar.Count;
            for (int awal = 0; awal < jumlahData; awal += ukuranRun)
                insertionSort(daftar, awal, Math.Min(awal + ukuranRun - 1, jumlahData - 1), bandingkan, urutTerbalik);
            for (int ukuran = ukuranRun; ukuran < jumlahData; ukuran *= 2)
            {
                for (int kiri = 0; kiri < jumlahData; kiri += 2 * ukuran)
                {
                    int tengah = Math.Min(kiri + ukuran - 1, jumlahData - 1);
                    int kanan = Math.Min(kiri + 2 * ukuran - 1, jumlahData - 1);
                    if (tengah < kanan) merge(daftar, kiri, tengah, kanan, bandingkan, urutTerbalik);
                }
                if (ukuran > jumlahData / 2) break;
            }
            return daftar;
        }

        private static void insertionSort<T>(List<T> daftar, int kiri, int kanan, Comparison<T> bandingkan, bool urutTerbalik)
        {
            for (int indeks = kiri + 1; indeks <= kanan; indeks++)
            {
                T sementara = daftar[indeks];
                int posisi = indeks - 1;
                while (posisi >= kiri && (urutTerbalik ? bandingkan(daftar[posisi], sementara) < 0 : bandingkan(daftar[posisi], sementara) > 0))
                {
                    daftar[posisi + 1] = daftar[posisi];
                    posisi--;
                }
                daftar[posisi + 1] = sementara;
            }
        }

        private static void merge<T>(List<T> daftar, int kiri, int tengah, int kanan, Comparison<T> bandingkan, bool urutTerbalik)
        {
            List<T> sisiKiri = daftar.GetRange(kiri, tengah - kiri + 1);
            List<T> sisiKanan = daftar.GetRange(tengah + 1, kanan - tengah);
            int indeksKiri = 0, indeksKanan = 0, tujuan = kiri;
            while (indeksKiri < sisiKiri.Count && indeksKanan < sisiKanan.Count)
            {
                int hasilBanding = bandingkan(sisiKiri[indeksKiri], sisiKanan[indeksKanan]);
                bool ambilKiri = urutTerbalik ? hasilBanding >= 0 : hasilBanding <= 0;
                daftar[tujuan++] = ambilKiri ? sisiKiri[indeksKiri++] : sisiKanan[indeksKanan++];
            }
            while (indeksKiri < sisiKiri.Count) daftar[tujuan++] = sisiKiri[indeksKiri++];
            while (indeksKanan < sisiKanan.Count) daftar[tujuan++] = sisiKanan[indeksKanan++];
        }

        internal static List<Barang> muatData(bool includeDeleted = true)
        {
            var kategoriMenurutId = muatKategori();
            var daftarBarang = new List<Barang>();
            if (!File.Exists(fileDataBarang)) return daftarBarang;
            foreach (string barisData in File.ReadLines(fileDataBarang, Encoding.UTF8))
            {
                if (cobaBacaBarang(barisData, kategoriMenurutId, out Barang barang) &&
                    (includeDeleted || !barang.StatusBarang.Equals("terhapus", StringComparison.OrdinalIgnoreCase)))
                    daftarBarang.Add(barang);
            }
            return timSort(daftarBarang, (barangKiri, barangKanan) => barangKiri.IdBarang.CompareTo(barangKanan.IdBarang));
        }

        internal static IReadOnlyDictionary<int, string> muatKategori()
        {
            var kategoriMenurutId = new Dictionary<int, string>();
            if (!File.Exists(fileKategori)) return kategoriMenurutId;
            foreach (string barisData in File.ReadLines(fileKategori, Encoding.UTF8))
            {
                string[] kolomData = barisData.Split(',', 2);
                if (kolomData.Length == 2 && int.TryParse(kolomData[0].Trim(), out int idBarang) && idBarang > 0 &&
                    !string.IsNullOrWhiteSpace(kolomData[1]))
                    kategoriMenurutId[idBarang] = kolomData[1].Trim();
            }
            return kategoriMenurutId;
        }

        internal static int tambahKategori(string nama)
        {
            validasiKolom(nama, "Nama kategori");
            lock (kunciBerkas)
            {
                var kategoriMenurutId = muatKategori();
                if (kategoriMenurutId.Values.Any(namaKategori => namaKategori.Equals(nama.Trim(), StringComparison.OrdinalIgnoreCase)))
                    throw new InvalidOperationException("Nama kategori sudah digunakan.");
                int idBarang = kategoriMenurutId.Count == 0 ? 1 : kategoriMenurutId.Keys.Max() + 1;
                kategoriMenurutId = new Dictionary<int, string>(kategoriMenurutId) { [idBarang] = nama.Trim() };
                string fileSementara = fileKategori + ".csharp.tmp";
                File.WriteAllLines(fileSementara, kategoriMenurutId.OrderBy(pasangan => pasangan.Key)
                    .Select(pasangan => $"{pasangan.Key},{pasangan.Value}"), pengkodeanUtf8);
                File.Move(fileSementara, fileKategori, true);
                return idBarang;
            }
        }

        internal static Barang tambahBarang(string nama, int idKategori, int stok, decimal harga)
        {
            validasiBarang(nama, stok, harga);
            lock (kunciBerkas)
            {
                var kategoriMenurutId = muatKategori();
                if (!kategoriMenurutId.TryGetValue(idKategori, out string? kategori))
                    throw new InvalidOperationException("Kategori tidak ditemukan.");
                var daftarBarang = muatData();
                if (daftarBarang.Any(barang => barang.NamaBarang.Equals(nama.Trim(), StringComparison.OrdinalIgnoreCase)))
                    throw new InvalidOperationException("Nama barang sudah digunakan, termasuk oleh barang di Recycle Bin.");
                int idBarang = buatIdBarang(idKategori, daftarBarang);
                if (idBarang == 0) throw new InvalidOperationException("ID kategori sudah penuh (maksimal 99 barang).");
                var barang = new Barang(idBarang, nama.Trim(), stok, harga, "aktif", kategori);
                var barisData = File.ReadAllLines(fileDataBarang, Encoding.UTF8).ToList();
                barisData.Add(formatBarang(barang));
                simpanDataBarang([.. barisData]);
                File.AppendAllText(fileRiwayatGudang,
                    $"[TAMBAH] Barang Baru: {barang.NamaBarang} | ID: {barang.IdBarang} | Kategori: {kategori} (ID: {idKategori}) | Jumlah: {stok} | Harga: {formatRupiah(harga)}{Environment.NewLine}", pengkodeanUtf8);
                return barang;
            }
        }

        internal static Barang ubahBarang(int idLama, string nama, int idKategori, int stok, decimal harga)
        {
            validasiBarang(nama, stok, harga);
            lock (kunciBerkas)
            {
                var kategoriMenurutId = muatKategori();
                if (!kategoriMenurutId.TryGetValue(idKategori, out string? kategori))
                    throw new InvalidOperationException("Kategori tidak ditemukan.");
                var daftarBarang = muatData();
                Barang? barangSaatIni = daftarBarang.FirstOrDefault(barang => barang.IdBarang == idLama && barang.StatusBarang.Equals("aktif", StringComparison.OrdinalIgnoreCase));
                if (barangSaatIni is null) throw new InvalidOperationException("Barang aktif tidak ditemukan.");
                if (daftarBarang.Any(barang => barang.IdBarang != idLama && barang.NamaBarang.Equals(nama.Trim(), StringComparison.OrdinalIgnoreCase)))
                    throw new InvalidOperationException("Nama barang sudah digunakan.");
                int idBaru = idLama / 100 == idKategori ? idLama : buatIdBarang(idKategori, daftarBarang);
                if (idBaru == 0) throw new InvalidOperationException("ID kategori sudah penuh (maksimal 99 barang).");
                var barangDiperbarui = barangSaatIni with { IdBarang = idBaru, NamaBarang = nama.Trim(), Stok = stok, Harga = harga, NamaKategori = kategori };
                string[] barisData = File.ReadAllLines(fileDataBarang, Encoding.UTF8);
                int indeks = Array.FindIndex(barisData, barisData => cobaBacaBarang(barisData, kategoriMenurutId, out Barang barang) && barang.IdBarang == idLama);
                if (indeks < 0) throw new InvalidOperationException("Baris barang tidak ditemukan di data.txt.");
                barisData[indeks] = formatBarang(barangDiperbarui);
                simpanDataBarang(barisData);
                return barangDiperbarui;
            }
        }

        internal static bool hapusBarang(int idBarang)
        {
            lock (kunciBerkas)
            {
                var kategoriMenurutId = muatKategori();
                string[] barisData = File.ReadAllLines(fileDataBarang, Encoding.UTF8);
                int indeks = Array.FindIndex(barisData, barisData => cobaBacaBarang(barisData, kategoriMenurutId, out Barang barang) &&
                    barang.IdBarang == idBarang && barang.StatusBarang.Equals("aktif", StringComparison.OrdinalIgnoreCase));
                if (indeks < 0) return false;
                cobaBacaBarang(barisData[indeks], kategoriMenurutId, out Barang barang);
                string[] kolomData = barisData[indeks].Split(',');
                if (kolomData.Length == 4) kolomData = [.. kolomData, "terhapus"];
                else kolomData[4] = "terhapus";
                barisData[indeks] = string.Join(',', kolomData);
                simpanDataBarang(barisData);
                File.AppendAllText(fileRiwayatGudang,
                    $"[HAPUS] {barang.NamaBarang} (ID: {barang.IdBarang}) dipindahkan ke Recycle Bin{Environment.NewLine}", pengkodeanUtf8);
                return true;
            }
        }

        internal static bool restoreBarang(int idBarang)
        {
            lock (kunciBerkas)
            {
                var kategoriMenurutId = muatKategori();
                string[] barisData = File.ReadAllLines(fileDataBarang, Encoding.UTF8);
                int indeks = Array.FindIndex(barisData, barisData => cobaBacaBarang(barisData, kategoriMenurutId, out Barang barang) &&
                    barang.IdBarang == idBarang && barang.StatusBarang.Equals("terhapus", StringComparison.OrdinalIgnoreCase));
                if (indeks < 0) return false;
                cobaBacaBarang(barisData[indeks], kategoriMenurutId, out Barang barang);
                string[] kolomData = barisData[indeks].Split(',');
                if (kolomData.Length == 4) kolomData = [.. kolomData, "aktif"];
                else kolomData[4] = "aktif";
                barisData[indeks] = string.Join(',', kolomData);
                simpanDataBarang(barisData);
                File.AppendAllText(fileRiwayatGudang,
                    $"[RESTORE] {barang.NamaBarang} (ID: {barang.IdBarang}) dikembalikan dari Recycle Bin{Environment.NewLine}", pengkodeanUtf8);
                return true;
            }
        }

        internal static bool hapusPermanenBarang(int idBarang)
        {
            lock (kunciBerkas)
            {
                var kategoriMenurutId = muatKategori();
                string[] barisData = File.ReadAllLines(fileDataBarang, Encoding.UTF8);
                int indeks = Array.FindIndex(barisData, barisData => cobaBacaBarang(barisData, kategoriMenurutId, out Barang barang) &&
                    barang.IdBarang == idBarang && barang.StatusBarang.Equals("terhapus", StringComparison.OrdinalIgnoreCase));
                if (indeks < 0) return false;
                cobaBacaBarang(barisData[indeks], kategoriMenurutId, out Barang barang);
                simpanDataBarang(barisData.Where((_, indeksBaris) => indeksBaris != indeks).ToArray());
                File.AppendAllText(fileRiwayatGudang,
                    $"[HAPUS PERMANEN] {barang.NamaBarang} (ID: {barang.IdBarang}) dihapus permanen{Environment.NewLine}", pengkodeanUtf8);
                return true;
            }
        }

        internal static List<Barang> cariBarangLinear(int pilihanCari, string kataKunci)
        {
            List<Barang> barangAktif = muatData(includeDeleted: false)
                .Where(barang => barang.StatusBarang.Equals("aktif", StringComparison.OrdinalIgnoreCase)).ToList();
            if (pilihanCari == 1 && int.TryParse(kataKunci, out int idBarang))
                return barangAktif.Where(barang => barang.IdBarang == idBarang).ToList();
            if (pilihanCari == 2)
                return barangAktif.Where(barang => barang.NamaBarang.Contains(kataKunci, StringComparison.OrdinalIgnoreCase)).ToList();
            if (pilihanCari == 3 && int.TryParse(kataKunci, out int idKategori))
                return barangAktif.Where(barang => barang.IdBarang / 100 == idKategori).ToList();
            return [];
        }

        internal static List<BarangKeranjang> muatKeranjang()
        {
            if (!File.Exists(fileKeranjang)) return [];
            var keranjangBelanja = new List<BarangKeranjang>();
            foreach (string barisData in File.ReadLines(fileKeranjang, Encoding.UTF8))
            {
                string[] kolomData = barisData.Split(',');
                if (kolomData.Length == 4 && int.TryParse(kolomData[0], out int idBarang) &&
                    !string.IsNullOrWhiteSpace(kolomData[1]) && int.TryParse(kolomData[2], out int jumlah) && jumlah > 0 &&
                    decimal.TryParse(kolomData[3], NumberStyles.Number, CultureInfo.InvariantCulture, out decimal harga) && harga >= 0)
                    keranjangBelanja.Add(new BarangKeranjang(idBarang, kolomData[1].Trim(), jumlah, harga));
            }
            return keranjangBelanja;
        }

        internal static bool tambahKeKeranjang(int idBarang, int jumlah, out string pesan)
        {
            if (jumlah <= 0) { pesan = "Jumlah harus lebih dari nol."; return false; }
            lock (kunciBerkas)
            {
                Barang? barang = muatData(includeDeleted: false).FirstOrDefault(barangKeranjang => barangKeranjang.IdBarang == idBarang && barangKeranjang.StatusBarang.Equals("aktif", StringComparison.OrdinalIgnoreCase));
                if (barang is null) { pesan = "Barang aktif tidak ditemukan."; return false; }
                int jumlahDiKeranjang = muatKeranjang().Where(barangKeranjang => barangKeranjang.IdBarang == idBarang).Sum(barangKeranjang => barangKeranjang.Jumlah);
                if (jumlah + jumlahDiKeranjang > barang.Stok)
                { pesan = $"Stok tidak cukup untuk keranjang. Tersedia {barang.Stok}, sudah dipesan {jumlahDiKeranjang}."; return false; }
                var keranjangBelanja = muatKeranjang();
                keranjangBelanja.Add(new BarangKeranjang(barang.IdBarang, barang.NamaBarang, jumlah, barang.Harga));
                simpanKeranjang(keranjangBelanja);
                pesan = $"{barang.NamaBarang} ditambahkan ke keranjang.";
                return true;
            }
        }

        internal static bool hapusDariKeranjang(int idBarang)
        {
            lock (kunciBerkas)
            {
                var keranjangBelanja = muatKeranjang();
                int indeks = keranjangBelanja.FindIndex(barangKeranjang => barangKeranjang.IdBarang == idBarang);
                if (indeks < 0) return false;
                keranjangBelanja.RemoveAt(indeks);
                simpanKeranjang(keranjangBelanja);
                return true;
            }
        }

        internal static bool checkout(string namaKasir, string metodePembayaran, decimal uangDibayarkan, out string pesan)
        {
            var metodeDiizinkan = new[] { "Cash", "Debit", "QRIS" };
            string? metode = metodeDiizinkan.FirstOrDefault(namaKategori => namaKategori.Equals(metodePembayaran, StringComparison.OrdinalIgnoreCase));
            if (metode is null) { pesan = "Metode pembayaran tidak valid."; return false; }
            lock (kunciBerkas)
            {
                var keranjangBelanja = muatKeranjang();
                if (keranjangBelanja.Count == 0) { pesan = "Keranjang masih kosong."; return false; }
                string dataBarangAsli = File.ReadAllText(fileDataBarang, Encoding.UTF8);
                bool riwayatSudahAda = File.Exists(fileRiwayatTransaksi);
                string riwayatTransaksiAsli = riwayatSudahAda ? File.ReadAllText(fileRiwayatTransaksi, Encoding.UTF8) : string.Empty;
                bool keranjangSudahAda = File.Exists(fileKeranjang);
                string keranjangAsli = File.Exists(fileKeranjang) ? File.ReadAllText(fileKeranjang, Encoding.UTF8) : string.Empty;
                var daftarBarang = muatData();
                var jumlahPerBarang = keranjangBelanja.GroupBy(barangKeranjang => barangKeranjang.IdBarang).ToDictionary(group => group.Key, group => group.Sum(barangKeranjang => barangKeranjang.Jumlah));
                foreach (var pasangan in jumlahPerBarang)
                {
                    Barang? barang = daftarBarang.FirstOrDefault(barangKeranjang => barangKeranjang.IdBarang == pasangan.Key && barangKeranjang.StatusBarang.Equals("aktif", StringComparison.OrdinalIgnoreCase));
                    if (barang is null) { pesan = $"Barang ID {pasangan.Key} tidak aktif. Hapus dari keranjang terlebih dahulu."; return false; }
                    if (pasangan.Value > barang.Stok) { pesan = $"Stok {barang.NamaBarang} tidak cukup. Tersedia {barang.Stok}."; return false; }
                }

                decimal totalHarga = keranjangBelanja.Sum(barangKeranjang => barangKeranjang.HargaSatuan * barangKeranjang.Jumlah);
                if (metode == "Cash" && uangDibayarkan < totalHarga)
                { pesan = $"Pembayaran kurang. Total belanja {formatRupiah(totalHarga)}."; return false; }
                if (metode != "Cash") uangDibayarkan = totalHarga;

                string[] barisData = File.ReadAllLines(fileDataBarang, Encoding.UTF8);
                var kategoriMenurutId = muatKategori();
                for (int indeksBaris = 0; indeksBaris < barisData.Length; indeksBaris++)
                {
                    if (!cobaBacaBarang(barisData[indeksBaris], kategoriMenurutId, out Barang barang) || !jumlahPerBarang.TryGetValue(barang.IdBarang, out int jumlah)) continue;
                    string[] kolomData = barisData[indeksBaris].Split(',');
                    kolomData[2] = (barang.Stok - jumlah).ToString(CultureInfo.InvariantCulture);
                    barisData[indeksBaris] = string.Join(',', kolomData);
                }

                simpanDataBarang(barisData);
                try
                {
                    string idTransaksi = catatTransaksi(keranjangBelanja, namaKasir, metode, totalHarga, uangDibayarkan);
                    simpanKeranjang([]);
                    pesan = string.Join(Environment.NewLine,
                        "PEMBAYARAN BERHASIL",
                        $"ID Transaksi: {idTransaksi}",
                        $"Tanggal: {DateTime.Now:yyyy-MM-dd HH:mm}",
                        $"Kasir: {namaKasir}",
                        $"Metode: {metode}",
                        "----------------------------------------",
                        string.Join(Environment.NewLine, keranjangBelanja.Select(barangKeranjang =>
                            $"[{barangKeranjang.IdBarang}] {barangKeranjang.NamaBarang} x{barangKeranjang.Jumlah} = {formatRupiah(barangKeranjang.HargaSatuan * barangKeranjang.Jumlah)}")),
                        $"TOTAL: {formatRupiah(totalHarga)}",
                        $"Dibayar: {formatRupiah(uangDibayarkan)}",
                        $"Kembalian: {formatRupiah(uangDibayarkan - totalHarga)}");
                }
                catch
                {
                    File.WriteAllText(fileDataBarang, dataBarangAsli, pengkodeanUtf8);
                    if (riwayatSudahAda) File.WriteAllText(fileRiwayatTransaksi, riwayatTransaksiAsli, pengkodeanUtf8);
                    else if (File.Exists(fileRiwayatTransaksi)) File.Delete(fileRiwayatTransaksi);
                    if (keranjangSudahAda) File.WriteAllText(fileKeranjang, keranjangAsli, pengkodeanUtf8);
                    else if (File.Exists(fileKeranjang)) File.Delete(fileKeranjang);
                    throw;
                }

                return true;
            }
        }

        internal static List<PenjualanHarian> summaryPenjualan()
        {
            if (!File.Exists(fileRiwayatTransaksi)) return [];
            string isiData = File.ReadAllText(fileRiwayatTransaksi, Encoding.UTF8);
            if (string.IsNullOrWhiteSpace(isiData)) return [];
            var hasilPenjualan = new Dictionary<DateTime, decimal>();
            foreach (string blokTransaksi in isiData.Split("=== TRANSAKSI ===", StringSplitOptions.RemoveEmptyEntries))
            {
                Match cocokTanggal = Regex.Match(blokTransaksi, @"Tanggal\s*:\s*(\d{4}-\d{2}-\d{2})");
                Match cocokTotal = Regex.Match(blokTransaksi, @"TOTAL\s*:\s*Rp([\d.]+)");
                if (!cocokTanggal.Success || !cocokTotal.Success ||
                    !DateTime.TryParseExact(cocokTanggal.Groups[1].Value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime tanggal))
                    continue;
                string angkaTotal = Regex.Replace(cocokTotal.Groups[1].Value, @"\D", string.Empty);
                if (!decimal.TryParse(angkaTotal, NumberStyles.None, CultureInfo.InvariantCulture, out decimal totalHarga)) continue;
                tanggal = tanggal.Date;
                hasilPenjualan[tanggal] = hasilPenjualan.GetValueOrDefault(tanggal) + totalHarga;
            }
            return hasilPenjualan.OrderBy(pasangan => pasangan.Key).Select(pasangan => new PenjualanHarian(pasangan.Key, pasangan.Value)).ToList();
        }

        internal static int hitungTransaksiHariIni(DateTime tanggalSaatIni)
        {
            if (!File.Exists(fileRiwayatTransaksi)) return 0;
            string tanggal = tanggalSaatIni.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            return File.ReadLines(fileRiwayatTransaksi, Encoding.UTF8)
                .Count(barisData => Regex.IsMatch(barisData, @"^Tanggal\s*:\s*" + Regex.Escape(tanggal), RegexOptions.CultureInvariant));
        }

        internal static bool restokBarang(int idBarang, int jumlahTambahan, out string pesan)
        {
            if (jumlahTambahan <= 0) { pesan = "Jumlah restok harus lebih dari nol."; return false; }
            lock (kunciBerkas)
            {
                string dataAsli = File.ReadAllText(fileDataBarang, Encoding.UTF8);
                string[] barisData = File.ReadAllLines(fileDataBarang, Encoding.UTF8);
                for (int indeksBaris = 0; indeksBaris < barisData.Length; indeksBaris++)
                {
                    if (!cobaBacaBarang(barisData[indeksBaris], muatKategori(), out Barang barang) || barang.IdBarang != idBarang)
                        continue;
                    if (!barang.StatusBarang.Equals("aktif", StringComparison.OrdinalIgnoreCase))
                    { pesan = "Produk tidak berstatus aktif."; return false; }
                    int stokBaru;
                    try { stokBaru = checked(barang.Stok + jumlahTambahan); }
                    catch (OverflowException) { pesan = "Jumlah stok melebihi batas."; return false; }

                    string[] kolomData = barisData[indeksBaris].Split(',');
                    kolomData[2] = stokBaru.ToString(CultureInfo.InvariantCulture);
                    if (kolomData.Length == 4) kolomData = [.. kolomData, "aktif"];
                    barisData[indeksBaris] = string.Join(',', kolomData);
                    simpanDataBarang(barisData);
                    try
                    {
                        File.AppendAllText(fileRiwayatGudang,
                            $"[RESTOK] {barang.NamaBarang} +{jumlahTambahan} (Total Stok: {stokBaru}){Environment.NewLine}", pengkodeanUtf8);
                    }
                    catch
                    {
                        File.WriteAllText(fileDataBarang, dataAsli, pengkodeanUtf8);
                        throw;
                    }
                    pesan = $"Stok {barang.NamaBarang} berhasil ditambah menjadi {stokBaru}.";
                    return true;
                }
            }
            pesan = "Produk tidak ditemukan.";
            return false;
        }

        internal static string bacaRiwayat(bool warehouse)
        {
            string jalurBerkas = warehouse ? fileRiwayatGudang : fileRiwayatTransaksi;
            return File.Exists(jalurBerkas) ? File.ReadAllText(jalurBerkas, Encoding.UTF8) : "Belum ada riwayat.";
        }

        internal static string formatRupiah(decimal jumlahTambahan) => jumlahTambahan.ToString("C0", budayaIndonesia);

        private static void validasiKolom(string namaKategori, string label)
        {
            if (string.IsNullOrWhiteSpace(namaKategori) || namaKategori.Contains(',') || namaKategori.Contains('\n') || namaKategori.Contains('\r'))
                throw new ArgumentException($"{label} wajib diisi dan tidak boleh mengandung koma atau baris baru.");
        }

        private static void validasiBarang(string nama, int stok, decimal harga)
        {
            validasiKolom(nama, "Nama barang");
            if (stok < 0) throw new ArgumentOutOfRangeException(nameof(stok), "Stok tidak boleh negatif.");
            if (harga < 0) throw new ArgumentOutOfRangeException(nameof(harga), "Harga tidak boleh negatif.");
        }

        private static int buatIdBarang(int idKategori, List<Barang> daftarBarang)
        {
            if (idKategori <= 0) return 0;
            int idDasar;
            try { idDasar = checked(idKategori * 100); }
            catch (OverflowException) { return 0; }
            int urutanMaksimum = daftarBarang.Where(barang => barang.IdBarang / 100 == idKategori)
                .Select(barang => barang.IdBarang - idDasar).DefaultIfEmpty(0).Max();
            return urutanMaksimum >= 99 ? 0 : idDasar + urutanMaksimum + 1;
        }

        private static string formatBarang(Barang barang) =>
            $"{barang.IdBarang},{barang.NamaBarang},{barang.Stok},{barang.Harga.ToString("0.##", CultureInfo.InvariantCulture)},{barang.StatusBarang}";

        private static void simpanKeranjang(IEnumerable<BarangKeranjang> keranjangBelanja)
        {
            string fileSementara = fileKeranjang + ".csharp.tmp";
            File.WriteAllLines(fileSementara, keranjangBelanja.Select(barangKeranjang =>
                $"{barangKeranjang.IdBarang},{barangKeranjang.NamaBarang},{barangKeranjang.Jumlah},{barangKeranjang.HargaSatuan.ToString("0.##", CultureInfo.InvariantCulture)}"), pengkodeanUtf8);
            File.Move(fileSementara, fileKeranjang, true);
        }

        private static bool cobaBacaBarang(string barisData, IReadOnlyDictionary<int, string> kategoriMenurutId, out Barang barang)
        {
            barang = null!;
            if (string.IsNullOrWhiteSpace(barisData)) return false;
            string[] kolomData = barisData.Split(',');
            if (kolomData.Length is not (4 or 5) ||
                !int.TryParse(kolomData[0].Trim(), out int idBarang) || idBarang <= 0 ||
                string.IsNullOrWhiteSpace(kolomData[1]) ||
                !int.TryParse(kolomData[2].Trim(), out int stok) || stok < 0 ||
                !decimal.TryParse(kolomData[3].Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal harga) || harga < 0)
                return false;

            string statusBarang = kolomData.Length == 5 ? kolomData[4].Trim() : "aktif";
            int idKategori = idBarang / 100;
            string kategori = kategoriMenurutId.TryGetValue(idKategori, out string? nama) ? nama : "Lainnya";
            barang = new Barang(idBarang, kolomData[1].Trim(), stok, harga, statusBarang, kategori);
            return true;
        }

        private static void simpanDataBarang(string[] barisData)
        {
            string fileSementara = fileDataBarang + ".csharp.tmp";
            File.WriteAllLines(fileSementara, barisData, pengkodeanUtf8);
            File.Move(fileSementara, fileDataBarang, true);
        }

        private static string catatTransaksi(List<BarangKeranjang> keranjangBelanja, string namaKasir, string metodePembayaran,
            decimal totalHarga, decimal uangDibayar)
        {
            DateTime now = DateTime.Now;
            int jumlahTransaksi = hitungTransaksiHariIni(now);
            decimal kembalian = uangDibayar - totalHarga;
            string idTransaksi = $"SMK{jumlahTransaksi + 1:000}{now:yyyyMMddHHmmss}";
            var barisData = new List<string>
            {
                "=== TRANSAKSI ===",
                $"ID Transaksi: {idTransaksi}",
                $"Tanggal : {now:yyyy-MM-dd HH:mm}",
                $"Kasir   : {namaKasir}",
                $"Metode  : {metodePembayaran}",
                "----------------------------------------"
            };
            foreach (BarangKeranjang barangKeranjang in keranjangBelanja)
                barisData.Add($"- [{barangKeranjang.IdBarang}] {barangKeranjang.NamaBarang} x{barangKeranjang.Jumlah} = {formatRupiah(barangKeranjang.HargaSatuan * barangKeranjang.Jumlah)}");
            barisData.AddRange([
                $"TOTAL: {formatRupiah(totalHarga)}",
                $"Dibayar: {formatRupiah(uangDibayar)}",
                $"Kembalian: {formatRupiah(kembalian)}",
                "----------------------------------------", "", ""]);
            File.AppendAllText(fileRiwayatTransaksi, string.Join(Environment.NewLine, barisData), pengkodeanUtf8);
            return idTransaksi;
        }
    }
}




