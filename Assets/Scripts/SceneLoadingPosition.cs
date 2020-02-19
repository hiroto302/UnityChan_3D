using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadingPosition : MonoBehaviour {
    [SerializeField]
    // 作成したSceneMovementDataファイルを設置
    private SceneMovementData sceneMovementData = null;

    // SceneMovementDataのGetSceneTypeメソッドでシーンタイプを取得し、どのシーンからどのシーンへの遷移なのかを調べる
    void Start() {

        // シーン遷移の種類に応じて初期位置のゲームオブジェクトの位置と角度に設定
        if (sceneMovementData.GetSceneType() == SceneMovementData.SceneType.StartGame) {
            var initialPosition = GameObject.Find("InitialPosition").transform;
            transform.position = initialPosition.position;
            transform.rotation = initialPosition.rotation;
        } else if (sceneMovementData.GetSceneType() == SceneMovementData.SceneType.FirstVillage) {
            // var initialPosition = GameObject.Find("InitialPosition").transform;
            var initialPosition = GameObject.Find("InitialPositionSecondToFirst").transform;
            transform.position = initialPosition.position;
            transform.rotation = initialPosition.rotation;
        } else if (sceneMovementData.GetSceneType() == SceneMovementData.SceneType.FirstVillageToWorldMap) {
            var initialPosition = GameObject.Find("InitialPositionFirstToSecond").transform;
            transform.position = initialPosition.position;
            transform.rotation = initialPosition.rotation;
        }
    }
}