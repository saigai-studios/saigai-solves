static mut PIECES: Vec<u8> = Vec::new();

#[no_mangle]
pub unsafe extern "C" fn get_bytes(len: *mut i32, capacity: *mut i32) -> *mut u8 {
    let mut buf: Vec<u8> = PIECES.clone();

    *len = buf.len() as i32;
    *capacity = buf.capacity() as i32;

    let bytes = buf.as_mut_ptr();
    // forget about the memory while C# works on it
    std::mem::forget(buf);
    return bytes;
}

#[no_mangle]
pub unsafe extern "C" fn free_bytes(data: *mut u8, len: i32, capacity: i32) {
    // re-assemble the memory to we can use it in rust
    let v = Vec::from_raw_parts(data, len as usize, capacity as usize);
    drop(v); // or it could be implicitly dropped
}
