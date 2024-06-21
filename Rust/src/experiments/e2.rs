#[derive(Default, Debug, Clone, PartialEq)]
pub struct ResponsePiece {
    pub count: i64,
    pub next: String,
    pub results: Vec<Piece>,
}
use interoptopus::ffi_function;

#[derive(Default, Debug, Clone, PartialEq)]
pub struct Piece {
    pub name: String,
    pub coordinates: Vec<u8>,
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn piece_get_name(
    piece: *const Piece,
    name: *mut *const char,
    length: *mut i32,
) {
    debug_assert!(!piece.is_null());

    let piece = &*piece;

    *name = piece.name.as_ptr() as *const char;
    *length = piece.name.len() as i32;
}

#[no_mangle]
pub unsafe extern "C" fn piece_get_height(
    piece: *const Piece,
    height: *mut *const char,
    length: *mut i32,
) {
    debug_assert!(!piece.is_null());

    let piece = &*piece;

    *height = piece.name.as_ptr() as *const char;
    *length = piece.name.len() as i32;
}

#[no_mangle]
pub unsafe extern "C" fn response_piece_get_results(
    response_piece: *const ResponsePiece,
    results: *mut *const Piece,
    length: *mut i32,
) {
    debug_assert!(!response_piece.is_null());

    let response_piece = &*response_piece;

    *results = response_piece.results.as_ptr();
    *length = response_piece.results.len() as i32;
}

#[no_mangle]
pub unsafe extern "C" fn response_piece_get_count(
    response_piece: *const ResponsePiece,
    count: *mut i32,
) {
    debug_assert!(!response_piece.is_null());

    let response_piece = &*response_piece;

    *count = response_piece.count as i32;
}
