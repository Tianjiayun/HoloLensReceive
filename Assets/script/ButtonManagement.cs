using System.Collections.Generic;
using UnityEngine;
using Mathd;
using Microsoft.MixedReality.Toolkit.UI;
using System;

public class ButtonManagement: MonoBehaviour
{
    public Save _save;
    public DrawBoundary _drawBoundary;
    public CreateLine _createLine;
    public mainPointMainpulation _main;
    
    int clickCount = 0;
    int clickTwoCount = 0;
    public bool isOthers = false;
    public bool isEditor = false;
    public OSC oscHandler;

    OscMessage msg = new OscMessage();
    public void clicksave()
    {
        msg.address = "/buttonAction";
        msg.values.Add("save");
        oscHandler.Send(msg);
        msg.values.Clear();
    }
    public void clickCancel()
    {
        msg.address = "/buttonAction";
        msg.values.Add("cancel");
        oscHandler.Send(msg);
        msg.values.Clear();
        //isEditor = false;
        if (_main._cancelDownPoints.Count > 0)
        {
            List<Vector3> cancelDownPos = new List<Vector3>();
            List<Vector3> cancelOutPos = new List<Vector3>();
            List<Vector3> cancelClonePos = new List<Vector3>();
            for (int i = 0; i < _main._cancelDownPoints[_main._cancelDownPoints.Count - 1].Count; i++)
            {
                cancelDownPos.Add(_main._cancelDownPoints[_main._cancelDownPoints.Count - 1][i]);
                //cancelOutPos.Add(_main._cancelOutPoints[_main._cancelOutPoints.Count - 1][i]);
                cancelClonePos.Add(_main._cancelClonePoints[_main._cancelClonePoints.Count - 1][i]);
                _main.downPoints[i] = cancelDownPos[i];
               // _main._outDownPoints[i] = cancelOutPos[i];
                _main._pointClone[i] = cancelClonePos[i];
            }
            _drawBoundary.DrawCatmullRom(cancelOutPos, CreateLine.lines[1]);
            cancelOutPos.RemoveAt(cancelOutPos.Count - 1);
            //_drawBoundary.DrawCatmullRom(cancelDownPos, CreateLine.lines[0]);
            //cancelDownPos.RemoveAt(cancelDownPos.Count - 1);
            for (int i = 0; i < _main._cancelDownPoints[_main._cancelDownPoints.Count - 1].Count; i++)
            {
                CreateLine.objArray[0][i].transform.position = _main._cancelDownPoints[_main._cancelDownPoints.Count - 1][i];
            }
            if (_main._editorI.Count > 0)
            {
                if (Math.Abs(_main._pointClone[_main._editorI[_main._editorI.Count - 1]].x - _main.downPoints[_main._editorI[_main._editorI.Count - 1]].x) > 0.0001 && Math.Abs(_main._pointClone[_main._editorI[_main._editorI.Count - 1]].y - _main.downPoints[_main._editorI[_main._editorI.Count - 1]].y) > 0.0001)
                {
                    _main.displayAllowance(_main._pointClone[_main._editorI[_main._editorI.Count - 1]], _main.downPoints[_main._editorI[_main._editorI.Count - 1]]);
                }
                else
                {
                    _main.displayAllowance(_main._editorI[_main._editorI.Count - 1]);
                }
                _main._editorI.RemoveAt(_main._editorI.Count - 1);
            }
            else
            {
                _main._distanceText.text = "0.00";
                _main._distanceText.fontSize = 80;
                _main._distanceText.rectTransform.localScale = new Vector3d(0.001, 0.001, 0.001);
                _main._distanceText.transform.position = new Vector3d(cancelDownPos[0].x + 0.005, cancelDownPos[0].y + 0.1, cancelDownPos[0].z);
            }
            _main._cancelDownPoints.RemoveAt(_main._cancelDownPoints.Count - 1);
            //_main._cancelOutPoints.RemoveAt(_main._cancelOutPoints.Count - 1);
            _main._cancelClonePoints.RemoveAt(_main._cancelClonePoints.Count - 1);

        }
        else
        {
            Debug.Log("no editor!");
        }
    }
    public void HideControlPoint()
    {
        isEditor = true;
        if (clickCount == 0)
        {
            for (int i = 0; i < CreateLine.objArray.Count; i++)
            {
                for (int j = 0; j < CreateLine.objArray[i].Length; j++)
                {
                    CreateLine.objArray[i][j].gameObject.SetActive(false);
                    CreateLine.objArray[i][j].isStatic = true;
                }

            }
            CreateLine.testList[2].gameObject.SetActive(false);
            clickCount++;
        }
        else
        {
            for (int i = 0; i < CreateLine.objArray.Count; i++)
            {
                for (int j = 0; j < CreateLine.objArray[i].Length; j++)
                {
                    CreateLine.objArray[i][j].gameObject.SetActive(true);
                    CreateLine.objArray[i][j].isStatic = false;
                }
            }
            CreateLine.testList[2].gameObject.SetActive(true);
            clickCount = 0;
        }

    }
    public void OneOrOthers()
    {
        isEditor = true;
        if (clickTwoCount == 0)
        {
            isOthers = true;
            clickTwoCount++;
        }
        else
        {
            isOthers = false;
            clickTwoCount = 0;
        }
        
    }
    public void OnSliderUpdated(SliderEventData eventData)
    {
        _main.text.text = $"{eventData.NewValue:F2}";
        _main._AdjustSlider = eventData.NewValue ;
    }

}
