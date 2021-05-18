using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PerlinNoise : MonoBehaviour
{
    public GameObject infoPlane;
    public GameObject xoff1Ball;
    public GameObject xoff2Ball;
    public GameObject xoff3Ball;
    TextMeshPro tmProText;
    float xOffTemp1;
    float xOffTemp2;
    float xOffTemp3;
    public float xOff1Total;
    public float xOff2Total;
    public float xOff3Total;
    
    float y;
    float perlinNoise = 0.0f;
    float perlinNoise1 = 0.0f;
    float perlinNoise2 = 0.0f;
    float perlinNoise3 = 0.0f;
    LineRenderer lineRendererAll;
    public LineRenderer lineRendererOctave1;
    public LineRenderer lineRendererOctave2;
    public LineRenderer lineRendererOctave3;
    public int lineRendererPointsCount = 200;
    float xOff = 0.0f;
    float xoff1;
    float xoff2;
    float xoff3;
    //xOffIncrement is the stepping frequency through the 'array' of perlin noise values.
    public float perlinNoiseFrequency = 0.02f;
    [Range(-0.1f, 0.1f)]
    public float perlinNoise1Frequency;
    [Range(-0.1f, 0.1f)]
    public float perlinNoise2Frequency;
    [Range(-0.1f, 0.1f)]
    public float perlinNoise3Frequency;

    [Range(0.0f, 6.0f)]
    public float perlinNoise1Amplitude;
    [Range(0.0f, 6.0f)]
    public float perlinNoise2Amplitude;
    [Range(0.0f, 6.0f)]
    public float perlinNoise3Amplitude;
    int pos = 0;

    float perlinNoiseWaveLength = 10f;
    float yPos;
    void Start()
    {
        lineRendererAll = transform.GetComponent<LineRenderer>();
        lineRendererAll.positionCount = lineRendererPointsCount;
        lineRendererOctave1.positionCount = lineRendererPointsCount;
        lineRendererOctave2.positionCount = lineRendererPointsCount;
        lineRendererOctave3.positionCount = lineRendererPointsCount;

        tmProText = infoPlane.GetComponent<TextMeshPro>();
    }

    

    void FixedUpdate()
    {
        //xOffTemp is used within for loop to iterate through all points in linerenderer 
        xOffTemp1 = 0.0f;
        xOffTemp2 = 45.0f;
        xOffTemp3 = -65.0f;
        for (int i = 0; i < 200; i++)
        {
            perlinNoise1 = Mathf.PerlinNoise(xoff1 + xOffTemp1, xoff1 + xOffTemp1);
            perlinNoise2 = Mathf.PerlinNoise(xoff2 + xOffTemp2, xoff2 + xOffTemp2);
            perlinNoise3 = Mathf.PerlinNoise(xoff3 + xOffTemp3, xoff3 + xOffTemp3);
            //yPos = map(perlinNoise, 0.0f, 1.0f, 0.0f, 10.0f) + map(Mathf.Sin(xoff  + xOffTemp), -1.0f, 1.0f, 0.0f, 10.0f);
            yPos = map(perlinNoise1, 0.0f, 1.0f, -perlinNoise1Amplitude, perlinNoise1Amplitude)
                 + map(perlinNoise2, 0.0f, 1.0f, -perlinNoise2Amplitude, perlinNoise2Amplitude)
                 + map(perlinNoise3, 0.0f, 1.0f, -perlinNoise3Amplitude, perlinNoise3Amplitude);
            
            lineRendererAll.SetPosition(i, new Vector3(0, yPos, i/perlinNoiseWaveLength));
            lineRendererOctave1.SetPosition(i, new Vector3(0, map(perlinNoise1, 0.0f, 1.0f, -perlinNoise1Amplitude, perlinNoise1Amplitude), i/perlinNoiseWaveLength));
            lineRendererOctave2.SetPosition(i, new Vector3(0, map(perlinNoise2, 0.0f, 1.0f, -perlinNoise2Amplitude, perlinNoise2Amplitude), i/perlinNoiseWaveLength));
            lineRendererOctave3.SetPosition(i, new Vector3(0, map(perlinNoise3, 0.0f, 1.0f, -perlinNoise3Amplitude, perlinNoise3Amplitude), i/perlinNoiseWaveLength));
            xOffTemp1 += perlinNoise1Frequency;
            xOffTemp2 += perlinNoise2Frequency;
            xOffTemp3 += perlinNoise3Frequency;
        }


        perlinNoise = Mathf.PerlinNoise(xoff1, xoff1);
        yPos = map(perlinNoise, 0.0f, 1.0f, -6, 6);

        xoff1Ball.transform.position = new Vector3(0, 14, map(xoff1 + xOffTemp1, -500f, 500f, 0f, 20f));
        xoff2Ball.transform.position = new Vector3(0, 13, map(xoff2 + xOffTemp2, -500f, 500f, 0f, 20f));
        xoff3Ball.transform.position = new Vector3(0, 12, map(xoff3 + xOffTemp3, -500f, 500f, 0f, 20f));

        xOff1Total = xoff1 + xOffTemp1;
        xOff2Total = xoff2 + xOffTemp2;
        xOff3Total = xoff3 + xOffTemp3;

        xoff1 += perlinNoise1Frequency;
        xoff2 += perlinNoise2Frequency;
        xoff3 += perlinNoise3Frequency;


        
        tmProText.text =    "OCTAVE 1 \n   Frequency\t\t" + perlinNoise1Frequency.ToString("0.000") + "\n   Amplitude\t\t" + perlinNoise1Amplitude.ToString("0.000") + "\n   xoff\t\t" + xOff1Total.ToString("000.0") + "\n\n" + 
                            "OCTAVE 2 \n   Frequency\t\t" + perlinNoise2Frequency.ToString("0.000") + "\n   Amplitude\t\t" + perlinNoise2Amplitude.ToString("0.000") + "\n   xoff\t\t" + xOff2Total.ToString("000.0") + "\n\n" + 
                            "OCTAVE 3 \n   Frequency\t\t" + perlinNoise3Frequency.ToString("0.000") + "\n   Amplitude\t\t" + perlinNoise3Amplitude.ToString("0.000") + "\n   xoff\t\t" + xOff3Total.ToString("000.0");
    }



    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s-a1)*(b2-b1)/(a2-a1);
    }
}
