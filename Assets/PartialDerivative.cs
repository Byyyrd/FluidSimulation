using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;

public class PartialDerivative : MonoBehaviour
{
    [SerializeField] private int population;
    [SerializeField] private float radius;
    [Range(0,5)]
    [SerializeField] private float speed;
    private Texture2D text;

    private Drawing graphics;
    private List<Particle> particles = new();
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
        SetupParticles();
    }
    private void SetupParticles()
    {
        particles.Clear();
        for (int i = 0; i < population; i++)
        {
            particles.Add(new(new(Random.Range(0,Camera.main.orthographicSize * 2),Random.Range(0, Camera.main.orthographicSize * 2)),Vector3.zero));
        }
    }
    // Update is called once per frame
    void Update()
    {
        foreach (Particle particle in particles)
        {
            particle.position.x += AbleitungX(particle.position.x, particle.position.y) * (speed/100);
            particle.position.y += AbleitungY(particle.position.x, particle.position.y) * (speed/100);
            graphics.DrawCircle(particle.position.x - Camera.main.orthographicSize, particle.position.y - Camera.main.orthographicSize, radius, Color.green);
        }   
    }

    float Funktion(float x,float y)
    {
        return Mathf.Cos(y - 3 + Mathf.Sin(x));
    }
    float AbleitungX(float x,float y)
    {
        return -Mathf.Sin(y - 3 + Mathf.Sin(x)) * Mathf.Cos(x);
    }
    float AbleitungY(float x, float y)
    {
        return -Mathf.Sin(y - 3 + Mathf.Sin(x)) * 1;
    }
}
