using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kodai.Wave.Two {

    public class MeshRenderer2D : MonoBehaviour {

        public WaveSimulation script;
        Mesh mesh;
        
        void Start() {
            GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        }

        void Update() {
            CreateVertices();
        }

        void CreateVertices() {
            
            mesh.name = "Procedural Grid";

            Vector3[] vertices = new Vector3[(script.num+1) * (script.num+1)];

            int i = 0;
            for (int y = 0; y <= script.num; y++) {
                for (int x = 0; x <= script.num; x++) {
                    vertices[i] = new Vector3(x, script.u_new[y+1,x+1] * 10, y);
                    i++;
                }
            }
            mesh.vertices = vertices;

            int[] triangles = new int[script.num * script.num * 6];
            for (int ti = 0, vi = 0, y = 0; y < script.num; y++, vi++) {
                for (int x = 0; x < script.num; x++, ti += 6, vi++) {
                    triangles[ti] = vi;
                    triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                    triangles[ti + 4] = triangles[ti + 1] = vi + script.num+ 1;
                    triangles[ti + 5] = vi + script.num + 2;
                }
            }
            mesh.triangles = triangles;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }
    }
}