using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;


public class render : MonoBehaviour
{
    [SerializeField] private Shader shader;
    [Range(0f,10f)]
    [SerializeField] private float radius;
    private Material material;
    private Circle circle;
    public List<Circle> allCircles;
    public void Start()
    {
        material = new(shader);
        allCircles = new()
        {
            new Circle(0, 0, radius)
        };

    }
    public class Circle
    {
        public float x, y;
        public float radius;
        public Circle(float x,float y, float radius)
        {
            this.x = x;
            this.y = y;
            this.radius = radius;
        }
    }
    public void OnRenderObject()
    {
        allCircles[0].radius = radius;
        
        material.SetPass(0);
        GL.Begin(GL.QUADS);
        GL.Color(Color.red);
        foreach (Circle circle in allCircles)
        {
            material.SetFloat("radius", radius);
            material.SetVector("center", new Vector4(circle.x, circle.y));
            GL.Vertex3(circle.x - radius, circle.y - radius, 0);
            GL.Vertex3(circle.x - radius, circle.y + radius, 0);
            GL.Vertex3(circle.x + radius, circle.y + radius, 0);
            GL.Vertex3(circle.x + radius, circle.y - radius, 0);
        }
        GL.End();
    }
}
