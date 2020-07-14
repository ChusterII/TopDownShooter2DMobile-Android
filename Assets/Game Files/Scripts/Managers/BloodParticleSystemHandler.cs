/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;

namespace WarKiwiCode.Game_Files.Scripts.Managers
{
    public class BloodParticleSystemHandler : MonoBehaviour {

        public static BloodParticleSystemHandler Instance { get; private set; }

        private MeshParticleSystemManager _meshParticleSystem;
        private List<Single> _singleList;

        private void Awake() {
            Instance = this;
            _meshParticleSystem = GetComponent<MeshParticleSystemManager>();
            _singleList = new List<Single>();
        }

        private void Update() {
            for (int i=0; i<_singleList.Count; i++) {
                Single single = _singleList[i];
                single.Update();
                if (single.IsParticleComplete()) {
                    _singleList.RemoveAt(i);
                    i--;
                }
            }
        }

        public void SpawnBlood(Vector3 position, Vector3 direction) {
            float bloodParticleCount = 3;
            for (int i = 0; i < bloodParticleCount; i++) {
                _singleList.Add(new Single(position, UtilsClass.ApplyRotationToVector(direction, Random.Range(-15f, 15f)), _meshParticleSystem));
            }
        }


        /*
     * Represents a single Dirt Particle
     * */
        private class Single {

            private readonly MeshParticleSystemManager _meshParticleSystem;
            private Vector3 _position;
            private readonly Vector3 _direction;
            private readonly int _quadIndex;
            private readonly Vector3 _quadSize;
            private float _moveSpeed;
            private float _rotation;
            private readonly int _uvIndex;

            public Single(Vector3 position, Vector3 direction, MeshParticleSystemManager meshParticleSystem) {
                this._position = position;
                this._direction = direction;
                this._meshParticleSystem = meshParticleSystem;

                _quadSize = new Vector3(0.2f, 0.2f);
                _rotation = Random.Range(0, 360f);
                _moveSpeed = Random.Range(5f, 7f);
                _uvIndex = Random.Range(0, 8);

                _quadIndex = meshParticleSystem.AddQuad(position, _rotation, _quadSize, false, _uvIndex);
            }

            public void Update() {
                _position += _direction * (_moveSpeed * Time.deltaTime);
                _rotation += 360f * (_moveSpeed / 10f) * Time.deltaTime;

                _meshParticleSystem.UpdateQuad(_quadIndex, _position, _rotation, _quadSize, false, _uvIndex);

                float slowDownFactor = 3.5f;
                _moveSpeed -= _moveSpeed * slowDownFactor * Time.deltaTime;
            }

            public bool IsParticleComplete() {
                return _moveSpeed < .1f;
            }

        }

    }
}
