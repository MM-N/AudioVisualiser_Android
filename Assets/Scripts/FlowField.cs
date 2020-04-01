using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField : MonoBehaviour
{
    FastNoise _fastNoise;

    //** GRID **//
    [SerializeField, GetSet("GridSize")]
    private Vector3Int _gridSize;
    public Vector3Int GridSize
    {
        get { return _gridSize; }
        set { _gridSize = value; }
    }

    [SerializeField, GetSet("Increment")]
    private float _increment;
    public float Increment
    {
        get { return _increment; }
        set { _increment = value; }
    }

    [SerializeField, GetSet("Offset")]
    private Vector3 _offset;
    public Vector3 Offset
    {
        get { return _offset; }
        set { _offset = value; }
    }

    [SerializeField, GetSet("OffsetSpeed")]
    private Vector3 _offsetSpeed;
    public Vector3 OffsetSpeed
    {
        get { return _offsetSpeed; }
        set { _offsetSpeed = value; }
    }

    [SerializeField, GetSet("CellSize")]
    private float _cellSize;
    public float CellSize
    {
        get { return _cellSize; }
        set { _cellSize = value; }
    }

    Vector3[,,] _flowFieldDirection;

    //** PARTICLES **//
    public GameObject particlePrefab;

    List<FlowFieldParticle> particles;

    [SerializeField, GetSet("NumberOfParticles")]
    private int _numberOfParticles;
    public int NumberOfParticles
    {
        get { return _numberOfParticles; }
        set { _numberOfParticles = value; }
    }

    [SerializeField, GetSet("ParticleSize")]
    private float _particleSize;
    public float ParticleSize
    {
        get { return _particleSize; }
        set { _particleSize = value; }
    }

    [SerializeField, GetSet("ParticleSpeed")]
    private float _particleSpeed;
    public float ParticleSpeed
    {
        get { return _particleSpeed; }
        set { _particleSpeed = value; }
    }

    [SerializeField, GetSet("ParticleRotSpeed")]
    private float _particleRotSpeed;
    public float ParticleRotSpeed
    {
        get { return _particleRotSpeed; }
        set { _particleRotSpeed = value; }
    }

    public float SpawnRadius;

    // Check if position of particle is inside radius of spawned particle, false if so
    bool ParticleSpawnValid(Vector3 position)
    {
        foreach (FlowFieldParticle particle in particles)
        {
            if ((Vector3.Distance(position, particle.transform.position) < SpawnRadius))
            {
                return false;
            }
        }

        return true;
    }


    // Start is called before the first frame update
    void Start()
    {
        _flowFieldDirection = new Vector3[GridSize.x, GridSize.y, GridSize.z];
        _fastNoise = new FastNoise();

        particles = new List<FlowFieldParticle>();

        SpawnParticles();

    }

    void SpawnParticles()
    {
        for (int i = 0; i < NumberOfParticles; i++)
        {
            //Keeping track of spawned particle valid positions
            int attempt = 0;

            while (attempt < 100)
            {
                Vector3 randomPos = new Vector3(
                Random.Range(this.transform.position.x, this.transform.position.x + GridSize.x * CellSize),
                Random.Range(this.transform.position.y, this.transform.position.y + GridSize.y * CellSize),
                Random.Range(this.transform.position.z, this.transform.position.z + GridSize.z * CellSize)
                );

                // May not need, just put directly in if statement
                bool isValid = ParticleSpawnValid(randomPos);

                if (isValid)
                {
                    GameObject particleInstance = (GameObject)Instantiate(particlePrefab);
                    particleInstance.transform.position = randomPos;
                    particleInstance.transform.parent = this.transform;
                    particleInstance.transform.localScale = new Vector3(ParticleSize, ParticleSize, ParticleSize);
                    particles.Add(particleInstance.GetComponent<FlowFieldParticle>());
                    break;
                }
                else 
                {
                    attempt++;
                }
            }

            //Debug.Log("Instantiated particles: " + particles.Count);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateFlowFieldDirections();
        ControlParticleMovement();
    }

    void CalculateFlowFieldDirections()
    {
        //** Creating the flow grid **//

        float xOff = 0.0f;
        for (int x = 0; x < GridSize.x; x++)
        {
            float yOff = 0.0f;
            for (int y = 0; y < GridSize.y; y++)
            {
                float zOff = 0.0f;
                for (int z = 0; z < GridSize.z; z++)
                {
                    //3D Grid, GetSimplex returns [0, 1], + 1 to get [1, 2]
                    float noise = _fastNoise.GetSimplex(xOff + Offset.x, yOff + Offset.y, zOff + Offset.z) + 1;

                    //** Get direction of noise **//
                    Vector3 noiseDirection = new Vector3(Mathf.Cos(noise * Mathf.PI), Mathf.Sin(noise * Mathf.PI), Mathf.Cos(noise * Mathf.PI));

                    //** Array of Noise Direction Vectors **//
                    _flowFieldDirection[x, y, z] = Vector3.Normalize(noiseDirection);

                    xOff += Increment;
                }
                yOff += Increment;
            }
            xOff += Increment;
        }
    }

    void ControlParticleMovement()
    {
        foreach (FlowFieldParticle p in particles)
        {

            //Check if particles have hit edge of the grid, if so move it other side
            // X EDGES
            if (p.transform.position.x > this.transform.position.x + (GridSize.x * CellSize))
            {
                p.transform.position = new Vector3(this.transform.position.x, p.transform.position.y, p.transform.position.z);
            }
            if (p.transform.position.x < this.transform.position.x)
            {
                p.transform.position = new Vector3(
                    this.transform.position.x + (GridSize.x * CellSize),
                    this.transform.position.y,
                    this.transform.position.z);
            }

            // Y EDGES
            if (p.transform.position.y > this.transform.position.y + (GridSize.y * CellSize))
            {
                p.transform.position = new Vector3(this.transform.position.x, p.transform.position.y, p.transform.position.z);
            }
            if (p.transform.position.y < this.transform.position.y)
            {
                p.transform.position = new Vector3(
                    this.transform.position.x,
                    this.transform.position.y + (GridSize.y * CellSize),
                    this.transform.position.z);
            }

            //Z EDGES
            if (p.transform.position.z > this.transform.position.z + (GridSize.z * CellSize))
            {
                p.transform.position = new Vector3(this.transform.position.x, p.transform.position.y, p.transform.position.z);
            }
            if (p.transform.position.z < this.transform.position.z)
            {
                p.transform.position = new Vector3(
                    this.transform.position.x,
                    this.transform.position.y,
                    this.transform.position.z + (GridSize.z * CellSize));
            }

            //Find cell is particle in
            Vector3Int particlePos = new Vector3Int(
                Mathf.FloorToInt(Mathf.Clamp((p.transform.position.x - this.transform.position.x) / CellSize, 0.0f, GridSize.x - 1.0f)),
                Mathf.FloorToInt(Mathf.Clamp((p.transform.position.y - this.transform.position.y) / CellSize, 0.0f, GridSize.y - 1.0f)),
                Mathf.FloorToInt(Mathf.Clamp((p.transform.position.z - this.transform.position.z) / CellSize, 0.0f, GridSize.z - 1.0f))
            );
            p.ApplyRotation(_flowFieldDirection[particlePos.x, particlePos.y, particlePos.z], ParticleRotSpeed);
            p.MoveSpeed = ParticleSpeed;
            p.transform.localScale = new Vector3(ParticleSize, ParticleSize, ParticleSize);
        }
    }

    private void OnDrawGizmos()
    {
        //Display area of our vector flow field
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(this.transform.position + new Vector3
            ((GridSize.x * CellSize) * 0.5f, (GridSize.y * CellSize) * 0.5f, (GridSize.z * CellSize * 0.5f)),
            new Vector3(GridSize.x * CellSize, GridSize.y * CellSize, GridSize.z * CellSize));

    }
}
