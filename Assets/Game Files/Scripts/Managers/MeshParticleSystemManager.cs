using System;
using System.Collections.Generic;
using CodeMonkey;
using CodeMonkey.Utils;
using UnityEngine;

namespace WarKiwiCode.Game_Files.Scripts.Managers
{
    public class MeshParticleSystemManager : MonoBehaviour
    {
        private const int MAX_QUAD_AMOUNT = 15000;
        
        // Set in the Editor using Pixel Values
        [System.Serializable]
        public struct ParticleUvPixels 
        {
            public Vector2Int uv00Pixels;
            public Vector2Int uv11Pixels;
        }
        
        // Holds normalized texture UV Coordinates
        private struct UvCoords 
        {
            public Vector2 uv00;
            public Vector2 uv11;
        }
        
        [SerializeField] private ParticleUvPixels[] particleUvPixelsArray;
        private UvCoords[] _uvCoordsArray;
        
        private Mesh _mesh;
        private Renderer _renderer;
        private Vector3[] _vertices;
        private Vector2[] _uv;
        private int[] _triangles;
        
        private int _quadIndex;
        
        private bool _updateVertices;
        private bool _updateUv;
        private bool _updateTriangles;

        private void Awake()
        {
            _mesh = new Mesh();
            
            _vertices = new Vector3[4 * MAX_QUAD_AMOUNT];
            _uv = new Vector2[4 * MAX_QUAD_AMOUNT];
            _triangles = new int[6 * MAX_QUAD_AMOUNT];

            _mesh.vertices = _vertices;
            _mesh.uv = _uv;
            _mesh.triangles = _triangles;
            _mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 10000f);

            GetComponent<MeshFilter>().mesh = _mesh;
            GetComponent<Renderer>().sortingOrder = -1;
            
            // Set up internal UV Normalized Array
            Material material = GetComponent<MeshRenderer>().material;
            Texture mainTexture = material.mainTexture;
            int textureWidth = mainTexture.width;
            int textureHeight = mainTexture.height;

            List<UvCoords> uvCoordsList = new List<UvCoords>();
            foreach (ParticleUvPixels particleUvPixels in particleUvPixelsArray) {
                UvCoords uvCoords = new UvCoords { 
                    uv00 = new Vector2((float)particleUvPixels.uv00Pixels.x / textureWidth, (float)particleUvPixels.uv00Pixels.y / textureHeight),
                    uv11 = new Vector2((float)particleUvPixels.uv11Pixels.x / textureWidth, (float)particleUvPixels.uv11Pixels.y / textureHeight),
                };
                uvCoordsList.Add(uvCoords);
            }
            _uvCoordsArray = uvCoordsList.ToArray();
        }

        

        public int AddQuad(Vector3 position, float rotation, Vector3 quadSize, bool skewed, int uvIndex)
        {
            if (_quadIndex >= MAX_QUAD_AMOUNT) return 0; // Mesh full
            // MARKER: Try to get the quad count. If it reaches the max, destroy the first quad and replace it with a new one
            
            UpdateQuad(_quadIndex, position, rotation ,quadSize, skewed, uvIndex);

            int spawnedQuadIndex = _quadIndex;
            _quadIndex++;
            return spawnedQuadIndex;
        }

        public void UpdateQuad(int quadIndex, Vector3 position, float rotation, Vector3 quadSize, bool skewed, int uvIndex)
        {
            //Relocate vertices
            int vIndex = quadIndex * 4;
            int vIndex0 = vIndex;
            int vIndex1 = vIndex + 1;
            int vIndex2 = vIndex + 2;
            int vIndex3 = vIndex + 3;
            
            if (skewed) {
                _vertices[vIndex0] = position + Quaternion.Euler(0, 0, rotation) * new Vector3(-quadSize.x, -quadSize.y);
                _vertices[vIndex1] = position + Quaternion.Euler(0, 0, rotation) * new Vector3(-quadSize.x, +quadSize.y);
                _vertices[vIndex2] = position + Quaternion.Euler(0, 0, rotation) * new Vector3(+quadSize.x, +quadSize.y);
                _vertices[vIndex3] = position + Quaternion.Euler(0, 0, rotation) * new Vector3(+quadSize.x, -quadSize.y);
            } else {
                _vertices[vIndex0] = position + Quaternion.Euler(0, 0, rotation - 180) * quadSize;
                _vertices[vIndex1] = position + Quaternion.Euler(0, 0, rotation - 270) * quadSize;
                _vertices[vIndex2] = position + Quaternion.Euler(0, 0, rotation - 0) * quadSize;
                _vertices[vIndex3] = position + Quaternion.Euler(0, 0, rotation - 90) * quadSize;
            }
            
            // UV
            UvCoords uvCoords = _uvCoordsArray[uvIndex];
            _uv[vIndex0] = uvCoords.uv00;
            _uv[vIndex1] = new Vector2(uvCoords.uv00.x, uvCoords.uv11.y);
            _uv[vIndex2] = uvCoords.uv11;
            _uv[vIndex3] = new Vector2(uvCoords.uv11.x, uvCoords.uv00.y);
            
            //Create triangles
            int tIndex = quadIndex * 6;
            
            _triangles[tIndex + 0] = vIndex0;
            _triangles[tIndex + 1] = vIndex1;
            _triangles[tIndex + 2] = vIndex2;

            _triangles[tIndex + 3] = vIndex0;
            _triangles[tIndex + 4] = vIndex2;
            _triangles[tIndex + 5] = vIndex3;

            _updateVertices = true;
            _updateUv = true;
            _updateTriangles = true;
        }
        
        public void DestroyQuad(int quadIndex) {
            // Destroy vertices
            int vIndex = quadIndex * 4;
            int vIndex0 = vIndex;
            int vIndex1 = vIndex + 1;
            int vIndex2 = vIndex + 2;
            int vIndex3 = vIndex + 3;
        
            _vertices[vIndex0] = Vector3.zero;
            _vertices[vIndex1] = Vector3.zero;
            _vertices[vIndex2] = Vector3.zero;
            _vertices[vIndex3] = Vector3.zero;

            _updateVertices = true;
        }

        private void LateUpdate() 
        {
            if (_updateVertices) 
            {
                _mesh.vertices = _vertices;
                _updateVertices = false;
            }
            if (_updateUv) 
            {
                _mesh.uv = _uv;
                _updateUv = false;
            }
            if (_updateTriangles)
            {
                _mesh.triangles = _triangles;
                _updateTriangles = false;
            }
        }
    }
}
