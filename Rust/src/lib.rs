use interoptopus::{ffi_function, ffi_type, Inventory, InventoryBuilder, function};

#[ffi_type]
#[repr(C)]
pub struct Vec2 {
    pub x: f32,
    pub y: f32,
}

#[ffi_function]
#[no_mangle]
pub extern "C" fn my_function(input: Vec2) -> Vec2 {
    input
}

pub fn my_inventory() -> Inventory {
    InventoryBuilder::new()
        .register(function!(my_function))
        .inventory()
}