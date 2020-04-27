using System.Collections.Generic;
using UnityEngine;

namespace MoreGizmos
{
    public class GizmosExDebugRenderer : MonoBehaviour, IGizmoRenderer
    {
        public void SetColor(Color color)
        {
            Gizmos.color = color;
        }

        public void SetTRS(Matrix4x4 trs)
        {
            Gizmos.matrix = trs;
        }

        public void DrawCube(Vector3 position, Vector3 size)
        {
            Gizmos.DrawCube(position, size);
        }

        public void DrawLine(Vector3 pointStart, Vector3 pointEnd)
        {
            Gizmos.DrawLine(pointStart, pointEnd);
        }

        public void DrawSphere(Vector3 center, float radius)
        {
            Gizmos.DrawSphere(center, radius);
        }

        private void OnDrawGizmos()
        {
            for (int i = 0; i < GizmosEx.instance.gizmos.Count; ++i)
            {
                GizmosEx.instance.gizmos[i].BeforeDraw(this);
                GizmosEx.instance.gizmos[i].Draw(this);

                if (GizmosEx.instance.gizmos[i].Expired) { GizmosEx.instance.gizmos.RemoveAt(i); }
            }
        }
    }
}