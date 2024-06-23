use interoptopus::ffi_function;
use interoptopus::{extra_type, function, Inventory, InventoryBuilder};

mod bus_mg;
mod overworld;

/// Include the ffi functions to be generated into the C# bindings file.
pub fn ffi_inventory() -> Inventory {
    InventoryBuilder::new()
        .register(function!(add_two_nums))
        // Overworld ffi exports
        .register(extra_type!(overworld::Vec2))
        .register(function!(overworld::update_anim))
        .register(function!(overworld::init_marker))
        .register(function!(overworld::update_pos_key))
        .register(function!(overworld::update_pos_click))
        // Bus minigame ffi exports
        .register(extra_type!(bus_mg::Coord))
        .register(function!(bus_mg::init_game))
        .register(function!(bus_mg::add_piece))
        .register(function!(bus_mg::add_coordinate))
        .register(function!(bus_mg::place_on_board))
        .register(function!(bus_mg::place_off_board))
        .register(function!(bus_mg::set_window))
        .register(function!(bus_mg::get_snap_pos))
        .register(function!(bus_mg::is_game_won))
        .inventory()
}

#[ffi_function]
#[no_mangle]
pub extern "C" fn add_two_nums(x: i32, y: i32) -> i32 {
    x + y
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn ut_add_two_nums() {
        assert_eq!(5, add_two_nums(3, 2));
    }
}
