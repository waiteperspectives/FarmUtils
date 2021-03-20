// Knows about the store, but not the user interface
module FarmUtils.DomainService

open SqlStreamStore
open FarmUtils.Domain

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
