# Othello_Cpp
##  概要
- ローカルネットワーク対戦とCPU対戦が行えるオセロアプリ
- 制作期間：1ヶ月（2023年7月）
- チーム構成：1人（個人制作）
- 開発環境：C++ /Visual Studio 2022

## 工夫点
- 配置する場所の指定を提示された配置できる箇所の一覧にある番号から指定するようにして、入力時にマス目を数えずに入力できるようにしました。
- オート設置機能を追加することによって、デバッグをしやすくしました。

### 振り返りと現在の視点
- 保守性への意識: 本作品は制作当時、コードの簡潔さを優先しコメント記述が不足している箇所があります。現在は、チーム開発を想定した可読性の高いコード設計とドキュメント化を最優先に意識して制作しています。
- ユーザーインターフェース: マス目指定の簡略化など、当時からUXの向上を意識していましたが、現在はエラーハンドリングや視覚的なフィードバックをより強化し、ユーザーが迷わない設計を心がけています。

## 実行フォルダへのリンク
- [実行フォルダはこちら](https://github.com/Ton-1211/Game-Portfolio/tree/main/Othello_Cpp/BuildFile)

## 主なソースコードの概要とリンク
|スクリプト名（リンク）|概要|
|:---|:---|
|[Board.cpp](https://github.com/Ton-1211/Game-Portfolio/blob/main/Othello_Cpp/ProjectFile(Othello_Cpp)/Othello_Cpp/Board.cpp)|設置可能な場所の計算や盤面の処理、表示を行います。|
|[Player.cpp](https://github.com/Ton-1211/Game-Portfolio/blob/main/Othello_Cpp/ProjectFile(Othello_Cpp)/Othello_Cpp/Player.cpp)|設置可能な場所の取得やCPUの設置箇所の設定を行います。|
|[Mode.cpp](https://github.com/Ton-1211/Game-Portfolio/blob/main/Othello_Cpp/ProjectFile(Othello_Cpp)/Othello_Cpp/Mode.cpp)|実行モードの設定や通信処理を行っています。|