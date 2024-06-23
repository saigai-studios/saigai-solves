use interoptopus::{ffi_function, ffi_type};

use crate::overworld::Vec2;

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn add_piece() -> PieceId {
    BUS_MG.add_piece()
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn add_coordinate(piece: PieceId, loc: Coord) {
    BUS_MG.add_coordinate(piece, loc);
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn get_snap_pos(piece: PieceId) -> Vec2 {
    BUS_MG.get_snap_pos(piece)
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn init_game(level: u32) {
    BUS_MG.initialize(level);
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn place_on_board(piece: PieceId, mouse_x: f32, mouse_y: f32) -> bool {
    BUS_MG.place_on_board(piece, mouse_x, mouse_y)
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn place_off_board(piece: PieceId, mouse_x: f32, mouse_y: f32) -> bool {
    BUS_MG.place_off_board(piece, mouse_x, mouse_y)
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn set_grid_space(x: f32, y: f32, width: f32, height: f32) -> () {
    BUS_MG.grid_space = GridSpace::set(x, y, width, height);
}

/// A container for all the pieces on the board
static mut BUS_MG: BusMg = BusMg::new();

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
    fn reset(&mut self) {
        self.pieces.clear();
    }

    /// Transforms screen position to cell coordinate.
    fn raw_mouse_pos_transform(&self, x: f32, y: f32) -> Option<Coord> {
        if x < self.grid_space.x || x > self.grid_space.x + self.grid_space.width {
            return None;
        }
        if y < self.grid_space.y || y > self.grid_space.y + self.grid_space.height {
            return None;
        }
        let scaled_x = (x - self.grid_space.x) / self.grid_space.width;
        let scaled_y = (y - self.grid_space.y) / self.grid_space.height;
        let discrete_x = (scaled_x * self.grid.get_width() as f32) as u8;
        let discrete_y = (scaled_y * self.grid.get_height() as f32) as u8;
        // scale the position down to discrete numbers within the domain of the grid space
        //  println!("{:?}", Coord::new(discrete_x, discrete_y));
        Some(Coord::new(discrete_x, discrete_y))
    }

    /// Transforms cell coordinate to screen position.
    fn raw_cell_pos_transform(&self, row: u8, col: u8) -> Vec2 {
        // get a value from [0, 1)
        let cont_x = row as f32 / self.grid.get_width() as f32;
        let cont_y = col as f32 / self.grid.get_height() as f32;

        // scale it according to the grid window space
        let screen_x = (cont_x * self.grid_space.width) + self.grid_space.x;
        let screen_y = (cont_y * self.grid_space.height) + self.grid_space.y;
        Vec2::with(screen_x, screen_y)
    }
}

/// These functions are exposed to FFI through the static variable calling them in a wrapped FFI function.
impl BusMg {
    pub fn initialize(&mut self, level: u32) {
        self.reset();
        self.grid.load(level);
    }

    pub fn add_piece(&mut self) -> PieceId {
        let id = self.pieces.len();
        self.pieces.push(Piece::new(id as u32));
        id as u32
    }

    pub fn add_coordinate(&mut self, piece: PieceId, loc: Coord) {
        self.pieces
            .get_mut(piece as usize)
            .unwrap()
            .add_coordinate(loc);
    }

    pub fn place_on_board(&mut self, piece: PieceId, mouse_x: f32, mouse_y: f32) -> bool {
        match self.raw_mouse_pos_transform(mouse_x, mouse_y) {
            Some(root) => {
                // check if the cells are available
                let selected = self.pieces.get_mut(piece as usize).unwrap();
                self.grid.place_piece(&root, selected)
            }
            None => false,
        }
    }

    pub fn place_off_board(&mut self, piece: PieceId, mouse_x: f32, mouse_y: f32) -> bool {
        match self.raw_mouse_pos_transform(mouse_x, mouse_y) {
            // we don't want to place the piece anywhere on the board
            Some(_) => false,
            // we selected a location off the board
            None => {
                let selected = self.pieces.get_mut(piece as usize).unwrap();
                let _ = self.grid.remove_piece(selected);
                true
            }
        }
    }

    /// Transforms the coordinates of the root cell of this piece into an x, y screen
    /// position according to the grid space window.
    ///
    /// Although this function should never be called for a piece that is not placed
    /// on the gird, if the piece is not on the grid it returns (0.0, 0.0).
    pub fn get_snap_pos(&self, piece: PieceId) -> Vec2 {
        let selected = self.pieces.get(piece as usize).unwrap();
        match selected.get_occupied_root_cell() {
            Some(root) => self.raw_cell_pos_transform(root.row, root.col),
            None => Vec2::new(),
        }
    }
}

#[derive(Clone, Debug, PartialEq)]
enum Cell {
    Free,
    Void,
    Used(PieceId),
}

#[ffi_type]
#[repr(C)]
#[derive(Copy, Clone, Debug)]
pub struct Coord {
    pub row: u8,
    pub col: u8,
}

impl Coord {
    pub fn new(x: u8, y: u8) -> Self {
        Self { row: x, col: y }
    }

    /// Add two coordinates together and create a new coordinate.
    pub fn translate(&self, c: &Coord) -> Self {
        Self {
            row: self.row + c.row,
            col: self.col + c.col,
        }
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

    pub fn set(x: f32, y: f32, width: f32, height: f32) -> Self {
        Self {
            x: x,
            y: y,
            width: width,
            height: height,
        }
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

    #[cfg(test)]
    fn get_cell_status(&self, row: usize, col: usize) -> &Cell {
        self.inner.get(col).unwrap().get(row).unwrap()
    }

    pub fn get_width(&self) -> u8 {
        self.width as u8
    }

    pub fn get_height(&self) -> u8 {
        self.height as u8
    }

    /// Attempts to place a piece in the board and updates the cells if
    /// successful.
    pub fn place_piece(&mut self, root: &Coord, piece: &mut Piece) -> bool {
        // check that each real coordinate can map to a valid cell location (check bounds)
        for relative_cell in piece.get_points() {
            let real_cell = root.translate(relative_cell);
            // check if this coordinate is in-bounds
            println!("{:?}", real_cell);
            if real_cell.row >= self.get_width() || real_cell.col >= self.get_height() {
                return false;
            }

            // check if this coordinate is available
            let space = self
                .inner
                .get(real_cell.col as usize)
                .unwrap()
                .get(real_cell.row as usize)
                .unwrap();
            match space {
                // allow placement on open cells
                Cell::Free => (),
                // allow placement to be placed back on a tile that already is occupied
                // by this piece because it is currently "lifted"
                Cell::Used(id) => {
                    if id != &piece.get_id() {
                        return false;
                    }
                }
                _ => return false,
            }
        }

        // remove previous cell positions
        let _ = self.remove_piece(piece);

        // fill each cell this piece occupies within the grid
        for relative_cell in piece.get_points() {
            let real_cell = root.translate(relative_cell);
            // check if this coordinate is available
            let space: &mut Cell = self
                .inner
                .get_mut(real_cell.col as usize)
                .unwrap()
                .get_mut(real_cell.row as usize)
                .unwrap();
            *space = Cell::Used(piece.get_id());
        }
        // remember which root cell this piece was placed at
        piece.prev_cell = Some(root.clone());
        true
    }

    /// Removes a piece from the grid. This frees the cells it once previously
    /// occupied.
    ///
    /// Returns false if the piece was not existing in the grid.
    fn remove_piece(&mut self, piece: &mut Piece) -> bool {
        match piece.prev_cell {
            Some(root) => {
                for relative_cell in piece.get_points() {
                    let real_cell = root.translate(relative_cell);
                    // update the state of this cell
                    let space: &mut Cell = self
                        .inner
                        .get_mut(real_cell.col as usize)
                        .unwrap()
                        .get_mut(real_cell.row as usize)
                        .unwrap();
                    *space = Cell::Free;
                }
                // lose memory of which root cell this piece was placed it (is now removed)
                piece.prev_cell = None;
                true
            }
            None => false,
        }
    }
}

type PieceId = u32;

pub struct Piece {
    points: Vec<Coord>,
    /// Track the last root coordinate where the piece is on the board, if it exists.
    prev_cell: Option<Coord>,
    id: PieceId,
}

impl Piece {
    pub fn new(id: PieceId) -> Self {
        Self {
            points: Vec::new(),
            prev_cell: None,
            id: id,
        }
    }

    pub fn get_occupied_root_cell(&self) -> Option<&Coord> {
        self.prev_cell.as_ref()
    }

    pub fn get_id(&self) -> PieceId {
        self.id
    }

    pub fn add_coordinate(&mut self, loc: Coord) -> () {
        self.points.push(loc);
    }

    pub fn get_points(&self) -> &Vec<Coord> {
        &self.points
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    fn make_basic_game(x: f32, y: f32) -> BusMg {
        let mut game = BusMg::new();
        game.grid_space = GridSpace::set(x, y, 80.0, 80.0);
        game.initialize(0);

        let id0 = game.add_piece();
        game.add_coordinate(id0, Coord::new(0, 0));

        let id1 = game.add_piece();
        game.add_coordinate(id1, Coord::new(0, 0));
        game.add_coordinate(id1, Coord::new(1, 0));

        let id2 = game.add_piece();
        game.add_coordinate(id2, Coord::new(0, 0));
        game.add_coordinate(id2, Coord::new(0, 1));
        game
    }

    #[test]
    fn ut_place_on_board_bad() {
        let mut game = make_basic_game(10.0, 20.0);
        assert_eq!(game.place_on_board(0, 5.0, 30.0), false);
        assert_eq!(game.place_on_board(0, 30.0, 15.0), false);
        assert_eq!(game.place_on_board(0, 95.0, 40.0), false);
        assert_eq!(game.place_on_board(0, 40.0, 105.0), false);
    }

    #[test]
    fn ut_place_on_board_ok() {
        let mut game = make_basic_game(10.0, 20.0);
        assert_eq!(game.place_on_board(0, 20.0, 30.0), true);

        let mut game = make_basic_game(10.0, 20.0);
        assert_eq!(game.place_on_board(0, 30.0, 25.0), true);

        let mut game = make_basic_game(10.0, 20.0);
        assert_eq!(game.place_on_board(0, 89.0, 40.0), true);

        let mut game = make_basic_game(10.0, 20.0);
        assert_eq!(game.place_on_board(0, 40.0, 99.0), true);
        // try to add to position that is already occupied by a different piece
        assert_eq!(game.place_on_board(1, 40.0, 99.0), false);
    }

    #[test]
    fn it_grid_updating() {
        let mut game = BusMg::new();
        game.grid_space = GridSpace::set(10.0, 20.0, 80.0, 80.0);
        game.initialize(0);

        assert_eq!(game.grid_space.x, 10.0);
        assert_eq!(game.grid_space.y, 20.0);
        assert_eq!(game.grid_space.width, 80.0);
        assert_eq!(game.grid_space.height, 80.0);

        assert_eq!(game.grid.get_width(), 8);
        assert_eq!(game.grid.get_height(), 8);

        let id0 = game.add_piece();
        game.add_coordinate(id0, Coord::new(0, 0));
        game.add_coordinate(id0, Coord::new(1, 0));

        assert_eq!(game.pieces.len(), 1);

        // add this piece (valid x, valid y, but secondary cell is too far right)
        assert_eq!(game.place_on_board(id0, 85.0, 90.0), false);

        // add this piece (valid x, invalid y)
        assert_eq!(game.place_on_board(id0, 20.0, 122.0), false);

        println!("Adding to board now...");
        // add this piece (valid x, invalid y)
        assert_eq!(game.place_on_board(id0, 21.0, 41.0), true);

        assert_eq!(game.grid.get_cell_status(1, 2), &Cell::Used(0));
        assert_eq!(game.grid.get_cell_status(2, 2), &Cell::Used(0));
        assert_eq!(game.grid.get_cell_status(3, 2), &Cell::Free);

        // add this piece (valid- already occupied by itself)
        assert_eq!(game.place_on_board(id0, 20.0, 40.0), true);
    }
}
