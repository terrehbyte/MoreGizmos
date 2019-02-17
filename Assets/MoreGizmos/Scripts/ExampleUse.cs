using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreGizmos
{
    public class ExampleUse : MonoBehaviour
    {
        public Color gizmoColor;

        // Update is called once per frame
        void Update()
        {
            GizmosEx.DrawCube(transform.position, transform.lossyScale, gizmoColor);
        }
    }
}