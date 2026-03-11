namespace Program;

proc _start() : long
{
    print("Start\n", 6);

    let size = 10;
    let arr = arrInit(size);
    let seed = 420;

    for (let i = 0; i < size; ++i)
    {
        seed = pseudoRandom(seed);
        arrIndex(arr, i) <- seed % (size * 2);
    }

    printArr(arr, size);

    print("\n", 1);
    quickSort(arr, size);
    printArr(arr, size);

    arrFree(arr);
}

proc pseudoRandom(seed : long) : long => (seed * 94813) % 42133;

proc quickSort(arr : ptr, size : long) : long => quickSortCore(arr, 0, size-1);

proc quickSortCore(arr : ptr, lo : long, hi : long) : long
{
    if lo >= hi return 0;

    let partition = partition(arr, lo, hi);
    quickSortCore(arr, lo, partition - 1);
    quickSortCore(arr, partition + 1, hi);
}

proc partition(arr : ptr, lo : long, hi : long) : long
{
    let pivot = deref arrIndex(arr, hi);

    let i = lo;

    for (let j = lo; j <= hi; ++j)
    {
        let elem = deref arrIndex(arr, j);
        if elem < pivot
        {
            arrIndex(arr, j) <- deref arrIndex(arr, i);
            arrIndex(arr, i) <- elem;
            i += 1;
        }
    }

    arrIndex(arr, hi) <- deref arrIndex(arr, i);
    arrIndex(arr, i) <- pivot;

    return i;
}

proc arrInit(size : long) : ptr
{
    let arr = arrAlloc(size);

    for (let i = 0; i < size; ++i)
    {
        arrIndex(arr, i) <- 0;
    }

    return arr;
}

proc arrFree(arr : ptr) : long => free(arr);

proc printArr(arr : ptr, size : long) : long
{
    for (let i = 0; i < size; ++i)
    {
        if i > 0 print(", ", 2);
        printNum((long)deref arrIndex(arr, i));
    }
}

proc arrSize(arr : ptr) : long => deref arrIndex(arr, -1);

proc arrAlloc(size : long) : ptr => malloc(size * 8);

proc arrIndex(arr : ptr, index : long) : ptr => arr + 7 + 8*index;

proc malloc(size : long) : ptr
    using "libc.so.6";

proc free(ptr : ptr) : long
    using "libc.so.6";

proc printNum(num : long) : long => printNumAny(num, 10);

proc printNumAny(num : long, base : long) : long
{
    if (num < 0)
    {
        print("-", 1);
        printNum(-num);
        return;
    }

    let digit = num % base;
    let rest = num / base;
    if (rest > 0) printNumAny(rest, base);
    printDigit(digit);
}

proc printDigit(digit : long) : long
{
    let digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    //  ^~~~~~ This is a pointer

    //           v This is pointer arithmetic (UTF-8: 1 byte per char)
    print(digits + digit, 1);
}

proc print(buf : ptr, len : long) : long => write(1, buf, len);

proc write(fd : long, buf : ptr, count : long) : long
    using "libc.so.6";
