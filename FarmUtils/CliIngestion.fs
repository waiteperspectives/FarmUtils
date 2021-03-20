// Consume the DocOpt parsed inputs dictionary and convert
// to CliArgs (HelpArgs | VersionArgs | CommandArgs | QueryArgs)
module FarmUtils.CliIngestion

open Docopt.Arguments
open FarmUtils.CliCommon

module CommandIngestion =
  type StopOrContinue =
    | Stop of CommandArgs
    | Continue of Dictionary
    
  let bind continueHandler stopOrContinue =
    match stopOrContinue with
    | Continue parsed -> continueHandler parsed
    | Stop value -> Stop value
    
  let makeBornArgs (parsed:Dictionary): CommandArgs =
          if parsed.ContainsKey("born") then
            let herName =
              match parsed.Item "<name>" with
              | Argument s -> s
              | _ -> failwith "Missing name"
            let herDam=
              match parsed.Item "--dam" with
              | Argument s -> s
              | _ -> failwith "Missing --dam"
            BornArgs { Name=herName; Dam=herDam }
          else
            failwith "Noop"
        
  let bornStop (parsed:Dictionary): StopOrContinue =
    try
      Stop(makeBornArgs parsed)
    with _ ->
      Continue parsed
      
  let makeDiedArgs (parsed:Dictionary): CommandArgs =
          if parsed.ContainsKey("died") then
            let herName =
              match parsed.Item "<name>" with
              | Argument s -> s
              | _ -> "???"
          
            DiedArgs { Name=herName }
          else
            failwith "Noop"

  let diedStop (parsed:Dictionary): StopOrContinue =
    try
      Stop(makeDiedArgs parsed)
    with _ ->
      Continue parsed

  let diedStop' = bind diedStop


module QueryIngestion =
  type StopOrContinue =
    | Stop of QueryArgs
    | Continue of Dictionary
    
  let bind continueHandler stopOrContinue =
    match stopOrContinue with
    | Continue parsed -> continueHandler parsed
    | Stop value -> Stop value
    
  let makeShowArgs (parsed:Dictionary): QueryArgs =
          // TODO: refactor using cow instead of show to allow showing other things
          if parsed.ContainsKey("cow") then
            let herName =
              match parsed.Item "<name>" with
              | Argument s -> s
              | _ -> failwith "Missing name"
            ShowArgs { Name=herName; }
          else
            failwith "Noop"
        
  let showStop (parsed:Dictionary): StopOrContinue =
    try
      Stop(makeShowArgs parsed)
    with _ ->
      Continue parsed
 
 module ProgramIngestion =
  type StopOrContinue =
    | Stop of CliArgs
    | Continue of Dictionary

  let bind continueHandler stopOrContinue =
    match stopOrContinue with
    | Continue parsed -> continueHandler parsed
    | Stop value -> Stop value

  let helpStop (usage:string) (parsed:Dictionary): StopOrContinue =
    match parsed.Item "-h" with
    | Flag _ -> Stop (HelpArgs usage)
    | _ -> Continue parsed

  let versionStop (parsed:Dictionary): StopOrContinue =
    match parsed.Item "--version" with
    | Flag _ -> Stop (VersionArgs CliMessages.VERSION)
    | _ -> Continue parsed
  let versionStop' = bind versionStop
    
  let commandStop (parsed:Dictionary): StopOrContinue =
    let commandRailroad =
      CommandIngestion.bornStop
      >> CommandIngestion.diedStop'
    parsed |> commandRailroad |> (fun stopOrContinue ->
        match stopOrContinue with
        | CommandIngestion.Stop commandArgs -> Stop(CommandArgs commandArgs)
        | CommandIngestion.Continue _ -> Continue parsed
      )
  let commandStop' = bind commandStop
    
  let queryStop (parsed:Dictionary): StopOrContinue =
    let queryRailroad =
      QueryIngestion.showStop
    parsed |> queryRailroad |> (fun stopOrContinue ->
        match stopOrContinue with
        | QueryIngestion.Stop queryArgs -> Stop(QueryArgs queryArgs)
        | QueryIngestion.Continue _ -> Continue parsed
      )
  let queryStop' = bind queryStop

// "Public"
let convertToCliArgs (env:Environment) (input:Dictionary): CliArgs =
  let helpStop' = ProgramIngestion.helpStop env.Usage
  let railroad =
    helpStop'
    >> ProgramIngestion.versionStop'
    >> ProgramIngestion.commandStop'
    >> ProgramIngestion.queryStop'
  input |> railroad |> (fun stopOrContinue ->
    match stopOrContinue with
    | ProgramIngestion.Stop cliArgs -> cliArgs
    | ProgramIngestion.Continue _ -> InvalidArgs CliMessages.INVALID_COMMAND)
  