pub struct PtrCounter {
    val: u32
}

impl PtrCounter {
    pub fn new(val: u32) -> PtrCounter {
        PtrCounter{val: val}
    }

    pub fn get(&self) -> u32 {
        self.val
    }

    pub fn incr(&mut self, by: u32) -> u32 {
        self.val += by;
        self.val
    }

    pub fn decr(&mut self, by: u32) -> u32 {
        self.val -= by;
        self.val
    }
}