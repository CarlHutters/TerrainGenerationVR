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

    private const string axiom = "X";

    private Stack<TransformInformation> transformStack;
    private Dictionary<char, string> rules;
    private string currentString = string.Empty;

    private void Start()
    {
        transformStack = new Stack<TransformInformation>();

        rules = new Dictionary<char, string>
        {
            {'X', "[F-[[X]+X]+F[+FX]-X]" },
            {'F', "FF" }
        };

        Generate();
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

        foreach(char c in currentString)
        {
            switch(c)
            {
                case 'F':
                    Vector3 initialPosition = transform.position;
                    transform.Translate(Vector3.up * length);

                    GameObject treeSegment = Instantiate(branch);
                    treeSegment.GetComponent<LineRenderer>().SetPosition(0, initialPosition);
                    treeSegment.GetComponent<LineRenderer>().SetPosition(1, transform.position);
                    break;

                case 'X':
                    break;

                case '+':
                    transform.Rotate(Vector3.back * angle);
                    break;

                case '-':
                    transform.Rotate(Vector3.forward * angle);
                    break;

                case '[':
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
                    break;

                default:
                    throw new InvalidOperationException("Invalid L-Tree operation");
            }
        }
    }
}
