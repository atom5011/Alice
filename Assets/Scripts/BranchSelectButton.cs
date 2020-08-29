using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]    // RequireComponent属性。引数で指定したコンポーネントのアタッチを強要する。アタッチされていない場合にはAddする。
public class BranchSelectButton : MonoBehaviour
{
    public Text txtBranchMessage;　　　　　// 選択肢用メッセージ表示の紐づけ用。このTextコンポーネントに分岐用メッセージを表示する
    public Button btnBranchSelectButton;   // ボタンにメソッドを登録させるために用意
    public CanvasGroup canvasGroup;        // ゲームオブジェクトの透明度(アルファ)を操作するため

    public int branchNo;                   // 分岐するシナリオ番号。この番号が次に再生されるシナリオの番号になる
    public bool isClickable;               // 重複タップ防止判定用
    public Ease easeType;                  // 移動アニメのタイプをインスペクター上で選択可能にする。基本はLinear

    private GameDirector gameDirector;     // タップした際にGameDirectorクラスのメソッドを呼び出すため紐づけをしておく
    private Sequence sequence;             // DoTweenのシーケンスを使用するために宣言しておく

    /// <summary>
    /// 分岐選択肢ボタンの初期設定
    /// </summary>
    /// <param name="message">分岐のメッセージ</param>
    /// <param name="no">分岐先となるシナリオデータの番号</param>
    /// <param name="director">GameDirector</param>
    public void InitializeBranchSelect(string message, int no, GameDirector director, int count)
    {
        // ゲームオブジェクトを透明にし、位置の調整
        canvasGroup.alpha = 0.0f;
        transform.position = new Vector3(transform.position.x, transform.position.y - (count * 150), transform.position.z);

        // シーケンス初期化
        sequence = DOTween.Sequence();

        // シーケンスを利用してアニメ処理(透明な状態で画面左端から中央にアニメ移動して表示)
        sequence.Append(transform.DOLocalMoveX(2000, 1.0f).SetEase(easeType));
        sequence.Join(canvasGroup.DOFade(1.0f, 1.0f));

        // 初期設定
        txtBranchMessage.text = message;
        branchNo = no;
        gameDirector = director;

        // ボタンにメソッドを設定(Buttonコンポーネントのインスペクタ－上で設定するOnClickと同じもの)
        btnBranchSelectButton.onClick.AddListener(OnClickChooseBranch);
    }

    /// <summary>
    /// 分岐選択肢ボタンを押すと呼ばれる
    /// 選択した分岐の番号をGameDirectorへ渡す
    /// </summary>
    private void OnClickChooseBranch()
    {
        if (isClickable)
        {
            // 一度タップしたら処理しない
            return;
        }

        // 一度タップしたら押せなくする
        isClickable = true;

        // 他の分岐選択肢ボタンを押せなくする
        gameDirector.InactiveBranchSelectButtons();

        // シーケンス初期化
        sequence = DOTween.Sequence();

        // 画面中央から画面右端にアニメ移動し、徐々に透明化
        sequence.Append(transform.DOLocalMoveX(4000, 1.0f).SetEase(easeType))
            .Join(canvasGroup.DOFade(0.0f, 1.0f))
            .AppendCallback(() => {
                Debug.Log("移動終了");
                // 選択した分岐のシナリオ番号を渡して次のシナリオの再生準備をする
                gameDirector.ChooseBranch(branchNo);　　　// 次の手順が終わるまでコメントアウトしておく
            }
        );
    }
}