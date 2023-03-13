.include not_main

.org $8000
.begin
.include bogus
loop:
lda #1
adc $0302
sta $0302
jmp loop

.include bogus
