//! The game logic and exposed FFI for the catch minigame.

use interoptopus::ffi_function;

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn is_next_spawn_ready() -> bool {
    CATCH_MG.is_next_spawn_ready()
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn is_catch_game_won() -> bool {
    CATCH_MG.is_game_won()
}

/// A container for the catch minigame
static mut CATCH_MG: CatchMg = CatchMg::new();

/// The number of points to earn in the game in order to win.
const POINTS_GOAL: u8 = 5;

struct CatchMg {
    points: u8,
    count_index: u32,
    count_limit: Option<u32>,
}

impl CatchMg {
    pub const fn new() -> Self {
        Self {
            points: 0,
            count_index: 0,
            count_limit: None,
        }
    }

    pub fn is_next_spawn_ready(&mut self) -> bool {
        if self.count_limit.is_none() == true {
            self.count_limit = Some(Self::set_count_limit());
        }

        if self.count_index >= self.count_limit.unwrap() {
            // reset the counter
            self.count_index = 0;
            self.count_limit = Some(Self::set_count_limit());
            true
        } else {
            //  increment the counter
            self.count_index += 1;
            false
        }
    }

    const fn set_count_limit() -> u32 {
        120
    }

    /// Checks if enough points are earned to complete the game.
    pub fn is_game_won(&self) -> bool {
        self.points >= POINTS_GOAL
    }
}

#[cfg(test)]
impl CatchMg {
    fn get_count_limit(&self) -> u32 {
        self.count_limit.unwrap()
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn ut_next_spawn() {
        let mut mg = CatchMg::new();
        // assert that we can break from the loop twice
        let mut counter = 0;
        while mg.is_next_spawn_ready() == false {
            counter += 1;
        }
        assert_eq!(counter, mg.get_count_limit());

        let mut counter = 0;
        while mg.is_next_spawn_ready() == false {
            counter += 1;
        }
        assert_eq!(counter, mg.get_count_limit());
    }
}
