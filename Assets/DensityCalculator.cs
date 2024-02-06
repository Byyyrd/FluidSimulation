using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;


public class DensityCalculator : MonoBehaviour
{
    [SerializeField] private CalculateParticals cp;
    [SerializeField] private float smoothingRadius;
    [SerializeField] public float targetDensity;
    [SerializeField] private float pressureForce;
    [SerializeField] private Vector2 externalForce;
    [SerializeField] private float viscosity;
    [SerializeField] private float speed;
    [SerializeField] private float nearDensityThreashhold = .2f;
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
        //Parallel.ForEach(particles, particle => {
        //    if (particle != null)
        //    {
        //        particle.density = CalculateDensity(particle.position);
        //    }
        //});
        particles = cp.particles;
        foreach (Particle particle in particles)
        {
            if (particle != null)
            {
                particle.density = CalculateDensity(particle.position);
            }
        }
        foreach(Particle particle in particles)
        {
            if (particle != null)
            {
                particle.velocity += speed * Time.deltaTime * CalculateAcceleration(particle);
            }
        }


        if (Input.GetMouseButtonDown(0))
        {
            position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        float density = CalculateDensity(position);
        _textMeshPro.text = $"Density: {density}";
        _textMeshPro.text += $"Gradient: {CalculateGradient(position)}";
        graphics.DrawCircle(position.x,position.y,smoothingRadius,new Color(1,0,0,.5f));
    }
    private Vector2 CalculateAcceleration(Particle particle)
    {
        return CalculateForceField(particle) / particle.density;
    }


    private Vector2 CalculateForceField(Particle particle)
    {
        return CalculatePressureGradient(particle) + externalForce + CalculateViscosity(particle);
    }
    private Vector2 CalculateViscosity(Particle particle)
    {
        Vector2 viscosityForce = Vector2.zero;
        const float mass = 1;
        foreach (Particle otherParticle in particles)
        {
            if (otherParticle == null || otherParticle == particle) continue;
            float dist = (otherParticle.position - particle.position).magnitude;
            float gradient2 = SmoothingFunctionDerivative2(dist, smoothingRadius);
            viscosityForce += gradient2 * mass * ((otherParticle.velocity - particle.velocity) / new Vector2(otherParticle.density, otherParticle.density));
        }
        return viscosity * viscosityForce;
    }
    private float CalculatePressure(Particle particle)
    {
        return (targetDensity - particle.density) * pressureForce;
    }


    private Vector2 CalculatePressureGradient(Particle particle)
    {
        const float mass = 1;
        Vector2 pressureGradient = Vector2.zero;
        foreach (Particle otherParticle in particles)
        {
            if (otherParticle == null || otherParticle == particle) continue;

            float dist = (otherParticle.position - particle.position).magnitude;
            Vector2 dir = (otherParticle.position - particle.position) / (dist + 0.0001f);
            float gradient;
			if (dist > nearDensityThreashhold)
            {
				gradient = Poly6Derivative(dist, smoothingRadius);
            }
            else
            {
                gradient = SpikeyDerivative(dist, smoothingRadius);
            }
			float otherPressure = CalculatePressure(otherParticle);
            float pressure = CalculatePressure(particle);
            pressureGradient += ((otherPressure + pressure) / 2 * particle.density) * gradient * mass * dir;
        }
        return -pressureGradient;
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
                float influence = 0;
                if(dist > nearDensityThreashhold)
                {
					influence = Poly6(dist, smoothingRadius);
                }
                else
                {
                    influence = Spikey(dist, smoothingRadius);
                }
                density += influence * mass / volume;
            }
            
        }
        return density;
    }
    private float CalculateFieldQuantity(Vector2 position)
    {
        float fieldQuantity = 0;
        float mass = 1;
        foreach (Particle particle in particles)
        {
            if (particle != null)
            {
                float dist = (position - particle.position).magnitude;
                float influence = 0;
                if (dist > .2)
				{
					influence = Poly6(dist, smoothingRadius);
				}
				else
				{
					influence = Spikey(dist, smoothingRadius);
				}
				fieldQuantity += influence * mass;
            }
        }
        return fieldQuantity;
    }
    private Vector2 CalculateGradient(Vector2 position)
    {
        Vector2 resultGradient = Vector2.zero;
        float mass = 1;
        foreach (Particle particle in particles)
        {
            if (particle != null)
            {
                float dist = (position - particle.position).magnitude;
                Vector2 dir = (position - particle.position) / (dist + 0.0001f);
                float gradient;
			if (dist > nearDensityThreashhold)
            {
				gradient = Poly6Derivative(dist, smoothingRadius);
            }
            else
            {
                gradient = SpikeyDerivative(dist, smoothingRadius);
            }
                float particleDensity = particle.density;
                resultGradient += CalculateFieldQuantity(particle.position) * dir * gradient * mass / (particleDensity+0.0001f);
            }
        }
        return resultGradient;
    }
    private void OnDrawGizmos()
    {
        //if (Application.isPlaying)
        //{
        //    foreach (Particle particle in particles)
        //    {
        //        if (particle != null)
        //        {
        //            particle.density = CalculateDensity(particle.position);
        //        }
        //    }
        //    foreach (Particle particle in particles)
        //    {
        //        if (particle != null)
        //        {
        //            Vector2 gradient = CalculateGradient(particle.position);
        //            Debug.Log(gradient);
        //            float densityOffset = targetDensity - CalculateDensity(particle.position);
        //            particle.velocity += gradient * densityOffset * pressureForce;
        //            Gizmos.DrawLine(particle.position, particle.position + gradient);
        //        }

        //    }
        //}

    }

    public float SmoothingFunction(float distance, float radius)
    {
        if (distance < radius)
        {
            return Mathf.Pow((distance - radius), 2) / Mathf.Pow(radius, 2);
        }
        return 0;

    }
    public float SmoothingFunctionDerivative(float distance, float radius)
    {
        if (distance < radius)
        {
            return (2 * (distance - radius)) / Mathf.Pow(radius, 2);
        }
        return 0;
    }
    public float SmoothingFunctionDerivative2(float distance, float radius)
    {
        if (distance < radius)
        {
            return 2 / Mathf.Pow(radius, 2);
        }
        return 0;
    }

    public float SmootingFunctionVolume(float radius)
    {
        return (Mathf.PI * Mathf.Pow(radius, 2)) / 6;
    }



    public float Poly6(float distance, float radius)
    {
        if (distance < radius)
        {
            float value =  Mathf.Pow(radius, 2) - Mathf.Pow(distance, 2);
            return Mathf.Pow(value,3) * (315/(64*Mathf.PI*Mathf.Pow(radius,9)));
        }
        return 0;
    }
    public float Poly6Derivative(float distance,float radius)
    {
        if (distance < radius)
        {
            float value = Mathf.Pow(radius, 2) - Mathf.Pow(distance, 2);
            return (-6*distance*Mathf.Pow(value, 2)) * (315 / (64 * Mathf.PI * Mathf.Pow(radius, 9)));
        }
        return 0;
    }

    public float Spikey(float distance,float radius)
    {
		return (15/(Mathf.PI * radius * 6)) * Mathf.Pow((radius - distance), 3);
	}

	public float SpikeyDerivative(float distance, float radius)
	{
		return -45 * Mathf.Pow((radius - distance), 2)/(Mathf.Pow(radius,6)*Mathf.PI);
	}
}
