using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Allow for persistent settings (like Unity) where you can enable/disable gizmos for certain classes
// TODO: Print stack of strings
//   - this should accept a method used to determine a value to print to screen
// TODO: support execution during edit-time via EditorTimeScripts (i.e. EditorGizmoArrow) a la Unreal
// TODO: provide a separate namespace for Editor only calls like "MoreGizmos.Editor"
//   - the default will be for run-time. evaluate DEVELOPMENT_BUILD to automatically remove gizmos
//   - we'll also need another one to handle "MoreGizmos.Trace" that will persist into build
// TODO: have support for different generations of gizmos? (those that are one-frame vs. duration)

// LONG-TODO: Support execution at run-time https://docs.unity3d.com/ScriptReference/GL.html
//            I don't like the idea of modifying script execution order, so this should be
//            added to a custom update loop

// TODO-ENG: add abstraction layer for different gizmo renders (e.g. GLbased vs UnityGizmos based)

// TODO: consider looking into adding this into the PlayerUpdateLoop

// TODO: consider having an IGizmoContributor interface that MoreGizmos can gather gizmos from

// Retained mode wrapper for Unity's Gizmos class
namespace MoreGizmos
{
    public class GizmosEx : MonoBehaviour
    {
        class GizmoSphere : GizmoDraw
        {
            public GizmoSphere(Vector3 pos, float rad, Color col)
            {
                position = pos;
                radius = rad;
                color = col;
            }

            public float radius;

            public override void Draw()
            {
                Gizmos.DrawSphere(position, radius);
            }
        }
        class GizmoCube : GizmoDraw
        {
            public GizmoCube(Vector3 pos, Vector3 siz, Color col)
            {
                position = pos;
                size = siz;
                color = col;
            }

            public Vector3 size;
            public override void Draw()
            {
                Gizmos.DrawCube(position, size);
            }
        }
        class GizmoCircle : GizmoDraw
        {
            public Vector3 normal;
            public float radius;
            public int sides;

            public GizmoCircle(Vector3 pos, Vector3 norm, float rad, int sideCount, Color col)
            {
                Debug.Assert(sideCount > 2);

                position = pos;
                normal = norm;
                radius = rad;
                sides = sideCount;
                color = col;
            }

            public override void Draw()
            {
                float step = 2 * Mathf.PI / (float)sides;
                Quaternion rot = Quaternion.FromToRotation(Vector3.forward, normal);

                for(int i = 0; i < sides; ++i)
                {
                    int startIdx = i;
                    int endIdx = (i+1) % (sides);

                    Vector3 legStart = rot * new Vector3(radius * Mathf.Cos(step * startIdx), radius * Mathf.Sin(step * startIdx), 0.0f);
                    Vector3 legEnd = rot * new Vector3(radius * Mathf.Cos(step * endIdx),   radius * Mathf.Sin(step * endIdx), 0.0f);

                    Gizmos.DrawLine(position + legStart, position + legEnd);
                }

                return; // remove below when done
            }
        }
        class GizmoSquare : GizmoDraw
        {
            public float degrees;
            public Vector3 normal;
            public Vector2 size;

            public GizmoSquare(Vector3 center, Vector3 norm, Vector2 extents, float deg, Color col)
            {
                position = center;
                normal = norm;
                size = extents;
                color = col;
                degrees = deg;
            }

            public override void Draw()
            {
                Quaternion rot = Quaternion.FromToRotation(Vector3.forward, normal);

                Vector2 halfExt = size / 2;

                Quaternion localRot = Quaternion.AngleAxis(degrees, Vector3.forward);

                Vector3[] verts =
                {
                    localRot * new Vector3(-halfExt.x,  halfExt.y, 0), // top-left
                    localRot * new Vector3( halfExt.x,  halfExt.y, 0), // top-right
                    localRot * new Vector3( halfExt.x, -halfExt.y, 0), // bottom-right
                    localRot * new Vector3(-halfExt.x, -halfExt.y, 0), // bottom-left
                };

                for(int i = 0; i < verts.Length; ++i)
                {
                    int endIdx = (i+1) % (verts.Length);

                    Vector3 legStart = (position + rot * verts[i]);
                    Vector3 legEnd = (position + rot * verts[endIdx]);

                    Gizmos.DrawLine(legStart, legEnd);
                }
            }
        }

        private List<GizmoDraw> gizmos = new List<GizmoDraw>();

        public static GizmosEx instance { get; private set; }

        public static void DrawSphere(Vector3 center, float radius, Color color = default(Color))
        {
            instance.gizmos.Add(new GizmoSphere( center, radius, color));
        }

        public static void DrawCube(Vector3 center, Vector3 size, Color color = default(Color))
        {
            instance.gizmos.Add(new GizmoCube( center, size, color ));
        }

        public static void DrawCircle(Vector3 center, Vector3 normal, float radius = 1.0f, int sides = 9, Color color = default(Color))
        {
            instance.gizmos.Add(new GizmoCircle( center, normal, radius, sides, color ));
        }

        public static void DrawSquare(Vector3 center, Vector3 normal, Vector2 size, float degrees = 0.0f, Color color = default(Color))
        {
            instance.gizmos.Add(new GizmoSquare( center, normal, size, degrees, color ));
        }

        public static void DrawCustomGizmo(GizmoDraw newGiz)
        {
            instance.gizmos.Add(newGiz);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InstantiateImplementation()
        {
            instance = new GameObject("[MoreGizmos]")
            {
                hideFlags = HideFlags.DontSave 
            }.AddComponent<GizmosEx>();
        }

        private void Start()
        {
            if(instance != this)
            {
                Debug.Log("Replacing existing implementation of MoreGizmos on " + instance.gameObject.name + " with implementation on " + gameObject.name);
                Destroy(instance);
                instance = this;
            }
        }

        private void OnDrawGizmos()
        {
            Color originalColor = Gizmos.color;

            for(int i = 0; i < gizmos.Count; ++i)
            {
                gizmos[i].BeforeDraw();
                gizmos[i].Draw();
            }

            gizmos.Clear();

            Gizmos.color = originalColor;
        }
    }

    public abstract class GizmoDraw
    {
        public Vector3 position;
        private Color _color;
        public Color color
        {
            get
            {
                return _color == Color.clear ? Color.magenta : _color;
            }
            set
            {
                _color = value;
            }
        }

        public virtual void BeforeDraw()
        {
            Gizmos.color = color;
        }
        public abstract void Draw();
    }

    public class MoreGizmosException : System.Exception
    {
        public MoreGizmosException()
        {
        }

        public MoreGizmosException(string message)
            : base(message)
        {
        }

        public MoreGizmosException(string message, System.Exception inner)
            : base(message, inner)
        {
        }
    }
}