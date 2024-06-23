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

type Markers = [Vec2; 4];

// Animation speed
const SPEED: i32 = 100;

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
}

impl Player {
    const fn new() -> Self {
        Self {
            curr: Vec2::new(),
            old: Vec2::new(),
            dest: Vec2::new(),
            curr_mark: 0,
            anim_count: SPEED * 2,
        }
    }

    fn move_lerp_rust(curr_time: i32, src: &Vec2, dest: &Vec2) -> Vec2 {
        Vec2 {
            x: Self::f_lerp(src.x, dest.x, (curr_time as f32) / (SPEED as f32)),
            y: Self::f_lerp(src.y, dest.y, (curr_time as f32) / (SPEED as f32)),
        }
    }

    fn f_lerp(src: f32, dest: f32, scale: f32) -> f32 {
        return src + ((dest - src) * scale);
    }

    fn get_current_mark(&self) -> usize {
        self.curr_mark as usize
    }
}

/// Functions exposed through the ffi.
impl Player {
    pub fn update_anim(&mut self, middle_waypoint: &Vec2, markers: &Markers) -> Vec2 {
        // Move to the middle waypoint
        if self.anim_count <= SPEED {
            //PLR.curr = move_lerp_rust(PLR.anim_count, PLR.old, PLR.dest);
            self.curr = Self::move_lerp_rust(self.anim_count, &self.old, middle_waypoint);
            self.anim_count += 1;
        // Now move to the final destination waypoint
        } else if self.anim_count <= SPEED * 2 {
            //PLR.curr = move_lerp_rust(PLR.anim_count, PLR.old, PLR.dest);
            self.curr = Self::move_lerp_rust(self.anim_count - SPEED, middle_waypoint, &self.dest);
            self.anim_count += 1;
        // Stay at the current position
        } else {
            self.curr = markers[self.get_current_mark()];
        }
        self.curr
    }

    pub fn update_pos_key(&mut self, is_left: bool, markers: &Markers) {
        // Decrement and wrap
        if is_left == true {
            // Left
            self.curr_mark -= 1;

            if self.curr_mark < 0 {
                self.curr_mark = 2;
            }
        } else {
            // Right
            self.curr_mark += 1;

            if self.curr_mark > 2 {
                self.curr_mark = 0;
            }
        }

        // Reset animation counter
        self.anim_count = 0;

        // Set starting position
        self.old = self.curr;

        // Set ending position
        self.dest = markers[self.get_current_mark()];
    }

    pub fn update_pos_click(&mut self, marker: i32, markers: &Markers) -> bool {
        // If marker matches the current, no update is needed
        if marker == self.curr_mark {
            // Check if player has reached marker
            if self.anim_count <= SPEED * 2 {
                return false; // Do not update level yet
            } else {
                return true; // Update the level
            }
        }

        // Update the player's current marker
        self.curr_mark = marker;

        // If intersection has been passed
        if self.anim_count > SPEED {
            // Reset animation counter
            self.anim_count = 0;

            // Set starting position
            self.old = self.curr;
        }

        // Set ending position - this is always changed
        self.dest = markers[self.get_current_mark()];
        false
    }
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
