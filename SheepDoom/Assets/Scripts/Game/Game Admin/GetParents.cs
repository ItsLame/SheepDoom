using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SheepDoom
{
    public class GetParents : MonoBehaviour
    {
        [SerializeField]
        private GameObject parent;

        public GameObject getParent()
        {
            return parent;
        }
    }
}

