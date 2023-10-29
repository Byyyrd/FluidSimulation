using TMPro;
using UnityEngine;


public class DensityCalculator : MonoBehaviour
{
    [SerializeField] private CalculateParticals cp;
    [SerializeField] private float smoothingRadius;
    private Particle[] particles;
    private Vector2 position = Vector2.zero;
    private Drawing graphics;
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    // Start is called before the first frame update
    void Start()
    {
        graphics = Drawing.Instance;
        particles = new Particle[0];
    }

    // Update is called once per frame
    void Update()
    {
        
        particles = cp.particles;
        if (Input.GetMouseButtonDown(0))
        {
            position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        _textMeshPro.text = $"Density: {CalculateDensity(position)}";

        graphics.DrawCircle(position.x,position.y,smoothingRadius,new Color(1,0,0,.5f));
    }
    public float SmoothingFunction(float distance,float radius)
    {
        if(distance < radius)
        {
            return Mathf.Pow((distance-radius), 2)/Mathf.Pow(radius,2);
        }
        return 0;

    }
    public float SmootingFunctionVolume(float radius)
    {
        return (Mathf.PI * Mathf.Pow(radius,2)) / 6;
    }
    private float CalculateDensity(Vector2 position)
    {
        float density = 0;
        float mass = 1;
        float volume = SmootingFunctionVolume(smoothingRadius);
        foreach (Particle particle in particles)
        {
            if(particle != null)
            {
                float dist = (position - particle.position).magnitude;
                float influence = SmoothingFunction(dist, smoothingRadius);
                density += influence * mass / volume;
            }
            
        }
        return density;
    }
    
}
