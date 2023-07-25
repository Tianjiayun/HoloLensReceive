using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Mathd;

public class Save : MonoBehaviour
{
    StreamWriter writer;
    public void onSave(List<Vector3> pos)
    {
        FileInfo file = new FileInfo(Application.dataPath + "/" + "ProcessDrownData.txt");
        if (file.Exists)//存在txt，删除以前的内容
        {
            file.Refresh();//刷新对象的状态

        }
        WriteData(file, ConvertOutputInfoToString(pos));
    }
    public void onSaveOut(List<Vector3> pos)
    {
        FileInfo file = new FileInfo(Application.dataPath + "/" + "ProcessOutData.txt");
        if (file.Exists)//存在txt，删除以前的内容
        {
            file.Refresh();//刷新对象的状态

        }
        WriteData(file, ConvertOutputInfoToString(pos));
    }
    public void endOnSave(List<Vector3> pos)
    {
        FileInfo file = new FileInfo(Application.dataPath + "/" + "EndData.txt");
        if (file.Exists)//存在txt，删除以前的内容
        {
            file.Refresh();//刷新对象的状态

        }
        WriteData(file, ConvertOutputInfoToString(pos));
    }
    public void endBoundaryOnSave(List<Vector3> pos)
    {
        FileInfo file = new FileInfo(Application.dataPath + "/" + "EndBoundaryData.txt");
        string save_endOuntBoundary = Application.dataPath + "/" + "EndBoundaryData.txt";
        if (file.Exists)//存在txt，删除以前的内容
        {
            file.Refresh();//刷新对象的状态
            if (save_endOuntBoundary != null)
            {
                File.WriteAllText(save_endOuntBoundary, string.Empty);
            }

        }
        WriteData(file, ConvertOutputInfoToString(pos));
    }
    string ConvertOutputInfoToString(List<Vector3> pos)
    {
        string Output = string.Empty;
        for (int i = 0; i < pos.Count; i++)
        {
            Output += pos[i].ToString();
            if (i < pos.Count - 1)
            {
                Output += "\n";
            }
        }
        return Output;
    }
    bool CheckData()
    {
        return true;
    }
    void WriteData(FileInfo file, string message)
    {
        if (CheckData())
        {
            WriteData(message, file);
            Debug.Log("Data Saved!");

        }
        else
        {
            Debug.LogWarning("Please check data before saving!");
        }
    }
    void WriteData(string message, FileInfo file)
    {

        writer = file.AppendText();//创建一个StreamWriter，它向FileInfo的此实例表示的文件追加文本内容
        writer.WriteLine(message);
        writer.Flush();
        writer.Dispose();
        writer.Close();
    }
}