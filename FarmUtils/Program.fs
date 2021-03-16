open Docopt
open Docopt.Arguments


// Desired CLI
//let DOC = """
//Herd Inventory
//=============================================================
//
//Usage:
//  cow born <name> --dam=<dam> [--asof=<date>] [--force]...
//  cow died <name> [--asof=<date>] [--force]...
//  cow bought <name> [--asof=<date>] [--force]...
//  cow sold <name> [--asof=<date>]...
//  cow show <name> [--asof=<date>]...
//  cow show --report=<report> [--asof=<date>]
//  cow (-h | --help)
//  cow --version
//
//
//Options:
//  -h --help     Show this screen.
//  --version     Show version.
//"""

let DOC = """
Herd Inventory
=============================================================

Usage:
  cow born <name> --dam=<dam> [--asof=<date>] [--force]...
  cow died <name> [--asof=<date>] [--force]...
  cow (-h | --help)
  cow --version


Options:
  -h --help     Show this screen.
  --version     Show version.
"""

let VERSION = "\nHerd Inventory - Version 1.0.0\n\n"
let NOTIMPLEMENTED = "This command has not yet been implemented"
let INVALID_COMMAND = sprintf "Invalid Command!\n%s" DOC
let COMMAND_KEYS = ["born"; "died"]
let QUERY_KEYS = ["show"]

type BornArgs = {
  Name: string
  Dam: string
}

type DiedArgs = {
  Name: string
}

type BornCommand = {
  Name: string
  Dam: string
}

type DiedCommand = {
  Name: string
}

type DomainCommand =
  | BornCommand of BornCommand
  | DiedCommand of DiedCommand
  
type BornEvent = {
  Name: string
  Dam: string
}

type DiedEvent = {
  Name: string
}

type DomainEvent =
  | BornEvent of BornEvent
  | DiedEvent of DiedEvent

// TODO: these functions feel repetitive, how to improve
let makeBornCommand(args:BornArgs): DomainCommand =
  BornCommand {Name=args.Name; Dam=args.Dam}
  
let makeDiedCommand(args:DiedArgs): DomainCommand =
  DiedCommand {Name=args.Name}

let makeBornEvent(cmd:BornCommand): DomainEvent =
  BornEvent {Name=cmd.Name; Dam=cmd.Dam;}
  
let makeDiedEvent(cmd:DiedCommand): DomainEvent =
  DiedEvent {Name=cmd.Name}
  
type ShowArgs = ShowArgs of string

type QueryArgs =
  | ShowArgs

type CommandArgs =
  | BornArgs of BornArgs
  | DiedArgs of DiedArgs
 
type CliArgs =
  | HelpArgs of string
  | VersionArgs of string
  | InvalidArgs of string
  | CommandArgs of CommandArgs
  | QueryArgs of QueryArgs
 
let makeBornArgs (parsed:Dictionary): CommandArgs =
      let herName =
        match parsed.Item "<name>" with
        | Argument s -> s
        | _ -> "???"
    
      let herDam=
        match parsed.Item "--dam" with
        | Argument s -> s
        | _ -> "???"
      
      BornArgs { Name=herName; Dam=herDam }
      
let makeDiedArgs (parsed:Dictionary): CommandArgs =
      let herName =
        match parsed.Item "<name>" with
        | Argument s -> s
        | _ -> "???"
    
      DiedArgs { Name=herName }
 
 // TODO: leverage complete pattern matching rather than using _
let makeCommandArgs (parsed:Dictionary): CliArgs =
   let subcommandType =
     parsed
     |> Seq.find((fun (KeyValue(k, _)) -> List.contains k COMMAND_KEYS))
   match subcommandType.Key with
   | "born" -> CommandArgs(makeBornArgs parsed)
   | "died" -> CommandArgs(makeDiedArgs parsed)
   | _ -> failwith "missing subcommand match"
  
// "Help" and "Version" are not domain objects, so should not be sent deeper
type CliResponse = CliResponse of string

type StopOrContinue =
  | Stop of CliArgs
  | Continue of Dictionary

let bind continueHandler stopOrContinue =
  match stopOrContinue with
  | Continue parsed -> continueHandler parsed
  | Stop value -> Stop value

let helpStop (parsed:Dictionary): StopOrContinue =
  let maybeHelp = parsed.Item "-h"
  match maybeHelp with
  | Flag _ -> Stop (HelpArgs DOC)
  | None -> Continue parsed
  | _ -> failwith "????"

let versionStop (parsed:Dictionary): StopOrContinue =
  let maybeVersion = parsed.Item "--version"
  match maybeVersion with
  | Flag _ -> Stop (VersionArgs VERSION)
  | None -> Continue parsed
  | _ -> failwith "????"

let commandStop (parsed:Dictionary): StopOrContinue =
  let found = parsed |> Seq.filter((fun (KeyValue(k, _)) -> List.contains k COMMAND_KEYS)) |> Seq.toList
  match found.Length with
  | 1 -> Stop(makeCommandArgs parsed)
  | _ -> Continue parsed
  
let queryStop (parsed:Dictionary): StopOrContinue =
  Continue parsed
  
// wire-up: (help | version | command | query)
// uses Railway oriented programing style of handling subcommands -
// if one is matched, parsing stops and executes the subcommand
let versionStop' = bind versionStop
let commandStop' = bind commandStop
let queryStop' = bind queryStop

let convertToCliArgs (input:Dictionary): CliArgs =
  let railroad = helpStop >> versionStop' >> commandStop' >> queryStop'
  input |> railroad |> (fun stopOrContinue ->
    match stopOrContinue with
    | Stop cliArgs -> cliArgs
    | Continue _ -> InvalidArgs INVALID_COMMAND)
 
let convertToDomainCommand(cmdArgs:CommandArgs): DomainCommand =
  match cmdArgs with
  | BornArgs args -> makeBornCommand args
  | DiedArgs args -> makeDiedCommand args

let handleDomainCommand (domainCmd:DomainCommand): DomainEvent list =
  match domainCmd with
  | BornCommand cmd -> [makeBornEvent cmd;]
  | DiedCommand cmd -> [makeDiedEvent cmd;]

let handleCmdArgs (cmdArgs:CommandArgs): CliResponse =
  cmdArgs
  |> convertToDomainCommand
  |> handleDomainCommand
  |> sprintf "Saved: %A"
  |> CliResponse
 
let handleCliArgs (args:CliArgs): CliResponse =
  match args with
  | HelpArgs _ -> CliResponse DOC
  | VersionArgs _ -> CliResponse VERSION
  | InvalidArgs _ -> CliResponse INVALID_COMMAND
  | CommandArgs cmdArgs -> cmdArgs |> handleCmdArgs
  | QueryArgs query -> CliResponse (query |> sprintf "%A")
 
let printCliResponse (response:CliResponse): unit =
  match response with
  | CliResponse s -> s |> printfn "%s"

[<EntryPoint>]
let main argv =
  try
    Docopt(DOC).Parse(argv)
    |> convertToCliArgs
    |> handleCliArgs
    |> printCliResponse
    0
  with ArgvException _ ->
    printfn "Error: %s" INVALID_COMMAND
    -1
