using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "SceneMovementData", menuName = "CreateSceneMovementData")]
public class SceneMovementData : ScriptableObject
{

    // どのシーンからどのシーンへの遷移をしているかを表す列挙型
    // sceneTypeはシーン遷移時に設定しておき、シーンを遷移した時のユニティちゃんの初期位置の設定に使用
        public enum SceneType {
        StartGame,
        FirstVillage,
        FirstVillageToWorldMap
    }
    [SerializeField]
    private SceneType sceneType;


    // このOnEnable() 関数はオブジェクトが有効/アクティブになったときに呼び出される
    // Startメソッドも似たようなタイミングで呼ばれるが、OnEnableメソッドの方がタイミングとして早く呼ばれる
    // sceneTypeの初期化
    public void OnEnable() {
        sceneType = SceneType.StartGame;
    }

    // シーンタイプの設定
    public void SetSceneType(SceneType scene) {
        sceneType = scene;
    }

    // シーンタイプを返す
    public SceneType GetSceneType() {
        return sceneType;
    }

    // このスクリプトを作成し、Assets/Dataフォルダ内でFolder SceneMovementDataを作成
    // SceneMovementDataフォルダ内で右クリックからCreate→CreateSceneMovementDataを選択
    // シーン遷移時にこのファイルを読み書きしてユニティちゃんの位置や角度を決定する
    // ここではシーン遷移時の場所を、InitialPositionとして空オブジェクトを作成し、そのインスペクタでTransformのPositionとRotationをそのシーンに移動した時のユニティちゃんの位置と角度をそれらのゲームオブジェクトのTransformで設定した
}