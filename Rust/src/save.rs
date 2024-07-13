use interoptopus::ffi_function;
use serde_derive::{Deserialize, Serialize};
use std::error::Error;

// Main save and load entry/exit points

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn data_load() -> bool {
    match SAVE_STATE.read(&SAVE_FILE) {
        Ok(_) => true,
        Err(_) => false,
    }
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn data_save() -> bool {
    match SAVE_STATE.write(&SAVE_FILE) {
        Ok(_) => true,
        Err(_) => false,
    }
}

// Accessors

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn data_get_language() -> u8 {
    SAVE_STATE.language.into()
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn data_has_earthquake_card(index: i32) -> bool {
    SAVE_STATE.earthquake[index as usize]
}

// Modifiers

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn data_set_language(index: i32) -> bool {
    SAVE_STATE.set_lang(Language::from(index as u8));
    true
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn data_unlock_earthquake_card(index: i32) -> bool {
    // return false if index is out of bounds
    if index as usize >= SAVE_STATE.earthquake.len() {
        false
    } else {
        SAVE_STATE.unlock_card(Map::Earthquake, index as usize);
        true
    }
}

/// A container for the save state
static mut SAVE_STATE: Save = Save::new();

/// The path to the save file.
const SAVE_FILE: &str = "saigai-saves.json";

#[derive(Serialize, Deserialize, Debug, Clone, Copy, PartialEq)]
pub enum Language {
    English,
    Japanese,
}

impl From<u8> for Language {
    fn from(value: u8) -> Self {
        match value {
            0 => Self::English,
            1 => Self::Japanese,
            _ => panic!("impossible language value!!"),
        }
    }
}

impl Into<u8> for Language {
    fn into(self) -> u8 {
        match self {
            Self::English => 0,
            Self::Japanese => 1,
        }
    }
}

pub enum Map {
    Earthquake,
}

#[derive(Serialize, Deserialize, Debug, PartialEq)]
struct Save {
    language: Language,
    earthquake: [bool; 3],
}

impl Save {
    pub const fn new() -> Self {
        Self {
            language: Language::English,
            earthquake: [false; 3],
        }
    }

    pub fn write(&self, path: &str) -> Result<(), Box<dyn Error>> {
        let contents = serde_json::to_string_pretty(self)?;
        std::fs::write(&path, &contents.as_bytes())?;
        Ok(())
    }

    pub fn read(&mut self, path: &str) -> Result<(), Box<dyn Error>> {
        let contents = std::fs::read_to_string(&path)?;
        let save: Save = serde_json::from_str(&contents)?;
        *self = save;
        Ok(())
    }

    pub fn set_lang(&mut self, lang: Language) -> () {
        self.language = lang;
    }

    pub fn unlock_card(&mut self, map: Map, zone: usize) -> () {
        match map {
            Map::Earthquake => self.earthquake[zone] = true,
        }
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn ut_save_and_load() {
        let mut data1 = Save::new();
        data1.set_lang(Language::Japanese);
        let _ = data1.write("./data.json").unwrap();

        let mut data2 = Save::new();
        let _ = data2.read("./data.json").unwrap();
        assert_eq!(data1, data2);
        let _ = std::fs::remove_file("./data.json").unwrap();
    }
}
