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
open System.Collections.Generic
open System.Runtime.Serialization

    type Circle = {Before: int list; Current: int; After: int list}

    type GameArena = {
        Players: int
        Player: int
        //Index: int
        Marble: int
        Circle:  Circle
        Scores: Map<int,int>
    }


    //let counterClockwiseIndex circle index n =
    //    let rsubCircle = circle.Keys |> List.ofSeq |> List.filter ((<=) index) |> List.rev
    //    List.concat 

    let placeIndex (circle: int list) index =
        let ub = circle.Length
        if (index < ub)
        then index + 2
        else 2

    //let makeNewCircle (circle: int list) index marble =
    //    if index <= circle.Length
    //    then 
    //        let prev = circle |> List.take (index - 1)
    //        let next = circle |> List.skip (index - 1)
    //        prev @ [marble] @ next
    //    else 
    //        circle @ [marble]
    
    //let removeFromCircle circle index =
    //    let prev = circle |> List.take (index - 1)
    //    let h::next = circle |> List.skip (index - 1)
    //    prev @ next, h


    let newGame players =
        {
            Players= players
            Player= 0
            //Index= 2
            Marble= 0
            Circle= {Before=[]; Current= 0; After=[]}
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
            if marble <> nth 
            then 
                let newMarble = marble + 1
                let newCircle = game.Circle |> rotate 1 |> insertMarble newMarble
                nextAdvance {game with Circle= newCircle; Marble= newMarble; Player= (player % players) + 1}
            else game
        
        nextAdvance game

    //let advance game nth =
    //    let rec nextAdvance game =
    //        let {Players= players; Player= player; Marble= marble; Index=index; Circle= circle; Scores= score} = game
    //        if marble = nth then game
    //        else
    //            let game = {game with Marble= marble + 1; Player= (player % players) + 1}
    //            if (marble + 1) % 23 = 0 
    //            then
    //                let newIndex, newIndex' = if (game.Index - 7) <= 0 then circle.Length + (game.Index - 7), circle.Length + (game.Index - 7)   else (game.Index - 7), (game.Index - 7)
    //                let newCircle, removedValue = removeFromCircle circle newIndex
    //                let newScore = 
    //                    match score.TryFind(player) with
    //                    | Some s -> (game.Marble + removedValue) + s
    //                    | None -> (game.Marble + removedValue)
                    
    //                nextAdvance {game with Index= newIndex'; Circle= newCircle; Scores= score.Add(game.Player % game.Players, newScore )}
    //            else         
    //                let newIndex = placeIndex circle index
    //                nextAdvance {game with Index= newIndex; Circle= makeNewCircle circle newIndex game.Marble}
        
    //    nextAdvance game
        




    let examples1() =
        let input = parseLines ""
        advance (newGame 9) 4


        //nextAdvance {Players= 9; Player= 1; Index= 2; Marble= 1; Circle= [0; 1]; Scores= 0}
        //nextAdvance {Players= 9; Player= 2; Index= 2; Marble= 2; Circle= [0;2;1]; Scores= 0}
        //nextAdvance {Players= 9; Player= 3; Index= 4; Marble= 3; Circle= [0;2;1;3]; Scores= 0}
        //nextAdvance {Players= 9; Player= 4; Index= 2; Marble= 4; Circle= [0; 4; 2; 1; 3]; Scores= 0}
        //nextAdvance {Players= 9; Player= 4; Index= 22; Marble= 23; Circle= [0;16;8;17;4;18;9;19;2;20;10;21;5;22;11;1;12;6;13;3;14;7;15]; Scores= 0}
        //advance (newGame 9) 25
        //let {Scores= score} = advance (newGame 10) 1618 
        //advance (newGame 10) 1618
        //advance (newGame 13) 7999


         



    let examples2() = 
        let input = parseLines ""
        ()

    let part1() = 
        let input = readLinesFromFile(@"day7.txt")
        ()


    let part2() = 
        let input = readLinesFromFile(@"day7.txt")
        ()