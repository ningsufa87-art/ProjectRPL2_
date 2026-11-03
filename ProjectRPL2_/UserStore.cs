using System.Text;

namespace ProjectRPL2_
{
    internal static class UserStore
    {
        internal sealed record User(string Username, string Role);

        internal static User? Authenticate(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || password is null)
                return null;

            var matches = new List<(string Username, string Password, string Role)>();
            foreach (string line in File.ReadLines(WarehouseStore.UserFile, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith('#'))
                    continue;
                if (!TryParseUser(line, out string fileUsername, out string filePassword, out string role))
                    continue;
                if (fileUsername.Equals(username.Trim(), StringComparison.OrdinalIgnoreCase))
                    matches.Add((fileUsername, filePassword, role));
            }

            if (matches.Count != 1)
                return null;

            var match = matches[0];
            if (!match.Password.Equals(password, StringComparison.Ordinal) ||
                !(match.Role.Equals("admin", StringComparison.OrdinalIgnoreCase) ||
                  match.Role.Equals("kasir", StringComparison.OrdinalIgnoreCase)))
                return null;

            return new User(match.Username, match.Role.ToLowerInvariant());
        }

        private static bool TryParseUser(string line, out string username, out string password, out string role)
        {
            username = password = role = string.Empty;
            char delimiter = line.Contains('|') ? '|' : ',';
            var fields = new List<string>(3);
            var field = new StringBuilder();
            bool quoted = false;

            for (int i = 0; i < line.Length; i++)
            {
                char current = line[i];
                if (current == '"')
                {
                    if (quoted && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        field.Append('"');
                        i++;
                    }
                    else
                    {
                        quoted = !quoted;
                    }
                }
                else if (current == delimiter && !quoted)
                {
                    fields.Add(field.ToString().Trim());
                    field.Clear();
                }
                else
                {
                    field.Append(current);
                }
            }

            if (quoted)
                return false;
            fields.Add(field.ToString().Trim());
            if (fields.Count != 3 || fields.Any(string.IsNullOrWhiteSpace))
                return false;

            (username, password, role) = (fields[0], fields[1], fields[2]);
            return true;
        }
    }
}
