using System;
using System.Collections.Generic;
using DG.Tweening;
using MEC;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WarKiwiCode.Game_Files.Scripts.Managers
{
    public class PowerUpManager : MonoBehaviour
    {

        // Needs to be in two separate lists
        [SerializeField] private List<PowerUp> powerUps;
        //[SerializeField] private List<PowerUp> bottomPowerUps;
        
        [Tooltip("General power up spawn chance")]
        [SerializeField] private float powerUpSpawnChance;

        [SerializeField] private RectTransform player1PowerUpPosition;
        

        private bool _player1HasPowerUp;
        private bool _player2HasPowerUp;
        private PowerUp _player1CurrentPowerUp;
        private PowerUp _player2CurrentPowerUp;

        // Start is called before the first frame update
        void Start()
        {
            GameEvents.instance.OnEnemyDeath += SpawnPowerUp;
            GameEvents.instance.OnPowerUpSpent += RemovePowerUp;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                SpawnPowerUp(new SpawnPositionAndArea(SpawnAreaName.Top, Camera.main.ScreenToWorldPoint(Input.mousePosition)));
            }
        }

        private void SpawnPowerUp(SpawnPositionAndArea deathAreaAndPosition)
        {
            // First, check to see if a power up will spawn.
            float check = Random.value;
            if (check <= powerUpSpawnChance)
            {
                // Get the Area where the enemy was killed, and get the player's status from there.
                switch (deathAreaAndPosition.spawnAreaName)
                {
                    case SpawnAreaName.Top:
                        if (!_player1HasPowerUp)
                        {
                            // Select a proper power up.
                            _player1CurrentPowerUp = SelectRandomPowerUp(powerUps);
                            
                            // Set PowerUp flag
                            _player1HasPowerUp = true;
                            
                            // Spawn a PowerUp
                            GameObject powerUp = ObjectPoolerManager.instance.Spawn(_player1CurrentPowerUp.powerUpName,
                                deathAreaAndPosition.spawnPosition, Quaternion.identity, SpawnAreaName.Top);
                            Timing.RunCoroutine(MoveToCanvas(powerUp));
                        }
                        else
                        {
                            check = Random.value;
                            if (check <= _player1CurrentPowerUp.spawnChance)
                            {
                                GameObject powerUp = ObjectPoolerManager.instance.Spawn(_player1CurrentPowerUp.powerUpName,
                                    deathAreaAndPosition.spawnPosition, Quaternion.identity, SpawnAreaName.Top);
                                Timing.RunCoroutine(MoveToCanvas(powerUp));
                            }
                        }
                        break;
                    case SpawnAreaName.Bottom:
                        if (!_player2HasPowerUp)
                        {
                            // Select a proper power up.
                            _player2CurrentPowerUp = SelectRandomPowerUp(powerUps);
                            
                            // Set PowerUp flag
                            _player2HasPowerUp = true;
                            
                            // Spawn a PowerUp
                            ObjectPoolerManager.instance.Spawn(_player2CurrentPowerUp.powerUpName,
                                deathAreaAndPosition.spawnPosition, Quaternion.identity, SpawnAreaName.Bottom);
                        }
                        break;
                }
            }
        }

        private IEnumerator<float> MoveToCanvas(GameObject powerUp)
        {
            powerUp.transform.SetParent(player1PowerUpPosition);
            yield return Timing.WaitForOneFrame;
            powerUp.transform.DOLocalMove(Vector3.zero, 1).SetEase(Ease.OutCubic);
        }

        private void RemovePowerUp(string playerTag)
        {
            if (playerTag.Equals("PlayerTop"))
            {
                _player1HasPowerUp = false;
                _player1CurrentPowerUp = null;
            }
            else
            {
                _player2HasPowerUp = false;
                _player2CurrentPowerUp = null;
            }
        }

        private PowerUp SelectRandomPowerUp(List<PowerUp> powerUpList)
        {
            // TODO: Add power up gradually as player completes chapters
            // TESTING: Random right now!
            int randomValue = Random.Range(0, powerUpList.Count);
            return powerUpList[randomValue];
        }


    }
}
