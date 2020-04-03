@rem このバッチファイルは、ゲームの様々な情報をデータベースに格納する処理です。

@rem 以下の三つの環境変数を実行環境に合わせて設定してください。
@rem -------------------------------------------------------------------
@rem ゲーム情報を格納するデータベースのパスを指定する。
@SET SYSTEM_DB=data\db\PoE2_SystemData.sqlite
@rem ゲーム情報を格納するデータベースの定義ファイルのパスを指定する。
@SET SCHEMA=data\scheme\PoE2_SystemData.sqlite.sql
@rem ゲーム本体のフォルダーのパスを指定する。
@SET BASE_DIR=P:\Games\Steam\steamapps\common\Pillars of Eternity II
@rem -------------------------------------------------------------------



@rem データベースの初期化
MieDataConverter.exe --init -s "%SYSTEM_DB%" -q "%SCHEMA%" -r

@rem vanilla
@echo vanilla
@SET DESIGN=%BASE_DIR%\PillarsOfEternityII_Data\exported\design
@SET   LANG=%BASE_DIR%\PillarsOfEternityII_Data\exported\localized\en
MieDataConverter.exe -l "%LANG%" -d "%DESIGN%" -s "%SYSTEM_DB%" -p vanilla -r

@rem dlc_a (chatter/conv/game)
@echo dlc_a
@SET DESIGN=%BASE_DIR%\PillarsOfEternityII_Data\laxa_exported\design
@SET   LANG=%BASE_DIR%\PillarsOfEternityII_Data\laxa_exported\localized\en
MieDataConverter.exe -l "%LANG%" -d "%DESIGN%" -s "%SYSTEM_DB%" -p laxa -r

@rem dlc_b (game)
@echo dlc_b 
@SET DESIGN=%BASE_DIR%\PillarsOfEternityII_Data\laxb_exported\design
@SET   LANG=%BASE_DIR%\PillarsOfEternityII_Data\laxb_exported\localized\en
MieDataConverter.exe -l "%LANG%" -d "%DESIGN%" -s "%SYSTEM_DB%" -p laxb -r

@rem dlc_c (game)
@echo dlc_c
@SET DESIGN=%BASE_DIR%\PillarsOfEternityII_Data\laxc_exported\design
@SET   LANG=%BASE_DIR%\PillarsOfEternityII_Data\laxc_exported\localized\en
MieDataConverter.exe -l "%LANG%" -d "%DESIGN%" -s "%SYSTEM_DB%" -p laxc -r

@rem dlc_d (conversations/game)
@echo dlc_d
@SET DESIGN=%BASE_DIR%\PillarsOfEternityII_Data\laxd_exported\design
@SET   LANG=%BASE_DIR%\PillarsOfEternityII_Data\laxd_exported\localized\en
MieDataConverter.exe -l "%LANG%" -d "%DESIGN%" -s "%SYSTEM_DB%" -p laxd -r

@rem dlc_e (なし)
@echo dlc_e
@echo @echo DLC_E - 言語ファイルなしのため処理不要

@rem dlc_f (game)
@echo dlc_f
@SET DESIGN=%BASE_DIR%\PillarsOfEternityII_Data\laxf_exported\design
@SET   LANG=%BASE_DIR%\PillarsOfEternityII_Data\laxf_exported\localized\en
MieDataConverter.exe -l "%LANG%" -d "%DESIGN%" -s "%SYSTEM_DB%" -p laxf -r

@rem dlc_g (game)
@echo dlc_g
@SET DESIGN=%BASE_DIR%\PillarsOfEternityII_Data\laxg_exported\design
@SET   LANG=%BASE_DIR%\PillarsOfEternityII_Data\laxg_exported\localized\en
MieDataConverter.exe -l "%LANG%" -d "%DESIGN%" -s "%SYSTEM_DB%" -p laxg -r

@rem dlc_h (game)
@echo dlc_h
@SET DESIGN=%BASE_DIR%\PillarsOfEternityII_Data\laxh_exported\design
@SET   LANG=%BASE_DIR%\PillarsOfEternityII_Data\laxh_exported\localized\en
MieDataConverter.exe -l "%LANG%" -d "%DESIGN%" -s "%SYSTEM_DB%" -p laxh -r

@rem dlc_i (conv/game)
@echo dlc_i
@SET DESIGN=%BASE_DIR%\PillarsOfEternityII_Data\laxi_exported\design
@SET   LANG=%BASE_DIR%\PillarsOfEternityII_Data\laxi_exported\localized\en
MieDataConverter.exe -l "%LANG%" -d "%DESIGN%" -s "%SYSTEM_DB%" -p laxi -r

@rem dlc_1 (chatter/conv/game/quests)
@echo dlc_1
@SET DESIGN=%BASE_DIR%\PillarsOfEternityII_Data\lax2_exported\design
@SET   LANG=%BASE_DIR%\PillarsOfEternityII_Data\lax2_exported\localized\en
MieDataConverter.exe -l "%LANG%" -d "%DESIGN%" -s "%SYSTEM_DB%" -p dlc1 -r

@rem dlc_2 (chatter/conv/game/quests)
@echo dlc_2
@SET DESIGN=%BASE_DIR%\PillarsOfEternityII_Data\lax1_exported\design
@SET   LANG=%BASE_DIR%\PillarsOfEternityII_Data\lax1_exported\localized\en
MieDataConverter.exe -l "%LANG%" -d "%DESIGN%" -s "%SYSTEM_DB%" -p dlc2 -r

@rem dlc_3 (chatter/conv/game/quests)
@echo dlc_3
@SET DESIGN=%BASE_DIR%\PillarsOfEternityII_Data\lax3_exported\design
@SET   LANG=%BASE_DIR%\PillarsOfEternityII_Data\lax3_exported\localized\en
MieDataConverter.exe -l "%LANG%" -d "%DESIGN%" -s "%SYSTEM_DB%" -p dlc3 -r

@pause
@exit /b
