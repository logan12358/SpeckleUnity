using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SpeckleUnityConverter
{
    static internal class Util
    {
        internal static Vector3 ToPoint(double x, double y, double z)
        {
            // Switch y and z
            return new Vector3((float)x, (float)z, (float)y);
        }

        internal static Vector3[] SpecklePointsToUnityPoints(List<double> specklePoints)
        {
            if (specklePoints.Count % 3 != 0) throw new Exception("Array malformed: length%3 != 0.");

            Vector3[] points = new Vector3[specklePoints.Count / 3];

            for (int i = 0; i < points.Length; i++)
            {
                points[i] = ToPoint(specklePoints[i * 3], specklePoints[i * 3 + 1], specklePoints[i * 3 + 2]);
            }
            
            return points;
        }

        internal static int[] SpeckleFacesToUnityTriangles(List<int> speckleFaces)
        {
            List<int> unityTriangles = new List<int>();

            for (int i = 0; i < speckleFaces.Count; i++)
            {
                if (speckleFaces[i] == 0) // Triangle
                {
                    unityTriangles.Add(speckleFaces[i+1]);
                    unityTriangles.Add(speckleFaces[i+3]);
                    unityTriangles.Add(speckleFaces[i+2]);
                    i += 3;
                }
                else if (speckleFaces[i] == 1) // Quad
                {
                    unityTriangles.Add(speckleFaces[i+1]);
                    unityTriangles.Add(speckleFaces[i+3]);
                    unityTriangles.Add(speckleFaces[i+2]);

                    unityTriangles.Add(speckleFaces[i+3]);
                    unityTriangles.Add(speckleFaces[i+1]);
                    unityTriangles.Add(speckleFaces[i+4]);
                    i += 4;
                }
                else
                {
                    throw new Exception("Speckle faces malformed: face not a triangle or a quad");
                }
            }

            return unityTriangles.ToArray();
        }
    }
}
