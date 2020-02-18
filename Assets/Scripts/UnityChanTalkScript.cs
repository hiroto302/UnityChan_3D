using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class UnityChanTalkScript : MonoBehaviour
{
    // 会話可能な相手
    private GameObject conversationPartner;
    // 会話可能アイコン
    [SerializeField]
    private GameObject talkIcon = null;

    // TalkUIゲームオブジェクト
    [SerializeField]
    private GameObject talkUI = null;
    // メッセージUI
    private Text messageText = null;
    // 表示するメッセージ
    private string allMessage = null;
    // 使用する分割文字列
    [SerializeField]
    private string splitString = "<>";
    // 分割したメッセージ
    private string[] splitMessage;
    // 分割したメッセージの何番目か
    private int messageNum;
    // テキストスピード
    [SerializeField]
    private float textSpeed = 0.05f;
    // 経過時間
    private float elapsedTime = 0f;
    // 今見ている文字番号
    private int nowTextNum = 0;
    // マウスクリックを促すアイコン
    [SerializeField]
    private Image clickIcon = null;
    // クリックアイコンの点滅秒数
    [SerializeField]
    private float clickFlashTime = 0.2f;
    // 1回分のメッセージを表示したかどうか
    private bool isOneMessage = false;
    // メッセージをすべて表示したかどうか
    private bool isEndMessage = false;

    void Start() {
        clickIcon.enabled = false;
        messageText = talkUI.GetComponentInChildren<Text>();
    }

    void Update() {
        // メッセージが終わっているか、メッセージがない場合はこれ以降何もしない
        if (isEndMessage || allMessage == null) {
            return;
        }

        // 1回に表示するメッセージを表示していない
        if (!isOneMessage) {
            // テキスト表示時間を経過したらメッセージを追加
            if (elapsedTime >= textSpeed) {
                messageText.text += splitMessage[messageNum][nowTextNum];

                nowTextNum++;
                elapsedTime = 0f;

                // メッセージを全部表示、または行数が最大数表示された
                if (nowTextNum >= splitMessage[messageNum].Length) {
                    isOneMessage = true;
                }
            }
            elapsedTime += Time.deltaTime;

            // メッセージ表示中にマウスの左ボタンを押したら一括表示
            if (Input.GetButtonDown("Jump")) {
                // ここまでに表示しているテキストに残りのメッセージを足す
                messageText.text += splitMessage[messageNum].Substring(nowTextNum);
                isOneMessage = true;
            }
            // 1回に表示するメッセージを表示した
        } else {

            elapsedTime += Time.deltaTime;

            // クリックアイコンを点滅する時間を超えた時、反転させる
            if (elapsedTime >= clickFlashTime) {
                clickIcon.enabled = !clickIcon.enabled;
                elapsedTime = 0f;
            }

            // マウスクリックされたら次の文字表示処理
            if (Input.GetButtonDown("Jump")) {
                nowTextNum = 0;
                messageNum++;
                messageText.text = "";
                clickIcon.enabled = false;
                elapsedTime = 0f;
                isOneMessage = false;

                // メッセージが全部表示されていたらゲームオブジェクト自体の削除
                if (messageNum >= splitMessage.Length) {
                    EndTalking();
                }
            }
        }
    }

    // LateUpdateメソッド Updateメソッドと同じように毎フレーム呼ばれるが、Updateより後に呼ばれる
    private void LateUpdate() {
        // 会話相手がいる場合はTalkIconの位置を会話相手の頭上に表示
        if (conversationPartner != null) {
            // Camera.main.GetComponent().WorldToScreenPointでメインカメラのCameraコンポーネントのWorldToScreenPointメソッドを使って引数で与えられたワールド位置をカメラのスクリーンの位置に変換
            // こうすることでTalkIconのサイズが変更せず会話相手が移動してもそのワールド位置をカメラのスクリーンの位置に変換し移動するようになる
            talkIcon.transform.Find("Panel").position = Camera.main.GetComponent<Camera>().WorldToScreenPoint(conversationPartner.transform.position + Vector3.up * 2f);
        }
    }

    // 会話相手を設定
    // 会話相手が範囲に入ってきた時、実行されるメソッド(ConversationScopeScriptで実行,引数にはVillagersが代入されてくる)
    public void SetConversationPartner(GameObject partnerObj) {
        talkIcon.SetActive(true);
        conversationPartner = partnerObj;
    }

    // 会話相手をリセット
    // 会話相手が設定されていなかったり、他の村人が会話相手として設定されている時に別の村人を引数で受け取っても会話相手はリセットしないようにした
    public void ResetConversationPartner(GameObject parterObj) {
        // 会話相手がいない場合は何もしない
        if(conversationPartner == null) {
            return;
        }
        // 会話相手と引数で受け取った相手が同じインスタンスIDを持つなら会話相手をなくす GameObjectにはそれぞれ固有の（ユニークな）IDが付けられている
        if(conversationPartner.GetInstanceID() == parterObj.GetInstanceID()) {
            talkIcon.SetActive(false);
            conversationPartner = null;
        }
    }
    // 会話相手を返す
    public GameObject GetConversationPartner() {
        return conversationPartner;
    }


    // 会話を開始する
    public void StartTalking() {
        var villagerScript = conversationPartner.GetComponent<VillagerScript>();
        villagerScript.SetState(VillagerScript.State.Talk, transform);
        this.allMessage = villagerScript.GetConversation().GetConversationMessage();
        // 分割文字列で一回に表示するメッセージを分割する
        splitMessage = Regex.Split(allMessage, @"\s*" + splitString + @"\s*", RegexOptions.IgnorePatternWhitespace);
        // 初期化処理
        nowTextNum = 0;
        messageNum = 0;
        messageText.text = "";
        talkUI.SetActive(true);
        talkIcon.SetActive(false);
        isOneMessage = false;
        isEndMessage = false;
        // 会話開始時の入力は一旦リセット
        Input.ResetInputAxes();
    }
    // 会話を終了する
    void EndTalking() {
        isEndMessage = true;
        talkUI.SetActive(false);
        // ユニティちゃんと村人両方の状態を変更する
        var villagerScript = conversationPartner.GetComponent<VillagerScript>();
        villagerScript.SetState(VillagerScript.State.Wait);
        GetComponent<UnityChanScript>().SetState(UnityChanScript.State.Normal);
        Input.ResetInputAxes();
    }
}