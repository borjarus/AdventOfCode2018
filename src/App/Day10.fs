(*

--- Day 10: The Stars Align ---

It's no use; your navigation system simply isn't capable of providing walking directions in the arctic circle, and certainly not in 1018.

The Elves suggest an alternative. In times like these, North Pole rescue operations will arrange points of light 
in the sky to guide missing Elves back to base. Unfortunately, the message is easy to miss: the points move slowly enough that
it takes hours to align them, but have so much momentum that they only stay aligned for a second. If you blink at the wrong time, 
it might be hours before another message appears.

You can see these points of light floating in the distance, and record their position in the sky and their velocity, 
the relative change in position per second (your puzzle input). The coordinates are all given from your perspective; given enough time,
those positions and velocities will move the points into a cohesive message!

Rather than wait, you decide to fast-forward the process and calculate what the points will eventually spell.

For example, suppose you note the following points:

position=< 9,  1> velocity=< 0,  2>
position=< 7,  0> velocity=<-1,  0>
position=< 3, -2> velocity=<-1,  1>
position=< 6, 10> velocity=<-2, -1>
position=< 2, -4> velocity=< 2,  2>
position=<-6, 10> velocity=< 2, -2>
position=< 1,  8> velocity=< 1, -1>
position=< 1,  7> velocity=< 1,  0>
position=<-3, 11> velocity=< 1, -2>
position=< 7,  6> velocity=<-1, -1>
position=<-2,  3> velocity=< 1,  0>
position=<-4,  3> velocity=< 2,  0>
position=<10, -3> velocity=<-1,  1>
position=< 5, 11> velocity=< 1, -2>
position=< 4,  7> velocity=< 0, -1>
position=< 8, -2> velocity=< 0,  1>
position=<15,  0> velocity=<-2,  0>
position=< 1,  6> velocity=< 1,  0>
position=< 8,  9> velocity=< 0, -1>
position=< 3,  3> velocity=<-1,  1>
position=< 0,  5> velocity=< 0, -1>
position=<-2,  2> velocity=< 2,  0>
position=< 5, -2> velocity=< 1,  2>
position=< 1,  4> velocity=< 2,  1>
position=<-2,  7> velocity=< 2, -2>
position=< 3,  6> velocity=<-1, -1>
position=< 5,  0> velocity=< 1,  0>
position=<-6,  0> velocity=< 2,  0>
position=< 5,  9> velocity=< 1, -2>
position=<14,  7> velocity=<-2,  0>
position=<-3,  6> velocity=< 2, -1>

Each line represents one point. Positions are given as <X, Y> pairs: X represents how far left (negative) or right (positive) the point appears, 
while Y represents how far up (negative) or down (positive) the point appears.

At 0 seconds, each point has the position given. Each second, each point's velocity is added to its position. 
So, a point with velocity <1, -2> is moving to the right, but is moving upward twice as quickly. If this point's initial position were <3, 9>, after 3 seconds, 
its position would become <6, 3>.

Over time, the points listed above would move like this:

Initially:
........#.............
................#.....
.........#.#..#.......
......................
#..........#.#.......#
...............#......
....#.................
..#.#....#............
.......#..............
......#...............
...#...#.#...#........
....#..#..#.........#.
.......#..............
...........#..#.......
#...........#.........
...#.......#..........

After 1 second:
......................
......................
..........#....#......
........#.....#.......
..#.........#......#..
......................
......#...............
....##.........#......
......#.#.............
.....##.##..#.........
........#.#...........
........#...#.....#...
..#...........#.......
....#.....#.#.........
......................
......................

After 2 seconds:
......................
......................
......................
..............#.......
....#..#...####..#....
......................
........#....#........
......#.#.............
.......#...#..........
.......#..#..#.#......
....#....#.#..........
.....#...#...##.#.....
........#.............
......................
......................
......................

After 3 seconds:
......................
......................
......................
......................
......#...#..###......
......#...#...#.......
......#...#...#.......
......#####...#.......
......#...#...#.......
......#...#...#.......
......#...#...#.......
......#...#..###......
......................
......................
......................
......................

After 4 seconds:
......................
......................
......................
............#.........
........##...#.#......
......#.....#..#......
.....#..##.##.#.......
.......##.#....#......
...........#....#.....
..............#.......
....#......#...#......
.....#.....##.........
...............#......
...............#......
......................
......................

After 3 seconds, the message appeared briefly: HI. Of course, your message will be much longer and will 
take many more seconds to appear.

What message will eventually appear in the sky?

--- Part Two ---
Good thing you didn't have to wait, because that would have taken a long time - much longer than 
the 3 seconds in the example above.

Impressed by your sub-hour communication capabilities, the Elves are curious: exactly 
how many seconds would they have needed to wait for that message to appear?


*)

module App.Day10
open Helpers


    let height = 10

    let iterate sList = 
        sList 
        |> List.map  (function                                       
            | p1, p2, v1, v2 -> (p1 + v1), (p2 + v2), v1, v2
            | _ -> failwith "incorrect list length")


    //let rec findGrouped listOfHeights inp   = 
    //    match iterate inp with
    //    | out when List.exists (fun el -> 
    //            (out |> List.groupBy (fun (_ ,y, _, _) -> y) |> List.length) = el) listOfHeights  -> out
    //    | out -> findGrouped listOfHeights out 

    let rec findGrouped listOfHeights inp   = 
        match iterate inp with
        | out when (out |> List.groupBy (fun (_ ,y, _, _) -> y) |> List.length) = height -> out
        | out -> findGrouped listOfHeights out 

    let rec findSeq inp = seq{
            let inner, out = match iterate inp with
                | out  -> (out |> List.groupBy (fun (_ ,y, _, _) -> y) |> List.length), out
            yield inner
            yield! findSeq out
            
        }

    let generateStr lst =
        [| 0 .. (Set.maxElement lst)|]
        |> Array.map (fun x -> if Set.contains x lst then '#' else '.')
        |> System.String

    let generateMassage lstOfStars =
        let minX = lstOfStars |> List.minBy fst |> fst
        let minY = lstOfStars |> List.minBy fst |> fst

        lstOfStars
        |> List.map (fun (x,y) -> 
            (x - minX), (y - minY)
        )
        |> List.groupBy snd
        |> List.sortBy fst
        |> List.map (fun (_, b) -> b  |> List.map fst |> Set.ofList |> generateStr)
        |> List.iter (fun str -> printfn "%s" str)

    let rec howManyCount i inp =
        match iterate inp with
        | out when (out |> List.groupBy (fun (_ ,y, _, _) -> y) |> List.length) = height -> i
        | out -> howManyCount (i + 1) out 

  let parseLine str =
        match str with
        | Regex @"position=<\s*(-?\d+),\s*(-?\d+)> velocity=<\s*(-?\d+),\s*(-?\d+)>" [p1; p2; v1; v2] -> (int p1, int p2, int v1, int v2)
        | _ -> failwith "parser error"

    let examples1() =
        let input = parseLines "position=< 9,  1> velocity=< 0,  2>\n
position=< 7,  0> velocity=<-1,  0>\n
position=< 3, -2> velocity=<-1,  1>\n
position=< 6, 10> velocity=<-2, -1>\n
position=< 2, -4> velocity=< 2,  2>\n
position=<-6, 10> velocity=< 2, -2>\n
position=< 1,  8> velocity=< 1, -1>\n
position=< 1,  7> velocity=< 1,  0>\n
position=<-3, 11> velocity=< 1, -2>\n
position=< 7,  6> velocity=<-1, -1>\n
position=<-2,  3> velocity=< 1,  0>\n
position=<-4,  3> velocity=< 2,  0>\n
position=<10, -3> velocity=<-1,  1>\n
position=< 5, 11> velocity=< 1, -2>\n
position=< 4,  7> velocity=< 0, -1>\n
position=< 8, -2> velocity=< 0,  1>\n
position=<15,  0> velocity=<-2,  0>\n
position=< 1,  6> velocity=< 1,  0>\n
position=< 8,  9> velocity=< 0, -1>\n
position=< 3,  3> velocity=<-1,  1>\n
position=< 0,  5> velocity=< 0, -1>\n
position=<-2,  2> velocity=< 2,  0>\n
position=< 5, -2> velocity=< 1,  2>\n
position=< 1,  4> velocity=< 2,  1>\n
position=<-2,  7> velocity=< 2, -2>\n
position=< 3,  6> velocity=<-1, -1>\n
position=< 5,  0> velocity=< 1,  0>\n
position=<-6,  0> velocity=< 2,  0>\n
position=< 5,  9> velocity=< 1, -2>\n
position=<14,  7> velocity=<-2,  0>\n
position=<-3,  6> velocity=< 2, -1>"

        let parsedInput = 
            input
            |> Seq.toList
            |> List.map parseLine

        let listOfHeights =
            parsedInput
            |> findSeq
            |> Seq.take 150
            |> Seq.toList
            |> List.distinct
            
        
        input
        |> Seq.toList
        |> List.map parseLine
        |> findGrouped listOfHeights
        |> List.map (fun (x,y,_,_) -> x,y) 
        |> generateMassage
        listOfHeights
        



        



    let examples2() = 
        let input = parseLines "position=< 9,  1> velocity=< 0,  2>\n
position=< 7,  0> velocity=<-1,  0>\n
position=< 3, -2> velocity=<-1,  1>\n
position=< 6, 10> velocity=<-2, -1>\n
position=< 2, -4> velocity=< 2,  2>\n
position=<-6, 10> velocity=< 2, -2>\n
position=< 1,  8> velocity=< 1, -1>\n
position=< 1,  7> velocity=< 1,  0>\n
position=<-3, 11> velocity=< 1, -2>\n
position=< 7,  6> velocity=<-1, -1>\n
position=<-2,  3> velocity=< 1,  0>\n
position=<-4,  3> velocity=< 2,  0>\n
position=<10, -3> velocity=<-1,  1>\n
position=< 5, 11> velocity=< 1, -2>\n
position=< 4,  7> velocity=< 0, -1>\n
position=< 8, -2> velocity=< 0,  1>\n
position=<15,  0> velocity=<-2,  0>\n
position=< 1,  6> velocity=< 1,  0>\n
position=< 8,  9> velocity=< 0, -1>\n
position=< 3,  3> velocity=<-1,  1>\n
position=< 0,  5> velocity=< 0, -1>\n
position=<-2,  2> velocity=< 2,  0>\n
position=< 5, -2> velocity=< 1,  2>\n
position=< 1,  4> velocity=< 2,  1>\n
position=<-2,  7> velocity=< 2, -2>\n
position=< 3,  6> velocity=<-1, -1>\n
position=< 5,  0> velocity=< 1,  0>\n
position=<-6,  0> velocity=< 2,  0>\n
position=< 5,  9> velocity=< 1, -2>\n
position=<14,  7> velocity=<-2,  0>\n
position=<-3,  6> velocity=< 2, -1>"
        let parsedInput = 
            input
            |> Seq.toList
            |> List.map parseLine

        parsedInput
        |> howManyCount 1

    let part1() = 
        let input = readLinesFromFile(@"day10.txt")
        let parsedInput = 
            input
            |> Seq.toList
            |> List.map parseLine

        let listOfHeights =
            parsedInput
            |> findSeq
            |> Seq.take 500
            |> Seq.toList
            |> List.distinct
            
        
        input
        |> Seq.toList
        |> List.map parseLine
        |> findGrouped listOfHeights
        |> List.map (fun (x,y,_,_) -> x,y) 
        |> generateMassage
        listOfHeights


    let part2() = 
        let input = readLinesFromFile(@"day10.txt")
        let parsedInput = 
            input
            |> Seq.toList
            |> List.map parseLine

        parsedInput
        |> howManyCount 1