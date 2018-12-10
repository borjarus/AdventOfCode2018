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


    type GameArena = {
        Players: int
        Player: int
        Index: int
        Marble: int
        Circle:  int list
        Scores: int
    }

    //let counterClockwiseIndex circle index n =
    //    let rsubCircle = circle.Keys |> List.ofSeq |> List.filter ((<=) index) |> List.rev
    //    List.concat 

    let placeIndex (circle: int list) index =
        let ub = circle.Length
        if (index < ub)
        then index + 2
        else 2

    let makeNewCircle (circle: int list) index marble =
        if index <= circle.Length
        then 
            let prev = circle |> List.take (index - 1)
            let next = circle |> List.skip (index - 1)
            prev @ [marble] @ next
        else 
            circle @ [marble]


    let newGame players =
        {
            Players= players
            Player= 1
            Index= 1
            Marble= 1
            Circle= [1; 0]
            Scores= 0
        }

    let advanceAction game =
        let {Players= players; Player= player; Marble= marble; Index=index; Circle= circle; Scores= scores} = game
        let game = {game with Marble= marble + 1; Player= (player % players) + 1}
        let newIndex = placeIndex circle index
        {game with Index= newIndex; Circle= makeNewCircle circle newIndex game.Marble}

    //    //if marble % 23 = 0 
    //    //then 
    //    //    let removeindex = 
    //    //    {game with 
    //    //        index=  
    //        }



  //  (defn advance [game]
  //(let [{:keys [players player index marble circle scores]} game
  //      marble (inc marble)
  //      player (inc (mod player players))
  //      game   (assoc game :marble marble :player player)]
  //  (if (zero? (mod marble 23))
  //    (let [remove-index (counter-clockwise-index circle index 7)
  //          scores       (update scores player + marble (circle remove-index))
  //          circle       (dissoc circle remove-index)
  //          index        (ffirst (or (subseq circle > remove-index) circle))]
  //      (assoc game :index index :circle circle :scores scores))
  //    (let [index  (place-index circle index)
  //          circle (assoc circle index marble)]
  //      (assoc game :index index :circle circle)))))




    let examples1() =
        let input = parseLines ""

        advanceAction {Players= 9; Player= 1; Index= 2; Marble= 1; Circle= [0; 1]; Scores= 0}
        advanceAction {Players= 9; Player= 2; Index= 2; Marble= 2; Circle= [0;2;1]; Scores= 0}
        advanceAction {Players= 9; Player= 3; Index= 4; Marble= 3; Circle= [0;2;1;3]; Scores= 0}
        advanceAction {Players= 9; Player= 4; Index= 2; Marble= 4; Circle= [0; 4; 2; 1; 3]; Scores= 0}
        advanceAction {Players= 9; Player= 4; Index= 22; Marble= 22; Circle= [0;16;8;17;4;18;9;19;2;20;10;21;5;22;11;1;12;6;13;3;14;7;15]; Scores= 0}


         



    let examples2() = 
        let input = parseLines ""
        ()

    let part1() = 
        let input = readLinesFromFile(@"day7.txt")
        ()


    let part2() = 
        let input = readLinesFromFile(@"day7.txt")
        ()