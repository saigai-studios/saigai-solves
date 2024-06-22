use interoptopus::{extra_type, function, Inventory, InventoryBuilder};
use interoptopus::{ffi_function, ffi_type};

/// A container for all the pieces on the board
static mut BUS_MG: BusMg = BusMg::new();

#[derive(Clone)]
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
    width: usize,
    height: usize,
}

impl Grid {
    pub const fn new() -> Self {
        Self {
            inner: Vec::new(),
            width: 0,
            height: 0,
        }
    }

    pub fn load(&mut self, level: u32) {
        use Cell::*;

        self.inner = match level {
            // standard board (only really useful for square/rectangular boards)
            0 => {
                self.width = 8;
                self.height = 8;
                vec![vec![Free; self.height]; self.width]
            }
            1 => {
                self.width = 10;
                self.height = 10;
                vec![vec![Free; self.height]; self.width]
            }
            // I like this one better cause I think it'll be more readable
            // plus we are making the boards anyway
            2 => {
                self.width = 8;
                self.height = 8;
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

    pub fn get_width(&self) -> u8 {
        self.width as u8
    }

    pub fn get_height(&self) -> u8 {
        self.height as u8
    }

    /// Attempts to place a piece in the board and updates the cells if
    /// successful.
    pub fn place_piece(&mut self, root: &Coord, piece: &Piece) -> bool {
        for relative_coord in piece.get_points() {
            let real_coord = root.add(relative_coord);
            // check if this coordinate is in-bounds
            if real_coord.row >= self.get_width() || real_coord.col >= self.get_height() {
                return false;
            }
            // check if this coordinate is available
            let space = self
                .inner
                .get(real_coord.col as usize)
                .unwrap()
                .get(real_coord.row as usize)
                .unwrap();
            match space {
                Cell::Free => (),
                _ => return false,
            }
        }
        // reach this point if all good!
        for relative_coord in piece.get_points() {
            let real_coord = root.add(relative_coord);
            // check if this coordinate is available
            let space: &mut Cell = self
                .inner
                .get_mut(real_coord.col as usize)
                .unwrap()
                .get_mut(real_coord.row as usize)
                .unwrap();
            *space = Cell::Used;
        }
        true
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
        Self { row: x, col: y }
    }

    /// Add two coordinates together and create a new coordinate.
    pub fn add(&self, c: &Coord) -> Self {
        Self {
            row: self.row + c.row,
            col: self.col + c.col,
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

    pub fn get_points(&self) -> &Vec<Coord> {
        &self.points
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
pub unsafe extern "C" fn place_on_board(piece: PieceId, mouse_x: f32, mouse_y: f32) -> bool {
    match raw_mouse_pos_transform(mouse_x, mouse_y) {
        Some(root) => {
            // check if the cells are available
            let selected = BUS_MG.pieces.get(piece as usize).unwrap();
            BUS_MG.grid.place_piece(&root, selected)
        }
        None => false,
    }
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn set_grid_space(x: f32, y: f32, width: f32, height: f32) -> () {
    BUS_MG.grid_space.x = x;
    BUS_MG.grid_space.y = y;
    BUS_MG.grid_space.width = width;
    BUS_MG.grid_space.height = height;
}

unsafe fn raw_mouse_pos_transform(x: f32, y: f32) -> Option<Coord> {
    if x < BUS_MG.grid_space.x || x > BUS_MG.grid_space.x + BUS_MG.grid_space.width {
        return None;
    }
    if y < BUS_MG.grid_space.y || y > BUS_MG.grid_space.y + BUS_MG.grid_space.height {
        return None;
    }
    let scaled_x = (x - BUS_MG.grid_space.x) / BUS_MG.grid_space.width;
    let scaled_y = (y - BUS_MG.grid_space.y) / BUS_MG.grid_space.height;
    let discrete_x = (scaled_x * BUS_MG.grid.get_width() as f32) as u8;
    let discrete_y = (scaled_y * BUS_MG.grid.get_height() as f32) as u8;
    // scale the position down to discrete numbers within the domain of the grid space
    Some(Coord::new(discrete_x, discrete_y))
}
