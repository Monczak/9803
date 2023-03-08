.org $ff00
.irq
lda #0
sta $0302
rti

.org $8000
.begin
loop:
lda #1
adc $0302
sta $0302
jmp loop
