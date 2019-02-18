using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;


namespace MoreGizmos
{
    class GizmosExEditor : MonoBehaviour
    {
        [MenuItem("GameObject/MoreGizmos/GizmosEx", false, 1)]
        static void CreateGizmosEx()
        {
            var existingGizmo = UnityEngine.Object.FindObjectOfType<GizmosEx>();
            if( existingGizmo != null)
            {
                Debug.LogWarning("GizmosEx already exists in the scene; aborting creation.", existingGizmo );
                return;
            }

            GameObject installer = new GameObject("GizmosEx");
            installer.AddComponent<GizmosEx>();
        }
    }
}