namespace Stdlib;

proc HeapAlloc(heapHandle : ptr, flags : long, size : long) : ptr
    using "kernel32.lib";

proc HeapFree(heapHandle : ptr, flags : long, pos : ptr) : bool
    using "kernel32.lib";

proc GetProcessHeap() : ptr
    using "kernel32.lib";

proc GetStdHandle(id : long) : ptr
    using "kernel32.lib";

proc WriteConsoleW(
    stdOut : ptr,
    message : ptr,
    messageLength : long,
    numberOfCharsWritten : ptr,
    zero : long) : long
    using "kernel32.lib";

proc malloc(size : long) : ptr
{
    let heapHandle = GetProcessHeap();
    return HeapAlloc(heapHandle, 0, size);
}

proc free(position : ptr) : long
{
    let heapHandle = GetProcessHeap();
    return HeapFree(heapHandle, 0, position);
}

proc print(buf : ptr, len : long) : long
{
    let stdOut = GetStdHandle(-11);
    let numberOfCharsWritten = 0;
    return WriteConsoleW(stdOut, buf, len, ref numberOfCharsWritten, 0);
}

proc printDigit(digit : long) : long
{
    let digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    print(digits + digit * 2, 1);
}
