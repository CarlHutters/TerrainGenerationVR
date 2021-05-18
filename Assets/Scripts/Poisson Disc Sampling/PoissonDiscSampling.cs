using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoissonDiscSampling : MonoBehaviour {

	[Range(0.1f, 1f)]
	public float radius = 1;
	private float tempRadius;
	public Vector2 regionSize = Vector2.one;
	private Vector2  tempRegionSize;
	public int rejectionSamples = 30;
	private int tempRejectionSamples;
	List<Vector2> points;
	List<GameObject> spheres = new List<GameObject>();

	public Material sphereCoreMaterial;
	public Material sphereShellMaterial;


	void Update()
	{
		if (Input.GetKeyDown("space") || tempRejectionSamples != rejectionSamples || tempRegionSize != regionSize || tempRadius != radius)
			callFunction();

		tempRejectionSamples = rejectionSamples;
		tempRegionSize = regionSize;
		tempRadius = radius;
	}

	public List<Vector2> GeneratePoints(float radius, Vector2 sampleRegionSize, int numSamplesBeforeRejection = 30) {
		float cellSize = radius/Mathf.Sqrt(2);

		int[,] grid = new int[Mathf.CeilToInt(sampleRegionSize.x/cellSize), Mathf.CeilToInt(sampleRegionSize.y/cellSize)];
		List<Vector2> points = new List<Vector2>();
		List<Vector2> spawnPoints = new List<Vector2>();

		spawnPoints.Add(sampleRegionSize/2);
		while (spawnPoints.Count > 0) {
			int spawnIndex = Random.Range(0,spawnPoints.Count);
			Vector2 spawnCentre = spawnPoints[spawnIndex];
			bool candidateAccepted = false;

			for (int i = 0; i < numSamplesBeforeRejection; i++)
			{
				float angle = Random.value * Mathf.PI * 2;
				Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
				Vector2 candidate = spawnCentre + dir * Random.Range(radius, 2*radius);
				if (IsValid(candidate, sampleRegionSize, cellSize, radius, points, grid)) {
					points.Add(candidate);
					spawnPoints.Add(candidate);
					grid[(int)(candidate.x/cellSize),(int)(candidate.y/cellSize)] = points.Count;
					candidateAccepted = true;
					break;
				}
			}
			if (!candidateAccepted) {
				spawnPoints.RemoveAt(spawnIndex);
			}

		}

		return points;
	}

	public bool IsValid(Vector2 candidate, Vector2 sampleRegionSize, float cellSize, float radius, List<Vector2> points, int[,] grid) {
		if (candidate.x >=0 && candidate.x < sampleRegionSize.x && candidate.y >= 0 && candidate.y < sampleRegionSize.y) {
			int cellX = (int)(candidate.x/cellSize);
			int cellY = (int)(candidate.y/cellSize);
			int searchStartX = Mathf.Max(0,cellX -2);
			int searchEndX = Mathf.Min(cellX+2,grid.GetLength(0)-1);
			int searchStartY = Mathf.Max(0,cellY -2);
			int searchEndY = Mathf.Min(cellY+2,grid.GetLength(1)-1);

			for (int x = searchStartX; x <= searchEndX; x++) {
				for (int y = searchStartY; y <= searchEndY; y++) {
					int pointIndex = grid[x,y]-1;
					if (pointIndex != -1) {
						float sqrDst = (candidate - points[pointIndex]).sqrMagnitude;
						if (sqrDst < radius*radius) {
							return false;
						}
					}
				}
			}
			return true;
		}
		return false;
	}

	void callFunction()
	{
		foreach (GameObject sphere in spheres)
		{
			Destroy(sphere);
		}

		points = GeneratePoints(radius, regionSize, rejectionSamples);

		if (points != null) {
			foreach (Vector2 point in points)
			{
				GameObject sphereShell = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				sphereShell.transform.localScale = new Vector3(radius,radius,radius);//radius/2,radius/2,radius/2);
				sphereShell.transform.position = point;
				sphereShell.GetComponent<Renderer>().material = sphereShellMaterial;
				spheres.Add(sphereShell);


				GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				sphere.GetComponent<Renderer>().material = sphereCoreMaterial;
				sphere.transform.localScale = new Vector3(.03f,.03f,.03f);//radius/2,radius/2,radius/2);
				sphere.transform.position = point;
				spheres.Add(sphere);
				
			}
		}
	}
}