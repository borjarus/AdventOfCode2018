module Helpers

open System
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

let getOrDefault key (map: Map<'T,'U>) ``default`` = 
    match map.TryFind(key) with
    | Some s -> s
    | None -> ``default``


let (|Regex|_|) pattern input =
        let m = Regex.Match(input, pattern)
        if m.Success then Some(List.tail [ for g in m.Groups -> g.Value ])
        else None

let (=>) a b = a, b

let (|*|) s1 l2 =
    s1
    |> Seq.map (fun e1 -> l2 |> List.map ((=>) e1))
    |> Seq.concat

let cycle (lst:'a list) = 
    let rec next () = 
        seq {
            for element in lst do
                yield element
            yield! next()
        }
    next()

module String =
    let trim (s: string) = s.Trim()
    let split (sep: string) (s: string) = (s.Split([| sep |], StringSplitOptions.None)) |> Array.toList


    
