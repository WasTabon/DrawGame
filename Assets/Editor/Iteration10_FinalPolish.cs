using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class Iteration10_FinalPolish
{
    [MenuItem("DrawGame/Add SFXManager to Bootstrap (Iteration 10)")]
    public static void AddSFXManagerToBootstrap()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name != "Bootstrap")
        {
            EditorUtility.DisplayDialog("Add SFXManager",
                "Open Bootstrap scene first.", "OK", "");
            return;
        }

        var existing = Object.FindObjectOfType<SFXManager>();
        if (existing != null)
        {
            Debug.Log("SFXManager already exists on Bootstrap scene.");
            return;
        }

        var go = new GameObject("SFXManager");
        go.AddComponent<SFXManager>();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("SFXManager added to Bootstrap scene!");
    }
}
