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

    Generator::new(config, saigai::ffi_inventory())
        .add_overload_writer(Unity::new())
        .write_file("../Assets/Scripts/Interop.cs")?;

    Ok(())
}

fn main() {
    let _ = bindings_csharp().unwrap();
}
