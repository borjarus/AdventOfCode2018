(*

--- Day 6: Chronal Coordinates ---
The device on your wrist beeps several times, and once again you feel like you're falling.

"Situation critical," the device announces. "Destination indeterminate. Chronal interference detected. Please specify new target coordinates."

The device then produces a list of coordinates (your puzzle input). Are they places it thinks are safe or dangerous? It recommends you check manual page 729. The Elves did not give you a manual.

If they're dangerous, maybe you can minimize the danger by finding the coordinate that gives the largest distance from the other points.

Using only the Manhattan distance, determine the area around each coordinate by counting the number of integer X,Y locations that are closest to that coordinate (and aren't tied in distance to any other coordinate).

Your goal is to find the size of the largest area that isn't infinite. For example, consider the following list of coordinates:

1, 1
1, 6
8, 3
3, 4
5, 5
8, 9
If we name these coordinates A through F, we can draw them on a grid, putting 0,0 at the top left:

..........
.A........
..........
........C.
...D......
.....E....
.B........
..........
..........
........F.
This view is partial - the actual grid extends infinitely in all directions. Using the Manhattan distance, each location's closest coordinate can be determined, shown here in lowercase:

aaaaa.cccc
aAaaa.cccc
aaaddecccc
aadddeccCc
..dDdeeccc
bb.deEeecc
bBb.eeee..
bbb.eeefff
bbb.eeffff
bbb.ffffFf
Locations shown as . are equally far from two or more coordinates, and so they don't count as being closest to any.

In this example, the areas of coordinates A, B, C, and F are infinite - while not shown here, their areas extend forever outside the visible grid. However, the areas of coordinates D and E are finite: D is closest to 9 locations, and E is closest to 17 (both including the coordinate's location itself). Therefore, in this example, the size of the largest area is 17.

What is the size of the largest area that isn't infinite?

*)

module App.Day6
open Helpers
open System

    let parseLine line = 
        match line with 
        | Regex @"(\d+), (\d+)" [d1; d2] -> (int32 d1, int32 d2)
        | _ -> failwith "parsing error"
    
    let maxX parsedInput = 
        parsedInput |> Seq.map fst |> Seq.reduce max |> int

    let maxY parsedInput = 
        parsedInput |> Seq.map snd |> Seq.reduce max |> int 


    
    let manhattan (x:int32, y: int32) (x': int32, y': int32) =
         (Math.Abs (x - x')) + (Math.Abs (y - y'))
    
    let closestTo  inp (x,y) =
        let closest =  
            inp |> Seq.map (fun (x',y') -> 
                (manhattan (x,y) (x',y'), (x',y'))) |> Seq.sortBy fst
        if (closest |> Seq.item 0 |> fst) <> (closest |> Seq.item 1 |> fst)
        then Some ((closest |> Seq.item 0 |> snd), (x,y))
        else None
    

    let examples1() =
        let input = 
            parseLines "1, 1\n
1, 6\n
8, 3\n
3, 4\n
5, 5\n
8, 9\n"
        let parsedInput = 
            input
            |> Seq.map parseLine
        
        
        
        let maxX = maxX parsedInput
        let maxY = maxY parsedInput

        let coordinates =
            seq {
                for x in [0.. maxX] do
                    for y in [0..maxY] do 
                    yield x, y}

        let isInfiniteArea area =
            area 
            |> Seq.exists (fun (x,y) ->
                if x = 0 || y = 0 || x >= maxX || y >= maxY 
                then true
                else false
            )


        let largestAreaSize =
            let groups = 
                coordinates
                |> Seq.filter (fun inp -> 
                    let x = inp |> closestTo parsedInput
                    match x with 
                    | Some _ -> true
                    | None -> false)
                |> Seq.groupBy fst 
            groups
            |> Seq.map snd
            |> Seq.map (fun inp ->
            if not (isInfiniteArea inp)
            then Some (Seq.length inp)
            else None)
            //|> Seq.filter (fun tup -> 
            //    if not (isInfiniteArea tup)
            //    then true
            //    else false
            //)

//(defn solve-1 []
//  (let [groups (group-by first (keep closest-to coordinates))]
//    (apply max (keep #(when (not (infinite-area? (map second %)))
//        (count %)) (vals groups)))))


        largestAreaSize |> Seq.toList






    let examples2() = 
        let input = "dabAcCaCBAcCcaDA"
        ()


    let part1() = 
        let input = readLinesFromFile(@"day6.txt") |> Seq.head
        ()

    let part2() = 
        let input = readLinesFromFile(@"day6.txt") |> Seq.head
        ()