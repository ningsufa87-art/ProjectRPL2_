# SMKMART — Aplikasi Kasir dan Gudang

Aplikasi desktop Windows Forms untuk melihat stok gudang, melakukan restok, dan mencatat penjualan SMKMART. Aplikasi dibuat dengan C# dan .NET 8.

## Persyaratan

- Windows
- .NET 8 SDK
- Visual Studio dengan dukungan pengembangan desktop .NET, atau terminal dengan `dotnet`

## Menjalankan aplikasi

1. Buka `ProjectRPL2_.slnx` di Visual Studio.
2. Pastikan berkas data berada di folder `ProjectRPL2_` seperti susunan di bawah.
3. Jalankan proyek `ProjectRPL2_`.

Alternatif melalui terminal dari folder utama repositori:

```powershell
dotnet build .\ProjectRPL2_\ProjectRPL2_.csproj
dotnet run --project .\ProjectRPL2_\ProjectRPL2_.csproj
```

Aplikasi membaca akun dari `ProjectRPL2_\User.txt`. Setiap baris berformat `username,password,role`. Peran yang dikenali adalah `Admin` dan `Kasir`. Pembaca juga menerima pemisah `|`.

## Fungsi aplikasi

### Admin

- Melihat daftar barang, kategori, stok, harga, dan status.
- Menambah stok barang yang aktif.
- Membuka riwayat gudang dan riwayat transaksi.

### Kasir

- Melihat barang yang berstatus aktif.
- Memilih barang dan jumlah yang dijual.
- Mencatat pembayaran dengan metode Cash, QRIS, atau Debit.
- Untuk pembayaran Cash, aplikasi menghitung dan mencatat kembalian.
- Setelah transaksi berhasil, stok berkurang dan riwayat transaksi diperbarui.

Kasir memproses satu jenis barang per transaksi. `keranjang.txt` disertakan sebagai berkas data proyek, tetapi belum digunakan oleh alur aplikasi C#.

## Berkas data

Simpan berkas berikut di folder proyek `ProjectRPL2_`:

```text
ProjectRPL2_/
├── User.txt
├── data.txt
├── kategori.txt
├── keranjang.txt
├── riwayat_gudang.txt
└── riwayat_transaksi.txt
```

Format berkas:

| Berkas | Isi setiap baris |
| --- | --- |
| `User.txt` | `username,password,role` |
| `data.txt` | `id,nama,stok,harga,status` (format empat kolom juga dibaca; status dianggap `aktif`) |
| `kategori.txt` | `id_kategori,nama_kategori` |
| `keranjang.txt` | Disediakan sebagai berkas proyek; belum dipakai aplikasi C# saat ini |
| `riwayat_gudang.txt` | Catatan perubahan stok dan aktivitas gudang |
| `riwayat_transaksi.txt` | Catatan transaksi penjualan dan pembayaran |

Baris barang berstatus `terhapus` tetap ditampilkan di halaman admin, tetapi tidak tersedia untuk penjualan. Kategori barang ditentukan dari kelompok angka ID barang, misalnya ID 101 masuk kategori 1 dan ID 201 masuk kategori 2.

Berkas TXT disalin ke folder output saat build. Saat dijalankan dari proyek, aplikasi mencari folder proyek yang memuat `ProjectRPL2_.csproj`, `User.txt`, dan `data.txt`; jika folder tersebut tidak ditemukan, aplikasi memakai folder tempat berkas `.exe` berada. Perubahan stok dan riwayat ditulis ke folder data yang sedang dipakai aplikasi.

## Menjaga data saat menggunakan Git

- Pertahankan enam berkas TXT di folder `ProjectRPL2_` agar akun, katalog, dan riwayat tersedia setelah proyek di-clone.
- Perubahan hasil restok dan transaksi mengubah `data.txt` atau berkas riwayat. Tinjau perubahan tersebut sebelum membuat commit.
- Jangan menghapus atau mengganti nama berkas data tanpa menyesuaikan kode aplikasi.
- `User.txt` berisi kata sandi dalam teks biasa. Gunakan akun latihan dan jangan unggah kata sandi yang dipakai pada layanan lain.

## Susunan kode utama

- `Form1.cs` — halaman login dan pengalihan berdasarkan peran.
- `UserStore.cs` — membaca serta memvalidasi akun pada `User.txt`.
- `WarehouseStore.cs` — membaca katalog, mengubah stok, dan mencatat riwayat.
- `admin.cs` — halaman admin dan fungsi restok.
- `kasir.cs` — halaman kasir dan proses penjualan.
- `RoleDashboard.cs` — tampilan daftar data dan pembuka riwayat.
