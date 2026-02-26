# General

- Prefer simple solutions
- Ask if unsure
- Keep answers concise
- Break solutions into small incremental steps
- Do one step at a time

# Tech stack

- .NET 8
- F#
- Argu (CLI argument parsing)
- YamlDotNet (YAML parsing)

# Build and test

- Build: `dotnet build`
- Pack: `dotnet pack`
- Clean: `dotnet clean`

> There are currently no automated test projects.

# Structure

Wye is a .NET 8 CLI tool (global tool) written in F# that acts as a task runner, allowing easy sharing of data across jobs via YAML configuration files.

- `Wye.Cli/` - Main CLI project (F# source files)
  - `Lib.fs` - Core library utilities
  - `Proc.fs` - Process execution logic
  - `Jobs.fs` - Job and step definitions
  - `Serializer.fs` - YAML serialization/deserialization
  - `Cmd.fs` - CLI command definitions
  - `Program.fs` - Entry point
- `examples/` - Example YAML configuration files
