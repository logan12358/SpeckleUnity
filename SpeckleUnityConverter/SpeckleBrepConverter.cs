using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SpeckleUnityConverter
{
    public static class SpeckleBrepConverter
    {
        public static GameObject ToNative(this SpeckleCore.SpeckleBrep speckleBrep)
        {
            return speckleBrep.DisplayValue.ToNative();
        }
    }
}
