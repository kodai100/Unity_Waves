using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kodai.Wave.Two {

    public class WaveSimulation : MonoBehaviour {

        public int num = 256;

        public float dt = 1/60f;
        public float dd = 0.1f;
        public float c = 1;

        private float[,] u_old;
        private float[,] u_tmp;
        public float[,] u_new;

        Texture2D texture;
        
        void Start() {
            u_old = new float[num + 2, num+2]; // 境界分を余分にとる
            u_tmp = new float[num + 2, num + 2];
            u_new = new float[num + 2, num + 2];

            texture = new Texture2D(256,256);
            texture.filterMode = FilterMode.Point;

            Initialize();

            ApplyTexture();
        }
        
        void Update() {

            for (int y = 1; y < num + 1; y++) {
                for (int x = 1; x < num + 1; x++) {
                    u_new[y, x] = 2f * u_tmp[y, x] - u_old[y, x] + c * c * dt * dt / (dd * dd) * (u_tmp[y + 1, x] + u_tmp[y - 1, x] + u_tmp[y, x + 1] + u_tmp[y, x - 1] - 4.0f * u_tmp[y,x]);
                }
            }

            if (Input.GetMouseButtonUp(0)) {
                for (int y = 1; y < num + 1; y++) {
                    for (int x = 1; x < num + 1; x++) {
                        if (Vector2.Distance(new Vector2(num / 2, num / 2), new Vector2(x, y)) < 10) {
                            u_new[y, x] = 0.1f;
                        }
                    }
                }
                
            }

            SetBoundaryCondition();

            ApplyTexture();
            
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
                    if(Vector2.Distance(new Vector2(num/2, num/2), new Vector2(x, y)) < 10) {
                        u_old[y, x] = 1;
                    }
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

        void ApplyTexture() {
            for (int y = 1; y < num + 1; y++) {
                for (int x = 1; x < num + 1; x++) {
                    texture.SetPixel(x - 1, y - 1, new Color(u_new[y,x]/2f +0.5f, u_new[y, x] / 2f + 0.5f, u_new[y, x] / 2f + 0.5f));
                }
            }
            texture.Apply();
        }
    }
}