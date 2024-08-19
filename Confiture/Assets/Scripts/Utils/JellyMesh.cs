using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyMesh : MonoBehaviour
{
    public float intensity = 1.0f;
    public float mass = 1f;
    public float stiffness = 1f;
    public float damping = 0.75f;

    private Mesh originalMesh, meshClone;
    private MeshRenderer renderer;

    private JellyVertex[] jv;
    private Vector3[] vertexArray;

    private void Start()
    {
        originalMesh = GetComponent<MeshFilter>().sharedMesh;
        meshClone = Instantiate(originalMesh);

        GetComponent<MeshFilter>().sharedMesh = meshClone;
        renderer = GetComponent<MeshRenderer>();

        jv = new JellyVertex[meshClone.vertices.Length];
        for(int i = 0; i < meshClone.vertices.Length; i++)
        {
            jv[i] = new JellyVertex(i, transform.TransformPoint(meshClone.vertices[i]));
        }


    }

    private void FixedUpdate()
    {
        vertexArray = originalMesh.vertices;
        for(int i = 0;i < jv.Length; i++)
        {
            Vector3 target = transform.TransformPoint(vertexArray[jv[i].ID]);
            float jellyIntensity = (1 - (renderer.bounds.max.y - target.y) / renderer.bounds.size.y) * intensity;

            jv[i].Shake(target, mass, stiffness, damping);
            target = transform.InverseTransformPoint(jv[i].position);
            vertexArray[jv[i].ID] = Vector3.Lerp(vertexArray[jv[i].ID], target, jellyIntensity);
        }
        meshClone.vertices = vertexArray;
    }

    public class JellyVertex
    {
        public int ID;
        public Vector3 position;
        public Vector3 velocity, force;

        public JellyVertex(int _id, Vector3 _pos)
        {
            ID = _id;
            position = _pos;
        }

        public void Shake(Vector3 target, float m, float s, float d)
        {
            force = (target - position) * s;
            velocity = (velocity + force / m) * d;
            position += velocity;

            if((velocity + force + force/m).magnitude < 0.001f)
                position = target;
        }
    }
}
