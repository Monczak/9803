.include not_main

.org $8000
.begin

jsr reset_box

loop:
jsr move_box
jmp loop
