module App.Main

    [<EntryPoint>]
    let main argv =
        let out = match argv with
                    | [|day|] -> match day with
                                    | "1" -> sprintf "%A" <| Day1.part2()
                                    | "2" -> sprintf "%A" <| Day2.part2()
                                    | "3" -> sprintf "%A" <| Day3.part2()
                                    | "4" -> sprintf "%A" <| Day4.part2()
                                    | "5" -> sprintf "%A" <| Day5.part2()
                                    | "6" -> sprintf "%A" <| Day6.part2()
                                    | "7" -> sprintf "%A" <| Day7.part2()
                                    | "8" -> sprintf "%A" <| Day8.part2()
                                    | "9" -> sprintf "%A" <| Day9.part2()
                                    | "10" -> sprintf "%A" <| Day10.part2()
                                    | _ -> ""
                    | _ -> "Insert number of day as parameter" 
        printfn "%s" out
        0 // return an integer exit code
