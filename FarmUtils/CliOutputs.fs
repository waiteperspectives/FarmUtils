module FarmUtils.Outputs

open FarmUtils.CliCommon

let printCliResponse (response:CliResponse): unit =
  match response with
  | CliResponse s -> s |> printfn "%s"

