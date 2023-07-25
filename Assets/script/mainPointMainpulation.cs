using System.Collections.Generic;
using UnityEngine;
using Mathd;
using TMPro;
using System;
using Vuforia;
using System.IO;
public class mainPointMainpulation : MonoBehaviour
{
    [Header("script")]
    public readData _readDataScript;
    public DrawBoundary _drawBoundary;
    public Save _save;
    public CreateLine _createLine;
    public Trigger _trigger;
    public ButtonManagement _buttonManagement;
    public Receive _receive;
    [Header("Button")]
    public TextMeshPro text;//滑条对应的值   
    public double _AdjustSlider=1;//滑条对应的值
    public TextMeshPro _distanceText;//距离值
    public TextMeshPro _distanceYText;

    [Header("point")]
    public List<Vector3> endPoint = new List<Vector3>();//编辑完最后保存的值
    public List<Vector3> downPoints = new List<Vector3>();//降采样之后的点
    public List<Vector3> _pointClone = new List<Vector3>();//克隆的点，为了编辑点进行测量
    public List<Vector3> _outDownPoints = new List<Vector3>();

    [Header("temp")]
    public List<List<Vector3>> _cancelDownPoints = new List<List<Vector3>>();
    public List<List<Vector3>> _cancelOutPoints = new List<List<Vector3>>();
    public List<List<Vector3>> _cancelClonePoints = new List<List<Vector3>>();
    List<Vector3> cancelDownTemp;
    List<Vector3> cancelOutTemp;
    List<Vector3> cancelCloneTemp;

    int moveIndex = 0;
    public int editorCount = 0;
    public OSC oscHandler;    
    public enum deltaxy
    {
        move_deltax,
        move_deltay,
        move_deltaxy
    }
    deltaxy _moveOritation;
    public List<int> _editorIndex = new List<int>();
    public List<int> _editorI = new List<int>();
    OscMessage editorMsg = new OscMessage();
    OscMessage endMsg = new OscMessage();
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(downPoints.Count > 0)
        {
            if (_buttonManagement.isEditor)
            {
                if (_buttonManagement.isOthers)
                {
                    editorAction();
                   
                }
                else
                {
                    twoeditorAction();

                }
                if (Math.Abs(_pointClone[moveIndex].x - downPoints[moveIndex].x) > 0.0001 && Math.Abs(_pointClone[moveIndex].y - downPoints[moveIndex].y) > 0.0001)
                {
                    displayAllowance(_pointClone[moveIndex], downPoints[moveIndex]);
                }
                else
                {
                    displayAllowance(moveIndex);
                }

            }

        }
        
    }
    /// <summary>
    /// 修改边界
    /// </summary>
    /// <param name="Pos"></param>
    /// <returns></returns>
    void editorAction()
    {
        if (_trigger.isManipulating && _receive.isMan && !_receive.isEditor_again)
        {
            if (editorCount == 0 && _editorIndex.Count > 0)
            {
                editorMsg.address = "/editorAction";
                _editorIndex.Clear();
                editorCount++;
                cancelDownTemp = new List<Vector3>();
                cancelOutTemp = new List<Vector3>();
                cancelCloneTemp = new List<Vector3>();
                for (int i = 0; i < downPoints.Count; i++)
                {
                    cancelDownTemp.Add(downPoints[i]);
                    cancelOutTemp.Add(_outDownPoints[i]);
                    cancelCloneTemp.Add(_pointClone[i]);
                    if (editorMsg.values.Count < 1)
                    {
                        editorMsg.values.Add(downPoints[i]);
                        editorMsg.values.Add(_outDownPoints[i]);
                    }
                    else
                    {
                        editorMsg.values[0] = downPoints[i];
                        editorMsg.values[1] = _outDownPoints[i];
                    }
                    oscHandler.Send(editorMsg);
                }
                _editorI.Add(moveIndex);
                _cancelDownPoints.Add(cancelDownTemp);
                _cancelOutPoints.Add(cancelOutTemp);
                _cancelClonePoints.Add(cancelCloneTemp);
                editorMsg.values.Add(1);
                oscHandler.Send(editorMsg);
                editorMsg.values.Clear();
            }
            for (int i = 0; i < CreateLine.objArray[0].Length; i++)
            {
                if (_trigger.handName == CreateLine._pointTag[0][i])
                {
                    if (Vector3.Distance(downPoints[i], CreateLine.objArray[0][i].transform.position) > 0.01)
                    {
                        editorCount++;
                        moveIndex = i;
                        _editorIndex.Add(i);
                        double deltax = (CreateLine.objArray[0][i].transform.position.x - downPoints[i].x) * _AdjustSlider;
                        double deltay = (CreateLine.objArray[0][i].transform.position.y - downPoints[i].y) * _AdjustSlider;
                        downPoints[i] = new Vector3d(downPoints[i].x + deltax, downPoints[i].y + deltay, downPoints[i].z);
                        _outDownPoints[i] = new Vector3d(_outDownPoints[i].x + deltax, _outDownPoints[i].y + deltay, _outDownPoints[i].z);
                        if (_editorIndex.Count > 0)
                        {
                            if (i == 0)
                            {
                                _pointClone[i] = moveOriantation(_pointClone[i], _pointClone[_pointClone.Count - 1], _pointClone[i + 1], _pointClone[i], deltax, deltay, i);
                            }
                            else if (i == downPoints.Count - 1)
                            {
                                _pointClone[i] = moveOriantation(_pointClone[i], _pointClone[i - 1], _pointClone[0], _pointClone[i], deltax, deltay, i);
                            }
                            else
                            {
                                _pointClone[i] = moveOriantation(_pointClone[i], _pointClone[i - 1], _pointClone[i + 1], _pointClone[i], deltax, deltay, i);
                            }
                        }
                        else
                        {
                            if (i == 0)
                            {
                                _pointClone[i] = moveOriantation2(_pointClone[i], _pointClone[downPoints.Count - 1], _pointClone[i + 1], _pointClone[i], deltax, deltay, i);
                            }
                            else if (i == downPoints.Count - 1)
                            {
                                _pointClone[i] = moveOriantation2(_pointClone[i], _pointClone[i - 1], _pointClone[0], _pointClone[i], deltax, deltay, i);
                            }
                            else
                            {
                                _pointClone[i] = moveOriantation2(_pointClone[i], _pointClone[i - 1], _pointClone[i + 1], _pointClone[i], deltax, deltay, i);
                            }
                        }
                        for (int j = 0; j < downPoints.Count; j++)
                        {
                            if ((Math.Abs(downPoints[j].y - downPoints[i].y) < 0.002) && i != j)
                            {
                                _pointClone[j] = new Vector3d(_pointClone[j].x + deltax, _pointClone[j].y, _pointClone[j].z);
                                downPoints[j] = new Vector3d(downPoints[j].x + deltax, downPoints[j].y + deltay, downPoints[j].z);
                                _outDownPoints[j] = new Vector3d(_outDownPoints[j].x + deltax, _outDownPoints[j].y + deltay, _outDownPoints[j].z);
                            }
                            else if (Math.Abs(downPoints[j].x - downPoints[i].x) < 0.001 && i != j)
                            {
                                _pointClone[j] = new Vector3d(_pointClone[j].x, _pointClone[j].y + deltay, _pointClone[j].z);
                                downPoints[j] = new Vector3d(downPoints[j].x + deltax, downPoints[j].y + deltay, downPoints[j].z);
                                _outDownPoints[j] = new Vector3d(_outDownPoints[j].x + deltax, _outDownPoints[j].y + deltay, _outDownPoints[j].z);
                            }
                        }

                    }
                }
            }
        }
        else if (editorCount > 0)
        {
            editorCount = 0;
            _drawBoundary.DrawCatmullRom(downPoints, CreateLine.lines[0]);
            downPoints.RemoveAt(downPoints.Count - 1);
            _drawBoundary.DrawCatmullRom(_outDownPoints, CreateLine.lines[1]);
            _outDownPoints.RemoveAt(_outDownPoints.Count - 1);

            for (int k = 0; k < CreateLine.objArray[0].Length; k++)
            {
                CreateLine.objArray[0][k].transform.position = downPoints[k];
            }
            endMsg.address = "/end";
            endMsg.values.Add(1);
            oscHandler.Send(endMsg);
            endMsg.values.Clear();
        }
        if (_receive.isEditor_again && _cancelDownPoints.Count > 0)
        {
            for (int k = 0; k < CreateLine.objArray[0].Length; k++)
            {
                if (editorMsg.values.Count < 1)
                {
                    editorMsg.values.Add(downPoints[k]);
                    editorMsg.values.Add(_outDownPoints[k]);
                }
                else
                {
                    editorMsg.values[0] = downPoints[k];
                    editorMsg.values[1] = _outDownPoints[k];
                }
                oscHandler.Send(editorMsg);
            }
            editorMsg.values.Add(1);
            oscHandler.Send(editorMsg);
            editorMsg.values.Clear();
            _receive.isEditor_again = false;
        }
    }
    void twoeditorAction()
    {
        if (_trigger.isManipulating && _receive.isMan && !_receive.isEditor_again)
        {
            if (editorCount == 0 && _editorIndex.Count > 0)
            {
                editorMsg.address = "/editorAction";
                _editorIndex.Clear();
                editorCount++;
                cancelDownTemp = new List<Vector3>();
                //cancelOutTemp = new List<Vector3>();
                cancelCloneTemp = new List<Vector3>();
                for (int i = 0; i < downPoints.Count; i++)
                {
                    cancelDownTemp.Add(downPoints[i]);
                    //cancelOutTemp.Add(_outDownPoints[i]);
                    cancelCloneTemp.Add(_pointClone[i]);
                    if (editorMsg.values.Count < 1)
                    {
                        editorMsg.values.Add(downPoints[i]);
                        //editorMsg.values.Add(_outDownPoints[i]);
                    }
                    else
                    {
                        editorMsg.values[0] = downPoints[i];
                        //editorMsg.values[1] = _outDownPoints[i];
                    }
                    oscHandler.Send(editorMsg);
                }
                _editorI.Add(moveIndex);
                _cancelDownPoints.Add(cancelDownTemp);
                //_cancelOutPoints.Add(cancelOutTemp);
                _cancelClonePoints.Add(cancelCloneTemp);
                editorMsg.values.Add(1);
                oscHandler.Send(editorMsg);
                editorMsg.values.Clear();
            }

            for (int i = 0; i < CreateLine.objArray[0].Length; i++)
            {
                if (_trigger.handName == CreateLine._pointTag[0][i])
                {
                    if (Vector3.Distance(downPoints[i], CreateLine.objArray[0][i].transform.position) > 0.01)
                    {
                        moveIndex = i;
                        editorCount++;
                        double deltax = (CreateLine.objArray[0][i].transform.position.x - downPoints[i].x) * _AdjustSlider;
                        double deltay = (CreateLine.objArray[0][i].transform.position.y - downPoints[i].y) * _AdjustSlider;
                        downPoints[i] = new Vector3d(downPoints[i].x + deltax, downPoints[i].y + deltay, downPoints[i].z);
                        //_outDownPoints[i] = new Vector3d(_outDownPoints[i].x + deltax, _outDownPoints[i].y + deltay, _outDownPoints[i].z);
                        if (_editorIndex.Count > 0)
                        {
                            if (i == 0)
                            {
                                _pointClone[i] = moveOriantation(_pointClone[i], _pointClone[_pointClone.Count - 1], _pointClone[i + 1], _pointClone[i], deltax, deltay, i);
                            }
                            else if (i == downPoints.Count - 1)
                            {
                                _pointClone[i] = moveOriantation(_pointClone[i], _pointClone[i - 1], _pointClone[0], _pointClone[i], deltax, deltay, i);
                            }
                            else
                            {
                                _pointClone[i] = moveOriantation(_pointClone[i], _pointClone[i - 1], _pointClone[i + 1], _pointClone[i], deltax, deltay, i);
                            }
                        }
                        else
                        {
                            if (i == 0)
                            {
                                _pointClone[i] = moveOriantation2(_pointClone[i], _pointClone[_pointClone.Count - 1], _pointClone[i + 1], _pointClone[i], deltax, deltay, i);
                            }
                            else if (i == downPoints.Count - 1)
                            {
                                _pointClone[i] = moveOriantation2(_pointClone[i], _pointClone[i - 1], _pointClone[0], _pointClone[i], deltax, deltay, i);
                            }
                            else
                            {
                                _pointClone[i] = moveOriantation2(_pointClone[i], _pointClone[i - 1], _pointClone[i + 1], _pointClone[i], deltax, deltay, i);
                            }
                        }
                        _editorIndex.Add(i);
                    }
                }

            }

        }
        else if (editorCount > 0)
        {
            editorCount = 0;
            _drawBoundary.DrawCatmullRom(downPoints, CreateLine.lines[0]);
            downPoints.RemoveAt(downPoints.Count - 1);
            //_drawBoundary.DrawCatmullRom(_outDownPoints, CreateLine.lines[1]);
            //_outDownPoints.RemoveAt(_outDownPoints.Count - 1);
            for (int k = 0; k < CreateLine.objArray[0].Length; k++)
            {
                CreateLine.objArray[0][k].transform.position = downPoints[k];
            }

            endMsg.address = "/end";
            endMsg.values.Add(1);
            oscHandler.Send(endMsg);
            endMsg.values.Clear();

        }
        if (_receive.isEditor_again && _cancelDownPoints.Count > 0)
        {
            for (int k = 0; k < CreateLine.objArray[0].Length; k++)
            {
                if (editorMsg.values.Count < 1)
                {
                    editorMsg.values.Add(downPoints[k]);
                    //editorMsg.values.Add(_outDownPoints[k]);
                }
                else
                {
                    editorMsg.values[0] = downPoints[k];
                    //editorMsg.values[1] = _outDownPoints[k];
                }
                oscHandler.Send(editorMsg);
            }
            editorMsg.values.Add(1);
            oscHandler.Send(editorMsg);
            editorMsg.values.Clear();
            _receive.isEditor_again = false;
        }
    }
    /// <summary>
    /// 间隙的显示
    /// </summary>
    /// <param name="lines"></param>
    /// <returns></returns>
    public void displayAllowance(int moveIndex)
    {
        _drawBoundary.DrawArrow(_pointClone[moveIndex], downPoints[moveIndex]);
        double distance = Vector3.Distance(_pointClone[moveIndex], downPoints[moveIndex]);
        _distanceText.text = $"{distance:F3}";
        _distanceText.fontSize = 80;
        _distanceText.rectTransform.localScale = new Vector3d(0.001, 0.001, 0.001);
        _distanceText.transform.position = (_pointClone[moveIndex] + downPoints[moveIndex]) / 2;
    }
    public void displayAllowance(Vector3d p1, Vector3d p2)
    {
        Vector3d px = new Vector3d(p2.x, p1.y, p1.z);
        Vector3d py=new Vector3d(p1.x, p2.y, p1.z);
        _drawBoundary.DrawArrow2(p1, px);
        _drawBoundary.DrawArrow3(p1,py);
        double distancex = Vector3.Distance(p1, px);
        double distancey=Vector3d.Distance(p1, py);
        _distanceYText.text = $"{distancey:F3}";
        _distanceYText.fontSize = 80;
        _distanceYText.rectTransform.localScale = new Vector3d(0.001, 0.001, 0.001);
        _distanceYText.transform.position = (p1 + py) / 2;

        _distanceText.text = $"{distancex:F3}";
        _distanceText.fontSize = 80;
        _distanceText.rectTransform.localScale = new Vector3d(0.001, 0.001, 0.001);
        _distanceText.transform.position = (p1 + px) / 2;
    }
    Vector3d moveOriantation2(Vector3d p, Vector3d p1, Vector3d p2, Vector3d point, double deltax, double deltay,int index)
    {
        double v = Vector3.Dot((p1 - p).normalized, (p2 - p).normalized);//求标量：a·b=|a||b|cos<a，b> 结果是一个标量。
        double angle = Mathf.Acos((float)v) * 180 / Math.PI;
        if (angle < 92 && angle >=89)
        {
            _moveOritation = deltaxy.move_deltaxy;
        }
        else
        {
            if (Math.Abs(p.x - p1.x) < 0.001 || Math.Abs(p.x - p2.x) < 0.001)
            {
                Debug.Log(deltay);
                point = new Vector3d(point.x, point.y + deltay, point.z);
                _moveOritation = deltaxy.move_deltay;
            }
            else if (Math.Abs(p.y - p1.y) < 0.002 || Math.Abs(p.y - p2.y) < 0.002)
            {                
                point = new Vector3d(point.x + deltax, point.y, point.z);
                _moveOritation = deltaxy.move_deltax;
            }
        }
        return point;
    }
    Vector3d moveOriantation(Vector3d p, Vector3d p1, Vector3d p2, Vector3d point, double deltax, double deltay, int index)
    {
        double v = Vector3.Dot((p1 - p).normalized, (p2 - p).normalized);//求标量：a·b=|a||b|cos<a，b> 结果是一个标量。
        double angle = Mathf.Acos((float)v) * 180 / Math.PI;//两个相邻向量之间的夹角，判断是否为顶点，如果是顶点就可是上下，左右移动，判断角度为88°
        if ((angle < 92) || (_moveOritation == deltaxy.move_deltaxy && _editorIndex[_editorIndex.Count - 1] == index))
        {
            _moveOritation = deltaxy.move_deltaxy;
        }
        else
        {
            if ((Math.Abs(p.x - p1.x) < 0.001 || Math.Abs(p.x - p2.x) < 0.001) || (_moveOritation == deltaxy.move_deltay && _editorIndex[_editorIndex.Count - 1] == index))
            {
                
                point = new Vector3d(point.x, point.y+deltay , point.z);                
                _moveOritation = deltaxy.move_deltay;

            }
            else if ((Math.Abs(p.y - p1.y) < 0.002 || Math.Abs(p.y - p2.y) < 0.002) || (_moveOritation == deltaxy.move_deltax && _editorIndex[_editorIndex.Count - 1] == index))
            {
                point = new Vector3d(point.x + deltax, point.y, point.z);                
                _moveOritation = deltaxy.move_deltax;
            }
        }
        return point;
    }

}
