use interoptopus::{ffi_function, ffi_type};

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn init_marker(ind: i32, pos: Vec2) {
    OVERWORLD.set_marker(ind as usize, pos)
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn update_pos_key(opt: bool) {
    PLAYER.update_pos_key(opt, &OVERWORLD.markers)
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn update_pos_click(marker: i32) -> bool {
    PLAYER.update_pos_click(marker, &OVERWORLD.markers)
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn update_anim() -> Vec2 {
    PLAYER.update_anim(&OVERWORLD.markers[3], &OVERWORLD.markers)
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn get_anim_state() -> PlayerAnim {
    PLAYER.get_anim_state()
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn set_anim_state(new_anim: PlayerAnim) {
    PLAYER.set_anim_state(new_anim);
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn set_speed(new_speed: i32) {
    PLAYER.set_speed(new_speed);
}

type Markers = [Vec2; 4];

// Animation speed
//const SPEED: i32 = 20;

const GAME_MARKER_FIRST: i32 = 0;
const GAME_MARKER_LAST: i32 = 2;

// Player variables for map traversal
static mut OVERWORLD: Overworld = Overworld::new();
static mut PLAYER: Player = Player::new();

pub struct Overworld {
    markers: Markers,
}

impl Overworld {
    pub const fn new() -> Self {
        Self {
            markers: [Vec2::new(); 4],
        }
    }

    pub fn set_marker(&mut self, index: usize, location: Vec2) {
        self.markers[index] = location;
    }
}

#[ffi_type]
#[repr(C)]
pub struct Player {
    pub curr: Vec2,
    pub old: Vec2,
    pub dest: Vec2,
    pub curr_mark: i32,
    pub anim_count: i32,
    pub anim_state: PlayerAnim,
    pub speed: i32,
}

impl Player {
    const fn new() -> Self {
        Self {
            curr: Vec2::new(),
            old: Vec2::new(),
            dest: Vec2::new(),
            curr_mark: 0,
            anim_count: 40,
            anim_state: PlayerAnim::IDLE,
            speed: 20,
        }
    }

    fn move_lerp_rust(speed: i32, curr_time: i32, src: &Vec2, dest: &Vec2) -> Vec2 {
        Vec2 {
            x: Self::f_lerp(src.x, dest.x, (curr_time as f32) / (speed as f32)),
            y: Self::f_lerp(src.y, dest.y, (curr_time as f32) / (speed as f32)),
        }
    }

    fn f_lerp(src: f32, dest: f32, scale: f32) -> f32 {
        return src + ((dest - src) * scale);
    }

    fn get_current_mark(&self) -> usize {
        self.curr_mark as usize
    }

    fn get_anim_state(&self) -> PlayerAnim {
        self.anim_state
    }

    fn set_anim_state(&mut self, new_anim: PlayerAnim) {
        self.anim_state = new_anim;
    }

    fn set_speed(&mut self, new_speed: i32) {
        self.speed = new_speed;
        self.anim_count = new_speed * 2;
    }
}

/// Functions exposed through the ffi.
impl Player {
    pub fn update_anim(&mut self, middle_waypoint: &Vec2, markers: &Markers) -> Vec2 {
        // Move to the middle waypoint
        if self.anim_count <= self.speed {
            self.curr =
                Self::move_lerp_rust(self.speed, self.anim_count, &self.old, middle_waypoint);
            self.anim_count += 1;
        // Now move to the final destination waypoint
        } else if self.anim_count <= self.speed * 2 {
            self.curr = Self::move_lerp_rust(
                self.speed,
                self.anim_count - self.speed,
                middle_waypoint,
                &self.dest,
            );
            self.anim_count += 1;

            // Update player's animation state to move towards from new marker
            match self.curr_mark {
                0 => self.anim_state = PlayerAnim::LEFT, // Moving towards left marker
                1 => self.anim_state = PlayerAnim::BACK, // Moving towards top marker (no up animation)
                2 => self.anim_state = PlayerAnim::RIGHT, // Moving towards right marker
                _ => self.anim_state = PlayerAnim::IDLE,
            }
        // Stay at the current position
        } else {
            self.curr = markers[self.get_current_mark()];
            self.anim_state = PlayerAnim::IDLE;
        }
        self.curr
    }

    pub fn update_pos_key(&mut self, is_left: bool, markers: &Markers) {
        // Move to the left
        if is_left == true {
            self.curr_mark -= 1;
            if self.curr_mark < GAME_MARKER_FIRST {
                self.curr_mark = GAME_MARKER_LAST;
            }
        // Move to the right
        } else {
            self.curr_mark += 1;
            if self.curr_mark > GAME_MARKER_LAST {
                self.curr_mark = GAME_MARKER_FIRST;
            }
        }

        // Reset animation counter
        self.anim_count = 0;
        // Set starting position
        self.old = self.curr;
        // Set ending position
        self.dest = markers[self.get_current_mark()];
    }

    /// Sets the marker for the player to move to. This function assumes that
    /// `marker` is already a valid index in the available minigame markers.
    ///
    /// This function returns true when the animation is needed to update
    /// on the frontend (?).
    pub fn update_pos_click(&mut self, marker: i32, markers: &Markers) -> bool {
        // If marker matches the current, no update is needed
        if marker == self.curr_mark {
            // Check if player has reached marker
            if self.anim_count <= self.speed * 2 {
                return false; // Do not update level yet
            } else {
                return true; // Update the level
            }
        }

        // Update the player's current marker
        let old_marker = self.curr_mark;
        self.curr_mark = marker;

        // If intersection has been passed
        if self.anim_count > self.speed {
            // Reset animation counter
            self.anim_count = 0;

            // Set starting position
            self.old = self.curr;

            // Update player's animation state to move away from old marker
            match old_marker {
                0 => self.anim_state = PlayerAnim::RIGHT, // Moving right from left marker
                1 => self.anim_state = PlayerAnim::FORWARD, // Moving down from top marker
                2 => self.anim_state = PlayerAnim::LEFT,  // Moving left from right marker
                _ => self.anim_state = PlayerAnim::IDLE,
            }
        }

        // Set ending position - this is always changed
        self.dest = markers[self.get_current_mark()];
        false
    }
}

#[ffi_type]
#[repr(C)]
#[derive(Copy, Clone)]
pub enum PlayerAnim {
    IDLE,
    LEFT,
    RIGHT,
    FORWARD,
    BACK,
}

#[ffi_type]
#[repr(C)]
#[derive(Copy, Clone)]
pub struct Vec2 {
    pub x: f32,
    pub y: f32,
}

impl Vec2 {
    pub const fn new() -> Self {
        Self { x: 0.0, y: 0.0 }
    }

    pub fn with(x: f32, y: f32) -> Self {
        Self { x: x, y: y }
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn it_update_pos_key() {
        let mut world = Overworld::new();
        // south region
        world.set_marker(0, Vec2::with(2.0, 0.0));
        // west region
        world.set_marker(1, Vec2::with(1.0, 1.0));
        // east region
        world.set_marker(2, Vec2::with(3.0, 1.0));
        // middle waypoint
        world.set_marker(3, Vec2::with(2.0, 1.0));

        let mut player = Player::new();
        // move around the map
        assert_eq!(player.curr_mark, GAME_MARKER_FIRST);
        player.update_pos_key(true, &world.markers);
        assert_eq!(player.curr_mark, GAME_MARKER_LAST);
        player.update_pos_key(true, &world.markers);
        assert_eq!(player.curr_mark, GAME_MARKER_LAST - 1);
        player.update_pos_key(true, &world.markers);
        assert_eq!(player.curr_mark, GAME_MARKER_LAST - 2);
        player.update_pos_key(false, &world.markers);
        assert_eq!(player.curr_mark, GAME_MARKER_LAST - 1);
    }
}
