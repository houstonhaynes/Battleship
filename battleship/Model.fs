module Model

let (boardx,boardy) = 10,10
let shipList = [5;4;4;4;3;3;3;3;2;2]
let random = new System.Random ()

type GameModel = {
    state: GameState
    player: PlayerState
    ai: PlayerState
} and GameState = 
    | Title | Placement of (Dir * (int list)) 
    | PlayerTurn | AITurn of float * bool | GameOver of bool
  and PlayerState = {
    ships: (string * Tile list) list
    shots: Tile list
} and 
    Tile = { x: int; y: int }
  and Dir = | North = 0 | East = 1 | South = 2 | West = 3

let nextIn tile direction = 
    match direction with
    | Dir.North -> { tile with y = tile.y - 1 }
    | Dir.East -> { tile with x = tile.x + 1 }
    | Dir.South -> { tile with y = tile.y + 1 }
    | _ -> { tile with x = tile.x - 1 }

let rec tilesIn tile direction length =
    let next = nextIn tile direction
    if length = 1 || next.x < 0 || next.x = boardx || next.y < 0 || next.y = boardy then
        [tile]
    else
        tile::(tilesIn next direction (length - 1))

let canPlace tiles board = 
    let boardTiles = List.collect snd board
    boardTiles |> List.exists (fun o -> List.contains o tiles) |> not

let nameForLength = 
    function
    | 2 -> "Submarine"
    | 3 -> "Destroyer"
    | 4 -> "Battleship"
    | _ -> "Carrier"

let randomPlacement () = 
    let rec findMatch length board =
        let tile = { 
            x = random.Next (0, boardx); 
            y = random.Next (0, boardy) } 
        let dir = enum<Dir>(random.Next(0,4))
        let tiles = tilesIn tile dir length
        if canPlace tiles board && tiles.Length = length then 
            (nameForLength length, tiles)
        else 
            findMatch length board
    List.fold (fun board shipLength -> 
        board @ [findMatch shipLength board]) [] shipList