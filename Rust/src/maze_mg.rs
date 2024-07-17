/// A container for the catch minigame
static mut MAZE_MG: MazeMg = MazeMg::new();

#[derive(Clone, Copy, PartialEq, Debug)]
enum Tile {
    Player,
    Empty,
    Block,
}

impl Tile {
    pub const fn new() -> Self {
        Self::Empty
    }
}

#[derive(Debug, PartialEq)]
struct MazeMg {
    map: [[Tile; 8]; 8],
}

impl MazeMg {
    pub const fn new() -> Self {
        Self {
            map: [[Tile::new(); 8]; 8],
        }
    }
}
