using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TextMessageViewer : MonoBehaviour
{
    public string[] messages;                                // 表示するメッセージの配列
    public Text txtMessage;                                  // メッセージ表示用
    public float wordSpeed;                                  // 1文字当たりの表示速度
    public Dictionary<int, CHARA_NAME_TYPE[]> displayCharas; //シナリオデータに用意されている立ち絵の情報を代入

    public int bgmNo;                                       //再生するBGMの設定番号
    public string[] branchMessages;                        //分岐用のメッセージの配列

    public Image imgBackground;                            //背景制御
    public List<DisplayChara> displayCharasList;             //立ち絵制御。imageだとEnebledを切っても変わらないのでクラスをリストにする。インスペクターで設定する
    public CHARA_NAME_TYPE[] charaTypes;                     // <= キャラの名前が入る配列
    public Text txtCharaType;                                // <= キャラの名前表示用

    public GameObject tapIconObj;             　             // <= タップを促す画像の制御用

    public int[] branchs;
    private int messagesIndex = 0;                           // 表示するメッセージの配列番号
    private int wordCount;                                   // １メッセージ当たりの文字の総数
    private bool isTapped = false;                           // 全文表示後にタップを待つフラグ
    private bool isDisplayedAllMessage = false;              // 全メッセージ表示完了のフラグ

    private IEnumerator waitCoroutine;          // <= 全文表示までの待機時間メソッド代入用。StopCoroutineできるようにしておく
    private Tween tween;                        // <= DoTween再生用。Killできるように代入して使用する

    public GameDirector gameDirector;


    void Start()
    {
        // タップアイコンを非表示にする　　　　
        tapIconObj.SetActive(false);

        // １文字ずつ文字を表示する処理をスタート
        StartCoroutine(DisplayMessage());     //  <= メソッドの呼び出し方法をStartCoroutineに変更
    }

    /// <summary>
    /// シナリオのメッセージや分岐などを設定
    /// </summary>
    /// <param name="scenarioData"></param>
    public void SetUpScenarioData(ScenarioMasterData.ScenarioData scenarioData)
    {
        Debug.Log("シナリオ番号 : " + scenarioData.scenarioNo + "のシナリオデータをセット");

        // シナリオの各データを準備
        messages = new string[scenarioData.messages.Length];
        messages = scenarioData.messages;

        charaTypes = new CHARA_NAME_TYPE[scenarioData.charaTypes.Length];
        charaTypes = scenarioData.charaTypes;

        branchs = new int[scenarioData.branchs.Length];
        branchs = scenarioData.branchs;

        displayCharas = new Dictionary<int, CHARA_NAME_TYPE[]>(scenarioData.displayCharas);

        //再生するBGMを設定
        bgmNo = scenarioData.bgmNo;

        //取得した番号のBGMを再生
        SoundManager.Instance.PlayBGM((SoundManager.BGM_Type)bgmNo);

        //分岐用のメッセージを設定
        branchMessages = new string[scenarioData.branchMessages.Length];
        branchMessages = scenarioData.branchMessages;

        // 初期化
        messagesIndex = 0;
        isDisplayedAllMessage = false;

        //シナリオの背景を設定
        imgBackground.sprite = Resources.Load<Sprite>("BackGround/" + scenarioData.backgroundImageNo);

        // 1文字ずつメッセージ表示を開始
        StartCoroutine(DisplayMessage());
        Debug.Log("シナリオ 再生開始");
    }

    void Update()
    {
        if (isDisplayedAllMessage)
        {
            // 全てのメッセージ表示が終了していたら処理を行わない
            return;
        }



        if (Input.GetMouseButtonDown(0) && tween != null)
        {
            // 文字送り中にタップした場合、文字送りを停止
            tween.Kill();
            tween = null;
            // 文字送りのための待機時間も停止
            if (waitCoroutine != null)
            {
                StopCoroutine(waitCoroutine);
                waitCoroutine = null;
            }
            // 全文をまとめて表示
            txtMessage.text = messages[messagesIndex];

            Debug.Log("文字送りスキップ 1ページまとめて 表示");

            // タップするまで全文を表示したまま待機
            StartCoroutine(NextTouch());
        }



        if (Input.GetMouseButtonDown(0) && wordCount == messages[messagesIndex].Length)
        {
            // 全文表示中にタップしたら全文表示を終了
            isTapped = true;
        }
    }

    /// <summary>
    /// １文字ずつ文字を表示
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisplayMessage()
    {    // <= 戻り値をvoidからIEnumerator型に変更
        isTapped = false;

        // 表示テキストをリセット
        txtMessage.text = "";

        txtCharaType.text = "";

        // Tweenをリセット
        tween = null;

        // 文字送りの待機時間を初期化
        if (waitCoroutine != null)
        {
            StopCoroutine(waitCoroutine);
            waitCoroutine = null;
        }

        // メッセージ表示中のキャラ名表示
        if (charaTypes[messagesIndex] != CHARA_NAME_TYPE.NO_NAME)
        {　　// <= CHARA_NAME_TYPEというenumを変数に代入せずにそのまま使用できます。[enum名.登録してあるキャラ名]で使用できます。
            // NoName以外の場合だけキャラ名表示
            txtCharaType.text = charaTypes[messagesIndex].ToString();  // <= enumはToString()メソッドで文字列に変換できます。その場合にはenumの部分は除いてキャラ名のみ変換対象になります。
        }

        //立ち絵表示するキャラを設定（メッセージ表示中のキャラだけはない）
        foreach (DisplayChara chara in displayCharasList)
        {
            chara.gameObject.SetActive(false);
            // 表示させるキャラの確認
            foreach (KeyValuePair<int, CHARA_NAME_TYPE[]> item in displayCharas)
            {
                // 何番目のメッセージか確認
                if (item.Key == messagesIndex)
                {
                    for (int i = 0; i < item.Value.Length; i++)
                    {
                        // 該当するキャラか確認
                        if (item.Value[i] == chara.charaNameType)
                        {
                            // 表示させる設定なら表示
                            chara.gameObject.SetActive(true);
                            Debug.Log(item.Value[i]);
                        }
                    }
                }
            }
        }



        // ① DoTweenの処理の前にWhile文を追加。1文字ずつの文字送り表示が終了するまでループ
        while (messages[messagesIndex].Length > wordCount)
        {

            // ② DoTweenの処理をtween変数に代入して使用するように修正
            tween = txtMessage.DOText(messages[messagesIndex], messages[messagesIndex].Length * wordSpeed).
                SetEase(Ease.Linear).OnComplete(() =>
                {
                    Debug.Log("文字送りで 全文表示 完了");
                    // (TODO) 他にも処理があれば追加する

                });

            // ③ 文字送り表示が終了するまでの待機時間を設定してコルーチン処理による待機を実行
            waitCoroutine = WaitTime();
            yield return StartCoroutine(waitCoroutine);
        }



        // タップするまで全文を表示したまま待機
        StartCoroutine(NextTouch());
    }


    /// <summary>
    /// 全文表示までの待機時間(文字数×1文字当たりの表示時間)
    /// タップして全文をまとめて表示した場合にはこの待機時間を停止
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(messages[messagesIndex].Length * wordSpeed);
    }



    /// <summary>
    /// タップするまで全文を表示したまま待機
    /// </summary>
    /// <returns></returns>
    private IEnumerator NextTouch()
    {
        yield return null;
        // 表示した文字の総数を更新
        wordCount = messages[messagesIndex].Length;

        // タップを促すイメージ表示
        tapIconObj.SetActive(true);

        // タップを待つ
        yield return new WaitUntil(() => isTapped);

        // タップを促すイメージを非表示
        tapIconObj.SetActive(false);

        // 次のメッセージ準備
        messagesIndex++;
        wordCount = 0;

        // リストに未表示のメッセージが残っている場合
        if (messagesIndex < messages.Length)
        {
            // １文字ずつ文字を表示する処理をスタート
            StartCoroutine(DisplayMessage());        //  <= メソッドの呼び出し方法をStartCoroutineに変更
        }
        else
        {
            // 全メッセージ表示終了
            isDisplayedAllMessage = true;

            Debug.Log("全メッセージ 表示終了");


            // エンディングか確認
            if (JudgeEnding())
            {
                // エンディングの場合の処理

                // 立ち絵キャラを非表示にする
                for (int i = 0; i < displayCharasList.Count; i++)
                {
                    displayCharasList[i].gameObject.SetActive(false);
                }

            }
            else
            {
                // 分岐ボタンの作成
                StartCoroutine(gameDirector.CreateBranchSelectButton(branchMessages));
            }
        }


    }
    private bool JudgeEnding()
    {
        // エンディングの条件によって分岐

        // Trueならエンディング

        return false;
    }
}