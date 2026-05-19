# PSRegedit

A PowerShell binary module for reading and writing the Windows Registry.

- [Requirements](#requirements)
- [Installation](#installation)
- [Usage](#usage)
    - [Basic Examples](#basic-examples)
    - [All Cmdlets](#all-cmdlets)
        - [Get-RegSubKeyNames](#get-regsubkeynames)
        - [Get-RegValue](#get-regvalue)
        - [Get-RegEntry](#get-regentry)
        - [Get-RegEntries](#get-regentries)
        - [Set-RegValue](#set-regvalue)
        - [New-RegKey](#new-regkey)
        - [Remove-RegValue](#remove-regvalue)
        - [Remove-RegTree](#remove-regtree)
    - [Registry Hives](#registry-hives)
    - [Registry Value Types](#registry-value-types)
    - [WinRegistryEntry Object](#winregistryentry-object)
- [Releases](#releases)

---

# <a id="requirements" />Requirements

- **PowerShell 7.5** or later — [Download](https://aka.ms/powershell)
- **.NET 9 Runtime** — included with PowerShell 7.5 (no separate installation needed)
- **Windows** — the Windows Registry is a Windows-only feature

> Older PowerShell versions (7.4 and below) ship with .NET 8 or earlier and will fail to load the module with a clear error message.

---

# <a id="installation" />Installation

### Build from source

1. Open `ps.regedit.sln` in Visual Studio 2022 or later
2. Build the solution — the post-build event automatically copies the output to the `Module\` folder
3. Import the module:

```powershell
Import-Module "C:\path\to\PS.RegEdit\Module\psregedit.psd1"
```

### Manual import (single session)

```powershell
Import-Module "C:\path\to\PS.RegEdit\Module\psregedit.psd1"
```

### Permanent import (all sessions)

Add the import line to your PowerShell profile (`$PROFILE`):

```powershell
Import-Module "C:\path\to\PS.RegEdit\Module\psregedit.psd1"
```

---

# <a id="usage" />Usage

## <a id="basic-examples" />Basic Examples

```powershell
Import-Module PSRegedit

# Read a value
Get-RegValue -Hive LocalMachine -Path "SOFTWARE\Microsoft\Windows NT\CurrentVersion" -Name "ProductName"

# Write a value
Set-RegValue -Hive CurrentUser -Path "SOFTWARE\MyApp" -Name "Theme" -Value "Dark" -ValueKind String

# List subkeys
Get-RegSubKeyNames -Hive LocalMachine -Path "SOFTWARE\Microsoft"

# Read all entries from a key
Get-RegEntries -Hive LocalMachine -Path "SOFTWARE\MyApp"

# Delete a value (prompts for confirmation)
Remove-RegValue -Hive CurrentUser -Path "SOFTWARE\MyApp" -Name "Theme"
```

---

## <a id="all-cmdlets" />All Cmdlets

---

### <a id="get-regsubkeynames" />Get-RegSubKeyNames

Returns the names of all direct subkeys under a registry key.

**Parameters:**

| Parameter | Type | Required | Description |
|---|---|---|---|
| `-Hive` | `RegistryHive` | Yes | Registry hive |
| `-Path` | `string` | Yes | Registry key path |

**Returns:** `string` — one subkey name per pipeline object

**Example:**

```powershell
# List all subkeys under HKLM\SOFTWARE\Microsoft
Get-RegSubKeyNames -Hive LocalMachine -Path "SOFTWARE\Microsoft"

# Collect into an array
$subkeys = Get-RegSubKeyNames -Hive CurrentUser -Path "SOFTWARE" | Sort-Object
```

---

### <a id="get-regvalue" />Get-RegValue

Reads the raw string representation of a registry value.

**Parameters:**

| Parameter | Type | Required | Description |
|---|---|---|---|
| `-Hive` | `RegistryHive` | Yes | Registry hive |
| `-Path` | `string` | Yes | Registry key path |
| `-Name` | `string` | No | Value name. Omit or leave empty to read the `(Default)` value |

**Returns:** `string` — the value data, or `$null` if not found

**Examples:**

```powershell
# Read a named value
Get-RegValue -Hive LocalMachine -Path "SOFTWARE\Microsoft\Windows NT\CurrentVersion" -Name "ProductName"

# Read the (Default) value of a key
Get-RegValue -Hive ClassesRoot -Path ".txt"

# Store result
$osName = Get-RegValue -Hive LocalMachine -Path "SOFTWARE\Microsoft\Windows NT\CurrentVersion" -Name "ProductName"
Write-Host "OS: $osName"
```

---

### <a id="get-regentry" />Get-RegEntry

Reads a single registry value and returns it as a `WinRegistryEntry<string>` object, which includes metadata such as the value kind, path, and validity state.

**Parameters:**

| Parameter | Type | Required | Description |
|---|---|---|---|
| `-Hive` | `RegistryHive` | Yes | Registry hive |
| `-Path` | `string` | Yes | Registry key path |
| `-Name` | `string` | Yes | Value name |

**Returns:** [`WinRegistryEntry<string>`](#winregistryentry-object)

**Examples:**

```powershell
# Read entry and inspect metadata
$entry = Get-RegEntry -Hive LocalMachine -Path "SOFTWARE\MyApp" -Name "Version"

if ($entry.IsSet) {
    Write-Host "Value  : $($entry.Value)"
    Write-Host "Kind   : $($entry.ValueKind)"
    Write-Host "Valid  : $($entry.IsValid)"
}

# Check if a value exists before acting
$entry = Get-RegEntry -Hive CurrentUser -Path "SOFTWARE\MyApp" -Name "FirstRun"
if (-not $entry.IsSet) {
    Set-RegValue -Hive CurrentUser -Path "SOFTWARE\MyApp" -Name "FirstRun" -Value "0" -ValueKind DWord
}
```

---

### <a id="get-regentries" />Get-RegEntries

Reads all values from a registry key and returns them as `WinRegistryEntry<string>` objects. Use `-Recursive` to include all subkeys.

**Parameters:**

| Parameter | Type | Required | Description |
|---|---|---|---|
| `-Hive` | `RegistryHive` | Yes | Registry hive |
| `-Path` | `string` | Yes | Registry key path |
| `-Recursive` | `switch` | No | Also retrieve entries from all subkeys |

**Returns:** `WinRegistryEntry<string>` — one object per pipeline entry

**Examples:**

```powershell
# Read all values directly under a key
Get-RegEntries -Hive CurrentUser -Path "SOFTWARE\MyApp"

# Read all values recursively (key + all subkeys)
Get-RegEntries -Hive LocalMachine -Path "SOFTWARE\MyApp" -Recursive

# Filter to only entries that were found
$set = Get-RegEntries -Hive CurrentUser -Path "SOFTWARE\MyApp" | Where-Object { $_.IsSet }

# Display as table
Get-RegEntries -Hive LocalMachine -Path "SOFTWARE\Microsoft\Windows NT\CurrentVersion" |
    Select-Object Name, Value, ValueKind |
    Format-Table -AutoSize
```

---

### <a id="set-regvalue" />Set-RegValue

Writes a value to the registry. The key path is created automatically if it does not exist. Supports `-WhatIf` and `-Confirm`.

**Parameters:**

| Parameter | Type | Required | Description |
|---|---|---|---|
| `-Hive` | `RegistryHive` | Yes | Registry hive |
| `-Path` | `string` | Yes | Registry key path |
| `-Name` | `string` | No | Value name. Omit or leave empty to write the `(Default)` value |
| `-Value` | `object` | Yes | The value to write |
| `-ValueKind` | `RegistryValueKind` | Yes | Registry data type (see [Registry Value Types](#registry-value-types)) |

**Examples:**

```powershell
# Write a string value
Set-RegValue -Hive CurrentUser -Path "SOFTWARE\MyApp" -Name "Theme" -Value "Dark" -ValueKind String

# Write a DWORD (integer)
Set-RegValue -Hive CurrentUser -Path "SOFTWARE\MyApp" -Name "MaxRetries" -Value 3 -ValueKind DWord

# Write the (Default) value
Set-RegValue -Hive ClassesRoot -Path "MyFileType" -Value "My Application File" -ValueKind String

# Preview without making changes
Set-RegValue -Hive CurrentUser -Path "SOFTWARE\MyApp" -Name "Debug" -Value 1 -ValueKind DWord -WhatIf
```

---

### <a id="new-regkey" />New-RegKey

Creates a new registry key. Does nothing if the key already exists. Supports `-WhatIf` and `-Confirm`.

**Parameters:**

| Parameter | Type | Required | Description |
|---|---|---|---|
| `-Hive` | `RegistryHive` | Yes | Registry hive |
| `-Path` | `string` | Yes | Registry key path to create |

**Examples:**

```powershell
# Create a new key
New-RegKey -Hive CurrentUser -Path "SOFTWARE\MyApp\Settings"

# Preview without making changes
New-RegKey -Hive CurrentUser -Path "SOFTWARE\MyApp\Settings" -WhatIf
```

---

### <a id="remove-regvalue" />Remove-RegValue

Deletes a single registry value. Prompts for confirmation by default (`ConfirmImpact = High`). Supports `-WhatIf` and `-Force`.

**Parameters:**

| Parameter | Type | Required | Description |
|---|---|---|---|
| `-Hive` | `RegistryHive` | Yes | Registry hive |
| `-Path` | `string` | Yes | Registry key path |
| `-Name` | `string` | Yes | Name of the value to delete |

**Examples:**

```powershell
# Delete a value (will prompt for confirmation)
Remove-RegValue -Hive CurrentUser -Path "SOFTWARE\MyApp" -Name "Theme"

# Delete without confirmation prompt
Remove-RegValue -Hive CurrentUser -Path "SOFTWARE\MyApp" -Name "Theme" -Force

# Preview without making changes
Remove-RegValue -Hive CurrentUser -Path "SOFTWARE\MyApp" -Name "Theme" -WhatIf
```

---

### <a id="remove-regtree" />Remove-RegTree

Deletes a registry key and **all** its subkeys and values recursively. This is a destructive operation — prompts for confirmation by default (`ConfirmImpact = High`). Supports `-WhatIf` and `-Force`.

**Parameters:**

| Parameter | Type | Required | Description |
|---|---|---|---|
| `-Hive` | `RegistryHive` | Yes | Registry hive |
| `-Path` | `string` | Yes | Registry key path to delete |

**Examples:**

```powershell
# Delete a key tree (will prompt for confirmation)
Remove-RegTree -Hive CurrentUser -Path "SOFTWARE\MyApp"

# Delete without confirmation prompt
Remove-RegTree -Hive CurrentUser -Path "SOFTWARE\MyApp" -Force

# Preview without making changes
Remove-RegTree -Hive CurrentUser -Path "SOFTWARE\MyApp" -WhatIf
```

---

## <a id="registry-hives" />Registry Hives

The `-Hive` parameter accepts any `RegistryHive` enum value. PowerShell tab-completes these automatically.

| Enum Value | Common Abbreviation | Description |
|---|---|---|
| `ClassesRoot` | HKCR | File type associations and COM registrations |
| `CurrentUser` | HKCU | Settings for the currently logged-in user |
| `LocalMachine` | HKLM | Machine-wide settings (requires elevation to write) |
| `Users` | HKU | Settings for all user profiles |

> `CurrentConfig` is not supported and will throw an `ArgumentException`.

---

## <a id="registry-value-types" />Registry Value Types

The `-ValueKind` parameter on `Set-RegValue` accepts any `RegistryValueKind` enum value.

| Enum Value | Registry Type | .NET Type | Description |
|---|---|---|---|
| `String` | REG_SZ | `string` | Plain text string |
| `ExpandString` | REG_EXPAND_SZ | `string` | String with environment variable placeholders (e.g. `%WINDIR%`) |
| `DWord` | REG_DWORD | `int` (32-bit) | 32-bit integer |
| `QWord` | REG_QWORD | `long` (64-bit) | 64-bit integer |
| `Binary` | REG_BINARY | `byte[]` | Raw binary data |
| `MultiString` | REG_MULTI_SZ | `string[]` | Array of strings |

---

## <a id="winregistryentry-object" />WinRegistryEntry Object

`Get-RegEntry` and `Get-RegEntries` return `WinRegistryEntry<string>` objects with the following properties:

| Property | Type | Description |
|---|---|---|
| `Hive` | `RegistryHive` | The registry hive of the entry |
| `Path` | `string` | The registry key path |
| `Name` | `string` | The value name (`$null` or empty for the default value) |
| `Value` | `string` | The value data as a string |
| `ValueKind` | `RegistryValueKind` | The data type of the value in the registry |
| `IsSet` | `bool` | `$true` if the value was found and successfully read |
| `IsValid` | `bool` | `$true` if `IsSet` is true and all validation rules pass |
| `IsLocked` | `bool` | `$true` if the entry has been locked to prevent re-reads |

**Example — working with the entry object:**

```powershell
$entry = Get-RegEntry -Hive LocalMachine -Path "SOFTWARE\MyApp" -Name "Version"

# Check if the value exists
if ($entry.IsSet) {
    Write-Host "Version: $($entry.Value) (Type: $($entry.ValueKind))"
} else {
    Write-Warning "Version value not found in registry."
}

# Pipeline: get all set entries and display as a table
Get-RegEntries -Hive CurrentUser -Path "SOFTWARE\MyApp" |
    Where-Object IsSet |
    Select-Object Name, Value, ValueKind |
    Format-Table -AutoSize
```

---

# Disclaimer

This software is provided **"as is"**, without warranty of any kind, express or implied, including but not limited to the warranties of merchantability, fitness for a particular purpose, and non-infringement.

The author(s) accept **no liability** for any direct, indirect, incidental, special, or consequential damages arising from the use or misuse of this module, including but not limited to:

- Unintended modification or deletion of registry keys and values
- System instability or data loss caused by incorrect registry edits
- Any damage to software, hardware, or data resulting from the use of this module

**The Windows Registry is a critical system component.** Incorrect modifications can render your operating system unstable or unbootable. Always back up your registry before making changes and test in a safe environment before deploying to production systems.

By using this module, you accept full responsibility for any changes made to the Windows Registry on your system.

---

# <a id="releases" />Releases

### 1.0.0 — 19 May 2026

Initial release.

**Cmdlets:**
- `Get-RegSubKeyNames` — list direct subkeys of a registry key
- `Get-RegValue` — read a raw string value
- `Get-RegEntry` — read a value as a `WinRegistryEntry<string>` with metadata
- `Get-RegEntries` — read all values from a key, optionally recursive
- `Set-RegValue` — write a value (creates the key path if needed)
- `New-RegKey` — create a registry key
- `Remove-RegValue` — delete a registry value (confirm prompt)
- `Remove-RegTree` — delete a registry key tree recursively (confirm prompt)

**Included classes:**
- `WinRegistry` — core read/write/delete operations
- `WinRegistryEntry<T>` — generic registry entry with fluent API, type conversion, and validation rules

**Requirements:** PowerShell 7.5 / .NET 9 / Windows
