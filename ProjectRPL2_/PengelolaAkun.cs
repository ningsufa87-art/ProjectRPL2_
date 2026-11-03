using System.Text;

namespace ProjectRPL2_
{
    internal static class PengelolaAkun
    {
        internal sealed record Pengguna(string NamaPengguna, string Peran);

        internal static Pengguna? validasiLogin(string namaPengguna, string kataSandi)
        {
            if (string.IsNullOrWhiteSpace(namaPengguna) || kataSandi is null)
                return null;

            var akunCocok = new List<(string Nama, string Sandi, string Peran)>();
            foreach (string baris in File.ReadLines(PenyimpananGudang.filePengguna, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(baris) || baris.TrimStart().StartsWith('#'))
                    continue;
                if (!cobaBacaAkun(baris, out string namaFile, out string sandiFile, out string peran))
                    continue;
                if (namaFile.Equals(namaPengguna.Trim(), StringComparison.OrdinalIgnoreCase))
                    akunCocok.Add((namaFile, sandiFile, peran));
            }

            // Nama pengguna ganda ditolak agar akun yang dipakai tidak ambigu.
            if (akunCocok.Count != 1)
                return null;

            var akun = akunCocok[0];
            if (!akun.Sandi.Equals(kataSandi, StringComparison.Ordinal) ||
                !(akun.Peran.Equals("admin", StringComparison.OrdinalIgnoreCase) ||
                  akun.Peran.Equals("kasir", StringComparison.OrdinalIgnoreCase)))
                return null;

            return new Pengguna(akun.Nama, akun.Peran.ToLowerInvariant());
        }

        private static bool cobaBacaAkun(string baris, out string namaPengguna, out string kataSandi, out string peran)
        {
            namaPengguna = kataSandi = peran = string.Empty;
            char pemisah = baris.Contains('|') ? '|' : ',';
            var kolom = new List<string>(3);
            var kolomSaatIni = new StringBuilder();
            bool sedangDikutip = false;

            for (int indeks = 0; indeks < baris.Length; indeks++)
            {
                char karakter = baris[indeks];
                if (karakter == '"')
                {
                    if (sedangDikutip && indeks + 1 < baris.Length && baris[indeks + 1] == '"')
                    {
                        kolomSaatIni.Append('"');
                        indeks++;
                    }
                    else
                    {
                        sedangDikutip = !sedangDikutip;
                    }
                }
                else if (karakter == pemisah && !sedangDikutip)
                {
                    kolom.Add(kolomSaatIni.ToString().Trim());
                    kolomSaatIni.Clear();
                }
                else
                {
                    kolomSaatIni.Append(karakter);
                }
            }

            if (sedangDikutip)
                return false;
            kolom.Add(kolomSaatIni.ToString().Trim());
            if (kolom.Count != 3 || kolom.Any(string.IsNullOrWhiteSpace))
                return false;

            (namaPengguna, kataSandi, peran) = (kolom[0], kolom[1], kolom[2]);
            return true;
        }
    }
}

