using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Renderer))]

public class Cube : MonoBehaviour
{
    public float SplitChance { get; private set; }
    public float ExplosionRadius { get; private set; }
    public float ExplosionPower { get; private set; }

    public event Action<Cube> Click;

    private void OnMouseDown()
    {
        Click?.Invoke(this);
        Destroy(gameObject);
    }

    public void Init(Vector3 position, Vector3 scale, float splitChance)
    {
        float baseExplosionRadius = 10f;
        float baseExplosionPower = 1000f;

        transform.position = position;
        transform.localScale = scale;
        GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV();
        SplitChance = splitChance;
        ExplosionRadius = baseExplosionRadius / transform.localScale.x;
        ExplosionPower = baseExplosionPower / transform.localScale.x;
    }
}