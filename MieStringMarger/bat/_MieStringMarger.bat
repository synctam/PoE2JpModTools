@rem ���̃o�b�`�t�@�C���́A�Q�[�����f�[�^�x�[�X������{�ꉻMOD���쐬���܂��B

@rem �ȉ��̌܂̊��ϐ������s���ɍ��킹�Đݒ肵�Ă��������B
@rem -------------------------------------------------------------------
@rem �Q�[����񂪊i�[����Ă���f�[�^�x�[�X�̃p�X���w�肷��B
@SET SYSTEM_DB=data\db\PoE2_SystemData.sqlite

@rem CSV�`���̖|��V�[�g�i��b�j�̃p�X���w�肷��B
@SET SHEET_CONV=data\csv\0PoE2(conv).csv
@rem CSV�`���̖|��V�[�g�i�V�X�e���j�̃p�X���w�肷��B
@SET SHEET_SYSTEM=data\csv\0PoE2(system).csv
@rem CSV�`���̖|��V�[�g�iChatter�j�̃p�X���w�肷��B
@SET SHEET_CHATTER=data\csv\0PoE2(chatter).csv

@rem ���{�ꉻMOD���o�͂���t�H���_�[�̃p�X���w�肷��B
@SET OUT_FOLDER=jpmod
@rem -------------------------------------------------------------------

MieStringMarger.exe ^
	-c "%SHEET_CONV%" ^
	-s "%SHEET_SYSTEM%" ^
	-t "%SHEET_CHATTER%" ^
	-d "%SYSTEM_DB%" ^
	-o "%OUT_FOLDER%" ^
	-r

@pause
@exit /b

�����l
�|��V�[�g�ɂ���

1.�|��V�[�g���󂯎��
�|��҂̕�����Excel�`���̖|��V�[�g���󂯎���Ă��������B

2.CSV�t�@�C���̍쐬
�V�[�g���u�S�̂̍�Ə�v�́u��b�v���ŁA�u0PoE2(conv).csv�v�Ƃ��ĕۑ�����B
�V�[�g���u�V�X�e���v�́u�V�X�e���v���ŁA�u0PoE2(system).csv�v�Ƃ��ĕۑ�����B
�V�[�g���uc�v�́uChatter�v���ŁA�u0PoE2(chatter).csv�v�Ƃ��ĕۑ�����B

���ӎ���
�ECSV�t�@�C���́A�J���}��؂�� UTF-8 �`���ŕۑ����Ă��������B
���g���� Excel �� UTF-8 �`�����T�|�[�g���Ă��Ȃ��ꍇ�́ALibreOffice ���g�p���Ă��������B

�u�z�[�� | LibreOffice - �I�t�B�X�X�C�[�g�̃��l�T���X�v
https://ja.libreoffice.org/
