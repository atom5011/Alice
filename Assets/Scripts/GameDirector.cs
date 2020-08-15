using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GameDirector : MonoBehaviour
{
    public TextMessageViewer textMessageViewer;

    private int currentScenarioNo;

    void Start()
    {
        // シナリオ番号の初期値をセット
        currentScenarioNo = 0;

        // 最初に再生するシナリオを読み込んでゲームスタート
        ProcScenarioData(currentScenarioNo);
    }

    /// <summary>
    /// 再生するシナリオを準備
    /// </summary>
    private void ProcScenarioData(int nextScenarioNo)
    {
        // 現在のシナリオ番号を次のシナリオ番号に更新
        currentScenarioNo = nextScenarioNo;

        // currentScenarioNoと合致するシナリオをシナリオデータから検索して、再生するシナリオを決定
        ScenarioMasterData.ScenarioData scenarioData = GameData.instance.scenarioSO.scenarioMasterData.scenario.Find(x => x.scenarioNo == currentScenarioNo);

        // 文字送りをするクラスにシナリオをセットして、メッセージを再生する(該当するメソッドを次の手順で追加するので、それまでこの処理はコメントアウトしておきます)
        //textMessageViewer.SetUpScenarioData(scenarioData);
    }
}
