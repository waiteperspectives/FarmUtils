module FarmUtils.CliCommon


[<RequireQualifiedAccess>]
module CliMessages = 
  let VERSION = "\nHerd Inventory - Version 1.0.0\n\n"
  let NOTIMPLEMENTED = "This command has not yet been implemented"
  let INVALID_COMMAND = "Invalid Command!\n"

let COMMAND_KEYS = ["born"; "died"]
let QUERY_KEYS = ["show"]
type ShowArgs = ShowArgs of string

type QueryArgs =
  | ShowArgs
  
type BornArgs = {
  Name: string
  Dam: string
}

type DiedArgs = {
  Name: string
}

type CommandArgs =
  | BornArgs of BornArgs
  | DiedArgs of DiedArgs

type CliArgs =
  | HelpArgs of string
  | VersionArgs of string
  | InvalidArgs of string
  | CommandArgs of CommandArgs
  | QueryArgs of QueryArgs

type CliResponse = CliResponse of string
