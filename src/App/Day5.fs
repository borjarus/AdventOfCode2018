(*

--- Day 5: Alchemical Reduction ---
You've managed to sneak in to the prototype suit manufacturing lab. The Elves are making decent progress, but are still 
struggling with the suit's size reduction capabilities.

While the very latest in 1518 alchemical technology might have solved their problem eventually, you can do better. 
You scan the chemical composition of the suit's material and discover that it is formed by extremely long polymers 
(one of which is available as your puzzle input).

The polymer is formed by smaller units which, when triggered, react with each other such that two adjacent units 
of the same type and opposite polarity are destroyed. Units' types are represented by letters; units' polarity is 
represented by capitalization. For instance, r and R are units with the same type but opposite polarity, whereas r and s 
are entirely different types and do not react.

For example:

In aA, a and A react, leaving nothing behind.
In abBA, bB destroys itself, leaving aA. As above, this then destroys itself, leaving nothing.
In abAB, no two adjacent units are of the same type, and so nothing happens.
In aabAAB, even though aa and AA are of the same type, their polarities match, and so nothing happens.
Now, consider a larger example, dabAcCaCBAcCcaDA:

dabAcCaCBAcCcaDA  The first 'cC' is removed.
dabAaCBAcaDA
dabAaCBAcCcaDA    This creates 'Aa', which is removed.
dabCBAcCcaDA      Either 'cC' or 'Cc' are removed (the result is the same).
dabCBAcaDA        No further actions can be taken.
dabCBAcaDA
After all possible reactions, the resulting polymer contains 10 units.

How many units remain after fully reacting the polymer you scanned? (Note: in this puzzle and others, the input is large; 
if you copy/paste your input, make sure you get the whole thing.)

--- Part Two ---
Time to improve the polymer.

One of the unit types is causing problems; it's preventing the polymer from collapsing as much as it should. 
Your goal is to figure out which unit type is causing the most problems, remove all instances of it (regardless of polarity), 
fully react the remaining polymer, and measure its length.

For example, again using the polymer dabAcCaCBAcCcaDA from above:

Removing all A/a units produces dbcCCBcCcD. Fully reacting this polymer produces dbCBcD, which has length 6.
Removing all B/b units produces daAcCaCAcCcaDA. Fully reacting this polymer produces daCAcaDA, which has length 8.
Removing all C/c units produces dabAaBAaDA. Fully reacting this polymer produces daDA, which has length 4.
Removing all D/d units produces abAcCaCBAcCcaA. Fully reacting this polymer produces abCBAc, which has length 6.
In this example, removing all C/c units was best, producing the answer 4.

What is the length of the shortest polymer you can produce by removing all units of exactly one type and fully 
reacting the result?

*)

module App.Day5
open Helpers
open System

    type Polymer = Plus of char | Minus of char

    let parseSingle inp = 
        match inp with 
        | Regex @"([a-z])" [x] -> Minus (char x)
        | Regex @"([A-Z])" [x] -> Plus (char x)
        | _ -> failwith "parse error"

    let polymerComparator inp =
         match inp with 
         |  Plus x, Minus y -> 
            if Char.ToLower(x) = Char.ToLower(y) then 
                None
            else 
                Some(Plus x, Minus y)
         | Minus x, Plus y -> 
            if Char.ToLower(x) = Char.ToLower(y) then 
                None
            else 
                Some(Minus x, Plus y)
         | _ -> Some inp

    let polymerComparator2 in1 in2 =
        let i1, i2 = in1
        let j1, j2 = in2
        if Char.ToLower(i1) = Char.ToLower(j1) && Char.ToLower(i1) = Char.ToLower(j1) && Char.ToLower(i2) = Char.ToLower(j2)
        then None
        else Some (i1,i2)

    let polymerParser (inp: string) =
        let polymers = Seq.toList inp 
        let rec parse acc (inp: char list) =       
            match inp with
            | x :: y :: rest -> 
                let x' = parseSingle (string x)
                let y' = parseSingle (string y)
                match polymerComparator (x',y') with
                | Some _ -> parse (x::acc) (y::rest)
                | None -> parse [] (List.concat [acc |> List.rev ;rest])
            | x :: rest -> parse (x::acc) [] 
            | [] -> acc
            | _ -> failwith "parse error"
        parse [] polymers |> List.rev |> String.Concat

    let polymerParserBest input =
        let inp, t1, t2 = input
        let polymers = Seq.toList inp 
        let rec parse acc (inp: char list) =       
            match inp with
            | x :: y :: rest -> 
                match polymerComparator2 (x,y) (t1, t2) with
                | Some _ -> parse (x::acc) (y::rest)
                | None -> parse [] (List.concat [acc |> List.rev ;rest])
            | x :: _ -> parse (x::acc) [] 
            | [] -> acc
            | _ -> failwith "parse error"
        parse [] polymers |> List.rev |> String.Concat


    let examples1() =
        let input = "dabAcCaCBAcCcaDA"
        (polymerParser input).Length

    

            

    let examples2() = 
        let input = "dabAcCaCBAcCcaDA"
        //let input = readLinesFromFile(@"day5.txt") |> Seq.head
        List.zip ['a'..'z'] ['A'..'Z']
        |> List.filter (fun el ->
            let letter = el |> fst
            String.exists (fun c -> c = letter) input
        )
        |> List.map (fun (a,b) -> 
            
            let optimized = 
                input |> Seq.toList |> List.filter (fun el -> 
                if el = a || el = b then false else true
                ) |> String.Concat
            (polymerParser optimized).Length


        ) |> List.sort |> List.head
        //input |> Seq.toList |> List.map Char.ToLower |> List.distinct |> List.sortDescending







    let part1() = 
        let input = readLinesFromFile(@"day5.txt")
        (input |> Seq.head |> polymerParser).Length


    let part2() = 
        let input = readLinesFromFile(@"day5.txt") |> Seq.head
        
        List.zip ['a'..'z'] ['A'..'Z']
        |> List.filter (fun el ->
            let letter = el |> fst
            String.exists (fun c -> c = letter) input
        )
        |> List.map (fun (a,b) -> 
            
            let optimized = 
                input |> Seq.toList |> List.filter (fun el -> 
                if el = a || el = b then false else true
                ) |> String.Concat
            (polymerParser optimized).Length


        ) |> List.sort |> List.head