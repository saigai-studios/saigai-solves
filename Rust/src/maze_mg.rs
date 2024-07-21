use crate::overworld::Vec2;

/// A container for the catch minigame
static mut _MAZE_MG: MazeMg = MazeMg::_new();

#[derive(Clone, Copy, PartialEq, Debug)]
enum Tile {
    _Player,
    _Empty,
    _Block,
    _Goal,
}

impl Tile {
    pub const fn _new() -> Self {
        Self::_Empty
    }
}

#[derive(Debug, PartialEq)]
struct MazeMg {
    player: Vec2,
    goal: Vec2,
}

impl MazeMg {
    pub const fn _new() -> Self {
        Self {
            player: Vec2::new(),
            goal: Vec2::new(),
        }
    }
}
