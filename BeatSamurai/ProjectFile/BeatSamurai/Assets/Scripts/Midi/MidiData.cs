using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://qiita.com/owts/items/8d641e8a8a423566db71を参考にした

public class MidiData
{
    public enum NoteType
    {
        Normal,     // 通常ノーツ
        LongStart,  // ロング開始地点
        LongEnd     // ロング終了地点
    }

    public struct NoteData
    {
        public int eventTime;   // ノーツタイミング（ms）
        public int laneIndex;   // レーン番号
        public NoteType type;   // ノーツの種類
    }

    public struct TempoData
    {
        public int eventTime;   // BPM変化のタイミング（ms）
        public float bpm;       // BPMの値
        public float tick;      // tickの値
    }

    /// <summary>
    /// ヘッダーチャンク情報を格納する構造体
    /// </summary>
    public struct HeaderChunkData
    {
        public byte[] chunkID;  // チャンクのIDを示す（4byte）
        public int dataLength;  // チャンクのデータ長（4byte）
        public short format;    // MIDIファイルフォーマット（2byte）
        public short tracks;    // トラック数（2byte）
        public short division;  // タイムベース 4分音符あたりのtick数
    }

    /// <summary>
    /// トラックチャンク情報を格納する構造体
    /// </summary>
    public struct TrackChunkData
    {
        public byte[] chunkID;  // チャンクのID（4byte）
        public int dataLength;  // チャンクのデータ長（4byte）
        public byte[] data;     // 演奏情報が入っているデータ
    }
}
