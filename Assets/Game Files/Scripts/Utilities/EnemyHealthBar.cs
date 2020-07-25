using System;
using UnityEngine;
using UnityEngine.Animations;

namespace WarKiwiCode.Game_Files.Scripts.Utilities
{
    public class EnemyHealthBar : MonoBehaviour
    {
        private Transform _transformCache;
        
        private void Start()
        {
            _transformCache = transform;
        }

        // Update is called once per frame
        private void LateUpdate()
        {
            FollowEnemy();
        }

        private void FollowEnemy()
        {
            _transformCache.rotation = Quaternion.identity;
            _transformCache.position = _transformCache.parent.position + new Vector3(0, 0.5f, 0);
        }
    }
}
