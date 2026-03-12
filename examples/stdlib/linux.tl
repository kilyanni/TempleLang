namespace Stdlib;

proc malloc(size : long) : ptr
    using "libc.so.6";

proc free(ptr : ptr) : long
    using "libc.so.6";

proc write(fd : long, buf : ptr, count : long) : long
    using "libc.so.6";

proc print(buf : ptr, len : long) : long => write(1, buf, len);

proc printDigit(digit : long) : long
{
    let digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    print(digits + digit, 1);
}

proc exit(code : long) : long
    using "libc.so.6";
