loop:
lda #1
adc $0302
sta $0302
jmp loop
