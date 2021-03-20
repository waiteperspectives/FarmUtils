// Consume CliArgs and handle, producing CliResponse
// Top-level concerns, e.g. Help and Version stop early
// Commands or Queries are converted to the domain types and processed
module FarmUtils.CliProcessing

open FarmUtils.CliCommon
open FarmUtils.Domain
open SqlStreamStore


let makeBornCommand(args:BornArgs): DomainCommand =
  BornCommand {Name=args.Name; Dam=args.Dam}
  
let makeDiedCommand(args:DiedArgs): DomainCommand =
  DiedCommand {Name=args.Name}

let convertToDomainCommand(cmdArgs:CommandArgs): DomainCommand =
  match cmdArgs with
  | BornArgs args -> makeBornCommand args
  | DiedArgs args -> makeDiedCommand args

let handleDomainCommand
  (store:InMemoryStreamStore)
  (domainCmd:DomainCommand): string =
    // first query the event store stream for the proper events
    // TODO: implement query
    let originalEvents = List.empty<DomainEvent>
    // second: decide - hydrate the appropriate state known by the domain,
    // then execute the new command, giving back events or an error
    let decision = decide originalEvents domainCmd
    // third: based on the result of the decision, save new events to the stream
    // TODO: implement save
    match decision with
    | Ok events -> events |> sprintf "Saving: %A"
    | Error err -> err |> sprintf "Error: %A"
 
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
 
