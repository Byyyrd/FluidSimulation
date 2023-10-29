using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CalculateParticals : MonoBehaviour
{

    [Range(0, 1023)]
    [SerializeField] private int population;
    [SerializeField] private float radius;
    [SerializeField] private Vector2 boundsSize;
    [SerializeField] private float collisionDamping;
    [SerializeField] private float gravity;
    [Range(0.01f, 0.1f)]
    [SerializeField] private float collisionStrengh;
    [Range(1,10)]
    [SerializeField] private float effectRadius;
    [Range(1, 10)]
    [SerializeField] private float dampingRate;

    private Particle[] particles = new Particle[1023];
    private int particleIndex = 0;
    private Drawing graphics;
    
    public class Particle
    {
        public Vector3 position;
        public Vector3 velocity;
        public Particle(Vector3 position, Vector3 velocity)
        {
            this.position = position;
            this.velocity = velocity;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        graphics = Drawing.Instance;
        CreateParticles();
    }

    // Update is called once per frame
    void Update()
    {
        //CreateParticles();
        for (uint i = 0; i < particleIndex; i++)
        {
            Particle particle = particles[i];
            for (int j = 0; j < particleIndex; j++)
            {
                if(i != j)
                {
                    CalculateCollision(particle, particles[j]);
                }
            }
            particle.velocity += new Vector3(0,gravity);
            particle.velocity.x = Mathf.Lerp(particle.velocity.x, 0, Time.deltaTime * dampingRate);
            particle.velocity.y = Mathf.Lerp(particle.velocity.y, 0, Time.deltaTime * dampingRate);
            particle.position += particle.velocity * Time.deltaTime;
            HandleCollisions(i);
            graphics.DrawCircle(particle.position.x, particle.position.y, radius, Color.blue);
        }
    }

    private void CreateParticles()
    {
        Array.Clear(particles, 0, particleIndex);
        particleIndex = 0;
        int rowSize = Mathf.RoundToInt(Mathf.Sqrt(population));
        int rowIndex = 0;
        float xOrigin = -rowSize * radius;
        float yOrigin = rowSize * radius;
        for (int i = 0; i < population; i++)
        {
            if (i % rowSize == 0 && i != 0)
            {
                rowIndex++;
            }
            float y = yOrigin - rowIndex * radius * 2;
            float x = xOrigin + radius * 2 * i;
            float xOffset = rowSize * radius * 2 * rowIndex;
            particles[particleIndex++] = new(new(x - xOffset, y), Vector3.zero);
        }
    }

    private void CalculateCollision(Particle particle1,Particle particle2)
    {
        float distance = Vector3.Distance(particle1.position, particle2.position);
        if (distance < radius * effectRadius)
        {
            Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1), 0);

            float angle1 = -Mathf.Atan2(particle1.position.y - particle2.position.y,particle1.position.x - particle2.position.x);
            particle1.velocity.x -= Mathf.Cos(angle1) * collisionStrengh * CalculatePriority(distance);
            particle1.velocity.y -= Mathf.Sin(angle1) * collisionStrengh * CalculatePriority(distance);
            particle1.velocity += randomOffset;

            float angle2 = -Mathf.Atan2(particle2.position.y - particle1.position.y ,particle2.position.x - particle1.position.x );
            particle2.velocity.x -= Mathf.Cos(angle2) * collisionStrengh * CalculatePriority(distance);
            particle2.velocity.y -= Mathf.Sin(angle2) * collisionStrengh * CalculatePriority(distance);
            particle2.velocity -= randomOffset;
        }
    }

    private float CalculatePriority(float distance)
    {
        return (float) Math.Pow(Math.PI,(double) Math.Abs(distance * 5)) - 0.5f;
    }
    
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(Vector2.zero, boundsSize);
    }
    void HandleCollisions(uint particleIndex)
    {
        Particle particle = particles[particleIndex];
        Vector2 vel = particle.velocity;
        Vector2 pos = particle.position;
        pos.y -= radius / 2;

        // Keep particle inside bounds
        Vector2 halfSize = Vector2.Scale(boundsSize, new(0.5f,0.5f));
        Vector2 edgeDst = halfSize - new Vector2(Mathf.Abs(pos.x),Mathf.Abs(pos.y));

        if (edgeDst.x <= 0)
        {
            pos.x = halfSize.x * Mathf.Sign(pos.x);
            vel.x *= -1 * collisionDamping;
        }
        if (edgeDst.y <= 0)
        {
            pos.y = halfSize.y * Mathf.Sign(pos.y);
            vel.y *= -1 * collisionDamping;
        }


        pos.y += radius / 2;
        // Update position and velocity
        particle.position = pos;
        particle.velocity = vel;
    }

}
