using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mathd;
using System;

public class DrawBoundary : MonoBehaviour
{
    public CreateLine _createLine;
    List<GameObject> _arrowGam=new List<GameObject>();
    List<GameObject> _arrowGam2 = new List<GameObject>();
    List<GameObject> _arrowGam3 = new List<GameObject>();
    public List<Vector3d> _D_value=new List<Vector3d>();//插值得到的四个顶点
    public List<Vector3d> _curvePoints=new List<Vector3d>();
    public void draw_Boundary(List<Vector3d> points)
    {
        CreateLine.line.positionCount = points.Count; //设置点的个数      
        CreateLine.line.loop = false; //是否首尾相接
       // Debug.Log("lines count:"+points.Count);
        for (int i = 0; i < points.Count; i++)
        {
            CreateLine.line.SetPosition(i, points[i]);
        }
    }
    public void draw_Boundary(Vector3d p0,Vector3d p1)
    {
        CreateLine.line.positionCount = 2; //设置点的个数      
        CreateLine.line.loop = false; //是否首尾相接
        CreateLine.line.SetPosition(0, p0);
        CreateLine.line.SetPosition(1, p1);
    }
    //Catmullrom样条曲线,就是进行的插值计算
    public void DrawCatmullRom(List<Vector3> points, LineRenderer line)
    {
        points.Add(points[0]);
        Vector3 _p0;
        Vector3 _m0;
        Vector3 _p1;
        Vector3 _m1;
        int segmentFrameCount = 10;
        line.positionCount = segmentFrameCount * (points.Count-1);
        line.loop = true;
        for (int i = 0; i < points.Count-1; i++)
        {
            _p0 = points[i];            
            _p1 = points[i + 1];
            _m0 = i > 0 ? 0.5f * (points[i + 1] - points[i - 1]) : points[i + 1] - points[i];
            _m1 = i < points.Count - 2 ? 0.5f * (points[i + 2] - points[i]) : points[i + 1] - points[i];
            Vector3d curvePos;
            float _t;
            float uStep = 1.0f / segmentFrameCount;

            if (i == points.Count - 2)
            {
                uStep = 1.0f / (segmentFrameCount - 1.0f);
            }
            for (int j = 0; j < segmentFrameCount; j++)
            {
                _t = j * uStep;
                curvePos = PositionOnCurve(_p0, _m0, _p1, _m1, _t);
                //_curvePoints.Add(curvePos);
                line.SetPosition(i * segmentFrameCount + j, curvePos);
            }
        }
    }
    Vector3 PositionOnCurve(Vector3 p0, Vector3 m0, Vector3 p1, Vector3 m1, float t)
    {
        Vector3 position = (2.0f * t * t * t - 3.0f * t * t + 1.0f) * p0
                        + (t * t * t - 2.0f * t * t + t) * m0
                        + (-2.0f * t * t * t + 3.0f * t * t) * p1
                        + (t * t * t - t * t) * m1;
        return position;
    }
    /// <summary>
    /// 双向箭头的绘制
    /// </summary>
    /// <param name="lines"></param>
    /// <returns></returns>
    public void DrawArrow(Vector3 p0,Vector3 p1)
    {
        if(_arrowGam.Count>0)
        {
            for(int i=0;i< _arrowGam.Count;i++)
            {
                Destroy(_arrowGam[i]);
            }
        }
        GameObject HeadArrowGam = new GameObject("HeadArrow");
        GameObject BodyArrowGam = new GameObject("BodyArrow");
        GameObject HeadArrowGamAnother = new GameObject("HeadArrowAnother");
        LineRenderer HeadLineRenderer = HeadArrowGam.AddComponent<LineRenderer>();
        LineRenderer BodyLineRenderer= BodyArrowGam.AddComponent<LineRenderer>();
        LineRenderer HeadLineRendererAnthor = HeadArrowGamAnother.AddComponent<LineRenderer>();

        BodyLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        BodyLineRenderer.startColor = Color.red;//设置颜色
        BodyLineRenderer.endColor = Color.red;
        BodyLineRenderer.startWidth = 0.001f;//设置宽度
        BodyLineRenderer.endWidth = 0.001f;
        BodyLineRenderer.SetPosition(0, p0);
        BodyLineRenderer.SetPosition(1, p1);
        Vector3d direction = p1 - p0;
        Vector3d directionAnthor = p0 - p1;
        direction.Normalize();
        directionAnthor.Normalize();

        HeadLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        HeadLineRenderer.startColor = Color.red;//设置颜色
        HeadLineRenderer.endColor = Color.red;
        HeadLineRenderer.startWidth = 0.01f;//设置宽度
        HeadLineRenderer.endWidth = 0;
        Vector3d HeadStart = p1;
        Vector3d HeadEnd = HeadStart + direction * 0.02;
        HeadLineRenderer.SetPosition(0,HeadStart);
        HeadLineRenderer.SetPosition(1,HeadEnd);

        HeadLineRendererAnthor.material = new Material(Shader.Find("Sprites/Default"));
        HeadLineRendererAnthor.startColor = Color.red;//设置颜色
        HeadLineRendererAnthor.endColor = Color.red;
        HeadLineRendererAnthor.startWidth = 0.01f;//设置宽度
        HeadLineRendererAnthor.endWidth = 0;
        Vector3d HeadStartAnthor = p0;
        Vector3d HeadEndAnthor = HeadStartAnthor + directionAnthor * 0.01;
        HeadLineRendererAnthor.SetPosition(0, HeadStartAnthor);
        HeadLineRendererAnthor.SetPosition(1, HeadEndAnthor);

        _arrowGam.Add(HeadArrowGam);
        _arrowGam.Add(BodyArrowGam);
        _arrowGam.Add(HeadArrowGamAnother);
    }
    public void DrawArrow2(Vector3 p0, Vector3 p1)
    {
        if (_arrowGam2.Count > 0)
        {
            for (int i = 0; i < _arrowGam2.Count; i++)
            {
                Destroy(_arrowGam2[i]);
            }
        }
        if (_arrowGam.Count > 0)
        {
            for (int i = 0; i < _arrowGam.Count; i++)
            {
                Destroy(_arrowGam[i]);
            }
        }
        GameObject HeadArrowGam2 = new GameObject("HeadArrow2");
        GameObject BodyArrowGam2 = new GameObject("BodyArrow2");
        GameObject HeadArrowGamAnother2 = new GameObject("HeadArrowAnother2");
        LineRenderer HeadLineRenderer2 = HeadArrowGam2.AddComponent<LineRenderer>();
        LineRenderer BodyLineRenderer2 = BodyArrowGam2.AddComponent<LineRenderer>();
        LineRenderer HeadLineRendererAnthor2 = HeadArrowGamAnother2.AddComponent<LineRenderer>();

        BodyLineRenderer2.material = new Material(Shader.Find("Sprites/Default"));
        BodyLineRenderer2.startColor = Color.black;//设置颜色
        BodyLineRenderer2.endColor = Color.black;
        BodyLineRenderer2.startWidth = 0.001f;//设置宽度
        BodyLineRenderer2.endWidth = 0.001f;
        BodyLineRenderer2.SetPosition(0, p0);
        BodyLineRenderer2.SetPosition(1, p1);
        Vector3d direction = p1 - p0;
        Vector3d directionAnthor = p0 - p1;
        direction.Normalize();
        directionAnthor.Normalize();

        HeadLineRenderer2.material = new Material(Shader.Find("Sprites/Default"));
        HeadLineRenderer2.startColor = Color.black;//设置颜色
        HeadLineRenderer2.endColor = Color.black;
        HeadLineRenderer2.startWidth = 0.005f;//设置宽度
        HeadLineRenderer2.endWidth = 0;
        Vector3d HeadStart = p1;
        Vector3d HeadEnd = HeadStart + direction * 0.02;
        HeadLineRenderer2.SetPosition(0, HeadStart);
        HeadLineRenderer2.SetPosition(1, HeadEnd);

        HeadLineRendererAnthor2.material = new Material(Shader.Find("Sprites/Default"));
        HeadLineRendererAnthor2.startColor = Color.black;//设置颜色
        HeadLineRendererAnthor2.endColor = Color.black;
        HeadLineRendererAnthor2.startWidth = 0.005f;//设置宽度
        HeadLineRendererAnthor2.endWidth = 0;
        Vector3d HeadStartAnthor = p0;
        Vector3d HeadEndAnthor = HeadStartAnthor + directionAnthor * 0.01;
        HeadLineRendererAnthor2.SetPosition(0, HeadStartAnthor);
        HeadLineRendererAnthor2.SetPosition(1, HeadEndAnthor);

        _arrowGam2.Add(HeadArrowGam2);
        _arrowGam2.Add(BodyArrowGam2);
        _arrowGam2.Add(HeadArrowGamAnother2);
    }
    public void DrawArrow3(Vector3 p0, Vector3 p1)
    {
        if (_arrowGam3.Count > 0)
        {
            for (int i = 0; i < _arrowGam3.Count; i++)
            {
                Destroy(_arrowGam3[i]);
            }
        }
        if (_arrowGam.Count > 0)
        {
            for (int i = 0; i < _arrowGam.Count; i++)
            {
                Destroy(_arrowGam[i]);
            }
        }
        GameObject HeadArrowGam3 = new GameObject("HeadArrow3");
        GameObject BodyArrowGam3 = new GameObject("BodyArrow3");
        GameObject HeadArrowGamAnother3 = new GameObject("HeadArrowAnother3");
        LineRenderer HeadLineRenderer3 = HeadArrowGam3.AddComponent<LineRenderer>();
        LineRenderer BodyLineRenderer3 = BodyArrowGam3.AddComponent<LineRenderer>();
        LineRenderer HeadLineRendererAnthor3 = HeadArrowGamAnother3.AddComponent<LineRenderer>();

        BodyLineRenderer3.material = new Material(Shader.Find("Sprites/Default"));
        BodyLineRenderer3.startColor = Color.black;//设置颜色
        BodyLineRenderer3.endColor = Color.black;
        BodyLineRenderer3.startWidth = 0.001f;//设置宽度
        BodyLineRenderer3.endWidth = 0.001f;
        BodyLineRenderer3.SetPosition(0, p0);
        BodyLineRenderer3.SetPosition(1, p1);
        Vector3d direction = p1 - p0;
        Vector3d directionAnthor = p0 - p1;
        direction.Normalize();
        directionAnthor.Normalize();

        HeadLineRenderer3.material = new Material(Shader.Find("Sprites/Default"));
        HeadLineRenderer3.startColor = Color.black;//设置颜色
        HeadLineRenderer3.endColor = Color.black;
        HeadLineRenderer3.startWidth = 0.005f;//设置宽度
        HeadLineRenderer3.endWidth = 0;
        Vector3d HeadStart = p1;
        Vector3d HeadEnd = HeadStart + direction * 0.02;
        HeadLineRenderer3.SetPosition(0, HeadStart);
        HeadLineRenderer3.SetPosition(1, HeadEnd);

        HeadLineRendererAnthor3.material = new Material(Shader.Find("Sprites/Default"));
        HeadLineRendererAnthor3.startColor = Color.black;//设置颜色
        HeadLineRendererAnthor3.endColor = Color.black;
        HeadLineRendererAnthor3.startWidth = 0.005f;//设置宽度
        HeadLineRendererAnthor3.endWidth = 0;
        Vector3d HeadStartAnthor = p0;
        Vector3d HeadEndAnthor = HeadStartAnthor + directionAnthor * 0.01;
        HeadLineRendererAnthor3.SetPosition(0, HeadStartAnthor);
        HeadLineRendererAnthor3.SetPosition(1, HeadEndAnthor);

        _arrowGam3.Add(HeadArrowGam3);
        _arrowGam3.Add(BodyArrowGam3);
        _arrowGam3.Add(HeadArrowGamAnother3);
    }
}
