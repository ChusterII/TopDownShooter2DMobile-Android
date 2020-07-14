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
using UnityEngine;

namespace WarKiwiCode.Game_Files.Scripts.Managers
{
    public class ShellParticleSystemHandler : MonoBehaviour {

        public static ShellParticleSystemHandler Instance { get; private set; }

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
                if (single.IsMovementComplete()) {
                    _singleList.RemoveAt(i);
                    i--;
                }
            }
        }

        public void SpawnShell(Vector3 position, Vector3 direction, int uvIndex) {
            _singleList.Add(new Single(position, direction, _meshParticleSystem, uvIndex));
        }


        /*
     * Represents a single Shell
     * */
        private class Single {

            private readonly MeshParticleSystemManager _meshParticleSystem;
            private Vector3 _position;
            private readonly Vector3 _direction;
            private readonly int _quadIndex;
            private readonly Vector3 _quadSize;
            private float _rotation;
            private float _moveSpeed;
            private int _uvIndex;

            public Single(Vector3 position, Vector3 direction, MeshParticleSystemManager meshParticleSystem, int uvIndex) {
                this._position = position;
                this._direction = direction;
                this._meshParticleSystem = meshParticleSystem;

                //_quadSize = new Vector3(0.03f, 0.05f);
                _quadSize = new Vector3(0.09f, 0.09f);
                _rotation = Random.Range(0, 360f);
                _moveSpeed = Random.Range(1f, 3f);
                _uvIndex = uvIndex;

                _quadIndex = meshParticleSystem.AddQuad(position, _rotation, _quadSize, true, _uvIndex);
            }

            public void Update() {
                _position += _direction * (_moveSpeed * Time.deltaTime);
                _rotation += 360f * (_moveSpeed / 10f) * Time.deltaTime;

                _meshParticleSystem.UpdateQuad(_quadIndex, _position, _rotation, _quadSize, true, _uvIndex);

                float slowDownFactor = 3.5f;
                _moveSpeed -= _moveSpeed * slowDownFactor * Time.deltaTime;
            }

            public bool IsMovementComplete() {
                return _moveSpeed < .1f;
            }

        }

    }
}
