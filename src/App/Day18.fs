(*

--- Day 18: Settlers of The North Pole ---
On the outskirts of the North Pole base construction project, many Elves are collecting lumber.

The lumber collection area is 50 acres by 50 acres; each acre can be either open ground (.), trees (|), 
or a lumberyard (#). You take a scan of the area (your puzzle input).

Strange magic is at work here: each minute, the landscape looks entirely different. In exactly one minute, 
an open acre can fill with trees, a wooded acre can be converted to a lumberyard, or a lumberyard can be cleared 
to open ground (the lumber having been sent to other projects).

The change to each acre is based entirely on the contents of that acre as well as the number of open, wooded, 
or lumberyard acres adjacent to it at the start of each minute. Here, "adjacent" means any of the eight acres 
surrounding that acre. (Acres on the edges of the lumber collection area might have fewer than eight adjacent acres; 
the missing acres aren't counted.)

In particular:

An open acre will become filled with trees if three or more adjacent acres contained trees. Otherwise, nothing happens.
An acre filled with trees will become a lumberyard if three or more adjacent acres were lumberyards. Otherwise, nothing happens.
An acre containing a lumberyard will remain a lumberyard if it was adjacent to at least one other lumberyard and 
at least one acre containing trees. Otherwise, it becomes open.
These changes happen across all acres simultaneously, each of them using the state of all acres at the beginning 
of the minute and changing to their new form by the end of that same minute. Changes that happen during the minute 
don't affect each other.

For example, suppose the lumber collection area is instead only 10 by 10 acres with this initial configuration:

Initial state:
.#.#...|#.
.....#|##|
.|..|...#.
..|#.....#
#.#|||#|#|
...#.||...
.|....|...
||...#|.#|
|.||||..|.
...#.|..|.

After 1 minute:
.......##.
......|###
.|..|...#.
..|#||...#
..##||.|#|
...#||||..
||...|||..
|||||.||.|
||||||||||
....||..|.

After 2 minutes:
.......#..
......|#..
.|.|||....
..##|||..#
..###|||#|
...#|||||.
|||||||||.
||||||||||
||||||||||
.|||||||||

After 3 minutes:
.......#..
....|||#..
.|.||||...
..###|||.#
...##|||#|
.||##|||||
||||||||||
||||||||||
||||||||||
||||||||||

After 4 minutes:
.....|.#..
...||||#..
.|.#||||..
..###||||#
...###||#|
|||##|||||
||||||||||
||||||||||
||||||||||
||||||||||

After 5 minutes:
....|||#..
...||||#..
.|.##||||.
..####|||#
.|.###||#|
|||###||||
||||||||||
||||||||||
||||||||||
||||||||||

After 6 minutes:
...||||#..
...||||#..
.|.###|||.
..#.##|||#
|||#.##|#|
|||###||||
||||#|||||
||||||||||
||||||||||
||||||||||

After 7 minutes:
...||||#..
..||#|##..
.|.####||.
||#..##||#
||##.##|#|
|||####|||
|||###||||
||||||||||
||||||||||
||||||||||

After 8 minutes:
..||||##..
..|#####..
|||#####|.
||#...##|#
||##..###|
||##.###||
|||####|||
||||#|||||
||||||||||
||||||||||

After 9 minutes:
..||###...
.||#####..
||##...##.
||#....###
|##....##|
||##..###|
||######||
|||###||||
||||||||||
||||||||||

After 10 minutes:
.||##.....
||###.....
||##......
|##.....##
|##.....##
|##....##|
||##.####|
||#####|||
||||#|||||
||||||||||
After 10 minutes, there are 37 wooded acres and 31 lumberyards. Multiplying the number of wooded acres by the number 
of lumberyards gives the total resource value after ten minutes: 37 * 31 = 1147.

What will the total resource value of the lumber collection area be after 10 minutes?

--- Part Two ---
This important natural resource will need to last for at least thousands of years. 
Are the Elves collecting this lumber sustainably?

What will the total resource value of the lumber collection area be after 1000000000 minutes?

*)

module App.Day18
open Helpers
  
    type Acre = Lumberyard | Tree | Open
    type CountOfNeightbour = {Lumberyard: int; Tree: int; Open: int}
        with 
            static member Zero = {Lumberyard= 0; Tree= 0; Open= 0}
            static member AddNeightbour counts = function
                | Lumberyard -> {counts with Lumberyard= counts.Lumberyard + 1}
                | Tree -> {counts with Tree= counts.Tree + 1}
                | Open -> {counts with Open= counts.Open + 1}
            static member GetNextCellState value {Tree= t; Lumberyard= l} = 
                match value with
                | Open -> if t >= 3 then Tree else Open
                | Tree -> if l >= 3 then Lumberyard else Tree
                | Lumberyard -> if l = 0 || t = 0 then Open else Lumberyard
   
    let neightbours x y =
        [|(x - 1, y - 1); (x, y - 1); (x + 1, y - 1); (x - 1, y);
          (x + 1, y); (x - 1, y + 1); (x, y + 1); (x + 1, y + 1)|]
     


    let step grid =
        let width, height = (Array2D.length1 grid, Array2D.length2 grid)
        let inBounds (x,y) = 0 <= x && x < width && 0 <= y && y < height
        let getNextState x y c =
            neightbours x y
            |> Array.filter inBounds
            |> Array.map (fun (x,y) -> grid.[x,y])
            |> Array.fold CountOfNeightbour.AddNeightbour CountOfNeightbour.Zero
            |> CountOfNeightbour.GetNextCellState c
        Array2D.mapi getNextState grid

    


    
    let parseInput inp = 
        let toAcre =
            function 
            | '#' -> Lumberyard
            | '|' -> Tree
            | '.' -> Open
            | _ -> failwith "parse error"
        inp
        |> (array2D >> Array2D.map toAcre)
    
    let score (grid: Acre[,]) =
        let counts = grid |> Seq.cast<Acre> |> Seq.fold CountOfNeightbour.AddNeightbour CountOfNeightbour.Zero
        counts.Lumberyard * counts.Tree
    
    let stepN n grid = Seq.init n id |> Seq.fold (fun x _ -> step x) grid 

    let generateANswer inp =
        let rec stepRec x grid cache =
            match Map.tryFind grid cache with
            | Some n -> 
                let cycleLen = x - n
                let howManySteps = (1000000000 - n) % cycleLen
                grid |> stepN howManySteps |> score
            | None -> stepRec (x + 1) (step grid) (Map.add grid x cache)
        stepRec 0 inp Map.empty


    let part1() =        
       readLinesFromFile(@"day18.txt") 
       |> parseInput
       |> (stepN 10 >> score)
       



    let part2() = 
            readLinesFromFile(@"day18.txt")
            |> parseInput
            |> generateANswer

        

