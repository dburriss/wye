
open System
open Wye


Environment.GetCommandLineArgs()
|> Array.tail
|> Cmd.parseArguments
|> fun config -> IO.File.ReadAllText(config.WyeFile)
|> Serializer.deserializeYaml
|> _.Jobs
|> Jobs.executeJobs
|> Jobs.writeOutput
