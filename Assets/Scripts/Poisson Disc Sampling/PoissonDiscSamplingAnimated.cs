using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoissonDiscSamplingAnimated : MonoBehaviour
{
    float radius;
    int planeSizeX; //Processing width

    int planeSizeZ; //Processing height
    float cellSize; //Processing w


    float planeHeight = 1f;

    int k;
    Vector3[] gridArray; //Processing grid
    int gridColumns; //Processing cols
    int gridRows; //Processing rows
    Vector3 currentSample; //Processing pos

    List<Vector3> activeList = new List<Vector3>();
    int randomIndex;

    int sampleCandidateCol;
    int sampleCandidateRow;

    List<GameObject> spheres = new List<GameObject>();

    void Start()
    {
        //Assign initial values to variables
        radius = .5f;
        k = 30;
        planeSizeX = 10; //Processing width
        planeSizeZ = 10; //Processing height
        cellSize = radius / Mathf.Sqrt(2); // ------------- 2 because number of dimensions is n=2.



        // STEP 0
        gridColumns = Mathf.FloorToInt(planeSizeX / cellSize);
        gridRows = Mathf.FloorToInt(planeSizeZ / cellSize);
        gridArray = new Vector3[gridColumns * gridRows];
        //Debug.Log("gridColumns = " + gridColumns + " gridRows = " + gridRows);

        for (int i = 0; i < gridColumns * gridRows; i++)
        {
            gridArray[i] = new Vector3(46,46,46);
        }

        //STEP 1
        currentSample = new Vector3(Random.Range(0f, (float)planeSizeX), planeHeight, Random.Range(0f, (float)planeSizeZ));
        gridArray[Mathf.FloorToInt(currentSample.x) + Mathf.FloorToInt(currentSample.z) * gridColumns] = currentSample;
        activeList.Add(currentSample);
        //Debug.Log(activeList);        


        // Randomly distributed points on the plane
        // for (int i = 0; i < 1000; i++)
        // {
        //     GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //     sphere.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
        //     sphere.transform.position = new Vector3(Random.Range(0f, (float)planeWidth), planeHeight, Random.Range(0f, (float)planeHeight));
        // }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (activeList.Count > 0)
        {
            randomIndex = Random.Range(0, activeList.Count);
            Vector3 activeSamplePos = activeList[randomIndex];
            bool validCandidateFound = false;
            for (int pointsWithinAnnulus = 0; pointsWithinAnnulus < k; pointsWithinAnnulus++)
            {
                float angle = Random.value * Mathf.PI * 2;
                Vector3 dir = new Vector3(Mathf.Sin(angle), planeHeight, Mathf.Cos(angle));
                Vector3 sampleCandidate = new Vector3(activeSamplePos.x + dir.x * Random.Range(radius, 2*radius), planeHeight, activeSamplePos.z + dir.z * Random.Range(radius, 2*radius));
                
                sampleCandidateCol = Mathf.FloorToInt(sampleCandidate.x / cellSize);
                sampleCandidateRow = Mathf.FloorToInt(sampleCandidate.z / cellSize);
                
                //Check distance to every neighbor (in cells surrounding current sample candidate)
                bool validCandidate = true;
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        Vector3 neighbor = gridArray[(i+sampleCandidateCol) + (j+sampleCandidateRow) * gridColumns];
                        if(neighbor != new Vector3(46,46,46))
                        {
                            if(Vector3.Distance(sampleCandidate, activeSamplePos) < radius)
                            {
                                validCandidate = false;
                            }
                        }
                    }                    
                }


                if(validCandidate)
                {
                    validCandidateFound = true;
                    gridArray[sampleCandidateCol + sampleCandidateRow * gridColumns] = sampleCandidate;
                    activeList.Add(sampleCandidate);
                    spheres.Add(GameObject.CreatePrimitive(PrimitiveType.Sphere));

                    break;
                }

                if(!validCandidateFound)
                {
                    //activeList.RemoveAt(randomIndex);
                }

                

                //Visualize annulus around point:
                // GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                // sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                // sphere.transform.position = candidate;
            }
        }


        for (int i = 0; i < gridArray.Length; i++)
        {
            if(gridArray[i] != new Vector3(46,46,46))
            {
                spheres[i].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
                spheres[i].transform.position = new Vector3(gridArray[i].x, 1f, gridArray[i].z);
                // GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                // sphere.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
                // sphere.transform.position = new Vector3(gridArray[i].x, 1f, gridArray[i].z);
            }
        }

        for (int i = 0; i < activeList.Count; i++)
        {
            spheres[i].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
            spheres[i].transform.position = new Vector3(gridArray[i].x, 1f, gridArray[i].z);
        }        
    }
}
