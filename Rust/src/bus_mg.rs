use interoptopus::{extra_type, function, Inventory, InventoryBuilder};
use interoptopus::{ffi_function, ffi_type};

/// A container for all the pieces on the board
static mut PIECES: Vec<Piece> = Vec::new();

#[ffi_type]
#[repr(C)]
#[derive(Copy, Clone)]
pub struct Coord {
    pub row: u8,
    pub col: u8,
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
    let id = PIECES.len();
    PIECES.push(Piece::new(id as u32));
    id as u32
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn add_coordinate(piece: PieceId, loc: Coord) {
    PIECES.get_mut(piece as usize).unwrap().add_coordinate(loc);
}
