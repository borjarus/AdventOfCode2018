module App.Main

    [<EntryPoint>]
    let main argv =
        let out = 
            match argv with
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
                | "11" -> sprintf "%A" <| Day11.part2()
                | "12" -> sprintf "%A" <| Day12.part2()
                | "13" -> sprintf "%A" <| Day13.part2()
                | "14" -> sprintf "%A" <| Day14.part2()
                | "15" -> sprintf "%A" <| Day15.part2()
                | "16" -> sprintf "%A" <| Day16.part2()
                | "17" -> sprintf "%A" <| Day17.part2()
                | "18" -> sprintf "%A" <| Day18.part2()
                | "19" -> sprintf "%A" <| Day19.part2()
                | "20" -> sprintf "%A" <| Day20.part2()

                | "22" -> sprintf "%A" <| Day22.part1()

                | _ -> ""
            | _ -> "Insert number of day as parameter" 
        printfn "%s" out
        0 // return an integer exit code
