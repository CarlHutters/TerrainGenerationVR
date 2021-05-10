using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;

public class TransformInformation
{
    public Vector3 position;
    public Quaternion rotation;
}

public class TreeGenerator : MonoBehaviour
{
    [SerializeField] private int iterations = 4;
    [SerializeField] private GameObject branch;
    [SerializeField] private float length = 10f;
    [SerializeField] private float angle = 30f;
    public float interval;

    private bool treeGenerated;

    //private GameObject treeSegment;
    private TubeRenderer tube;
    private bool newBranch;

    private GameObject treeSegment;
    private List<Vector3> position = new List<Vector3>();

    private const string axiom = "X";

    private Stack<TransformInformation> transformStack;
    private Dictionary<char, string> rules;
    private string currentString = string.Empty;

    private void Start()
    {
        //for (int i = 0; i < 6; i++)
        //{
        //    Generate();
        //    transform.Rotate(0.0f, 30.0f, 0.0f, Space.World);
        //    iterations = UnityEngine.Random.Range(3, 5);
        //    angle = UnityEngine.Random.Range(5, 36);
        //}

        //tube = treeSegment.GetComponent<TubeRenderer>();
        //tube.SetPositions(position.ToArray());
        //tube.material = mat;
        
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (treeGenerated)
            {
                GameObject[] branches = GameObject.FindGameObjectsWithTag("Branch");

                foreach (GameObject branch in branches)
                    GameObject.Destroy(branch);

                treeGenerated = false;
            }


            if (!treeGenerated)
            {
                transformStack = new Stack<TransformInformation>();

                int ruleset = UnityEngine.Random.Range(0, 3);

                if (ruleset == 0)
                {
                    rules = new Dictionary<char, string>
                    {
                    {'X', "[F[+X][-X]FX]" },
                    {'F', "FF" }
                    };
                }

                if (ruleset == 1)
                {
                    rules = new Dictionary<char, string>
                    {
                    { 'X', "[F-[[X]+X]+F[+FX]-X]" },
                    {'F', "FF" }
                    };
                }

                if (ruleset == 2)
                {
                    rules = new Dictionary<char, string>
                    {
                    { 'X', "[F[+X]F[-X]+X]" },
                    {'F', "FF" }
                    };
                }

                if (ruleset == 3)
                {
                    rules = new Dictionary<char, string>
                    {
                    { 'F', "F" },
                    {'F', "FF-[-F+F+F]+[+F-F-F]" }
                    };
                }

                Generate();
                
                treeGenerated = true;
            }
        }
    }

    private void Generate()
    {
        currentString = axiom;

        StringBuilder stringBuilder = new StringBuilder();

        for(int i = 0; i < iterations; i++)
        {
            foreach (char c in currentString)
            {
                stringBuilder.Append(rules.ContainsKey(c) ? rules[c] : c.ToString());
            }

            currentString = stringBuilder.ToString();
            stringBuilder = new StringBuilder();
        }

        StartCoroutine(GrowTree());
    }

    private IEnumerator GrowTree()
    {
        foreach (char c in currentString)
        {
            switch (c)
            {
                case 'F':
                    yield return new WaitForSeconds(interval);

                    Vector3 initialPosition = transform.position;
                    transform.Translate(Vector3.up * length);

                    treeSegment = Instantiate(branch);
                    treeSegment.GetComponent<LineRenderer>().SetPosition(0, initialPosition);
                    treeSegment.GetComponent<LineRenderer>().SetPosition(1, transform.position);

                    //if (newBranch && position.Count > 10)
                    //{
                    //    int x = 1;

                    //    while (position[position.Count - x] != transform.position && x < 10)
                    //    {
                    //        Debug.Log("Position[index - x] " + position[position.Count - x] + " Transform Position " + transform.position);
                    //        position.Add(position[position.Count - x]);
                    //        x += 2;
                    //    }
                    //    newBranch = false;
                    //}

                    //position.Add(transform.position);
                    //transform.Translate(Vector3.up * length);

                    //VectorLine.SetRay3D(Color.green, initialPosition, transform.up * 1);

                    //TubeRenderer tube = treeSegment.GetComponent<TubeRenderer>();
                    //tube.SetPositions(new Vector3[]{ Vector3.zero, new Vector3(5f, 5f, 0), Vector3.right * 20 });

                    //Mesh meshTree = new Mesh();

                    //treeSegment.AddComponent<MeshFilter>();
                    //treeSegment.GetComponent<LineRenderer>().BakeMesh(meshTree, true);
                    //treeSegment.GetComponent<MeshFilter>().sharedMesh = meshTree;
                    //treeSegment.AddComponent<MeshRenderer>();
                    //treeSegment.GetComponent<MeshRenderer>().sharedMaterial = mat;
                    //Destroy(treeSegment);
                    //Debug.Log("Hejsa");

                    treeSegment.transform.SetParent(gameObject.transform);
                    break;

                case 'X':
                    break;

                case '+':
                    transform.Rotate(Vector3.back * angle);
                    break;

                case '-':
                    transform.Rotate(Vector3.forward * angle);
                    break;

                case '/':
                    transform.Rotate(Vector3.left * angle);
                    break;

                case '[':
                    newBranch = true;
                    transformStack.Push(new TransformInformation()
                    {
                        position = transform.position,
                        rotation = transform.rotation
                    });
                    break;

                case ']':
                    TransformInformation ti = transformStack.Pop();
                    transform.position = ti.position;
                    transform.rotation = ti.rotation;
                    //VectorLine.SetRay3D(Color.green, transform.position, transform.up * 1);
                    break;

                default:
                    throw new InvalidOperationException("Invalid L-Tree operation");
            }
        }
    }
}
