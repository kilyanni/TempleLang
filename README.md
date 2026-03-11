# C--

Compiler for C-- (formerly TempleLang), a small systems language that compiles to x64 via NASM.
Targets Linux and Windows.

## Usage

To get the CLI, [download a release](https://github.com/blenderfreaky/TempleLang/releases) or build from source.

```
TempleLang.CLI 1.0.0

  -f, --file        Required. Source files to compile. First file determines output path.
                    Pass multiple files space-separated: -f main.tl stdlib/linux.tl

  -t, --target      Path to place the output executable in.

  -r, --run         Run the generated executable upon successful compilation.

  -i, --printIL     Output the intermediate language to the target directory.

  -a, --printASM    Output the assembler to the target directory.

  -p, --platform    Target platform: 'windows' or 'linux'. Defaults to current OS.

  --help            Display this help screen.

  --version         Display version information.
```

## Prerequisites

**All platforms:** [NASM](https://www.nasm.us/) must be installed and on `PATH`.

**Linux:** `gcc` must be on `PATH` (used as a linker driver for libc).

**Windows:** `link.exe` (MSVC linker) must be on `PATH`.
It ships with the MSVC Build Tools, available via the [Visual Studio installer](https://visualstudio.microsoft.com/downloads/).
Run the compiler from a Developer Command Prompt or after calling `vcvarsall.bat`, which sets
the `LIB` environment variable so `link.exe` can find the Windows SDK libraries automatically.
If neither `LIB` nor `WindowsSdkDir`/`WindowsSdkVersion` are set, the compiler falls back to the
default Windows 10 SDK path (`C:\Program Files (x86)\Windows Kits\10\Lib\10.0.18362.0\um\x64` (HACK)).

## Standard library

The `stdlib/` directory provides platform bindings and utilities:

| File | Contents |
|------|----------|
| `stdlib/linux.tl` | `malloc`, `free`, `print`, `printDigit` via libc |
| `stdlib/windows.tl` | `malloc`, `free`, `print`, `printDigit` via kernel32 |
| `stdlib/strings.tl` | `printNum`, `printNumAny` (platform-agnostic) |

Source files can import other files with `import "relative/path.tl";`. Imports are resolved
relative to the importing file, deduplicated, and cycle-safe.

## Example

See [QuickSort](https://github.com/blenderfreaky/TempleLang/tree/master/QuickSort) for an example.

`QuickSort.tl` unconditionally imports `stdlib/strings.tl` for number formatting. The user
supplies the OS binding at compile time:

```sh
# Linux
./TempleLang.CLI -f QuickSort/QuickSort.tl stdlib/linux.tl -r

# Windows (from Developer Command Prompt)
./TempleLang.CLI -f QuickSort/QuickSort.tl stdlib/windows.tl -r

# Cross-compile (run on Linux, target Windows)
./TempleLang.CLI -f QuickSort/QuickSort.tl stdlib/windows.tl --platform windows
```
