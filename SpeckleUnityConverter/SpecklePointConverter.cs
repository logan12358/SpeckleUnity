using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SpeckleUnityConverter
{
    public static class SpecklePointConverter
    {
        public static GameObject ToNative(this SpeckleCore.SpecklePoint specklePoint)
        {        
            GameObject go = SpecklePrefabs.InstantiatePoint();
            go.GetComponent<UnitySpeckleObjectData>().Id = specklePoint._id;
            go.GetComponent<UnitySpeckleObjectData>().SpeckleObject = specklePoint;

            Vector3 unityPoint = Util.SpecklePointsToUnityPoints(specklePoint.Value)[0];
            Vector3[] pts = new Vector3[] { unityPoint, unityPoint };
            go.GetComponent<LineRenderer>().positionCount = pts.Length;
            go.GetComponent<LineRenderer>().SetPositions(pts);

            return go;
        }
    }
}
