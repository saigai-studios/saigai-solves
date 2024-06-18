use interoptopus::{ffi_function, ffi_type, function, Inventory, InventoryBuilder};

#[ffi_type]
#[repr(C)]
#[derive(Copy,Clone)]
pub struct Vec2 {
    pub x: f32,
    pub y: f32,
}

const fn new() -> Vec2{
    Vec2 { x: 0.0, y: 0.0 }
}

#[ffi_type]
#[repr(C)]
pub struct Player {
    pub curr: Vec2,
    pub old: Vec2,
    pub dest: Vec2,
    pub curr_mark: i32,
    pub anim_count: i32,
}

// Anim constants
const speed: i32 = 100;

// Player variables
static mut marker_pos: [Vec2; 3] = [new(); 3];
static mut plr: Player = 
Player{
    curr: new(),
    old: new(),
    dest: new(),
    curr_mark: 0,
    anim_count: speed,
};

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn init_marker(ind: i32, pos: Vec2){
    marker_pos[ind as usize] = pos;
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn update_pos(opt: bool) {
    // Decrement and wrap
    if opt{ // Left
        plr.curr_mark -= 1;

        if plr.curr_mark < 0{
            plr.curr_mark = 2;
        }
    }
    else{ // Right
        plr.curr_mark += 1;

        if plr.curr_mark > 2{
            plr.curr_mark = 0;
        }
    }

    // Reset animation counter
    plr.anim_count = 0;

    // Set starting position
    plr.old = plr.curr;

    // Set ending position
    plr.dest = marker_pos[plr.curr_mark as usize];
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn update_anim() -> Vec2{
    if plr.anim_count <= speed{
        plr.curr = move_lerp_rust(plr.anim_count, plr.old, plr.dest);
        plr.anim_count += 1;
    }
    else {
        plr.curr = marker_pos[plr.curr_mark as usize];
    }

    return plr.curr;
}

#[ffi_function]
#[no_mangle]
pub extern "C" fn my_function(input: Vec2) {
    println!("{}", input.x);
}

#[ffi_function]
#[no_mangle]
pub extern "C" fn add_two_nums(x: i32, y: i32) -> i32 {
    let result = x + y;
    println!("X + Y = {}", result);
    // info!("Result is: {}", result);
    result
}

pub fn move_lerp_rust(curr_time: i32, src: Vec2, dest: Vec2) -> Vec2 {
    Vec2{
        x: f_lerp(src.x, dest.x, (curr_time as f32) / (speed as f32)),
        y: f_lerp(src.y, dest.y, (curr_time as f32) / (speed as f32))
    }
}

pub fn f_lerp(src: f32, dest: f32, scale: f32) -> f32 {
    return src + ((dest - src) * scale);
}

pub fn ffi_inventory() -> Inventory {
    {
        InventoryBuilder::new()
            .register(function!(my_function))
            .register(function!(add_two_nums))
            .register(function!(update_anim))
            .register(function!(init_marker))
            .register(function!(update_pos))
            .inventory()
    }
}

use interoptopus::util::NamespaceMappings;
use interoptopus::{Error, Interop};
use interoptopus_backend_csharp::Unsafe;

pub fn bindings_csharp() -> Result<(), Error> {
    use interoptopus_backend_csharp::overloads::Unity;
    use interoptopus_backend_csharp::{Config, Generator};

    let config = Config {
        use_unsafe: Unsafe::UnsafeKeyword,
        dll_name: "saigai".to_string(),
        namespace_mappings: NamespaceMappings::new("Saigai.Studios"),
        ..Config::default()
    };

    Generator::new(config.clone(), ffi_inventory())
        .add_overload_writer(Unity::new())
        .write_file("bindings/csharp/Interop.cs")?;

    Generator::new(config, ffi_inventory())
        .add_overload_writer(Unity::new())
        .write_file("../Assets/Scripts/Interop.cs")?;

    Ok(())
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn ut_add_two_nums() {
        assert_eq!(5, add_two_nums(3, 2));
    }
}