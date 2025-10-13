section .data
    x dd 5, 3, 2, 6, 1, 7, 4
    y dd 0, 10, 1, 9, 2, 8, 5
    array_size equ 7
    result_msg db 'Среднее арифметическое разности: ', 0
    result_msg_len equ $ - result_msg
    newline db 10, 0

section .bss
    differences resd 7
    sum resd 1
    average resd 1
    temp_str resb 32

section .text
    global _start

_start:
    mov rcx, array_size
    mov rsi, 0
    mov rdi, 0
    mov dword [sum], 0

calculate_differences:
    mov eax, [x + rsi*4]
    mov ebx, [y + rsi*4]
    sub eax, ebx
    mov [differences + rdi*4], eax
    add [sum], eax
    inc rsi
    inc rdi
    loop calculate_differences

calculate_average:
    mov eax, [sum]
    mov ebx, array_size
    cdq
    idiv ebx
    mov [average], eax

print_result:
    mov rax, 1
    mov rdi, 1
    mov rsi, result_msg
    mov rdx, result_msg_len
    syscall

    mov eax, [average]
    call int_to_string
    
    mov rax, 1
    mov rdi, 1
    mov rsi, temp_str
    mov rdx, r8
    syscall
    
    mov rax, 1
    mov rdi, 1
    mov rsi, newline
    mov rdx, 1
    syscall

exit:
    mov rax, 60
    mov rdi, 0
    syscall

int_to_string:
    push rax
    push rbx
    push rcx
    push rdx
    
    mov rdi, temp_str
    mov rcx, 0
    
    cmp eax, 0
    jge positive
    
    neg eax
    mov byte [rdi], '-'
    inc rdi
    inc rcx

positive:
    cmp eax, 0
    jne convert_loop
    mov byte [rdi], '0'
    inc rdi
    inc rcx
    jmp string_done

convert_loop:
    cmp eax, 0
    je reverse_string
    
    mov edx, 0
    mov ebx, 10
    div ebx
    
    add edx, '0'
    mov [rdi], dl
    inc rdi
    inc rcx
    
    jmp convert_loop

reverse_string:
    mov rax, temp_str
    cmp byte [rax], '-'
    jne start_reverse
    inc rax
    dec rcx

start_reverse:
    mov rsi, rax
    mov rdi, rax
    add rdi, rcx
    dec rdi

reverse_loop:
    cmp rsi, rdi
    jge string_done
    
    mov al, [rsi]
    mov bl, [rdi]
    mov [rsi], bl
    mov [rdi], al
    
    inc rsi
    dec rdi
    jmp reverse_loop

string_done:
    mov rdi, temp_str
    mov rax, 0
count_chars:
    cmp byte [rdi + rax], 0
    je count_done
    inc rax
    cmp rax, 32
    jl count_chars
count_done:
    mov r8, rax
    
    pop rdx
    pop rcx
    pop rbx
    pop rax
    ret
