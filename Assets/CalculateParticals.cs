using System;
using UnityEngine;

public class CalculateParticals : MonoBehaviour
{

    [Range(0, 1023)]
    [SerializeField] private int population;
    [SerializeField] private float radius;
    [SerializeField] private Vector2 boundsSize;
    [SerializeField] private float collisionDamping;
    [SerializeField] private float gravity;
    [SerializeField] private DensityCalculator dc;
    [SerializeField] private float massEarth = (float)(5.972 * Mathf.Pow(10,24));
    [SerializeField] private float earthRadius = (float)(6371000);
    [SerializeField] private float gravitationalConstant = (float) (6.6743 * Mathf.Pow(10,-11));

    public Particle[] particles = new Particle[1023];
    private int particleIndex = 0;
    private Drawing graphics;
    
    // Start is called before the first frame update
    void Start()
    {
        graphics = Drawing.Instance;
        CreateRandomParticles();
        gravity = (float)((massEarth * gravitationalConstant) / Math.Pow(earthRadius,2));
    }

    // Update is called once per frame
    void Update()
    {
        //CreateParticles();
        for (uint i = 0; i < particleIndex; i++)
        {
            Particle particle = particles[i];
//<<<<<<< Updated upstream
            //particle.velocity += new Vector2(0,gravity * Time.deltaTime);
//=======
            particle.velocity += new Vector2(0,-gravity * Time.deltaTime);
//>>>>>>> Stashed changes
            particle.position += particle.velocity * Time.deltaTime;
            HandleCollisions(i);
            Color color = Color.white;
            if (particle.density > dc.targetDensity)
                color = Color.red;
            if (particle.density < dc.targetDensity)
                color = Color.blue;
            graphics.DrawCircle(particle.position.x, particle.position.y, radius,color);
        }
    }
    private void CreateRandomParticles()
    {
        Array.Clear(particles, 0, particleIndex);
        for (int i = 0; i < population; i++)
        {
            particles[particleIndex++] = new(new(UnityEngine.Random.Range(-boundsSize.x / 2, boundsSize.x/2), UnityEngine.Random.Range(-boundsSize.y / 2, boundsSize.y / 2)), Vector2.zero);
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
        if (distance < radius)
        {
            float angle1 = -Mathf.Atan2(particle1.position.y - particle2.position.y,particle1.position.x - particle2.position.x);
            particle1.velocity.x += Mathf.Cos(angle1);
            particle1.velocity.y += Mathf.Sin(angle1);

            float angle2 = -Mathf.Atan2( particle2.position.y - particle1.position.y , particle2.position.x - particle1.position.x ); ;
            particle1.velocity.x += Mathf.Cos(angle2);
            particle1.velocity.y += Mathf.Sin(angle2);
        }

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
        particle.position = pos ;
        particle.velocity = vel;
    }

}
