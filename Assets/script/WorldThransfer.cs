using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;

public class WorldThransfer : MonoBehaviour
{
    static Matrix4x4 hololensToWorldMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);// 计算从Hololens坐标系到世界坐标系的转换矩阵
    static Matrix4x4 worldToVuforiaMatrix; // 计算从世界坐标系到Vuforia坐标系的转换矩阵
    static Matrix4x4 hololensToVuforiaMatrix;// 计算从Hololens坐标系到Vuforia坐标系的转换矩阵
    static Vector3 virtualPositionInVuforia;
    static Matrix4x4 WorldTohololensMatrix = Matrix4x4.TRS(-Vector3.zero, Quaternion.Inverse(Quaternion.identity), Vector3.one);// 计算从Hololens坐标系到世界坐标系的转换矩阵
    static Matrix4x4 VuforiaToworldMatrix; // 计算从世界坐标系到Vuforia坐标系的转换矩阵
    static Matrix4x4 VuforiaTohololensMatrix;// 计算从Hololens坐标系到Vuforia坐标系的转换矩阵
    static Vector3 virtualPositionInHoloLens;
    public static Vector3 transformPos(Vector3 ImagePos,Quaternion ImageQuaternion,Vector3 pos)
    {
        worldToVuforiaMatrix = Matrix4x4.TRS(-ImagePos, Quaternion.Inverse(ImageQuaternion), Vector3.one);
        hololensToVuforiaMatrix = worldToVuforiaMatrix * hololensToWorldMatrix;
        virtualPositionInVuforia = hololensToVuforiaMatrix.MultiplyPoint(pos);
        return virtualPositionInVuforia; 
    }
    public static List<Vector3> transformPosList(Vector3 ImagePos, Quaternion ImageQuaternion, List<Vector3> pos)
    {
        List<Vector3> list = new List<Vector3>();
        worldToVuforiaMatrix = Matrix4x4.TRS(-ImagePos, Quaternion.Inverse(ImageQuaternion), Vector3.one);
        hololensToVuforiaMatrix = worldToVuforiaMatrix * hololensToWorldMatrix;
        for(int i=0;i<pos.Count;i++)
        {
            list.Add(hololensToVuforiaMatrix.MultiplyPoint(pos[i]));
        }
        return list;
    }
    public static Vector3 vuforiaToHoloLnesPos(Vector3 ImagePos, Quaternion ImageQuaternion, Vector3 pos)
    {
        VuforiaToworldMatrix = Matrix4x4.TRS(ImagePos, ImageQuaternion, Vector3.one);
        VuforiaTohololensMatrix =   WorldTohololensMatrix*VuforiaToworldMatrix;
        virtualPositionInHoloLens = VuforiaTohololensMatrix.MultiplyPoint(pos);
        return virtualPositionInHoloLens;
    }
    public static List<Vector3> vuforiaToHoloLnes(Vector3 ImagePos, Quaternion ImageQuaternion, List<Vector3> pos)
    {
        List<Vector3> list = new List<Vector3>();
        VuforiaToworldMatrix = Matrix4x4.TRS(ImagePos, ImageQuaternion, Vector3.one);
        VuforiaTohololensMatrix = WorldTohololensMatrix * VuforiaToworldMatrix;
        for (int i = 0; i < pos.Count; i++)
        {
            list.Add(VuforiaTohololensMatrix.MultiplyPoint(pos[i]));
        }
        return list;
    }
}

