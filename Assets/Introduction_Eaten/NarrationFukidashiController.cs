using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrationFukidashiController : MonoBehaviour
{
    // スクロールとタップの判定
    private float FingerPosY0; // タップし指が画面に触れた瞬間の指のx座標
    private float FingerPosY1; // タップし指が画面から離れた瞬間のx座標
    private float PosDiff = 0.5f; // Y座標の差の閾値

    // イントロフキダシ
    public GameObject[] introFukidashi; // オブジェクト
    private Vector3[] introPos; // 初期位置
    private float[] introAmplitude; // 振幅
    private Animator[] introfu_an; // アニメーター
    private int countTapforIntro; // タップした回数

    // メインフキダシ
    public GameObject[] mainFukidashi; // オブジェクト
    private Vector3[] mainPos; // 初期位置
    private float[] mainAmplitude; // 振幅
    private Animator[] mainfu_an; // アニメーター
    private int countTapforMain; // タップした回数

    // バケツとプリン
    public GameObject[] BaketsuPrin; // オブジェクト
    private Animator[] bp_an; // アニメーター

    void Start()
    {
        // 初期化
        introPos = new Vector3[introFukidashi.Length];
        introAmplitude = new float[introFukidashi.Length];
        introfu_an = new Animator[introFukidashi.Length];

        mainPos = new Vector3[mainFukidashi.Length];
        mainAmplitude = new float[mainFukidashi.Length];
        mainfu_an = new Animator[mainFukidashi.Length];

        bp_an = new Animator[BaketsuPrin.Length];

        countTapforIntro = 0;
        countTapforMain = 0;

        // イントロフキダシ
        for (int i = 0; i < introFukidashi.Length; i++)
        {
            // 初期位置取得
            introPos[i] = introFukidashi[i].transform.position;
            // アニメーションの振幅をランダムに設定
            introAmplitude[i] = Random.Range(0.15f, 0.2f);
            // 偶数番目のフキダシの場合は逆方向に揺らす
            if (i % 2 == 0)
            {
                introAmplitude[i] *= -1;
            }
            // アニメーター取得
            introfu_an[i] = introFukidashi[i].GetComponent<Animator>();
        }

        // メインフキダシ
        for (int i = 0; i < mainFukidashi.Length; i++)
        {
            // 初期位置取得
            mainPos[i] = mainFukidashi[i].transform.position;
            // アニメーションの振幅をランダムに設定
            mainAmplitude[i] = Random.Range(0.15f, 0.2f);
            // 偶数番目のフキダシの場合は逆方向に揺らす
            if (i % 2 == 0)
            {
                mainAmplitude[i] *= -1;
            }
            // アニメーター取得
            mainfu_an[i] = mainFukidashi[i].GetComponent<Animator>();
        }

        // バケツとプリン
        for (int i = 0; i < BaketsuPrin.Length; i++)
        {
            // アニメーター取得
            bp_an[i] = BaketsuPrin[i].GetComponent<Animator>();
        }
    }

    void Update()
    {
        // フキダシが漂うアニメーション
        for (int i = 0; i < introFukidashi.Length; i++)
        {
            introFloating(i);
        }
        for (int i = 0; i < mainFukidashi.Length; i++)
        {
            mainFloating(i);
        }

        // スマホ画面をタップした瞬間
        if (Input.GetMouseButtonDown(0))
        {
            // 指のY座標を取得
            FingerPosY0 = Input.mousePosition.y;
        }
        // スマホ画面から指を離した瞬間
        if (Input.GetMouseButtonUp(0))
        {
            // 指のY座標を取得
            FingerPosY1 = Input.mousePosition.y;
            Debug.Log(Mathf.Abs(FingerPosY0 - FingerPosY1));

            // スクロールではなくタップだと判定された時、フキダシのフェードイン、フェードアウトを開始
            if (Mathf.Abs(FingerPosY0 - FingerPosY1) < PosDiff)
            {
                countTapforIntro++;
                countTapforMain++;

                if (countTapforIntro == 6)
                {
                    // 繰り返しのため初期化
                    countTapforIntro = 1;

                    introfu_an[2].SetBool("3End", false);

                    introfu_an[0].SetBool("AllEnd", true);
                    introfu_an[1].SetBool("AllEnd", true);
                    introfu_an[2].SetBool("AllEnd", true);

                }
                if (countTapforIntro == 1) // タップ6回目も
                {
                    // バケツとプリンアニメーションへ遷移
                    bp_an[0].SetBool("BornPrin", true);
                    bp_an[1].SetBool("BornPrin", true);

                    if (countTapforMain == 6)
                    {
                        // メインフキダシアニメーション2へ遷移
                        mainfu_an[0].SetBool("Start", false);
                        mainfu_an[1].SetBool("1End", true);
                    }
                }
                else if (countTapforIntro == 2) // タップ7回目も
                {
                    // イントロフキダシアニメーション1へ遷移
                    introfu_an[0].SetBool("Start", true);

                    if (countTapforMain == 7)
                    {
                        // メインフキダシアニメーション3へ遷移
                        mainfu_an[1].SetBool("1End", false);
                        mainfu_an[2].SetBool("2End", true);
                    }

                }
                else if (countTapforIntro == 3) // タップ8回目も
                {
                    // イントロフキダシアニメーション2へ遷移
                    introfu_an[0].SetBool("Start", false);

                    introfu_an[0].SetBool("1End", true);
                    introfu_an[1].SetBool("1End", true);

                    if (countTapforMain == 8)
                    {
                        // メインフキダシアニメーション終了
                        mainfu_an[2].SetBool("2End", false);
                    }

                }
                else if (countTapforIntro == 4) // タップ9回目も
                {
                    // イントロフキダシアニメーション3へ遷移
                    introfu_an[0].SetBool("1End", false);
                    introfu_an[1].SetBool("1End", false);

                    introfu_an[1].SetBool("2End", true);
                    introfu_an[2].SetBool("2End", true);
                }
                else if (countTapforIntro == 5) // タップ10回目も
                {
                    // イントロフキダシアニメーション終了
                    bp_an[1].SetBool("BornPrin", false);
                    bp_an[0].SetBool("BornPrin", false);
                    introfu_an[1].SetBool("2End", false);
                    introfu_an[2].SetBool("2End", false);
                    introfu_an[0].SetBool("AllEnd", false);
                    introfu_an[1].SetBool("AllEnd", false);
                    introfu_an[2].SetBool("AllEnd", false);

                    introfu_an[2].SetBool("3End", true);

                    if (countTapforMain == 5)
                    {
                        // メインフキダシアニメーション1へ遷移
                        mainfu_an[0].SetBool("Start", true);
                    }
                }
            }
        }
    }

    // 漂うアニメーション

    //イントロフキダシ
    void introFloating(int i)
    {
        // 移動方向
        Vector3 moveDirection = new Vector3(
            Mathf.Sin(Time.time * 0.5f + i * 0.5f) * introAmplitude[i],
            Mathf.Sin(Time.time + i * 0.5f) * introAmplitude[i],
            0
        );
        // フキダシが向いている方向の平面上で動かす
        introFukidashi[i].transform.position = introPos[i] + transform.TransformDirection(moveDirection);
    }

    // メインフキダシ
    void mainFloating(int i)
    {
        // 移動方向
        Vector3 moveDirection = new Vector3(
            Mathf.Sin(Time.time * 0.5f + i * 0.5f) * mainAmplitude[i],
            Mathf.Sin(Time.time + i * 0.5f) * mainAmplitude[i],
            0
        );
        // フキダシが向いている方向の平面上で動かす
        mainFukidashi[i].transform.position = mainPos[i] + transform.TransformDirection(moveDirection);
    }
}
