
use std::{ffi::c_int, mem::MaybeUninit};
https://users.rust-lang.org/t/ffi-how-to-pass-a-array-with-structs-to-a-c-func-that-fills-the-array-out-pointer-and-then-how-to-access-the-items-after-in-my-rust-code/83798/2

#[derive(Debug)]
#[repr(C)]
struct Device {
    id: c_int,
}

fn main() {
    // using an array
    let mut devices: [MaybeUninit<Device>; 8] = MaybeUninit::uninit_array();

    unsafe {
        let num_initialized =
            initialize_devices(devices.as_mut_ptr().cast(), devices.len() as c_int);
        assert!(num_initialized >= 0, "An error occurred while initializing");

        let initialized: &[Device] =
            MaybeUninit::slice_assume_init_ref(&devices[..num_initialized as usize]);
    }

    // using a vec
    let mut devices: Vec<Device> = Vec::with_capacity(8);

    unsafe {
        // Get a reference to the uninitialized part of our Vec's buffer
        let uninitialized = devices.spare_capacity_mut();
        let num_initialized = initialize_devices(
            uninitialized.as_mut_ptr().cast(),
            uninitialized.len() as c_int,
        );
        assert!(num_initialized >= 0, "An error occurred while initializing");

        // Tell the Vec that we've manually initialized some elements
        devices.set_len(num_initialized as usize);
    }
}

/// Try to populate the provided array with initialized devices, returning
/// the number of devices that were initialized.
///
/// A negative return value indicates an error occurred.
///
/// (Imagine this function came from your C library)
unsafe extern "C" fn initialize_devices(devices: *mut Device, len: c_int) -> c_int {
    if len < 2 {
        return -1;
    }

    devices.write(Device { id: 1 });
    devices.add(1).write(Device { id: 2 });
    2
}