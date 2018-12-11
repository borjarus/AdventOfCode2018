(*

--- Day 9: Marble Mania ---
You talk to the Elves while you wait for your navigation system to initialize. To pass the time, 
they introduce you to their favorite marble game.

The Elves play this game by taking turns arranging the marbles in a circle according to very particular rules. 
The marbles are numbered starting with 0 and increasing by 1 until every marble has a number.

First, the marble numbered 0 is placed in the circle. At this point, while it contains only a single marble, 
it is still a circle: the marble is both clockwise from itself and counter-clockwise from itself. 
This marble is designated the current marble.

Then, each Elf takes a turn placing the lowest-numbered remaining marble into the circle between 
the marbles that are 1 and 2 marbles clockwise of the current marble. (When the circle is large enough, 
this means that there is one marble between the marble that was just placed and the current marble.) 
The marble that was just placed then becomes the current marble.

However, if the marble that is about to be placed has a number which is a multiple of 23, something entirely 
different happens. First, the current player keeps the marble they would have placed, adding it to their score. 
In addition, the marble 7 marbles counter-clockwise from the current marble is removed from the circle 
and also added to the current player's score. The marble located immediately clockwise of the marble 
that was removed becomes the new current marble.

For example, suppose there are 9 players. After the marble with value 0 is placed in the middle, 
each player (shown in square brackets) takes a turn. The result of each of those turns would produce 
circles of marbles like this, where clockwise is to the right and the resulting current marble is in parentheses:

[-] (0)
[1]  0 (1)
[2]  0 (2) 1 
[3]  0  2  1 (3)
[4]  0 (4) 2  1  3 
[5]  0  4  2 (5) 1  3 
[6]  0  4  2  5  1 (6) 3 
[7]  0  4  2  5  1  6  3 (7)
[8]  0 (8) 4  2  5  1  6  3  7 
[9]  0  8  4 (9) 2  5  1  6  3  7 
[1]  0  8  4  9  2(10) 5  1  6  3  7 
[2]  0  8  4  9  2 10  5(11) 1  6  3  7 
[3]  0  8  4  9  2 10  5 11  1(12) 6  3  7 
[4]  0  8  4  9  2 10  5 11  1 12  6(13) 3  7 
[5]  0  8  4  9  2 10  5 11  1 12  6 13  3(14) 7 
[6]  0  8  4  9  2 10  5 11  1 12  6 13  3 14  7(15)
[7]  0(16) 8  4  9  2 10  5 11  1 12  6 13  3 14  7 15 
[8]  0 16  8(17) 4  9  2 10  5 11  1 12  6 13  3 14  7 15 
[9]  0 16  8 17  4(18) 9  2 10  5 11  1 12  6 13  3 14  7 15 
[1]  0 16  8 17  4 18  9(19) 2 10  5 11  1 12  6 13  3 14  7 15 
[2]  0 16  8 17  4 18  9 19  2(20)10  5 11  1 12  6 13  3 14  7 15 
[3]  0 16  8 17  4 18  9 19  2 20 10(21) 5 11  1 12  6 13  3 14  7 15 
[4]  0 16  8 17  4 18  9 19  2 20 10 21  5(22)11  1 12  6 13  3 14  7 15 
[5]  0 16  8 17  4 18(19) 2 20 10 21  5 22 11  1 12  6 13  3 14  7 15 
[6]  0 16  8 17  4 18 19  2(24)20 10 21  5 22 11  1 12  6 13  3 14  7 15 
[7]  0 16  8 17  4 18 19  2 24 20(25)10 21  5 22 11  1 12  6 13  3 14  7 15

The goal is to be the player with the highest score after the last marble is used up. 
Assuming the example above ends after the marble numbered 25, the winning score is 23+9=32 
(because player 5 kept marble 23 and removed marble 9, while no other player got any points 
in this very short example game).

Here are a few more examples:

10 players; last marble is worth 1618 points: high score is 8317
13 players; last marble is worth 7999 points: high score is 146373
17 players; last marble is worth 1104 points: high score is 2764
21 players; last marble is worth 6111 points: high score is 54718
30 players; last marble is worth 5807 points: high score is 37305
What is the winning Elf's score?

*)

module App.Day9
open Helpers

    type Circle = {Before: int64 list; Current: int64; After: int64 list}

    type GameArena = {
        Players: int64
        Player: int64
        Marble: int64
        Circle:  Circle
        Scores: Map<int64,int64>
    }

    let newGame players =
        {
            Players= players
            Player= 1L
            //Index= 2
            Marble= 1L
            Circle= {Before=[]; Current= 0L; After=[]}
            Scores= Map.empty
        }
    
    let rotateCounterClockwise {Before=before; Current=curr; After=after} =
        match before with
        | [] ->
            let x::xs = List.rev (curr::after)
            {Before=xs; Current= x; After=[]}
        | x :: xs -> {Before= xs; Current=x; After=curr::after}
    
    let rotateClockwise {Before=before; Current=curr; After=after} =
        match after with
        | [] ->
            let x::xs = List.rev (curr::before)
            {Before=[]; Current= x; After= xs}
        | x::xs -> {Before= curr::before; Current= x; After= xs}
    
    let rec rotate n circle =
        match n with
        | 0 -> circle
        | _ when n > 0 -> circle |> rotateClockwise |> rotate (n - 1)
        | _ -> circle |> rotateCounterClockwise |> rotate (n + 1)
    
    let insertMarble marble circle = {circle with Before= circle.Current::circle.Before; Current= marble} 

    let removeCurrent {Before=before; After=after} =
        match after with 
        | [] -> {Before= List.tail before; Current= List.head before; After=after} |> rotate 1
        | x::xs -> {Before=before; Current= x; After= xs}

    let advance game nth =
        let rec nextAdvance game =
            let {Players= players; Player= player; Marble= marble;  Circle= circle; Scores= score} = game
            if marble = nth  then game
            elif marble % 23L = 0L 
            then
                let getScore = getOrDefault player score 0L
                let rotated = circle |> rotate (-7)
                let newScore = Map.add player (getScore + rotated.Current + marble) score
                let newCircle = rotated |> removeCurrent
                nextAdvance {game with Circle= newCircle; Marble= marble + 1L; Player= (player % players) + 1L; Scores= newScore}       
            else 
                let newCircle = game.Circle |> rotate 1 |> insertMarble marble
                nextAdvance {game with Circle= newCircle; Marble= marble + 1L; Player= (player % players) + 1L}       
        nextAdvance game

        
    let parseLine str =
        match str with
        | Regex @"(\d+) players; last marble is worth (\d+) points" [players; marblePoints] -> (players, marblePoints)
        | _ -> failwith "parser error"



    let examples1() =
        let input1 = "10 players; last marble is worth 1618 points: high score is 8317\n"
        let input2 = "13 players; last marble is worth 7999 points: high score is 146373\n"
        let input3 = "17 players; last marble is worth 1104 points: high score is 2764\n"
        let input4 = "21 players; last marble is worth 6111 points: high score is 54718\n"
        let input5 = "30 players; last marble is worth 5807 points: high score is 37305\n"
        let input = parseLines <| String.concat "" [input1; input2; input3; input4; input5]

        input
        |> Seq.map parseLine
        |> Seq.toList
        |> List.map (fun (players, maxPoints) ->
            let {Scores= score} = advance (newGame (int64 players)) (int64 maxPoints)
            score |> Map.toSeq |> Seq.map snd |> Seq.max
        )


    let examples2() = 
        let input = parseLines ""
        ()

    let part1() = 
        let input = readLinesFromFile(@"day9.txt")
        input
        |> Seq.map parseLine
        |> Seq.toList
        |> List.map (fun (players, maxPoints) ->
            let {Scores= score} = advance (newGame (int64 players)) (int64 maxPoints)
            score |> Map.toSeq |> Seq.map snd |> Seq.max
        )


    let part2() = 
        let input = readLinesFromFile(@"day9.txt")
        input
        |> Seq.map parseLine
        |> Seq.toList
        |> List.map (fun (players, maxPoints) ->
            let newMax = 100L * (int64 maxPoints)
            let {Scores= score} = advance (newGame (int64 players)) newMax
            score |> Map.toSeq |> Seq.map snd |> Seq.max
        )