using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SpeckleUnityConverter
{
    public static class SpeckleCurveConverter
    {
        public static GameObject ToNative(this SpeckleCore.SpeckleCurve speckleCurve)
        {
            return speckleCurve.DisplayValue.ToNative();
        }
    }
}
