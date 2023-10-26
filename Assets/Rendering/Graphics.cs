using System;
using UnityEditor;
using UnityEngine;

public class Graphics : MonoBehaviour
{
    private static Graphics _Instance;
    public static Graphics Instance
    {
        get
        {
            if (!_Instance)
            {
                _Instance = new GameObject().AddComponent<Graphics>();
                _Instance.name = _Instance.GetType().ToString();
                DontDestroyOnLoad(_Instance.gameObject);
            }
            return _Instance;
        }
    }

    [SerializeField] private Material material;

    private Matrix4x4[] matrices = new Matrix4x4[1023];
    private int matrixIndex = 0;

    private Vector4[] colors = new Vector4[1023];
    private int colorIndex = 0;
    
    private MaterialPropertyBlock block;
    private Mesh mesh;

    
    
    public void DrawCircle(float x,float y,float radius,Color? color = null)
    {

        color ??= Color.white;
        Vector3 position = new Vector3(x,y);
        Quaternion rotation = Quaternion.identity;
        Vector3 scale = 2 * radius * Vector3.one;
        Matrix4x4 mat = Matrix4x4.TRS(position,rotation,scale);

        matrices[matrixIndex++] = mat;
        colors[colorIndex++] = (Vector4)color;

        //Set Colors in Shader to use with instance Id
        block.SetVectorArray("_Colors", colors);
    }
    private void Setup()
    {
        //TODO: Better alternative for loading Material
        material = Resources.Load("ParticleMaterial") as Material;
        mesh = CreateQuad();
        material.enableInstancing = true;
        block = new MaterialPropertyBlock();
    }

    private Mesh CreateQuad(float width = 1f, float height = 1f)
    {
        // Create a quad mesh.
        var mesh = new Mesh();

        float w = width * .5f;
        float h = height * .5f;
        var vertices = new Vector3[4] {
            new Vector3(-w, -h, 0),
            new Vector3(w, -h, 0),
            new Vector3(-w, h, 0),
            new Vector3(w, h, 0)
        };

        var tris = new int[6] {
            // lower left tri.
            0, 2, 1,
            // lower right tri
            2, 3, 1
        };

        var normals = new Vector3[4] {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
        };

        var uv = new Vector2[4] {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
        };

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.normals = normals;
        mesh.uv = uv;

        return mesh;
    }

    private void Start()
    {
        Setup();
    }

    private void Update()
    {
        // Draw a bunch of meshes each frame.
        UnityEngine.Graphics.DrawMeshInstanced(mesh, 0, material, matrices, matrixIndex, block);

        //Cleanup
        Array.Clear(matrices, 0,matrixIndex);
        Array.Clear(colors, 0, colorIndex);
        colorIndex = 0;
        matrixIndex = 0;
        block.SetVectorArray("_Colors", colors);
    }
}