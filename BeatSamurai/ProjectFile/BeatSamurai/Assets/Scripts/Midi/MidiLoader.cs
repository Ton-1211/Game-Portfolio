using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class MidiLoader
{
    public List<MidiData.NoteData> noteList = new List<MidiData.NoteData>();
    public List<MidiData.TempoData> tempoList = new List<MidiData.TempoData>();

    float musicTime = 0f;
    public void LoadMIDI(string filePath)
    {
        noteList.Clear();
        tempoList.Clear();

        using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using(BinaryReader reader = new BinaryReader(stream))
        {
            /* ヘッダチャンク侵入 */
            MidiData.HeaderChunkData headerChunk = new MidiData.HeaderChunkData();

            headerChunk.chunkID = reader.ReadBytes(4);// チャンクID
            // 自分のPCがリトルエンディアンならバイト順を逆にする
            if(BitConverter.IsLittleEndian)
            {
                // ヘッダ部のデータ長(値は6固定)
                byte[] byteArray = reader.ReadBytes(4);
                Array.Reverse(byteArray);
                headerChunk.dataLength = BitConverter.ToInt32(byteArray, 0);

                // フォーマット（2byte）
                byteArray = reader.ReadBytes(2);
                Array.Reverse(byteArray);
                headerChunk.format = BitConverter.ToInt16(byteArray, 0);

                // トラック数（2byte）
                byteArray = reader.ReadBytes(2);
                Array.Reverse(byteArray);
                headerChunk.tracks = BitConverter.ToInt16(byteArray, 0);

                // タイムベース（2byte）
                byteArray = reader.ReadBytes(2);
                Array.Reverse(byteArray);
                headerChunk.division = BitConverter.ToInt16(byteArray, 0);
            }
            else
            {
                // ヘッダ部のデータ長（値は6固定）
                headerChunk.dataLength = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                // フォーマット（2byte）
                headerChunk.format = BitConverter.ToInt16(reader.ReadBytes(2), 0);
                // トラック数（2byte）
                headerChunk.tracks = BitConverter.ToInt16(reader.ReadBytes(2), 0);
                // タイムベース（2byte）
                headerChunk.division = BitConverter.ToInt16(reader.ReadBytes(2), 0);
            }
            Debug.Log("トラック数" + headerChunk.tracks);

            /* トラックチャンク侵入 */
            MidiData.TrackChunkData[] trackChunks = new MidiData.TrackChunkData[headerChunk.tracks];

            for (int i = 0; i < headerChunk.tracks; i++)
            {
                trackChunks[i].chunkID = reader.ReadBytes(4);// チャンクID
                                                         
                if (BitConverter.IsLittleEndian)// リトルエンディアンのときの変換
                {
                    // トラックのデータ長読み込み
                    byte[] byteArray = reader.ReadBytes(4);
                    Array.Reverse(byteArray);
                    trackChunks[i].dataLength = BitConverter.ToInt32(byteArray, 0);
                }
                else
                {
                    trackChunks[i].dataLength = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                }

                // データ部読み込み
                trackChunks[i].data = reader.ReadBytes(trackChunks[i].dataLength);

                TrackDataAnalysis(trackChunks[i].data, headerChunk);
            }
        }
        ModificationEvantTimes();

        Debug.Log("テンポ種類:" + tempoList.Count + "個");
        
        //for(int i = 0; i < noteList.Count; i++)
        //{
        //    musicTime += noteList[i].eventTime;
        //}
        Debug.Log("曲の長さ:" + musicTime);
        for(int i = 0; i < tempoList.Count; i++)
        {
            Debug.Log("BPM変化タイミング:" + tempoList[i].eventTime + " BPM:" + tempoList[i].bpm + " Tick:" + tempoList[i].tick);
        }
    }

    /// <summary>
    /// トラックデータ解析
    /// </summary>
    public void TrackDataAnalysis(byte[] data, MidiData.HeaderChunkData headerChunk)
    {
        uint currentTime = 0;               // 現在の時間
        byte statusByte = 0;
        bool[] longFlags = new bool[128];   // ロングノーツ用のフラグ

        for(int i =0; i < data.Length;)// データ分回す
        {
            uint deltaTime = 0;

            while(true)
            {
                byte temp = data[i++];

                deltaTime |= temp & (uint)0x7f; // 下位7bitを格納
                if ((temp & 0x80) == 0) break;  // 最上位1bitが0（かたまりの終わり）ならデータ終了
                deltaTime = deltaTime << 7;                // 次の下位7bit用にビット移動
            }
            deltaTime += deltaTime;

            /* ランニングステータスチェック */
            if (data[i] < 0x80)
            {
                // ランニングステータス適応（前回のステータスバイトを再使用）
            }
            else
            {
                statusByte = data[i++];// ステータスバイトを保存
            }

            byte dataByte0, dataByte1, dataLength;

            /* MIDIイベント（ステータスバイト0x80～0xEF） */
            if(statusByte >= 0x80 && statusByte <= 0xef)
            {
                switch(statusByte & 0xf0)
                {
                    /* チャンネルメッセージ */
                    case 0x80:// ノートオフ
                        dataByte0 = data[i++];// どのキーが話されたか
                        dataByte1 = data[i++];// ベロシティ値
                        if (longFlags[dataByte0])// 前のレーンがロングノーツのとき
                        {
                            // ロング終了地点のノート情報を作成
                            MidiData.NoteData note = new MidiData.NoteData();
                            note.eventTime = (int)currentTime;
                            note.laneIndex = (int)dataByte0;
                            note.type = MidiData.NoteType.LongEnd;

                            noteList.Add(note);// リストに追加
                            longFlags[note.laneIndex] = false;// ロングノーツのフラグを解除
                        }
                        break;
                    case 0x90:// ノートオン（ノートオフが呼ばれるまで押しっぱなし扱い）
                        dataByte0 = data[i++];// どのキーが押されたか
                        dataByte1 = data[i++];// ベロシティ値、音の強さ
                        {
                            // ノート情報を生成
                            MidiData.NoteData note = new MidiData.NoteData();
                            note.eventTime = (int)currentTime;
                            note.laneIndex = (int)dataByte0;
                            note.type = MidiData.NoteType.Normal;
                            if(dataByte1 == 127)// ベロシティ値が最大のときだけロングの始点としている
                            {
                                note.type = MidiData.NoteType.LongStart;
                                longFlags[note.laneIndex] = true;
                            }
                            if(dataByte1 == 0)// ノートオフイベントではなく、ベロシティ値0をノートオフとしている形式に対応
                            {
                                if (longFlags[note.laneIndex])// 同じレーンで前回がロングノーツ始点のとき
                                {
                                    note.type = MidiData.NoteType.LongEnd;
                                    longFlags[note.laneIndex] = false;
                                }
                            }
                            noteList.Add(note);
                        }
                        break;
                    case 0xa0:// ポリフォニック キープレッシャー
                        i += 2;// 使用しないのでスルー
                        break;
                    case 0xb0:// コントロールチェンジ
                        dataByte0 = data[i++];// コントロールする番号
                        dataByte1 = data[i++];// 設定する値

                        // 0x00～0x77までがコントロールチェンジで、それ以上はチャンネルメッセージとして処理する
                        if(dataByte0 < 0x78)
                        {
                            // コントロールチェンジ
                        }
                        else
                        {
                            // チャンネルモードメッセージは一律データバイトを2つ使用している
                            // チャンネルモードメッセージ
                            switch(dataByte0)
                            {
                                case 0x78:// オールサウンドオフ
                                    // 該当するチャンネルの発音中の音を直ちに消音する（オールノートオフより強力）
                                    break;
                                case 0x79:// リセットオールコントローラ
                                    // 該当するチャンネルの全種類のコントロール値を初期化する
                                    break;
                                case 0x7a:// ローカルコントロール
                                    // オフ：鍵盤を弾いてもMIDIメッセージは送信されるだけでピアノ自体から音は出ない
                                    // オン：鍵盤を弾くと音源から音が出る
                                    break;
                                case 0x7b:// オールノートオフ
                                    // 該当するチャンネルの発音中の音すべてに対してノートオフ命令を出す
                                    break;
                                /* MIDIモード設定 */
                                // オムニのオン・オフ、モノ・ポリモードの4種類
                                case 0x7c:// オムニモードオフ
                                    break;
                                case 0x7d:// オムニモードオン
                                    break;
                                case 0x7e:// モノモードオン
                                    break;
                                case 0x7f:// モノモードオフ
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case 0xc0:// プログラムチェンジ（音色を変える命令）
                        i += 1;
                        break;
                    case 0xd0:// チャンネルプレッシャー
                        i += 1;
                        break;
                    case 0xe0:// ピッチベンド
                        i += 2;
                        break;
                    default:
                        break;
                }
            }
            /* システムエクスクルーシブ（SysEx）イベント */
            else if (statusByte == 0x70 || statusByte == 0x7f)
            {
                dataLength = data[i++];
                i += dataLength;
            }

            /* メタイベント */
            else if(statusByte == 0xff)
            {
                byte metaEventID = data[i++];// メタイベントの番号
                dataLength = data[i++];

                switch(metaEventID)
                {
                    case 0x00:  // シーケンスメッセージ
                        i += dataLength;
                        break;
                    case 0x01:  // テキストイベント
                        i += dataLength;
                        break;
                    case 0x02:  // 著作権表示
                        i += dataLength;
                        break;
                    case 0x03:  // シーケンス/トラック名
                        i += dataLength;
                        break;
                    case 0x04:  // 楽器名
                        i += dataLength;
                        break;
                    case 0x05:  // 歌詞
                        i += dataLength;
                        break;
                    case 0x06:  // マーカー
                        i += dataLength;
                        break;
                    case 0x07:  // キューポイント
                        i += dataLength;
                        break;
                    case 0x20:  // MIDIチャンネルプリフィクス
                        i += dataLength;
                        break;
                    case 0x21:  // MIDIポートプリフィックス
                        i += dataLength;
                        break;
                    case 0x2f:  // トラック終了
                        i += dataLength;
                        musicTime = currentTime;
                        break;
                    case 0x51:// テンポ変更
                        {
                            // テンポ変更情報リストに格納する
                            MidiData.TempoData tempoData = new MidiData.TempoData();
                            tempoData.eventTime = (int)currentTime;

                            // 4分音符の長さがマイクロ秒単位で格納されている
                            uint tempo = 0;
                            tempo |= data[i++];
                            tempo <<= 8;
                            tempo |= data[i++];
                            tempo <<= 8;
                            tempo |= data[i++];

                            tempoData.bpm = 60000000 / (float)tempo;// BPM割り出し
                            tempoData.bpm = Mathf.Floor(tempoData.bpm * 10) / 10;// 小数点第1で切り捨て処理
                            tempoData.tick = (60 / tempoData.bpm / headerChunk.division * 1000);// tick値割り出し
                            tempoList.Add(tempoData);// リスト追加
                        }
                        break;
                    case 0x54:// SMTPEオフセット
                        i += dataLength;
                        break;
                    case 0x58:// 拍子
                        i += dataLength;
                        break;
                    case 0x59:// 調号
                        i += dataLength;
                        break;
                    case 0x7f:// シーケンサ固有メタイベント
                        i += dataLength;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    void ModificationEvantTimes()
    {
        List<MidiData.TempoData> tempTempoList = new List<MidiData.TempoData>(tempoList);// 一次格納用（計算前の時間を保持したいため）

        for (int i = 1; i < tempoList.Count; i++)// テンポイベント時間修正
        {
            MidiData.TempoData tempo = tempoList[i];

            int timeDifference = tempTempoList[i].eventTime - tempTempoList[i - 1].eventTime;
            tempo.eventTime = (int)(timeDifference * tempoList[i - 1].tick) + tempoList[i - 1].eventTime;// 設定されているテンポで計算しなおす

            tempoList[i] = tempo;
        }

        for(int i = 0; i < noteList.Count; i++)// ノーツイベント時間修正
        {
            for(int j = tempoList.Count - 1; j >= 0; j--)
            {
                if (noteList[i].eventTime >= tempTempoList[j].eventTime)
                {
                    MidiData.NoteData note = noteList[i];

                    int timeDifference = noteList[i].eventTime - tempTempoList[j].eventTime;
                    note.eventTime = (int)(timeDifference * tempTempoList[j].tick) + tempoList[j].eventTime;// 計算後のテンポ変更イベント時間+そこからの自身の時間
                    noteList[i] = note;
                    break;
                }
            }
        }
    }
}
