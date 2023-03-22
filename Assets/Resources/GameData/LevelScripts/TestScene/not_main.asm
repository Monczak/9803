.org $ff00
.irq
jsr reset_box
rti

reset_box:
lda #$80
sta $0302
rts

move_box:
lda #1
adc $0302
sta $0302
rts