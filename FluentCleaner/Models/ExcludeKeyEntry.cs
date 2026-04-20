namespace FluentCleaner.Models;

//What kind of resource the ExcludeKey protects.
public enum ExcludeType
{
    File,  // Specific file or filename pattern inside a directory.
    Path,  // Entire directory subtree.
    Reg    // Registry key or value (not processed during file scanning).
}

/* Parsed representation of one ExcludeKeyN= line from Winapp2.ini.
   Format:  ExcludeKey1=<TYPE>|<path>[|<filename pattern>]
   Example: ExcludeKey1=FILE|%AppData%\Mozilla\Firefox\Profiles\|places.sqlite */
public class ExcludeKeyEntry
{
    // Whether this exclusion covers a file, a directory tree, or a registry entry.
    public ExcludeType Type { get; set; }

    // Base path of the exclusion. May contain %EnvVar% tokens.
    public string Path { get; set; } = "";

    /* Optional filename or pattern within Path (e.g. "places.sqlite" or "*.db").
       When set, only that specific file is excluded and not the whole directory.
       When null, the entire directory is excluded (PATH-style behaviour). */
    public string? Pattern { get; set; }

    public static ExcludeKeyEntry Parse(string value)
    {
        var parts = value.Split('|');
        var entry = new ExcludeKeyEntry();

        if (parts.Length > 0)
        {
            entry.Type = parts[0].Trim().ToUpperInvariant() switch
            {
                "FILE" => ExcludeType.File,
                "PATH" => ExcludeType.Path,
                "REG"  => ExcludeType.Reg,
                _      => ExcludeType.File
            };
        }
        if (parts.Length > 1) entry.Path    = parts[1].Trim();
        if (parts.Length > 2) entry.Pattern = parts[2].Trim();

        return entry;
    }
}
