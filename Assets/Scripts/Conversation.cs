using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Conversation 単に会話内容をmessageフィールドで保持して置くだけのスクリプト

// CreateAssetMenuアトリビュートをConversationクラスに取り付け、エディターのメニュー項目からConversationクラスのデータを作成し、ファイルとしてフォルダに作成することを可能にする
// fileNameはデフォルトのファイルの名前、menuNameはUnityのエディターメニューのCreateの中で表示される項目名
[Serializable]
[CreateAssetMenu(fileName = "Conversation", menuName = "CreateConversation")]


// 会話内容を保存するためのクラス
// ScriptableObjectを継承したクラスはゲームオブジェクトに取り付ける事が出来ないので、ゲームオブジェクトに取り付けた他のスクリプトからConversationクラスをインスタンス化する
public class Conversation : ScriptableObject
{
    // 会話内容
    [SerializeField]
    [Multiline(100)]
    private string message = null;

    // 会話内容を返す
    public string GetConversationMessage() {
        return message;
    }
}
