use std::{error::Error, path::PathBuf};

use serde_derive::{Deserialize, Serialize};

#[derive(Serialize, Deserialize, Debug)]
pub enum Language {
    English,
    Japanese,
}

#[derive(Serialize, Deserialize, Debug)]
struct Save {
    lang: Language,
    earthquake: [bool; 3],
}

impl Save {
    pub fn new() -> Self {
        Self {
            lang: Language::English,
            earthquake: [false; 3],
        }
    }

    pub fn write(&self, path: &PathBuf) -> Result<(), Box<dyn Error>> {
        let contents = serde_json::to_string_pretty(self)?;
        std::fs::write(&path, &contents.as_bytes())?;
        Ok(())
    }

    pub fn read(&mut self, path: &PathBuf) -> Result<(), Box<dyn Error>> {
        let contents = std::fs::read_to_string(&path)?;
        let save: Save = serde_json::from_str(&contents)?;
        *self = save;
        Ok(())
    }
}
