namespace Stdlib;

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
