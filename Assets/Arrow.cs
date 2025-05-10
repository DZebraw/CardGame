using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Vector2 StartPoint;
    private Vector2 EndingPoint;
    private RectTransform arrow;

    private float ArrowLength;
    private float ArrowTheta;
    private Vector2 ArrowPosition;
    
    void Start()
    {
        arrow = transform.GetComponent<RectTransform>();
    }
    
    void Update()
    {
        //因为以Canvas为基准，会发现和鼠标和箭头有偏差，所以减去我们CanvsaPOSX,POSY，使得没有偏差
        EndingPoint = Input.mousePosition - new Vector3(960.0f,540.0f,0.0f);
        //计算变量
        ArrowPosition = new Vector2((EndingPoint.x + StartPoint.x) / 2, (EndingPoint.y + StartPoint.y) / 2);
        ArrowLength = Mathf.Sqrt((EndingPoint.x - StartPoint.x) * (EndingPoint.x - StartPoint.x) +
                                 (EndingPoint.y - StartPoint.y));
        ArrowTheta = Mathf.Atan2(EndingPoint.y - StartPoint.y, EndingPoint.x - StartPoint.x);

        //赋值
        arrow.localPosition = ArrowPosition;
        arrow.sizeDelta = new Vector2(ArrowLength, arrow.sizeDelta.y);
        arrow.localEulerAngles = new Vector3(0.0f, 0.0f, ArrowTheta * 180 /  Mathf.PI);
    }

    public void SetStartPoint(Vector2 _startPoint)
    {
        StartPoint = _startPoint - new Vector2(960.0f,540.0f);
    }
}
