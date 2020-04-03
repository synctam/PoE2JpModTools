@rem このバッチファイルは、ゲーム情報データベースから日本語化MODを作成します。

@rem 以下の五つの環境変数を実行環境に合わせて設定してください。
@rem -------------------------------------------------------------------
@rem ゲーム情報が格納されているデータベースのパスを指定する。
@SET SYSTEM_DB=data\db\PoE2_SystemData.sqlite

@rem CSV形式の翻訳シート（会話）のパスを指定する。
@SET SHEET_CONV=data\csv\0PoE2(conv).csv
@rem CSV形式の翻訳シート（システム）のパスを指定する。
@SET SHEET_SYSTEM=data\csv\0PoE2(system).csv
@rem CSV形式の翻訳シート（Chatter）のパスを指定する。
@SET SHEET_CHATTER=data\csv\0PoE2(chatter).csv

@rem 日本語化MODを出力するフォルダーのパスを指定する。
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

■備考
翻訳シートについて

1.翻訳シートを受け取り
翻訳者の方からExcel形式の翻訳シートを受け取ってください。

2.CSVファイルの作成
シート名「全体の作業場」は「会話」分で、「0PoE2(conv).csv」として保存する。
シート名「システム」は「システム」分で、「0PoE2(system).csv」として保存する。
シート名「c」は「Chatter」分で、「0PoE2(chatter).csv」として保存する。

注意事項
・CSVファイルは、カンマ区切りで UTF-8 形式で保存してください。
お使いの Excel で UTF-8 形式をサポートしていない場合は、LibreOffice を使用してください。

「ホーム | LibreOffice - オフィススイートのルネサンス」
https://ja.libreoffice.org/
