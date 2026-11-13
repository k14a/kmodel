# kmodel

## アーキテクチャ
- KintoneNetLibrary.CodeGenのラッパー
- Clean Architecture（Domain / Application / Infrastructure / Api / DataBuilder）
- Domain は他プロジェクトを参照しない

## 公開リポジトリとしての注意
このリポジトリは OSS として公開している。README・TODO.md・CHANGELOG.md・コードのコメント・テスト・コミットメッセージ等には、
利用側のプロジェクトや社内の固有名詞（プロジェクト名・クラス名・テーブル名・取引先や媒体の名前・社内のファイルパス等）や、
kintone のサブドメイン・アプリ名・アプリ ID・フィールドコード・API トークンを書かない。例には `example.cybozu.com` などの架空の値を使う。

## ビルド・テスト
- dotnet build kmodel.slnx
- dotnet test

## コーディングルール
- コメントは日本語を使用
- インスタンス変数、インスタンスメソッドへのアクセスには`this`を付加する
- `if`,`for`,`foreach`など`{}`が不要な場合でも必ず`{}`を使用する
