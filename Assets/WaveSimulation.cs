using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kodai.Wave.Two {

    public class WaveSimulation : MonoBehaviour {

        public int num = 256;

        public float decay = 0.99f;
        public float dt = 1/60f;
        public float dd = 0.1f;
        public float c = 1;

        public float interaction_depth = -0.2f;

        private float[,] u_old;
        private float[,] u_tmp;
        public float[,] u_new;
        public Transform sphere;
        
        void Start() {
            u_old = new float[num + 2, num+2]; // 境界分を余分にとる
            u_tmp = new float[num + 2, num + 2];
            u_new = new float[num + 2, num + 2];

            Initialize();
        }

        float time = 0;
        void Update() {

            for (int y = 1; y < num + 1; y++) {
                for (int x = 1; x < num + 1; x++) {
                    u_new[y, x] = decay * (2f * u_tmp[y, x] - u_old[y, x] + c * c * dt * dt / (dd * dd) * (u_tmp[y + 1, x] + u_tmp[y - 1, x] + u_tmp[y, x + 1] + u_tmp[y, x - 1] - 4.0f * u_tmp[y,x]));
                }
            }

            time += Time.deltaTime;
            if(sphere != null) {
                for (int y = 1; y < num + 1; y++) {
                    for (int x = 1; x < num + 1; x++) {
                        if (Vector2.Distance(new Vector2(sphere.position.x, sphere.position.z), new Vector2(x, y)) < 5) {
                            u_new[y, x] = interaction_depth;
                        }
                    }
                }
            }

            SetBoundaryCondition();
            
            FlipBuffer();
        }

        void SetBoundaryCondition() {
            for(int x = 0; x < num + 2; x++) {
                u_old[0, x] = 0;
                u_old[num + 1, x] = 0;
                u_tmp[0, x] = 0;
                u_tmp[num + 1, x] = 0;
                u_new[0, x] = 0;
                u_new[num + 1, x] = 0;
            }

            for (int y = 0; y < num + 2; y++) {
                u_old[y, 0] = 0;
                u_old[y, num + 1] = 0;
                u_tmp[y, 0] = 0;
                u_tmp[y, num + 1] = 0;
                u_new[y, 0] = 0;
                u_new[y, num + 1] = 0;
            }
        }

        void Initialize() {

            // Initialize u0
            for(int y = 1; y<num + 1; y++) {
                for (int x = 1; x < num + 1; x++) {
                    u_old[y, x] = 0;
                    u_tmp[y, x] = 0;
                    u_new[y, x] = 0;
                }
            }


            for (int y = 1; y < num + 1; y++) {
                for (int x = 1; x < num + 1; x++) {
                    u_tmp[y, x] = u_old[y, x] + c * c / 2.0f * dt * dt / (dd * dd) * (u_old[y + 1, x] + u_old[y - 1, x] + u_old[y, x + 1] + u_old[y, x - 1] - 4.0f * u_old[y,x]);
                }
            }
            SetBoundaryCondition();

        }

        void FlipBuffer() {
            for (int x = 0; x < num + 2; x++) {
                for (int y = 0; y < num + 2; y++) {
                    u_old[y, x] = u_tmp[y, x];
                    u_tmp[y, x] = u_new[y, x];
                }
            }
        }
        
    }
}