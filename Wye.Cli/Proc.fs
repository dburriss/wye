namespace Wye
 // ========================================================
// Proc: Proc module 
// For calling CLI process on the machine
// ========================================================
module Proc =
    open System
    open System.Diagnostics
    let private fileName (dir: string) command ext = 
        let pathSplit = Env.dirSep()
        let path = if IO.Path.EndsInDirectorySeparator(dir) then dir else dir + pathSplit
        $"{path}{command}{ext}"

    let private searchDirectoryForCommand extensions command dir =
        let fullFilePath = fileName dir command
        let toCheck = 
            extensions
            |> List.map fullFilePath
        toCheck |> List.filter (fun f -> IO.File.Exists(f))

    let private fetchCommand os envPATH command =
        let extensions = Env.executables os
        let dirs = envPATH |> String.split (string IO.Path.PathSeparator) |> List.ofArray
        let executables = 
            dirs 
            |> List.map (searchDirectoryForCommand extensions command)
            |> List.concat
        
        executables |> List.tryHead

    let run (command: string) (args: string seq) (envs: (string*string) list) workingDir =
        let commandOpt = fetchCommand (Env.os()) (Env.path()) command
        match commandOpt with
        | Some cmd ->
            Debug.WriteLine(sprintf "RUNNING COMMAND: %s" cmd)
            let info = ProcessStartInfo(cmd, args)
            info.WorkingDirectory <- workingDir //must be full path not using ~/
            info.UseShellExecute <- false
            info.RedirectStandardOutput <- true
            info.RedirectStandardError <- true
            do envs |> List.iter (fun (k,v) -> info.EnvironmentVariables.Add(k, v))

            let p = Process.Start(info)
            
            let output =
                [|
                    while not p.StandardOutput.EndOfStream do
                        let o = p.StandardOutput.ReadLine()
                        if String.isNotEmpty o then o
                |]
                
            let err = p.StandardError.ReadToEnd()
            p.WaitForExit()
            if p.ExitCode = 0 || String.IsNullOrEmpty(err) then
                Ok output
            else Error (p.ExitCode, err)
        | None -> Error(-1, $"Unable to find executable ({command}). Make sure it appears in your PATH.")
