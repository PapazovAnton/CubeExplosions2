using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Explosion))]

public class Spawner : MonoBehaviour
{
    [SerializeField] private Cube cubePrefab;
    [SerializeField] private int _minCubes = 2;
    [SerializeField] private int _maxCubes = 7;

    private Explosion _explosion;
    private List<Cube> _cubes = new List<Cube>();

    private void OnEnable()
    {
        foreach (var cube in _cubes)
        {
            cube.Click += TrySpawnCubes;
        }
    }

    private void OnDisable()
    {
        foreach (var cube in _cubes)
        {
            cube.Click -= TrySpawnCubes;
        }
    }

    private void Start()
    {
        _explosion = GetComponent<Explosion>();
        SpawnInitialCube();
    }

    private void SpawnInitialCube()
    {
        Vector3 scale = new Vector3(3, 3, 3);
        Vector3[] cubePositions = new Vector3[]
        {
            new Vector3(0f, 3f, -40f),
            new Vector3(-6f, 3f, -40f),
            new Vector3(6f, 3f, -40f)
        };

        float splitChance = 1f;

        foreach (Vector3 position in cubePositions)
        {
            Cube newCube = Instantiate(cubePrefab, position, Quaternion.identity);
            newCube.Init(position, scale, splitChance);
            newCube.Click += TrySpawnCubes;
            _cubes.Add(newCube);
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

        cube.Click -= TrySpawnCubes;
        _cubes.Remove(cube);
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

            newCube.Click += TrySpawnCubes;
            _cubes.Add(newCube);

            if (newCube.TryGetComponent(out Rigidbody rigidbody) && _explosion != null)
            {
                _explosion.ApplyExplosionForce(rigidbody, position, explosionRadius, explosionForce);
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
            Rigidbody rigidbody = hit.GetComponent<Rigidbody>();

            if (rigidbody != null)
            {
                float proximity = (explosionPosition - rigidbody.transform.position).magnitude;
                float effect = maxEffect - (proximity / explosionRadius);
                _explosion.ApplyExplosionForce(rigidbody, explosionPosition, explosionRadius, explosionPower * effect);
            }
        }
    }
}