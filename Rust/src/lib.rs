use interoptopus::{extra_type, function, Inventory, InventoryBuilder};
use interoptopus::{ffi_function, ffi_type};

// pub mod experiments;
mod bus_mg;

/// Include the ffi functions to be generated into the C# bindings file.
pub fn ffi_inventory() -> Inventory {
    InventoryBuilder::new()
        .register(extra_type!(bus_mg::Coord))
        .register(extra_type!(Vec2))
        .register(function!(add_two_nums))
        .register(function!(update_anim))
        .register(function!(init_marker))
        .register(function!(update_pos_key))
        .register(function!(update_pos_click))
        .register(function!(bus_mg::add_piece))
        .register(function!(bus_mg::add_coordinate))
        .inventory()
}

// Anim constants
const SPEED: i32 = 100;

// Player variables
static mut MARKER_POS: [Vec2; 4] = [Vec2::new(); 4];
static mut PLR: Player = Player::new();

#[ffi_type]
#[repr(C)]
#[derive(Copy, Clone)]
pub struct Vec2 {
    pub x: f32,
    pub y: f32,
}

impl Vec2 {
    const fn new() -> Self {
        Self { x: 0.0, y: 0.0 }
    }
}

// #[ffi_type]
// #[repr(C)]
// #[derive(Copy, Clone)]
// pub struct Coord {
//     pub row: i32,
//     pub col: i32,
// }

// impl Coord {
//     const fn new() -> Self {
//         Self { row: 0, col: 0 }
//     }
// }

#[ffi_type]
#[repr(C)]
pub struct Player {
    pub curr: Vec2,
    pub old: Vec2,
    pub dest: Vec2,
    pub curr_mark: i32,
    pub anim_count: i32,
}

impl Player {
    const fn new() -> Self {
        Self {
            curr: Vec2::new(),
            old: Vec2::new(),
            dest: Vec2::new(),
            curr_mark: 0,
            anim_count: SPEED * 2,
        }
    }
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn init_marker(ind: i32, pos: Vec2) {
    MARKER_POS[ind as usize] = pos;
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn update_pos_key(opt: bool) {
    // Decrement and wrap
    if opt == true {
        // Left
        PLR.curr_mark -= 1;

        if PLR.curr_mark < 0 {
            PLR.curr_mark = 2;
        }
    } else {
        // Right
        PLR.curr_mark += 1;

        if PLR.curr_mark > 2 {
            PLR.curr_mark = 0;
        }
    }

    // Reset animation counter
    PLR.anim_count = 0;

    // Set starting position
    PLR.old = PLR.curr;

    // Set ending position
    PLR.dest = MARKER_POS[PLR.curr_mark as usize];
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn update_pos_click(marker: i32) -> bool {
    // If marker matches the current, no update is needed
    if marker == PLR.curr_mark {
        // Check if player has reached marker
        if PLR.anim_count <= SPEED * 2 {
            return false; // Do not update level yet
        } else {
            return true; // Update the level
        }
    }

    // Update the player's current marker
    PLR.curr_mark = marker;

    // If intersection has been passed
    if PLR.anim_count > SPEED {
        // Reset animation counter
        PLR.anim_count = 0;

        // Set starting position
        PLR.old = PLR.curr;
    }

    // Set ending position - this is always changed
    PLR.dest = MARKER_POS[PLR.curr_mark as usize];

    return false;
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn update_anim() -> Vec2 {
    if PLR.anim_count <= SPEED {
        //PLR.curr = move_lerp_rust(PLR.anim_count, PLR.old, PLR.dest);
        PLR.curr = move_lerp_rust(PLR.anim_count, PLR.old, MARKER_POS[3]);
        PLR.anim_count += 1;
    } else if PLR.anim_count <= SPEED * 2 {
        //PLR.curr = move_lerp_rust(PLR.anim_count, PLR.old, PLR.dest);
        PLR.curr = move_lerp_rust(PLR.anim_count - SPEED, MARKER_POS[3], PLR.dest);
        PLR.anim_count += 1;
    } else {
        PLR.curr = MARKER_POS[PLR.curr_mark as usize];
    }

    return PLR.curr;
}
/*
// minigame stuff
#[ffi_type]
#[repr(C)]
enum GameState {
    WIN,
    LOSS,
    PAUSE,
    PLAYING
}

#[derive(Clone)]
enum Cell {
    FREE(),
    USED,
    VOID
}

pub struct Game {
    pub gs: GameState,
}

impl Game {
    fn new() -> Self {
        Self {
            gs: GameState::PLAYING,
        }
    }
}
struct BusGame {
    pub game: Game,
    pub grid: Vec<Vec<Cell>>,
}

impl BusGame {
    fn new(ver: u32) -> Self {
        Self {
            game: Game::new(),
            grid: BusGame::make_grid(ver),
        }
    }

    fn make_grid(num: u32) -> Vec<Vec<Cell>> {
        let mut grid: Vec<Vec<Cell>> = match num {
            // standard board (only really useful for square/rectangular boards)
            0 => {
                vec![vec![Cell::FREE; 10]; 10]
            },
            // I like this one better cause I think it'll be more readable
            // plus we are making the boards anyway
            1 => {
                vec![
                    vec![Cell::FREE, Cell::FREE, Cell::FREE, Cell::VOID, Cell::VOID, Cell::FREE, Cell::FREE, Cell::FREE],
                    vec![Cell::FREE, Cell::FREE, Cell::FREE, Cell::VOID, Cell::VOID, Cell::FREE, Cell::FREE, Cell::FREE],
                    vec![Cell::FREE, Cell::FREE, Cell::FREE, Cell::VOID, Cell::VOID, Cell::FREE, Cell::FREE, Cell::FREE],
                    vec![Cell::VOID, Cell::VOID, Cell::VOID, Cell::VOID, Cell::VOID, Cell::VOID, Cell::VOID, Cell::VOID],
                    vec![Cell::VOID, Cell::VOID, Cell::VOID, Cell::VOID, Cell::VOID, Cell::VOID, Cell::VOID, Cell::VOID],
                    vec![Cell::FREE, Cell::FREE, Cell::FREE, Cell::VOID, Cell::VOID, Cell::FREE, Cell::FREE, Cell::FREE],
                    vec![Cell::FREE, Cell::FREE, Cell::FREE, Cell::VOID, Cell::VOID, Cell::FREE, Cell::FREE, Cell::FREE],
                    vec![Cell::FREE, Cell::FREE, Cell::FREE, Cell::VOID, Cell::VOID, Cell::FREE, Cell::FREE, Cell::FREE],
                ]
            },
            _ => {
                vec![vec![Cell::FREE; 8]; 8]
            },
        };
        grid
    }
}
#[ffi_function]
#[no_mangle]
// scrap later
pub unsafe extern "C" fn piece_fits() -> bool {
    // check if a piece fits in the
    true
}
*/

#[ffi_function]
#[no_mangle]
pub extern "C" fn add_two_nums(x: i32, y: i32) -> i32 {
    let result = x + y;
    // println!("X + Y = {}", result);
    result
}

fn move_lerp_rust(curr_time: i32, src: Vec2, dest: Vec2) -> Vec2 {
    Vec2 {
        x: f_lerp(src.x, dest.x, (curr_time as f32) / (SPEED as f32)),
        y: f_lerp(src.y, dest.y, (curr_time as f32) / (SPEED as f32)),
    }
}

fn f_lerp(src: f32, dest: f32, scale: f32) -> f32 {
    return src + ((dest - src) * scale);
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn ut_add_two_nums() {
        assert_eq!(5, add_two_nums(3, 2));
    }
}
