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
    [SerializeField] private Gradient gradient;
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
            float dx = AbleitungX(particle.position.x, particle.position.y) ;
            float dy = AbleitungY(particle.position.x, particle.position.y) ;
            particle.position.x += dx * (speed / 100);
            particle.position.y += dy * (speed / 100);
            float avarageDelta = (Mathf.Abs(dx) + Mathf.Abs(dy)) / 2;
            Color color = gradient.Evaluate(avarageDelta);
            graphics.DrawCircle(particle.position.x - Camera.main.orthographicSize, particle.position.y - Camera.main.orthographicSize, radius, color);
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
