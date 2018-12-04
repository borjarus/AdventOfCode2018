(*
--- Day 2: Inventory Management System ---
You stop falling through time, catch your breath, and check the screen on the device. "Destination reached. Current Year: 1518. Current Location: North Pole Utility Closet 83N10." You made it! Now, to find those anomalies.

Outside the utility closet, you hear footsteps and a voice. "...I'm not sure either. But now that so many people have chimneys, maybe he could sneak in that way?" Another voice responds, "Actually, we've been working on a new kind of suit that would let him fit through tight spaces like that. But, I heard that a few days ago, they lost the prototype fabric, the design plans, everything! Nobody on the team can even seem to remember important details of the project!"

"Wouldn't they have had enough fabric to fill several boxes in the warehouse? They'd be stored together, so the box IDs should be similar. Too bad it would take forever to search the warehouse for two similar box IDs..." They walk too far away to hear any more.

Late at night, you sneak to the warehouse - who knows what kinds of paradoxes you could cause if you were discovered - and use your fancy wrist device to quickly scan every box and produce a list of the likely candidates (your puzzle input).

To make sure you didn't miss any, you scan the likely candidate boxes again, counting the number that have an ID containing exactly two of any letter and then separately counting those with exactly three of any letter. You can multiply those two counts together to get a rudimentary checksum and compare it to what your device predicts.

For example, if you see the following box IDs:

abcdef contains no letters that appear exactly two or three times.
bababc contains two a and three b, so it counts for both.
abbcde contains two b, but no letter appears exactly three times.
abcccd contains three c, but no letter appears exactly two times.
aabcdd contains two a and two d, but it only counts once.
abcdee contains two e.
ababab contains three a and three b, but it only counts once.
Of these box IDs, four of them contain a letter which appears exactly twice, and three of them contain a letter which appears exactly three times. Multiplying these together produces a checksum of 4 * 3 = 12.

--- Part Two ---
Confident that your list of box IDs is complete, you're ready to find the boxes full of prototype fabric.

The boxes will have IDs which differ by exactly one character at the same position in both strings. For example, given the following box IDs:

abcde
fghij
klmno
pqrst
fguij
axcye
wvxyz
The IDs abcde and axcye are close, but they differ by two characters (the second and fourth). However, the IDs fghij and fguij differ by exactly one character, the third (h and u). Those must be the correct boxes.

What letters are common between the two correct box IDs? (In the example above, this is found by removing the differing character from either ID, producing fgij.)
*)

module App.Day2
    open Helpers
    open System.Collections.Generic

    let charFreq (s: string) =
        let charList: char list = Seq.toList s
        let out = Dictionary<char,int>()
        charList |> List.iter 
            (fun el -> 
                match (out.ContainsKey el) with
                    | true -> out.[el] <- out.[el] + 1
                    | false -> out.Add(el, 1)) 
        out
    
    let findCheckSum (dict: Dictionary<char, int>) =
        let mutable out = (0,0)
        dict |> Seq.iter (fun el -> match el.Value with
                            | 2 -> out <- if (fst out) = 0 then (fst out) + 1, snd out else out
                            | 3 -> out <- if (snd out) = 0 then fst out, (snd out) + 1 else out
                            | _ -> () )
        out

    let sum input  =
        let a,b = input |> List.fold (fun acc el -> 
            let v = el |> charFreq |> findCheckSum
            let f1 = (fst v) + (fst acc)
            let f2 = (snd v) + (snd acc)
            (f1, f2)) (0,0)
        a * b

    let identicallyFun (input: string seq) =
        let closedTo id1 id2 =
            Seq.zip id1 id2
            |> Seq.filter ((<||) (=))
            |> Seq.length

        input
        |> Seq.choose (fun id -> 
            input
            |> Seq.tryFind (closedTo id >> ((=) (id.Length - 1)))
            |> Option.map (fun m -> id, m) 
        )
        |> Seq.head 
        ||> Seq.zip 
        |> Seq.choose (fun (c1,c2) -> if c1 = c2 then Some (string c1) else None) 
        |> String.concat ""
    


    let examples1() =
        let testInput1 = "abcdef"
        let testInput2 = "bababc"
        let testInput3 = "abbcde"
        let testInput4 = "abcccd"
        let testInput5 = "aabcdd"
        let testInput6 = "abcdee"
        let testInput7 = "ababab"
        let testInput = [testInput1; testInput2; testInput3; testInput4; testInput5; testInput6; testInput7] 
        
        sum testInput
            
            

    let examples2() = 
        let testInput = "abcde\nfghij\nklmno\npqrst\nfguij\naxcye\nwvxyz"
        identicallyFun (testInput |> parseLines)






    let part1() = 
        let input = readLinesFromFile(@"day2.txt")
        let out = Seq.toList(input) |> sum

        out

    let part2() = 
        let input = readLinesFromFile(@"day2.txt")
        let out = identicallyFun input

        out