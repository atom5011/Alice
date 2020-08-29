using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;    // <= Linqを使用する場合には宣言が必要

public class GameData : MonoBehaviour
{
    public static GameData instance;    // 自分自身（GameDataクラス）を代入する変数

    public float BGM_Volume = 1.0f;    
    public float SE_Volume = 1.0f;     
    public bool Mute = false;

    public ScenarioSO scenarioSO;       // スクリプタブル・オブジェクトを代入する変数

    void Awake()
    {
        // シングルトンにする
        if (instance == null)
        {
            // instanceの値がnullの場合には、変数に自分自身（GameDataクラス）を入れる
            instance = this;

            // シーン遷移をしても破棄されないメソッドを実行する（対象となるのは引数に指定したもの。ここではGameDataのアタッチされているゲームオブジェクト）
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            // instanceがnullではない場合にはヒエラルキー上にすでにGameDataクラスが存在しているので、重複するGameDataを破棄する
            Destroy(this.gameObject);
        }
        // ScenarioDataのリストを作成
        CreateScenarioDataList();
    }

    /// <summary>
    /// ScenarioDataのリストを作成
    /// </summary>
    private void CreateScenarioDataList()
    {
        // スクリプタブル・オブジェクトを初期化
        scenarioSO.scenarioMasterData = new ScenarioMasterData();

        // Jsonファイルを元にSenarioDataを作成して、戻り値を受け取る
        scenarioSO.scenarioMasterData = LoadMasterDataFromJson.LoadScenarioMasterDataFromJson();

        // 文字列を適宜な型に変換して配列に代入
        foreach (ScenarioMasterData.ScenarioData scenarioData in scenarioSO.scenarioMasterData.scenario)
        {
            // 1行にまとめられているメッセージをカンマの位置で区切って配列に順番に入れる
            scenarioData.messages = scenarioData.messageString.Split(',').ToArray();

            // 1行にまとめられているキャラ名をカンマの位置で区切って、Selectを使い、文字列をEnumで用意したキャラの名前に型変換（キャスト）して配列に順番に入れる
            scenarioData.charaTypes = scenarioData.charaNoString.Split(',').Select(x => (CHARA_NAME_TYPE)Enum.Parse(typeof(CHARA_NAME_TYPE), x)).ToArray();

            // 1行にまとめられている分岐の番号をカンマの位置で区切って、Selectを使い、文字列からint型に型変換して配列に入れる
            scenarioData.branchs = scenarioData.branchString.Split(',').Select(x => int.Parse(x)).ToArray();

            scenarioData.branchMessages = scenarioData.branchMessageString.Split(',').ToArray();　　//分岐メッセージを配列にする

            // 1行にまとめられているメッセージ表示に合わせて表示するキャラの名前を半角スラッシュの位置で区切ってリストに入れる
            List<string> strList = scenarioData.displayCharaString.Split('/').ToList();

            int i = 0;

            // Dictinaryを初期化
            scenarioData.displayCharas = new Dictionary<int, CHARA_NAME_TYPE[]>();

            // foreachを使いリストの中身を１つずつ取り出してstr変数に代入
            foreach (string str in strList)
            {
                // 1行にまとめらているメッセージ表示に合わせて表示するキャラの名前をカンマで区切って、Selectを使い、文字列をEnumで用意したキャラの名前に型変換（キャスト）して配列に順番に入れる
                CHARA_NAME_TYPE[] displayChara = str.Split(',').Select(x => (CHARA_NAME_TYPE)Enum.Parse(typeof(CHARA_NAME_TYPE), x)).ToArray();
                // Dictinaryに追加
                scenarioData.displayCharas.Add(i, displayChara);
                i++;
            }
        }
        Debug.Log("Create ScenarioDataList");
    }
}