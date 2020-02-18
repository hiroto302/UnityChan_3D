using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    // 会話範囲に入った時,実行するスクリプト
public class ConversationScopeScript : MonoBehaviour
{

    void OnTriggerStay(Collider col) {
        if (col.tag == "Player"
            && col.GetComponent<UnityChanScript>().GetState() != UnityChanScript.State.Talk
            ) {
            // ユニティちゃんが近づいたら会話相手として自分のゲームオブジェクトを渡す
            // transform.parent.gameObjectでこのスクリプトをアタッチしている子オブジェクトの親オブジェクト(ここではVillagers)を指定して代入
            col.GetComponent<UnityChanTalkScript>().SetConversationPartner(transform.parent.gameObject);
        }
    }

    void OnTriggerExit(Collider col) {
        if (col.tag == "Player"
            && col.GetComponent<UnityChanScript>().GetState() != UnityChanScript.State.Talk
            ) {
            // ユニティちゃんが範囲外に遠ざかったら会話相手から外す
            col.GetComponent<UnityChanTalkScript>().ResetConversationPartner(transform.parent.gameObject);
        }
    }
}