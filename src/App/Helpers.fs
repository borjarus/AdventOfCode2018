module Helpers

open System.IO
open System.Text.RegularExpressions

let readLinesFromFile (filePath:string) = seq {
    use sr = new StreamReader (filePath)
    while not sr.EndOfStream do
        yield sr.ReadLine ()
}

let readLines(s: string) =
    s.Split [| '\n' |] 

let parseLines (s: string) =
    s.Split([|'\r'; '\n'|])
   |> Array.choose (fun row ->
        match row.Trim() with
        | "" -> None
        | "\n" -> None
        | row -> Some row)
    |> Seq.ofArray

let (|Regex|_|) pattern input =
        let m = Regex.Match(input, pattern)
        if m.Success then Some(List.tail [ for g in m.Groups -> g.Value ])
        else None


