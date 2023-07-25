using System.Collections.Generic;
using UnityEngine;
using Mathd;
using Vuforia;

public class Receive : MonoBehaviour, ITrackableEventHandler
{
    public OSC oscHandler;
    public CreateLine _createLine;
    public DrawBoundary _drawBoundary;
    public ButtonManagement _buttonManagement;
    public mainPointMainpulation _main;
    public List<Vector3> _cancelDownPoints = new List<Vector3>();//降采样之后的点
    public List<Vector3> _cancelPointClone = new List<Vector3>();//克隆的点，为了编辑点进行测量
    public List<Vector3> _cancelOutDownPoints = new List<Vector3>();
    public Vector3d _fingerPos= Vector3d.zero;
    public string _tagName;
    public List<Vector3> _downPosInverse = new List<Vector3>();
    public List<Vector3> _outPosInverse=new List<Vector3>();
    public List<Vector3> _clonePosInverse=new List<Vector3>();
    List<Vector3> receiveDownPoint=new List<Vector3>();
    List<Vector3> receiveOutPoint = new List<Vector3>();
    List<Vector3> receiveClonePoint = new List<Vector3>();
    public GameObject vuforiaMarker;
    TrackableBehaviour mTrackableBehaviour;
    public bool isMan = true;
    public bool isEditor_again = false;
    // Start is called before the first frame update
    void Start()
    {
        mTrackableBehaviour = vuforiaMarker.GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
        _buttonManagement.isEditor = true;
        Application.runInBackground = true;
        oscHandler.SetAddressHandler("/sendPos", DrawOriginalBoundary);
        oscHandler.SetAddressHandler("/notAction", action);
        oscHandler.SetAddressHandler("/againEditorSend", sendPC);
    }
    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            //if (_main.downPoints.Count > 0)
            //{
            //    for (int i = 0; i < _main.downPoints.Count; i++)
            //    {
            //        _main.downPoints[i] = vuforiaMarker.transform.TransformPoint(_downPosInverse[i]);
            //        _main._pointClone[i] = vuforiaMarker.transform.TransformPoint(_clonePosInverse[i]);
            //        _main._outDownPoints[i] = vuforiaMarker.transform.TransformPoint(_outPosInverse[i]);
            //        Destroy(CreateLine.objArray[0][i]);
            //        //Destroy(CreateLine.objArray[1][i]);
            //    }
            //    CreateLine.objArray.Clear();
            //    _drawBoundary.DrawCatmullRom(_main.downPoints, CreateLine.lines[0]);
            //    _main.downPoints.RemoveAt(_main.downPoints.Count - 1);                
            //    _createLine.RePointOtherProtract(_main.downPoints.Count, 0.001f, _main.downPoints,0);
                
            //    _drawBoundary.DrawCatmullRom(_main._outDownPoints, CreateLine.lines[1]);
            //    _main._outDownPoints.RemoveAt(_main._outDownPoints.Count - 1);                
            //    //_createLine.RePointOtherProtract(_main._outDownPoints.Count, 0.001f, _main._outDownPoints,1);
               
            //    //_drawBoundary.DrawCatmullRom(_main._pointClone, CreateLine.lines[2]);
            //    //_main._pointClone.RemoveAt(_main._pointClone.Count - 1);
            //}

        }
        else
        {

        }
    }
    // Update is called once per frame
    void DrawOriginalBoundary(OscMessage msg)
    {
        _main.downPoints.Add(msg.GetVector3(0));
        //_main._outDownPoints.Add(msg.GetVector3(1));
        _main._pointClone.Add(msg.GetVector3(0));
        receiveDownPoint.Add(msg.GetVector3(0));
        //receiveOutPoint.Add(msg.GetVector3(1));
        receiveClonePoint.Add(msg.GetVector3(0));

        //_main.downPoints.Add(vuforiaMarker.transform.TransformPoint(msg.GetVector3d(0)));
        //_main._pointClone.Add(vuforiaMarker.transform.TransformPoint(msg.GetVector3d(0)));
        //_main._outDownPoints.Add(vuforiaMarker.transform.TransformPoint(msg.GetVector3d(1)));

        //if (msg.values.Count > 2 && _main.downPoints.Count > 0)
        if (msg.values.Count > 1 && _main.downPoints.Count > 0)
        {
            _main.downPoints.RemoveAt(_main.downPoints.Count - 1);
            _main._pointClone.RemoveAt(_main._pointClone.Count - 1);
            //_main._outDownPoints.RemoveAt(_main._outDownPoints.Count - 1);

            receiveDownPoint.RemoveAt(receiveDownPoint.Count - 1);
            //receiveOutPoint.RemoveAt(receiveOutPoint.Count - 1);
            receiveClonePoint.RemoveAt(receiveClonePoint.Count - 1);

            _main._cancelDownPoints.Add(receiveDownPoint);
            //_main._cancelOutPoints.Add(receiveOutPoint);
            _main._cancelClonePoints.Add(receiveClonePoint);
           // _main.editorCount = _main._cancelDownPoints.Count;

            _createLine.Protract("line_boundary", Color.white, 0.1f);
            _drawBoundary.DrawCatmullRom(_main.downPoints, CreateLine.line);
            _main.downPoints.RemoveAt(_main.downPoints.Count - 1);
            _createLine.PointOtherProtract(_main.downPoints.Count, 0.1f, _main.downPoints);

            //_createLine.Protract("line_ICP", Color.black, 0.01f);
            //_drawBoundary.DrawCatmullRom(_main._outDownPoints, CreateLine.line);
            //_main._outDownPoints.RemoveAt(_main._outDownPoints.Count - 1);

            for (int i = 0; i < _main.downPoints.Count; i++)
            {
                _downPosInverse.Add(vuforiaMarker.transform.InverseTransformPoint(_main.downPoints[i]));
                //_outPosInverse.Add(vuforiaMarker.transform.InverseTransformPoint(_main._outDownPoints[i]));
                _clonePosInverse.Add(vuforiaMarker.transform.InverseTransformPoint(_main._pointClone[i]));
            }
            receiveDownPoint =new List<Vector3>();
            receiveOutPoint = new List<Vector3>();
            receiveClonePoint = new List<Vector3>();
        }
    }
    void action(OscMessage msg)
    {
        if(msg.values.Count>0)
        {
            isMan = msg.GetBool(0);
        }
    }
    void sendPC(OscMessage msg)
    {
        if (msg.values.Count > 0)
        {
            isEditor_again = msg.GetBool(0);
        }
    }

}
