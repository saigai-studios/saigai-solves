//! The game logic and exposed FFI for the catch minigame.

use interoptopus::ffi_function;

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn init_catch_game(hard_mode: bool) -> () {
    CATCH_MG = CatchMg::new();
    if hard_mode == true {
        CATCH_MG.enable_hard_mode();
    }
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn is_next_spawn_ready() -> bool {
    CATCH_MG.is_next_spawn_ready()
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn spawn_new_item() -> i32 {
    CATCH_MG.spawn_new_item().into()
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn is_catch_game_won() -> bool {
    CATCH_MG.is_game_won()
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn good_catch() -> i32 {
    let reward = match CATCH_MG.is_hard_mode_enabled() {
        false => 100,
        true => 50,
    };
    CATCH_MG.increase_score(reward)
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn bad_catch() -> i32 {
    let penalty = match CATCH_MG.is_hard_mode_enabled() {
        false => 100,
        true => 500,
    };
    CATCH_MG.decrease_score(penalty)
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn missed_catch() -> i32 {
    let penalty = match CATCH_MG.is_hard_mode_enabled() {
        false => 100,
        true => 200,
    };
    CATCH_MG.decrease_score(penalty)
}

/// A container for the catch minigame
static mut CATCH_MG: CatchMg = CatchMg::new();

/// The number of points to earn in the game in order to win.
const POINTS_GOAL: i32 = 1_000;

enum Item {
    Pet,
    Rock,
}

impl Into<i32> for Item {
    fn into(self) -> i32 {
        match self {
            Self::Pet => 0,
            Self::Rock => 1,
        }
    }
}

struct CatchMg {
    points: i32,
    count_index: u32,
    count_limit: Option<u32>,
    hard_mode: bool,
}

impl CatchMg {
    pub const fn new() -> Self {
        Self {
            points: 0,
            count_index: 0,
            count_limit: None,
            hard_mode: false,
        }
    }

    pub fn is_hard_mode_enabled(&self) -> bool {
        self.hard_mode
    }

    pub fn enable_hard_mode(&mut self) -> () {
        self.hard_mode = true;
    }

    pub fn is_next_spawn_ready(&mut self) -> bool {
        if self.is_game_won() == true {
            return false;
        }

        if self.count_limit.is_none() == true {
            self.count_limit = Some(self.set_count_limit());
        }

        if self.count_index >= self.count_limit.unwrap() {
            // reset the counter
            self.count_index = 0;
            self.count_limit = Some(self.set_count_limit());
            true
        } else {
            //  increment the counter
            self.count_index += 1;
            false
        }
    }

    fn set_count_limit(&self) -> u32 {
        match self.hard_mode {
            false => 120, // roughly ~2 seconds per drop
            true => 85,   // roughly ~1.5 seconds per drop
        }
    }

    /// Randomize how to select next item to drop.
    pub fn spawn_new_item(&self) -> Item {
        let draw = rand::random::<u8>();
        // determine odds of selecting a pet if draw < odds
        let odds = match &self.hard_mode {
            // roughly ~2/3 chance
            false => 171,
            true => {
                match self.points >= 950 {
                    // roughly ~1/10 chance
                    true => 25,
                    // roughly ~1/2 chance
                    false => 128,
                }
            }
        };
        match draw < odds {
            true => Item::Pet,
            false => Item::Rock,
        }
    }

    /// Checks if enough points are earned to complete the game.
    pub fn is_game_won(&self) -> bool {
        self.points >= POINTS_GOAL
    }

    pub fn increase_score(&mut self, count: u16) -> i32 {
        let count = count as i32;
        self.points += count;
        self.points
    }

    pub fn decrease_score(&mut self, count: u16) -> i32 {
        let count = count as i32;
        if count > self.points {
            self.points = 0;
        } else {
            self.points -= count;
        }
        self.points
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

    #[test]
    fn ut_zero_score() {
        let mut mg = CatchMg::new();

        mg.increase_score(100);
        assert_eq!(mg.points, 100);
        mg.decrease_score(60);
        assert_eq!(mg.points, 40);
        mg.decrease_score(70);
        assert_eq!(mg.points, 0);
        mg.decrease_score(10);
        assert_eq!(mg.points, 0);
        mg.increase_score(25);
        assert_eq!(mg.points, 25);
    }

    #[test]
    fn ut_win_game() {
        let mut mg = CatchMg::new();
        assert_eq!(mg.is_game_won(), false);
        mg.increase_score(100);
        assert_eq!(mg.is_game_won(), false);
        mg.increase_score(899);
        assert_eq!(mg.is_game_won(), false);
        mg.increase_score(1);
        assert_eq!(mg.is_game_won(), true);

        let mut mg = CatchMg::new();
        assert_eq!(mg.is_game_won(), false);
        mg.increase_score(1001);
        assert_eq!(mg.is_game_won(), true);
    }

    #[test]
    fn ut_win_game_hard() {
        let mut mg = CatchMg::new();
        mg.enable_hard_mode();
        assert_eq!(mg.is_game_won(), false);
        mg.increase_score(500);
        assert_eq!(mg.is_game_won(), false);
        mg.increase_score(498);
        assert_eq!(mg.is_game_won(), false);
        mg.increase_score(2);
        assert_eq!(mg.is_game_won(), true);

        let mut mg = CatchMg::new();
        mg.enable_hard_mode();
        assert_eq!(mg.is_game_won(), false);
        mg.increase_score(1002);
        assert_eq!(mg.is_game_won(), true);
    }
}
