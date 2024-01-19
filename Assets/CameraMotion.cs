using UnityEngine;
 
public class CameraMotion : MonoBehaviour
{
    private float FingerPosY0; // タップし指が画面に触れた瞬間の指のy座標
    private float FingerPosY1; // 現在の指のy座標
    private float PosDiff; // タップの初めと終わりのY座標の差分
    private float NextPosY; // カメラを移動させる先の座標

    void Update()
    {
        CameraMove();
    }

    void CameraMove()
    {
        // タップし指が画面に触れた瞬間の指のy座標を取得
        if (Input.GetMouseButtonDown(0))
        {
            FingerPosY0 = Input.mousePosition.y;
        }

        // 現在の指のy座標を取得
        if (Input.GetMouseButton(0))
        {
            FingerPosY1 = Input.mousePosition.y;
            PosDiff = FingerPosY1 - FingerPosY0;
            NextPosY = Camera.main.transform.position.y - PosDiff * 0.005f;

            // カメラの移動制限（iPhone15を想定）
            if (NextPosY > -2.7 && NextPosY < 8.55)
            {
                Camera.main.transform.position = new Vector3(0, NextPosY, -16.5f);
            }
            FingerPosY0 = Input.mousePosition.y;
        }
    }
}