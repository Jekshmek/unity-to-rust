  
#![allow(non_snake_case)]

mod counter;

use counter::PtrCounter;
use std::mem::transmute;
// Мы используем Rust для выделения памяти, чтобы создать наш счетчик в куче, используя Box , а затем преобразуем это поле в необработанный указатель. 
// Эта хитрость избавляет от необходимости вручную распределять память
//Наш деструктор работает аналогично, переводя указатель счетчика обратно в блок, а затем автоматически сбрасывает его .

// Функция преобразует этот указатель в исходный тип и вызывает нужный метод, передавая любые аргументы и, наконец, возвращая результат (если есть).
// В отличие от нашего деструктора, мы не хотим превращать эти указатели обратно в коробку, пока не будем готовы уничтожить ее.
#[no_mangle]
pub extern fn createCounter(val: u32) -> *mut PtrCounter {
    let _counter = unsafe { transmute(Box::new(PtrCounter::new(val))) };// выделение памяти в куче и возврат указателя на эту память
    _counter
}

#[no_mangle]
pub extern fn getCounterValue(ptr: *mut PtrCounter) -> u32 {
    let mut _counter = unsafe { &mut *ptr };// восстановление
    _counter.get()
}

#[no_mangle]
pub extern fn incrementCounterBy(ptr: *mut PtrCounter, by: u32) -> u32 {
    let mut _counter = unsafe { &mut *ptr };// восстановление
    _counter.incr(by)
}

#[no_mangle]
pub extern fn decrementCounterBy(ptr: *mut PtrCounter, by: u32) -> u32 {
    let mut _counter = unsafe { &mut *ptr };// восстановление
    _counter.decr(by)
}

#[no_mangle]
pub extern fn destroyCounter(ptr: *mut PtrCounter) {
    let _counter: Box<PtrCounter> = unsafe{ transmute(ptr) };//удаление через выделение памяти в куче и не возврата указателя
    // Drop
}


