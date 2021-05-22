using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    //public Vector3Int gridSize;
    public float crownSizeRadius;
    public int minDistance = 2;
    public int maxDistance = 15;
    public float growSpeed = 0;

    private bool doneGrowing;

    // Branches
    public GameObject branchPrefab;
    private Dictionary<Vector3, GameObject> branches;

    public int trunkHeight = 40;
    public int branchLength = 2;

    // Leaves
    public GameObject leafPrefab;
    public int leafAmount;
    [HideInInspector]
    public List<Leaf> leaves;
    public float spawnRadius;

    GameObject currentBranch = null;

    bool leafSpawnValidation(Vector3 position)
    {
        bool valid = true;
        foreach (Leaf leaf in leaves)
        {
            if (Vector3.Distance(position, leaf.transform.position) < spawnRadius)
            {
                valid = false;
                break;
            }
        }
        if (valid)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Start()
    {
        GenerateCrown();
        GenerateTrunk();
    }

    private void GenerateCrown()
    {
        leaves = new List<Leaf>();

        for (int i = 0; i < leafAmount; i++)
        {
            int attempt = 0;

            while (attempt < 100)
            {
                Vector3 randomPosition = transform.position + Random.insideUnitSphere * crownSizeRadius;
                randomPosition.y = randomPosition.y + trunkHeight + crownSizeRadius;

                    //new Vector3(
                    //Random.Range(transform.position.x - (gridSize.x * crownSize) * 0.5f, transform.position.x + (gridSize.x * crownSize) * 0.5f),
                    //Random.Range(transform.position.y + (gridSize.y * crownSize) * 1.5f, transform.position.y + (gridSize.y * crownSize) * 2.5f),
                    //Random.Range(transform.position.z - (gridSize.z * crownSize) * 0.5f, transform.position.z + (gridSize.z * crownSize) * 0.5f));

                bool isValid = leafSpawnValidation(randomPosition);

                if (isValid)
                {
                    GameObject leafInstance = (GameObject)Instantiate(leafPrefab);
                    leafInstance.transform.position = randomPosition;
                    leafInstance.transform.parent = transform;
                    leaves.Add(leafInstance.GetComponent<Leaf>());
                    break;
                }
                if (!isValid)
                {
                    attempt++;
                }
            }
        }
    }

    private void GenerateTrunk()
    {
        branches = new Dictionary<Vector3, GameObject>();

        //branchPrefab.transform.position = transform.position;
        //branchPrefab.GetComponent<Branch>().growDirection = new Vector3(0, 1, 0);

        currentBranch = (GameObject)Instantiate(branchPrefab);
        //Branch currentBranch = new Branch();
        currentBranch.transform.parent = transform;
        currentBranch.transform.position = new Vector3(transform.position.x, transform.position.y + branchLength, transform.position.z);
        currentBranch.GetComponent<Branch>().growDirection = new Vector3(0, 1, 0);
        currentBranch.GetComponent<LineRenderer>().SetPosition(0, transform.position);
        currentBranch.GetComponent<LineRenderer>().SetPosition(1, currentBranch.transform.position);
        branches.Add(currentBranch.transform.position, currentBranch);

        if (Vector3.Distance(transform.position, currentBranch.transform.position) < trunkHeight)
        {
            StartCoroutine(FixedTrunkGrowth(growSpeed));
        }
    }

    private IEnumerator FixedTrunkGrowth(float speed)
    {
        yield return new WaitForSeconds(speed);
        GameObject trunk = (GameObject)Instantiate(branchPrefab);
        //Branch trunk = new Branch();
        trunk.transform.parent = transform;
        trunk.transform.position = new Vector3(currentBranch.transform.position.x, currentBranch.transform.position.y + branchLength, currentBranch.transform.position.z);
        trunk.GetComponent<Branch>().growDirection = new Vector3(0, 1, 0);
        trunk.GetComponent<LineRenderer>().SetPosition(0, currentBranch.transform.position);
        trunk.GetComponent<LineRenderer>().SetPosition(1, trunk.transform.position);
        branches.Add(trunk.transform.position, trunk);
        currentBranch = trunk;

        if (Vector3.Distance(transform.position, currentBranch.transform.position) < trunkHeight)
        {
            StartCoroutine(FixedTrunkGrowth(growSpeed));
        }
        else if (Vector3.Distance(transform.position, currentBranch.transform.position) >= trunkHeight)
        {
            StartCoroutine(FixedGrowth(growSpeed));
        }
    }

    private void Grow()
    {
        if (doneGrowing)
        {
            //Debug.Log(leaves.Count);
            
            for (int i = 0; i < leaves.Count; i++)
            {
                if (leaves[i] != null)
                    Destroy(leaves[i].gameObject);
            }

            return;
        }

        // If there are no leaves left, it is done growing
        if (leaves.Count == 0)
        {
            doneGrowing = true;
            return;
        }

        // Process leaves
        for (int i = 0; i < leaves.Count; i++)
        {
            bool leafRemoved = false;

            leaves[i].closestBranch = null;
            Vector3 direction = Vector3.zero;

            // Find the nearest branch for this leaf
            foreach (GameObject b in branches.Values)
            {
                direction = leaves[i].transform.position - b.transform.position;    // Direction to branch from this leaf
                float distance = Mathf.Round(direction.magnitude);                  // Distance to the branch from this leaf

                if (distance <= minDistance)                                        // If minimum leaf distance is reached, we remove it
                {
                    Destroy(leaves[i].gameObject);
                    leaves.Remove(leaves[i]);
                    i--;
                    leafRemoved = true;
                    break;
                }
                else if (distance <= maxDistance)                                   // If the branch is in range, determine if it is the nearest
                {
                    if (leaves[i].closestBranch == null)
                        leaves[i].closestBranch = b;
                    else if (Vector3.Distance(leaves[i].transform.position, leaves[i].closestBranch.transform.position) > distance)
                        leaves[i].closestBranch = b;
                }
            }

            // If the leaf was removed it will be skipped
            if (!leafRemoved)
            {
                if (leaves[i].closestBranch != null)
                {
                    Vector3 dir = leaves[i].transform.position - leaves[i].closestBranch.transform.position;
                    dir.Normalize();
                    leaves[i].closestBranch.GetComponent<Branch>().growDirection += dir;        // Add to grow direction of branch
                    leaves[i].closestBranch.GetComponent<Branch>().growCount++;
                }
            }
        }

        // Generate new branches
        HashSet<GameObject> newBranches = new HashSet<GameObject>();

        foreach (GameObject b in branches.Values)
        {
            if (b.GetComponent<Branch>().growCount > 0)                             // If at least one leaf is affecting the branch
            {
                Vector3 avgDirection = b.GetComponent<Branch>().growDirection / b.GetComponent<Branch>().growCount;
                avgDirection.Normalize();

                GameObject newBranch = (GameObject)Instantiate(branchPrefab);
                // Branch currentBranch = new Branch();
                newBranch.transform.parent = transform;
                newBranch.transform.position = b.transform.position + avgDirection * branchLength;
                newBranch.GetComponent<Branch>().growDirection = avgDirection;
                newBranch.GetComponent<LineRenderer>().SetPosition(0, b.transform.position);
                newBranch.GetComponent<LineRenderer>().SetPosition(1, newBranch.transform.position);
                newBranches.Add(newBranch);
                b.GetComponent<Branch>().Reset();
            }
        }

        // Add the new branches to the tree
        bool branchAdded = false;
        
        foreach (GameObject b in newBranches)
        {
            // Check if branch already exists
            GameObject existing;
            if (!branches.TryGetValue(b.transform.position, out existing))
            {
                branches.Add(b.transform.position, b);
                branchAdded = true;
            }
        }

        // If no branches were added it is done
        // Handles issues where leaves equal out each other, making branches grow without ever reaching the leaf
        if (!branchAdded)
            doneGrowing = true;
    }

    private IEnumerator FixedGrowth(float speed)
    {
        yield return new WaitForSeconds(speed);
        Grow();
        StartCoroutine(FixedGrowth(speed));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position + new Vector3(transform.position.x, transform.position.y + trunkHeight + crownSizeRadius, transform.position.z), crownSizeRadius);
        //Gizmos.DrawWireCube(transform.position + new Vector3(transform.position.x, (gridSize.y * crownSize) * 2.0f, transform.position.z), new Vector3(gridSize.x * crownSize, gridSize.y * crownSize, gridSize.z * crownSize));
    }
}
