using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SpeckleUnityConverter
{
    class SpeckleConverter : MonoBehaviour
    {
        public static SpeckleConverter Instance { get; private set; }

        public GameObject MeshPrefab;
        public GameObject LinePrefab;
        public GameObject PointPrefab;

        void Awake()
        {
            if (Instance != null) throw new InvalidOperationException("A Converter component has already been instantiated");

            Instance = this;
        }

        void OnDestroy()
        {
            Instance = null;
        }
    }
}
