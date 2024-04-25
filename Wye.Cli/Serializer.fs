namespace Wye

open Wye.Jobs

[<RequireQualifiedAccess>]
module Serializer =

    open System.Collections.Generic
    open YamlDotNet.Serialization
    
    [<CLIMutable>]
    [<YamlSerializable>]
    type YStep = {
        
        [<YamlMember>] command : string
    }
    
    [<CLIMutable>]
    [<YamlSerializable>]
    type YVar = {
        [<YamlMember>] name : string
        [<YamlMember>] value : string
    }
    
    [<CLIMutable>]
    [<YamlSerializable>]
    type YJob = {
         [<YamlMember>] id : string
         [<YamlMember>] dependsOn : string array
         [<YamlMember>] vars : YVar array
         [<YamlMember>] steps : YStep array
    }
    
    [<CLIMutable>]
    [<YamlSerializable>]
    type YJobs = {
        [<YamlMember>] jobs : YJob array
    }
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
