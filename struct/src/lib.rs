#![allow(non_snake_case)]

mod counter;

use counter::{Counter, Args};
use std::mem::transmute;

#[no_mangle]
pub extern fn createCounterStruct(args: Args) -> *mut Counter {
    let _counter = unsafe { transmute(Box::new(Counter::new(args))) };
    _counter
}

#[no_mangle]
pub extern fn getCounterValueStruct(ptr: *mut Counter) -> u32 {
    let mut _counter = unsafe { &mut *ptr };
    _counter.get()
}

#[no_mangle]
pub extern fn incrementCounterByStruct(ptr: *mut Counter) -> u32 {
    let mut _counter = unsafe { &mut *ptr };
    _counter.incr()
}

#[no_mangle]
pub extern fn decrementCounterByStruct(ptr: *mut Counter) -> u32 {
    let mut _counter = unsafe { &mut *ptr };
    _counter.decr()
}

#[no_mangle]
pub extern fn destroyCounterStruct(ptr: *mut Counter) {
    let _counter: Box<Counter> = unsafe{ transmute(ptr) };
    // Drop
}