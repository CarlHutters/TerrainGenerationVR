using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseFlowField : MonoBehaviour
{
    FastNoise fastNoise;
    public Vector3Int gridSize;
    public float cellSize;
    public Vector3[,,] flowFieldDirection;
    public float increment;
    public Vector3 offset;
    public Vector3 offsetSpeed;

    // Particles
    public GameObject particlePrefab;
    public int particleAmount;
    [HideInInspector]
    public List<FlowFieldParticle> particles;
    public float particleScale;
    public float spawnRadius;
    public float particleMoveSpeed;
    public float particleRotateSpeed;

    [SerializeField] private GameObject flowDirection;
    private GameObject flow;

    bool particleSpawnValidation(Vector3 position)
    {
        bool valid = true;
        foreach (FlowFieldParticle particle in particles)
        {
            if (Vector3.Distance(position, particle.transform.position) < spawnRadius)
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
        flowFieldDirection = new Vector3[gridSize.x, gridSize.y, gridSize.z];
        fastNoise = new FastNoise();
        particles = new List<FlowFieldParticle>();
        
        for (int i = 0; i < particleAmount; i++)
        {
            int attempt = 0;

            while(attempt < 100)
            {
                Vector3 randomPosition = new Vector3(
                Random.Range(transform.position.x, transform.position.x + gridSize.x * cellSize),
                Random.Range(transform.position.y, transform.position.y + gridSize.y * cellSize),
                Random.Range(transform.position.z, transform.position.z + gridSize.z * cellSize));

                bool isValid = particleSpawnValidation(randomPosition);

                if (isValid)
                {
                    GameObject particleInstance = (GameObject)Instantiate(particlePrefab);
                    particleInstance.transform.position = randomPosition;
                    particleInstance.transform.parent = transform;
                    particleInstance.transform.localScale = new Vector3(particleScale, particleScale, particleScale);
                    particles.Add(particleInstance.GetComponent<FlowFieldParticle>());
                    break;
                }
                if (!isValid)
                {
                    attempt++;
                }
            }
        }
    }

    private void Update()
    {
        CalculateFlowFieldDirections();
        ParticleBehaviour();
    }

    private void CalculateFlowFieldDirections()
    {
        offset = new Vector3(offset.x + (offsetSpeed.x * Time.deltaTime), offset.y + (offsetSpeed.y * Time.deltaTime), offset.z + (offsetSpeed.z * Time.deltaTime));

        float xOff = 0.0f;
        for (int x = 0; x < gridSize.x; x++)
        {
            float yOff = 0.0f;
            for (int y = 0; y < gridSize.y; y++)
            {
                float zOff = 0.0f;
                for (int z = 0; z < gridSize.z; z++)
                {
                    float noise = fastNoise.GetSimplex(xOff + offset.x, yOff + offset.y, zOff + offset.z) + 1;
                    Vector3 noiseDirection = new Vector3(Mathf.Cos(noise * Mathf.PI), Mathf.Sin(noise * Mathf.PI), Mathf.Cos(noise * Mathf.PI));
                    flowFieldDirection[x, y, z] = Vector3.Normalize(noiseDirection);

                    //flow = Instantiate(flowDirection);
                    //flow.GetComponent<LineRenderer>().SetPosition(0, new Vector3(x, y, z) + transform.position);
                    //flow.GetComponent<LineRenderer>().SetPosition(1, new Vector3(x, y, z) + transform.position + Vector3.Normalize(noiseDirection));

                    zOff += increment;
                }
                yOff += increment;
            }
            xOff += increment;
        }
    }

    private void ParticleBehaviour()
    {
        foreach(FlowFieldParticle particle in particles)
        {
            // Check edges on x
            if (particle.transform.position.x > transform.position.x + (gridSize.x * cellSize))
            {
                particle.transform.position = new Vector3(transform.position.x, particle.transform.position.y, particle.transform.position.z);
            }
            if (particle.transform.position.x < transform.position.x)
            {
                particle.transform.position = new Vector3(transform.position.x + (gridSize.x * cellSize), particle.transform.position.y, particle.transform.position.z);
            }

            // Check edges on y
            if (particle.transform.position.y > transform.position.y + (gridSize.y * cellSize))
            {
                particle.transform.position = new Vector3(particle.transform.position.x, transform.position.y, particle.transform.position.z);
            }
            if (particle.transform.position.y < transform.position.y)
            {
                particle.transform.position = new Vector3(particle.transform.position.x, transform.position.y + (gridSize.y * cellSize), particle.transform.position.z);
            }

            // Check edges on z
            if (particle.transform.position.z > transform.position.z + (gridSize.z * cellSize))
            {
                particle.transform.position = new Vector3(particle.transform.position.x, particle.transform.position.y, transform.position.z);
            }
            if (particle.transform.position.z < transform.position.z)
            {
                particle.transform.position = new Vector3(particle.transform.position.x, particle.transform.position.y, transform.position.z + (gridSize.z * cellSize));
            }

            Vector3Int particlePosition = new Vector3Int(
                Mathf.FloorToInt(Mathf.Clamp((particle.transform.position.x - transform.position.x) / cellSize, 0, gridSize.x - 1)),
                Mathf.FloorToInt(Mathf.Clamp((particle.transform.position.y - transform.position.y) / cellSize, 0, gridSize.y - 1)),
                Mathf.FloorToInt(Mathf.Clamp((particle.transform.position.z - transform.position.z) / cellSize, 0, gridSize.z - 1)));

            particle.ApplyRotation(flowFieldDirection[particlePosition.x, particlePosition.y, particlePosition.z], particleRotateSpeed);
            particle.moveSpeed = particleMoveSpeed;
            particle.transform.localScale = new Vector3(particleScale, particleScale, particleScale);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position + new Vector3((gridSize.x * cellSize) * 0.5f, (gridSize.y * cellSize) * 0.5f, (gridSize.z * cellSize) * 0.5f),
        new Vector3(gridSize.x * cellSize, gridSize.y * cellSize, gridSize.z * cellSize));
    }

}
