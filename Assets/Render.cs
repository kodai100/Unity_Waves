using UnityEngine;

namespace Kodai.Wave.Two {

    [RequireComponent(typeof(WaveSimulationCS))]
    public class Render : MonoBehaviour {

        public WaveSimulationCS GPUScript;

        public Material ParticleRenderMat;

        void OnRenderObject() {
            DrawObject();
        }

        void DrawObject() {
            Material m = ParticleRenderMat;
            m.SetPass(0);
            m.SetInt("_Num", GPUScript.num);
            m.SetBuffer("_Points", GPUScript.GetBuffer());
            Graphics.DrawProcedural(MeshTopology.Points, GPUScript.GetBufferSize());
        }

    }

}