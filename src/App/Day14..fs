(*

--- Day 14: Chocolate Charts ---
You finally have a chance to look at all of the produce moving around. Chocolate, cinnamon, mint, 
chili peppers, nutmeg, vanilla... the Elves must be growing these plants to make hot chocolate! 
As you realize this, you hear a conversation in the distance. When you go to investigate, you discover 
two Elves in what appears to be a makeshift underground kitchen/laboratory.

The Elves are trying to come up with the ultimate hot chocolate recipe; they're even maintaining 
a scoreboard which tracks the quality score (0-9) of each recipe.

Only two recipes are on the board: the first recipe got a score of 3, the second, 7. Each of the two Elves has 
a current recipe: the first Elf starts with the first recipe, and the second Elf starts with the second recipe.

To create new recipes, the two Elves combine their current recipes. This creates new recipes from the digits 
of the sum of the current recipes' scores. With the current recipes' scores of 3 and 7, their sum is 10, 
and so two new recipes would be created: the first with score 1 and the second with score 0. If the current 
recipes' scores were 2 and 3, the sum, 5, would only create one recipe (with a score of 5) with its single digit.

The new recipes are added to the end of the scoreboard in the order they are created. So, after the first round, 
the scoreboard is 3, 7, 1, 0.

After all new recipes are added to the scoreboard, each Elf picks a new current recipe. To do this, 
the Elf steps forward through the scoreboard a number of recipes equal to 1 plus the score of their current recipe. 
So, after the first round, the first Elf moves forward 1 + 3 = 4 times, while the second 
Elf moves forward 1 + 7 = 8 times. If they run out of recipes, they loop back around to the beginning. 
After the first round, both Elves happen to loop around until they land on the same recipe that 
they had in the beginning; in general, they will move to different recipes.

Drawing the first Elf as parentheses and the second Elf as square brackets, they continue this process:

(3)[7]
(3)[7] 1  0 
 3  7  1 [0](1) 0 
 3  7  1  0 [1] 0 (1)
(3) 7  1  0  1  0 [1] 2 
 3  7  1  0 (1) 0  1  2 [4]
 3  7  1 [0] 1  0 (1) 2  4  5 
 3  7  1  0 [1] 0  1  2 (4) 5  1 
 3 (7) 1  0  1  0 [1] 2  4  5  1  5 
 3  7  1  0  1  0  1  2 [4](5) 1  5  8 
 3 (7) 1  0  1  0  1  2  4  5  1  5  8 [9]
 3  7  1  0  1  0  1 [2] 4 (5) 1  5  8  9  1  6 
 3  7  1  0  1  0  1  2  4  5 [1] 5  8  9  1 (6) 7 
 3  7  1  0 (1) 0  1  2  4  5  1  5 [8] 9  1  6  7  7 
 3  7 [1] 0  1  0 (1) 2  4  5  1  5  8  9  1  6  7  7  9 
 3  7  1  0 [1] 0  1  2 (4) 5  1  5  8  9  1  6  7  7  9  2 
The Elves think their skill will improve after making a few recipes (your puzzle input). 
However, that could take ages; you can speed this up considerably by identifying the scores of the 
ten recipes after that. For example:

If the Elves think their skill will improve after making 9 recipes, the scores of the ten recipes
after the first nine on the scoreboard would be 5158916779 (highlighted in the last line of the diagram).

After 5 recipes, the scores of the next ten would be 0124515891.
After 18 recipes, the scores of the next ten would be 9251071085.
After 2018 recipes, the scores of the next ten would be 5941429882.
What are the scores of the ten recipes immediately after the number of recipes in your puzzle input?

--- Part Two ---

As it turns out, you got the Elves' plan backwards. They actually want to know how many recipes appear on the scoreboard to the 
left of the first recipes whose scores are the digits from your puzzle input.

    51589 first appears after 9 recipes.
    01245 first appears after 5 recipes.
    92510 first appears after 18 recipes.
    59414 first appears after 2018 recipes.

How many recipes appear on the scoreboard to the left of the score sequence in your puzzle input?


*)

module App.Day14
open Helpers
open System

    type Buffer = int * int * int * int * int * int


    type Scores = {t: Buffer; score: Buffer; bufCount: int; matched: bool}
        with 
            static member Push (_, i2, i3, i4, i5, i6) inp = (i2, i3, i4, i5, i6, inp)
            static member Empty = (0,0,0,0,0,0)

    type Elves = {e1: int; e2: int; score: int[]; scoreAdded: int list; numOfScores: int}
        with 
            static member GetElfIndex elf elves = if elf = 1 then elves.e1 else elves.e2
            static member SetElfIndex elf elves index = 
                if elf = 1
                then {elves with e1 = index}
                else {elves with e2 = index}
            static member GetScore elf elves = elves.score.[Elves.GetElfIndex elf elves]

    

    let getNewScores elves =
        let total = (Elves.GetScore 1 elves) + (Elves.GetScore 2 elves)
        let scoreModList = [total % 10]
        if total >= 10 
        then (total / 10) :: scoreModList
        else scoreModList
    
    let updateScores elves = 
        let nScores = getNewScores elves
        let aScores = List.fold (fun acc el -> el :: acc) elves.scoreAdded nScores
        let nsCount = elves.numOfScores + List.length nScores
        {elves with scoreAdded= aScores; numOfScores= nsCount}

    let addQueuedScores elves =
        let nScores = elves.scoreAdded |> List.toArray |> Array.rev
        {elves with score= Array.append elves.score nScores; scoreAdded= []}
        
    let rec move elf elves =
        let lScores = Array.length elves.score
        let score = Elves.GetElfIndex elf elves + Elves.GetScore elf elves + 1
        if score < lScores || elves.scoreAdded = [] 
        then (score % lScores) |> Elves.SetElfIndex elf elves 
        else elves |> addQueuedScores |> move elf
    
    let  step = updateScores >> move 1 >> move 2

    let startRecord = {e1= 0; e2= 1; score= [| 3; 7 |]; scoreAdded= []; numOfScores= 2 }



    let addScore buf inp =
        if buf.matched 
        then buf
        else 
            let nBuff = Scores.Push buf.score inp
            let isTargetedBuff = buf.t = nBuff
            {buf with score= nBuff; bufCount= buf.bufCount + 1; matched= isTargetedBuff}
    
    let startBuffer t scores =
        List.fold addScore {t= t; score= Scores.Empty; bufCount= (-6); matched= false} scores

    let targetStrToBuffer =
        Seq.map (fun c -> int c - int '0')
        >> Seq.fold Scores.Push Scores.Empty
         


    let part1() =        
        let input = int "765071"
        let rec generate elves =
            if elves.numOfScores = input + 10
            then 
                let appened = addQueuedScores elves
                let ch = 
                    Array.sub appened.score input 10
                    |> Array.map (fun c -> char (c + int '0'))
                String ch
            else elves |> step |> generate
    
        generate startRecord


    let part2() = 
        let input = "765071"
        let rec generate elves buf =
            let nScoreBuff = getNewScores elves |> List.fold addScore buf
            if nScoreBuff.matched 
            then nScoreBuff.bufCount
            else generate (step elves) nScoreBuff
        let target = targetStrToBuffer input
        let buf = startBuffer target [3; 7]
        generate startRecord buf
            