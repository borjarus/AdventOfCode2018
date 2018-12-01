module App.Main

    open App.Day1

    [<EntryPoint>]
    let main argv =
        let out = match argv with
                    | [|day|] -> match day with
                                    | "1" -> sprintf "%A" <| Day1.part2()
                                    | _ -> ""
                    | _ -> "Insert number of day as parameter" 
        printfn "%s" out
        0 // return an integer exit code
