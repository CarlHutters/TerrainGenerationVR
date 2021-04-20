using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    public GameObject tree;
    private GameObject treeInstance;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            tree.gameObject.transform.position = new Vector3(UnityEngine.Random.Range(0, 100), UnityEngine.Random.Range(0, 10), UnityEngine.Random.Range(0, 100));
            treeInstance = Instantiate(tree);
            treeInstance.transform.SetParent(gameObject.transform);
        }
    }
}
