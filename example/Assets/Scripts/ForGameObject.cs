using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class ForGameObject : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // Все dll в папке Assets/Plagins
    // передача и возврат числа  
    //  unity_rust.dll ...unity-to-rust-master\target\debug\unity_rust.dll скопирована в Assets/Plugins
    [DllImport("unity_rust")]
    private static extern int double_input(int x);

    [DllImport("unity_rust")]
    private static extern int triple_input(int x);
    //------------------------------------------------------------------------------------------

    // Работа с методами
    // ptr_counter.dll ...pointers\target\debug\ptr_counter.dll скопирована в Assets/Plugins
    /*
     Поскольку мы эффективно передаем ссылки на память, понятие объекта с методами на самом
    деле не существует за границей FFI. Чтобы обойти это ограничение, мы можем определить
    статические функции, которые работают с указателями, которые мы интерпретируем
    как исходный объект. Хост затем удерживает этот указатель и использует его при
    вызове этих функций.
    Вот простая реалимзация структуры счетчика с методами увеличения и уменьшения
    Конструктор - конструктор создает экземпляр объекта в памяти и возвращает указатель на него.
    Деструктор для созданных объектов. Мы несем ответственность за очистку памяти, выделенной иностранным языком.
    Функция, действующая как прокси для каждого метода объекта, который мы хотим вызвать ,
    каждая функция, выступающая в качестве прокси, получает указатель в качестве первого аргумента
    */
    [DllImport("ptr_counter")]
    unsafe private static extern int* createCounter(int val);
   
    [DllImport("ptr_counter")]
    unsafe private static extern int  getCounterValue(int* ptr);
    
    [DllImport("ptr_counter")]
    unsafe private static extern int  incrementCounterBy(int* ptr, int val);

    [DllImport("ptr_counter")]
    unsafe private static extern int decrementCounterBy(int* ptr, int val);

    [DllImport("ptr_counter")]
    unsafe private static extern void destroyCounter(int* ptr);
    //------------------------------------------------------------------------------------------

    // Вариант с передачей структуры 
    // struct_counter.dll ...struct\target\debug\struct_counter.dll скопирована в Assets/Plugins
    /*
       передача структуры и указателя тип int и возврат просто того же указателя,
       посылаем указатель в rust, он преобразуется в структуру Counter,
       а в C# указатель преобразуется в тип int
     */
     public struct Args
     {
            public int init;
            public int by;
     };
  
     public Args obj_args;
     
    [DllImport("struct_counter")]
    unsafe private static extern int* createCounterStruct(Args args);

    [DllImport("struct_counter")]
    unsafe private static extern int getCounterValueStruct(int* ptr);

    [DllImport("struct_counter")]
    unsafe private static extern int incrementCounterByStruct(int* ptr);

    [DllImport("struct_counter")]
    unsafe private static extern int decrementCounterByStruct(int* ptr);

    [DllImport("struct_counter")]
    unsafe private static extern void destroyCounterStruct(int* ptr);
    //------------------------------------------------------------------------------------------

    //Вариант с массивом
    // array_counter.dll ...arrays\target\debug\array_counter.dll скопирована в Assets/Plugins
    /*
     Передача массива оказывается наименее простой из трех техник, поскольку мы не можем просто передавать массив назад и вперед,
     как мы можем с помощью указателей или структур. В большинстве случаев массив может быть представлен указателем на
     первый элемент в массиве и его длиной, поэтому мы будем использовать его.
     Другой вопрос - это владение: кому принадлежит память массива?
     Самый безопасный вариант - позволить хосту быть ответственным за память, так как он обладает большей информацией о том,
     как память должна быть освобождена. Вы передаете массив, манипулируете им на месте, а затем, вместо возврата массива,
     вызывающая сторона может просто прочитать его содержимое после завершения функции.
     Тип массива в Rust должен иметь известную длину во время компиляции, поэтому нам нужно использовать срез или
     «просмотр» массива, который мы суммируем в нашем счетчике:
    
     Из основного языка мы можем просто указать тип массива в качестве аргумента
     */
     [DllImport("array_counter")]
     unsafe private static extern int* createCounterArray(int val);

     [DllImport("array_counter")]
     unsafe private static extern int getCounterValueArray(int* ptr);

     [DllImport("array_counter")]
     unsafe private static extern int incrementCounterByArray(int* ptr,int* bys_ptr,int bys_len);
     
     [DllImport("array_counter")]    
     unsafe private static extern int decrementCounterByArray(int* ptr,int* bys_ptr,int bys_len);
     
     [DllImport("array_counter")]   
     unsafe private static extern void destroyCounterArray(int* ptr);
     //------------------------------------------------------------------------------------------
     
    // Start is called before the first frame update
    void Start()
    {
        //------------------------------------------------------------------------------------------
         // Простой вариант использования ffi
        /*
         int test = double_input(4);
          Debug.Log(test); // 8
          Debug.Log(triple_input(4)); // 12
        */
        //------------------------------------------------------------------------------------------
        // Вариант счетчика через указатели
        /*
         unsafe
        {
            //created counter
            int* ptr = createCounter(7);
            int val = getCounterValue(ptr);
            Debug.Log(val);//7
            val = incrementCounterBy(ptr, 3);
            Debug.Log(val);//10
            val = decrementCounterBy(ptr, 2);
            Debug.Log(val);//8
            destroyCounter(ptr);
        }
        */
        //------------------------------------------------------------------------------------------
        // Вариант счетчика через указатели и структуры
        /*
         unsafe
        {
            obj_args.init = 4;
            obj_args.by = 2;
            int* ptr = createCounterStruct(obj_args);
            int val = getCounterValueStruct(ptr);
            Debug.Log(val);//4
            val = incrementCounterByStruct(ptr);
            Debug.Log(val);//6
            val = decrementCounterByStruct(ptr);
            Debug.Log(val);//4
            destroyCounterStruct(ptr);
        }*/
        
        //------------------------------------------------------------------------------------------
        // Вариант счетчика через указатели и массив
        unsafe
        {
           //#1
            // stackalloc -  выделить память под массив в стеке
            const int bys_len = 3;
            int* bys_ptr = stackalloc int[bys_len]{1,1,2}; // выделяем память в стеке под bys_len объектов int
            //int* bys_ptr = stackalloc int[bys_len];
            //int* p = bys_ptr;//Для манипуляций с массивом создаем указатель p, который указывает на первый элемент массива, в котором всего bys_len элементов
            //*(p++)= 1; // присваиваем первой ячейке значение 1 и
            //*(p++)= 1;
            //*(p++)= 2;
            
            int* ptr = createCounterArray(4);
            int val = getCounterValueArray(ptr);
            Debug.Log(val);//4
            val = incrementCounterByArray(ptr, bys_ptr, bys_len);
            Debug.Log(val);//8
            val = incrementCounterByArray(ptr, bys_ptr, bys_len);
            Debug.Log(val);//12
            val = decrementCounterByArray(ptr, bys_ptr, bys_len);
            Debug.Log(val);//8
            destroyCounterArray(ptr);
            
            /*
             // #2
             // fixed 
              int[] iArray = new int[3] { 1, 1, 2  };
              // fixed используется когда данные за указателем будут размещены в куче и что бы сборщик мусора не почистил 
              // Чтобы фиксировать на все время работы указатели на объекты классов используется оператор fixed.
            fixed (int* bys_ptr = iArray)
            {
                //int[] nums2 = new int[3] { 1, 1, 2  };
                //int[] bys_ptr = &nums2;
                
                int* ptr = createCounterArray(4);
                int val = getCounterValueArray(ptr);
                Debug.Log(val);//4
                // К текущему значению val(который) добавить все значения массива
                val = incrementCounterByArray(ptr, bys_ptr, iArray.Length);
                Debug.Log(val);//8
                val = incrementCounterByArray(ptr, bys_ptr, iArray.Length);
                Debug.Log(val);//12
                val = decrementCounterByArray(ptr, bys_ptr, iArray.Length);
                Debug.Log(val);//8
                destroyCounterArray(ptr);
            } //После завершения блока fixed закрепление с переменных снимается, и они могут быть подвержены сборке мусора.
            */
        }
    }
    void Update() {}
    void run() {}
}
