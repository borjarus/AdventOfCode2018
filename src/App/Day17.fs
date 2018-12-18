(*

--- Day 17: Reservoir Research ---
You arrive in the year 18. If it weren't for the coat you got in 1018, you would be very cold: the North Pole base 
hasn't even been constructed.

Rather, it hasn't been constructed yet. The Elves are making a little progress, but there's not a lot of liquid water 
in this climate, so they're getting very dehydrated. Maybe there's more underground?

You scan a two-dimensional vertical slice of the ground nearby and discover that it is mostly sand with veins of clay. 
The scan only provides data with a granularity of square meters, but it should be good enough to determine how much water 
is trapped there. In the scan, x represents the distance to the right, and y represents the distance down. There is also 
a spring of water near the surface at x=500, y=0. The scan identifies which square meters are clay (your puzzle input).

For example, suppose your scan shows the following veins of clay:

x=495, y=2..7
y=7, x=495..501
x=501, y=3..7
x=498, y=2..4
x=506, y=1..2
x=498, y=10..13
x=504, y=10..13
y=13, x=498..504
Rendering clay as #, sand as ., and the water spring as +, and with x increasing to the right and y increasing downward, 
this becomes:

   44444455555555
   99999900000000
   45678901234567
 0 ......+.......
 1 ............#.
 2 .#..#.......#.
 3 .#..#..#......
 4 .#..#..#......
 5 .#.....#......
 6 .#.....#......
 7 .#######......
 8 ..............
 9 ..............
10 ....#.....#...
11 ....#.....#...
12 ....#.....#...
13 ....#######...
The spring of water will produce water forever. Water can move through sand, but is blocked by clay. 
Water always moves down when possible, and spreads to the left and right otherwise, filling space that has clay 
on both sides and falling out otherwise.

For example, if five squares of water are created, they will flow downward until they reach the clay and settle there. 
Water that has come to rest is shown here as ~, while sand through which water has passed (but which is now dry again) 
is shown as |:

......+.......
......|.....#.
.#..#.|.....#.
.#..#.|#......
.#..#.|#......
.#....|#......
.#~~~~~#......
.#######......
..............
..............
....#.....#...
....#.....#...
....#.....#...
....#######...
Two squares of water can't occupy the same location. If another five squares of water are created, 
they will settle on the first five, filling the clay reservoir a little more:

......+.......
......|.....#.
.#..#.|.....#.
.#..#.|#......
.#..#.|#......
.#~~~~~#......
.#~~~~~#......
.#######......
..............
..............
....#.....#...
....#.....#...
....#.....#...
....#######...
Water pressure does not apply in this scenario. If another four squares of water are created, 
they will stay on the right side of the barrier, and no water will reach the left side:

......+.......
......|.....#.
.#..#.|.....#.
.#..#~~#......
.#..#~~#......
.#~~~~~#......
.#~~~~~#......
.#######......
..............
..............
....#.....#...
....#.....#...
....#.....#...
....#######...
At this point, the top reservoir overflows. While water can reach the tiles above the surface of the water, 
it cannot settle there, and so the next five squares of water settle like this:

......+.......
......|.....#.
.#..#||||...#.
.#..#~~#|.....
.#..#~~#|.....
.#~~~~~#|.....
.#~~~~~#|.....
.#######|.....
........|.....
........|.....
....#...|.#...
....#...|.#...
....#~~~~~#...
....#######...
Note especially the leftmost |: the new squares of water can reach this tile, but cannot stop there. 
Instead, eventually, they all fall to the right and settle in the reservoir below.

After 10 more squares of water, the bottom reservoir is also full:

......+.......
......|.....#.
.#..#||||...#.
.#..#~~#|.....
.#..#~~#|.....
.#~~~~~#|.....
.#~~~~~#|.....
.#######|.....
........|.....
........|.....
....#~~~~~#...
....#~~~~~#...
....#~~~~~#...
....#######...
Finally, while there is nowhere left for the water to settle, it can reach a few more tiles before 
overflowing beyond the bottom of the scanned data:

......+.......    (line not counted: above minimum y value)
......|.....#.
.#..#||||...#.
.#..#~~#|.....
.#..#~~#|.....
.#~~~~~#|.....
.#~~~~~#|.....
.#######|.....
........|.....
...|||||||||..
...|#~~~~~#|..
...|#~~~~~#|..
...|#~~~~~#|..
...|#######|..
...|.......|..    (line not counted: below maximum y value)
...|.......|..    (line not counted: below maximum y value)
...|.......|..    (line not counted: below maximum y value)
How many tiles can be reached by the water? To prevent counting forever, ignore tiles with a y coordinate 
smaller than the smallest y coordinate in your scan data or larger than the largest one. Any x coordinate is valid. 
In this example, the lowest y coordinate given is 1, and the highest is 13, causing the water spring (in row 0) 
and the water falling off the bottom of the render (in rows 14 through infinity) to be ignored.

So, in the example above, counting both water at rest (~) and other sand tiles the water can hypothetically reach (|), 
the total number of tiles the water can reach is 57.

How many tiles can the water reach within the range of y values in your scan?

--- Part Two ---
After a very long time, the water spring will run dry. How much water will be retained?

In the example above, water that won't eventually drain out is shown as ~, a total of 29 tiles.

How many water tiles are left after the water spring stops producing water and all remaining water not at rest has drained?

*)

module App.Day17
open Helpers
    
    type Tile = Clay | Sand | StillWater | MovingWater

    let parseLine (s: string) =
        match s with
        | Regex @"([x|y])=(\d+), ([x|y])=(\d+)..(\d+)" [p1; d1; _; r1; r2] ->
            if p1.[0] = 'x'
            then (int d1, int r1),(int d1, int r2)
            else (int r1, int d1),(int r2, int d1)
        | _ -> failwith "parse error"
    

    let getYBounds =
        let updateBounds (minY, maxY) (y1, y2) = min minY y1, max maxY y2
        Seq.map (fun ((_,y1), (_,y2)) -> (y1,y2))
        >> Seq.reduce updateBounds

        
    let search x y dir grid sources =
        let rec search' x =
            let curr = Map.tryFind (x,y) grid
            let below = Map.tryFind (x, y + 1) grid
            match (curr, below) with
            | (Some Clay, _) -> x - dir, false, sources
            | (_, None) | (_, Some Sand) -> x, true, (x,y)::sources
            | (Some MovingWater, Some MovingWater) -> x, true, sources
            | _ -> search' (x + dir)
        search' x
    
    let processWaterSource maxY grid (x,y) =
        match Map.tryFind (x,y) grid with
        | Some MovingWater ->
            let rec processYPos grid sources y =
                if y > maxY 
                then grid, sources
                else 
                    match Map.tryFind (x,y) grid with
                    | Some Clay | Some StillWater ->
                        let prevY = y - 1
                        let left, leftOverflow, sources = search x prevY (-1) grid sources
                        let right, rightOverflow, sources = search x prevY 1 grid sources
                        let isOverflow = leftOverflow || rightOverflow
                        let tileType = if isOverflow then MovingWater else StillWater
                        let grid =
                            seq {
                                for x = left to right do yield ((x, prevY), tileType)
                            }
                            |> Seq.fold (fun g (k,v) -> Map.add k v g) grid 
                        processYPos grid sources prevY
                    | Some MovingWater ->
                        grid, sources
                    | Some Sand | None ->
                        processYPos (Map.add (x,y) MovingWater grid) sources (y + 1)
            processYPos grid [] (y + 1)
        | _ -> grid, []

    
    let simulate grid bounds initS =
        let rec simulate' grid sources =
            match sources with
            | [] -> grid 
            | x :: xs ->
                let grid', newS = processWaterSource bounds grid x
                let newS' = newS |> List.fold (fun xs' x' -> x' :: xs') xs
                simulate' grid' newS'
        simulate' (Map.add initS MovingWater grid) [initS]
   

    let getCounts inp =
        let minY, maxY = getYBounds inp
        let grid = seq {
            for (x1, y1), (x2, y2) in inp do
                for x = x1 to x2 do
                    for y = y1 to y2 do
                        yield (x,y), Clay } |> Map.ofSeq

        simulate grid maxY (500,0)
        |> Map.toSeq
        |> Seq.filter (fun ((_,y),_) -> minY <= y && y <= maxY)
        |> Seq.countBy snd
        |> Map.ofSeq

   

    let part1() =        
       readLinesFromFile(@"day17.txt")
       |> Seq.map parseLine
       |> (getCounts >> (fun c -> Map.find StillWater c + Map.find MovingWater c))
        



    let part2() = 
        readLinesFromFile(@"day17.txt")
       |> Seq.map parseLine
       |> (getCounts >> Map.find StillWater)
        
