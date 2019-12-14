### ﻿Настройки Uniry
Разрешить unsafe код:
Assets->Project Settings->Player->Other Settings
Configurations: Allow unsafe Code

В папке проекта Assets создать файл smcs.rsp и gmcs.rsp с первой строкой:
-unsafe

### Сборка dll
cd pointers
cargo build

Скопировать dll ...target\debug\ptr_counter.dll в ...Assets/Plugins


[Medium](https://medium.com/jim-fleming/complex-types-with-rust-s-ffi-315d14619479)
[Github](https://github.com/jimfleming/rust-ffi-complex-types)