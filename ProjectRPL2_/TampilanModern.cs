using System.Drawing;
using Material3.WinForms.Controls;
using Material3.WinForms.Theming;

namespace ProjectRPL2_
{
    internal static class TampilanModern
    {
        private static bool modeGelap;

        internal static bool ModeGelap => modeGelap;
        internal static Color Latar => modeGelap ? Color.FromArgb(18, 19, 24) : Color.FromArgb(246, 247, 251);
        internal static Color Permukaan => modeGelap ? Color.FromArgb(30, 32, 39) : Color.White;
        internal static Color PermukaanLembut => modeGelap ? Color.FromArgb(43, 46, 56) : Color.FromArgb(239, 240, 248);
        internal static Color Teks => modeGelap ? Color.FromArgb(232, 230, 239) : Color.FromArgb(30, 34, 46);
        internal static Color TeksSekunder => modeGelap ? Color.FromArgb(190, 191, 204) : Color.FromArgb(103, 111, 128);
        internal static Color Aksen => modeGelap ? Color.FromArgb(190, 194, 255) : Color.FromArgb(76, 86, 220);
        private static Color TeksAksen => modeGelap ? Color.FromArgb(42, 45, 75) : Color.White;

        internal static void aturForm(Form form)
        {
            Rectangle areaKerja = Screen.FromPoint(Cursor.Position).WorkingArea;
            int lebarMaks = Math.Max(320, areaKerja.Width - 32);
            int tinggiMaks = Math.Max(320, areaKerja.Height - 48);
            form.BackColor = Latar;
            form.ForeColor = Teks;
            form.Font = new Font("Segoe UI", 9.5F);
            form.AutoScaleMode = AutoScaleMode.Dpi;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimumSize = new Size(Math.Min(760, lebarMaks), Math.Min(500, tinggiMaks));
            form.ClientSize = new Size(Math.Min(1220, lebarMaks), Math.Min(760, tinggiMaks));
            form.MaximizeBox = true;
            form.MinimizeBox = true;
            form.TopMost = false;
        }

        internal static void aturUkuranDialog(Form form, Size ukuranDisarankan, Size ukuranMinimum)
        {
            Rectangle areaKerja = Screen.FromPoint(Cursor.Position).WorkingArea;
            int lebarMaks = Math.Max(420, areaKerja.Width - 64);
            int tinggiMaks = Math.Max(320, areaKerja.Height - 80);
            int lebarMin = Math.Min(ukuranMinimum.Width, lebarMaks);
            int tinggiMin = Math.Min(ukuranMinimum.Height, tinggiMaks);
            form.MinimumSize = new Size(lebarMin, tinggiMin);
            form.ClientSize = new Size(Math.Max(lebarMin, Math.Min(ukuranDisarankan.Width, lebarMaks)),
                Math.Max(tinggiMin, Math.Min(ukuranDisarankan.Height, tinggiMaks)));
        }

        internal static void aturTema(bool gelap)
        {
            modeGelap = gelap;
            ThemeManager.IsDark = gelap;
            foreach (Form form in Application.OpenForms.Cast<Form>().ToList())
                aturPaletForm(form);
        }

        internal static void gantiTema() => aturTema(!modeGelap);

        internal static void aturPaletForm(Form form)
        {
            form.BackColor = Latar;
            form.ForeColor = Teks;
            terapkanWarna(form);
        }

        private static void terapkanWarna(Control kontrol)
        {
            string peranWarna = kontrol.Tag as string ?? string.Empty;
            if (peranWarna == "panelAksen")
            {
                kontrol.BackColor = modeGelap ? Color.FromArgb(43, 47, 83) : Color.FromArgb(67, 77, 194);
                kontrol.ForeColor = Color.White;
            }
            else if (peranWarna == "teksPutih")
            {
                kontrol.ForeColor = Color.White;
            }
            else if (peranWarna == "teksAksen")
            {
                kontrol.ForeColor = Aksen;
            }
            else if (kontrol is MaterialButton)
            {
                if (kontrol.Name.StartsWith("tombolTema", StringComparison.Ordinal))
                    kontrol.Text = modeGelap ? "Mode terang" : "Mode gelap";
            }
            else if (kontrol is Button tombol)
            {
                if (peranWarna == "aksen")
                {
                    tombol.BackColor = Aksen;
                    tombol.ForeColor = TeksAksen;
                }
                else if (peranWarna == "tautan")
                {
                    tombol.BackColor = Permukaan;
                    tombol.ForeColor = Aksen;
                }
                else
                {
                    tombol.BackColor = PermukaanLembut;
                    tombol.ForeColor = Teks;
                }
            }
            else if (kontrol is ListView daftar)
            {
                daftar.BackColor = Permukaan;
                daftar.ForeColor = Teks;
                if (!daftar.OwnerDraw) aturGambarDaftar(daftar);
                daftar.Invalidate();
            }
            else if (kontrol is RichTextBox || kontrol is TextBox || kontrol is ComboBox)
            {
                kontrol.BackColor = Permukaan;
                kontrol.ForeColor = Teks;
            }
            else if (kontrol is Label label)
            {
                label.ForeColor = peranWarna == "sekunder" ? TeksSekunder : Teks;
            }
            else if (kontrol is Panel || kontrol is FlowLayoutPanel || kontrol is TableLayoutPanel)
            {
                kontrol.BackColor = peranWarna == "permukaan" ? Permukaan : Latar;
                kontrol.ForeColor = Teks;
            }
            foreach (Control anak in kontrol.Controls)
                terapkanWarna(anak);
        }

        private static void aturGambarDaftar(ListView daftar)
        {
            daftar.OwnerDraw = true;
            daftar.DrawColumnHeader += (_, e) =>
            {
                if (e.Header is null) return;
                using var latar = new SolidBrush(PermukaanLembut);
                e.Graphics.FillRectangle(latar, e.Bounds);
                Rectangle areaTeks = Rectangle.Inflate(e.Bounds, -6, 0);
                TextRenderer.DrawText(e.Graphics, e.Header.Text, daftar.Font, areaTeks, Teks,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | flagsPerataan(e.Header.TextAlign));
            };
            daftar.DrawItem += (_, e) =>
            {
                if (daftar.View == View.Details || e.Item is null) return;
                Color warnaLatar = e.Item.Selected ? Aksen : Permukaan;
                Color warnaTeks = e.Item.Selected ? TeksAksen : Teks;
                using var latar = new SolidBrush(warnaLatar);
                e.Graphics.FillRectangle(latar, e.Bounds);
                TextRenderer.DrawText(e.Graphics, e.Item.Text, daftar.Font, e.Bounds, warnaTeks,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
            };
            daftar.DrawSubItem += (_, e) =>
            {
                if (e.Item is null || e.SubItem is null) return;
                bool dipilih = e.Item.Selected;
                Color warnaLatar = dipilih ? Aksen : Permukaan;
                Color warnaTeks = dipilih ? TeksAksen : Teks;
                using var latar = new SolidBrush(warnaLatar);
                e.Graphics.FillRectangle(latar, e.Bounds);
                Rectangle areaTeks = Rectangle.Inflate(e.Bounds, -6, 0);
                HorizontalAlignment perataan = e.ColumnIndex >= 0 && e.ColumnIndex < daftar.Columns.Count
                    ? daftar.Columns[e.ColumnIndex].TextAlign : HorizontalAlignment.Left;
                TextRenderer.DrawText(e.Graphics, e.SubItem.Text, daftar.Font, areaTeks, warnaTeks,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | flagsPerataan(perataan));
            };
        }

        private static TextFormatFlags flagsPerataan(HorizontalAlignment perataan) => perataan switch
        {
            HorizontalAlignment.Center => TextFormatFlags.HorizontalCenter,
            HorizontalAlignment.Right => TextFormatFlags.Right,
            _ => TextFormatFlags.Left
        };

        internal static Panel buatHeader(string judul, string keterangan, string namaPengguna, Action aksiKeluar)
        {
            var header = new Panel
            {
                Name = "panelHeader", Dock = DockStyle.Fill, Height = 88, Padding = new Padding(22, 8, 18, 8),
                BackColor = Permukaan, Tag = "permukaan"
            };
            var layoutHeader = new TableLayoutPanel
            {
                Name = "tataLetakHeader", Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1,
                BackColor = Permukaan, Tag = "permukaan", Margin = Padding.Empty
            };
            layoutHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layoutHeader.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            var panelJudul = new Panel { Dock = DockStyle.Fill, BackColor = Permukaan, Tag = "permukaan", MinimumSize = new Size(180, 0) };
            var labelJudul = new Label
            {
                Name = "labelJudulHalaman", Text = judul, Dock = DockStyle.Top, Height = 36,
                AutoEllipsis = true, Font = new Font("Segoe UI", 17, FontStyle.Bold), ForeColor = Teks
            };
            var labelKeterangan = new Label
            {
                Name = "labelKeteranganHalaman", Text = keterangan, Dock = DockStyle.Top, Height = 28,
                AutoEllipsis = true, Font = new Font("Segoe UI", 9.5F), ForeColor = TeksSekunder
            };
            panelJudul.Controls.Add(labelKeterangan);
            panelJudul.Controls.Add(labelJudul);

            var panelAksi = new FlowLayoutPanel
            {
                Name = "panelAksiHeader", Dock = DockStyle.Fill, AutoSize = true, WrapContents = false,
                FlowDirection = FlowDirection.RightToLeft, BackColor = Permukaan, Tag = "permukaan",
                Padding = new Padding(0, 12, 0, 0), Margin = Padding.Empty
            };
            var tombolKeluar = buatTombol("tombolGantiAkun", "Keluar / Ganti akun", sekunder: true);
            tombolKeluar.MinimumSize = new Size(154, 40);
            tombolKeluar.Margin = new Padding(7, 5, 7, 0);
            tombolKeluar.Click += (_, _) =>
            {
                if (MessageBox.Show(formPemilik(header), "Kembali ke halaman login?", "Keluar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    aksiKeluar();
            };
            var tombolTema = buatTombolTema("tombolTemaHeader");
            tombolTema.Margin = new Padding(7, 5, 7, 0);
            var labelPengguna = new Label
            {
                Name = "labelPenggunaAktif", Text = $"{namaPengguna}  ·  Aktif", Width = 118, Height = 40,
                AutoEllipsis = true, TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Teks, Margin = new Padding(7, 5, 7, 0)
            };
            panelAksi.Controls.Add(tombolKeluar);
            panelAksi.Controls.Add(new Panel
            {
                Name = "jarakTombolHeader", Width = 10, Height = 1, Margin = Padding.Empty,
                BackColor = Permukaan, Tag = "permukaan"
            });
            panelAksi.Controls.Add(tombolTema);
            panelAksi.Controls.Add(labelPengguna);
            void sesuaikanAksiHeader()
            {
                bool ringkas = header.FindForm()?.ClientSize.Width < 1000;
                labelPengguna.Visible = !ringkas;
                tombolKeluar.Width = ringkas ? 138 : 154;
                tombolTema.Width = ringkas ? 112 : 124;
            }
            header.Resize += (_, _) => sesuaikanAksiHeader();
            header.HandleCreated += (_, _) => sesuaikanAksiHeader();
            layoutHeader.Controls.Add(panelJudul, 0, 0);
            layoutHeader.Controls.Add(panelAksi, 1, 0);
            header.Controls.Add(layoutHeader);
            return header;
        }

        internal static Button buatTombolTema(string namaKontrol)
        {
            var tombolTema = buatTombol(namaKontrol, modeGelap ? "Mode terang" : "Mode gelap", sekunder: true);
            tombolTema.MinimumSize = new Size(124, 40);
            tombolTema.Click += (_, _) => gantiTema();
            return tombolTema;
        }

        internal static Button buatTombol(string nama, string teks, bool sekunder = false)
        {
            var tombol = new MaterialButton
            {
                Name = nama,
                Text = teks,
                AutoSize = true,
                MinimumSize = new Size(112, 40),
                Padding = new Padding(12, 3, 12, 3),
                Font = new Font("Segoe UI", 9.25F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Variant = sekunder ? MaterialButtonVariant.Tonal : MaterialButtonVariant.Filled,
                Cursor = Cursors.Hand,
                Margin = new Padding(5)
            };
            return tombol;
        }

        internal static void aturDaftar(ListBox daftar)
        {
            daftar.BackColor = Permukaan;
            daftar.ForeColor = Teks;
            daftar.BorderStyle = BorderStyle.None;
            daftar.Font = new Font("Segoe UI", 10.5F);
            daftar.IntegralHeight = false;
            daftar.ItemHeight = 32;
        }

        private static Form? formPemilik(Control kontrol) => kontrol.FindForm();
    }
}
