using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Mathd;
public class readData : MonoBehaviour
{
    
    /// <summary>
    /// 读边界点
    /// </summary>
    /// <param name="lines"></param>
    public List<Vector3d> readout_interBoundary(List<string> lines, string path)
    {
        List<Vector3d> _posData3D = new List<Vector3d>();
        if(lines.Count == 0)
        {
            Debug.Log("no save datas");
            return null;
        }
        else
        {
            for (int i = 0; i < lines.Count; i++)
            {

                if (lines[i] == string.Empty)
                {
                    lines.RemoveAt(i);
                    File.WriteAllLines(path, lines.ToArray());
                }
            }
            for (int i=11;i<lines.Count; i++)
            {                
                _posData3D.Add(_Parse(lines[i]));
            }           
            return _posData3D;
           // return posData;
        }
    }
    //字符串转换成vector3
     Vector3 Parse(string str)
     {
        string[] s = str.Split(' ');
        return new Vector3((float)(double.Parse(s[0])*0.01), (float)(double.Parse(s[1])*0.01), (float)(-double.Parse(s[2])*0.01));
     }
    Vector3d _Parse(string str)
    {
        string[] s = str.Split(' ');
        return new Vector3d(double.Parse(s[0]) * 0.01, double.Parse(s[1]) * 0.01, double.Parse(s[2]) * (-0.01));
    }

}
