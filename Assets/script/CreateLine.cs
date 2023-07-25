using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mathd;

public class CreateLine : MonoBehaviour
{
    //[HideInInspector]
    public static GameObject test;//画线的物体
    //[HideInInspector]
    public static LineRenderer line;
    GameObject obj;//点的位置
    //[HideInInspector]
    public static  List<LineRenderer> lines = new List<LineRenderer>();//lineRenderer List
    public static List<GameObject> testList = new List<GameObject>();//lineRenderer List
    public static List<string> lineTag = new List<string>();
    public static List<List<string>> _pointTag= new List<List<string>>(); 
    List<string> _objTag= new List<string>();
    //[HideInInspector]
    public static List<GameObject[]> objArray = new List<GameObject[]>();//存放boxcollider,修改控制点的时候使用的  
    public static GameObject[] objects;
    int pointIndex = 0;
    public void Protract(string name, Color proColor, double proWidth)
    {
        pointIndex = 0;
        test = new GameObject(name);
        line = test.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = proColor;//设置颜色
        line.endColor = proColor;
        line.startWidth = (float)proWidth;//设置宽度
        line.endWidth = (float)proWidth;
        testList.Add(test);
        lines.Add(line);
        lineTag.Add(name);
    }
    public void PointOtherProtract(int positionCount,  double proWidth, List<Vector3> points)
    {
        objects = new GameObject[positionCount];
        for (int i = 0; i < positionCount; i++)
        {            
            obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);// new GameObject(name);
            obj.name = "keypoint0" + pointIndex;
            obj.AddComponent<ObjectManipulator>();
            obj.AddComponent<NearInteractionGrabbable>();
            obj.GetComponent<MeshRenderer>().material.color = Color.gray;
            obj.transform.position = points[i];
            obj.transform.parent = line.transform;
            obj.transform.localScale = new Vector3((float)proWidth, (float)proWidth, (float)proWidth);
            objects[i] = obj;
            _objTag.Add("keypoint0" + pointIndex);
            pointIndex++;
        }
        _pointTag.Add(_objTag);
        objArray.Add(objects);

    }
    public void RePointOtherProtract(int positionCount, double proWidth, List<Vector3> points, int index)
    {
        objects = new GameObject[positionCount];
        pointIndex = 0;
        for (int i = 0; i < positionCount; i++)
        {
            obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);// new GameObject(name);
            obj.AddComponent<ObjectManipulator>();
            obj.AddComponent<NearInteractionGrabbable>();
            obj.GetComponent<MeshRenderer>().material.color = Color.gray;
            obj.transform.position = points[i];
            obj.transform.parent = lines[index].transform;
            obj.name = "keypoint0" + pointIndex;
            obj.transform.localScale = new Vector3((float)proWidth, (float)proWidth, (float)proWidth);
            objects[i] = obj;
            pointIndex++;
        }
        objArray.Add(objects);

    }
}
