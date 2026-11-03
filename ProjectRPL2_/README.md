# SMKMART — Sistem Kasir dan Pengelolaan Gudang

Aplikasi desktop Windows Forms untuk mengelola katalog dan stok SMKMART, mencatat aktivitas gudang, serta memproses penjualan. Kode menggunakan C# dengan .NET 8 dan menyimpan data pada berkas TXT di folder proyek.

UI memakai kontrol WinForms dari Material 3, dengan halaman login, panel terpisah untuk Admin dan Kasir, serta tombol **Keluar / Ganti akun** untuk kembali ke login.

Tema terang dan gelap dapat diganti dari halaman login maupun header Admin/Kasir. Ukuran jendela dan dialog menyesuaikan area kerja layar; tabel daftar barang dan laporan menyesuaikan lebar jendelanya.

## Persyaratan

- Windows
- .NET 8 SDK
- Visual Studio dengan beban kerja pengembangan desktop .NET, atau terminal dengan `dotnet`

## Menjalankan aplikasi

Buka `ProjectRPL2_.slnx` di Visual Studio, lalu jalankan proyek `ProjectRPL2_`.

Atau jalankan dari PowerShell di folder utama repositori:

```powershell
dotnet build .\ProjectRPL2_\ProjectRPL2_.csproj
dotnet run --project .\ProjectRPL2_\ProjectRPL2_.csproj
```

Login memakai akun di `ProjectRPL2_\User.txt`. Format setiap baris adalah `username,password,role`. Peran yang didukung: `Admin` dan `Kasir`. Username dan password dicocokkan persis setelah spasi awal/akhir dibuang; peran tidak peka huruf besar/kecil. Baris dengan pemisah koma maupun `|` dapat dibaca. Masukkan `0` untuk username dan password untuk keluar.

## Fitur Admin

- Melihat daftar barang yang dikelompokkan per kategori, termasuk rentang ID, stok, harga, dan peringatan stok rendah di bawah 5 unit.
- Menambah kategori dan barang baru. ID barang dibuat dari ID kategori; satu kategori menyediakan rentang sampai 99 barang.
- Menambah stok barang aktif.
- Mengubah nama, kategori, stok, dan harga barang.
- Menghapus barang secara lunak ke Recycle Bin, memulihkan barang, atau menghapusnya permanen.
- Mencari barang secara linear berdasarkan ID, nama, atau ID kategori.
- Membaca riwayat gudang dan transaksi.
- Memilih bulan lalu melihat jumlah penjualan per hari dan total bulanan.

## Fitur Kasir

- Melihat barang aktif dan mencari berdasarkan ID, nama, atau kategori.
- Menambahkan beberapa barang ke keranjang, melihat keranjang, dan menghapus barang dari keranjang.
- Checkout seluruh keranjang dengan metode Cash, Debit, atau QRIS.
- Memvalidasi stok dan pembayaran. Pembayaran Cash menghitung kembalian; Debit dan QRIS dibayar sebesar total belanja.
- Menampilkan struk barang sebelum pembayaran dan struk transaksi lengkap sesudahnya. Setelah checkout berhasil, stok diperbarui, keranjang dikosongkan, dan transaksi dicatat.
- Membaca riwayat transaksi dan ringkasan penjualan harian/bulanan.

## Algoritma dan tampilan

- Algoritma TimSort dari kode Python dipindahkan dan digunakan untuk menyusun barang berdasarkan kategori serta ID.
- Pencarian linear mengikuti pilihan Python: ID, potongan nama, atau kategori.
- Aplikasi memeriksa ulang stok seluruh barang sebelum checkout supaya transaksi tidak membuat stok negatif.
- WinForms memakai kontrol Material 3 dengan navigasi panel samping. Tombol **Keluar / Ganti akun** kembali ke halaman login.

## Berkas data

Seluruh berkas berada di folder `ProjectRPL2_`:

```text
ProjectRPL2_/
├── User.txt
├── data.txt
├── kategori.txt
├── keranjang.txt
├── riwayat_gudang.txt
└── riwayat_transaksi.txt
```

| Berkas | Format |
| --- | --- |
| `User.txt` | `username,password,role` |
| `data.txt` | `id,nama,stok,harga,status`; baris empat kolom juga dibaca dan dianggap berstatus `aktif` |
| `kategori.txt` | `id_kategori,nama_kategori` |
| `keranjang.txt` | `id,nama,jumlah,harga_satuan`; diisi saat barang dimasukkan ke keranjang dan dikosongkan setelah checkout |
| `riwayat_gudang.txt` | Catatan tambah, restok, ubah, hapus, restore, dan penghapusan permanen |
| `riwayat_transaksi.txt` | Catatan item transaksi, waktu, kasir, metode pembayaran, total, pembayaran, dan kembalian |

Kategori barang ditentukan dari kelompok ID: ID 101–199 untuk kategori 1, ID 201–299 untuk kategori 2, dan seterusnya. Barang berstatus `terhapus` tersembunyi dari daftar kasir dan dapat dipulihkan melalui Recycle Bin Admin.

Aplikasi mencari folder data dengan menaiki folder dari lokasi `.exe` sampai menemukan `ProjectRPL2_.csproj`, `User.txt`, dan `data.txt`. Jika dijalankan dari hasil publish yang tidak memuat file proyek, aplikasi menggunakan folder `.exe`. Proyek mengatur agar enam berkas TXT disalin ke folder output saat build.

## Algoritma penting

- Pencarian berdasarkan ID, nama, dan kategori memeriksa katalog aktif.
- Daftar barang diurutkan berdasarkan ID.
- ID barang baru menggunakan nomor berikutnya yang tersedia pada rentang kategori dan tidak memakai ulang ID barang terhapus.
- Checkout menghitung jumlah per barang, memeriksa ulang seluruh stok sebelum mengurangi stok, lalu mencatat pembayaran dan kembalian.
- Ringkasan penjualan mengelompokkan transaksi yang memiliki format `=== TRANSAKSI ===` berdasarkan tanggal dan bulan.

## Menjaga data dan menggunakan Git

- Sertakan enam berkas TXT di folder `ProjectRPL2_` saat melakukan commit agar proyek dapat dijalankan setelah di-clone.
- Restok mengubah `data.txt`; penambahan barang dan kategori mengubah `data.txt` atau `kategori.txt`; keranjang dan aktivitas transaksi juga memperbarui berkas TXT terkait. Periksa perubahan data sebelum commit.
- `User.txt` menyimpan password dalam teks biasa. Gunakan akun latihan; jangan gunakan password yang dipakai pada layanan lain.

## Kode utama

- `FormLogin.cs` — login dan pengalihan berdasarkan peran; tombol memakai nama seperti `tombolLogin`.
- `PengelolaAkun.cs` — membaca dan memvalidasi akun.
- `PenyimpananGudang.cs` — operasi kategori, katalog, stok, keranjang, checkout, dan ringkasan penjualan.
- `FormAdmin.cs` — form Admin dan fitur pengelolaan gudang.
- `FormKasir.cs` — form Kasir, keranjang belanja, dan checkout.
- `TampilanPeran.cs` — dialog input, tampilan riwayat, pencarian, dan ringkasan.
- `TampilanModern.cs` — warna, tata letak, dan gaya kontrol aplikasi.
