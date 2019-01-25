using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SpeckleUnityConverter
{
    class SpecklePrefabs : MonoBehaviour
    {
        public static SpecklePrefabs Instance { get; private set; }

        public GameObject MeshPrefab;
        public GameObject LinePrefab;
        public GameObject PointPrefab;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                throw new InvalidOperationException("A Converter component has already been instantiated");
            }

            Instance = this;
        }

        void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public static GameObject InstantiateMesh()
        {
            return Instantiate(Instance.MeshPrefab);
        }

        public static GameObject InstantiateLine()
        {
            return Instantiate(Instance.LinePrefab);
        }

        public static GameObject InstantiatePoint()
        {
            return Instantiate(Instance.PointPrefab);
        }
    }
}
