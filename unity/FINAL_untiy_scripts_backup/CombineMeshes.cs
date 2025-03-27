using UnityEngine;

public class CombineMeshes : MonoBehaviour
{
    [ContextMenu("Combine Meshes")]
    void Combine()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine, true, true);

        gameObject.AddComponent<MeshFilter>().mesh = combinedMesh;
        gameObject.AddComponent<MeshRenderer>().material = meshFilters[0].GetComponent<MeshRenderer>().sharedMaterial;

        // 禁用子物件
        foreach (var mf in meshFilters)
        {
            mf.gameObject.SetActive(false);
        }

        Debug.Log("Meshes combined into one.");
    }
}
