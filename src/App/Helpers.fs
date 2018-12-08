module Helpers

open System.IO
open System.Text.RegularExpressions

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

let readLinesFromFile (filePath:string) = seq {
    use sr = new StreamReader (filePath)
    while not sr.EndOfStream do
        yield sr.ReadLine()
}



let (|Regex|_|) pattern input =
        let m = Regex.Match(input, pattern)
        if m.Success then Some(List.tail [ for g in m.Groups -> g.Value ])
        else None

let (=>) a b = a, b

let (|*|) s1 l2 =
    s1
    |> Seq.map (fun e1 -> l2 |> List.map ((=>) e1))
    |> Seq.concat


