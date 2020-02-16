using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 設定した場所を周回するためのスクリプト
public class VillagerScript : MonoBehaviour
{
    // enumを利用した、状態の場合分け
    public enum State
    {
        Wait,
        Walk
    }

    // 目的地
    private Vector3 destination;
    // 巡回する位置の親
    [SerializeField]
    private Transform patrolPointsParent = null;
    // 巡回する位置
    private Transform[] patrolPositions;
    // 次に巡回する位置
    private int nowPatrolPosition = 0;
    // エージェント
    private NavMeshAgent navMeshAgent;
    // アニメーター
    private Animator animator;
    // 村人の状態
    private State state;
    // 待機した時間
    private float elapsedTime;
    // 待機する時間
    [SerializeField]
    private float waitTime = 5f;

    void OnEnable() {
    }

    private void Start() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        // 巡回地点を設定
        patrolPositions = new Transform[patrolPointsParent.transform.childCount];  //インスペクター上でセットした親オブジェクトをの子をカウント
        for (int i = 0; i < patrolPointsParent.transform.childCount; i++) {
            patrolPositions[i] = patrolPointsParent.transform.GetChild(i);         //配列に格納
        }
        SetState(State.Wait);
    }

    void Update() {
        // 見回り
        if (state == State.Walk) {
            // エージェントの潜在的な速さを設定
            animator.SetFloat("Speed", navMeshAgent.desiredVelocity.magnitude);

            // 目的地に到着したかどうかの判定 navMeshAgent.remainingDistance で目的地とエージェントの距離
            if (navMeshAgent.remainingDistance < 0.1f) {
                SetState(State.Wait);
            }
            // 到着していたら一定時間待つ
        } else if (state == State.Wait) {
            elapsedTime += Time.deltaTime;  //時間の計測

            // 待ち時間を越えたら次の目的地を設定
            if (elapsedTime > waitTime) {
                SetState(State.Walk);
            }
        }
    }

    // 村人の状態変更
    public void SetState(State state) {
        this.state = state;
        if (state == State.Wait) {
            elapsedTime = 0f;                               //待ち時間の初期化
            animator.SetFloat("Speed", 0f);
        } else if(state == State.Walk) {
            SetNextPosition();
            navMeshAgent.SetDestination(GetDestination());  //navMeshAgent.SetDestination 引数に与えた位置を目的地に設定し、移動を開始
        }
    }

    // 巡回地点を順に周る
    // SetNextPositionメソッド 次の巡回ポイントを目的地に設定するメソッド
    public void SetNextPosition() {
        SetDestination(patrolPositions[nowPatrolPosition].position);
        nowPatrolPosition++;                                            //次の目的をセットするために加算する
        if (nowPatrolPosition >= patrolPositions.Length) {              //1周したらリセット
            nowPatrolPosition = 0;
        }
    }
    // 目的地を設定する 引数で受け取った位置を目的地にするメソッド
    public void SetDestination(Vector3 position) {
        destination = position;
    }

    // 現在の目的地
    public Vector3 GetDestination() {
        return destination;
    }
}