using UnityEngine;
using UnityEngine.SceneManagement;

public class MissingScriptFinder : MonoBehaviour
{
    [ContextMenu("Find Missing Scripts")]
    private void FindMissingScripts()
    {
        Debug.Log("Searching for Missing Scripts");
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject obj in rootObjects)
        {
            CheckForMissingScripts(obj);
        }
    }

    private void CheckForMissingScripts(GameObject obj)
    {
        Component[] components = obj.GetComponents<Component>();

        foreach (Component component in components)
        {
            if (component == null)
            {
                Debug.LogError("Missing script component found on GameObject: " + obj.name, obj);
            }
        }

        // Recursively check child objects
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            CheckForMissingScripts(obj.transform.GetChild(i).gameObject);
        }
    }
}
