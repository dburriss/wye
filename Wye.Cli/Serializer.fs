namespace Wye

open Wye.Jobs

[<RequireQualifiedAccess>]
module Serializer =

    open System.Collections.Generic
    open YamlDotNet.Serialization
    [<CLIMutable>]
    type YStep = { command : string }
    [<CLIMutable>]
    type YVar = { name : string; value : string }
    [<CLIMutable>]
    type YJob = {
         id : string
         dependsOn : string array
         vars : YVar array
         steps : YStep array
    }
    [<CLIMutable>]
    type YJobs = { jobs : YJob array }
    let private deserializer = DeserializerBuilder().Build()
    let deserializeYaml (yamlString : string) =
        let y = deserializer.Deserialize<YJobs>(yamlString)
        
        // map to Jobs.Job
        let jobs = y.jobs |> Array.toList |> List.map (fun yJob ->
            let dependsOn = yJob.dependsOn |> Option.ofObj |> Option.defaultValue [||] |> Array.toList
            let vars = yJob.vars |> Option.ofObj |> Option.defaultValue [||] |> Array.toList |> List.map (fun x -> (x.name, x.value))
            let steps = yJob.steps |> Array.toList |> List.map (fun x -> { Command = x.command })
            {
                Id = yJob.id
                DependsOn = dependsOn
                Vars = vars
                Steps = steps 
            }
        )
        let result: Jobs =
            {
                Jobs = jobs
            }
        result
