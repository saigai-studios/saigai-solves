use interoptopus::{ffi_function, ffi_type, function, Inventory, InventoryBuilder};
use log::info;

#[ffi_type]
#[repr(C)]
pub struct Vec2 {
    pub x: f32,
    pub y: f32,
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
    info!("Result is: {}", result);
    result
}

pub fn ffi_inventory() -> Inventory {
    {
        InventoryBuilder::new()
            .register(function!(my_function))
            .register(function!(add_two_nums))
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

    Generator::new(config, ffi_inventory())
        .add_overload_writer(Unity::new())
        .write_file("../Assets/Scripts/Interop.cs")?;

    Ok(())
}
