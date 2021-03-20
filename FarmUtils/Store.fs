module FarmUtils.Store

open System
open SqlStreamStore.FSharp
open SqlStreamStore.Streams
open FarmUtils.Domain

type EventStream = {
    Name:string
}

let saveEvents
    (store:SqlStreamStore.InMemoryStreamStore)
    (stream:EventStream) 
    (events:DomainEvent list) =
    0