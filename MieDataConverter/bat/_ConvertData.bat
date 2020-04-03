@rem ���̃o�b�`�t�@�C���́A�Q�[���̗l�X�ȏ����f�[�^�x�[�X�Ɋi�[���鏈���ł��B

@rem �ȉ��̎O�̊��ϐ������s���ɍ��킹�Đݒ肵�Ă��������B
@rem -------------------------------------------------------------------
@rem �Q�[�������i�[����f�[�^�x�[�X�̃p�X���w�肷��B
@SET SYSTEM_DB=data\db\PoE2_SystemData.sqlite
@rem �Q�[�������i�[����f�[�^�x�[�X�̒�`�t�@�C���̃p�X���w�肷��B
@SET SCHEMA=data\scheme\PoE2_SystemData.sqlite.sql
@rem �Q�[���{�̂̃t�H���_�[�̃p�X���w�肷��B
@SET BASE_DIR=P:\Games\Steam\steamapps\common\Pillars of Eternity II
@rem -------------------------------------------------------------------



@rem �f�[�^�x�[�X�̏�����
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

@rem dlc_e (�Ȃ�)
@echo dlc_e
@echo @echo DLC_E - ����t�@�C���Ȃ��̂��ߏ����s�v

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
