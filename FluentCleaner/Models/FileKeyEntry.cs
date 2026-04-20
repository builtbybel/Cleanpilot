namespace FluentCleaner.Models;

// Controls how a FileKey directory is scanned.
public enum FileKeyFlag
{
    None,       // Top-level files only.
    Recurse,    // Scan all subdirectories recursively.
    RemoveSelf  // Like Recurse, but also prune empty directories afterwards.
}

/* Parsed representation of one FileKeyN= line from Winapp2.ini.
   Format:  FileKey1=<path>|<pattern(s)>[|RECURSE|REMOVESELF]
   Example: FileKey1=%LocalAppData%\Temp|*.tmp;*.log|RECURSE */
public class FileKeyEntry
{
    // Directory path to scan. May contain %EnvVar% tokens and * wildcards in path segments.
    public string Path { get; set; } = "";

    /* Semicolon-separated file filter(s), e.g. "*.tmp" or "*.log;*.bak".
       Defaults to "*.*" when no pattern is specified in the ini. */
    public string Pattern { get; set; } = "*.*";

    // Whether to recurse into subdirectories and whether to remove empty dirs afterwards.
    public FileKeyFlag Flag { get; set; } = FileKeyFlag.None;

    public static FileKeyEntry Parse(string value)
    {
        var parts = value.Split('|');
        var entry = new FileKeyEntry { Path = parts[0].Trim() };

        // parts[1] can be either a file pattern OR a flag (when no pattern is given)
        if (parts.Length == 2)
        {
            var p = parts[1].Trim().ToUpperInvariant();
            if (p is "RECURSE" or "REMOVESELF")
                entry.Flag = p == "RECURSE" ? FileKeyFlag.Recurse : FileKeyFlag.RemoveSelf;
            else
                entry.Pattern = parts[1].Trim();
        }
        else if (parts.Length > 2)
        {
            entry.Pattern = parts[1].Trim();
            entry.Flag = parts[2].Trim().ToUpperInvariant() switch
            {
                "RECURSE"    => FileKeyFlag.Recurse,
                "REMOVESELF" => FileKeyFlag.RemoveSelf,
                _            => FileKeyFlag.None
            };
        }

        return entry;
    }
}
