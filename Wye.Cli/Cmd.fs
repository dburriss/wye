namespace Wye

open Argu.ArguAttributes

module Cmd =
    open Argu
    
    type RunArgs =
        | [<AltCommandLine("-c"); MainCommand>] Config_File of config_file: string

        interface IArgParserTemplate with
            member s.Usage =
                match s with
                | Config_File _ -> "specify a Wye YAML config file (required)."

    
    type CliArguments =
        | [<CliPrefix(CliPrefix.None)>] Run of ParseResults<RunArgs>
        | [<Hidden>] Working_Directory of path: string
        | [<Hidden>] Log_Level of level: int

        interface IArgParserTemplate with
            member s.Usage =
                match s with
                | Run _ -> "run a Wye pipeline."
                | Working_Directory _ -> "specify a working directory. Default is the current directory."
                | Log_Level _ -> "set the log level: 0=ERROR, 1=INFO, 2=VERBOSE. Default is 0."

    type Config = {
        WyeFile: string
        WorkingDirectory: string
        LogLevel: int
    }
    
    let parser = ArgumentParser.Create<CliArguments>(programName = "wye") 
    let usage = parser.PrintUsage() 
    let parseArguments (args: string array) =
        try
            let result = parser.ParseCommandLine(args)
            let runOpt = result.GetResult(Run)
            let wyeFile = runOpt |> _.GetResult(Config_File) 
            let workingDirectory = result.TryGetResult(Working_Directory) |> Option.defaultValue "."
            let logLevel = result.TryGetResult(Log_Level) |> Option.defaultValue 0
            { WyeFile = wyeFile; WorkingDirectory = workingDirectory; LogLevel = logLevel }
        with
        | :? ArguParseException as ex ->
            printfn "%s" ex.Message
            printfn "%s" usage
            exit 1
