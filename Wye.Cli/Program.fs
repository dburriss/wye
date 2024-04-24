
open Wye.Jobs

// let jobs = [
//     { 
//         Id = "jhello"
//         Vars = []
//         Steps = [
//             { Command = """echo "hello"; echo "hello" >> "$OUTPUT" """ }
//             { Command = """echo "world";echo "world" >> "$OUTPUT" """ }
//         ]
//         DependsOn = [] 
//     }
//     { 
//         Id = "jhello2"
//         Vars = []
//         Steps = [
//             { Command = """echo "hello"; echo "hello" >> "$OUTPUT" """ }
//             { Command = """echo "world";echo "world" >> "$OUTPUT" """ }
//         ]
//         DependsOn = [] 
//     }
//     { 
//         Id = "jprint";
//         Vars = [
//             ("OUTPUT1", "$$jobs.jhello.steps.0.output")
//             ("OUTPUT2", "$$jobs.jhello.steps.1.output")
//         ]
//         Steps = [
//             { Command = """echo "$OUTPUT1 $OUTPUT2" """ }
//         ]
//         DependsOn = ["jhello"] 
//     }
//
// ]

let yamlString = """
jobs:
  - id: hello
    steps:
      - command: echo "Hello" >> $OUTPUT
      - command: echo "World" >> $OUTPUT
  - id: print
    dependsOn: [hello]
    vars:
      - name: GREET
        value: $$jobs.hello.steps.0.output
      - name: PLACE
        value: $$jobs.hello.steps.1.output
    steps:
      - command: echo "$GREET $PLACE"
"""
let jobs = Wye.Serializer.deserializeYaml yamlString


let plan = executionPlanParallel jobs.Jobs
let result: list<array<list<string>>> = executeJobs jobs.Jobs
// printfn "%A" plan
// printfn "%A" result
writeOutput result
