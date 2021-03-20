open Docopt
open FarmUtils.CliCommon
open FarmUtils.CliIngestion
open FarmUtils.CliProcessing
open FarmUtils.Outputs


let DOC = """
FarmUtils
=============================================================

Usage:
  farmutils born <name> --dam=<dam> [--asof=<date>] [--force]...
  farmutils died <name> [--asof=<date>] [--force]...
  farmutils cow <name> [--asof=<date>]...
  farmutils (-h | --help)
  farmutils --version


Options:
  -h --help     Show this screen.
  --version     Show version.
"""

let store = new SqlStreamStore.InMemoryStreamStore()
let env = {Usage=DOC;Store=store;}

[<EntryPoint>]
let main argv =
  try
    Docopt(DOC).Parse(argv)
    |> convertToCliArgs env
    |> handleCliArgs env
    |> printCliResponse
    0
  with ArgvException _ ->
    printfn "Error: %s" CliMessages.INVALID_COMMAND
    printfn "%s" DOC
    -1
