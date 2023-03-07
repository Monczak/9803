.org $8000
loop:
lda #$fe
adc $0302
sta $0302
jmp loop
