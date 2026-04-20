namespace FluentCleaner.Models;

/* Parsed representation of one RegKeyN= line from Winapp2.ini.
   Format:  RegKey1=<HIVE\SubKey>[|ValueName]
   Without a ValueName the entire key tree is deleted; with one only that value is removed. */
public class RegKeyEntry
{
    // Full registry path including hive, e.g. "HKCU\Software\Microsoft\Foo".
    public string KeyPath { get; set; } = "";

    // Optional. If set, only this named value is deleted rather than the whole key.
    public string? ValueName { get; set; }

    public static RegKeyEntry Parse(string value)
    {
        var idx = value.IndexOf('|');
        if (idx >= 0)
        {
            return new RegKeyEntry
            {
                KeyPath   = value[..idx].Trim(),
                ValueName = value[(idx + 1)..].Trim()
            };
        }
        return new RegKeyEntry { KeyPath = value.Trim() };
    }
}
