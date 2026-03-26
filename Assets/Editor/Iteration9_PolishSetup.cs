using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class Iteration9_PolishSetup
{
    [MenuItem("DrawGame/Update Game Scene - Polish Effects (Iteration 9)")]
    public static void UpdateGameScene()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name != "Game")
        {
            if (!EditorUtility.DisplayDialog("Update Game Scene",
                "Current scene is '" + scene.name + "'. Are you on the Game scene?",
                "Yes, continue", "Cancel"))
                return;
        }

        SetupParticleSpawner();
        SetupCameraShake();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Game scene updated with polish effects!");
    }

    private static void SetupParticleSpawner()
    {
        var existing = Object.FindObjectOfType<ParticleSpawner>();
        if (existing != null)
        {
            Debug.Log("ParticleSpawner already exists.");
            return;
        }

        var go = new GameObject("ParticleSpawner");
        go.AddComponent<ParticleSpawner>();
    }

    private static void SetupCameraShake()
    {
        var cam = Camera.main;
        Debug.Assert(cam != null, "Main Camera not found!");

        var existing = cam.GetComponent<CameraShake>();
        if (existing != null)
        {
            Debug.Log("CameraShake already exists on camera.");
            return;
        }

        cam.gameObject.AddComponent<CameraShake>();
    }
}
