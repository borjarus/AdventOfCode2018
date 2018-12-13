(*

--- Day 13: Mine Cart Madness ---

A crop of this size requires significant logistics to transport produce, soil, fertilizer, and so on. The Elves are very busy pushing things around in carts on some kind of rudimentary system of tracks they've come up with.

Seeing as how cart-and-track systems don't appear in recorded history for another 1000 years, the Elves seem to be making this up as they go along. They haven't even figured out how to avoid collisions yet.

You map out the tracks (your puzzle input) and see where you can help.

Tracks consist of straight paths (| and -), curves (/ and \), and intersections (+). Curves connect exactly two perpendicular pieces of track; for example, this is a closed loop:

/----\
|    |
|    |
\----/

Intersections occur when two perpendicular paths cross. At an intersection, a cart is capable of turning left, turning right, or continuing straight. 
Here are two loops connected by two intersections:

/-----\
|     |
|  /--+--\
|  |  |  |
\--+--/  |
   |     |
   \-----/

Several carts are also on the tracks. Carts always face either up (^), down (v), left (<), or right (>). (On your initial map, the track 
under each cart is a straight path matching the direction the cart is facing.)

Each time a cart has the option to turn (by arriving at any intersection), it turns left the first time, goes straight the second time, 
turns right the third time, and then repeats those directions starting again with left the fourth time, straight the fifth time, and so on. 
This process is independent of the particular intersection at which the cart has arrived - that is, the cart has no per-intersection memory.

Carts all move at the same speed; they take turns moving a single step at a time. They do this based on their current location: 
carts on the top row move first (acting from left to right), then carts on the second row move (again from left to right), then carts 
on the third row, and so on. Once each cart has moved one step, the process repeats; each of these loops is called a tick.

For example, suppose there are two carts on a straight track:

|  |  |  |  |
v  |  |  |  |
|  v  v  |  |
|  |  |  v  X
|  |  ^  ^  |
^  ^  |  |  |
|  |  |  |  |

First, the top cart moves. It is facing down (v), so it moves down one square. Second, the bottom cart moves. It is facing up (^), 
so it moves up one square. Because all carts have moved, the first tick ends. Then, the process repeats, starting with the first cart.
The first cart moves down, then the second cart moves up - right into the first cart, colliding with it! (The location of the crash is marked with an X.)
This ends the second and last tick.

Here is a longer example:

/->-\        
|   |  /----\
| /-+--+-\  |
| | |  | v  |
\-+-/  \-+--/
  \------/   

/-->\        
|   |  /----\
| /-+--+-\  |
| | |  | |  |
\-+-/  \->--/
  \------/   

/---v        
|   |  /----\
| /-+--+-\  |
| | |  | |  |
\-+-/  \-+>-/
  \------/   

/---\        
|   v  /----\
| /-+--+-\  |
| | |  | |  |
\-+-/  \-+->/
  \------/   

/---\        
|   |  /----\
| /->--+-\  |
| | |  | |  |
\-+-/  \-+--^
  \------/   

/---\        
|   |  /----\
| /-+>-+-\  |
| | |  | |  ^
\-+-/  \-+--/
  \------/   

/---\        
|   |  /----\
| /-+->+-\  ^
| | |  | |  |
\-+-/  \-+--/
  \------/   

/---\        
|   |  /----<
| /-+-->-\  |
| | |  | |  |
\-+-/  \-+--/
  \------/   

/---\        
|   |  /---<\
| /-+--+>\  |
| | |  | |  |
\-+-/  \-+--/
  \------/   

/---\        
|   |  /--<-\
| /-+--+-v  |
| | |  | |  |
\-+-/  \-+--/
  \------/   

/---\        
|   |  /-<--\
| /-+--+-\  |
| | |  | v  |
\-+-/  \-+--/
  \------/   

/---\        
|   |  /<---\
| /-+--+-\  |
| | |  | |  |
\-+-/  \-<--/
  \------/   

/---\        
|   |  v----\
| /-+--+-\  |
| | |  | |  |
\-+-/  \<+--/
  \------/   

/---\        
|   |  /----\
| /-+--v-\  |
| | |  | |  |
\-+-/  ^-+--/
  \------/   

/---\        
|   |  /----\
| /-+--+-\  |
| | |  X |  |
\-+-/  \-+--/
  \------/   

After following their respective paths for a while, the carts eventually crash. To help prevent crashes, you'd like to know 
the location of the first crash. Locations are given in X,Y coordinates, where the furthest left column is X=0 and the furthest top row is Y=0:

           111
 0123456789012
0/---\        
1|   |  /----\
2| /-+--+-\  |
3| | |  X |  |
4\-+-/  \-+--/
5  \------/   

In this example, the location of the first crash is 7,3.


*)

module App.Day13
open Helpers

    type Direction = Left | Right | Straight
    type Cart = { x:int; y: int; xDir: int; yDir: int; nextTurn: Direction}
        with 
            static member GetPosition cart = cart.x, cart.y
            static member New x y xDir yDir = {x= x; y= y; xDir= xDir; yDir= yDir; nextTurn= Left}
        

    type TrackTypes = Vertical | Horizontal | Intersection | ForwardCurve | BackCurve | NoTrack

    type TrackLogs = {
        cartLocations: Map<int * int, int>
        carts: Map<int, Cart>
        collisions: (int * int) list
    }


    let extractCarts tracks = 
        let grid = tracks |> array2D

        let carts = seq {
            for y = 0 to (grid.GetLength 0) - 1 do 
                for x = 0 to (grid.GetLength 1) - 1 do
                    match grid.[x,y] with
                    | '^' -> yield Cart.New x y 0 -1
                    | 'v' -> yield Cart.New x y 0 1
                    | '<' -> yield Cart.New x y -1 0
                    | '>' -> yield Cart.New x y 1 0
                    | _ -> ()
        }

        let cartToTrackType =
            function 
            | '^' | 'v' | '|' -> Vertical
            | '<' | '>' | '-' -> Horizontal
            | '+' -> Intersection
            | '/' -> ForwardCurve
            | '\\' -> BackCurve
            | ' ' -> NoTrack
            | _ -> failwith "invalid track type"
        
        let extractedGrid = grid |> Array2D.map cartToTrackType
        extractedGrid, carts

    let tickCart (grid: TrackTypes[,]) cart =
        let newX, newY = cart.x + cart.xDir, cart.y + cart.yDir
        let cartInNewPosition = {cart with x= newX; y= newY}
        match grid.[newX,newY] with
        | Vertical | Horizontal -> cartInNewPosition
        | ForwardCurve -> {cartInNewPosition with xDir=(-cart.yDir); yDir=(-cart.xDir)}
        | BackCurve -> {cartInNewPosition with xDir=cart.yDir; yDir=cart.xDir}
        | Intersection ->
            match cart.nextTurn with
            | Left -> {cartInNewPosition with xDir=cart.yDir; yDir=(-cart.xDir); nextTurn=Straight}
            | Straight -> {cartInNewPosition with nextTurn=Right}
            | Right -> {cartInNewPosition with xDir=(-cart.yDir); yDir=cart.xDir; nextTurn=Left}
        | NoTrack -> failwithf "Cart fell off the track at %i, %i" newX newY

    let handleCollision trackLogs oldCart newCart cartID =
        let newPos = Cart.GetPosition newCart
        let collidedID = Map.find newPos trackLogs.cartLocations

        let updatedLocations = 
            trackLogs.cartLocations
            |> Map.remove (Cart.GetPosition oldCart)
            |> Map.remove newPos
        
        let updatedCarts =
            trackLogs.carts
            |> Map.remove collidedID
            |> Map.remove cartID
        
        let updatedCollisions = newPos::trackLogs.collisions
        {cartLocations= updatedLocations; carts= updatedCarts; collisions= updatedCollisions}
    
    let handleNoCollision  trackLogs oldCart newCart cartID =
        let updatedLocations =
            trackLogs.cartLocations
            |> Map.remove (Cart.GetPosition oldCart)
            |> Map.add (Cart.GetPosition newCart) cartID
        let updatedCarts = trackLogs.carts |> Map.add cartID newCart
        {cartLocations= updatedLocations; carts= updatedCarts; collisions= trackLogs.collisions}
    






    let examples1() =
        let input = parseLines ""
        ()

    let examples2() =
        let input = parseLines ""
        ()


    let part1() =        
        let input = readLinesFromFile(@"day13.txt")
        ()


    let part2() = 
        let input = readLinesFromFile(@"day13.txt")
        ()