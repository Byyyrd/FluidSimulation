using UnityEngine;

public class Particle
{
    public Vector2 position = Vector3.zero;
    public Vector2 velocity = Vector3.zero;
    public float density = 0;
    public Particle(Vector2 position, Vector2 velocity)
    {
        this.position = position;
        this.velocity = velocity;
    }
}