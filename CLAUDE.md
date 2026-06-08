# kmodel

## アーキテクチャ
- KintoneNetLibrary.CodeGenのラッパー
- Clean Architecture（Domain / Application / Infrastructure / Api / DataBuilder）
- Domain は他プロジェクトを参照しない

## ビルド・テスト
- dotnet build kmodel.sln
- dotnet test

## コーディングルール
- コメントは日本語を使用
- インスタンス変数、インスタンスメソッドへのアクセスには`this`を付加する
- `if`,`for`,`foreach`など`{}`が不要な場合でも必ず`{}`を使用する
