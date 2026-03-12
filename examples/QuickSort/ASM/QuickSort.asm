section .data
FALSE: equ     0
TRUE: equ     1
longC0: equ     10
ptrC1: db      `-`
ptrC2: db      `Start\n`
longC3: equ     6
longC4: equ     420
longC5: equ     2
ptrC6: db      `\n`
longC7: equ     94813
longC8: equ     42133
ptrC9: db      `, `
longC10: equ     8
longC11: equ     7
ptrC12: db      `0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ`
section .text
    global  _start
    extern  malloc
    extern  free
    extern  write
printNum: ; printNum(num : long) : long
        sub     qword rsp, 32 ; Allocate stack
    
      ; <>T1 = Call printNumAny(num, 10)
      ; In = { num }
        mov     qword [rsp + 16], qword rdi ; Store live variable onto stack (num)
        mov     qword rdi, qword [rsp + 16] ; Pass parameter #0
        mov     qword rsi, qword longC0 ; Pass parameter #1
        call    printNumAny
        mov     qword r15, qword rax ; Assign return value to <>T1
      ; Out = { <>T1 }
      ; /
    
      ; Return <>T1
      ; In = { <>T1 }
        mov     qword rax, qword r15 ; Return <>T1
        jmp     .__exit
      ; Out = {  }
      ; /
    
    .__exit:          ; Function exit/return label
        add     qword rsp, 32 ; Return stack
        ret     
    
printNumAny: ; printNumAny(num : long, base : long) : long
        sub     qword rsp, 48 ; Allocate stack
    
        mov     qword r14, qword rsi
      ; <>T2 = num ComparisonLessThan 0
      ; In = { num, base }
        cmp     qword rdi, qword FALSE ; Set condition codes according to operands
        jl      .CG0 ; Jump to True if the comparison is true
        mov     qword r13, qword FALSE ; Assign false to output
        jmp     .CG1 ; Jump to Exit
    .CG0:          ; True
        mov     qword r13, qword TRUE ; Assign true to output
    .CG1:          ; Exit
      ; Out = { num, base, <>T2 }
      ; /
    
      ; If <>T2 Jump .T3
      ; In = { num, base, <>T2 }
        test    qword r13, qword r13 ; Set condition codes according to condition
        jnz     .T3 ; Jump if condition is true/non-zero
      ; Out = { num, base }
      ; /
    
      ; Jump .T4
      ; In = { num, base }
        jmp     .T4
      ; Out = { num, base }
      ; /
    
      ; .T3:
      ; In = { num, base }
    .T3:         
      ; Out = { num, base }
      ; /
    
      ; _ = Call print(-, 1)
      ; In = { num, base }
        mov     qword [rsp + 32], qword rdi ; Store live variable onto stack (num)
        mov     qword [rsp + 24], qword r14 ; Store live variable onto stack (base)
        mov     qword rdi, qword ptrC1 ; Pass parameter #0
        mov     qword rsi, qword TRUE ; Pass parameter #1
        call    print
        mov     qword rdi, qword [rsp + 32] ; Restore live variable from stack (num)
        mov     qword r14, qword [rsp + 24] ; Restore live variable from stack (base)
      ; Out = { num, base }
      ; /
    
      ; <>T5 = ArithmeticNegation num
      ; In = { num, base }
        mov     qword r13, qword rdi ; Assign operand to target
        neg     qword r13
      ; Out = { num, base, <>T5 }
      ; /
    
      ; _ = Call printNum(<>T5)
      ; In = { num, base, <>T5 }
        mov     qword [rsp + 32], qword rdi ; Store live variable onto stack (num)
        mov     qword [rsp + 24], qword r14 ; Store live variable onto stack (base)
        mov     qword [rsp + 16], qword r13 ; Store live variable onto stack (<>T5)
        mov     qword rdi, qword [rsp + 16] ; Pass parameter #0
        call    printNum
        mov     qword rdi, qword [rsp + 32] ; Restore live variable from stack (num)
        mov     qword r14, qword [rsp + 24] ; Restore live variable from stack (base)
      ; Out = { num, base }
      ; /
    
      ; Return 
      ; In = { num, base }
        jmp     .__exit
      ; Out = { num, base }
      ; /
    
      ; .T4:
      ; In = { num, base }
    .T4:         
      ; Out = { num, base }
      ; /
    
      ; <>T6 = num Remainder base
      ; In = { num, base }
        xor     qword rdx, qword rdx ; Empty out higher bits of dividend
        mov     qword rax, qword rdi ; Assign LHS to dividend
        idiv    qword r14 ; Assign remainder to RDX, quotient to RAX
        mov     qword r13, qword rdx ; Assign result to target memory
      ; Out = { <>T6, num, base }
      ; /
    
      ; _ = digit Assign <>T6
      ; In = { <>T6, num, base }
        mov     qword r12, qword r13
      ; Out = { digit, num, base }
      ; /
    
      ; <>T7 = num Divide base
      ; In = { digit, num, base }
        xor     qword rdx, qword rdx ; Empty out higher bits of dividend
        mov     qword rax, qword rdi ; Assign LHS to dividend
        idiv    qword r14 ; Assign remainder to RDX, quotient to RAX
        mov     qword r11, qword rax ; Assign result to target memory
      ; Out = { digit, <>T7, base }
      ; /
    
      ; _ = rest Assign <>T7
      ; In = { digit, <>T7, base }
        mov     qword r13, qword r11
      ; Out = { digit, rest, base }
      ; /
    
      ; <>T8 = rest ComparisonGreaterThan 0
      ; In = { digit, rest, base }
        cmp     qword r13, qword FALSE ; Set condition codes according to operands
        jg      .CG2 ; Jump to True if the comparison is true
        mov     qword rdi, qword FALSE ; Assign false to output
        jmp     .CG3 ; Jump to Exit
    .CG2:          ; True
        mov     qword rdi, qword TRUE ; Assign true to output
    .CG3:          ; Exit
      ; Out = { digit, rest, base, <>T8 }
      ; /
    
      ; If <>T8 Jump .T9
      ; In = { digit, rest, base, <>T8 }
        test    qword rdi, qword rdi ; Set condition codes according to condition
        jnz     .T9 ; Jump if condition is true/non-zero
      ; Out = { digit, rest, base }
      ; /
    
      ; Jump .T10
      ; In = { digit }
        jmp     .T10
      ; Out = { digit }
      ; /
    
      ; .T9:
      ; In = { digit, rest, base }
    .T9:         
      ; Out = { digit, rest, base }
      ; /
    
      ; _ = Call printNumAny(rest, base)
      ; In = { digit, rest, base }
        mov     qword [rsp + 32], qword r12 ; Store live variable onto stack (digit)
        mov     qword [rsp + 24], qword r13 ; Store live variable onto stack (rest)
        mov     qword [rsp + 16], qword r14 ; Store live variable onto stack (base)
        mov     qword rdi, qword [rsp + 24] ; Pass parameter #0
        mov     qword rsi, qword [rsp + 16] ; Pass parameter #1
        call    printNumAny
        mov     qword r12, qword [rsp + 32] ; Restore live variable from stack (digit)
      ; Out = { digit }
      ; /
    
      ; .T10:
      ; In = { digit }
    .T10:         
      ; Out = { digit }
      ; /
    
      ; _ = Call printDigit(digit)
      ; In = { digit }
        mov     qword [rsp + 32], qword r12 ; Store live variable onto stack (digit)
        mov     qword rdi, qword [rsp + 32] ; Pass parameter #0
        call    printDigit
      ; Out = {  }
      ; /
    
    .__exit:          ; Function exit/return label
        add     qword rsp, 48 ; Return stack
        ret     
    
_start: ; _start() : long
        sub     qword rsp, 48 ; Allocate stack
    
      ; _ = Call print(Start\n, 6)
      ; In = {  }
        mov     qword rdi, qword ptrC2 ; Pass parameter #0
        mov     qword rsi, qword longC3 ; Pass parameter #1
        call    print
      ; Out = {  }
      ; /
    
      ; _ = size Assign 10
      ; In = {  }
        mov     qword r15, qword longC0
      ; Out = { size }
      ; /
    
      ; <>T11 = Call arrInit(size)
      ; In = { size }
        mov     qword [rsp + 32], qword r15 ; Store live variable onto stack (size)
        mov     qword rdi, qword [rsp + 32] ; Pass parameter #0
        call    arrInit
        mov     qword r14, qword rax ; Assign return value to <>T11
        mov     qword r15, qword [rsp + 32] ; Restore live variable from stack (size)
      ; Out = { <>T11, size }
      ; /
    
      ; _ = arr Assign <>T11
      ; In = { <>T11, size }
        mov     qword r13, qword r14
      ; Out = { arr, size }
      ; /
    
      ; _ = seed Assign 420
      ; In = { arr, size }
        mov     qword r12, qword longC4
      ; Out = { arr, seed, size }
      ; /
    
      ; _ = i Assign 0
      ; In = { arr, seed, size }
        mov     qword r14, qword FALSE
      ; Out = { arr, seed, size, i }
      ; /
    
      ; Jump .T13
      ; In = { arr, seed, size, i }
        jmp     .T13
      ; Out = { arr, seed, size, i }
      ; /
    
      ; .T12:
      ; In = { arr, seed, size, i }
    .T12:         
      ; Out = { arr, seed, size, i }
      ; /
    
      ; <>T15 = PreIncrement i
      ; In = { arr, seed, size, i }
        inc     qword r14
      ; Out = { arr, seed, size, i }
      ; /
    
      ; .T13:
      ; In = { arr, seed, size, i }
    .T13:         
      ; Out = { arr, seed, size, i }
      ; /
    
      ; <>T16 = i ComparisonLessThan size
      ; In = { arr, seed, size, i }
        cmp     qword r14, qword r15 ; Set condition codes according to operands
        jl      .CG0 ; Jump to True if the comparison is true
        mov     qword r11, qword FALSE ; Assign false to output
        jmp     .CG1 ; Jump to Exit
    .CG0:          ; True
        mov     qword r11, qword TRUE ; Assign true to output
    .CG1:          ; Exit
      ; Out = { arr, seed, size, i, <>T16 }
      ; /
    
      ; If !<>T16 Jump .T14
      ; In = { arr, seed, size, i, <>T16 }
        test    qword r11, qword r11 ; Set condition codes according to condition
        jz      .T14 ; Jump if condition is false/zero
      ; Out = { arr, seed, size, i }
      ; /
    
      ; <>T17 = Call pseudoRandom(seed)
      ; In = { arr, seed, size, i }
        mov     qword [rsp + 32], qword r13 ; Store live variable onto stack (arr)
        mov     qword [rsp + 24], qword r12 ; Store live variable onto stack (seed)
        mov     qword [rsp + 16], qword r15 ; Store live variable onto stack (size)
        mov     qword [rsp + 8], qword r14 ; Store live variable onto stack (i)
        mov     qword rdi, qword [rsp + 24] ; Pass parameter #0
        call    pseudoRandom
        mov     qword r10, qword rax ; Assign return value to <>T17
        mov     qword r13, qword [rsp + 32] ; Restore live variable from stack (arr)
        mov     qword r15, qword [rsp + 16] ; Restore live variable from stack (size)
        mov     qword r14, qword [rsp + 8] ; Restore live variable from stack (i)
      ; Out = { arr, <>T17, size, i }
      ; /
    
      ; _ = seed Assign <>T17
      ; In = { arr, <>T17, size, i }
        mov     qword r12, qword r10
      ; Out = { arr, seed, size, i }
      ; /
    
      ; <>T18 = Call arrIndex(arr, i)
      ; In = { arr, seed, size, i }
        mov     qword [rsp + 32], qword r13 ; Store live variable onto stack (arr)
        mov     qword [rsp + 24], qword r12 ; Store live variable onto stack (seed)
        mov     qword [rsp + 16], qword r15 ; Store live variable onto stack (size)
        mov     qword [rsp + 8], qword r14 ; Store live variable onto stack (i)
        mov     qword rdi, qword [rsp + 32] ; Pass parameter #0
        mov     qword rsi, qword [rsp + 8] ; Pass parameter #1
        call    arrIndex
        mov     qword r11, qword rax ; Assign return value to <>T18
        mov     qword r13, qword [rsp + 32] ; Restore live variable from stack (arr)
        mov     qword r12, qword [rsp + 24] ; Restore live variable from stack (seed)
        mov     qword r15, qword [rsp + 16] ; Restore live variable from stack (size)
        mov     qword r14, qword [rsp + 8] ; Restore live variable from stack (i)
      ; Out = { <>T18, seed, size, arr, i }
      ; /
    
      ; <>T20 = size Multiply 2
      ; In = { <>T18, seed, size, arr, i }
        mov     qword r10, qword r15 ; Assign LHS to target memory
        imul    qword r10, qword longC5
      ; Out = { <>T18, seed, <>T20, arr, size, i }
      ; /
    
      ; <>T19 = seed Remainder <>T20
      ; In = { <>T18, seed, <>T20, arr, size, i }
        xor     qword rdx, qword rdx ; Empty out higher bits of dividend
        mov     qword rax, qword r12 ; Assign LHS to dividend
        idiv    qword r10 ; Assign remainder to RDX, quotient to RAX
        mov     qword r9, qword rdx ; Assign result to target memory
      ; Out = { <>T18, <>T19, arr, seed, size, i }
      ; /
    
      ; _ = <>T18 ReferenceAssign <>T19
      ; In = { <>T18, <>T19, arr, seed, size, i }
        mov     qword [r11], qword r9
      ; Out = { arr, seed, size, i }
      ; /
    
      ; Jump .T12
      ; In = { arr, seed, size, i }
        jmp     .T12
      ; Out = { arr, seed, size, i }
      ; /
    
      ; .T14:
      ; In = { arr, size }
    .T14:         
      ; Out = { arr, size }
      ; /
    
      ; _ = Call printArr(arr, size)
      ; In = { arr, size }
        mov     qword [rsp + 32], qword r13 ; Store live variable onto stack (arr)
        mov     qword [rsp + 24], qword r15 ; Store live variable onto stack (size)
        mov     qword rdi, qword [rsp + 32] ; Pass parameter #0
        mov     qword rsi, qword [rsp + 24] ; Pass parameter #1
        call    printArr
        mov     qword r13, qword [rsp + 32] ; Restore live variable from stack (arr)
        mov     qword r15, qword [rsp + 24] ; Restore live variable from stack (size)
      ; Out = { arr, size }
      ; /
    
      ; _ = Call print(\n, 1)
      ; In = { arr, size }
        mov     qword [rsp + 32], qword r13 ; Store live variable onto stack (arr)
        mov     qword [rsp + 24], qword r15 ; Store live variable onto stack (size)
        mov     qword rdi, qword ptrC6 ; Pass parameter #0
        mov     qword rsi, qword TRUE ; Pass parameter #1
        call    print
        mov     qword r13, qword [rsp + 32] ; Restore live variable from stack (arr)
        mov     qword r15, qword [rsp + 24] ; Restore live variable from stack (size)
      ; Out = { arr, size }
      ; /
    
      ; _ = Call quickSort(arr, size)
      ; In = { arr, size }
        mov     qword [rsp + 32], qword r13 ; Store live variable onto stack (arr)
        mov     qword [rsp + 24], qword r15 ; Store live variable onto stack (size)
        mov     qword rdi, qword [rsp + 32] ; Pass parameter #0
        mov     qword rsi, qword [rsp + 24] ; Pass parameter #1
        call    quickSort
        mov     qword r13, qword [rsp + 32] ; Restore live variable from stack (arr)
        mov     qword r15, qword [rsp + 24] ; Restore live variable from stack (size)
      ; Out = { arr, size }
      ; /
    
      ; _ = Call printArr(arr, size)
      ; In = { arr, size }
        mov     qword [rsp + 32], qword r13 ; Store live variable onto stack (arr)
        mov     qword [rsp + 24], qword r15 ; Store live variable onto stack (size)
        mov     qword rdi, qword [rsp + 32] ; Pass parameter #0
        mov     qword rsi, qword [rsp + 24] ; Pass parameter #1
        call    printArr
        mov     qword r13, qword [rsp + 32] ; Restore live variable from stack (arr)
      ; Out = { arr }
      ; /
    
      ; _ = Call arrFree(arr)
      ; In = { arr }
        mov     qword [rsp + 32], qword r13 ; Store live variable onto stack (arr)
        mov     qword rdi, qword [rsp + 32] ; Pass parameter #0
        call    arrFree
      ; Out = {  }
      ; /
    
    .__exit:          ; Function exit/return label
        add     qword rsp, 48 ; Return stack
        ret     
    
pseudoRandom: ; pseudoRandom(seed : long) : long
        sub     qword rsp, 16 ; Allocate stack
    
      ; <>T22 = seed Multiply 94813
      ; In = { seed }
        mov     qword r15, qword rdi ; Assign LHS to target memory
        imul    qword r15, qword longC7
      ; Out = { <>T22 }
      ; /
    
      ; <>T21 = <>T22 Remainder 42133
      ; In = { <>T22 }
        xor     qword rdx, qword rdx ; Empty out higher bits of dividend
        mov     qword rax, qword r15 ; Assign LHS to dividend
        mov     qword rbx, qword longC8 ; Move divisor into RBX, as a register is required for idiv
        idiv    qword rbx ; Assign remainder to RDX, quotient to RAX
        mov     qword r13, qword rdx ; Assign result to target memory
      ; Out = { <>T21 }
      ; /
    
      ; Return <>T21
      ; In = { <>T21 }
        mov     qword rax, qword r13 ; Return <>T21
        jmp     .__exit
      ; Out = {  }
      ; /
    
    .__exit:          ; Function exit/return label
        add     qword rsp, 16 ; Return stack
        ret     
    
quickSort: ; quickSort(arr : ptr, size : long) : long
        sub     qword rsp, 32 ; Allocate stack
    
        mov     qword r13, qword rsi
      ; <>T24 = size Subtract 1
      ; In = { arr, size }
        mov     qword r14, qword r13 ; Assign LHS to target memory
        sub     qword r14, qword TRUE
      ; Out = { arr, <>T24 }
      ; /
    
      ; <>T23 = Call quickSortCore(arr, 0, <>T24)
      ; In = { arr, <>T24 }
        mov     qword [rsp + 16], qword rdi ; Store live variable onto stack (arr)
        mov     qword [rsp + 8], qword r14 ; Store live variable onto stack (<>T24)
        mov     qword rdi, qword [rsp + 16] ; Pass parameter #0
        mov     qword rsi, qword FALSE ; Pass parameter #1
        mov     qword rdx, qword [rsp + 8] ; Pass parameter #2
        call    quickSortCore
        mov     qword r12, qword rax ; Assign return value to <>T23
      ; Out = { <>T23 }
      ; /
    
      ; Return <>T23
      ; In = { <>T23 }
        mov     qword rax, qword r12 ; Return <>T23
        jmp     .__exit
      ; Out = {  }
      ; /
    
    .__exit:          ; Function exit/return label
        add     qword rsp, 32 ; Return stack
        ret     
    
quickSortCore: ; quickSortCore(arr : ptr, lo : long, hi : long) : long
        sub     qword rsp, 64 ; Allocate stack
    
        mov     qword r13, qword rsi
      ; <>T25 = lo ComparisonGreaterThanOrEqual hi
      ; In = { arr, hi, lo }
        cmp     qword r13, qword rdx ; Set condition codes according to operands
        jge     .CG0 ; Jump to True if the comparison is true
        mov     qword r12, qword FALSE ; Assign false to output
        jmp     .CG1 ; Jump to Exit
    .CG0:          ; True
        mov     qword r12, qword TRUE ; Assign true to output
    .CG1:          ; Exit
      ; Out = { arr, hi, lo, <>T25 }
      ; /
    
      ; If <>T25 Jump .T26
      ; In = { arr, hi, lo, <>T25 }
        test    qword r12, qword r12 ; Set condition codes according to condition
        jnz     .T26 ; Jump if condition is true/non-zero
      ; Out = { arr, hi, lo }
      ; /
    
      ; Jump .T27
      ; In = { arr, hi, lo }
        jmp     .T27
      ; Out = { arr, hi, lo }
      ; /
    
      ; .T26:
      ; In = { arr, hi, lo }
    .T26:         
      ; Out = { arr, hi, lo }
      ; /
    
      ; Return 0
      ; In = { arr, hi, lo }
        mov     qword rax, qword FALSE ; Return 0
        jmp     .__exit
      ; Out = { arr, hi, lo }
      ; /
    
      ; .T27:
      ; In = { arr, hi, lo }
    .T27:         
      ; Out = { arr, hi, lo }
      ; /
    
      ; <>T28 = Call partition(arr, lo, hi)
      ; In = { arr, hi, lo }
        mov     qword [rsp + 48], qword rdi ; Store live variable onto stack (arr)
        mov     qword [rsp + 40], qword rdx ; Store live variable onto stack (hi)
        mov     qword [rsp + 32], qword r13 ; Store live variable onto stack (lo)
        mov     qword rdi, qword [rsp + 48] ; Pass parameter #0
        mov     qword rsi, qword [rsp + 32] ; Pass parameter #1
        mov     qword rdx, qword [rsp + 40] ; Pass parameter #2
        call    partition
        mov     qword r12, qword rax ; Assign return value to <>T28
        mov     qword rdi, qword [rsp + 48] ; Restore live variable from stack (arr)
        mov     qword rdx, qword [rsp + 40] ; Restore live variable from stack (hi)
        mov     qword r13, qword [rsp + 32] ; Restore live variable from stack (lo)
      ; Out = { arr, <>T28, hi, lo }
      ; /
    
      ; _ = partition Assign <>T28
      ; In = { arr, <>T28, hi, lo }
        mov     qword r11, qword r12
      ; Out = { arr, partition, hi, lo }
      ; /
    
      ; <>T29 = partition Subtract 1
      ; In = { arr, partition, hi, lo }
        mov     qword r10, qword r11 ; Assign LHS to target memory
        sub     qword r10, qword TRUE
      ; Out = { arr, partition, hi, lo, <>T29 }
      ; /
    
      ; _ = Call quickSortCore(arr, lo, <>T29)
      ; In = { arr, partition, hi, lo, <>T29 }
        mov     qword [rsp + 48], qword rdi ; Store live variable onto stack (arr)
        mov     qword [rsp + 40], qword r11 ; Store live variable onto stack (partition)
        mov     qword [rsp + 32], qword rdx ; Store live variable onto stack (hi)
        mov     qword [rsp + 24], qword r13 ; Store live variable onto stack (lo)
        mov     qword [rsp + 16], qword r10 ; Store live variable onto stack (<>T29)
        mov     qword rdi, qword [rsp + 48] ; Pass parameter #0
        mov     qword rsi, qword [rsp + 24] ; Pass parameter #1
        mov     qword rdx, qword [rsp + 16] ; Pass parameter #2
        call    quickSortCore
        mov     qword rdi, qword [rsp + 48] ; Restore live variable from stack (arr)
        mov     qword r11, qword [rsp + 40] ; Restore live variable from stack (partition)
        mov     qword rdx, qword [rsp + 32] ; Restore live variable from stack (hi)
      ; Out = { arr, partition, hi }
      ; /
    
      ; <>T30 = partition Add 1
      ; In = { arr, partition, hi }
        mov     qword r12, qword r11 ; Assign LHS to target memory
        add     qword r12, qword TRUE
      ; Out = { arr, <>T30, hi }
      ; /
    
      ; _ = Call quickSortCore(arr, <>T30, hi)
      ; In = { arr, <>T30, hi }
        mov     qword [rsp + 48], qword rdi ; Store live variable onto stack (arr)
        mov     qword [rsp + 40], qword r12 ; Store live variable onto stack (<>T30)
        mov     qword [rsp + 32], qword rdx ; Store live variable onto stack (hi)
        mov     qword rdi, qword [rsp + 48] ; Pass parameter #0
        mov     qword rsi, qword [rsp + 40] ; Pass parameter #1
        mov     qword rdx, qword [rsp + 32] ; Pass parameter #2
        call    quickSortCore
      ; Out = {  }
      ; /
    
    .__exit:          ; Function exit/return label
        add     qword rsp, 64 ; Return stack
        ret     
    
partition: ; partition(arr : ptr, lo : long, hi : long) : long
        sub     qword rsp, 80 ; Allocate stack
    
        mov     qword r14, qword rsi
      ; <>T32 = Call arrIndex(arr, hi)
      ; In = { arr, lo, hi }
        mov     qword [rsp + 64], qword rdi ; Store live variable onto stack (arr)
        mov     qword [rsp + 56], qword r14 ; Store live variable onto stack (lo)
        mov     qword [rsp + 48], qword rdx ; Store live variable onto stack (hi)
        mov     qword rdi, qword [rsp + 64] ; Pass parameter #0
        mov     qword rsi, qword [rsp + 48] ; Pass parameter #1
        call    arrIndex
        mov     qword r13, qword rax ; Assign return value to <>T32
        mov     qword rdi, qword [rsp + 64] ; Restore live variable from stack (arr)
        mov     qword r14, qword [rsp + 56] ; Restore live variable from stack (lo)
        mov     qword rdx, qword [rsp + 48] ; Restore live variable from stack (hi)
      ; Out = { arr, lo, <>T32, hi }
      ; /
    
      ; <>T31 = Dereference <>T32
      ; In = { arr, lo, <>T32, hi }
        mov     qword r11, [r13] ; Dereference <>T32
      ; Out = { arr, lo, <>T31, hi }
      ; /
    
      ; _ = pivot Assign <>T31
      ; In = { arr, lo, <>T31, hi }
        mov     qword r10, qword r11
      ; Out = { arr, lo, pivot, hi }
      ; /
    
      ; _ = i Assign lo
      ; In = { arr, lo, pivot, hi }
        mov     qword r13, qword r14
      ; Out = { i, arr, lo, pivot, hi }
      ; /
    
      ; _ = j Assign lo
      ; In = { i, arr, lo, pivot, hi }
        mov     qword r11, qword r14
      ; Out = { i, arr, j, pivot, hi }
      ; /
    
      ; Jump .T34
      ; In = { i, arr, j, pivot, hi }
        jmp     .T34
      ; Out = { i, arr, j, pivot, hi }
      ; /
    
      ; .T33:
      ; In = { i, arr, j, pivot, hi }
    .T33:         
      ; Out = { i, arr, j, pivot, hi }
      ; /
    
      ; <>T36 = PreIncrement j
      ; In = { i, arr, j, pivot, hi }
        inc     qword r11
      ; Out = { i, arr, j, pivot, hi }
      ; /
    
      ; .T34:
      ; In = { i, arr, j, pivot, hi }
    .T34:         
      ; Out = { i, arr, j, pivot, hi }
      ; /
    
      ; <>T37 = j ComparisonLessThanOrEqual hi
      ; In = { i, arr, j, pivot, hi }
        cmp     qword r11, qword rdx ; Set condition codes according to operands
        jle     .CG0 ; Jump to True if the comparison is true
        mov     qword r14, qword FALSE ; Assign false to output
        jmp     .CG1 ; Jump to Exit
    .CG0:          ; True
        mov     qword r14, qword TRUE ; Assign true to output
    .CG1:          ; Exit
      ; Out = { i, arr, j, pivot, hi, <>T37 }
      ; /
    
      ; If !<>T37 Jump .T35
      ; In = { i, arr, j, pivot, hi, <>T37 }
        test    qword r14, qword r14 ; Set condition codes according to condition
        jz      .T35 ; Jump if condition is false/zero
      ; Out = { i, arr, j, pivot, hi }
      ; /
    
      ; <>T39 = Call arrIndex(arr, j)
      ; In = { i, arr, j, pivot, hi }
        mov     qword [rsp + 64], qword r13 ; Store live variable onto stack (i)
        mov     qword [rsp + 56], qword rdi ; Store live variable onto stack (arr)
        mov     qword [rsp + 48], qword r11 ; Store live variable onto stack (j)
        mov     qword [rsp + 40], qword r10 ; Store live variable onto stack (pivot)
        mov     qword [rsp + 32], qword rdx ; Store live variable onto stack (hi)
        mov     qword rdi, qword [rsp + 56] ; Pass parameter #0
        mov     qword rsi, qword [rsp + 48] ; Pass parameter #1
        call    arrIndex
        mov     qword r9, qword rax ; Assign return value to <>T39
        mov     qword r13, qword [rsp + 64] ; Restore live variable from stack (i)
        mov     qword rdi, qword [rsp + 56] ; Restore live variable from stack (arr)
        mov     qword r11, qword [rsp + 48] ; Restore live variable from stack (j)
        mov     qword r10, qword [rsp + 40] ; Restore live variable from stack (pivot)
        mov     qword rdx, qword [rsp + 32] ; Restore live variable from stack (hi)
      ; Out = { i, arr, <>T39, j, pivot, hi }
      ; /
    
      ; <>T38 = Dereference <>T39
      ; In = { i, arr, <>T39, j, pivot, hi }
        mov     qword r14, [r9] ; Dereference <>T39
      ; Out = { i, arr, <>T38, j, pivot, hi }
      ; /
    
      ; _ = elem Assign <>T38
      ; In = { i, arr, <>T38, j, pivot, hi }
        mov     qword r8, qword r14
      ; Out = { i, arr, elem, j, pivot, hi }
      ; /
    
      ; <>T40 = elem ComparisonLessThan pivot
      ; In = { i, arr, elem, j, pivot, hi }
        cmp     qword r8, qword r10 ; Set condition codes according to operands
        jl      .CG2 ; Jump to True if the comparison is true
        mov     qword r9, qword FALSE ; Assign false to output
        jmp     .CG3 ; Jump to Exit
    .CG2:          ; True
        mov     qword r9, qword TRUE ; Assign true to output
    .CG3:          ; Exit
      ; Out = { i, arr, elem, j, <>T40, pivot, hi }
      ; /
    
      ; If <>T40 Jump .T41
      ; In = { i, arr, elem, j, <>T40, pivot, hi }
        test    qword r9, qword r9 ; Set condition codes according to condition
        jnz     .T41 ; Jump if condition is true/non-zero
      ; Out = { i, arr, elem, j, pivot, hi }
      ; /
    
      ; Jump .T42
      ; In = { i, arr, j, pivot, hi }
        jmp     .T42
      ; Out = { i, arr, j, pivot, hi }
      ; /
    
      ; .T41:
      ; In = { i, arr, elem, j, pivot, hi }
    .T41:         
      ; Out = { i, arr, elem, j, pivot, hi }
      ; /
    
      ; <>T43 = Call arrIndex(arr, j)
      ; In = { i, arr, elem, j, pivot, hi }
        mov     qword [rsp + 64], qword r13 ; Store live variable onto stack (i)
        mov     qword [rsp + 56], qword rdi ; Store live variable onto stack (arr)
        mov     qword [rsp + 48], qword r8 ; Store live variable onto stack (elem)
        mov     qword [rsp + 40], qword r11 ; Store live variable onto stack (j)
        mov     qword [rsp + 32], qword r10 ; Store live variable onto stack (pivot)
        mov     qword [rsp + 24], qword rdx ; Store live variable onto stack (hi)
        mov     qword rdi, qword [rsp + 56] ; Pass parameter #0
        mov     qword rsi, qword [rsp + 40] ; Pass parameter #1
        call    arrIndex
        mov     qword r9, qword rax ; Assign return value to <>T43
        mov     qword r13, qword [rsp + 64] ; Restore live variable from stack (i)
        mov     qword rdi, qword [rsp + 56] ; Restore live variable from stack (arr)
        mov     qword r8, qword [rsp + 48] ; Restore live variable from stack (elem)
        mov     qword r11, qword [rsp + 40] ; Restore live variable from stack (j)
        mov     qword r10, qword [rsp + 32] ; Restore live variable from stack (pivot)
        mov     qword rdx, qword [rsp + 24] ; Restore live variable from stack (hi)
      ; Out = { i, arr, elem, <>T43, j, pivot, hi }
      ; /
    
      ; <>T45 = Call arrIndex(arr, i)
      ; In = { i, arr, elem, <>T43, j, pivot, hi }
        mov     qword [rsp + 64], qword r13 ; Store live variable onto stack (i)
        mov     qword [rsp + 56], qword rdi ; Store live variable onto stack (arr)
        mov     qword [rsp + 48], qword r8 ; Store live variable onto stack (elem)
        mov     qword [rsp + 40], qword r9 ; Store live variable onto stack (<>T43)
        mov     qword [rsp + 32], qword r11 ; Store live variable onto stack (j)
        mov     qword [rsp + 24], qword r10 ; Store live variable onto stack (pivot)
        mov     qword [rsp + 16], qword rdx ; Store live variable onto stack (hi)
        mov     qword rdi, qword [rsp + 56] ; Pass parameter #0
        mov     qword rsi, qword [rsp + 64] ; Pass parameter #1
        call    arrIndex
        mov     qword r14, qword rax ; Assign return value to <>T45
        mov     qword r13, qword [rsp + 64] ; Restore live variable from stack (i)
        mov     qword rdi, qword [rsp + 56] ; Restore live variable from stack (arr)
        mov     qword r8, qword [rsp + 48] ; Restore live variable from stack (elem)
        mov     qword r9, qword [rsp + 40] ; Restore live variable from stack (<>T43)
        mov     qword r11, qword [rsp + 32] ; Restore live variable from stack (j)
        mov     qword r10, qword [rsp + 24] ; Restore live variable from stack (pivot)
        mov     qword rdx, qword [rsp + 16] ; Restore live variable from stack (hi)
      ; Out = { i, arr, elem, <>T43, <>T45, j, pivot, hi }
      ; /
    
      ; <>T44 = Dereference <>T45
      ; In = { i, arr, elem, <>T43, <>T45, j, pivot, hi }
        mov     qword rsi, [r14] ; Dereference <>T45
      ; Out = { i, arr, elem, <>T43, <>T44, j, pivot, hi }
      ; /
    
      ; _ = <>T43 ReferenceAssign <>T44
      ; In = { i, arr, elem, <>T43, <>T44, j, pivot, hi }
        mov     qword [r9], qword rsi
      ; Out = { i, arr, elem, j, pivot, hi }
      ; /
    
      ; <>T46 = Call arrIndex(arr, i)
      ; In = { i, arr, elem, j, pivot, hi }
        mov     qword [rsp + 64], qword r13 ; Store live variable onto stack (i)
        mov     qword [rsp + 56], qword rdi ; Store live variable onto stack (arr)
        mov     qword [rsp + 48], qword r8 ; Store live variable onto stack (elem)
        mov     qword [rsp + 40], qword r11 ; Store live variable onto stack (j)
        mov     qword [rsp + 32], qword r10 ; Store live variable onto stack (pivot)
        mov     qword [rsp + 24], qword rdx ; Store live variable onto stack (hi)
        mov     qword rdi, qword [rsp + 56] ; Pass parameter #0
        mov     qword rsi, qword [rsp + 64] ; Pass parameter #1
        call    arrIndex
        mov     qword r14, qword rax ; Assign return value to <>T46
        mov     qword r13, qword [rsp + 64] ; Restore live variable from stack (i)
        mov     qword rdi, qword [rsp + 56] ; Restore live variable from stack (arr)
        mov     qword r8, qword [rsp + 48] ; Restore live variable from stack (elem)
        mov     qword r11, qword [rsp + 40] ; Restore live variable from stack (j)
        mov     qword r10, qword [rsp + 32] ; Restore live variable from stack (pivot)
        mov     qword rdx, qword [rsp + 24] ; Restore live variable from stack (hi)
      ; Out = { i, <>T46, elem, arr, j, pivot, hi }
      ; /
    
      ; _ = <>T46 ReferenceAssign elem
      ; In = { i, <>T46, elem, arr, j, pivot, hi }
        mov     qword [r14], qword r8
      ; Out = { i, arr, j, pivot, hi }
      ; /
    
      ; <>T47 = i Add 1
      ; In = { i, arr, j, pivot, hi }
        mov     qword rsi, qword r13 ; Assign LHS to target memory
        add     qword rsi, qword TRUE
      ; Out = { <>T47, arr, j, pivot, hi }
      ; /
    
      ; _ = i Assign <>T47
      ; In = { <>T47, arr, j, pivot, hi }
        mov     qword r13, qword rsi
      ; Out = { i, arr, j, pivot, hi }
      ; /
    
      ; .T42:
      ; In = { i, arr, j, pivot, hi }
    .T42:         
      ; Out = { i, arr, j, pivot, hi }
      ; /
    
      ; Jump .T33
      ; In = { i, arr, j, pivot, hi }
        jmp     .T33
      ; Out = { i, arr, j, pivot, hi }
      ; /
    
      ; .T35:
      ; In = { i, arr, pivot, hi }
    .T35:         
      ; Out = { i, arr, pivot, hi }
      ; /
    
      ; <>T48 = Call arrIndex(arr, hi)
      ; In = { i, arr, pivot, hi }
        mov     qword [rsp + 64], qword r13 ; Store live variable onto stack (i)
        mov     qword [rsp + 56], qword rdi ; Store live variable onto stack (arr)
        mov     qword [rsp + 48], qword r10 ; Store live variable onto stack (pivot)
        mov     qword [rsp + 40], qword rdx ; Store live variable onto stack (hi)
        mov     qword rdi, qword [rsp + 56] ; Pass parameter #0
        mov     qword rsi, qword [rsp + 40] ; Pass parameter #1
        call    arrIndex
        mov     qword rsi, qword rax ; Assign return value to <>T48
        mov     qword r13, qword [rsp + 64] ; Restore live variable from stack (i)
        mov     qword rdi, qword [rsp + 56] ; Restore live variable from stack (arr)
        mov     qword r10, qword [rsp + 48] ; Restore live variable from stack (pivot)
      ; Out = { i, arr, pivot, <>T48 }
      ; /
    
      ; <>T50 = Call arrIndex(arr, i)
      ; In = { i, arr, pivot, <>T48 }
        mov     qword [rsp + 64], qword r13 ; Store live variable onto stack (i)
        mov     qword [rsp + 56], qword rdi ; Store live variable onto stack (arr)
        mov     qword [rsp + 48], qword r10 ; Store live variable onto stack (pivot)
        mov     qword [rsp + 40], qword rsi ; Store live variable onto stack (<>T48)
        mov     qword rdi, qword [rsp + 56] ; Pass parameter #0
        mov     qword rsi, qword [rsp + 64] ; Pass parameter #1
        call    arrIndex
        mov     qword r14, qword rax ; Assign return value to <>T50
        mov     qword r13, qword [rsp + 64] ; Restore live variable from stack (i)
        mov     qword rdi, qword [rsp + 56] ; Restore live variable from stack (arr)
        mov     qword r10, qword [rsp + 48] ; Restore live variable from stack (pivot)
        mov     qword rsi, qword [rsp + 40] ; Restore live variable from stack (<>T48)
      ; Out = { i, arr, pivot, <>T48, <>T50 }
      ; /
    
      ; <>T49 = Dereference <>T50
      ; In = { i, arr, pivot, <>T48, <>T50 }
        mov     qword rdx, [r14] ; Dereference <>T50
      ; Out = { i, arr, pivot, <>T48, <>T49 }
      ; /
    
      ; _ = <>T48 ReferenceAssign <>T49
      ; In = { i, arr, pivot, <>T48, <>T49 }
        mov     qword [rsi], qword rdx
      ; Out = { i, arr, pivot }
      ; /
    
      ; <>T51 = Call arrIndex(arr, i)
      ; In = { i, arr, pivot }
        mov     qword [rsp + 64], qword r13 ; Store live variable onto stack (i)
        mov     qword [rsp + 56], qword rdi ; Store live variable onto stack (arr)
        mov     qword [rsp + 48], qword r10 ; Store live variable onto stack (pivot)
        mov     qword rdi, qword [rsp + 56] ; Pass parameter #0
        mov     qword rsi, qword [rsp + 64] ; Pass parameter #1
        call    arrIndex
        mov     qword r14, qword rax ; Assign return value to <>T51
        mov     qword r13, qword [rsp + 64] ; Restore live variable from stack (i)
        mov     qword r10, qword [rsp + 48] ; Restore live variable from stack (pivot)
      ; Out = { i, <>T51, pivot }
      ; /
    
      ; _ = <>T51 ReferenceAssign pivot
      ; In = { i, <>T51, pivot }
        mov     qword [r14], qword r10
      ; Out = { i }
      ; /
    
      ; Return i
      ; In = { i }
        mov     qword rax, qword r13 ; Return i
        jmp     .__exit
      ; Out = {  }
      ; /
    
    .__exit:          ; Function exit/return label
        add     qword rsp, 80 ; Return stack
        ret     
    
arrInit: ; arrInit(size : long) : ptr
        sub     qword rsp, 48 ; Allocate stack
    
      ; <>T52 = Call arrAlloc(size)
      ; In = { size }
        mov     qword [rsp + 32], qword rdi ; Store live variable onto stack (size)
        mov     qword rdi, qword [rsp + 32] ; Pass parameter #0
        call    arrAlloc
        mov     qword r15, qword rax ; Assign return value to <>T52
        mov     qword rdi, qword [rsp + 32] ; Restore live variable from stack (size)
      ; Out = { <>T52, size }
      ; /
    
      ; _ = arr Assign <>T52
      ; In = { <>T52, size }
        mov     qword r13, qword r15
      ; Out = { arr, size }
      ; /
    
      ; _ = i Assign 0
      ; In = { arr, size }
        mov     qword r12, qword FALSE
      ; Out = { arr, i, size }
      ; /
    
      ; Jump .T54
      ; In = { arr, i, size }
        jmp     .T54
      ; Out = { arr, i, size }
      ; /
    
      ; .T53:
      ; In = { arr, i, size }
    .T53:         
      ; Out = { arr, i, size }
      ; /
    
      ; <>T56 = PreIncrement i
      ; In = { arr, i, size }
        inc     qword r12
      ; Out = { arr, i, size }
      ; /
    
      ; .T54:
      ; In = { arr, i, size }
    .T54:         
      ; Out = { arr, i, size }
      ; /
    
      ; <>T57 = i ComparisonLessThan size
      ; In = { arr, i, size }
        cmp     qword r12, qword rdi ; Set condition codes according to operands
        jl      .CG0 ; Jump to True if the comparison is true
        mov     qword r15, qword FALSE ; Assign false to output
        jmp     .CG1 ; Jump to Exit
    .CG0:          ; True
        mov     qword r15, qword TRUE ; Assign true to output
    .CG1:          ; Exit
      ; Out = { arr, i, <>T57, size }
      ; /
    
      ; If !<>T57 Jump .T55
      ; In = { arr, i, <>T57, size }
        test    qword r15, qword r15 ; Set condition codes according to condition
        jz      .T55 ; Jump if condition is false/zero
      ; Out = { arr, i, size }
      ; /
    
      ; <>T58 = Call arrIndex(arr, i)
      ; In = { arr, i, size }
        mov     qword [rsp + 32], qword r13 ; Store live variable onto stack (arr)
        mov     qword [rsp + 24], qword r12 ; Store live variable onto stack (i)
        mov     qword [rsp + 16], qword rdi ; Store live variable onto stack (size)
        mov     qword rdi, qword [rsp + 32] ; Pass parameter #0
        mov     qword rsi, qword [rsp + 24] ; Pass parameter #1
        call    arrIndex
        mov     qword r11, qword rax ; Assign return value to <>T58
        mov     qword r13, qword [rsp + 32] ; Restore live variable from stack (arr)
        mov     qword r12, qword [rsp + 24] ; Restore live variable from stack (i)
        mov     qword rdi, qword [rsp + 16] ; Restore live variable from stack (size)
      ; Out = { <>T58, arr, i, size }
      ; /
    
      ; _ = <>T58 ReferenceAssign 0
      ; In = { <>T58, arr, i, size }
        mov     qword [r11], qword FALSE
      ; Out = { arr, i, size }
      ; /
    
      ; Jump .T53
      ; In = { arr, i, size }
        jmp     .T53
      ; Out = { arr, i, size }
      ; /
    
      ; .T55:
      ; In = { arr }
    .T55:         
      ; Out = { arr }
      ; /
    
      ; Return arr
      ; In = { arr }
        mov     qword rax, qword r13 ; Return arr
        jmp     .__exit
      ; Out = {  }
      ; /
    
    .__exit:          ; Function exit/return label
        add     qword rsp, 48 ; Return stack
        ret     
    
arrFree: ; arrFree(arr : ptr) : long
        sub     qword rsp, 32 ; Allocate stack
    
      ; <>T59 = Call free(arr)
      ; In = { arr }
        mov     qword [rsp + 16], qword rdi ; Store live variable onto stack (arr)
        mov     qword rdi, qword [rsp + 16] ; Pass parameter #0
        call    free
        mov     qword r15, qword rax ; Assign return value to <>T59
      ; Out = { <>T59 }
      ; /
    
      ; Return <>T59
      ; In = { <>T59 }
        mov     qword rax, qword r15 ; Return <>T59
        jmp     .__exit
      ; Out = {  }
      ; /
    
    .__exit:          ; Function exit/return label
        add     qword rsp, 32 ; Return stack
        ret     
    
printArr: ; printArr(arr : ptr, size : long) : long
        sub     qword rsp, 48 ; Allocate stack
    
        mov     qword r13, qword rsi
      ; _ = i Assign 0
      ; In = { arr, size }
        mov     qword r14, qword FALSE
      ; Out = { arr, i, size }
      ; /
    
      ; Jump .T61
      ; In = { arr, i, size }
        jmp     .T61
      ; Out = { arr, i, size }
      ; /
    
      ; .T60:
      ; In = { arr, i, size }
    .T60:         
      ; Out = { arr, i, size }
      ; /
    
      ; <>T63 = PreIncrement i
      ; In = { arr, i, size }
        inc     qword r14
      ; Out = { arr, i, size }
      ; /
    
      ; .T61:
      ; In = { arr, i, size }
    .T61:         
      ; Out = { arr, i, size }
      ; /
    
      ; <>T64 = i ComparisonLessThan size
      ; In = { arr, i, size }
        cmp     qword r14, qword r13 ; Set condition codes according to operands
        jl      .CG0 ; Jump to True if the comparison is true
        mov     qword r12, qword FALSE ; Assign false to output
        jmp     .CG1 ; Jump to Exit
    .CG0:          ; True
        mov     qword r12, qword TRUE ; Assign true to output
    .CG1:          ; Exit
      ; Out = { arr, i, <>T64, size }
      ; /
    
      ; If !<>T64 Jump .T62
      ; In = { arr, i, <>T64, size }
        test    qword r12, qword r12 ; Set condition codes according to condition
        jz      .T62 ; Jump if condition is false/zero
      ; Out = { arr, i, size }
      ; /
    
      ; <>T65 = i ComparisonGreaterThan 0
      ; In = { arr, i, size }
        cmp     qword r14, qword FALSE ; Set condition codes according to operands
        jg      .CG2 ; Jump to True if the comparison is true
        mov     qword r11, qword FALSE ; Assign false to output
        jmp     .CG3 ; Jump to Exit
    .CG2:          ; True
        mov     qword r11, qword TRUE ; Assign true to output
    .CG3:          ; Exit
      ; Out = { arr, i, <>T65, size }
      ; /
    
      ; If <>T65 Jump .T66
      ; In = { arr, i, <>T65, size }
        test    qword r11, qword r11 ; Set condition codes according to condition
        jnz     .T66 ; Jump if condition is true/non-zero
      ; Out = { arr, i, size }
      ; /
    
      ; Jump .T67
      ; In = { arr, i, size }
        jmp     .T67
      ; Out = { arr, i, size }
      ; /
    
      ; .T66:
      ; In = { arr, i, size }
    .T66:         
      ; Out = { arr, i, size }
      ; /
    
      ; _ = Call print(, , 2)
      ; In = { arr, i, size }
        mov     qword [rsp + 32], qword rdi ; Store live variable onto stack (arr)
        mov     qword [rsp + 24], qword r14 ; Store live variable onto stack (i)
        mov     qword [rsp + 16], qword r13 ; Store live variable onto stack (size)
        mov     qword rdi, qword ptrC9 ; Pass parameter #0
        mov     qword rsi, qword longC5 ; Pass parameter #1
        call    print
        mov     qword rdi, qword [rsp + 32] ; Restore live variable from stack (arr)
        mov     qword r14, qword [rsp + 24] ; Restore live variable from stack (i)
        mov     qword r13, qword [rsp + 16] ; Restore live variable from stack (size)
      ; Out = { arr, i, size }
      ; /
    
      ; .T67:
      ; In = { arr, i, size }
    .T67:         
      ; Out = { arr, i, size }
      ; /
    
      ; <>T69 = Call arrIndex(arr, i)
      ; In = { arr, i, size }
        mov     qword [rsp + 32], qword rdi ; Store live variable onto stack (arr)
        mov     qword [rsp + 24], qword r14 ; Store live variable onto stack (i)
        mov     qword [rsp + 16], qword r13 ; Store live variable onto stack (size)
        mov     qword rdi, qword [rsp + 32] ; Pass parameter #0
        mov     qword rsi, qword [rsp + 24] ; Pass parameter #1
        call    arrIndex
        mov     qword r11, qword rax ; Assign return value to <>T69
        mov     qword rdi, qword [rsp + 32] ; Restore live variable from stack (arr)
        mov     qword r14, qword [rsp + 24] ; Restore live variable from stack (i)
        mov     qword r13, qword [rsp + 16] ; Restore live variable from stack (size)
      ; Out = { <>T69, arr, i, size }
      ; /
    
      ; <>T68 = Dereference <>T69
      ; In = { <>T69, arr, i, size }
        mov     qword r12, [r11] ; Dereference <>T69
      ; Out = { <>T68, arr, i, size }
      ; /
    
      ; _ = Call printNum(<>T68)
      ; In = { <>T68, arr, i, size }
        mov     qword [rsp + 32], qword r12 ; Store live variable onto stack (<>T68)
        mov     qword [rsp + 24], qword rdi ; Store live variable onto stack (arr)
        mov     qword [rsp + 16], qword r14 ; Store live variable onto stack (i)
        mov     qword [rsp + 8], qword r13 ; Store live variable onto stack (size)
        mov     qword rdi, qword [rsp + 32] ; Pass parameter #0
        call    printNum
        mov     qword rdi, qword [rsp + 24] ; Restore live variable from stack (arr)
        mov     qword r14, qword [rsp + 16] ; Restore live variable from stack (i)
        mov     qword r13, qword [rsp + 8] ; Restore live variable from stack (size)
      ; Out = { arr, i, size }
      ; /
    
      ; Jump .T60
      ; In = { arr, i, size }
        jmp     .T60
      ; Out = { arr, i, size }
      ; /
    
      ; .T62:
      ; In = {  }
    .T62:         
      ; Out = {  }
      ; /
    
    .__exit:          ; Function exit/return label
        add     qword rsp, 48 ; Return stack
        ret     
    
arrAlloc: ; arrAlloc(size : long) : ptr
        sub     qword rsp, 32 ; Allocate stack
    
      ; <>T71 = size Multiply 8
      ; In = { size }
        mov     qword r15, qword rdi ; Assign LHS to target memory
        imul    qword r15, qword longC10
      ; Out = { <>T71 }
      ; /
    
      ; <>T70 = Call malloc(<>T71)
      ; In = { <>T71 }
        mov     qword [rsp + 16], qword r15 ; Store live variable onto stack (<>T71)
        mov     qword rdi, qword [rsp + 16] ; Pass parameter #0
        call    malloc
        mov     qword r13, qword rax ; Assign return value to <>T70
      ; Out = { <>T70 }
      ; /
    
      ; Return <>T70
      ; In = { <>T70 }
        mov     qword rax, qword r13 ; Return <>T70
        jmp     .__exit
      ; Out = {  }
      ; /
    
    .__exit:          ; Function exit/return label
        add     qword rsp, 32 ; Return stack
        ret     
    
arrIndex: ; arrIndex(arr : ptr, index : long) : ptr
        sub     qword rsp, 16 ; Allocate stack
    
        mov     qword r14, qword rsi
      ; <>T73 = arr Add 7
      ; In = { arr, index }
        mov     qword r15, qword rdi ; Assign LHS to target memory
        add     qword r15, qword longC11
      ; Out = { <>T73, index }
      ; /
    
      ; <>T74 = 8 Multiply index
      ; In = { <>T73, index }
        mov     qword r12, qword longC10 ; Assign LHS to target memory
        imul    qword r12, qword r14
      ; Out = { <>T73, <>T74 }
      ; /
    
      ; <>T72 = <>T73 Add <>T74
      ; In = { <>T73, <>T74 }
        mov     qword rdi, qword r15 ; Assign LHS to target memory
        add     qword rdi, qword r12
      ; Out = { <>T72 }
      ; /
    
      ; Return <>T72
      ; In = { <>T72 }
        mov     qword rax, qword rdi ; Return <>T72
        jmp     .__exit
      ; Out = {  }
      ; /
    
    .__exit:          ; Function exit/return label
        add     qword rsp, 16 ; Return stack
        ret     
    
print: ; print(buf : ptr, len : long) : long
        sub     qword rsp, 32 ; Allocate stack
    
        mov     qword r13, qword rsi
      ; <>T75 = Call write(1, buf, len)
      ; In = { buf, len }
        mov     qword [rsp + 16], qword rdi ; Store live variable onto stack (buf)
        mov     qword [rsp + 8], qword r13 ; Store live variable onto stack (len)
        mov     qword rdi, qword TRUE ; Pass parameter #0
        mov     qword rsi, qword [rsp + 16] ; Pass parameter #1
        mov     qword rdx, qword [rsp + 8] ; Pass parameter #2
        call    write
        mov     qword r15, qword rax ; Assign return value to <>T75
      ; Out = { <>T75 }
      ; /
    
      ; Return <>T75
      ; In = { <>T75 }
        mov     qword rax, qword r15 ; Return <>T75
        jmp     .__exit
      ; Out = {  }
      ; /
    
    .__exit:          ; Function exit/return label
        add     qword rsp, 32 ; Return stack
        ret     
    
printDigit: ; printDigit(digit : long) : long
        sub     qword rsp, 32 ; Allocate stack
    
      ; _ = digits Assign 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ
      ; In = { digit }
        mov     qword r15, qword ptrC12
      ; Out = { digits, digit }
      ; /
    
      ; <>T76 = digits Add digit
      ; In = { digits, digit }
        mov     qword r13, qword r15 ; Assign LHS to target memory
        add     qword r13, qword rdi
      ; Out = { <>T76 }
      ; /
    
      ; _ = Call print(<>T76, 1)
      ; In = { <>T76 }
        mov     qword [rsp + 16], qword r13 ; Store live variable onto stack (<>T76)
        mov     qword rdi, qword [rsp + 16] ; Pass parameter #0
        mov     qword rsi, qword TRUE ; Pass parameter #1
        call    print
      ; Out = {  }
      ; /
    
    .__exit:          ; Function exit/return label
        add     qword rsp, 32 ; Return stack
        ret     
    
