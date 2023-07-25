using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NSMatrix;
using System;
using System.Numerics;
using Mathd;

namespace FitPlane
{
    class Program
    {
        static Matrix matrix1, matrix2;
        public static double sumXX = 0, sumXY = 0, sumX = 0, sumY = 0, sumXZ = 0, sumYZ = 0, sumZ = 0, sumYY = 0;
        static double[,] data, result;
        /// <summary>
        /// 求解平面方程的系数
        /// </summary>
        /// <param name="points"></param>
        public static Matrix plane(List<UnityEngine.Vector3> points)
        {
            data = new double[3, 3];
            result = new double[3, 1];
            for (int i = 0; i < points.Count; i++)
            {
                sumXX += Math.Pow(points[i].x, 2);
                sumXY += points[i].x * points[i].y;
                sumX += points[i].x;
                sumYY += Math.Pow(points[i].y, 2);
                sumY += points[i].y;
                sumXZ += points[i].z * points[i].x;
                sumYZ += points[i].y * points[i].z;
                sumZ += points[i].z;
            }
            data[0, 0] = sumXX;
            data[0, 1] = data[1, 0] = sumXY;
            data[0, 2] = data[2, 0] = sumX;
            data[1, 1] = sumYY;
            data[1, 2] = data[2, 1] = sumY;
            data[2, 2] = points.Count;
            result[0, 0] = sumXZ;
            result[1, 0] = sumYZ;
            result[2, 0] = sumZ;
            matrix1 = new Matrix(data);
            matrix2 = new Matrix(result);
            //return (matrix1.Transpose().Multiply(temp)).InvertGaussJordan().Multiply(temp.Transpose()).Multiply(matrix2);
            return matrix1.InvertGaussJordan().Multiply(matrix2);


        }
        public static Matrix plane2(List<Vector3d> points)
        {
            Matrix M;
            Matrix temp;
            Matrix L;
            double[,] data = new double[points.Count, 1];
            double[,] M_B = new double[3, points.Count];
            for (int j = 0; j < points.Count; j++)
            {
                data[j, 0] = 1;
            }
            L = new Matrix(data);
            for (int i = 0; i < points.Count; i++)
            {
                M_B[0, i] = points[i].x;
                M_B[1, i] = points[i].y;
                M_B[2, i] = points[i].z;
            }
            M = new Matrix(M_B);
            temp = new Matrix(M_B);
            return (M.Multiply(temp.Transpose())).InvertGaussJordan().Multiply(M).Multiply(L);
        }

    }
}
