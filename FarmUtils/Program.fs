open Docopt
open FarmUtils.CliCommon
open FarmUtils.CliIngestion
open FarmUtils.CliProcessing
open FarmUtils.Outputs


let DOC = """
FarmUtils
=============================================================

Usage:
  cow born <name> --dam=<dam> [--asof=<date>] [--force]...
  cow died <name> [--asof=<date>] [--force]...
  cow show <name> [--asof=<date>]...
  cow (-h | --help)
  cow --version


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
