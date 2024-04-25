namespace Wye
module Util =
    let tap f x = f x; x

// ========================================================
// String: String module 
// Helpers for working with strings
// ========================================================
module String =
    open System
    let isNotEmpty s = not (String.IsNullOrEmpty s)
    let join (ss: string seq) = String.Join("", ss)
    let lower (s: string) = s.ToLowerInvariant()
    let trim (s: string) = s.Trim()
    let replace (oldValue: string) newValue (s: string) = s.Replace(oldValue,newValue)
    let split (by: string) (s: string) = s.Split(by)
    let endsWith (tail: string) (s: string) = s.EndsWith(tail)
    let startsWith (head: string) (s: string) = s.StartsWith(head)
    let contains (search: string) (s: string) = s.Contains(search)

// ========================================================
// Env: Env module 
// Helper functions for interacting with the local environment
// ========================================================
type OS = | Windows | Linux | OSX | FreeBSD
module Env =
    open System
    open System.Runtime.InteropServices
    let os() =
        if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then Windows
        elif RuntimeInformation.IsOSPlatform(OSPlatform.Linux) then Linux
        elif RuntimeInformation.IsOSPlatform(OSPlatform.OSX) then OSX
        else FreeBSD

    let varNames() : string seq = seq { for k in Environment.GetEnvironmentVariables().Keys -> k :?> string }

    let path() =
        Environment.GetEnvironmentVariable("PATH")

    let tryGet name =
        let value = Environment.GetEnvironmentVariable(name)
        if isNull value then None
        else Some value

    let executables os =
        match os with
        | Windows -> [".exe";".bat";".cmd";".ps1"]
        | _ -> ["";".sh"]

    let dirSep() = string IO.Path.DirectorySeparatorChar
