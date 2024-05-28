using UnityEngine;

public class Explosion : MonoBehaviour
{
    public void ApplyExplosionForce(Rigidbody body, Vector3 explosionOrigin, float explosionRadius, float explosionPower)
    {
        body.AddExplosionForce(explosionPower, explosionOrigin, explosionRadius);
    }
}