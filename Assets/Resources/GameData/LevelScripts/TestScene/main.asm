﻿; .include not_main


.org $8000
.begin

loop: jmp loop

;jsr not_main.reset_box
;
;loop:
;jsr not_main.move_box
;jmp loop
