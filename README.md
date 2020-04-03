# PoE2JpModTools
Pillars of Eternity 2 日本語化MOD作成ツール

作者と連絡が取れなくなったときに備え、PoE2の日本語化MODを作成するツールのソースコードを公開することにしました。公開を前提に作成したものでないことをご了承願います。また、使い方などの説明などはありませんので、プログラムソースから判断していただけると助かります。

## 開発環境 ##
Visual Studio 2019 Community Edition

## 使用ＤＬＬについて ##
本ツールでは Pillars of Eternity のＤＬＬを使用しています。
Pillars of Eternity 2 の PillarsOfEternityII_Data\Managed フォルダーにある二つのＤＬＬを、プロジェクト「MieOELib」の refs フォルダーにコピーし「参照設定」および「ローカルにコピー」を true に設定してください。
- OEICommon.dll
- OEIFormats.dll 


日本語化MODを作成するツールは以下の三つです。
1. MieDataConverter:  ゲームの各種ファイルから日本語化に必要な情報をDatabase(SQLite)に格納する
2. MieStringMarger:  Database と CSV形式翻訳シートから、日本語化MODを作成する
3. MieFontConverter:  PoE2のフォントデータ(MonoBehaviour)を変換する

### MieDataConverter ###
```
SystemDBを初期化する。
  usage: MieDataConverter.exe --init -s <SystemDB> -q <schema>
SystemDBにデータを取り込む。
  usage: MieDataConverter.exe -l <Lang> -d <Design> -s <SystemDB>
OPTIONS:
  -l, --lang=VALUE           言語フォルダーのパス名。例：localized\en
  -d, --design=VALUE         付加フォルダーのパス名。例：design
  -s, --systemdb=VALUE       System Databaseのパス。
  -p, --product=VALUE        ProductLine
  -q, --sql=VALUE            システムDBのスキーマのパス
      --init                 データベースを初期化する。
  -r, --replace              DBがすでに存在する場合は、上書きする。
  -h, --help                 ヘルプ
Example:
  SystemDBにデータを取り込む。
    MieDataConverter.exe -l data\localized\en -d data\design -s PoE2SystemDB.sqlite
終了コード:
 0  正常終了
 1  異常終了
```

### MieStringMarger ###
```
日本語化MODを作成する。
  usage: MieStringMarger.exe -c <Trans Sheet(conv) path> -s <Trans Sheet(system) path> -t <Trans Sheet(chatter) path> -d <system db path> -o <japanized lang folder path> [-r]
OPTIONS:
  -c, --conv=VALUE           翻訳シート（会話）のパス。
  -s, --system=VALUE         翻訳シート（システム）のパス。
  -t, --chatter=VALUE        翻訳シート（Chatter）のパス。
  -d, --db=VALUE             SystemDBのパス。
  -o, --out=VALUE            出力フォルダーのパス
  -r                         出力用言語ファイルが既に存在する場合はを上書きする。
  -h, --help                 ヘルプ
Example:
  翻訳シート(-c,-s,-t)とSystemDB(-d)から日本語化MODをフォルダー(-o)に作成する。
    MieStringMarger.exe -c PoE2Sheet(conv).csv -s PoE2Sheet(system).csv -t PoE2Sheet(chatter).csv -d PoE2SystemDB.sqlite -o jpmod -r
終了コード:
 0  正常終了
 1  異常終了
```

### MieFontConverter ###
```
TMPの座標情報を変換する。
  usage: MieFontConverter.exe -s <source> -t <target> -f <format> [-o <output>] [-l]
OPTIONS:
  -s, --source=VALUE         変換元の座標情報ファイルのパス名。
  -t, --target=VALUE         変換先の座標情報ファイルのパス名。
  -o, --output=VALUE         出力フォルダーのパス。
                               省略時は変換先フォルダー内に'new_font'フォルダーを作成する。
  -f, --format=VALUE         座標ファイルの出力形式。
                               例：PoE2 の場合は -f PoE2
  -a, --ascender             Ascenderの値を調整する。
                               PoE2のAIエディターで文字が表示されない場合にのみ指定する。
  -l, --list                 検証リストを出力する。
  -d, --detail               検証リスト出力時に詳細情報を出力する。
  -h, --help                 ヘルプ
Example:
  変換元(-s)と変換先(-t)を読み込み、new_fontフォルダーに'PoE2'形式で座標情報と検証リストを出力する。
    MieFontConverter.exe -s resources_00014.-2 -t sharedassets0_00001.114 -o new_font -f PoE2 -l
終了コード:
 0  正常終了
 1  異常終了
```
