use interoptopus::{extra_type, function, Inventory, InventoryBuilder};
use interoptopus::{ffi_function, ffi_type};

/// A container for all the pieces on the board
static mut BUS_MG: BusMg = BusMg::new();

#[derive(Clone, Copy)]
enum Cell {
    Free,
    Void,
    Used,
}

impl Cell {
    pub fn new() -> Self {
        Self::Free
    }
}

struct Grid {
    inner: Vec<Vec<Cell>>,
}

impl Grid {
    pub const fn new() -> Self {
        Self { inner: Vec::new() }
    }

    pub fn load(&mut self, level: u32) {
        use Cell::*;

        self.inner = match level {
            // standard board (only really useful for square/rectangular boards)
            0 => {
                vec![vec![Free; 8]; 8]
            }
            1 => {
                vec![vec![Free; 10]; 10]
            }
            // I like this one better cause I think it'll be more readable
            // plus we are making the boards anyway
            2 => {
                vec![
                    vec![Free, Free, Free, Void, Void, Free, Free, Free],
                    vec![Free, Free, Free, Void, Void, Free, Free, Free],
                    vec![Free, Free, Free, Void, Void, Free, Free, Free],
                    vec![Void, Void, Void, Void, Void, Void, Void, Void],
                    vec![Void, Void, Void, Void, Void, Void, Void, Void],
                    vec![Free, Free, Free, Void, Void, Free, Free, Free],
                    vec![Free, Free, Free, Void, Void, Free, Free, Free],
                    vec![Free, Free, Free, Void, Void, Free, Free, Free],
                ]
            }
            _ => {
                panic!("unsupported level")
            }
        };
    }
}

struct GridSpace {
    pub x: f32,
    pub y: f32,
    pub width: f32,
    pub height: f32,
}

impl GridSpace {
    pub const fn new() -> Self {
        Self {
            x: 0.0,
            y: 0.0,
            width: 0.0,
            height: 0.0,
        }
    }
}

struct BusMg {
    /// The collection of pieces available for the user to move.
    pieces: Vec<Piece>,
    /// A container for storing whether or not a piece occupies a section of the game board.
    grid: Grid,
    /// The literal space contained by the grid.
    grid_space: GridSpace,
}

impl BusMg {
    pub const fn new() -> Self {
        Self {
            pieces: Vec::new(),
            grid: Grid::new(),
            grid_space: GridSpace::new(),
        }
    }

    /// Resets the game contents.
    pub fn reset(&mut self) {
        self.pieces.clear();
    }
}

#[ffi_type]
#[repr(C)]
#[derive(Copy, Clone)]
pub struct Coord {
    pub row: u8,
    pub col: u8,
}

impl Coord {
    pub fn new(x: u8, y: u8) -> Self {
        Self {
            row: x,
            col: y,
        }
    }
}

type PieceId = u32;

pub struct Piece {
    points: Vec<Coord>,
    id: PieceId,
}

impl Piece {
    pub fn new(id: PieceId) -> Self {
        Self {
            points: Vec::new(),
            id: id,
        }
    }

    pub fn add_coordinate(&mut self, loc: Coord) -> () {
        self.points.push(loc);
    }

    pub fn get_id(&self) -> PieceId {
        self.id
    }

    /// Updates all coordinates according to a translation in the x and y
    /// directions.
    pub fn move_piece(&mut self, diff_x: u8, diff_y: u8) {
        self.points.iter_mut().for_each(|f| {
            f.row += diff_x;
            f.col += diff_y;
        })
    }
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn add_piece() -> PieceId {
    let id = BUS_MG.pieces.len();
    BUS_MG.pieces.push(Piece::new(id as u32));
    id as u32
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn add_coordinate(piece: PieceId, loc: Coord) {
    BUS_MG
        .pieces
        .get_mut(piece as usize)
        .unwrap()
        .add_coordinate(loc);
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn init_game(level: u32) {
    BUS_MG.reset();
    BUS_MG.grid.load(level);
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn place_on_board(piece: PieceId, mouse_x: f32, mouse_y: f32) -> Coord {
    let selected = BUS_MG.pieces.get_mut(piece as usize).unwrap();
    let root_cell = raw_mouse_pos_transform(mouse_x, mouse_y);
    root_cell
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn set_grid_space(x: f32, y: f32, width: f32, height: f32) -> () {
    BUS_MG.grid_space.x = x;
    BUS_MG.grid_space.y = y;
    BUS_MG.grid_space.width = width;
    BUS_MG.grid_space.height = height;
}

unsafe fn raw_mouse_pos_transform(x: f32, y: f32) -> Coord {
    let scaled_x = (x - BUS_MG.grid_space.x) / BUS_MG.grid_space.width;
    let scaled_y = (y - BUS_MG.grid_space.y) / BUS_MG.grid_space.height;
    let discrete_x = (scaled_x / BUS_MG.grid.inner.len() as f32) as u8;
    let discrete_y = (scaled_y / BUS_MG.grid.inner.get(0).unwrap().len() as f32) as u8;
    // scale the position down to discrete numbers within the domain of the grid space
    Coord::new(discrete_x, discrete_y)
}