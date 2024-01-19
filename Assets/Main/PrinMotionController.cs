using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrinMotionController : MonoBehaviour
{
    // スクロールとタップの判定
    private float FingerPosY0; // タップし指が画面に触れた瞬間の指のY座標
    private float FingerPosY1; // タップし指が画面から離れた瞬間のY座標
    private float PosDiff = 0.5f; // Y座標の差の閾値

    // プリンちゃんの体全体のアニメーション
    private Animator an;
    private Rigidbody rb;
    public float JumpPower = 42; // ジャンプの強さ
    private int countTap = 0; // タップした回数に応じてアニメーションの遷移を変更

    // プリンちゃんの顔のアニメーション
    private GameObject princhan;
    private SkinnedMeshRenderer smr;
    private float countTime = 0.0f; // 時間を計測
    public float blinkTriggerTime = 4.0f; // まばたきの発動タイミング
    private float notTouchTime = 0; // 放置されている時間を計測
    private bool notAngry = true; // 怒っているかいないかの指標
    private bool wasHappy = false; // trueの時のみ怒りに遷移可能

    // フキダシ（2種）のアニメーション
    private GameObject happyFukidashi;
    private Animator hf_an;
    private GameObject angryFukidashi;
    private Animator af_an;

    // お花の背景エフェクトのアニメーション
    private GameObject flower;
    private Animator fl_an;

    void Start()
    {
        // プリンちゃんの体全体のアニメーション
        an = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // プリンちゃんの顔のアニメーション
        princhan = GameObject.Find("プリンちゃん");
        smr = princhan.GetComponent<SkinnedMeshRenderer>();

        // フキダシ（2種）のアニメーション
        happyFukidashi = GameObject.Find("HappyFukidashi");
        hf_an = happyFukidashi.GetComponent<Animator>();
        angryFukidashi = GameObject.Find("AngryFukidashi");
        af_an = angryFukidashi.GetComponent<Animator>();

        // お花の背景エフェクトのアニメーション
        flower = GameObject.Find("FlowerParticles");
        fl_an = flower.GetComponent<Animator>();
    }

    void Update()
    {
        // 怒り発動タイミングまで
        if (notTouchTime < 4.3)
        {
            // まばたき発動のための時間を計測
            CheckCountTime();
        }

        if(wasHappy == true)
        {
            // 放置されている時間を計測
            CheckNotTouchTime();
        }

        // スマホ画面をタップした瞬間
        if (Input.GetMouseButtonDown(0))
        {
            // 指のx座標を取得
            FingerPosY0 = Input.mousePosition.y;
        }
        // スマホ画面から指を離した瞬間
        if (Input.GetMouseButtonUp(0))
        {
            // 指のx座標を取得
            FingerPosY1 = Input.mousePosition.y;
            Debug.Log(Mathf.Abs(FingerPosY0 - FingerPosY1));

            //////////////////////// タップ判定 ////////////////////////
           　
            // スクロールではなくタップだと判定された時、フキダシのフェードイン、フェードアウトを開始
            if (Mathf.Abs(FingerPosY0 - FingerPosY1) < PosDiff)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    //////////////////////// レイキャスト判定 ////////////////////////
                    
                    if (hit.collider.gameObject.name == "MainPrin")
                    {
                        // 怒っている時は怒りを解除
                        if (notAngry == false)
                        {
                            // 待機モーション中かつ、遷移中でないとき
                            if (an.GetCurrentAnimatorStateInfo(0).IsName("アーマチュア|idle") && !an.IsInTransition(0))
                            {
                                // 怒りの目からにっこり目にして通常目に戻す
                                StartCoroutine("HappyEye");
                                // happy（喜びモーション）へ遷移
                                an.SetTrigger("HappyTrigger");
                                rb.AddForce(Vector3.up * JumpPower, ForceMode.Impulse);
                                // 口が笑う処理開始
                                StartCoroutine("LaughMouth");
                            }
                        }

                        // 怒っていない時
                        else
                        {
                            // 待機モーション中かつ、遷移中でないとき
                            if (an.GetCurrentAnimatorStateInfo(0).IsName("アーマチュア|idle") && !an.IsInTransition(0))
                            {
                                // 放置時間をリセット
                                notTouchTime = 0.0f;

                                if (countTap % 5 == 0 && countTap != 0)
                                {
                                    // happy（喜びモーション）へ遷移
                                    an.SetTrigger("HappyTrigger");
                                    rb.AddForce(Vector3.up * JumpPower, ForceMode.Impulse);
                                    // 口が笑う処理開始
                                    StartCoroutine("LaughMouth");
                                    // ハッピーフキダシアニメーションへ遷移
                                    hf_an.SetTrigger("FukidashiTrigger");
                                    // お花の背景エフェクトのアニメーションへ遷移
                                    fl_an.SetTrigger("FlowerTrigger");
                                    wasHappy = true;
                                    countTap += 1;
                                }
                                else
                                {
                                    // jiggle（揺れモーション）へ遷移
                                    an.SetTrigger("JiggleTrigger");
                                    countTap += 1;

                                }
                            }
                        }
                    }
                }
                
            }
        }
    }

    //////////////////////// まばたき ////////////////////////

    // まばたき発動のための時間を計測する関数
    void CheckCountTime()
    {
        // 時間を計測
        countTime += Time.deltaTime;
        // 時間がまばたきの発動タイミングを超えたら
        if (countTime > blinkTriggerTime)
        {
            // 計測時間をリセット
            countTime = 0.0f;
            // まばたきの発動タイミングの変数に
            // 1.5fから6.0fの間でランダムな数値を取得
            blinkTriggerTime = Random.Range(1.5f, 6.0f);
            // 目を閉じる処理開始
            StartCoroutine("CloseEye");
        }
    }

    // 目を閉じる
    IEnumerator CloseEye()
    {
        // 徐々に目を閉じる
        smr.SetBlendShapeWeight(0, 80.0f);
        yield return new WaitForSeconds(0.008f);
        smr.SetBlendShapeWeight(0, 100.0f);
        yield return new WaitForSeconds(0.10f);
        // 目を開く処理開始
        StartCoroutine("OpenEyefromCloseEye");
    }

    // 目を戻す
    IEnumerator OpenEyefromCloseEye()
    {
        // 徐々に目を戻す
        smr.SetBlendShapeWeight(0, 60.0f);
        yield return new WaitForSeconds(0.015f);
        smr.SetBlendShapeWeight(0, 0.0f);
    }

    //////////////////////// 笑う ////////////////////////

    // 口が笑う
    IEnumerator LaughMouth()
    {
        // 徐々に口が笑う
        smr.SetBlendShapeWeight(1, 20.0f);
        yield return new WaitForSeconds(0.008f);
        smr.SetBlendShapeWeight(1, 0.0f);
        yield return new WaitForSeconds(4.0f);
        // 口を戻す処理開始
        StartCoroutine("Mouth");
    }

    // 口を戻す
    IEnumerator Mouth()
    {
        // 徐々に口を戻す
        smr.SetBlendShapeWeight(1, 25.0f);
        yield return new WaitForSeconds(0.02f);
        smr.SetBlendShapeWeight(1, 50.0f);
        yield return new WaitForSeconds(0.02f);
        smr.SetBlendShapeWeight(1, 75.0f);
        yield return new WaitForSeconds(0.02f);
        smr.SetBlendShapeWeight(1, 100.0f);
    }

    //////////////////////// 怒る ////////////////////////

    // 放置されている時間を計測する関数
    void CheckNotTouchTime()
    {
        // 時間を計測
        notTouchTime += Time.deltaTime;
        // Debug.Log(notTouchTime);
        // 時間が怒り発動タイミングを超えたら
        if (notTouchTime > 4.3 && notAngry == true)
        {
            notAngry = false;
            // 怒りの目にする処理開始
            StartCoroutine("AngryEye");
        }
    }

    // 怒りの目にする
    IEnumerator AngryEye()
    {
        //　怒りフキダシをフェードイン
        af_an.SetBool("Start",true);
        // 徐々に怒りの目にする
        smr.SetBlendShapeWeight(2, 15.0f);
        yield return new WaitForSeconds(0.008f);
        smr.SetBlendShapeWeight(2, 30.0f);
        yield return new WaitForSeconds(0.008f);
        smr.SetBlendShapeWeight(2, 45.0f);
        yield return new WaitForSeconds(0.008f);
        smr.SetBlendShapeWeight(2, 70.0f);
        af_an.SetBool("Start", false);
    }

    // 怒りの目からにっこり目にして通常目に戻す
    IEnumerator HappyEye()
    {
        //　怒りフキダシをフェードアウト
        af_an.SetBool("End",true);
        // 怒りの目を戻す
        smr.SetBlendShapeWeight(2, 0.0f);
        yield return new WaitForSeconds(0.0f);
        // にっこり目にして2秒待つ
        smr.SetBlendShapeWeight(3, 100.0f);
        yield return new WaitForSeconds(2.0f);
        // 徐々に通常目に戻す
        smr.SetBlendShapeWeight(3, 75.0f);
        yield return new WaitForSeconds(0.008f);
        smr.SetBlendShapeWeight(3, 50.0f);
        yield return new WaitForSeconds(0.008f);
        smr.SetBlendShapeWeight(3, 25.0f);
        yield return new WaitForSeconds(0.008f);
        smr.SetBlendShapeWeight(3, 0.0f);
        // 計測時間をリセット
        notTouchTime = 0.0f;
        notAngry = true;
        af_an.SetBool("End", false);
    }
}