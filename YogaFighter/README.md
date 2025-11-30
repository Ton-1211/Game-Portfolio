# Yoga Fighter
## 作品情報
- コンセプト：体を伸ばして攻撃を吸収、反撃していくフィットネスゲーム
- 制作期間：10ヶ月（2024年4月～2025年1月）
- チーム構成：プランナー1人、プログラマー2人（本人含む）、デザイナー2人
- 開発環境：Unity(2021.3.23f1)/Visual Studio 2022
- 担当箇所：
    - ゲーム全体の処理や管理の設計・実装
    - 敵弾の生成パターンを設定・JSON形式で出力・編集できるエディタツールの作成
- **使用機材**:**[Sony mocopi]**(https://www.sony.co.jp/en/Products/mocopi-dev/jp/documents/Home/Aboutmocopi.html)
- **使用プラグイン**：**[mocopi Receiver Pluguin for Unity]**(https://www.sony.co.jp/en/Products/mocopi-dev/jp/documents/ReceiverPlugin/Unity/AboutPlugin.html)  
当初はNintendo SwitchのJoy-conを用いたトラッキングを考えていました。  
しかし腕や足、頭等の全身の動きを使用者の負担をより少なくトラッキングするために、さらに軽量で小型なモーションキャプチャーデバイス「mocopi」を採用しています。  
また、mocopiのスマートフォンアプリからトラッキング情報をUnityに通信しモデルに適用するため、上記プラグインを導入しています。
![両手首、両足首、後頭部、腰の6点でトラッキングを行っています。](Tracking.png)

## 工夫点
- 敵の攻撃の位置や時間の設定を設定・調整を行いやすくするために、ツールで設定を行えるようにして、JSONでの書き出し、読み込みで編集できるようにしました。  
攻撃も、JSONファイルを登録するだけにして順番の調整も行いやすくしました。  
![実際のツール画面です。JSONファイルを読み込んで編集する事もできます。](AttackEditor.png)
![同名のファイルを保存する前には、上書き保存をするか確認するダイアログを出しています。](ConfirmationDialog.png)

- 敵の攻撃が飛んでくる場所がどこか（どこに手足を動かせば良いのか）をわかりやすくするため、到着予測地点を表示するようにしました。
![表示された予測地点に実際に攻撃が到達している様子です。](Prediction.png)

## 動画ファイル・実行フォルダへのリンク
- [動画ファイルはこちら](https://github.com/Ton-1211/DAG-programmer-task/blob/main/YogaFighter/YogaFighter_%E3%83%97%E3%83%AC%E3%82%A4%E5%8B%95%E7%94%BB.mp4)
※GitHub上では動画を再生できません。「View raw」を押してダウンロードして視聴をお願いします。

- [実行フォルダはこちら](https://github.com/Ton-1211/DAG-programmer-task/tree/main/YogaFighter/BuildFile(YogaFighter))

## 主なソースコードの概要とリンク
|スクリプト名（リンク）|概要|
|:---|:---|
|[GameFlowManager.cs](https://github.com/Ton-1211/DAG-programmer-task/blob/main/YogaFighter/ProjectFile/YogaFighter/Assets/Scripts/GameFlow/GameFlowManager.cs)|ゲームの流れを行います。(リザルト表示も含む)|
|[BOSSAttackJSONEditor.cs](https://github.com/Ton-1211/DAG-programmer-task/blob/main/YogaFighter/ProjectFile/YogaFighter/Assets/Scripts/Editor/BossAttackJSONEditor.cs)|敵の攻撃の設定とJSONファイルへの書き出し、編集を行う|
|[JSONReader.cs](https://github.com/Ton-1211/DAG-programmer-task/blob/main/YogaFighter/ProjectFile/YogaFighter/Assets/Scripts/JSONReader.cs)|JSONファイルを読み込んで敵の攻撃を生成します。|
|[AttackBulletScript.cs](https://github.com/Ton-1211/DAG-programmer-task/blob/main/YogaFighter/ProjectFile/YogaFighter/Assets/Scripts/AttackBulletScript.cs)|敵弾の移動や吸収処理、到達予測地点の表示を行います。|