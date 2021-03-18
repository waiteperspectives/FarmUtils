// Commands, Events, Streams, State Transitions ...
module FarmUtils.Domain

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
  
let makeBornEvent(cmd:BornCommand): DomainEvent =
  BornEvent {Name=cmd.Name; Dam=cmd.Dam;}
  
let makeDiedEvent(cmd:DiedCommand): DomainEvent =
  DiedEvent {Name=cmd.Name}

let handleDomainCommand (domainCmd:DomainCommand): DomainEvent list =
  match domainCmd with
  | BornCommand cmd -> [makeBornEvent cmd;]
  | DiedCommand cmd -> [makeDiedEvent cmd;]

