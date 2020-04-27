using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TODO: Allow for persistent settings (like Unity) where you can enable/disable gizmos for certain classes
// TODO: Print stack of strings
//   - this should accept a method used to determine a value to print to screen
// TODO: support execution during edit-time via EditorTimeScripts (i.e. EditorGizmoArrow) a la Unreal
// TODO: provide a separate namespace for Editor/Developer only calls like "MoreGizmos.Editor"/"MoreGizmos.Editor"
//   - the default will be for run-time. evaluate DEVELOPMENT_BUILD to automatically remove gizmos
//   - we'll also need another one to handle "MoreGizmos.Trace" that will persist into build

// TODO: consider looking into adding this into the PlayerUpdateLoop

// TODO: consider having an IGizmoContributor interface that MoreGizmos can gather gizmos from

// LONG-TODO: Support execution at run-time https://docs.unity3d.com/ScriptReference/GL.html

// Retained mode wrapper for Unity's Gizmos class
namespace MoreGizmos
{
    public class GizmosEx
    {
        public class GizmoSphere : GizmoDraw
        {
            public GizmoSphere(Vector3 pos, float rad, Color col)
            {
                position = pos;
                radius = rad;
                color = col;
            }

            public float radius;

            public override void Draw(IGizmoRenderer renderer)
            {
                renderer.DrawSphere(position, radius);
            }
        }
        public class GizmoCube : GizmoDraw
        {
            public GizmoCube(Vector3 pos, Vector3 siz, Color col)
            {
                position = pos;
                size = siz;
                color = col;
            }

            public Vector3 size;
            public override void Draw(IGizmoRenderer renderer)
            {
                renderer.DrawCube(position, size);
            }
        }
        public class GizmoCircle : GizmoDraw
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

            public override void Draw(IGizmoRenderer renderer)
            {
                float step = 2 * Mathf.PI / (float)sides;
                Quaternion rot = Quaternion.FromToRotation(Vector3.forward, normal);

                for (int i = 0; i < sides; ++i)
                {
                    int startIdx = i;
                    int endIdx = (i + 1) % (sides);

                    Vector3 legStart = rot * new Vector3(radius * Mathf.Cos(step * startIdx), radius * Mathf.Sin(step * startIdx), 0.0f);
                    Vector3 legEnd = rot * new Vector3(radius * Mathf.Cos(step * endIdx), radius * Mathf.Sin(step * endIdx), 0.0f);

                    renderer.DrawLine(position + legStart, position + legEnd);
                }
            }
        }
        public class GizmoSquare : GizmoDraw
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

            public override void Draw(IGizmoRenderer renderer)
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

                for (int i = 0; i < verts.Length; ++i)
                {
                    int endIdx = (i + 1) % (verts.Length);

                    Vector3 legStart = (position + rot * verts[i]);
                    Vector3 legEnd = (position + rot * verts[endIdx]);

                    renderer.DrawLine(legStart, legEnd);
                }
            }
        }
        public class GizmoLine : GizmoDraw
        {
            public Vector3 startPosition;
            public Vector3 endPosition;

            public GizmoLine(Vector3 startPos, Vector3 endPos, Color col)
            {
                startPosition = startPos;
                endPosition = endPos;
                color = col;
            }

            public override void Draw(IGizmoRenderer renderer)
            {
                renderer.DrawLine(startPosition, endPosition);
            }
        }

        static GizmosEx()
        {
            instance = new GizmosEx();
        }
        public static GizmosEx instance;

        // TODO: replace this with a circular buffer or a ring buffer or *something*
        // TODO: tie this into the player update loop manually so we don't have to expose this publicly
        public List<GizmoDraw> gizmos = new List<GizmoDraw>();

        public static GizmoSphere DrawSphere(Vector3 center, float radius, Color color = default)
        {
            return DrawCustomGizmo(new GizmoSphere(center, radius, color));
        }

        public static GizmoCube DrawCube(Vector3 center, Vector3 size, Color color = default)
        {
            return DrawCustomGizmo(new GizmoCube(center, size, color));
        }

        public static GizmoCircle DrawCircle(Vector3 center, Vector3 normal, float radius = 1.0f, int sides = 9, Color color = default)
        {
            return DrawCustomGizmo(new GizmoCircle(center, normal, radius, sides, color));
        }

        public static GizmoSquare DrawSquare(Vector3 center, Vector3 normal, Vector2 size, float degrees = 0.0f, Color color = default)
        {
            return DrawCustomGizmo(new GizmoSquare(center, normal, size, degrees, color));
        }

        public static GizmoLine DrawLine(Vector3 startPos, Vector3 endPos, Color color = default)
        {
            return DrawCustomGizmo(new GizmoLine(startPos, endPos, color));
        }

        public static GizmoLine DrawRay(Vector3 origin, Vector3 ray, Color color = default)
        {
            return DrawCustomGizmo(new GizmoLine(origin, origin + ray, color));
        }

        public static T DrawCustomGizmo<T>(T newGiz) where T : GizmoDraw
        {
            if (newGiz.duration == 0.0f && Time.inFixedTimeStep)
            {
                newGiz.duration += Time.fixedDeltaTime;
            }

            instance.gizmos.Add(newGiz);
            return instance.gizmos[instance.gizmos.Count - 1] as T;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InstantiateImplementation()
        {
            if (GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IGizmoRenderer>().Count() == 0)
            {
                new GameObject("[MoreGizmos]")
                {
                    hideFlags = HideFlags.None
                }.AddComponent<GizmosExDebugRenderer>();
            }
        }
    }

    public abstract class GizmoDraw
    {
        public Matrix4x4 matrix = Matrix4x4.identity;
        public Vector3 position;
        public float duration;
        private readonly float spawnTime = Time.time;
        public Color color = Color.magenta;

        public virtual bool Expired
        {
            get
            {
                return Time.time >= spawnTime + duration;
            }
        }

        public virtual void BeforeDraw(IGizmoRenderer renderer)
        {
            renderer.SetColor(color);
            renderer.SetTRS(matrix);
        }
        public abstract void Draw(IGizmoRenderer renderer);
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

    public interface IGizmoRenderer
    {
        void SetColor(Color color);
        void SetTRS(Matrix4x4 trs);
        void DrawLine(Vector3 pointStart, Vector3 pointEnd);
        void DrawSphere(Vector3 center, float radius);
        void DrawCube(Vector3 position, Vector3 size);
    }
}