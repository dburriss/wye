# Wye

> This is alpha software. Use at your own risk.

A task runner allowing easy sharing of data across jobs.

## Requirements

- Currently only supports bash
- `dotnet` for installing the tool. [Download here](https://dotnet.microsoft.com/en-us/download)

## Installation

```bash
dotnet tool install -g Wye.Cli
```

## Usage

Create a `config.yml` file in the root of your project.
    
```yaml
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

```
Then execute the following command:

```bash
wye run ./config.yml
```

## How it works

Wye is a task runner that allows you to share data between jobs. 
It does this by creating a temporary file for each job and storing the output of each step in that file. 
Storing is done by simply sending the output to `$OUTPUT`.
The output of a step can be referenced in another job by using the `$$jobs.<job-id>.steps.<step-id>.output` syntax.

When you define `vars` key value pairs in a job, you can reference that variable in the job.
