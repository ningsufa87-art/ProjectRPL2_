using System.Globalization;
using System.Windows.Forms;

namespace ProjectRPL2_
{
    internal static class TampilanPeran
    {
        internal static ListView buatDaftarBarang(string namaKontrol = "daftarBarang")
        {
            var daftarBarang = new ListView
            {
                Name = namaKontrol,
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                MultiSelect = false,
                HideSelection = false,
                BorderStyle = BorderStyle.None,
                BackColor = TampilanModern.Permukaan,
                ForeColor = TampilanModern.Teks,
                Font = new Font("Segoe UI", 10F),
                UseCompatibleStateImageBehavior = false,
                Tag = "permukaan"
            };
            daftarBarang.Columns.Add("ID", 76, HorizontalAlignment.Left);
            daftarBarang.Columns.Add("Nama Barang", 250, HorizontalAlignment.Left);
            daftarBarang.Columns.Add("Kategori", 190, HorizontalAlignment.Left);
            daftarBarang.Columns.Add("Stok", 90, HorizontalAlignment.Right);
            daftarBarang.Columns.Add("Harga", 145, HorizontalAlignment.Right);
            daftarBarang.Columns.Add("Status", 150, HorizontalAlignment.Left);
            daftarBarang.Resize += (_, _) => aturLebarKolomBarang(daftarBarang);
            aturLebarKolomBarang(daftarBarang);
            return daftarBarang;
        }

        private static void aturLebarKolomBarang(ListView daftar)
        {
            if (daftar.Columns.Count < 6 || daftar.ClientSize.Width <= 0) return;
            int lebarIsi = Math.Max(480, daftar.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 4);
            int id = Math.Max(54, (int)(lebarIsi * 0.09));
            int nama = Math.Max(110, (int)(lebarIsi * 0.27));
            int kategori = Math.Max(90, (int)(lebarIsi * 0.18));
            int stok = Math.Max(58, (int)(lebarIsi * 0.10));
            int harga = Math.Max(86, (int)(lebarIsi * 0.16));
            daftar.Columns[0].Width = id;
            daftar.Columns[1].Width = nama;
            daftar.Columns[2].Width = kategori;
            daftar.Columns[3].Width = stok;
            daftar.Columns[4].Width = harga;
            daftar.Columns[5].Width = Math.Max(90, lebarIsi - id - nama - kategori - stok - harga);
        }

        private static void aturLebarKolomKeranjang(ListView daftar)
        {
            if (daftar.Columns.Count < 5 || daftar.ClientSize.Width <= 0) return;
            int lebarIsi = Math.Max(460, daftar.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 4);
            int id = Math.Max(50, (int)(lebarIsi * 0.08));
            int nama = Math.Max(110, (int)(lebarIsi * 0.30));
            int jumlah = Math.Max(55, (int)(lebarIsi * 0.10));
            int harga = Math.Max(85, (int)(lebarIsi * 0.22));
            daftar.Columns[0].Width = id;
            daftar.Columns[1].Width = nama;
            daftar.Columns[2].Width = jumlah;
            daftar.Columns[3].Width = harga;
            daftar.Columns[4].Width = Math.Max(90, lebarIsi - id - nama - jumlah - harga);
        }

        internal static void muatBarangKeDaftar(ListView kontrolDaftar, IEnumerable<PenyimpananGudang.Barang> barang, bool kelompokKategori = true)
        {
            kontrolDaftar.BeginUpdate();
            try
            {
                kontrolDaftar.Items.Clear();
                kontrolDaftar.Groups.Clear();
                var daftarTerurut = PenyimpananGudang.timSort(barang.ToList(), (barangKiri, barangKanan) =>
                {
                    int bandingKategori = barangKiri.IdBarang / 100 == barangKanan.IdBarang / 100
                        ? 0 : (barangKiri.IdBarang / 100).CompareTo(barangKanan.IdBarang / 100);
                    return bandingKategori != 0 ? bandingKategori : barangKiri.IdBarang.CompareTo(barangKanan.IdBarang);
                });
                var grupKategori = new Dictionary<int, ListViewGroup>();
                foreach (PenyimpananGudang.Barang barangSaatIni in daftarTerurut)
                {
                    int idKategori = barangSaatIni.IdBarang / 100;
                    if (kelompokKategori && !grupKategori.TryGetValue(idKategori, out _))
                    {
                        var grup = new ListViewGroup($"{barangSaatIni.NamaKategori}  ·  ID {idKategori}  ·  Range {idKategori * 100 + 1}–{idKategori * 100 + 99}", HorizontalAlignment.Left);
                        grupKategori[idKategori] = grup;
                        kontrolDaftar.Groups.Add(grup);
                    }
                    string status = barangSaatIni.StatusBarang;
                    if (barangSaatIni.StatusBarang.Equals("aktif", StringComparison.OrdinalIgnoreCase) && barangSaatIni.Stok < 5)
                        status = "⚠ Stok rendah";
                    var barisBarang = new ListViewItem(barangSaatIni.IdBarang.ToString(CultureInfo.InvariantCulture))
                    {
                        Tag = barangSaatIni,
                        Group = kelompokKategori ? grupKategori[idKategori] : null
                    };
                    barisBarang.SubItems.Add(barangSaatIni.NamaBarang);
                    barisBarang.SubItems.Add(barangSaatIni.NamaKategori);
                    barisBarang.SubItems.Add(barangSaatIni.Stok.ToString("N0", CultureInfo.GetCultureInfo("id-ID")));
                    barisBarang.SubItems.Add(PenyimpananGudang.formatRupiah(barangSaatIni.Harga));
                    barisBarang.SubItems.Add(status);
                    if (status.StartsWith("⚠", StringComparison.Ordinal))
                    {
                        barisBarang.UseItemStyleForSubItems = false;
                        barisBarang.SubItems[5].ForeColor = Color.FromArgb(173, 99, 17);
                        barisBarang.SubItems[5].Font = new Font(kontrolDaftar.Font, FontStyle.Bold);
                    }
                    kontrolDaftar.Items.Add(barisBarang);
                }
                aturLebarKolomBarang(kontrolDaftar);
                if (kontrolDaftar.Items.Count > 0)
                {
                    kontrolDaftar.Items[0].Selected = true;
                    kontrolDaftar.Items[0].Focused = true;
                }
            }
            finally { kontrolDaftar.EndUpdate(); }
        }

        internal static PenyimpananGudang.Barang? barangTerpilih(ListView kontrolDaftar) =>
            kontrolDaftar.SelectedItems.Count > 0 ? kontrolDaftar.SelectedItems[0].Tag as PenyimpananGudang.Barang : null;

        internal static ListView buatDaftarKeranjang(string namaKontrol = "daftarIsiKeranjang")
        {
            var daftar = new ListView
            {
                Name = namaKontrol, Dock = DockStyle.Fill, View = View.Details,
                FullRowSelect = true, GridLines = true, MultiSelect = false,
                HideSelection = false, BorderStyle = BorderStyle.None,
                BackColor = TampilanModern.Permukaan, Font = new Font("Segoe UI", 10F),
                UseCompatibleStateImageBehavior = false, Tag = "permukaan"
            };
            daftar.Columns.Add("ID", 70);
            daftar.Columns.Add("Nama Barang", 280);
            daftar.Columns.Add("Jumlah", 90, HorizontalAlignment.Right);
            daftar.Columns.Add("Harga Satuan", 150, HorizontalAlignment.Right);
            daftar.Columns.Add("Subtotal", 160, HorizontalAlignment.Right);
            daftar.Resize += (_, _) => aturLebarKolomKeranjang(daftar);
            aturLebarKolomKeranjang(daftar);
            return daftar;
        }

        internal static void muatKeranjangKeDaftar(ListView kontrolDaftar, IEnumerable<PenyimpananGudang.BarangKeranjang> keranjangBelanja)
        {
            kontrolDaftar.Items.Clear();
            foreach (var item in keranjangBelanja)
            {
                var baris = new ListViewItem(item.IdBarang.ToString(CultureInfo.InvariantCulture)) { Tag = item };
                baris.SubItems.Add(item.NamaBarang);
                baris.SubItems.Add(item.Jumlah.ToString(CultureInfo.GetCultureInfo("id-ID")));
                baris.SubItems.Add(PenyimpananGudang.formatRupiah(item.HargaSatuan));
                baris.SubItems.Add(PenyimpananGudang.formatRupiah(item.HargaSatuan * item.Jumlah));
                kontrolDaftar.Items.Add(baris);
            }
        }

        internal static string formatBarang(PenyimpananGudang.Barang barang) =>
            $"{barang.IdBarang} | {barang.NamaBarang} | {barang.NamaKategori} | Stok {barang.Stok} | " +
            $"{PenyimpananGudang.formatRupiah(barang.Harga)} | {barang.StatusBarang}" +
            (barang.StatusBarang.Equals("aktif", StringComparison.OrdinalIgnoreCase) && barang.Stok < 5 ? " | STOK RENDAH" : string.Empty);

        internal static void tampilkanDaftarBarang(IWin32Window pemilikForm, string judul, IEnumerable<PenyimpananGudang.Barang> barang)
        {
            var dataBarang = barang.ToList();
            if (dataBarang.Count == 0)
            {
                MessageBox.Show(pemilikForm, "Barang tidak ditemukan.", judul, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using var formDaftar = new Form
            {
                Name = "formHasilPencarianBarang", Text = judul, Width = 980, Height = 600,
                MinimumSize = new Size(760, 440), StartPosition = FormStartPosition.CenterParent,
                BackColor = TampilanModern.Latar, Font = new Font("Segoe UI", 10F)
            };
            var panelIsi = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20), BackColor = TampilanModern.Latar };
            var daftarBarang = buatDaftarBarang("daftarHasilPencarian");
            muatBarangKeDaftar(daftarBarang, dataBarang, kelompokKategori: true);
            var labelHasil = new Label
            {
                Text = $"HASIL PENCARIAN  ·  {dataBarang.Count} barang", Dock = DockStyle.Top, Height = 42,
                Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = TampilanModern.Teks
            };
            panelIsi.Controls.Add(daftarBarang);
            panelIsi.Controls.Add(labelHasil);
            formDaftar.Controls.Add(panelIsi);
            TampilanModern.aturUkuranDialog(formDaftar, new Size(980, 600), new Size(760, 440));
            TampilanModern.aturPaletForm(formDaftar);
            formDaftar.ShowDialog(pemilikForm);
        }

        internal static void tampilkanRiwayat(IWin32Window pemilikForm, string judul, bool riwayatGudang)
        {
            string isiRiwayat = PenyimpananGudang.bacaRiwayat(riwayatGudang);
            if (string.IsNullOrWhiteSpace(isiRiwayat)) isiRiwayat = "Belum ada riwayat.";
            tampilkanTeks(pemilikForm, judul, isiRiwayat.Trim());
        }

        internal static void tampilkanTeks(IWin32Window pemilikForm, string judul, string isi)
        {
            using var formRiwayat = new Form
            {
                Name = "formTampilanRiwayat", Text = judul, Width = 900, Height = 600,
                StartPosition = FormStartPosition.CenterParent, MinimumSize = new Size(620, 420),
                BackColor = TampilanModern.Latar, Font = new Font("Segoe UI", 10F)
            };
            var labelJudul = new Label
            {
                Text = judul.ToUpperInvariant(), Dock = DockStyle.Top, Height = 54,
                Font = new Font("Segoe UI", 15, FontStyle.Bold), ForeColor = TampilanModern.Teks,
                Padding = new Padding(22, 12, 0, 0)
            };
            var kotakTeksRiwayat = new RichTextBox
            {
                Name = "kotakTeksRiwayat", ReadOnly = true, DetectUrls = false,
                ScrollBars = RichTextBoxScrollBars.Both, WordWrap = false, Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10), Text = isi, BackColor = TampilanModern.Permukaan, Tag = "permukaan",
                BorderStyle = BorderStyle.None, Margin = new Padding(18)
            };
            formRiwayat.Controls.Add(kotakTeksRiwayat);
            formRiwayat.Controls.Add(labelJudul);
            TampilanModern.aturUkuranDialog(formRiwayat, new Size(900, 600), new Size(620, 420));
            TampilanModern.aturPaletForm(formRiwayat);
            formRiwayat.ShowDialog(pemilikForm);
        }

        internal static void tampilkanSummaryPenjualan(IWin32Window pemilikForm)
        {
            try
            {
                List<PenyimpananGudang.PenjualanHarian> dataPenjualan = PenyimpananGudang.summaryPenjualan();
                if (dataPenjualan.Count == 0)
                {
                    MessageBox.Show(pemilikForm, "Belum ada riwayat transaksi atau format transaksi belum dapat dibaca.", "Summary Penjualan", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var bulanTersedia = dataPenjualan.GroupBy(penjualan => penjualan.Tanggal.ToString("yyyy-MM", CultureInfo.InvariantCulture))
                    .OrderBy(grup => grup.Key).ToList();
                using var formPilihBulan = new Form
                {
                    Name = "formPilihBulan", Text = "Summary Penjualan", Width = 540, Height = 250,
                    StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog,
                    MinimizeBox = false, MaximizeBox = false, BackColor = TampilanModern.Latar,
                    Font = new Font("Segoe UI", 10F)
                };
                var labelPilihBulan = new Label
                {
                    Text = "Pilih bulan penjualan yang ingin ditampilkan:", Dock = DockStyle.Fill,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = TampilanModern.Teks
                };
                var pilihanBulan = new ComboBox
                {
                    Name = "pilihanBulan", Dock = DockStyle.Top, Height = 40, Tag = "permukaan",
                    DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 11F), Margin = new Padding(0, 10, 0, 0)
                };
                var budaya = CultureInfo.GetCultureInfo("id-ID");
                foreach (var bulan in bulanTersedia)
                {
                    DateTime tanggalBulan = DateTime.ParseExact(bulan.Key + "-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    pilihanBulan.Items.Add(new PilihanBulan(bulan.Key, tanggalBulan.ToString("MMMM yyyy", budaya)));
                }
                pilihanBulan.DisplayMember = nameof(PilihanBulan.Teks);
                if (pilihanBulan.Items.Count > 0) pilihanBulan.SelectedIndex = pilihanBulan.Items.Count - 1;
                var tombolLihat = TampilanModern.buatTombol("tombolLihatSummary", "Lihat laporan");
                tombolLihat.DialogResult = DialogResult.None;
                var tombolKembali = TampilanModern.buatTombol("tombolKembaliSummary", "Kembali", sekunder: true);
                tombolKembali.DialogResult = DialogResult.Cancel;
                tombolLihat.Click += (_, _) =>
                {
                    if (pilihanBulan.SelectedItem is not PilihanBulan bulanDipilih) return;
                    var transaksiBulanan = dataPenjualan.Where(penjualan => penjualan.Tanggal.ToString("yyyy-MM", CultureInfo.InvariantCulture) == bulanDipilih.Kunci).ToList();
                    tampilkanLaporanBulanan(formPilihBulan, bulanDipilih.Teks, transaksiBulanan);
                    pilihanBulan.Focus();
                };
                var panelTombol = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, WrapContents = false,
                    BackColor = TampilanModern.Latar, Padding = new Padding(0, 8, 0, 0)
                };
                panelTombol.Controls.Add(tombolLihat);
                panelTombol.Controls.Add(tombolKembali);
                var tataLetakDialog = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill, Padding = new Padding(20), ColumnCount = 1, RowCount = 3,
                    BackColor = TampilanModern.Latar
                };
                tataLetakDialog.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
                tataLetakDialog.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                tataLetakDialog.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
                tataLetakDialog.Controls.Add(labelPilihBulan, 0, 0);
                tataLetakDialog.Controls.Add(pilihanBulan, 0, 1);
                tataLetakDialog.Controls.Add(panelTombol, 0, 2);
                formPilihBulan.Controls.Add(tataLetakDialog);
                formPilihBulan.AcceptButton = tombolLihat;
                formPilihBulan.CancelButton = tombolKembali;
                TampilanModern.aturUkuranDialog(formPilihBulan, new Size(520, 235), new Size(460, 220));
                TampilanModern.aturPaletForm(formPilihBulan);
                formPilihBulan.ShowDialog(pemilikForm);
            }
            catch (Exception ex) { tampilkanKesalahanData(pemilikForm, ex); }
        }

        private sealed record PilihanBulan(string Kunci, string Teks);

        private static void tampilkanLaporanBulanan(IWin32Window pemilikForm, string judulBulan, List<PenyimpananGudang.PenjualanHarian> dataHarian)
        {
            using var formLaporan = new Form
            {
                Name = "formLaporanBulanan", Text = $"Penjualan {judulBulan}", Width = 680, Height = 540,
                MinimumSize = new Size(560, 420), StartPosition = FormStartPosition.CenterParent,
                BackColor = TampilanModern.Latar, Font = new Font("Segoe UI", 10F)
            };
            var panelIsi = new Panel { Dock = DockStyle.Fill, Padding = new Padding(22), BackColor = TampilanModern.Latar };
            var labelJudul = new Label
            {
                Text = $"PENJUALAN {judulBulan.ToUpperInvariant()}", Dock = DockStyle.Top, Height = 54,
                Font = new Font("Segoe UI", 15, FontStyle.Bold), ForeColor = TampilanModern.Teks
            };
            var daftarPenjualan = new ListView
            {
                Name = "daftarPenjualanHarian", Dock = DockStyle.Fill, View = View.Details,
                FullRowSelect = true, GridLines = true, BackColor = TampilanModern.Permukaan, BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10F), UseCompatibleStateImageBehavior = false, Tag = "permukaan"
            };
            daftarPenjualan.Columns.Add("Tanggal", 300);
            daftarPenjualan.Columns.Add("Total Penjualan", 240, HorizontalAlignment.Right);
            void aturKolomLaporan()
            {
                int lebarIsi = Math.Max(320, daftarPenjualan.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 4);
                daftarPenjualan.Columns[0].Width = (int)(lebarIsi * 0.56);
                daftarPenjualan.Columns[1].Width = lebarIsi - daftarPenjualan.Columns[0].Width;
            }
            daftarPenjualan.Resize += (_, _) => aturKolomLaporan();
            aturKolomLaporan();
            foreach (var penjualan in dataHarian.OrderBy(penjualan => penjualan.Tanggal))
            {
                var baris = new ListViewItem(penjualan.Tanggal.ToString("dd MMMM yyyy", CultureInfo.GetCultureInfo("id-ID")));
                baris.SubItems.Add(PenyimpananGudang.formatRupiah(penjualan.TotalPenjualan));
                daftarPenjualan.Items.Add(baris);
            }
            var labelTotal = new Label
            {
                Text = $"TOTAL BULAN   {PenyimpananGudang.formatRupiah(dataHarian.Sum(penjualan => penjualan.TotalPenjualan))}",
                Dock = DockStyle.Bottom, Height = 54, TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = TampilanModern.Aksen
            };
            panelIsi.Controls.Add(daftarPenjualan);
            panelIsi.Controls.Add(labelTotal);
            panelIsi.Controls.Add(labelJudul);
            formLaporan.Controls.Add(panelIsi);
            TampilanModern.aturUkuranDialog(formLaporan, new Size(680, 540), new Size(560, 420));
            TampilanModern.aturPaletForm(formLaporan);
            formLaporan.ShowDialog(pemilikForm);
        }

        internal static string? tanyaPengguna(string pertanyaan, string judul)
        {
            using var formInput = new Form
            {
                Name = "formInput", Text = judul,
                StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog,
                MinimizeBox = false, MaximizeBox = false, BackColor = TampilanModern.Latar,
                Font = new Font("Segoe UI", 10F), AutoScaleMode = AutoScaleMode.Dpi
            };
            var labelPertanyaan = new RichTextBox
            {
                Name = "labelPertanyaan", Text = pertanyaan, Dock = DockStyle.Fill,
                ReadOnly = true, BorderStyle = BorderStyle.None, ScrollBars = RichTextBoxScrollBars.Vertical,
                DetectUrls = false, Font = new Font("Segoe UI", 10F), BackColor = TampilanModern.Latar,
                ForeColor = TampilanModern.Teks
            };
            var inputJawaban = new TextBox
            {
                Name = "inputJawaban", Dock = DockStyle.Fill, Tag = "permukaan", Font = new Font("Segoe UI", 11F)
            };
            var tombolSetuju = TampilanModern.buatTombol("tombolSetuju", "Lanjut");
            tombolSetuju.DialogResult = DialogResult.OK;
            var tombolBatal = TampilanModern.buatTombol("tombolBatal", "Batal", sekunder: true);
            tombolBatal.DialogResult = DialogResult.Cancel;
            var panelTombol = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, WrapContents = false,
                BackColor = TampilanModern.Latar, Padding = new Padding(0, 5, 0, 0)
            };
            panelTombol.Controls.Add(tombolSetuju);
            panelTombol.Controls.Add(tombolBatal);
            var tataLetakInput = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, Padding = new Padding(20), ColumnCount = 1, RowCount = 3,
                BackColor = TampilanModern.Latar
            };
            tataLetakInput.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            tataLetakInput.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            tataLetakInput.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
            tataLetakInput.Controls.Add(labelPertanyaan, 0, 0);
            tataLetakInput.Controls.Add(inputJawaban, 0, 1);
            tataLetakInput.Controls.Add(panelTombol, 0, 2);
            formInput.Controls.Add(tataLetakInput);
            TampilanModern.aturUkuranDialog(formInput, new Size(520, 255), new Size(450, 220));
            formInput.AcceptButton = tombolSetuju;
            formInput.CancelButton = tombolBatal;
            TampilanModern.aturPaletForm(formInput);
            Form? pemilikForm = Form.ActiveForm;
            DialogResult hasil = pemilikForm is null ? formInput.ShowDialog() : formInput.ShowDialog(pemilikForm);
            return hasil == DialogResult.OK ? inputJawaban.Text : null;
        }

        internal static void tampilkanKesalahanData(IWin32Window pemilikForm, Exception kesalahan)
        {
            MessageBox.Show(pemilikForm,
                $"Data gudang tidak dapat diproses. Pastikan folder berikut tersedia dan dapat ditulis:\r\n{PenyimpananGudang.folderData}\r\n\r\n{kesalahan.Message}",
                "Kesalahan data", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
