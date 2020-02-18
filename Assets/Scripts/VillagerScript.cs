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
        Walk,
        Talk
    }

    //会話機能の追加
    // 会話内容保持スクリプト
    [SerializeField]
    private Conversation conversation = null;
    // ユニティちゃんのTransform
    private Transform conversationPartnerTransform;
    // 村人がユニティちゃんの方向に回転するスピード
    [SerializeField]
    private float rotationSpeed = 2f;



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
        // // 見回り
        // if (state == State.Walk) {
        //     // エージェントの潜在的な速さを設定
        //     animator.SetFloat("Speed", navMeshAgent.desiredVelocity.magnitude);

        //     // 目的地に到着したかどうかの判定 navMeshAgent.remainingDistance で目的地とエージェントの距離
        //     if (navMeshAgent.remainingDistance < 0.1f) {
        //         SetState(State.Wait);
        //     }
        //     // 到着していたら一定時間待つ
        // } else if (state == State.Wait) {
        //     elapsedTime += Time.deltaTime;  //時間の計測

        //     // 待ち時間を越えたら次の目的地を設定
        //     if (elapsedTime > waitTime) {
        //         SetState(State.Walk);
        //     }
        // }

        //会話機能追加に伴い、State.Talk時の処理も追加
        // 見回り
        if (state == State.Walk) {
        // エージェントの潜在的な速さを設定
        animator.SetFloat("Speed", navMeshAgent.desiredVelocity.magnitude);

        // 目的地に到着したかどうかの判定
        if (navMeshAgent.remainingDistance < 0.1f) {
            SetState(State.Wait);
        }
        // 到着していたら一定時間待つ
        } else if (state == State.Wait) {
        elapsedTime += Time.deltaTime;

        // 待ち時間を越えたら次の目的地を設定
        if (elapsedTime > waitTime) {
            SetState(State.Walk);
        }
        } else if(state == State.Talk) {
        // 村人がユニティちゃんの方向をある程度向くまで回転させる
        // ユニティちゃんの位置から村人の位置を引いてユニティちゃんの方向を求めたものと村人の前方の角度をVector3.Angleで求め5度より大きい時は村人をユニティちゃんの方向に向かせる
        // ユニティちゃんの位置で、Yだけ村人のYの位置を設定しているのは、高さが違う時にYの位置を合わせて計算したい為
        if (Vector3.Angle(transform.forward, new Vector3(conversationPartnerTransform.position.x, transform.position.y, conversationPartnerTransform.position.z) - transform.position) > 5f) {
            // Quaternion.Lerpで徐々に現在の角度からユニティちゃんの方向の角度へと変更
            // Quaternion.LookRotationでユニティちゃんの方向のQuaternionを求める
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(conversationPartnerTransform.position.x, transform.position.y, conversationPartnerTransform.position.z) - transform.position), rotationSpeed * Time.deltaTime);
            animator.SetFloat("Speed", 1f);
        } else {
            animator.SetFloat("Speed", 0f);
        }
    }
    }

    // // 村人の状態変更
    // public void SetState(State state) {
    //     this.state = state;
    //     if (state == State.Wait) {
    //         elapsedTime = 0f;                               //待ち時間の初期化
    //         animator.SetFloat("Speed", 0f);
    //     } else if(state == State.Walk) {
    //         SetNextPosition();
    //         navMeshAgent.SetDestination(GetDestination());  //navMeshAgent.SetDestination 引数に与えた位置を目的地に設定し、移動を開始
    //     }
    // }

    // 村人の状態変更 State.Talk 追加
    public void SetState(State state, Transform conversationPartnerTransform = null) {
    this.state = state;
    if (state == State.Wait) {
        elapsedTime = 0f;
        animator.SetFloat("Speed", 0f);
    } else if(state == State.Walk) {
        SetNextPosition();
        navMeshAgent.SetDestination(GetDestination());
        navMeshAgent.isStopped = false;
    } else if(state == State.Talk) {
        navMeshAgent.isStopped = true;
        animator.SetFloat("Speed", 0f);
        this.conversationPartnerTransform = conversationPartnerTransform;
    }
}

    // Conversionスクリプトを返す
    public Conversation GetConversation() {
        return conversation;
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