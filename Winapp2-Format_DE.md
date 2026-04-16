# Winapp2.ini – Unterstützte Felder

Referenz für alle Felder die FluentCleaner beim Parsen und Ausführen versteht.

---

## Eintrag-Struktur

```ini
[App Name *]
LangSecRef=3021
Detect=HKLM\Software\MyApp
DetectFile=%LocalAppData%\MyApp
SpecialDetect=DET_CHROME
Warning=Löscht gespeicherte Passwörter
Default=False
FileKey1=%AppData%\MyApp|*.log;*.tmp
FileKey2=%AppData%\MyApp\Cache|*|REMOVESELF
RegKey1=HKCU\Software\MyApp\MRU
ExcludeKey1=FILE|%AppData%\MyApp\|important.db
```

---

## Erkennung (mindestens eines erforderlich)

| Feld | Format | Beschreibung |
|---|---|---|
| `Detect` | `HKLM\Software\Foo` | Registry-Schlüssel muss existieren |
| `Detect` | `HKLM\Software\Foo\|Value` | Registry-Wert muss existieren (`\|` trennt Key und Value) |
| `DetectFile` | `%LocalAppData%\MyApp` | Datei oder Ordner muss existieren |
| `DetectFile` | `%LocalAppData%\Chrome*` | Wildcards (`*`, `?`) im Pfad erlaubt |
| `SpecialDetect` | `DET_CHROME` | Kurzname für bekannte Apps (siehe unten) |

Mehrere `Detect`/`DetectFile`-Zeilen werden mit **OR** verknüpft – einer reicht.

### Bekannte SpecialDetect-Codes

| Code | Prüft |
|---|---|
| `DET_CHROME` | `%LocalAppData%\Google\Chrome\User Data` |
| `DET_FIREFOX` | `%AppData%\Mozilla\Firefox` |
| `DET_EDGE` | `%LocalAppData%\Microsoft\Edge\User Data` |
| `DET_OPERA` | `%AppData%\Opera Software\Opera Stable` |
| `DET_OPERA_GX` | — |
| `DET_THUNDERBIRD` | `%AppData%\Thunderbird` |
| `DET_IE` | Registry-Pfad für Internet Explorer |
| `DET_WINSTORE` | `%LocalAppData%\Packages` |

---

## FileKey

```
FileKeyN=<Pfad>|<Muster>[|RECURSE|REMOVESELF]
```

| Variante | Beispiel | Verhalten |
|---|---|---|
| Nur Pfad + Muster | `%Temp%\MyApp\|*.tmp` | Nur direkt im Ordner |
| Multi-Muster | `%Temp%\|*.log;*.tmp;*.bak` | Mehrere Muster mit `;` getrennt |
| RECURSE | `%AppData%\MyApp\|*.log\|RECURSE` | Alle Unterordner durchsuchen |
| REMOVESELF | `%AppData%\MyApp\Cache\|*\|REMOVESELF` | Dateien löschen + leere Ordner danach entfernen |
| Flag ohne Muster | `%AppData%\MyApp\Cache\|REMOVESELF` | `*.*` als Muster, Flag gilt trotzdem |

### Pfad-Variablen

| Variable | Pfad |
|---|---|
| `%AppData%` | `C:\Users\Name\AppData\Roaming` |
| `%LocalAppData%` | `C:\Users\Name\AppData\Local` |
| `%LocalLowAppData%` | `C:\Users\Name\AppData\LocalLow` |
| `%ProgramData%` / `%CommonAppData%` | `C:\ProgramData` |
| `%ProgramFiles%` | `C:\Program Files` *(+ automatisch x86-Variante)* |
| `%ProgramFiles(x86)%` / `%ProgramFilesX86%` | `C:\Program Files (x86)` |
| `%UserProfile%` | `C:\Users\Name` |
| `%SystemRoot%` / `%WinDir%` | `C:\Windows` |
| `%System%` | `C:\Windows\System32` |
| `%Temp%` / `%Tmp%` | Temp-Ordner des Benutzers |
| `%SystemDrive%` | `C:` |
| `%Documents%`, `%Desktop%`, `%Music%`, `%Pictures%`, `%Videos%` | Bekannte Benutzerordner |

Wildcards (`*`, `?`) sind in **Pfad-Segmenten** erlaubt:
```
%LocalAppData%\Google\Chrome*\User Data\*\Cache
```
→ wird zur Laufzeit zu allen passenden konkreten Pfaden aufgelöst.

---

## RegKey

```
RegKeyN=<HIVE>\<Pfad>[\|<Wertname>]
```

| Variante | Beispiel | Verhalten |
|---|---|---|
| Ganzen Schlüssel löschen | `HKCU\Software\MyApp\MRU` | Schlüssel + alle Unterschlüssel |
| Einzelnen Wert löschen | `HKCU\Software\MyApp\|LastRun` | Nur dieser Wert |

### Unterstützte Hives

`HKCU`, `HKLM`, `HKU`, `HKCC`, `HKCR`  
(auch als Langform: `HKEY_CURRENT_USER` usw.)

---

## ExcludeKey

```
ExcludeKeyN=<TYP>|<Pfad>\|[<Muster>]
```

Schützt Dateien/Ordner vor dem Löschen, auch wenn ein FileKey sie treffen würde.

| Typ | Beispiel | Verhalten |
|---|---|---|
| `FILE` + exakter Name | `FILE\|%AppData%\MyApp\|config.db` | Nur genau diese Datei im Ordner |
| `FILE` + Wildcard | `FILE\|%AppData%\MyApp\|*.db` | Alle `.db`-Dateien direkt im Ordner |
| `PATH` ohne Muster | `PATH\|%AppData%\MyApp\Profiles\` | Ganzen Ordner + Unterordner schützen |
| `PATH` + Wildcard | `PATH\|%AppData%\MyApp\_Data\|*` | Alle Dateien rekursiv schützen |
| `PATH` + Wildcard | `PATH\|%AppData%\MyApp\Cache\|*.db` | Nur `.db`-Dateien rekursiv schützen |
| `REG` | `REG\|HKCU\Software\MyApp\` | Registry-Ausschluss (wird ignoriert beim Scan) |

> **Hinweis:** `FILE` mit literalem Muster schützt nur direkte Kinder des Ordners.  
> `PATH` mit Wildcard-Muster schützt alle Dateien im gesamten Teilbaum.

---

## Weitere Felder

| Feld | Beschreibung |
|---|---|
| `LangSecRef` | Kategorienummer für die UI-Gruppierung (z. B. `3029` = Google Chrome) |
| `Section` | Freitext-Kategorie als Fallback wenn kein `LangSecRef` bekannt |
| `Warning` | Warntext der vor dem Löschen angezeigt wird |
| `Default` | `True` / `False` – ob der Eintrag standardmäßig aktiviert ist |
