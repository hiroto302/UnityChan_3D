using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneManager : MonoBehaviour
{
    // public staticのついた変数は(クラス名).(変数名)で取得可能になる
    // 自身を表すLoadSceneManagerを保持して置くloadSceneManagerフィールドを作成しstaticを付けてインスタンス化することなく使用出来るようにした
    public static LoadSceneManager loadSceneManager;
    // シーン移動に関するデータファイル
    [SerializeField]
    private SceneMovementData sceneMovementData = null;
    // フェードプレハブ
    [SerializeField]
    private GameObject fadePrefab = null;
    // フェードインスタンス
    private GameObject fadeInstance;
    // フェードの画像
    private Image fadeImage;
    [SerializeField]
    private float fadeSpeed = 5f;

    // AwakeメソッドはStartメソッドより早く実行される
    // AwakeメソッドではloadSceneManagerが設定されていなければ自身のスクリプトを設定し、DontDestroyOnLoadメソッドを使って自身が取り付けてあるSceneManagerゲームオブジェクトをシーンを移動しても残すようにします。

    private void Awake() {
        // LoadSceneMangerは常に一つだけにする
        // シーン遷移した時はloadSceneManagerが既に設定されているはずなのでDestoryでゲームオブジェクトを削除し、SceneManagerゲームオブジェクトが複数存在しないようにする
        if(loadSceneManager == null) {
            loadSceneManager = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
    // 次のシーンを呼び出す
    // IEnumerator型を戻り値とした関数を定義することで、その関数をコルーチンとして扱うことが出来る
    // IEnumerator型はコレクションを扱うためのインターフェースなので、using System.Collections;しておく必要がある
    public void GoToNextScene(SceneMovementData.SceneType scene) {
        sceneMovementData.SetSceneType(scene);
        StartCoroutine(FadeAndLoadScene(scene));
    }

    // フェードアウト→シーンの読み込み→フェードインという処理を作成
    // フェードをした後にシーン読み込み
    IEnumerator FadeAndLoadScene(SceneMovementData.SceneType scene) {
        // フェードUIのインスタンス化
        fadeInstance = Instantiate<GameObject>(fadePrefab);
        fadeImage = fadeInstance.GetComponentInChildren<Image>();
        // フェードアウト処理
        yield return StartCoroutine(Fade(1f));

        // シーンの読み込み
        if (scene == SceneMovementData.SceneType.FirstVillage) {
            yield return StartCoroutine(LoadScene("UnityChan3D"));
        } else if (scene == SceneMovementData.SceneType.FirstVillageToWorldMap) {
            yield return StartCoroutine(LoadScene("SecondStage"));
        }

        // フェードUIのインスタンス化
        fadeInstance = Instantiate<GameObject>(fadePrefab);
        fadeImage = fadeInstance.GetComponentInChildren<Image>();
        fadeImage.color = new Color(0f, 0f, 0f, 1f);

        // フェードイン処理
        yield return StartCoroutine(Fade(0f));

        Destroy(fadeInstance);
    }
    // フェード処理
    // fadeImangeのアルファ値を変化させ、fadeImageのアルファ値と目的のアルファ値であるalphaを引いてMathf.Absで絶対値を求め、その差が0.01より大きい間はフェード処理を続ける
    // それ以外の時はコルーチンが終了し次の処理を行う
    IEnumerator Fade(float alpha) {
        var fadeImageAlpha = fadeImage.color.a;

        while (Mathf.Abs(fadeImageAlpha - alpha) > 0.01f) {
            fadeImageAlpha = Mathf.Lerp(fadeImageAlpha, alpha, fadeSpeed * Time.deltaTime);
            fadeImage.color = new Color(0f, 0f, 0f, fadeImageAlpha);
            yield return null;
        }
    }
    // 実際にシーンを読み込む処理
    // LoadSceneメソッドではSceneManager.LoadSceneAsyncを使って引数で受け取ったシーンの非同期な読み込みをします
    // SceneManager.LoadSceneAsyncの戻り値はAsyncOperationでそのisDoneプロパティで読み込みが終了したかどうかが判定
    IEnumerator LoadScene(string sceneName) {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);

        while (!async.isDone) {
            yield return null;
        }
    }
}