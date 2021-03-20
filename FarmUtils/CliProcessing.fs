// Consume CliArgs and handle, producing CliResponse
// Top-level concerns, e.g. Help and Version stop early
// Commands or Queries are converted to the domain types and processed
module FarmUtils.CliProcessing

open SqlStreamStore
open FarmUtils.CliCommon
open FarmUtils.Domain
open FarmUtils.DomainService


let makeBornCommand(args:BornArgs): DomainCommand =
  BornCommand {Name=args.Name; Dam=args.Dam}
  
let makeDiedCommand(args:DiedArgs): DomainCommand =
  DiedCommand {Name=args.Name}

let convertToDomainCommand(cmdArgs:CommandArgs): DomainCommand =
  match cmdArgs with
  | BornArgs args -> makeBornCommand args
  | DiedArgs args -> makeDiedCommand args
 
let handleCmdArgs (store:InMemoryStreamStore) (cmdArgs:CommandArgs): CliResponse =
  cmdArgs
  |> convertToDomainCommand
  |> handleDomainCommand store
  |> CliResponse
  
let handleQueryArgs (queryArgs:QueryArgs): CliResponse =
  match queryArgs with
  | ShowArgs args -> CliResponse (sprintf "Showing info for %s" args.Name)

let handleCliArgs (env:Environment) (args:CliArgs): CliResponse =
  match args with
  | HelpArgs _ -> CliResponse env.Usage
  | VersionArgs _ -> CliResponse CliMessages.VERSION
  | InvalidArgs _ -> CliResponse CliMessages.INVALID_COMMAND
  | CommandArgs cmdArgs -> cmdArgs |> handleCmdArgs env.Store
  | QueryArgs queryArgs -> queryArgs |> handleQueryArgs
 
