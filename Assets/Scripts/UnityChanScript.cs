using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class UnityChanScript : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;
    // キャラクターの速度
    private Vector3 velocity;
    // キャラクターの歩くスピード
    [SerializeField]
    private float walkSpeed = 2f;
    // キャラクターの走るスピード
    [SerializeField]
    private float runSpeed = 4f;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // キャラクターが接地しているかどうか
        if(characterController.isGrounded) {
            velocity = Vector3.zero; // Vector3.zeroを入れて速度を0にする

            var input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));  //横軸の入力,縦軸の入力

            // input.magnitudeで入力（ベクトル）の長さを取得
            if(input.magnitude > 0.1f) {
                //transform.LookAt 引数で指定したベクトルの方向を向かせるメソッド
                // input.normalizedで入力の単位ベクトル（長さが1のベクトルで方向を得る）を足すことで現在の位置に入力した方向を足して、その方向を向かせるようにします。
                transform.LookAt(transform.position + input.normalized);

                animator.SetFloat("Speed", input.magnitude);
                if (input.magnitude > 0.5f) {
                    velocity += transform.forward * runSpeed;
                } else {
                    velocity += transform.forward * walkSpeed;
                }
            } else {
                animator.SetFloat("Speed", 0f);
            }
        }

        velocity.y += Physics.gravity.y * Time.deltaTime;  //重力 UnityメニューのEdit→Project Settings→Physicsで設定されている値
        characterController.Move(velocity * Time.deltaTime);  //characterControllerのMoveメソッドの引数に速度を渡してキャラクターを移動
    }
}