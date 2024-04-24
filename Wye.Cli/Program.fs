
open Wye.Jobs

let jobs = [
    { 
        Id = "jhello"
        Vars = []
        Steps = [
            { Command = """echo "hello"; echo "hello" >> "$OUTPUT" """ }
            { Command = """echo "world";echo "world" >> "$OUTPUT" """ }
        ]
        DependsOn = [] 
    }
    { 
        Id = "jhello2"
        Vars = []
        Steps = [
            { Command = """echo "hello"; echo "hello" >> "$OUTPUT" """ }
            { Command = """echo "world";echo "world" >> "$OUTPUT" """ }
        ]
        DependsOn = [] 
    }
    { 
        Id = "jprint";
        Vars = [
            ("OUTPUT1", "$$jobs.jhello.steps.0.output")
            ("OUTPUT2", "$$jobs.jhello.steps.1.output")
        ]
        Steps = [
            { Command = """echo "$OUTPUT1 $OUTPUT2" """ }
        ]
        DependsOn = ["jhello"] 
    }
    // { 
    //     Id = "D";
    //     Steps = [
    //         { Command = "echo"; Args = ["D"]; Output = Text; Target = StdOut }
    //     ]
    //     DependsOn = ["B"; "C"] 
    // }
]


let plan = executionPlanParallel jobs
let result: list<array<list<string>>> = executeJobs jobs
// printfn "%A" plan
// printfn "%A" result
writeOutput result
