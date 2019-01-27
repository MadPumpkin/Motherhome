using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using UnityEngine;
using System.Text;
using System.IO;
using System.Reflection;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Legend
{
    public class World : MonoBehaviour
    {
        public static World Instance;

        public Material RedHitMaterial;
        public LayerMask LOSCheckMask;
        public LayerMask PickupPlacementCheckMask;
        public LayerMask PathFindingMask;

        [Header("Audio")]
        public AudioClip[] ButtonSelect;
        public AudioClip[] ButtonSubmit;

        [Header("Prefabs")]
        public GameObject CombatTextPrefab;

        [NonSerialized]
        public bool IsShuttingDown;

        public World()
        {
            Instance = this;
        }

        void OnApplicationQuit()
        {
            IsShuttingDown = true;
        }
    }
}