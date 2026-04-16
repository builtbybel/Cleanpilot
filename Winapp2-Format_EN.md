# Winapp2.ini Format

Quick reference for everything FluentCleaner reads from a winapp2.ini file.

---

## What an entry looks like

```ini
[App Name *]
LangSecRef=3021
Detect=HKLM\Software\MyApp
DetectFile=%LocalAppData%\MyApp
SpecialDetect=DET_CHROME
Warning=This removes saved passwords
Default=False
FileKey1=%AppData%\MyApp|*.log;*.tmp
FileKey2=%AppData%\MyApp\Cache|*|REMOVESELF
RegKey1=HKCU\Software\MyApp\MRU
ExcludeKey1=FILE|%AppData%\MyApp\|important.db
```

---

## Detection

At least one detection field is required — otherwise the entry is hidden entirely.
Multiple `Detect` / `DetectFile` lines use **OR** logic, so one hit is enough.

| Field | Format | What it checks |
|---|---|---|
| `Detect` | `HKLM\Software\Foo` | registry key exists |
| `Detect` | `HKLM\Software\Foo\|Value` | specific registry value exists |
| `DetectFile` | `%LocalAppData%\MyApp` | file or folder exists |
| `DetectFile` | `%LocalAppData%\Chrome*` | wildcards in the path work fine |
| `SpecialDetect` | `DET_CHROME` | shorthand for well-known apps (see below) |

### SpecialDetect codes

| Code | Checks |
|---|---|
| `DET_CHROME` | `%LocalAppData%\Google\Chrome\User Data` |
| `DET_FIREFOX` | `%AppData%\Mozilla\Firefox` |
| `DET_EDGE` | `%LocalAppData%\Microsoft\Edge\User Data` |
| `DET_OPERA` | `%AppData%\Opera Software\Opera Stable` |
| `DET_THUNDERBIRD` | `%AppData%\Thunderbird` |
| `DET_IE` | registry path for Internet Explorer |
| `DET_WINSTORE` | `%LocalAppData%\Packages` |

---

## FileKey

```
FileKeyN=<path>|<pattern>[|RECURSE|REMOVESELF]
```

| Variant | Example | What happens |
|---|---|---|
| path + pattern | `%Temp%\MyApp\|*.tmp` | top-level files only |
| multiple patterns | `%Temp%\|*.log;*.tmp;*.bak` | semicolon-separated, all matched |
| RECURSE | `%AppData%\MyApp\|*.log|RECURSE` | walks all subdirectories |
| REMOVESELF | `%AppData%\MyApp\Cache\|*|REMOVESELF` | deletes files, then prunes empty folders |
| flag without pattern | `%AppData%\MyApp\Cache\|REMOVESELF` | defaults to `*.*`, flag still applies |

### Path variables

| Variable | Resolves to |
|---|---|
| `%AppData%` | `C:\Users\Name\AppData\Roaming` |
| `%LocalAppData%` | `C:\Users\Name\AppData\Local` |
| `%LocalLowAppData%` | `C:\Users\Name\AppData\LocalLow` |
| `%ProgramData%` / `%CommonAppData%` | `C:\ProgramData` |
| `%ProgramFiles%` | `C:\Program Files` — *x86 variant is tried automatically too* |
| `%ProgramFiles(x86)%` / `%ProgramFilesX86%` | `C:\Program Files (x86)` |
| `%UserProfile%` | `C:\Users\Name` |
| `%SystemRoot%` / `%WinDir%` | `C:\Windows` |
| `%System%` | `C:\Windows\System32` |
| `%Temp%` / `%Tmp%` | user temp folder |
| `%SystemDrive%` | `C:` |
| `%Documents%`, `%Desktop%`, `%Music%`, `%Pictures%`, `%Videos%` | the usual shell folders |

Wildcards work inside path segments too:
```
%LocalAppData%\Google\Chrome*\User Data\*\Cache
```
At scan time this gets expanded to every real path that matches.

---

## RegKey

```
RegKeyN=<HIVE>\<path>[\|<value name>]
```

| Variant | Example | What happens |
|---|---|---|
| whole key | `HKCU\Software\MyApp\MRU` | deletes the key and everything under it |
| single value | `HKCU\Software\MyApp\|LastRun` | deletes just that value |

Supported hives: `HKCU`, `HKLM`, `HKU`, `HKCC`, `HKCR` — long forms like `HKEY_CURRENT_USER` work too.

---

## ExcludeKey

```
ExcludeKeyN=<TYPE>|<path>\|[<pattern>]
```

Anything matched here gets skipped during scanning, even if a FileKey would have caught it.

| Type | Example | What it protects |
|---|---|---|
| `FILE` + exact name | `FILE\|%AppData%\MyApp\|config.db` | that one file, direct child of the folder only |
| `FILE` + wildcard | `FILE\|%AppData%\MyApp\|*.db` | all `.db` files directly in that folder |
| `PATH` no pattern | `PATH\|%AppData%\MyApp\Profiles\` | the whole folder tree |
| `PATH` + `*` | `PATH\|%AppData%\MyApp\_Data\|*` | every file in the tree |
| `PATH` + wildcard | `PATH\|%AppData%\MyApp\Cache\|*.db` | only `.db` files, but recursively |
| `REG` | `REG\|HKCU\Software\MyApp\` | registry exclusion — ignored during file scan |

> `FILE` with a literal name only covers direct children of the folder.  
> `PATH` with a wildcard pattern covers the whole subtree.

---

## Other fields

| Field | What it does |
|---|---|
| `LangSecRef` | category number for UI grouping (`3029` = Google Chrome, etc.) |
| `Section` | free-text category, used as fallback if `LangSecRef` isn't a known code |
| `Warning` | shown to the user before cleaning starts |
| `Default` | `True` / `False` — whether the entry is checked by default |
