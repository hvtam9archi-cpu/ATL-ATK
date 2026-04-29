(setq 3DUY-ALERT
       (strcat
	 "LISP Th\\U+1ED1ng kê Block Att, Dynamic (AutoCAD Table) v1.06"
	 "\nTác gi\\U+1EA3: 3Duy"
	 "\nTên l\\U+1EC7nh:"
	 "\n     ATL - Thi\\U+1EBFt l\\U+1EADp"
	 "\n     ATK - Th\\U+1ED1ng kê chi ti\\U+1EBFt Block Att"
	 "\n     AT1 - Th\\U+1ED1ng kê nhanh Block Att"
	 "\n     DY1 - Th\\U+1ED1ng kê nhanh Block Dynamic"
	 "\n     ATDY1 - Th\\U+1ED1ng kê nhanh Block Att-Dynamic"
	 "\n     ATC - \\U+0110\\U+1EBFm s\\U+1ED1 l\\U+01B0\\U+1EE3ng Att (Block Att)"
	 "\n     DYC - \\U+0110\\U+1EBFm s\\U+1ED1 l\\U+01B0\\U+1EE3ng Parameter (Block Dynamic)"
	 "\n     ATDYC - \\U+0110\\U+1EBFm s\\U+1ED1 l\\U+01B0\\U+1EE3ng Att và Parameter (Block Att-Dynamic)"
	 "\n     BLC - \\U+0110\\U+1EBFm s\\U+1ED1 l\\U+01B0\\U+1EE3ng Block"
	 "\n     NDC - \\U+0110\\U+1EBFm n\\U+1ED9i dung Att, Text"
	 "\n     ATKINFO - Link h\\U+01B0\\U+1EDBng d\\U+1EABn"
	 "\n     ?? - B\\U+1EA3ng tên l\\U+1EC7nh"
	 )
      )

(princ (strcat "\n" 3DUY-ALERT))
(defun C:?? () (princ (strcat "\n" 3DUY-ALERT)) (alert 3DUY-ALERT) (princ))
(vl-load-com)

;Khoi tao bien
(defun ND:ATK_setvar ()
  (setq 3DUY-ATK-DEFAULT
	 '(
	   (3DUY-ATK-TBLSTYLE . "0")
	   (3DUY-ATK-TXTSTYLE . "0")
	   (3DUY-ATK-TITLE . "DANH M\\U+1EE4C B\\U+1EA2N V\\U+1EBC")
	   (3DUY-ATK-CAOCHU . "100")
	   (3DUY-ATK-HEI . "360")
	   (3DUY-ATK-AUTO . "1")
	   (3DUY-ATK-SUM . "1")
	   (3DUY-ATK-TAG1 . "%%STT")
	   (3DUY-ATK-TIT1 . "STT")
	   (3DUY-ATK-WID1 . "500")
	   (3DUY-ATK-TAG2 . "TENBV")
	   (3DUY-ATK-TIT2 . "Tên b\\U+1EA3n v\\U+1EBD")
	   (3DUY-ATK-WID2 . "2500")
	   (3DUY-ATK-TAG3 . "MADUAN \"-\" MABV")
	   (3DUY-ATK-TIT3 . "Mã b\\U+1EA3n v\\U+1EBD")
	   (3DUY-ATK-WID3 . "1200")
	   (3DUY-ATK-TAG4 . "\"A3\"")
	   (3DUY-ATK-TIT4 . "Kh\\U+1ED5 gi\\U+1EA5y")
	   (3DUY-ATK-WID4 . "800")
	   (3DUY-ATK-TAG5 . "")
	   (3DUY-ATK-TIT5 . "")
	   (3DUY-ATK-WID5 . "1000")
	   (3DUY-ATK-TAG6 . "")
	   (3DUY-ATK-TIT6 . "")
	   (3DUY-ATK-WID6 . "1000")
	   (3DUY-ATK-TAG7 . "")
	   (3DUY-ATK-TIT7 . "")
	   (3DUY-ATK-WID7 . "1000")
	   (3DUY-ATK-TAG8 . "")
	   (3DUY-ATK-TIT8 . "")
	   (3DUY-ATK-WID8 . "1000")
	   (3DUY-ATK-TAG9 . "")
	   (3DUY-ATK-TIT9 . "")
	   (3DUY-ATK-WID9 . "1000")
	   )
	)
  (setq 3DUY-ATK-SYM (mapcar 'car 3DUY-ATK-DEFAULT))
  (mapcar '(lambda (lst) (if (not (eval (car lst))) (set (car lst) (cdr lst)))) 3DUY-ATK-DEFAULT)
  (or 3DUY-ATK-TOTALCOL (setq 3DUY-ATK-TOTALCOL 4))
  (or 3DUY-ATK-TBLSTYLE-NAME (setq 3DUY-ATK-TBLSTYLE-NAME (getvar "CTABLESTYLE")))
  (or 3DUY-ATK-TXTSTYLE-NAME (setq 3DUY-ATK-TXTSTYLE-NAME (getvar "TEXTSTYLE")))
  (if (tblsearch "BLOCK" "KHUNG TEN A3") (setq 3DUY-ATK-BLK-NAME "KHUNG TEN A3"))
  (setq 3DUY-ATK2-DEFAULT
	 '(
	   (3DUY-ATK-SEL-MAN . "1")
	   (3DUY-ATK-SEL-LAY . "0")
	   (3DUY-ATK-SEL-ALL . "0")
	   (3DUY-ATK-SEL-1BLK . "1")
	   (3DUY-ATK-SEL-NBLK . "0")
	   (3DUY-ATK-SORT-SEL . "0")
	   (3DUY-ATK-SORT-POS . "0")
	   (3DUY-ATK-SORT-POS1 . "0")
	   (3DUY-ATK-SORT-POS2 . "0")
	   (3DUY-ATK-SORT-FUZZ . "100")
	   (3DUY-ATK-SORT-COL . "1")
	   (3DUY-ATK-SORT-COLN . "2")
	   )
	)
  (setq 3DUY-ATK2-SYM (mapcar 'car 3DUY-ATK2-DEFAULT))
  (mapcar '(lambda (lst) (if (not (eval (car lst))) (set (car lst) (cdr lst)))) 3DUY-ATK2-DEFAULT)
  (setq 3DUY-ATK-SORT-POS-LIST (list "Trái -> Ph\\U+1EA3i" "Ph\\U+1EA3i -> Trái" "Trên -> D\\U+01B0\\U+1EDBi" "D\\U+01B0\\U+1EDBi -> Trên"))
  (princ)
  )
(ND:ATK_setvar)

;Huong dan cach nhap Tag
(defun ND:ATK_tips ()
  (alert
    (strcat
      "H\\U+01B0\\U+1EDBng d\\U+1EABn cách nh\\U+1EADp Tag:"
      "\n"
      "\n   - Nh\\U+1EADp tên Tag ho\\U+1EB7c Copy t\\U+1EEB danh sách Tag"
      "\n"
      "\n   - Nh\\U+1EADp %%STT \\U+0111\\U+1EC3 \\U+0111\\U+1ECBnh ngh\\U+0129a s\\U+1ED1 th\\U+1EE9 t\\U+1EF1 t\\U+0103ng d\\U+1EA7n"
      "\n"
      "\n   - Nh\\U+1EADp %%NAME \\U+0111\\U+1EC3 \\U+0111\\U+1ECBnh ngh\\U+0129a tên Block"
      "\n   - Nh\\U+1EADp %%BLK \\U+0111\\U+1EC3 t\\U+1EA1o hình \\U+1EA3nh Block"
      "\n"
      "\n   - Nh\\U+1EADp %%X, %%Y, %%Z \\U+0111\\U+1EC3 \\U+0111\\U+1ECBnh ngh\\U+0129a t\\U+1ECDa \\U+0111\\U+1ED9 X, Y, Z c\\U+1EE7a Block"
      "\n"
      "\n   - Nh\\U+1EADp %%+, %%-, %%*, %%/ \\U+0111\\U+1EC3 th\\U+1EF1c hi\\U+1EC7n phép tính t\\U+1EEB trái qua ph\\U+1EA3i"
      "\n   (không phân bi\\U+1EC7t, không \\U+01B0u tiên * / tr\\U+01B0\\U+1EDBc + - sau)"
      "\n   - Nh\\U+1EADp %%sothuc \\U+0111\\U+1EC3 \\U+0111\\U+1ECBnh ngh\\U+0129a s\\U+1ED1 th\\U+1EF1c"
      "\n   Ví d\\U+1EE5 phép tính v\\U+1EDBi c\\U+1EA3 Tag (bi\\U+1EBFn) và s\\U+1ED1 th\\U+1EF1c (h\\U+1EB1ng s\\U+1ED1):"
      "\n   TAG  %%+  %%2  %%*  %%3.5, t\\U+01B0\\U+01A1ng \\U+0111\\U+01B0\\U+01A1ng: (TAG+2)*3.5"
      "\n"
      "\n   - Nh\\U+1EADp \"noidung\" \\U+0111\\U+1EC3 \\U+0111\\U+1ECBnh ngh\\U+0129a các n\\U+1ED9i dung c\\U+1ED1 \\U+0111\\U+1ECBnh"
      "\n   - \\U+0110\\U+1EC3 k\\U+1EBFt h\\U+1EE3p nhi\\U+1EC1u Tag trong 1 c\\U+1ED9t, \\U+0111\\U+1EB7t d\\U+1EA5u cách gi\\U+1EEFa các tên Tag"
      "\n   Ví d\\U+1EE5: TAG1 TAG2 ho\\U+1EB7c MADUAN \" - \" MABANVE"
      )
    )
  )

;--------------------------------
;      Thong ke Block Att        
;--------------------------------
(defun C:atk ( / ATT CAOCHU COL COLTIT COLWID ELST ELST1 I LST LST_ADD LST_ATT LST_MOD LST_RESULT LST_STR LST_SUM LST_TAG LST_TK PREFIX PT ROWHEI STR STR1 STT% SUFFIX SUM TBLSTYLE TITLE TRIM_LEFT TRIM_RIGHT TXTSTYLE X)
  (if
    (and
      (> (ND:ATK_options) 0)
      (if (and (not 3DUY-ATK-BLK-NAME) (= 3DUY-ATK-SEL-1BLK "1")) (alert "Ch\\U+01B0a khai báo Block Att !") t)
      (if (or (= 3DUY-ATK-SEL-LAY "1") (= 3DUY-ATK-SEL-ALL "1"))
	(progn
	  (setq elst nil)
	  (setq lst (ND:layoutlist))
	  (if (= 3DUY-ATK-SEL-ALL "1") (setq lst (cons "Model" lst)))
	  (foreach layout lst
	    (setq elst1 (vl-remove-if 'listp (mapcar 'cadr (if (ssget "X" (list (cons 0 "INSERT") (cons 410 layout))) (ssnamex (ssget "_P"))))))
	    (if (= 3DUY-ATK-SORT-COL "0")
	      (setq elst1 (ND:ATK_sortXY elst1 3DUY-ATK-SORT-POS1 3DUY-ATK-SORT-POS2 (atof 3DUY-ATK-SORT-FUZZ)))
	      )
	    (setq elst (append elst elst1))
	    )
	  elst
	  )
	(progn
	  (setq elst (vl-remove-if-not 'ND:att_get (vl-remove-if 'listp (mapcar 'cadr (if (ssget (list (cons 0 "INSERT"))) (ssnamex (ssget "_P")))))))
	  (if (= 3DUY-ATK-SORT-POS "1")
	    (setq elst (ND:ATK_sortXY elst 3DUY-ATK-SORT-POS1 3DUY-ATK-SORT-POS2 (atof 3DUY-ATK-SORT-FUZZ)))
	    elst
	    )
	  )
	)
      (if (= 3DUY-ATK-SEL-1BLK "1")
	(setq elst (vl-remove-if-not '(lambda (e) (= (ND:get_blkname e) 3DUY-ATK-BLK-NAME)) elst))
	elst
	)
      (setq pt (getpoint "\nChon diem xuat bang thong ke: "))
      )
    (progn
      (vla-startundomark (vla-get-activedocument (vlax-get-acad-object)))
      (setq tblstyle (if (member 3DUY-ATK-TBLSTYLE-NAME (ND:tablestyle_lst)) 3DUY-ATK-TBLSTYLE-NAME (getvar "CTABLESTYLE")))
      (setq txtstyle (if (member 3DUY-ATK-TXTSTYLE-NAME (ND:table_lst "STYLE")) 3DUY-ATK-TXTSTYLE-NAME (getvar "TEXTSTYLE")))
      (setq title 3DUY-ATK-TITLE)
      (if (<= (setq caochu (atof 3DUY-ATK-CAOCHU)) 0) (setq caochu 1))
      (if (<= (setq rowhei (atof 3DUY-ATK-HEI)) 0) (setq rowhei 1))
      (setq colwid nil)
      (setq coltit nil)
      (repeat (setq i 3DUY-ATK-TOTALCOL)
	(setq colwid (cons (cons (1- i) (if (<= (setq col (atof (eval (read (strcat "3DUY-ATK-WID" (itoa i)))))) 0) 1 col)) colwid))
	(setq coltit (cons (eval (read (strcat "3DUY-ATK-TIT" (itoa i)))) coltit))
	(setq i (1- i))
	)
      (setq stt% "!@#$%^&*(STT)*&^%$#@!")
      (setq lst_tk nil)
      (foreach ent elst
	(setq lst_att (ND:att_get ent))
	(setq lst_add nil)
	(setq col 1)
	(repeat 3DUY-ATK-TOTALCOL
	  (setq lst_tag (ND:ATK_str->list (eval (read (strcat "3DUY-ATK-TAG" (itoa col))))))
	  (setq prefix "")
	  (setq trim_left 0)
	  (setq i 0)
	  (while (and lst_tag (= (type (setq str (nth i lst_tag))) 'str))
	    (setq prefix (strcat prefix str))
	    (setq i (1+ i))
	    (setq trim_left i)
	    )
	  (setq suffix nil)
	  (setq trim_right 0)
	  (setq i 0)
	  (while (and lst_tag (= (type (setq str (nth i (reverse lst_tag)))) 'str))
	    (setq suffix (cons str suffix))
	    (setq i (1+ i))
	    (setq trim_right i)
	    )
	  (if suffix
	    (setq suffix (apply 'strcat suffix))
	    (setq suffix "")
	    )
	  (setq lst_str nil)
	  (foreach tag lst_tag
	    (if (and (listp tag) (setq att (strcase (cdr tag))))
	      (setq str1
		     (cond
		       ((= att "%%STT") stt%)
		       ((= att "%%NAME") (ND:get_blkname ent))
		       ((= att "%%BLK") (strcat "%%BLK" (ND:get_blkname ent)))
		       ((= att "%%X") (rtos (car (ND:ins_pt ent)) 2 (getvar "LUPREC")))
		       ((= att "%%Y") (rtos (cadr (ND:ins_pt ent)) 2 (getvar "LUPREC")))
		       ((= att "%%Z") (rtos (caddr (ND:ins_pt ent)) 2 (getvar "LUPREC")))
		       ((member att (list "%%+" "%%-" "%%*" "%%/")) (read (substr att 3 1)))
		       ((and (wcmatch att "%%*") (ND:strnump (substr att 3))) (atof (substr att 3)))
		       ((assoc att lst_att) (ND:unformat (cdr (assoc att lst_att)) nil))
		       (t nil)
		       ))
	      (setq str1 tag)
	      )
	    (setq lst_str (cons str1 lst_str))
	    )
	  (setq lst_str (reverse lst_str))
	  (if (vl-remove-if '(lambda (x) (= (type x) 'str)) (vl-remove nil lst_str))
	    (progn
	      (repeat trim_left (setq lst_str (cdr lst_str)))
	      (repeat trim_right (setq lst_str (reverse (cdr (reverse lst_str)))))
	      (setq lst_str (mapcar '(lambda (x) (if (= (type x) 'str) (atof x) x)) (vl-remove nil lst_str)))
	      (setq str (strcat prefix (rtos (ND:ATK_math lst_str) 2 (getvar "LUPREC")) suffix))
	      )
	    (setq str (apply 'strcat (vl-remove nil lst_str)))
	    )
	  (setq lst_add (append lst_add (list str)))
	  (setq col (1+ col))
	  )
	(setq lst_tk (append lst_tk (list lst_add)))
	)
      (if (= 3DUY-ATK-SORT-COL "1")
	(setq lst_tk (vl-sort lst_tk '(lambda (a b) (ND:alphanumcompare (nth (atoi 3DUY-ATK-SORT-COLN) a) (nth (atoi 3DUY-ATK-SORT-COLN) b) nil))))
	)
      (if (= 3DUY-ATK-SUM "1")
	(progn
	  (setvar "DIMZIN" 0)
	  (setq lst_sum nil)
	  (repeat (setq i 3DUY-ATK-TOTALCOL)
	    (setq sum (apply '+ (mapcar '(lambda (x) (atof (nth (1- i) x))) lst_tk)))
	    (setq str (vl-string-right-trim "." (vl-string-right-trim "0" (rtos sum 2 8))))
	    (setq lst_sum (cons str lst_sum))
	    (setq i (1- i))
	    )
	  (setq lst_tk (append lst_tk (list lst_sum)))
	  )
	)
      (setq lst_mod nil)
      (repeat (setq i (length lst_tk))
	(setq lst (mapcar '(lambda (x) (ND:str-subst (itoa i) stt% x)) (nth (1- i) lst_tk)))
	(setq lst_mod (cons lst lst_mod))
	(setq i (1- i))
	)
      (setq lst_result (append (list (list title)) (list coltit) lst_mod))
      (if (= 3DUY-ATK-AUTO "1")
	(ND:list->table-AC lst_result caochu (* caochu 3.) nil pt tblstyle txtstyle (getvar "CLAYER"))
	(ND:list->table
	  lst_result
	  (list (cons 0 rowhei))
	  colwid
	  (list (cons 0 caochu))
	  (list (cons 0 acMiddleCenter) (cons 1 acMiddleCenter) (cons 2 (list (cons 0 acMiddleCenter) (cons 1 acMiddleLeft) (cons 2 acMiddleCenter))))
	  nil
	  pt
	  tblstyle
	  txtstyle
	  (getvar "CLAYER")
	  )
	)
      (vla-endundomark (vla-get-activedocument (vlax-get-acad-object)))
      )
    )
  (princ)
  )

;--------------------------------
;         DCL Thiet lap          
;--------------------------------

;Thiet lap
(defun C:atl ( / DCH DCL DIALOG FLAG TITLE)
  (setq title "Th\\U+1ED1ng kê Block Att, Dynamic (AutoCAD Table) v1.06")
  (setq dialog "ATK")
  (setq dcl (vl-filename-mktemp nil nil ".dcl"))
  (cond
    ((not (ND:ATK_dcl dcl dialog title (cond ((> 3DUY-ATK-TOTALCOL 9) 9) ((< 3DUY-ATK-TOTALCOL 1) 1) (t 3DUY-ATK-TOTALCOL))))
     (princ "\nDCL file could not be written.")
     )
    ((<= (setq dch (load_dialog dcl)) 0)
     (princ "\nDCL file could not be loaded.")
     )
    (t
     (setq flag 99)
     (while (> flag 10)
       (if (not (new_dialog dialog dch "" (cond ( *screenpoint* ) ('(-1 -1)))))
	 (progn
	   (princ "\nProgram dialog could not be loaded.")
	   (exit)
	   )
	 )
       (ND:ATK_settile)
       (ND:ATK_modetile)
       (action_tile "3DUY-ATK-AUTO" "(ND:ATK_modetile)")
       (action_tile "3DUY-ATK-BUT-SEL" "(ND:ATK_savevar 11)")
       (action_tile "3DUY-ATK-BUT-ADD" "(ND:ATK_savevar 2)")
       (action_tile "3DUY-ATK-BUT-DEL" "(ND:ATK_savevar 3)")
       (action_tile "3DUY-ATK-BUT-DEFAULT" "(ND:ATK_default) (ND:ATK_savevar 4)")
       (action_tile "accept" "(ND:ATK_savevar 1)")
       (action_tile "cancel" "(done_dialog 0)")
       (action_tile "3DUY-ATK-BUT-COPY" "(ND:ATK_copytag)")
       (action_tile "3DUY-ATK-BUT-SAVE" "(ND:ATK_savesettings)")
       (action_tile "3DUY-ATK-BUT-LOAD" "(ND:ATK_loadsettings) (done_dialog 5)")
       (action_tile "3DUY-ATK-BUT-TIP" "(ND:ATK_tips)")
       (action_tile "3DUY-ATK-INFO" "(C:atkinfo)")
       (setq flag (start_dialog))
       (if (= flag 11) (ND:ATK_selectblock))
       )
     (unload_dialog dch)
     )
    )
  (cond
    ((= flag 2) (setq 3DUY-ATK-TOTALCOL (min (1+ 3DUY-ATK-TOTALCOL) 9)) (C:atl))
    ((= flag 3) (setq 3DUY-ATK-TOTALCOL (max (1- 3DUY-ATK-TOTALCOL) 1)) (C:atl))
    ((= flag 4) (setq 3DUY-ATK-TOTALCOL 4) (C:atl))
    ((= flag 5) (C:atl))
    (t nil)
    )
  (princ)
  )

;Chon Block Att
(defun ND:ATK_selectblock ( / ENT I LST)
  (while (not (setq 3DUY-ATK-TAG-LIST (mapcar 'car (ND:att_get (setq ent (ND:ent_pick "INSERT" "Select Block Att: ")))))))
  (setq 3DUY-ATK-BLK-NAME (ND:get_blkname ent))
  (setq 3DUY-ATK-TAG1 "%%STT")
  (setq i 0)
  (setq lst (vl-remove-if-not '(lambda (x) (wcmatch (vl-symbol-name x) "3DUY-ATK-TAG*")) 3DUY-ATK-SYM))
  (repeat (min (length 3DUY-ATK-TAG-LIST) 8)
    (set (nth (1+ i) lst) (nth i 3DUY-ATK-TAG-LIST))
    (setq i (1+ i))
    )
  )

;Copy Tag
(defun ND:ATK_copytag ()
  (if 3DUY-ATK-TAG-LIST (ND:SetClipBoardText (nth (atoi (get_tile "3DUY-ATK-LISTBOX")) 3DUY-ATK-TAG-LIST)))
  )

;Save settings to .txt
(defun ND:ATK_savesettings ( / file_write)
  (if (setq 3DUY-ATK-PATH (getfiled "Luu cai dat" (if 3DUY-ATK-PATH 3DUY-ATK-PATH "Thong ke Block Att, Dynamic - Settings") "txt" 1))
    (progn
      (setq file_write (open 3DUY-ATK-PATH "W"))
      (if 3DUY-ATK-BLK-NAME (write-line (strcat "3DUY-ATK-BLK-NAME" "\t" 3DUY-ATK-BLK-NAME) file_write))
      (write-line (strcat "3DUY-ATK-TOTALCOL" "\t" (itoa 3DUY-ATK-TOTALCOL)) file_write)
      (write-line (strcat "3DUY-ATK-TBLSTYLE-NAME" "\t" 3DUY-ATK-TBLSTYLE-NAME) file_write)
      (write-line (strcat "3DUY-ATK-TXTSTYLE-NAME" "\t" 3DUY-ATK-TXTSTYLE-NAME) file_write)
      (foreach sym 3DUY-ATK-SYM
	(if (get_tile (vl-symbol-name sym))
	  (write-line (strcat (vl-symbol-name sym) "\t" (get_tile (vl-symbol-name sym))) file_write)
	  )
	)
      (foreach sym 3DUY-ATK2-SYM
	(write-line (strcat (vl-symbol-name sym) "\t" (eval sym)) file_write)
	)
      (close file_write)
      )
    )
  )

;Load settings from .txt
(defun ND:ATK_loadsettings ( / file_open lst str)
  (if (setq 3DUY-ATK-PATH (getfiled "Luu cai dat" (if 3DUY-ATK-PATH 3DUY-ATK-PATH "") "txt" 0))
    (progn
      (setq file_open (open 3DUY-ATK-PATH "R"))
      (while (setq str (read-line file_open))
	(setq lst (ND:str->list str "\t"))
	(set (read (car lst)) (if (cadr lst) (cadr lst) ""))
	)
      (close file_open)
      (setq 3DUY-ATK-TOTALCOL (atoi 3DUY-ATK-TOTALCOL))
      (if (and 3DUY-ATK-BLK-NAME (not (tblsearch "BLOCK" 3DUY-ATK-BLK-NAME)))
	(alert (strcat "Không t\\U+1ED3n t\\U+1EA1i Block: " 3DUY-ATK-BLK-NAME))
	)
      (if (not (member 3DUY-ATK-TBLSTYLE-NAME (ND:tablestyle_lst)))
	(alert (strcat "Không t\\U+1ED3n t\\U+1EA1i Table Style: " 3DUY-ATK-TBLSTYLE-NAME))
	)
      (if (not (member 3DUY-ATK-TXTSTYLE-NAME (ND:table_lst "STYLE")))
	(alert (strcat "Không t\\U+1ED3n t\\U+1EA1i Text Style: " 3DUY-ATK-TXTSTYLE-NAME))
	)
      )
    )
  )

;Tu dong dien theo bien
(defun ND:ATK_settile ( / lst sym)
  (if (and 3DUY-ATK-BLK-NAME (tblsearch "BLOCK" 3DUY-ATK-BLK-NAME))
    (progn
      (set_tile "3DUY-ATK-BLK-NOTE" 3DUY-ATK-BLK-NAME)
      (if (setq 3DUY-ATK-TAG-LIST (ND:att_get-blk 3DUY-ATK-BLK-NAME))
	(progn
	  (start_list "3DUY-ATK-LISTBOX" 3)
	  (mapcar 'add_list 3DUY-ATK-TAG-LIST)
	  (end_list)
	  )
	)
      )
    (set_tile "3DUY-ATK-BLK-NOTE" "Nothing selected")
    )
  
  (setq lst (ND:tablestyle_lst))
  (start_list "3DUY-ATK-TBLSTYLE" 3)
  (mapcar 'add_list lst)
  (end_list)
  (if (member 3DUY-ATK-TBLSTYLE-NAME lst)
    (setq 3DUY-ATK-TBLSTYLE (itoa (vl-position 3DUY-ATK-TBLSTYLE-NAME lst)))
    )
  
  (setq lst (ND:table_lst "STYLE"))
  (start_list "3DUY-ATK-TXTSTYLE" 3)
  (mapcar 'add_list lst)
  (end_list)
  (if (member 3DUY-ATK-TXTSTYLE-NAME lst)
    (setq 3DUY-ATK-TXTSTYLE (itoa (vl-position 3DUY-ATK-TXTSTYLE-NAME lst)))
    )
  
  (mapcar '(lambda (sym) (set_tile (vl-symbol-name sym) (eval sym))) 3DUY-ATK-SYM)
  )

;Bat/tat tile
(defun ND:ATK_modetile ( / lst)
  (setq lst (list "3DUY-ATK-HEI" "3DUY-ATK-WID1" "3DUY-ATK-WID2" "3DUY-ATK-WID3" "3DUY-ATK-WID4" "3DUY-ATK-WID5" "3DUY-ATK-WID6" "3DUY-ATK-WID7" "3DUY-ATK-WID8" "3DUY-ATK-WID9"))
  (if (= (get_tile "3DUY-ATK-AUTO") "1")
    (mapcar '(lambda (x) (mode_tile x 1)) lst)
    (mapcar '(lambda (x) (mode_tile x 0)) lst)
    )
  )

;Khoi phuc bien mac dinh
(defun ND:ATK_default ()
  (mapcar '(lambda (lst) (set_tile (vl-symbol-name (car lst)) (cdr lst))) 3DUY-ATK-DEFAULT)
  )

;Luu gia tri dien vao bien
(defun ND:ATK_savevar (flag / sym)
  (mapcar '(lambda (sym) (if (get_tile (vl-symbol-name sym)) (set sym (get_tile (vl-symbol-name sym))))) 3DUY-ATK-SYM)
  (setq 3DUY-ATK-TBLSTYLE-NAME (nth (atoi 3DUY-ATK-TBLSTYLE) (ND:tablestyle_lst)))
  (setq 3DUY-ATK-TXTSTYLE-NAME (nth (atoi 3DUY-ATK-TXTSTYLE) (ND:table_lst "STYLE")))
  (setq *screenpoint* (done_dialog flag))
  )

;Tao file DCL
(defun ND:ATK_dcl (dcl dialog title col / des i)
  (setq des (open dcl "w"))
  (write-line
    (strcat dialog ":"
	    "\ndialog"
	    "\n\t{"
	    (strcat "\n\tlabel = \"" title "\";")
	    ) des)
  (foreach x
	   '(
	     "	: column"
	     "		{"
	     "		: row"
	     "			{"
	     "			: boxed_column"
	     "				{"
	     "				label = \"Khai báo Block\";"
	     "				: button { key = \"3DUY-ATK-BUT-SEL\"; label = \"Ch\\U+1ECDn Block Att\"; width = 20; }"
	     "				: text { key = \"3DUY-ATK-BLK-NOTE\"; value = \"Nothing selected\"; }"
	     "				spacer_1;"
	     "				}"
	     "			: boxed_column"
	     "				{"
	     "				label = \"Danh sách Tag\";"
	     "				: row"
	     "					{"
	     "					: button { key = \"3DUY-ATK-BUT-COPY\"; label = \"Copy\"; alignment = top; }"
	     "					: list_box { key = \"3DUY-ATK-LISTBOX\"; multiple_select = false; height = 7; width = 20; }"
	     "					}"
	     "				}"
	     "			: boxed_column"
	     "				{"
	     "				label = \"Cài \\U+0111\\U+1EB7t s\\U+1EB5n\";"
	     "				fixed_width = true;"
	     "				: button { key = \"3DUY-ATK-BUT-SAVE\"; label = \"Save\"; }"
	     "				: button { key = \"3DUY-ATK-BUT-LOAD\"; label = \"Load\"; }"
	     "				: button { key = \"3DUY-ATK-BUT-DEFAULT\"; label = \"Default\"; }"
	     "				}"
	     "			}"
	     "		: boxed_row"
	     "			{"
	     "			label = \"Thi\\U+1EBFt l\\U+1EADp b\\U+1EA3ng\";"
	     "			: column"
	     "				{"
	     "				: popup_list { key = \"3DUY-ATK-TBLSTYLE\"; label = \"Table Style\"; edit_width = 29.5; }"
	     "				: popup_list { key = \"3DUY-ATK-TXTSTYLE\"; label = \"Text Style\"; edit_width = 29.5; }"
	     "				: edit_box { key = \"3DUY-ATK-TITLE\"; label = \"Tiêu \\U+0111\\U+1EC1\"; edit_width = 30; }"
	     "				}"
	     "			spacer_1;"
	     "			: column"
	     "				{"
	     "				fixed_width = true;"
	     "				width = 25;"
	     "				: edit_box { key = \"3DUY-ATK-CAOCHU\"; label = \"Cao ch\\U+1EEF\"; edit_width = 5; }"
	     "				: edit_box { key = \"3DUY-ATK-HEI\"; label = \"Cao hàng\"; edit_width = 5; }"
	     "				: toggle { key = \"3DUY-ATK-AUTO\"; label = \"Hàng c\\U+1ED9t t\\U+1EF1 \\U+0111\\U+1ED9ng\"; }"
	     "				: toggle { key = \"3DUY-ATK-SUM\"; label = \"Thêm hàng tính t\\U+1ED5ng\"; }"
	     "				}"
	     "			}"
	     "		: boxed_column"
	     "			{"
	     "			label = \"N\\U+1ED9i dung c\\U+1ED9t\";"
	     "			: row"
	     "				{"
	     "				: button { key = \"3DUY-ATK-BUT-TIP\"; label = \"H\\U+01B0\\U+1EDBng d\\U+1EABn cách nh\\U+1EADp Tag\"; width = 29; }"
	     "				: button { key = \"3DUY-ATK-BUT-ADD\"; label = \"Thêm c\\U+1ED9t\"; }"
	     "				: button { key = \"3DUY-ATK-BUT-DEL\"; label = \"B\\U+1EDBt c\\U+1ED9t\"; }"
	     "				}"
	     )
    (write-line x des)
    )

  (setq i 0)
  (repeat col
    (setq i (1+ i))
    (foreach x
	     (list
	       "		: row"
	       "			{"
	       (strcat "		: edit_box { key = \"3DUY-ATK-TAG" (itoa i) "\"; label = \"C\\U+1ED9t " (itoa i) ": Tag\"; edit_width = 15; }")
	       "			spacer_1;"
	       (strcat "		: edit_box { key = \"3DUY-ATK-TIT" (itoa i) "\"; label = \"Tên c\\U+1ED9t\"; edit_width = 15; }")
	       "			spacer_1;"
	       (strcat "		: edit_box { key = \"3DUY-ATK-WID" (itoa i) "\"; label = \"R\\U+1ED9ng c\\U+1ED9t\"; edit_width = 5; }")
	       "			}"
	       )
      (write-line x des)
      )
    )

  (foreach x
	   '(
	     "			}"
	     "		: boxed_column"
	     "			{"
	     "			label = \"Tác gi\\U+1EA3\";"
	     "			: text { key = \"3DUY-TACGIA\"; value = \"3Duy  |  Phone: 0922161194  |  Email: 3Duy3Duy@gmail.com\" ; }"
	     "			}"
	     "		: row"
	     "			{"
	     "			: spacer { width = 3; }"
	     "			: button { key = \"accept\"; label = \"OK\"; is_default = true; fixed_width = true ; width = 11 ; }"
	     "			: button { key = \"cancel\"; label = \"Cancel\"; is_default = false; is_cancel = true; fixed_width = true ; width = 11 ; }"
	     "			: button { key = \"3DUY-ATK-INFO\"; label = \"Info\"; fixed_width = true; width = 11 ; }"
	     "			: spacer { width = 3; }"
	     "			}"
	     "		}"
	     )
    (write-line x des)
    )
  
  (write-line "\t}" des)
  (setq des (close des))
  (while (not (findfile dcl)))
  dcl
  )

;--------------------------------
;     DCL Thong ke Options       
;--------------------------------

;Mo hop thoai
(defun ND:ATK_options ( / DCH DCL DIALOG FLAG TITLE)
  (setq title "Thi\\U+1EBFt l\\U+1EADp ch\\U+1ECDn, s\\U+1EAFp x\\U+1EBFp Block Att")
  (setq dialog "ATK2")
  (setq dcl (vl-filename-mktemp nil nil ".dcl"))
  (setq flag 0)
  (cond
    ((not (ND:ATK2_dcl dcl dialog title))
     (princ "\nDCL file could not be written.")
     )
    ((<= (setq dch (load_dialog dcl)) 0)
     (princ "\nDCL file could not be loaded.")
     )
    ((not (new_dialog dialog dch))
     (princ "\nProgram dialog could not be loaded.")
     )
    (t
     (ND:ATK2_settile)
     (ND:ATK2_button-pos)
     (ND:ATK2_modetile 0)
     (action_tile "3DUY-ATK-SORT-SEL" "(ND:ATK2_modetile 1)")
     (action_tile "3DUY-ATK-SORT-POS" "(ND:ATK2_modetile 2)")
     (action_tile "3DUY-ATK-SORT-COL" "(ND:ATK2_modetile 3)")
     (action_tile "3DUY-ATK-SORT-POS1" "(ND:ATK2_button-pos)")
     (action_tile "accept" "(ND:ATK2_savevar 1)")
     (action_tile "cancel" "(done_dialog)")
     (setq flag (start_dialog))
     (unload_dialog dch)
     )
    )
  flag
  )

;Tu dong dien theo bien
(defun ND:ATK2_settile ( / i lst_col sym)
  (start_list "3DUY-ATK-SORT-POS1" 3)
  (mapcar 'add_list 3DUY-ATK-SORT-POS-LIST)
  (end_list)
  (start_list "3DUY-ATK-SORT-POS2" 3)
  (mapcar 'add_list 3DUY-ATK-SORT-POS-LIST)
  (end_list)
  (repeat (setq i 3DUY-ATK-TOTALCOL)
    (setq lst_col (cons (strcat (itoa i) " - " (eval (read (strcat "3DUY-ATK-TIT" (itoa i))))) lst_col))
    (setq i (1- i))
    )
  (start_list "3DUY-ATK-SORT-COLN" 3)
  (mapcar 'add_list lst_col)
  (end_list)
  (mapcar '(lambda (sym) (set_tile (vl-symbol-name sym) (eval sym))) 3DUY-ATK2-SYM)
  )

;Tu dong dien theo bien (pos2)
(defun ND:ATK2_button-pos ( / lst pos2)
  (setq pos2 (get_tile "3DUY-ATK-SORT-POS2"))
  (if (wcmatch (get_tile "3DUY-ATK-SORT-POS1") "0,1")
    (setq lst (cddr 3DUY-ATK-SORT-POS-LIST))
    (setq lst (list (car 3DUY-ATK-SORT-POS-LIST) (cadr 3DUY-ATK-SORT-POS-LIST)))
    )
  (start_list "3DUY-ATK-SORT-POS2" 3)
  (mapcar 'add_list lst)
  (end_list)
  (set_tile "3DUY-ATK-SORT-POS2" pos2)
  )

;Bat/tat tile
(defun ND:ATK2_modetile (button / col pos)
  (cond
    ((= button 1) (set_tile "3DUY-ATK-SORT-POS" "0") (set_tile "3DUY-ATK-SORT-COL" "0"))
    ((= button 2) (set_tile "3DUY-ATK-SORT-SEL" "0") (set_tile "3DUY-ATK-SORT-COL" "0"))
    ((= button 3) (set_tile "3DUY-ATK-SORT-SEL" "0") (set_tile "3DUY-ATK-SORT-POS" "0"))
    (t nil)
    )
  (setq pos (- 1 (atoi (get_tile "3DUY-ATK-SORT-POS"))))
  (mode_tile "3DUY-ATK-SORT-POS1" pos)
  (mode_tile "3DUY-ATK-SORT-POS2" pos)
  (mode_tile "3DUY-ATK-SORT-FUZZ" pos)
  (setq col (- 1 (atoi (get_tile "3DUY-ATK-SORT-COL"))))
  (mode_tile "3DUY-ATK-SORT-COLN" col)
  )

;Luu gia tri dien vao bien
(defun ND:ATK2_savevar (flag / sym)
  (mapcar '(lambda (sym) (if (get_tile (vl-symbol-name sym)) (set sym (get_tile (vl-symbol-name sym))))) 3DUY-ATK2-SYM)
  (done_dialog flag)
  )

;Tao file DCL
(defun ND:ATK2_dcl (dcl dialog title / des i)
  (setq des (open dcl "w"))
  (write-line
    (strcat dialog ":"
	    "\ndialog"
	    "\n\t{"
	    (strcat "\n\tlabel = \"" title "\";")
	    ) des)
  (foreach x
	   '(
	     "	: column"
	     "		{"
	     "		: row"
	     "			{"
	     "			: column"
	     "				{"
	     "				: boxed_column"
	     "					{"
	     "					label = \"Ph\\U+01B0\\U+01A1ng th\\U+1EE9c ch\\U+1ECDn Block\";"
	     "					spacer_0;"
	     "					: radio_button { key = \"3DUY-ATK-SEL-MAN\"; label = \"Ch\\U+1ECDn th\\U+1EE7 công\"; }"
	     "					: radio_button { key = \"3DUY-ATK-SEL-LAY\"; label = \"T\\U+1EA5t c\\U+1EA3 Layout\"; }"
	     "					: radio_button { key = \"3DUY-ATK-SEL-ALL\"; label = \"T\\U+1EA5t c\\U+1EA3 Layout và Model\"; }"
	     "					spacer_0;"
	     "					}"
	     "				: boxed_column"
	     "					{"
	     "					label = \"Th\\U+1ED1ng kê Block Att\";"
	     "					spacer_0;"
	     "					: radio_button { key = \"3DUY-ATK-SEL-1BLK\"; label = \"Ch\\U+1EC9 th\\U+1ED1ng kê 1 lo\\U+1EA1i Block Att \\U+0111ã khai báo\"; }"
	     "					: radio_button { key = \"3DUY-ATK-SEL-NBLK\"; label = \"T\\U+1EA5t c\\U+1EA3 Block Att \\U+0111\\U+01B0\\U+1EE3c ch\\U+1ECDn\"; }"
	     "					spacer_0;"
	     "					}"
	     "				}"
	     "			: boxed_column"
	     "				{"
	     "				label = \"Th\\U+1EE9 t\\U+1EF1 s\\U+1EAFp x\\U+1EBFp\";"
	     "				: radio_button { key = \"3DUY-ATK-SORT-SEL\"; label = \"Theo th\\U+1EE9 t\\U+1EF1 ch\\U+1ECDn\"; }"
	     "				: radio_button { key = \"3DUY-ATK-SORT-POS\"; label = \"Theo v\\U+1ECB trí Block\"; }"
	     "				: popup_list { key = \"3DUY-ATK-SORT-POS1\"; label = \"Th\\U+1EE9 t\\U+1EF1 trong 1 nhóm\"; edit_width = 16.5; }"
	     "				: popup_list { key = \"3DUY-ATK-SORT-POS2\"; label = \"Th\\U+1EE9 t\\U+1EF1 các nhóm\"; edit_width = 16.5; }"
	     "				: edit_box { key = \"3DUY-ATK-SORT-FUZZ\"; label = \"Sai s\\U+1ED1 kho\\U+1EA3ng cách\"; edit_width = 17; }"
	     "				: radio_button { key = \"3DUY-ATK-SORT-COL\"; label = \"Theo c\\U+1ED9t\"; }"
	     "				: popup_list { key = \"3DUY-ATK-SORT-COLN\"; label = \"C\\U+1ED9t\"; edit_width = 16.5; }"
	     "				}"
	     "			}"
	     "		ok_cancel;"
	     "		}"
	     )
    (write-line x des)
    )
  (write-line "\t}" des)
  (setq des (close des))
  (while (not (findfile dcl)))
  dcl
  )

;--------------------------------
;    Cac lenh thong ke nhanh     
;--------------------------------

;Thong ke Block Attribute va Dynamic
(defun C:atdy1 ( / E ELST ENT I LST LST_ADD LST_ATT LST_ATT1 LST_DYN LST_DYN1 LST_MOD LST_NAME LST_RESULT LST_SUM LST_TK NAME PT STR SUM X)
  (if
    (and
      (setq elst (vl-remove-if 'listp (mapcar 'cadr (if (ssget (list (cons 0 "INSERT"))) (ssnamex (ssget "_P"))))))
      (setq elst (vl-sort elst '(lambda (a b) (< (ND:get_blkname a) (ND:get_blkname b)))))
      (setq pt (getpoint "\nChon diem xuat bang thong ke: "))
      )
    (progn
      (vla-startundomark (vla-get-activedocument (vlax-get-acad-object)))
      (setq lst_name (vl-sort (ND:unique (mapcar '(lambda (e) (ND:get_blkname e)) elst)) '<))
      (setq lst_att (ND:unique (apply 'append (mapcar 'ND:att_get-blk lst_name))))
      (setq lst_dyn (vl-remove "Origin" (vl-sort (ND:unique (apply 'append (mapcar '(lambda (ent) (mapcar 'car (ND:dyn_get-visible ent))) elst))) '<)))
      (setq lst_tk nil)
      (setq i 0)
      (foreach ent elst
	(setq name (ND:get_blkname ent))
	(setq lst_add nil)
	(setq lst_att1 (ND:att_get ent))
	(foreach att lst_att
	  (if (assoc att lst_att1)
	    (setq lst_add (append lst_add (list (ND:unformat (cdr (assoc att lst_att1)) nil))))
	    (setq lst_add (append lst_add (list "")))
	    )
	  )
	(setq lst_dyn1 (ND:dyn_get-visible ent))
	(foreach dyn lst_dyn
	  (if (assoc dyn lst_dyn1)
	    (setq lst_add (append lst_add (list (cdr (assoc dyn lst_dyn1)))))
	    (setq lst_add (append lst_add (list "")))
	    )
	  )
	(setq lst_add (append (list (itoa (1+ i)) name) lst_add))
	(setq lst_tk (append lst_tk (list lst_add)))
	(setq i (1+ i))
	)
      (if (= 3DUY-ATK-SUM "1")
	(progn
	  (setvar "DIMZIN" 0)
	  (setq lst_sum nil)
	  (repeat (setq i (length (car lst_tk)))
	    (setq sum (apply '+ (mapcar '(lambda (x) (if (numberp (nth (1- i) x)) (nth (1- i) x) (atof (nth (1- i) x)))) lst_tk)))
	    (setq str (vl-string-right-trim "." (vl-string-right-trim "0" (rtos sum 2 (getvar "LUPREC")))))
	    (setq lst_sum (cons str lst_sum))
	    (setq i (1- i))
	    )
	  (setq lst_sum (cons "T\\U+1ED4NG" (cdr lst_sum)))
	  (setq lst_tk (append lst_tk (list lst_sum)))
	  )
	)
      (setq lst_mod nil)
      (repeat (setq i (length lst_tk))
	(setq lst (mapcar '(lambda (x) (if (numberp x) (rtos x 2 (getvar "LUPREC")) x)) (nth (1- i) lst_tk)))
	(setq lst_mod (cons lst lst_mod))
	(setq i (1- i))
	)
      (setq lst_result (append (list (list "B\\U+1EA2NG TH\\U+1ED0NG KÊ BLOCK ATTRIBUTE-DYNAMIC") (append (list "STT" "Tên Block") (append lst_att lst_dyn))) lst_mod))
      (ND:list->table-AC lst_result (atof 3DUY-ATK-CAOCHU) (* (atof 3DUY-ATK-CAOCHU) 3) nil pt 3DUY-ATK-TBLSTYLE-NAME 3DUY-ATK-TXTSTYLE-NAME (getvar "CLAYER"))
      (vla-endundomark (vla-get-activedocument (vlax-get-acad-object)))
      )
    )
  (princ)
  )

;Thong ke Block Attribute
(defun C:at1 ( / E ELST I LST_ADD LST_ATT LST_ATT1 LST_NAME LST_RESULT LST_SUM LST_TK NAME PT STR SUM X)
  (if
    (and
      (setq elst (vl-remove-if-not 'ND:att_get (vl-remove-if 'listp (mapcar 'cadr (if (ssget (list (cons 0 "INSERT"))) (ssnamex (ssget "_P")))))))
      (setq elst (vl-sort elst '(lambda (a b) (< (ND:get_blkname a) (ND:get_blkname b)))))
      (setq pt (getpoint "\nChon diem xuat bang thong ke: "))
      )
    (progn
      (vla-startundomark (vla-get-activedocument (vlax-get-acad-object)))
      (setq lst_name (vl-sort (ND:unique (mapcar '(lambda (e) (ND:get_blkname e)) elst)) '<))
      (setq lst_att (ND:unique (apply 'append (mapcar 'ND:att_get-blk lst_name))))
      (setq lst_tk nil)
      (setq i 0)
      (foreach ent elst
	(setq name (ND:get_blkname ent))
	(setq lst_att1 (ND:att_get ent))
	(setq lst_add nil)
	(foreach att lst_att
	  (if (assoc att lst_att1)
	    (setq lst_add (append lst_add (list (ND:unformat (cdr (assoc att lst_att1)) nil))))
	    (setq lst_add (append lst_add (list "")))
	    )
	  )
	(setq lst_add (append (list (itoa (1+ i)) name) lst_add))
	(setq lst_tk (append lst_tk (list lst_add)))
	(setq i (1+ i))
	)
      (if (= 3DUY-ATK-SUM "1")
	(progn
	  (setvar "DIMZIN" 0)
	  (setq lst_sum nil)
	  (repeat (setq i (length (car lst_tk)))
	    (setq sum (apply '+ (mapcar '(lambda (x) (atof (nth (1- i) x))) lst_tk)))
	    (setq str (vl-string-right-trim "." (vl-string-right-trim "0" (rtos sum 2 (getvar "LUPREC")))))
	    (setq lst_sum (cons str lst_sum))
	    (setq i (1- i))
	    )
	  (setq lst_sum (cons "T\\U+1ED4NG" (cdr lst_sum)))
	  (setq lst_tk (append lst_tk (list lst_sum)))
	  )
	)
      (setq lst_result (append (list (list "B\\U+1EA2NG TH\\U+1ED0NG KÊ BLOCK ATTRIBUTE") (append (list "STT" "Tên Block") lst_att)) lst_tk))
      (ND:list->table-AC lst_result (atof 3DUY-ATK-CAOCHU) (* (atof 3DUY-ATK-CAOCHU) 3) nil pt 3DUY-ATK-TBLSTYLE-NAME 3DUY-ATK-TXTSTYLE-NAME (getvar "CLAYER"))
      (vla-endundomark (vla-get-activedocument (vlax-get-acad-object)))
      )
    )
  (princ)
  )

;Thong ke Block Dynamic
(defun C:dy1 ( / ELST ENT I LST LST_ADD LST_DYN LST_DYN1 LST_MOD LST_RESULT LST_SUM LST_TK NAME PT STR SUM X)
  (if
    (and
      (setq elst (vl-remove-if-not 'ND:dyn_get (vl-remove-if 'listp (mapcar 'cadr (if (ssget (list (cons 0 "INSERT"))) (ssnamex (ssget "_P")))))))
      (setq pt (getpoint "\nChon diem xuat bang thong ke: "))
      )
    (progn
      (vla-startundomark (vla-get-activedocument (vlax-get-acad-object)))
      (setq lst_dyn (vl-remove "Origin" (vl-sort (ND:unique (apply 'append (mapcar '(lambda (ent) (mapcar 'car (ND:dyn_get-visible ent))) elst))) '<)))
      (setq lst_tk nil)
      (setq i 0)
      (foreach ent elst
	(setq name (ND:get_blkname ent))
	(setq lst_dyn1 (ND:dyn_get-visible ent))
	(setq lst_add nil)
	(foreach dyn lst_dyn
	  (if (assoc dyn lst_dyn1)
	    (setq lst_add (append lst_add (list (cdr (assoc dyn lst_dyn1)))))
	    (setq lst_add (append lst_add (list "")))
	    )
	  )
	(setq lst_add (append (list (itoa (1+ i)) name) lst_add))
	(setq lst_tk (append lst_tk (list lst_add)))
	(setq i (1+ i))
	)
      (if (= 3DUY-ATK-SUM "1")
	(progn
	  (setvar "DIMZIN" 0)
	  (setq lst_sum nil)
	  (repeat (setq i (length (car lst_tk)))
	    (setq sum (apply '+ (mapcar '(lambda (x) (if (numberp (nth (1- i) x)) (nth (1- i) x) (atof (nth (1- i) x)))) lst_tk)))
	    (setq str (vl-string-right-trim "." (vl-string-right-trim "0" (rtos sum 2 (getvar "LUPREC")))))
	    (setq lst_sum (cons str lst_sum))
	    (setq i (1- i))
	    )
	  (setq lst_sum (cons "T\\U+1ED4NG" (cdr lst_sum)))
	  (setq lst_tk (append lst_tk (list lst_sum)))
	  )
	)
      (setq lst_mod nil)
      (repeat (setq i (length lst_tk))
	(setq lst (mapcar '(lambda (x) (if (numberp x) (rtos x 2 (getvar "LUPREC")) x)) (nth (1- i) lst_tk)))
	(setq lst_mod (cons lst lst_mod))
	(setq i (1- i))
	)
      (setq lst_result (append (list (list "B\\U+1EA2NG TH\\U+1ED0NG KÊ BLOCK DYNAMIC") (append (list "STT" "Tên Block") lst_dyn)) lst_mod))
      (ND:list->table-AC lst_result (atof 3DUY-ATK-CAOCHU) (* (atof 3DUY-ATK-CAOCHU) 3) nil pt 3DUY-ATK-TBLSTYLE-NAME 3DUY-ATK-TXTSTYLE-NAME (getvar "CLAYER"))
      (vla-endundomark (vla-get-activedocument (vlax-get-acad-object)))
      )
    )
  (princ)
  )

;Dem Att va Parameter
(defun C:atdyc ( / A B E ELST ENT I LST LST_ADD LST_ATT LST_COUNT LST_DYN LST_MOD LST_NAME LST_PARAM LST_RESULT LST_SUM LST_TAG LST_TK PT STR SUM VAL X)
  (if
    (and
      (setq elst (vl-remove-if 'listp (mapcar 'cadr (if (ssget (list (cons 0 "INSERT"))) (ssnamex (ssget "_P"))))))
      (setq lst_name (vl-sort (ND:unique (mapcar '(lambda (e) (ND:get_blkname e)) elst)) '<))
      (progn
	(setq lst_tag (ND:unique (apply 'append (mapcar 'ND:att_get-blk lst_name))))
	(setq lst_param (vl-remove "Origin" (vl-sort (ND:unique (apply 'append (mapcar '(lambda (ent) (mapcar 'car (ND:dyn_get-visible ent))) elst))) '<)))
	(if (> (length lst_tag) 1)
	  (setq lst_tag (ND:listbox lst_tag "Ch\\U+1ECDn 1 ho\\U+1EB7c nhi\\U+1EC1u Att" 10 16 1))
	  )
	(if (> (length lst_param) 1)
	  (setq lst_param (ND:listbox lst_param "Ch\\U+1ECDn 1 ho\\U+1EB7c nhi\\U+1EC1u Parameter" 10 16 1))
	  )
	(or lst_tag lst_param)
	)
      (setq pt (getpoint "\nChon diem xuat bang thong ke: "))
      )
    (progn
      (vla-startundomark (vla-get-activedocument (vlax-get-acad-object)))
      (setq elst (vl-sort elst '(lambda (a b) (ND:alphanumcompare (ND:get_blkname a) (ND:get_blkname b) t))))
      (setq lst_count nil)
      (foreach ent elst
	(setq lst_add nil)
	(setq lst_att (ND:att_get ent))
	(foreach tag lst_tag
	  (setq str (if (assoc tag lst_att) (ND:unformat (cdr (assoc tag lst_att)) nil) ""))
	  (setq lst_add (append lst_add (list str)))
	  )
	(setq lst_dyn (ND:dyn_get-visible ent))
	(foreach param lst_param
	  (if (setq val (cdr (assoc param lst_dyn)))
	    (if (numberp val)
	      (setq val (* 0.0001 (atoi (rtos (/ val 0.0001) 2 0))))
	      )
	    (setq val "")
	    )
	  (setq lst_add (append lst_add (list val)))
	  )
	(setq lst_count (ND:assoc+ lst_add lst_count))
	)
      (setq lst_tk nil)
      (repeat (setq i (length lst_count))
	(setq lst (nth (1- i) lst_count))
	(setq lst_add (append (list (itoa i)) (car lst) (list (itoa (cdr lst)))))
	(setq lst_tk (cons lst_add lst_tk))
	(setq i (1- i))
	)
      (if (= 3DUY-ATK-SUM "1")
	(progn
	  (setvar "DIMZIN" 0)
	  (setq lst_sum nil)
	  (repeat (setq i (length (car lst_tk)))
	    (setq sum (apply '+ (mapcar '(lambda (x / a b)
					   (if (not (numberp (setq a (nth (1- i) x)))) (setq a (atof a)))
					   (setq b (if (= i (length (car lst_tk))) 1 (atof (last x))))
					   (* a b)
					   )
					lst_tk
					)))
	    (setq str (vl-string-right-trim "." (vl-string-right-trim "0" (rtos sum 2 (getvar "LUPREC")))))
	    (setq lst_sum (cons str lst_sum))
	    (setq i (1- i))
	    )
	  (setq lst_sum (cons "T\\U+1ED4NG" (cdr lst_sum)))
	  (setq lst_tk (append lst_tk (list lst_sum)))
	  )
	)
      (setq lst_mod nil)
      (repeat (setq i (length lst_tk))
	(setq lst (mapcar '(lambda (x) (if (numberp x) (rtos x 2 (getvar "LUPREC")) x)) (nth (1- i) lst_tk)))
	(setq lst_mod (cons lst lst_mod))
	(setq i (1- i))
	)
      (setq lst_result (append (list (list "B\\U+1EA2NG TH\\U+1ED0NG KÊ S\\U+1ED0 L\\U+01AF\\U+1EE2NG ATTRIBUTE-PARAMETER") (append (list "STT") (append lst_tag lst_param) (list "S\\U+1ED1 l\\U+01B0\\U+1EE3ng"))) lst_mod))
      (ND:list->table-AC lst_result (atof 3DUY-ATK-CAOCHU) (* (atof 3DUY-ATK-CAOCHU) 3) nil pt 3DUY-ATK-TBLSTYLE-NAME 3DUY-ATK-TXTSTYLE-NAME (getvar "CLAYER"))
      (vla-endundomark (vla-get-activedocument (vlax-get-acad-object)))
      )
    )
  (princ)
  )

;Dem Att
(defun C:atc ( / E ELST I LST LST_ADD LST_ATT LST_COUNT LST_MOD LST_NAME LST_RESULT LST_SUM LST_TAG NAME PT STR STR_ADD SUM X)
  (if
    (and
      (setq elst (vl-remove-if-not 'ND:att_get (vl-remove-if 'listp (mapcar 'cadr (if (ssget (list (cons 0 "INSERT"))) (ssnamex (ssget "_P")))))))
      (setq lst_name (vl-sort (ND:unique (mapcar '(lambda (e) (ND:get_blkname e)) elst)) '<))
      (setq lst_tag (ND:unique (apply 'append (mapcar 'ND:att_get-blk lst_name))))
      (if (> (length lst_tag) 1)
	(setq lst_tag (ND:listbox lst_tag "Ch\\U+1ECDn 1 ho\\U+1EB7c nhi\\U+1EC1u Tag" 10 16 1))
	t
	)
      (setq pt (getpoint "\nChon diem xuat bang thong ke: "))
      )
    (progn
      (vla-startundomark (vla-get-activedocument (vlax-get-acad-object)))
      (setq lst_count nil)
      (foreach ent elst
	(setq name (ND:get_blkname ent))
	(setq lst_att (ND:att_get ent))
	(setq str_add name)
	(foreach tag lst_tag
	  (setq str (if (assoc tag lst_att) (ND:unformat (cdr (assoc tag lst_att)) nil) ""))
	  (setq str_add (strcat str_add "\t" str))
	  )
	(setq lst_count (ND:assoc+ str_add lst_count))
	)
      (setq lst_count (vl-sort lst_count '(lambda (a b) (ND:alphanumcompare (car a) (car b) t))))
      (setq lst_mod nil)
      (repeat (setq i (length lst_count))
	(setq lst (nth (1- i) lst_count))
	(setq lst_add (append (list (itoa i)) (ND:str->list1 (car lst) "\t") (list (itoa (cdr lst)))))
	(setq lst_mod (cons lst_add lst_mod))
	(setq i (1- i))
	)
      (if (= 3DUY-ATK-SUM "1")
	(progn
	  (setvar "DIMZIN" 0)
	  (setq lst_sum nil)
	  (repeat (setq i (length (car lst_mod)))
	    (setq sum (apply '+ (mapcar '(lambda (x) (* (atof (nth (1- i) x)) (if (= i (length (car lst_mod))) 1 (atof (last x))))) lst_mod)))
	    (setq str (vl-string-right-trim "." (vl-string-right-trim "0" (rtos sum 2 (getvar "LUPREC")))))
	    (setq lst_sum (cons str lst_sum))
	    (setq i (1- i))
	    )
	  (setq lst_sum (cons "T\\U+1ED4NG" (cdr lst_sum)))
	  (setq lst_mod (append lst_mod (list lst_sum)))
	  )
	)
      (setq lst_result (append (list (list "B\\U+1EA2NG TH\\U+1ED0NG KÊ S\\U+1ED0 L\\U+01AF\\U+1EE2NG ATTRIBUTE") (append (list "STT" "Tên Block") lst_tag (list "S\\U+1ED1 l\\U+01B0\\U+1EE3ng"))) lst_mod))
      (ND:list->table-AC lst_result (atof 3DUY-ATK-CAOCHU) (* (atof 3DUY-ATK-CAOCHU) 3) nil pt 3DUY-ATK-TBLSTYLE-NAME 3DUY-ATK-TXTSTYLE-NAME (getvar "CLAYER"))
      (vla-endundomark (vla-get-activedocument (vlax-get-acad-object)))
      )
    )
  (princ)
  )

;Dem Parameter
(defun C:dyc ( / A B ELST ENT I LST LST_ADD LST_COUNT LST_DYN LST_MOD LST_PARAM LST_RESULT LST_SUM LST_TK PT STR SUM VAL X)
  (if
    (and
      (setq elst (vl-remove-if-not 'ND:dyn_get (vl-remove-if 'listp (mapcar 'cadr (if (ssget (list (cons 0 "INSERT"))) (ssnamex (ssget "_P")))))))
      (setq lst_param (vl-remove "Origin" (vl-sort (ND:unique (apply 'append (mapcar '(lambda (ent) (mapcar 'car (ND:dyn_get-visible ent))) elst))) '<)))
      (if (> (length lst_param) 1)
	(setq lst_param (ND:listbox lst_param "Ch\\U+1ECDn 1 ho\\U+1EB7c nhi\\U+1EC1u Parameter" 10 16 1))
	t
	)
      (setq pt (getpoint "\nChon diem xuat bang thong ke: "))
      )
    (progn
      (vla-startundomark (vla-get-activedocument (vlax-get-acad-object)))
      (setq elst (vl-sort elst '(lambda (a b) (ND:alphanumcompare (ND:get_blkname a) (ND:get_blkname b) t))))
      (setq lst_count nil)
      (foreach ent elst
	(setq lst_add nil)
	(setq lst_dyn (ND:dyn_get-visible ent))
	(foreach param lst_param
	  (if (setq val (cdr (assoc param lst_dyn)))
	    (if (numberp val)
	      (setq val (* 0.0001 (atoi (rtos (/ val 0.0001) 2 0))))
	      )
	    (setq val "")
	    )
	  (setq lst_add (append lst_add (list val)))
	  )
	(setq lst_count (ND:assoc+ lst_add lst_count))
	)
      (setq lst_tk nil)
      (repeat (setq i (length lst_count))
	(setq lst (nth (1- i) lst_count))
	(setq lst_add (append (list (itoa i)) (car lst) (list (itoa (cdr lst)))))
	(setq lst_tk (cons lst_add lst_tk))
	(setq i (1- i))
	)
      (if (= 3DUY-ATK-SUM "1")
	(progn
	  (setvar "DIMZIN" 0)
	  (setq lst_sum nil)
	  (repeat (setq i (length (car lst_tk)))
	    (setq sum (apply '+ (mapcar '(lambda (x / a b)
					   (if (not (numberp (setq a (nth (1- i) x)))) (setq a (atof a)))
					   (setq b (if (= i (length (car lst_tk))) 1 (atof (last x))))
					   (* a b)
					   )
					lst_tk
					)))
	    (setq str (vl-string-right-trim "." (vl-string-right-trim "0" (rtos sum 2 (getvar "LUPREC")))))
	    (setq lst_sum (cons str lst_sum))
	    (setq i (1- i))
	    )
	  (setq lst_sum (cons "T\\U+1ED4NG" (cdr lst_sum)))
	  (setq lst_tk (append lst_tk (list lst_sum)))
	  )
	)
      (setq lst_mod nil)
      (repeat (setq i (length lst_tk))
	(setq lst (mapcar '(lambda (x) (if (numberp x) (rtos x 2 (getvar "LUPREC")) x)) (nth (1- i) lst_tk)))
	(setq lst_mod (cons lst lst_mod))
	(setq i (1- i))
	)
      (setq lst_result (append (list (list "B\\U+1EA2NG TH\\U+1ED0NG KÊ S\\U+1ED0 L\\U+01AF\\U+1EE2NG PARAMETER") (append (list "STT") lst_param (list "S\\U+1ED1 l\\U+01B0\\U+1EE3ng"))) lst_mod))
      (ND:list->table-AC lst_result (atof 3DUY-ATK-CAOCHU) (* (atof 3DUY-ATK-CAOCHU) 3) nil pt 3DUY-ATK-TBLSTYLE-NAME 3DUY-ATK-TXTSTYLE-NAME (getvar "CLAYER"))
      (vla-endundomark (vla-get-activedocument (vlax-get-acad-object)))
      )
    )
  (princ)
  )

;Dem noi dung Att+Text
(defun C:ndc ( / ELST I LST LST_ADD LST_COUNT LST_MOD LST_RESULT PT STR)
  (if
    (and
      (setq elst (vl-remove-if 'listp (mapcar 'cadr (if (ssget (list (cons 0 "INSERT,*TEXT"))) (ssnamex (ssget "_P"))))))
      (setq pt (getpoint "\nChon diem xuat bang thong ke: "))
      )
    (progn
      (vla-startundomark (vla-get-activedocument (vlax-get-acad-object)))
      (setq lst_count nil)
      (foreach ent elst
	(foreach lst (ND:att_get ent)
	  (setq lst_count (ND:assoc+ (cdr lst) lst_count))
	  )
	(if (setq str (cdr (assoc 1 (entget ent))))
	  (setq lst_count (ND:assoc+ str lst_count))
	  )
	)
      (setq lst_count (vl-sort lst_count '(lambda (a b) (ND:alphanumcompare (car a) (car b) t))))
      (setq lst_mod nil)
      (repeat (setq i (length lst_count))
	(setq lst (nth (1- i) lst_count))
	(setq lst_add (append (list (itoa i)) (ND:str->list1 (car lst) "\t") (list (itoa (cdr lst)))))
	(setq lst_mod (cons lst_add lst_mod))
	(setq i (1- i))
	)
      (setq lst_result (append (list (list "B\\U+1EA2NG TH\\U+1ED0NG KÊ") (list "STT" "N\\U+1ED8I DUNG" "SL")) lst_mod))
      (ND:list->table-AC lst_result (atof 3DUY-ATK-CAOCHU) (* (atof 3DUY-ATK-CAOCHU) 3) nil pt 3DUY-ATK-TBLSTYLE-NAME 3DUY-ATK-TXTSTYLE-NAME (getvar "CLAYER"))
      (vla-endundomark (vla-get-activedocument (vlax-get-acad-object)))
      )
    )
  (princ)
  )

;Dem Block
(defun C:blc ( / BLK_NAME COUNT ELST ENT_LAST ENT_TMP I LST LST_ADD LST_RESULT LST_TK LST_VIS NAME OPT OTHO PIC PT SNAP VIS VIS_NAME VIS_PAR X)
  (if
    (and
      (setq elst (vl-remove-if 'listp (mapcar 'cadr (if (ssget (list (cons 0 "INSERT"))) (ssnamex (ssget "_P"))))))
      (if (vl-remove nil (mapcar 'ND:dyn_visparam elst))
	(setq opt (setq 3DUY-ATK-DYNCNT (ND:get_key (list "Yes" "No") (if 3DUY-ATK-DYNCNT 3DUY-ATK-DYNCNT "Yes") "Dem theo trang thai Block (Visibility)")))
	(setq opt "No")
	)
      (setq pt (getpoint "\nChon diem xuat bang thong ke: "))
      )
    (progn
      (vla-startundomark (vla-get-activedocument (vlax-get-acad-object)))
      (if (= opt "Yes")
	(progn
	  (setq lst_vis nil)
	  (foreach ent elst
	    (setq name (ND:get_blkname ent))
	    (if (not (setq vis (ND:dyn_visparam ent))) (setq vis ""))
	    (setq lst_vis (ND:nassoc+ (list name vis) lst_vis))
	    )
	  (setq lst_vis (vl-sort lst_vis '(lambda (a b) (< (car a) (car b)))))
	  (setq lst_tk nil)
	  (setq i 0)
	  (foreach blk lst_vis
	    (setq blk_name (car blk))
	    (setq lst (vl-sort (cdr blk) '(lambda (a b) (< (car a) (car b)))))
	    (foreach vis lst
	      (setq i (1+ i))
	      (if (/= (setq vis_name (car vis)) "")
		(progn
		  (setq ent_tmp (ND:vla_insert blk_name '(0 0) "0" 1 0 nil nil))
		  (setq vis_par (ND:getvisibilityparametername ent_tmp))
		  (ND:dyn_set ent_tmp vis_par vis_name)
		  (setq snap (getvar "OSMODE"))
		  (setq otho (getvar "ORTHOMODE"))
		  (setvar "OSMODE" 0)
		  (setvar "ORTHOMODE" 0)
		  (command "COPYBASE" '(0 0) (ssadd ent_tmp) "")
		  (command "PASTEBLOCK" '(0 0))
		  (setvar "OSMODE" snap)
		  (setvar "ORTHOMODE" otho)
		  (setq ent_last (entlast))
		  (setq pic (strcat "%%BLK" (ND:get_blkname ent_last)))
		  (if (/= ent_tmp ent_last) (entdel ent_last))
		  (entdel ent_tmp)
		  )
		(setq pic (strcat "%%BLK" blk_name))
		)
	      (setq lst_add (list (itoa i) pic blk_name vis_name (itoa (cadr vis))))
	      (setq lst_tk (append lst_tk (list lst_add)))
	      )
	    )
	  (setq lst_result (append (list
				     (list "B\\U+1EA3ng th\\U+1ED1ng kê")
				     (list "STT" "Minh h\\U+1ECDa" "Tên Block" "Tr\\U+1EA1ng thái" "S\\U+1ED1 l\\U+01B0\\U+1EE3ng"))
				   lst_tk
				   ))
	  (ND:list->table-AC lst_result (atof 3DUY-ATK-CAOCHU) (* (atof 3DUY-ATK-CAOCHU) 3) nil pt 3DUY-ATK-TBLSTYLE-NAME 3DUY-ATK-TXTSTYLE-NAME (getvar "CLAYER"))
	  )
	(progn
	  (setq lst_tk nil)
	  (foreach ent elst
	    (setq name (ND:get_blkname ent))
	    (if (setq count (cadr (assoc name lst_tk)))
	      (setq lst_tk (subst (list name (1+ count)) (assoc name lst_tk) lst_tk))
	      (setq lst_tk (cons (list name 1) lst_tk))
	      )
	    )
	  (setq lst_tk (vl-sort lst_tk '(lambda (a b) (< (car a) (car b)))))
	  (setq i 0)
	  (setq lst_tk (mapcar '(lambda (x) (list (itoa (setq i (1+ i))) (strcat "%%BLK" (car x)) (car x) (itoa (cadr x)))) lst_tk))
	  (setq lst_result (append (list
				     (list "B\\U+1EA2NG TH\\U+1ED0NG KÊ S\\U+1ED0 L\\U+01AF\\U+1EE2NG BLOCK")
				     (list "STT" "Minh h\\U+1ECDa" "Tên Block" "S\\U+1ED1 l\\U+01B0\\U+1EE3ng"))
				   lst_tk
				   ))
	  (ND:list->table-AC lst_result (atof 3DUY-ATK-CAOCHU) (* (atof 3DUY-ATK-CAOCHU) 3) nil pt 3DUY-ATK-TBLSTYLE-NAME 3DUY-ATK-TXTSTYLE-NAME (getvar "CLAYER"))
	  )
	)
      (vla-endundomark (vla-get-activedocument (vlax-get-acad-object)))
      )
    )
  (princ)
  )

(defun C:atkinfo ()
  (startapp "explorer" "https://drive.google.com/drive/folders/1wEGtQeuCU3jjl9mu07upkiC9quuRx4jH")
  (princ)
  )

;--------------------------------------------------
;--------------------------------------------------
;--------------------------------------------------
;--------------------------------------------------
;--------------------------------------------------
;------------------SUB FUNCTION--------------------
;--------------------------------------------------
;--------------------------------------------------
;--------------------------------------------------
;--------------------------------------------------
;--------------------------------------------------

;--------------------------------
;---------- TABLE LIST ----------
;--------------------------------

;Danh sach ten layout tu trai qua phai
(defun ND:layoutlist ( / EPRDICT EPRLAY RESULT)
  (setq eprDict (entget (cdadr (member (cons 3 "ACAD_LAYOUT") (entget (namedobjdict))))))
  (setq result nil)
  (foreach itm eprDict
    (if (and (= (car itm) 350) (setq eprLay (member (cons 100 "AcDbLayout") (entget (cdr itm)))))
      (setq result (cons (cons (cdr (assoc 71 eprLay)) (cdr (assoc 1 eprLay))) result))
      )
    )
  (mapcar 'cdr (cdr (vl-sort result '(lambda (a b) (< (car a) (car b))))))
  )

;Table list
(defun ND:table_lst (name / e lst)
  (while (setq e (tblnext name (null e))) (setq lst (cons (cdr (assoc 2 e)) lst)))
  (setq lst (vl-sort (vl-remove-if '(lambda (e) (or (= e "") (member 42 (vl-string->list e)) (member 124 (vl-string->list e)))) lst) '<))
  )

;Get all Table Style list
(defun ND:tablestyle_lst ( / acad_table_lst table_lst)
  (setq acad_table_lst (dictsearch (namedobjdict) "Acad_TableStyle"))
  (foreach item	acad_table_lst
    (if	(= (car item) 3)
      (setq table_lst (append table_lst (list (cdr item))))
      )
    )
  table_lst
  )

;-----------------------------------
;---------- TAO DOI TUONG ----------
;-----------------------------------

;VLAINSERT-Block
(defun ND:vla_insert (block pt layer scale rotation attribute dynamic / acdoc acspc itm lst obj)
  (setq acdoc (vla-get-ActiveDocument (vlax-get-acad-object)))
  (setq acspc (vlax-get-property acdoc (if (= (getvar "CVPORT") 1) 'paperspace 'modelspace)))
  (if (listp scale)
    (setq obj (vla-InsertBlock acspc (vlax-3d-point pt) block (car scale) (cadr scale) (caddr scale) rotation))
    (setq obj (vla-InsertBlock acspc (vlax-3d-point pt) block scale scale scale rotation))
    )
  (vlax-put-property obj 'layer layer)
  (foreach att (vlax-invoke obj 'getattributes)
    (if (setq itm (assoc (vla-get-tagstring att) attribute))
      (vla-put-textstring att (cdr itm))
      )
    )
  (setq dynamic (mapcar '(lambda (lst) (cons (strcase (car lst)) (cdr lst))) dynamic))
  (foreach dyn (vlax-invoke obj 'getdynamicblockproperties)
    (if (setq itm (assoc (strcase (vla-get-propertyname dyn)) dynamic))
      (vla-put-value dyn (vlax-make-variant (cdr itm) (vlax-variant-type (vla-get-value dyn))))
      )
    )
  (vlax-vla-object->ename obj)
  )

;--------------------------------
;---------- ATTRIBUTTE ----------
;--------------------------------

;Thong ke Att theo Block Name (str)
(defun ND:att_get-blk (blkname / blk dxf lst)
  (if blkname
    (progn
      (setq blk (tblobjname "BLOCK" blkname))
      (while (setq blk (entnext blk))
	(setq dxf (entget blk))
	(if (= (cdr (assoc 0 dxf)) "ATTDEF")
	  (setq lst (cons (cdr (assoc 2 dxf)) lst))
	  )
	)
      (reverse lst)
      )
    )
  )

;Thong ke Att.Val theo Insert
(defun ND:att_get (ent / enx)
  (if (and (setq ent (entnext ent)) (= "ATTRIB" (cdr (assoc 0 (setq enx (entget ent))))))
    (cons (cons (cdr (assoc 2 enx)) (cdr (assoc 1 (reverse enx)))) (ND:att_get ent))
    )
  )

;--------------------------------
;----------- DYNAMIC ------------
;--------------------------------

;Thong ke Dyn.Val theo Insert
(defun ND:dyn_get (ent)
  (mapcar '(lambda (dyn) (cons (vla-get-propertyname dyn) (vlax-get dyn 'value)))
	  (vlax-invoke (vlax-ename->vla-object ent) 'getdynamicblockproperties)
	  )
  )

;Thong ke Dyn.Val theo Insert (visible)
(defun ND:dyn_get-visible (ent)
  (vl-remove nil
    (mapcar '(lambda (x / lst prop val)
	       (if
		 (and
		   (setq prop (vla-get-propertyname x))
		   (setq lst (list ent (strcat "AcDbDynBlockProperty" prop)))
		   (setq val (vl-catch-all-apply 'getpropertyvalue lst))
		   (not (vl-catch-all-error-p val))
		   )
		 (cons prop val)
		 )
	       )
	    (vlax-invoke (vlax-ename->vla-object ent) 'getdynamicblockproperties)
	    )
    )
  )

;Set Dyn (1 property) theo Insert
(defun ND:dyn_set (ent prp val)
  (setq prp (strcase prp))
  (vl-some
    '(lambda ( x )
       (if (= prp (strcase (vla-get-propertyname x)))
	 (progn
	   (vla-put-value x (vlax-make-variant val (vlax-variant-type (vla-get-value x))))
	   (cond (val) (t))
	   )
	 )
       )
    (vlax-invoke (vlax-ename->vla-object ent) 'getdynamicblockproperties)
    )
  )

;Lay nhan hieu trang thai cua Insert (ent)
(defun ND:getvisibilityparametername (blk / vis)
  (if
    (and
      (setq blk (vlax-ename->vla-object blk))
      (vlax-property-available-p blk 'effectivename)
      (setq blk
	     (vla-item
	       (vla-get-blocks (vla-get-document blk))
	       (ND:get_blkname (vlax-vla-object->ename blk))
	       )
	    )
      (= :vlax-true (vla-get-isdynamicblock blk))
      (= :vlax-true (vla-get-hasextensiondictionary blk))
      (setq vis
	     (vl-some
	       '(lambda ( pair )
		  (if
		    (and
		      (= 360 (car pair))
		      (= "BLOCKVISIBILITYPARAMETER" (cdr (assoc 0 (entget (cdr pair)))))
		      )
		    (cdr pair)
		    )
		  )
	       (dictsearch
		 (vlax-vla-object->ename (vla-getextensiondictionary blk))
		 "ACAD_ENHANCEDBLOCK"
		 )
	       )
	    )
      )
    (cdr (assoc 301 (entget vis)))
    )
  )

;Lay ten trang thai hien hanh cua Insert (ent)
(defun ND:dyn_visparam (obj / obj1 vis)
  (if
    (and
      (setq obj (vlax-ename->vla-object obj))
      (vlax-property-available-p obj 'effectivename)
      (setq obj1
	     (vla-item
	       (vla-get-blocks (vla-get-document obj))
	       (ND:get_blkname (vlax-vla-object->ename obj))
	       )
	    )
      (= :vlax-true (vla-get-isdynamicblock obj1))
      (= :vlax-true (vla-get-hasextensiondictionary obj1))
      (setq vis
	     (vl-some
	       '(lambda	(pair)
		  (if
		    (and
		      (= 360 (car pair))
		      (= "BLOCKVISIBILITYPARAMETER"
			 (cdr (assoc 0 (entget (cdr pair))))
			 )
		      )
		     (cdr pair)
		    )
		  )
	       (dictsearch
		 (vlax-vla-object->ename (vla-getextensiondictionary obj1))
		 "ACAD_ENHANCEDBLOCK"
		 )
	       )
	    )
      )
     (vl-some '(lambda (x) (if (= (strcase (cdr (assoc 301 (entget vis)))) (strcase (vla-get-propertyname x))) (vlax-get x 'value)))
	      (vlax-invoke obj 'getdynamicblockproperties)
	      )
    )
  )

;-------------------------------------
;---------- XU LY DOI TUONG ----------
;-------------------------------------

;Lay ten Block (co dau tieng Viet)
(defun ND:get_blkname (ent / lst name)
  (vl-catch-all-apply '(lambda ()
			 (setq lst (entget (cdr (assoc 340 (entget (vlax-vla-object->ename (vla-item (vla-item (vla-GetExtensionDictionary (vlax-ename->vla-object ent)) "AcDbBlockRepresentation") "AcDbRepData")))))))
			 ))
  (if (not lst) (setq lst (entget ent)))
  (setq name (cdr (assoc 2 lst)))
  )

;Xoa phan tu trung trong list
(defun ND:unique (lst)
  (if lst (cons (car lst) (ND:unique (vl-remove (car lst) (cdr lst)))))
  )

;Assoc+
(defun ND:assoc+ (key lst / itm)
  (if (setq itm (assoc key lst))
    (subst (cons key (1+ (cdr itm))) itm lst)
    (cons (cons key 1) lst)
    )
  )

;Nested Assoc+
(defun ND:nassoc+ (key lst / itm)
  (if key
    (if (setq itm (assoc (car key) lst))
      (subst (cons (car key) (ND:nassoc+ (cdr key) (cdr itm))) itm lst)
      (cons  (cons (car key) (ND:nassoc+ (cdr key) nil)) lst)
      )
    (if lst (list (1+ (car lst))) '(1))
    )
  )

;Kiem tra string co phai so thuc khong
(defun ND:strnump (str)
  (if
    (and
      (= (type str) 'str)
      (or
	(/= (atof str) 0)
	(not (vl-remove-if '(lambda (x) (member x '(44 46 48))) (vl-string->list str)))
	)
      )
    t
    )
  )

;Lay diem chen (diem can le text) doi tuong
(defun ND:ins_pt (ent / lst pt)
  (if (= (type ent) 'ename)
    (progn
      (setq lst (entget ent))
      (setq pt (cdr (assoc 11 lst)))
      (if (or
	    (not pt)
	    (equal (list (car pt) (cadr pt)) '(0 0))
	    (equal (list (car pt) (cadr pt)) '(1 0))
	    )
	(cdr (assoc 10 lst))
	pt
	)
      )
    )
  )

;Unformat String
; mtx =t: string in mtext ; =nil: string in dtext
(defun ND:unformat (str mtx / rx)
  (defun ND:replace (new old str)
    (vlax-put-property rx 'pattern old)
    (vlax-invoke rx 'replace str new)
    )
  (if (setq rx (vlax-get-or-create-object "VBScript.RegExp"))
    (progn
      (setq str
	     (vl-catch-all-apply
	       (function
		 (lambda ()
		   (vlax-put-property rx 'global     actrue)
		   (vlax-put-property rx 'multiline  actrue)
		   (vlax-put-property rx 'ignorecase acfalse)
		   (foreach pair
			    '(
			      ("\032"    . "\\\\\\\\")
			      (" "       . "\\\\P|\\n|\\t")
			      ("$1"      . "\\\\(\\\\[ACcFfHLlOopQTW])|\\\\[ACcFfHLlOopQTW][^\\\\;]*;|\\\\[ACcFfHLlOopQTW]")
			      ("$1$2/$3" . "([^\\\\])\\\\S([^;]*)[/#\\^]([^;]*);")
			      ("$1$2"    . "\\\\(\\\\S)|[\\\\](})|}")
			      ("$1"      . "[\\\\]({)|{")
			      )
		     (setq str (ND:replace (car pair) (cdr pair) str))
		     )
		   (if mtx
		     (ND:replace "\\\\" "\032" (ND:replace "\\$1$2$3" "(\\\\[ACcFfHLlOoPpQSTW])|({)|(})" str))
		     (ND:replace "\\"   "\032" str)
		     )
		   )
		 )
	       )
	    )
      (vlax-release-object rx)
      (if (null (vl-catch-all-error-p str))
	str
	)
      )
    )
  )

;Mutiple String subst
(defun ND:str-subst (new old str / del i)
  (if (/= new old)
    (progn
      (setq del "")
      (while (setq i (vl-string-search old str))
	(setq del (strcat del (vl-string-subst new old (substr str 1 (+ i (strlen old))))))
	(setq str (substr str (+ i 1 (strlen old))))
	)
      (strcat del str)
      )
    str
    )
  )

;String to List (Leemac)
; Del: str ngat
(defun ND:str->list1 (str del / len lst pos)
  (setq len (1+ (strlen del)))
  (while (setq pos (vl-string-search del str))
    (setq lst (cons (substr str 1 pos) lst)
	  str (substr str (+ pos len))
	  )
    )
  (reverse (cons str lst))
  )

;String to List
; Del: str ngat, la str or list str (nil = space va tab)
(defun ND:str->list (str del / del1 len lst lst1 pos)
  (if (not del) (setq del (list " " "\t")))
  (if (not (listp del)) (setq del (list del)))
  (while (setq lst1 (vl-remove nil (mapcar '(lambda (x) (vl-string-search x str)) del)))
    (setq pos (apply 'min lst1))
    (setq del1 (vl-remove-if-not '(lambda (x) (vl-string-search x str)) del))
    (setq del1 (vl-sort del1 '(lambda (a b) (< (vl-string-search a str) (vl-string-search b str)))))
    (setq len (1+ (strlen (car del1))))
    (setq lst (cons (substr str 1 pos) lst))
    (setq str (substr str (+ pos len)))
    )
  (vl-remove "" (reverse (cons str lst)))
  )

;String to List
; (ND:ATK_str->list "TAG1 \"123\" TAG2")
(defun ND:ATK_str->list (str / A B C CHA LST RESULT STRP Y)
  (setq a 1)
  (setq b 1)
  (setq c t)
  (setq lst nil)
  (repeat (strlen str)
    (setq cha (substr str a 1))
    (if (or (and (= cha " ") c) (= a (strlen str)))
      (progn
	(setq lst (cons (substr str b (- a b -1)) lst))
	(setq b a)
	)
      )
    (if (= cha "\"")
      (progn
	(setq c (not c))
	(setq lst (cons (substr str b (- a b -1)) lst))
	(setq b a)
	)
      )
    (setq a (1+ a))
    )
  (setq result nil)
  (foreach x lst
    (if (vl-string-search "\"" x) (setq strp t) (setq strp nil))
    (if (or
	  (> (strlen (setq y (vl-string-trim " " (vl-string-trim "\"" x)))) 0)
	  (and (= (substr x 1 1) "\"")
	       (= (substr x (strlen x) 1) "\"")
	       (> (strlen (setq y (vl-string-trim "\"" x))) 0)
	       )
	  )
      (if strp
	(setq result (cons y result))
	(setq result (cons (cons "ATT" y) result))
	)
      )
    )
  result
  )

; ND:alphanumcompare: SO SANH 2 STRING (BAO GOM CHU VA SO KET HOP)
; s1, s2: string
; nump: boolean (quy doi string ve so thuc)
; CACH DUNG
; (vl-sort '("-1" "-2" "0" "1" "2" "3.1" "3.15" "3.2") '(lambda (a b) (ND:alphanumcompare a b t)))
; (vl-sort '("-1" "-2" "0" "1" "2" "3.1" "3.15" "3.2" "N1" "N2" "N10" "T20" "T20A" "T20B") '(lambda (a b) (ND:alphanumcompare a b nil)))
; (vl-sort EntityList '(lambda (a b) (ND:alphanumcompare (cdr (assoc 1 (entget a))) (cdr (assoc 1 (entget b))) nil)))
(defun ND:alphanumcompare (s1 s2 nump / r1 r2 t1 t2)
  (if nump
    (vl-catch-all-error-p
      (vl-catch-all-apply
	(function
	  (lambda ()
	    (setq r1 (read s1) r2 (read s2))
	    )))))
  (if (not (and (numberp r1) (numberp r2)))
    (progn
      (while (and (/= s1 "") (/= s2 "") (eq (ascii s1) (ascii s2)))
	(setq s1 (substr s1 2) s2 (substr s2 2))
	)
      (setq t1 (atoi (strcat "1" s1)) t2 (atoi (strcat "1" s2)))
      )
    )
  (cond
    ((and nump (numberp r1) (numberp r2)) (< r1 r2))
    ((or
       (eq t1 t2)
       (and (>= (ascii s1) 48) (<= (ascii s1) 57) (or (< (ascii s2) 48) (> (ascii s2) 57)))
       (and (>= (ascii s2) 48) (<= (ascii s2) 57) (or (< (ascii s1) 48) (> (ascii s1) 57)))
       )
     (< (ascii s1) (ascii s2))
     )
    (t (< t1 t2))
    )
  )

;Sap xep elst
; order1: trong 1 nhom
; order2: trong cac nhom
; 0:L>R - 1:R>L - 2:U>D - 3:D>U
; fuzz: sai so
(defun ND:ATK_sortXY (elst order1 order2 fuzz)
  (vl-sort elst
	   '(lambda (e1 e2 / p1 p2)
	      (setq p1 (trans (ND:ins_pt e1) 0 1))
	      (setq p2 (trans (ND:ins_pt e2) 0 1))
	      (if (wcmatch order1 "0,1")
		(cond
		  ((equal (cadr p1) (cadr p2) fuzz)
		   (if (= order1 "0")
		     (< (car p1) (car p2))
		     (> (car p1) (car p2))
		     )
		   )
		  (t
		   (if (= order2 "0")
		     (> (cadr p1) (cadr p2))
		     (< (cadr p1) (cadr p2))
		     )
		   )
		  )
		(cond
		  ((equal (car p1) (car p2) fuzz)
		   (if (= order1 "2")
		     (> (cadr p1) (cadr p2))
		     (< (cadr p1) (cadr p2))
		     )
		   )
		  (t
		   (if (= order2 "0")
		     (< (car p1) (car p2))
		     (> (car p1) (car p2))
		     )
		   )
		  )
		)
	      )
	 )
  )

;-------------------------------
;---------- NHAP LIEU ----------
;-------------------------------

;Chon doi tuong
(defun ND:ent_pick (typ promp / ent)
  (while (not ent)
    (while (not (setq ent (car (entsel (strcat "\n" promp))))))
    (if (not (wcmatch (cdr (assoc 0 (entget ent))) typ)) (setq ent nil))
    )
  ent
  )

;Nhap Keyword
(defun ND:get_key (key default promp / del init key_init key_select result select str)
  (setq key_init key)
  (foreach del (list " " "_")
    (setq key_init (mapcar '(lambda (str) (while (vl-string-search del str) (setq str (vl-string-subst "" del str))) str) key_init))
    )
  (setq key_select (mapcar '(lambda (str) (while (vl-string-search "_" str) (setq str (vl-string-subst "" "_" str))) str) key))
  (setq init (strcat (car key_init) (apply 'strcat (mapcar (function (lambda (x) (strcat " " x))) (cdr key_init)))))
  (setq select (strcat (car key_select) (apply 'strcat (mapcar (function (lambda (x) (strcat "/" x))) (cdr key_select)))))
  (if (member default key) (setq default (nth (vl-position default key) key_select)) (setq default (car key_select)))
  (initget init)
  (setq promp (strcat "\n" promp " [" select "] <" default ">: "))
  (if (setq result (getkword promp))
    (nth (vl-position result key_init) key)
    (nth (vl-position default key_select) key)
    )
  )

;Chon trong hop thoai
(defun ND:listbox (lst msg wid hei bit / dch des tmp rtn)
  (if (> (length lst) 1)
    (progn
      (cond
	((not
	   (and
	     (setq tmp (vl-filename-mktemp nil nil ".dcl"))
	     (setq des (open tmp "w"))
	     (write-line
	       (strcat
		 "listbox:dialog{label=\""
		 msg
		 "\";spacer;:list_box{key=\"list\";multiple_select="
		 (if (= 1 (logand 1 bit))
		   "true"
		   "false"
		 )
		 (strcat ";width="
			 (rtos wid 2 0)
			 ";height="
			 (rtos hei 2 0)
			 ";}spacer;ok_cancel;}"
		 )
	       )
	       des
	     )
	     (not (close des))
	     (< 0 (setq dch (load_dialog tmp)))
	     (new_dialog "listbox" dch)
	   )
	 )
	 (prompt "\nError Loading List Box Dialog.")
	)
	(t
	 (start_list "list")
	 (foreach itm lst (add_list itm))
	 (end_list)
	 (setq rtn (set_tile "list" "0"))
	 (action_tile "list" "(setq rtn $value)")
	 (setq rtn
		(if (= 1 (start_dialog))
		  (if (= 2 (logand 2 bit))
		    (read (strcat "(" rtn ")"))
		    (mapcar '(lambda (x) (nth x lst))
			    (read (strcat "(" rtn ")"))
		    )
		  )
		)
	 )
	)
      )
      (if (< 0 dch)
	(unload_dialog dch)
      )
      (if (and tmp (setq tmp (findfile tmp)))
	(vl-file-delete tmp)
      )
      rtn
    )
    lst
    )
  )

;--------------------------------
;----------- HAM KHAC -----------
;--------------------------------

;Set Clipboard Text
(defun ND:SetClipBoardText (str / htmlfile result)
  (setq result
	 (vlax-invoke
	   (vlax-get
	     (vlax-get
	       (setq htmlfile (vlax-create-object "htmlfile"))
	       'ParentWindow
	       )
	     'ClipBoardData
	     )
	   'SetData
	   "Text"
            str
	   )
	)
  (vlax-release-object htmlfile)
  str
  )

;Tinh toan tu trai qua phai
;(ND:ATK_math (list 1. '+ 2. '* 4 '/ 3))
;(ND:ATK_math (list '+ 2.))
;(ND:ATK_math (list '* 2.))
(defun ND:ATK_math (lst / I LAST_MEM LST_FUNC REP RESULT STR)
  (if (numberp (setq result (car lst)))
    (setq lst (cdr lst))
    (setq result 0.)
    )
  (setq lst_func nil)
  (setq i 0)
  (repeat (length lst)
    (setq str (nth i lst))
    (setq last_mem (last lst_func))
    (if (and (numberp str) (= (type last_mem) 'sym))
      (setq rep (cons last_mem (float str))
	    lst_func (append (reverse (cdr (reverse lst_func))) (list rep))
	    ))
    (if (and (not (numberp str)) (listp last_mem))
      (setq lst_func (append lst_func (list str)))
      )
    (setq i (1+ i))
    )
  (foreach func lst_func
    (if (listp func) (setq result (eval (cons (car func) (list result (cdr func))))))
    )
  result
  )

;(defun C:l2ttest ()
;  (ND:list->table
;    (list
;      (list "Danh muc ban ve")
;      (list "STT" "Ten ban ve" "Ma ban ve")
;      (list "1" "Tong mat bang giao thong" "VH.HT.GT.01")
;      (list "2" "Mat cat ngang dien hinh" "VH.HT.GT.02")
;      (list "3" "Chi tiet cau tao giao thong" "VH.HT.GT.03")
;      )
;    (list (cons 0 7.5) (cons 1 5.));Rows height
;    (list (cons 0 8.) (cons 1 40.) (cons 2 20.));Columns width
;    (list (cons 0 2.) (cons 1 1.5) (cons 2 1.));Text height at rows
;    (list (cons 0 acMiddleCenter) (cons 1 acMiddleCenter) (cons 2 (list (cons 0 acMiddleCenter) (cons 1 acMiddleLeft) (cons 2 acMiddleCenter))));Text Alignment at rows (and columns)
;    (list (list 0 0 0 3));Merge cells
;    (getpoint)
;    (getvar "CTABLESTYLE")
;    (getvar "TEXTSTYLE")
;    (getvar "CLAYER")
;    )
;  (princ)
;  )
;List to AutoCAD Table
(defun ND:list->table (lst_table lst_row lst_col lst_txhe lst_txal lst_merge pt tblstyle txtstyle layer / BLK COL COLWID DOC I J NCOLS NROWS ROWHEI SPACE TBOBJ TXAL TXAL1 TXHE)
  (setq doc (vla-get-activedocument (vlax-get-acad-object)))
  (setq space (if (> (getvar "CVPORT") 1) (vla-get-modelspace doc) (vla-get-paperspace doc)))
  (setq ncols (apply 'max (mapcar 'length lst_table)))
  (setq nrows (length lst_table))
  (setq tbobj (vla-AddTable space (vlax-3d-point pt) nrows ncols 10 10))
  (vla-put-Layer tbobj layer)
  (vla-put-RegenerateTableSuppressed tbobj :vlax-true)
  (vla-put-StyleName tbobj tblstyle)
  (vla-put-HorzCellMargin tbobj (cdr (last lst_txhe)))
  (vla-put-VertCellMargin tbobj (cdr (last lst_txhe)))
  (setq i 0)
  (foreach row lst_table
    (if (not (setq rowhei (cdr (assoc i lst_row)))) (setq rowhei (cdr (last lst_row))))
    (if (not (setq txhe (cdr (assoc i lst_txhe)))) (setq txhe (cdr (last lst_txhe))))
    (setq j 0)
    (repeat ncols
      (if (not (setq txal (cdr (assoc i lst_txal)))) (setq txal (cdr (last lst_txal))))
      (if (listp (setq txal1 txal))
	(if (not (setq txal (cdr (assoc j txal1)))) (setq txal (cdr (last txal1))))
	)
      (vlax-invoke tbobj 'SetCellTextStyle i j txtstyle)
      (vla-SetCellAlignment tbobj i j txal)
      (vla-SetCellTextHeight tbobj i j txhe)
      (if (setq col (nth j row))
	(if (and
	      (wcmatch col "%%BLK*")
	      (setq blk (substr col 6))
	      (tblsearch "BLOCK" blk)
	      (vlax-method-applicable-p tbobj 'setblocktablerecordid)
	      )
	  (vla-SetBlockTableRecordId tbobj i j (vla-get-ObjectID (vla-item (vla-get-Blocks doc) blk)) :vlax-true)
	  (if (/= col "") (vla-SetText tbobj i j col))
	  )
	)
      (setq j (1+ j))
      )
    (vla-SetRowHeight tbobj i rowhei)
    (setq i (1+ i))
    )
  (setq j 0)
  (repeat ncols
    (if (not (setq colwid (cdr (assoc j lst_col)))) (setq colwid (cdr (last lst_col))))
    (vla-SetColumnWidth tbobj j colwid)
    (setq j (1+ j))
    )
  (if (>= (atof (getvar "ACADVER")) 18)
    (foreach lst lst_merge
      (vla-MergeCells tbobj (car lst) (cadr lst) (caddr lst) (cadddr lst))
      )
    )
  (vla-put-RegenerateTableSuppressed tbobj :vlax-false)
  tbobj
  )

;List to AutoCAD Table (Auto columns width)
(defun ND:list->table-AC (lst_table txhe rowhei lst_merge pt tblstyle txtstyle layer / BLK COL DOC I J LEN LENS NCOLS NROWS SPACE TBOBJ X)
  (setq doc (vla-get-activedocument (vlax-get-acad-object)))
  (setq space (if (> (getvar "CVPORT") 1) (vla-get-modelspace doc) (vla-get-paperspace doc)))
  (setq ncols (apply 'max (mapcar 'length lst_table)))
  (setq nrows (length lst_table))
  (setq tbobj (vla-AddTable space (vlax-3d-point pt) nrows ncols 10 10))
  (vla-put-Layer tbobj layer)
  (vla-put-RegenerateTableSuppressed tbobj :vlax-true)
  (vla-put-StyleName tbobj tblstyle)
  (vla-put-HorzCellMargin tbobj 0.)
  (vla-put-VertCellMargin tbobj 0.)
  (setq lens nil)
  (setq i 0)
  (foreach row lst_table
    (setq j 0)
    (setq len nil)
    (repeat ncols
      (vlax-invoke tbobj 'SetCellTextStyle i j txtstyle)
      (vla-SetCellAlignment tbobj i j acMiddleCenter)
      (vla-SetCellTextHeight tbobj i j txhe)
      (if (setq col (nth j row))
	(if (and
	      (wcmatch col "%%BLK*")
	      (setq blk (substr col 6))
	      (tblsearch "BLOCK" blk)
	      (vlax-method-applicable-p tbobj 'setblocktablerecordid)
	      )
	  (progn
	    (vla-SetBlockTableRecordId tbobj i j (vla-get-ObjectID (vla-item (vla-get-Blocks doc) blk)) :vlax-true)
	    (setq len (cons (* rowhei 2.) len))
	    )
	  (progn
	    (if (/= col "") (vla-SetText tbobj i j col))
	    (setq len (cons (+ (caadr (textbox (list (cons 1 (if (= col "") "1" col)) (cons 40 txhe) (cons 7 txtstyle)))) (* txhe 4.)) len))
	    )
	  )
	)
      (setq j (1+ j))
      )
    (if (= (length len) ncols) (setq lens (append lens (list (reverse len)))))
    (vla-SetRowHeight tbobj i rowhei)
    (setq i (1+ i))
    )
  (setq j 0)
  (repeat ncols
    (vla-SetColumnWidth tbobj j (apply 'max (mapcar '(lambda (x) (if (nth j x) (nth j x) 0)) lens)))
    (setq j (1+ j))
    )
  (if (>= (atof (getvar "ACADVER")) 18)
    (foreach lst lst_merge
      (vla-MergeCells tbobj (car lst) (cadr lst) (caddr lst) (cadddr lst))
      )
    )
  (vla-put-RegenerateTableSuppressed tbobj :vlax-false)
  tbobj
  )