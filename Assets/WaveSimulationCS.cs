using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodai.Wave.Two {

    public class WaveSimulationCS : MonoBehaviour {

        public int num = 256;
        private int bufferSize;

        public float decay = 0.99f;
        public float dt = 1 / 60f;
        public float dd = 0.1f;
        public float c = 1;

        public float interaction_depth = -0.2f;

        public Transform obj;

        #region GPU
        public ComputeShader cs;
        ComputeBuffer u_old;
        ComputeBuffer u_tmp;
        ComputeBuffer u_new;
        const int SIMULATION_BLOCK_SIZE = 32;
        int threadGroupSize;
        #endregion GPU

        void Start() {
            bufferSize = (num + 2) * (num + 2);

            u_old = new ComputeBuffer(bufferSize, sizeof(float));
            u_tmp = new ComputeBuffer(bufferSize, sizeof(float));
            u_new = new ComputeBuffer(bufferSize, sizeof(float));
            threadGroupSize = Mathf.CeilToInt(bufferSize / SIMULATION_BLOCK_SIZE) + 1;

            Initialize();
            
        }
        
        void Update() {
            int kernel = -1;
            kernel = cs.FindKernel("Update");
            cs.SetBuffer(kernel, "u_old", u_old);
            cs.SetBuffer(kernel, "u_tmp", u_tmp);
            cs.SetBuffer(kernel, "u_new", u_new);
            cs.Dispatch(kernel, threadGroupSize, 1, 1);
            
            kernel = cs.FindKernel("Interaction");
            cs.SetBuffer(kernel, "u_new", u_new);
            cs.SetVector("_ObjPos", obj.position);
            cs.Dispatch(kernel, threadGroupSize, 1, 1);

            kernel = cs.FindKernel("CopyBuffer");
            cs.SetBuffer(kernel, "u_old", u_old);
            cs.SetBuffer(kernel, "u_tmp", u_tmp);
            cs.SetBuffer(kernel, "u_new", u_new);
            cs.Dispatch(kernel, threadGroupSize, 1, 1);
            
            // ChangeBuffer(); だめ。教訓。
        }

        void OnDestroy() {
            u_old.Release();
            u_tmp.Release();
            u_new.Release();
        }

        void Initialize() {

            SetCSVariables();

            int kernel = -1;

            kernel = cs.FindKernel("InitializeOld");
            cs.SetBuffer(kernel, "u_old", u_old);
            cs.SetBuffer(kernel, "u_new", u_new);
            cs.Dispatch(kernel, threadGroupSize, 1, 1);

            kernel = cs.FindKernel("InitializeTmp");
            cs.SetBuffer(kernel, "u_old", u_old);
            cs.SetBuffer(kernel, "u_tmp", u_tmp);
            cs.Dispatch(kernel, threadGroupSize, 1, 1);

        }

        void SetCSVariables() {
            cs.SetInt("_Num", num + 2);
            cs.SetInt("_BufferSize", bufferSize);
            cs.SetFloat("_DT", dt);
            cs.SetFloat("_DD", dd);
            cs.SetFloat("_C", c);
            cs.SetFloat("_Decay", decay);
            cs.SetFloat("_InteractionIntensity", interaction_depth);
        }

        void ChangeBuffer() {
            // 絶対やっちゃダメ！CS側でコピーせよ！
            //u_old = u_tmp;
            //u_tmp = u_new;
        }
        
        public int GetBufferSize() {
            return bufferSize;
        }

        public ComputeBuffer GetBuffer() {
            return u_new;
        }
    }
}