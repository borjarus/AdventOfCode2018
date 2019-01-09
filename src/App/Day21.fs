(*

--- Day 21: Chronal Conversion ---
You should have been watching where you were going, because as you wander the new North Pole base, you trip and 
fall into a very deep hole!

Just kidding. You're falling through time again.

If you keep up your current pace, you should have resolved all of the temporal anomalies by the next time the device activates. 
Since you have very little interest in browsing history in 500-year increments for the rest of your life, you need 
to find a way to get back to your present time.

After a little research, you discover two important facts about the behavior of the device:

First, you discover that the device is hard-wired to always send you back in time in 500-year increments. 
Changing this is probably not feasible.

Second, you discover the activation system (your puzzle input) for the time travel module. Currently, 
it appears to run forever without halting.

If you can cause the activation system to halt at a specific moment, maybe you can make the device send you so 
far back in time that you cause an integer underflow in time itself and wrap around back to your current time!

The device executes the program as specified in manual section one and manual section two.

Your goal is to figure out how the program works and cause it to halt. You can only control register 0; 
every other register begins at 0 as usual.

Because time travel is a dangerous activity, the activation system begins with a few instructions which 
verify that bitwise AND (via bani) does a numeric operation and not an operation as if the inputs were interpreted as strings. 
If the test fails, it enters an infinite loop re-running the test instead of allowing the program to execute normally. 
If the test passes, the program continues, and assumes that all other bitwise operations (banr, bori, and borr) 
also interpret their inputs as numbers. (Clearly, the Elves who wrote this system were worried that someone might 
introduce a bug while trying to emulate this system with a scripting language.)

What is the lowest non-negative integer value for register 0 that causes the program to halt after executing the 
fewest instructions? (Executing the same instruction multiple times counts as multiple instructions executed.)

--- Part Two ---
In order to determine the timing window for your underflow exploit, you also need an upper bound:

What is the lowest non-negative integer value for register 0 that causes the program to halt after executing the most instructions? 
(The program must actually halt; running forever does not count as halting.)

*)

module App.Day21
open Helpers
     
     type Instruction = string * int * int * int

     let parseIpReg inp =
        match inp with 
        | Regex @"(\d+)" [d] -> int d
        | _ -> failwith "parse error"

     let parseLine inp = 
        match inp with 
        | Regex @"([a-z]+) (\d+) (\d+) (\d+)" [o; r1; r2; r3] -> o, int r1, int r2, int r3
        | _ -> failwith "parse error"

     let parseInput inp =

        let ipReg = inp |> Seq.head |> parseIpReg
        let instr = inp |> Seq.tail |> Seq.map parseLine |> Seq.toArray
        instr, ipReg


     let run (oper : Instruction[]) registers ipReg check =
        let toInt = (function true -> 1 | false -> 0)
        let len = oper.Length - 1
        let rec tick (reg :int[]) ip =
            reg.[ipReg] <- ip 

            match check reg with 
            | Some resp -> resp 
            | _ ->
                let instr, A, B, C = oper.[ip % len]
                match instr with
                | "addr" -> reg.[C] <- (reg.[A] + reg.[B]);
                | "addi" -> reg.[C] <- (reg.[A] + B);
                | "mulr" -> reg.[C] <- (reg.[A] * reg.[B]);
                | "muli" -> reg.[C] <- (reg.[A] * B);
                | "banr" -> reg.[C] <- (reg.[A] &&& reg.[B]);
                | "bani" -> reg.[C] <- (reg.[A] &&& B);
                | "borr" -> reg.[C] <- (reg.[A] ||| reg.[B]);
                | "bori" -> reg.[C] <- (reg.[A] ||| B);
                | "setr" -> reg.[C] <- (reg.[A]);
                | "seti" -> reg.[C] <- A;
                | "gtir" -> reg.[C] <- ((A > reg.[B]) |> toInt);
                | "gtri" -> reg.[C] <- ((reg.[A] > B) |> toInt);
                | "gtrr" -> reg.[C] <- ((reg.[A] > reg.[B]) |> toInt);
                | "eqir" -> reg.[C] <- ((A = reg.[B]) |> toInt);
                | "eqri" -> reg.[C] <- ((reg.[A] = B) |> toInt);
                | "eqrr" -> reg.[C] <- ((reg.[A] = reg.[B]) |> toInt);
                tick reg (reg.[ipReg] + 1)
        tick registers 0

    let run2 (oper : Instruction[]) registers ipReg (seen: Set<int>) =
        let toInt = (function true -> 1 | false -> 0)
        let len = oper.Length
        let rec tick (reg :int[]) ip prev =
            let next = 
                reg.[ipReg] <- ip 
                let instr, A, B, C = oper.[ip % len]
                match instr with
                | "addr" -> reg.[C] <- (reg.[A] + reg.[B]);
                | "addi" -> reg.[C] <- (reg.[A] + B);
                | "mulr" -> reg.[C] <- (reg.[A] * reg.[B]);
                | "muli" -> reg.[C] <- (reg.[A] * B);
                | "banr" -> reg.[C] <- (reg.[A] &&& reg.[B]);
                | "bani" -> reg.[C] <- (reg.[A] &&& B);
                | "borr" -> reg.[C] <- (reg.[A] ||| reg.[B]);
                | "bori" -> reg.[C] <- (reg.[A] ||| B);
                | "setr" -> reg.[C] <- (reg.[A]);
                | "seti" -> reg.[C] <- A;
                | "gtir" -> reg.[C] <- ((A > reg.[B]) |> toInt);
                | "gtri" -> reg.[C] <- ((reg.[A] > B) |> toInt);
                | "gtrr" -> reg.[C] <- ((reg.[A] > reg.[B]) |> toInt);
                | "eqir" -> reg.[C] <- ((A = reg.[B]) |> toInt);
                | "eqri" -> reg.[C] <- ((reg.[A] = B) |> toInt);
                | "eqrr" -> reg.[C] <- ((reg.[A] = reg.[B]) |> toInt);
                

            let v = reg.[2]

            if reg.[ipReg] <> 28 
            then next; tick reg (reg.[ipReg] + 1) prev
            elif seen.Contains v |> not
            then seen.Add v |> ignore; next; tick reg (reg.[ipReg] + 1) v
            else prev


        tick registers 0 0

 
    let part1() =        
        let (oper, ipReg) = readLinesFromFile(@"day21.txt") |> parseInput
        let reg = (Array.zeroCreate 6)
        
        let check (reg: int[]) =
            if reg.[ipReg] = 28 
            then Some reg.[2]
            else None

        run oper reg ipReg check

        


    let part2() = 
        let (oper, ipReg) = readLinesFromFile(@"day21.txt") |> parseInput
        let reg = (Array.zeroCreate 6)
            
        run2 oper reg ipReg Set.empty
