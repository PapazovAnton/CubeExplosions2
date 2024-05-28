using UnityEngine;

[RequireComponent(typeof(Explosion))]

public class Spawner : MonoBehaviour
{
    [SerializeField] private Cube cubePrefab;
    [SerializeField] private int _minCubes = 2;
    [SerializeField] private int _maxCubes = 7;

    private Explosion _explosion;

    private void OnEnable()
    {
        Cube.Click += TrySpawnCubes;
    }

    private void OnDisable()
    {
        Cube.Click -= TrySpawnCubes;
    }

    private void Awake()
    {
        _explosion = GetComponent<Explosion>();
    }

    private void Start()
    {
        SpawnInitialCube();
    }

    private void SpawnInitialCube()
    {
        Vector3 scale = new Vector3(3, 3, 3);
        Vector3[] cubePositions = new Vector3[]
        {
            new Vector3(0f, 3f, -53f),
            new Vector3(-6f, 3f, -53f),
            new Vector3(6f, 3f, -53f)
        };

        float splitChance = 1f;

        foreach (Vector3 position in cubePositions)
        {
            Cube newCube = Instantiate(cubePrefab, position, Quaternion.identity);
            newCube.Init(position, scale, splitChance);
        }
    }

    private void TrySpawnCubes(Cube cube)
    {
        if (Random.value <= cube.SplitChance)
        {
            SpawnCubes(cube);
        } 
        else
        {
            GenerateExplosion(cube);
        }
    }

    private void SpawnCubes(Cube cube)
    {
        int explosionForce = 500;
        int explosionRadius = 45;
        int scaleReduce = 2;
        int splitReduce = 2;

        Vector3 position = cube.transform.position;
        Vector3 scale = cube.transform.localScale / scaleReduce;
        float newSplitChance = cube.SplitChance / splitReduce;

        int count = Random.Range(_minCubes, _maxCubes);

        for (int i = 0; i < count; i++)
        {
            Cube newCube = Instantiate(cubePrefab, position, Quaternion.identity);
            newCube.Init(position, scale, newSplitChance);

            if (newCube.TryGetComponent(out Rigidbody rb) && _explosion != null)
            {
                _explosion.ApplyExplosionForce(rb, position, explosionRadius, explosionForce);
            }
        }
    }

    private void GenerateExplosion(Cube cube)
    {
        Vector3 explosionPosition = cube.transform.position;
        float explosionRadius = cube.ExplosionRadius;
        float explosionPower = cube.ExplosionPower;
        float maxEffect = 1f;

        Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                float proximity = (explosionPosition - rb.transform.position).magnitude;
                float effect = maxEffect - (proximity / explosionRadius);
                _explosion.ApplyExplosionForce(rb, explosionPosition, explosionRadius, explosionPower * effect);
            }
        }
    }
}