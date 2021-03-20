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
  
type DomainError = DomainError of string
  
let makeBornEvent(cmd:BornCommand): DomainEvent =
  BornEvent {Name=cmd.Name; Dam=cmd.Dam;}
  
let makeDiedEvent(cmd:DiedCommand): DomainEvent =
  DiedEvent {Name=cmd.Name}

let decide
  (originalEvents:DomainEvent list)
  (command:DomainCommand):
  Result<DomainEvent list, DomainError> =
    match command with
    | BornCommand cmd -> Ok [makeBornEvent cmd;]
    | DiedCommand cmd -> Ok [makeDiedEvent cmd;]
