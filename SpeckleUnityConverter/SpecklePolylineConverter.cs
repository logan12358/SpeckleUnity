using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SpeckleUnityConverter
{
    public static class SpecklePolylineConverter
    {
        public static GameObject ToNative(this SpeckleCore.SpecklePolyline specklePolyline)
        {
            GameObject go = SpecklePrefabs.InstantiateLine();
            go.GetComponent<UnitySpeckleObjectData>().Id = specklePolyline._id;
            go.GetComponent<UnitySpeckleObjectData>().SpeckleObject = specklePolyline;

            Vector3[] pts = Util.SpecklePointsToUnityPoints(specklePolyline.Value);
            go.GetComponent<LineRenderer>().positionCount = pts.Length;
            go.GetComponent<LineRenderer>().SetPositions(pts);     
       
            return go;
        }
    }
}
