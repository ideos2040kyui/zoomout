using UnityEngine;
using TMPro; // TextMeshPro を使うために必要

public class GameSystem : MonoBehaviour
{
    [Header("ゲームオブジェクト設定")]
    [Tooltip("大きさの変わる絵画のTransform")]
    public Transform paintingTransform;

    [Tooltip("目標となる額縁のTransform")]
    public Transform frameTransform;

    [Header("UI設定")]
    [Tooltip("スコアを表示するUIテキスト (TextMeshPro)")]
    public TextMeshProUGUI scoreText;

    [Tooltip("結果を表示するUIテキスト (TextMeshPro)")]
    public TextMeshProUGUI resultText;

    [Header("ゲーム難易度設定")]
    [Tooltip("絵画の最初の大きさ")]
    public float initialScale = 5.0f;

    [Tooltip("絵画が小さくなるスピード")]
    public float scaleDownSpeed = 0.05f;

    // ゲームが進行中かどうかを管理するフラグ
    private bool isGameActive = true;
    // ぴったりサイズの目標となるスケール値
    private float targetScale = 0.266f;

    /// <summary>
    /// ゲーム開始時に一度だけ呼ばれる処理
    /// </summary>
    void Start()
    {
        InitializeGame();
    }

    /// <summary>
    /// フレームごとに毎回呼ばれる処理
    /// </summary>
    void Update()
    {
        // ゲームが終了していたら、Rキーでのリスタート待機状態にする
        if (!isGameActive)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                InitializeGame(); // ゲームを初期化して再スタート
            }
            return; // ゲームが終了しているので、以降の処理は行わない
        }

        // ゲームが進行中なら、絵画を徐々に小さくする
        ScaleDownPainting();

        // マウスの左クリックを検出
        if (Input.GetMouseButtonDown(0))
        {
            // 縮小を止めて、スコアを計算する
            StopAndEvaluate();
        }
    }

    /// <summary>
    /// ゲームを初期状態に戻す処理
    /// </summary>
    void InitializeGame()
    {
        isGameActive = true;

        // 絵画を初期サイズに設定
        paintingTransform.localScale = Vector3.one * initialScale;

        // UIテキストを初期状態に設定
        scoreText.text = "クリックでストップ！";
        resultText.text = "ひきすぎ注意！";
    }

    /// <summary>
    /// 絵画を小さくする処理
    /// </summary>
    void ScaleDownPainting()
    {
        // 現在のスケールを取得
        float currentScale = paintingTransform.localScale.x;
        float limitScale = 0.001f;

        // 目標スケールに向かって一定の速度で減少
        float newScale = Mathf.MoveTowards(currentScale, limitScale, scaleDownSpeed * Time.deltaTime);

        // tar
        if (newScale > limitScale)
        {
            paintingTransform.localScale = new Vector3(newScale, newScale, newScale);
        }
        else
        {
            // もし小さくなりすぎたら、自動的にゲームを終了させる
            paintingTransform.localScale = Vector3.zero;
            StopAndEvaluate();
        }
    }

    /// <summary>
    /// 絵画の動きを止めて、スコアを計算・表示する処理
    /// </summary>
    void StopAndEvaluate()
    {
        isGameActive = false; // ゲームを終了状態にする

        // 現在の絵画のスケールと、目標の額縁スケールの差（の絶対値）を計算
        float difference = paintingTransform.localScale.x - targetScale;

        // 評価とスコアを決定
        float score;
        string resultMessage;

        if (difference < 0.0f)
        {
            score = 0.0f;
            resultMessage = "ひきすぎだよ...";
        }
        else
        {
            score = 100.0f - difference * 150;
            if (score == 100.0f)
            {
                resultMessage = "天才！！";
            }
            else if (score > 90.0f)
            {
                resultMessage = "すごい！！";

            }
            else if (score > 80.0f)
            {
                resultMessage = "なかなかやるな";
            }
            else if (score > 60.0f)
            {
                resultMessage = "まずまずだな";
            }
            else
            {
                resultMessage = "もう少しがんばろう...";
            }
            if (score < 0.0f)
            {
                score = 0.0f;
                resultMessage = "何の絵かわからんよ...";
            }
        }

        // UIに結果を反映
        scoreText.text = "スコア: " + score;
        resultText.text = resultMessage + "\n<size=24>Rキーでリスタート</size>"; // 評価とリスタート案内を表示
    }
}
