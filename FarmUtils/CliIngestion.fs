// Consume the DocOpt parsed inputs dictionary and convert
// to CliArgs (HelpArgs | VersionArgs | CommandArgs | QueryArgs)
module FarmUtils.CliIngestion

open Docopt.Arguments
open FarmUtils.CliCommon

 
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
 
let makeCommandArgs (parsed:Dictionary): CliArgs =
   let subcommandType =
     parsed
     |> Seq.find((fun (KeyValue(k, _)) -> List.contains k COMMAND_KEYS))
   match subcommandType.Key with
   | "born" -> CommandArgs(makeBornArgs parsed)
   | "died" -> CommandArgs(makeDiedArgs parsed)
   | _ -> failwith "missing subcommand match"


type StopOrContinue =
  | Stop of CliArgs
  | Continue of Dictionary

let bind continueHandler stopOrContinue =
  match stopOrContinue with
  | Continue parsed -> continueHandler parsed
  | Stop value -> Stop value

let helpStop (parsed:Dictionary): StopOrContinue =
  match parsed.Item "-h" with
  | Flag _ -> Stop (HelpArgs "TODO")
  | _ -> Continue parsed

let versionStop (parsed:Dictionary): StopOrContinue =
  match parsed.Item "--version" with
  | Flag _ -> Stop (VersionArgs CliMessages.VERSION)
  | _ -> Continue parsed

let commandStop (parsed:Dictionary): StopOrContinue =
  let found = parsed |> Seq.filter((fun (KeyValue(k, _)) -> List.contains k COMMAND_KEYS)) |> Seq.toList
  match found.Length with
  | 1 -> Stop(makeCommandArgs parsed)
  | _ -> Continue parsed
  
let queryStop (parsed:Dictionary): StopOrContinue =
  Continue parsed
  
let versionStop' = bind versionStop
let commandStop' = bind commandStop
let queryStop' = bind queryStop

let convertToCliArgs (input:Dictionary): CliArgs =
  let railroad = helpStop >> versionStop' >> commandStop' >> queryStop'
  input |> railroad |> (fun stopOrContinue ->
    match stopOrContinue with
    | Stop cliArgs -> cliArgs
    | Continue _ -> InvalidArgs CliMessages.INVALID_COMMAND)
  