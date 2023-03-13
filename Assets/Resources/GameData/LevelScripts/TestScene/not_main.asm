.include main

.org $ff00
.irq
lda #0
sta $0302
bogus
rti
