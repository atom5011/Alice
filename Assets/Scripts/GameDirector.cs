using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using DG.Tweening;

public class GameDirector : MonoBehaviour
{
    public List<BranchSelectButton> branchSelectButtonList = new List<BranchSelectButton>();    
    public BranchSelectButton BranchSelectButtonPrefab;                                         // プレファブにしたBranchSelectButtonを登録
    public Transform branchButtonTran;                                                          // BranchSelectButtonを生成する地点

    public TextMessageViewer textMessageViewer;

    private int currentScenarioNo;

    void Start()
    {
        // シナリオ番号の初期値をセット
        currentScenarioNo = 0;

        // 最初に再生するシナリオを読み込んでゲームスタート
        ProcScenarioData(currentScenarioNo);

        // 分岐選択肢ボタン生成用のDebug処理
        //StartCoroutine(CreateBranchSelectButton(3));           // 分岐選択肢ボタンの生成処理。引数で指定された数だけボタンを生成する
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
        textMessageViewer.SetUpScenarioData(scenarioData);
    }
    /// <summary>
    /// 全メッセージ再生後に分岐用ボタンを作成(今回はDebugなので、すぐにボタンを生成)
    /// </summary>
    public IEnumerator CreateBranchSelectButton(string[] branchMessages)
    {
        // 引数の数だけボタンを生成
        for (int i = 0; i < branchMessages.Length; i++)
        {
            // 分岐選択肢ボタンの生成
            BranchSelectButton branchSelectButton = Instantiate(BranchSelectButtonPrefab, branchButtonTran, false);

            // ボタンの設定処理
            branchSelectButton.InitializeBranchSelect(branchMessages[i], i, this, i);

            // ボタン用Listにボタンを追加
            branchSelectButtonList.Add(branchSelectButton);

            // 0.5秒待ってから、次のボタンがある場合にはボタンを生成(まとめて生成ではなく、順番にボタン生成する)
            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// 選択した分岐の番号で次のシナリオを設定(BranchSelectButtonクラスから呼ばれる)
    /// </summary>
    /// <param name="branchNo"></param>
    public void ChooseBranch(int senarioNo)
    {
        Debug.Log("分岐選択 シナリオ番号 :" + senarioNo);

        // 次のシナリオの呼び出し
        ProcScenarioData(senarioNo);
    }

    /// <summary>
    /// タップされていない分岐ボタンを重複して押せないように制御
    /// </summary>
    public void InactiveBranchSelectButtons()
    {
        for (int i = 0; i < branchSelectButtonList.Count; i++)
        {
            if (!branchSelectButtonList[i].isClickable)
            {
                branchSelectButtonList[i].isClickable = true;
                branchSelectButtonList[i].canvasGroup.DOFade(0.0f, 0.5f);
            }
        }
        // 選択肢ボタンのリストをクリア
        branchSelectButtonList.Clear();
    }
}


