(*

--- Day 16: Chronal Classification ---
As you see the Elves defend their hot chocolate successfully, you go back to falling through time. This is going to become a problem.

If you're ever going to return to your own time, you need to understand how this device on your wrist works. You have a little 
while before you reach your next destination, and with a bit of trial and error, you manage to pull up a programming manual 
on the device's tiny screen.

According to the manual, the device has four registers (numbered 0 through 3) that can be manipulated by instructions containing 
one of 16 opcodes. The registers start with the value 0.

Every instruction consists of four values: an opcode, two inputs (named A and B), and an output (named C), in that order. 
The opcode specifies the behavior of the instruction and how the inputs are interpreted. The output, C, is always 
treated as a register.

In the opcode descriptions below, if something says "value A", it means to take the number given as A literally. 
(This is also called an "immediate" value.) If something says "register A", it means to use the number given as 
A to read from (or write to) the register with that number. So, if the opcode addi adds register A and value B, 
storing the result in register C, and the instruction addi 0 7 3 is encountered, it would add 7 to the value 
contained by register 0 and store the sum in register 3, never modifying registers 0, 1, or 2 in the process.

Many opcodes are similar except for how they interpret their arguments. The opcodes fall into seven general categories:

Addition:

addr (add register) stores into register C the result of adding register A and register B.
addi (add immediate) stores into register C the result of adding register A and value B.
Multiplication:

mulr (multiply register) stores into register C the result of multiplying register A and register B.
muli (multiply immediate) stores into register C the result of multiplying register A and value B.
Bitwise AND:

banr (bitwise AND register) stores into register C the result of the bitwise AND of register A and register B.
bani (bitwise AND immediate) stores into register C the result of the bitwise AND of register A and value B.
Bitwise OR:

borr (bitwise OR register) stores into register C the result of the bitwise OR of register A and register B.
bori (bitwise OR immediate) stores into register C the result of the bitwise OR of register A and value B.
Assignment:

setr (set register) copies the contents of register A into register C. (Input B is ignored.)
seti (set immediate) stores value A into register C. (Input B is ignored.)
Greater-than testing:

gtir (greater-than immediate/register) sets register C to 1 if value A is greater than register B. Otherwise, register C is set to 0.
gtri (greater-than register/immediate) sets register C to 1 if register A is greater than value B. Otherwise, register C is set to 0.
gtrr (greater-than register/register) sets register C to 1 if register A is greater than register B. Otherwise, register C is set to 0.
Equality testing:

eqir (equal immediate/register) sets register C to 1 if value A is equal to register B. Otherwise, register C is set to 0.
eqri (equal register/immediate) sets register C to 1 if register A is equal to value B. Otherwise, register C is set to 0.
eqrr (equal register/register) sets register C to 1 if register A is equal to register B. Otherwise, register C is set to 0.
Unfortunately, while the manual gives the name of each opcode, it doesn't seem to indicate the number. However, 
you can monitor the CPU to see the contents of the registers before and after instructions are executed to try to work them out. 
Each opcode has a number from 0 through 15, but the manual doesn't say which is which. For example, suppose you capture 
the following sample:

Before: [3, 2, 1, 1]
9 2 1 2
After:  [3, 2, 2, 1]
This sample shows the effect of the instruction 9 2 1 2 on the registers. Before the instruction is executed, 
register 0 has value 3, register 1 has value 2, and registers 2 and 3 have value 1. After the instruction is executed, 
register 2's value becomes 2.

The instruction itself, 9 2 1 2, means that opcode 9 was executed with A=2, B=1, and C=2. Opcode 9 could be any of the 16 
opcodes listed above, but only three of them behave in a way that would cause the result shown in the sample:

Opcode 9 could be mulr: register 2 (which has a value of 1) times register 1 (which has a value of 2) produces 2, which 
matches the value stored in the output register, register 2.
Opcode 9 could be addi: register 2 (which has a value of 1) plus value 1 produces 2, which matches the value stored 
in the output register, register 2.
Opcode 9 could be seti: value 2 matches the value stored in the output register, register 2; the number given for B is irrelevant.
None of the other opcodes produce the result captured in the sample. Because of this, the sample above behaves like three opcodes.

You collect many of these samples (the first section of your puzzle input). The manual also includes a small test program 
(the second section of your puzzle input) - you can ignore it for now.

Ignoring the opcode numbers, how many samples in your puzzle input behave like three or more opcodes?

--- Part Two ---
Using the samples you collected, work out the number of each opcode and execute the test program 
(the second section of your puzzle input).

What value is contained in register 0 after executing the test program?

*)

module App.Day16
open Helpers

    type Registers = int * int * int * int
    type InstructionsType = {OpCode: int; Args: int * int * int}
    type InstructionHandler = int * int * int -> Registers -> Registers
    type Sample = Registers * InstructionsType * Registers

    type Instructions() as x =
        let get i ((r1,r2,r3,r4): Registers) =
            match i with
            | 0 -> r1
            | 1 -> r2
            | 2 -> r3
            | 3 -> r4
            | _ -> failwith "error invalid register"

        let set i v ((r1,r2,r3,r4): Registers) =
            match i with
            | 0 -> (v,r2,r3,r4)
            | 1 -> (r1,v,r3,r4)
            | 2 -> (r1,r2,v,r4)
            | 3 -> (r1,r2,r3,v)
            | _ -> failwith "error invalid register"

        let useFirst a _ = a
        let gt a b = if a > b then 1 else 0 
        let eq a b = if a = b then 1 else 0

        member x.Get i = get i
        
        member x.Inst f v1 v2 c = set c (f v1 v2)
        member x.Instir f (a,b,c) regs = x.Inst f a (get b regs) c regs
        member x.Instri f (a,b,c) regs = x.Inst f (get a regs) b c regs
        member x.Instrr f (a,b,c) regs = x.Inst f (get a regs) (get b regs) c regs

        member x.Addr = x.Instrr (+)
        member x.Addi = x.Instri (+)
        member x.Mulr = x.Instrr (*)
        member x.Muli = x.Instri (*)
        member x.Banr = x.Instrr (&&&)
        member x.Bani = x.Instri (&&&)
        member x.Borr = x.Instrr (|||)
        member x.Bori = x.Instri (|||)
        member x.Setr = x.Instri useFirst
        member x.Seti = x.Instir useFirst
        member x.Gtir = x.Instir gt
        member x.Gtri = x.Instri gt
        member x.Gtrr = x.Instrr gt
        member x.Eqir = x.Instir eq
        member x.Eqri = x.Instri eq
        member x.Eqrr = x.Instrr eq

    let parseInput inp =
        let parseRegisters (l: string ) =
            match l with 
            | Regex @"Before: \[(\d+), (\d+), (\d+), (\d+)\]" [r1; r2; r3; r4] -> (int r1, int r2,int r3, int r4)
            | Regex @"After:  \[(\d+), (\d+), (\d+), (\d+)\]" [r1; r2; r3; r4] -> (int r1, int r2,int r3, int r4)
            | _ -> failwith "parse error"


        let parseInstructions (l: string) =
            match l with 
            | Regex @"(\d+) (\d+) (\d+) (\d+)" [code; i1; i2; i3] -> {OpCode= int code; Args= (int i1,int i2, int i3)}
            | _ -> failwith "parse error"
        
        let parseSample  l1 l2 l3 : Sample =
            let b = parseRegisters l1
            let i = parseInstructions l2
            let a = parseRegisters l3
            b, i, a
        
        let lineList = inp |> Seq.toList

        let rec getSamples l samples = 
            match l with 
            | b :: i :: a :: _ :: nxt :: rst -> 
                let sample = parseSample b i a
                if nxt = ""
                then sample :: samples, rst
                else getSamples (nxt::rst) (sample::samples)
            | _ -> failwith "unexpected EOL"
        let samples, linesAfter = getSamples lineList []
        let instructions = List.map parseInstructions (List.tail linesAfter)
        samples, instructions

    let instructions: InstructionHandler[] =
        let x = Instructions()
        [|x.Addr; x.Addi; x.Mulr; x.Muli; x.Banr; x.Bani; x.Borr; x.Bori
          x.Setr; x.Seti; x.Gtir; x.Gtri; x.Gtrr; x.Eqir; x.Eqri; x.Eqrr |]
    
    let couldBeInstruction (b, {Args= args}, a) inst = inst args b = a
    let couldBeThreeOrMoreInstructions sample =
        let potentials = instructions |> Array.filter (couldBeInstruction sample)
        Array.length potentials >= 3

    let allCanBeInstruction samples inst =
        samples 
        |> List.exists (fun sample -> not <| couldBeInstruction sample inst)
        |> not
    
    let possibleInstructions samples =
        instructions
        |> Array.mapi (fun i v -> (i,v))
        |> Array.filter (snd >> allCanBeInstruction samples)
        |> Array.map fst
        |> Set.ofArray
    
    let getAllPosibleOpMappings =
        List.groupBy (fun (_,i,_) -> i.OpCode)
        >> List.sortBy fst 
        >> List.map (snd >> possibleInstructions)
    
    let getOpMapping samples =
        let possibleOpMappings = getAllPosibleOpMappings samples
        let rec findMapping mapping seen =
            function 
            | func :: xs ->
                let choices = Set.difference func seen |> Seq.toList
                let rec findChoice =
                    function
                    | [] -> false, []
                    | x' :: xs' ->
                        let isFound , mapping = findMapping (x'::mapping) (Set.add x' seen) xs
                        if isFound 
                        then true, mapping
                        else findChoice xs'
                findChoice choices
            | [] -> true, mapping

        findMapping [] Set.empty possibleOpMappings
        |> snd
        |> List.rev
        |> List.toArray
        |> Array.map (fun x -> instructions.[x])





    let part1() =        
        let samples, _ = readLinesFromFile(@"day16.txt") |> parseInput
        samples
        |> List.filter couldBeThreeOrMoreInstructions
        |> List.length
        



    let part2() = 
        let samples,instr = readLinesFromFile(@"day16.txt") |> parseInput
        let x = Instructions()
        let ops = getOpMapping samples
        let applyInstr reg instr =
            let i = ops.[instr.OpCode]
            i instr.Args reg
        List.fold applyInstr (0,0,0,0) instr |> x.Get 0
        
