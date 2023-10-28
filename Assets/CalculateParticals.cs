using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateParticals : MonoBehaviour
{

    [Range(0, 1023)]
    [SerializeField] private int population;
    [SerializeField] private float radius;

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
        CreateParticles();
        for (int i = 0; i < particleIndex; i++)
        {
            Particle particle = particles[i];
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
}
