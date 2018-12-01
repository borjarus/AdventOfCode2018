module Helpers

open System.IO

let readLinesFromFile (filePath:string) = seq {
    use sr = new StreamReader (filePath)
    while not sr.EndOfStream do
        yield sr.ReadLine ()
}

let readLines(s: string) =
    s.Split [| '\n' |] 


