use interoptopus::ffi_function;
use interoptopus::{extra_type, function, Inventory, InventoryBuilder};

mod bus_mg;
mod catch_mg;
mod maze_mg;
mod overworld;
mod save;

/// Include the ffi functions to be generated into the C# bindings file.
pub fn ffi_inventory() -> Inventory {
    InventoryBuilder::new()
        // Save state ffi exports
        .register(function!(save::data_load))
        .register(function!(save::data_save))
        .register(function!(save::data_get_language))
        .register(function!(save::data_set_language))
        .register(function!(save::data_has_earthquake_card))
        .register(function!(save::data_unlock_earthquake_card))
        // Overworld ffi exports
        .register(extra_type!(overworld::Vec2))
        .register(extra_type!(overworld::PlayerAnim))
        .register(function!(overworld::update_anim))
        .register(function!(overworld::init_marker))
        .register(function!(overworld::update_pos_key))
        .register(function!(overworld::update_pos_click))
        .register(function!(overworld::get_anim_state))
        .register(function!(overworld::set_anim_state))
        .register(function!(overworld::set_speed))
        // Bus minigame ffi exports
        .register(extra_type!(bus_mg::Coord))
        .register(function!(bus_mg::init_bus_game))
        .register(function!(bus_mg::add_piece))
        .register(function!(bus_mg::add_coordinate))
        .register(function!(bus_mg::place_on_board))
        .register(function!(bus_mg::place_off_board))
        .register(function!(bus_mg::set_window))
        .register(function!(bus_mg::get_snap_pos))
        .register(function!(bus_mg::is_bus_game_won))
        // Catch minigame ffi exports
        .register(function!(catch_mg::init_catch_game))
        .register(function!(catch_mg::is_next_spawn_ready))
        .register(function!(catch_mg::is_catch_game_won))
        .register(function!(catch_mg::good_catch))
        .register(function!(catch_mg::missed_catch))
        .register(function!(catch_mg::bad_catch))
        // Maze minigame ffi exports
        // Finalize the inventory
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
