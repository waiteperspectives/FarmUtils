// Consume CliArgs and handle, producing CliResponse
// Top-level concerns, e.g. Help and Version stop early
// Commands or Queries are converted to the domain types and processed
module FarmUtils.CliProcessing

open FarmUtils.CliCommon
open FarmUtils.CliIngestion
open FarmUtils.Domain


let makeBornCommand(args:BornArgs): DomainCommand =
  BornCommand {Name=args.Name; Dam=args.Dam}
  
let makeDiedCommand(args:DiedArgs): DomainCommand =
  DiedCommand {Name=args.Name}

let convertToDomainCommand(cmdArgs:CommandArgs): DomainCommand =
  match cmdArgs with
  | BornArgs args -> makeBornCommand args
  | DiedArgs args -> makeDiedCommand args
 
let handleCmdArgs (cmdArgs:CommandArgs): CliResponse =
  cmdArgs
  |> convertToDomainCommand
  |> handleDomainCommand
  |> sprintf "Saved: %A"
  |> CliResponse
  
let handleQueryArgs (queryArgs:QueryArgs): CliResponse =
  match queryArgs with
  | ShowArgs args -> CliResponse (sprintf "Showing info for %s" args.Name)

let handleCliArgs (env:Map<string,string>) (args:CliArgs): CliResponse =
  match args with
  | HelpArgs _ -> CliResponse env.["usage"]
  | VersionArgs _ -> CliResponse CliMessages.VERSION
  | InvalidArgs _ -> CliResponse CliMessages.INVALID_COMMAND
  | CommandArgs cmdArgs -> cmdArgs |> handleCmdArgs
  | QueryArgs queryArgs -> queryArgs |> handleQueryArgs
 
