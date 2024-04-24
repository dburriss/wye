namespace Wye

module Jobs =
    open System
    open FSharp

    type Step = {
        Command: string
    }

    type Job = {
        Id: string
        Vars: (string * string) list
        Steps: Step list
        DependsOn: string list
    }

    let executionPlanParallel jobs =
        let rec buildPlan (jobs: Job list) (plan: Job list list) =
            match jobs with
            | [] -> plan
            | _ ->
                let ready = jobs |> List.filter (fun j -> j.DependsOn |> List.forall (fun d -> plan |> List.concat |> List.exists (fun p -> p.Id = d)))
                let notReady = jobs |> List.filter (fun j -> j.DependsOn |> List.exists (fun d -> not (plan |> List.concat |> List.exists (fun p -> p.Id = d))))
                buildPlan notReady (plan @ [ready])
        buildPlan jobs []

    let executionPlanSequetial jobs =
        let rec buildPlan (jobs: Job list) (plan: Job list) =
            match jobs with
            | [] -> plan
            | _ ->
                let ready = jobs |> List.filter (fun j -> j.DependsOn |> List.forall (fun d -> plan |> List.exists (fun p -> p.Id = d)))
                let notReady = jobs |> List.filter (fun j -> j.DependsOn |> List.exists (fun d -> not (plan |> List.exists (fun p -> p.Id = d))))
                buildPlan notReady (plan @ ready)
        buildPlan jobs []


    let private serialize a =
        Text.Json.JsonSerializer.Serialize(a)

    let private deserialize<'a> (json: string) =
        Text.Json.JsonSerializer.Deserialize<'a>(json)
    let executeJobs jobs =
        // todo: have a log file for the complete run
        let jobsId = $"jobs-{DateTime.Now.ToFileTimeUtc()}"
        IO.Directory.CreateDirectory(jobsId) |> ignore
        let jobsDir = IO.Path.GetFullPath(jobsId)
        let plan = executionPlanParallel jobs
        plan 
        |> List.map (
            fun js -> 
                js 
                |> List.toArray
                |> Array.Parallel.map (
                    fun j -> 
                        let jPath = IO.Path.Combine(jobsDir, j.Id)
                        IO.Directory.CreateDirectory(jPath) |> ignore
                        let jobDir = IO.Path.GetFullPath(jPath)
                        j.Steps 
                        |> List.mapi (
                            fun i s ->
                                // get envs and set them as env vars
                                let inputEnvs = 
                                    j.Vars
                                    |> List.map (fun (k,v) -> 
                                        let value = 
                                            if v.StartsWith("$$jobs.") && v.Contains("step") && v.EndsWith("output") then
                                                let parts = v.Split('.')
                                                let job = parts.[1]
                                                let step = parts.[3]
                                                let outputDir = IO.Path.Combine(jobsDir, job)
                                                let output = IO.Path.Combine(outputDir, $"step-{int step}.out")
                                                let outVal = IO.File.ReadAllText(output) |> String.trim
                                                outVal
                                            else v
                                        (k, value)
                                    )

                                // set the output file
                                let output = IO.Path.Combine(jobDir, $"step-{i}.out")
                                let exportOutput = ("OUTPUT", output)
                                // set the env vars
                                let envs = inputEnvs @ [exportOutput]
                                // execute the command
                                Proc.run "bash" ["-c";s.Command] envs "." 
                                |> function | Ok o -> o | Error e -> [|sprintf "Error: %A" e|]
                                |> Array.toList
                        )
                        |> List.concat
                )
        )
    let writeOutput (result: list<array<list<string>>>) =
        result
        |> List.iter (
            fun js -> 
                js 
                |> Array.iter (
                    fun j -> 
                        j 
                        |> List.iter (
                            fun s -> printfn "%s" s
                        )
                )
        )    
