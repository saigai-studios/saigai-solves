// https://github.com/testdouble/rust-ffi-complex-example

use interoptopus::{ffi_type, ffi_function};

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
            height: 0.0
        }
    }
}

static mut GRID_SPACE: GridSpace = GridSpace::new();

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn set_grid_space(x: f32, y: f32, width: f32, height: f32) -> () {
    GRID_SPACE.x = x;
    GRID_SPACE.y = y;
    GRID_SPACE.width = width;
    GRID_SPACE.height = height;
}


#[ffi_type]
#[repr(C)]
#[derive(Copy, Clone)]
pub struct Coordinate {
    x: u8,
    y: u8,
}

impl Coordinate {
    pub fn new(x: u8, y: u8) -> Self {
        Self {
            x: x,
            y: y,
        }
    }
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn raw_mouse_pos_transform(x: f32, y: f32) -> Coordinate {
    let scaled_x = (x - GRID_SPACE.x) / GRID_SPACE.width;
    let scaled_y = (y - GRID_SPACE.y) / GRID_SPACE.height;
    let discrete_x = (scaled_x / 8 as f32) as u8;
    let discrete_y = (scaled_y / 8 as f32) as u8;
    // scale the position down to discrete numbers within the domain of the grid space
    Coordinate::new(discrete_x, discrete_y)
}

/// A container for storing whether or not a piece occupies a section of the game board.
static mut BOARD: [[bool; 8]; 8] = [[false; 8]; 8];

/// A container for all the pieces on the board
static mut PIECES: Vec<Piece> = Vec::new();

// idea: could send pointer of object and then update it like so...
// or pass in an ID
type PieceId = u32;
pub struct Piece {
    points: Vec<(u8, u8)>,
    id: PieceId,
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn add_piece() -> PieceId {
    let id = PIECES.len();
    PIECES.push(Piece::new(id as u32));
    id as u32
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn add_coordinate(piece: PieceId, x: u8, y: u8) {
    PIECES.get_mut(piece as usize).unwrap().add_coordinate(x, y);
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn move_piece(piece: PieceId, diff_x: u8, diff_y: u8) {
    PIECES
        .get_mut(piece as usize)
        .unwrap()
        .move_piece(diff_x, diff_y);
}

impl Piece {
    pub fn new(id: PieceId) -> Self {
        Self {
            points: Vec::new(),
            id: id,
        }
    }

    pub fn add_coordinate(&mut self, x: u8, y: u8) -> () {
        self.points.push((x, y));
    }

    pub fn get_id(&self) -> PieceId {
        self.id
    }

    /// Updates all coordinates according to a translation in the x and y
    /// directions.
    pub fn move_piece(&mut self, diff_x: u8, diff_y: u8) {
        self.points.iter_mut().for_each(|f| {
            f.0 += diff_x;
            f.1 += diff_y;
        })
    }
}
