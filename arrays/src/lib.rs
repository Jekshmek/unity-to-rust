#![allow(non_snake_case)]

mod counter;

use counter::Counter;
use std::mem::transmute;
//В FFI нам нужен указатель на первое значение в срезе и его длину. 
// `bys_ptr: *const u32, bys_len: usize`
// Затем мы можем использовать std::slice::from_raw_parts для повторной сборки среза 
// (или std::vec::Vec::from_raw_parts для создания вектора)

#[no_mangle]
pub extern fn createCounterArray(val: u32) -> *mut Counter {
    let _counter = unsafe { transmute(Box::new(Counter::new(val))) };// выделение памяти в куче и возврат указателя на эту память
    _counter
}

#[no_mangle]
pub extern fn getCounterValueArray(ptr: *mut Counter) -> u32 {
    let mut _counter = unsafe { &mut *ptr };// восстановление
    _counter.get()
}

#[no_mangle]
pub extern fn incrementCounterByArray(ptr: *mut Counter, bys_ptr: *const u32, bys_len: usize) -> u32 {
    let mut _counter = unsafe { &mut *ptr };// восстановление
    let bys = unsafe { std::slice::from_raw_parts(bys_ptr, bys_len) };
    _counter.incr(bys)
}

#[no_mangle]
pub extern fn decrementCounterByArray(ptr: *mut Counter, bys_ptr: *const u32, bys_len: usize) -> u32 {
    let mut _counter = unsafe { &mut *ptr };// восстановление
    let bys = unsafe { std::slice::from_raw_parts(bys_ptr, bys_len) };
    _counter.decr(bys)
}

#[no_mangle]
pub extern fn destroyCounterArray(ptr: *mut Counter) {
    let _counter: Box<Counter> = unsafe{ transmute(ptr) };//удаление через выделение памяти в куче и не возврата указателя
    // Drop
}