using System;
using System.Collections.Generic;
using DG.Tweening;
using Doozy.Engine;
using MEC;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

namespace WarKiwiCode.Game_Files.Scripts.Managers
{
    public class UiManager : MonoBehaviour
    {

        
        [SerializeField] private TextMeshProUGUI currentWeaponButtonText;

        [Header("Ammo fields")]
        [SerializeField] private TextMeshProUGUI player1MaxAmmoText;
        [SerializeField] private TextMeshProUGUI player1CurrentAmmoText;
        [SerializeField] private TextMeshProUGUI player2MaxAmmoText;
        [SerializeField] private TextMeshProUGUI player2CurrentAmmoText;
        [SerializeField] private TextMeshProUGUI player1CurrentClipsText;
        [SerializeField] private TextMeshProUGUI player2CurrentClipsText;
        [SerializeField] private GameObject player1InfiniteClipsText;
        [SerializeField] private GameObject player2InfiniteClipsText;

        [Header("Reloading fields")]
        [SerializeField] private GameObject player1ReloadingText;
        [SerializeField] private GameObject player2ReloadingText;
        [SerializeField] private Slider player1ReloadingProgressBar;
        [SerializeField] private Slider player2ReloadingProgressBar;
        [SerializeField] private float progressBarFillSpeed = 0.5f;
        
        [Header("Health bars")]
        [SerializeField] private SpriteRenderer[] player1HealthPips;
        [SerializeField] private GameObject[] player2HealthPips;
        [SerializeField] private Color player1Damaged;
        [SerializeField] private Color player2Damaged;
        

        [Header("Start animations")]
        [SerializeField] private GameObject tacticalModeGrid;
        [SerializeField] private GameObject tacticalModeGridTop;
        [SerializeField] private GameObject tacticalModeGridBottom;
        [SerializeField] private float gridSpeed;
        [SerializeField] private Light2D backgroundLight;
        [SerializeField] private GameObject player1;
        [SerializeField] private GameObject player2;
        [SerializeField] private GameObject blakeSprite;
        [SerializeField] private GameObject zinjiSprite;
        

        private float _targetProgress = 0;
        private Vector3 _gridOriginalPosition;
        private float _backgroundLightOriginalIntensity;
        private int _player1CurrentHealthIndex;
        private int _player2CurrentHealthIndex;

        public static UiManager instance;
        private void Awake()
        {
            instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            _gridOriginalPosition = tacticalModeGrid.transform.position;
            _backgroundLightOriginalIntensity = backgroundLight.intensity;
            DisplayZinjiSprite(true);
            DisplayBlakeSprite(true);
            ResetPlayersHealthIndex();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                DamageHealthBar(1, "PlayerTop");
            }
        }

        public void DisplayBlakeSprite(bool value)
        {
            blakeSprite.SetActive(value);
            player2.SetActive(!value);
        }

        public void DisplayZinjiSprite(bool value)
        {
            zinjiSprite.SetActive(value);
            player1.SetActive(!value);
        }

        public void StartTacticalMode(GameObject grid) => Timing.RunCoroutine(TacticalModeGridMovement(grid));
        
        //public void StartTacticalModeTop(GameObject go) => Timing.RunCoroutine(TacticalModeGridMovement());

        private IEnumerator<float> TacticalModeGridMovement(GameObject grid)
        {
            yield return Timing.WaitForSeconds(1f);
            grid.transform.DOMoveY(-grid.transform.position.y, gridSpeed);
            DOTween.To(() => backgroundLight.intensity, intensity => backgroundLight.intensity = intensity, 0.15f, gridSpeed);
            yield return Timing.WaitForSeconds(gridSpeed);
            GameEventMessage.SendEvent("Tactical Mode Animation Done");
        }

        #region Ammo Settings

        public void SetPlayer1CurrentAmmoText(int ammoCount) => player1CurrentAmmoText.text = ammoCount.ToString();

        public void SetPlayer1MaxAmmoText(int ammoMax) => player1MaxAmmoText.text = " | " + ammoMax;

        public void SetPlayer2CurrentAmmoText(int ammoCount) => player2CurrentAmmoText.text = ammoCount.ToString();

        public void SetPlayer2MaxAmmoText(int ammoMax) => player2MaxAmmoText.text = " | " + ammoMax;

        public void SetPlayer1CurrentClipsText(int currentClips)
        {
            if (currentClips == 0)
            {
                player1CurrentClipsText.gameObject.SetActive(false);
                player1InfiniteClipsText.gameObject.SetActive(true);
            }
            else
            {
                player1CurrentClipsText.gameObject.SetActive(true);
                player1InfiniteClipsText.gameObject.SetActive(false);
                player1CurrentClipsText.text = currentClips.ToString();  
            }
        }

        public void SetPlayer2CurrentClipsText(int currentClips)
        {
            if (currentClips == 0)
            {
                player2CurrentClipsText.gameObject.SetActive(false);
                player2InfiniteClipsText.gameObject.SetActive(true);
            }
            else
            {
                player2CurrentClipsText.gameObject.SetActive(true);
                player2InfiniteClipsText.gameObject.SetActive(false);
                player2CurrentClipsText.text = currentClips.ToString();  
            }
        } 

        #endregion

        #region Reloading

        public void DisplayPlayer1ReloadProgressBar(bool value) => player1ReloadingProgressBar.gameObject.SetActive(value);

        public void DisplayPlayer1ReloadText(bool value) => player1ReloadingText.SetActive(value);

        public void DisplayPlayer2ReloadProgressBar(bool value) => player2ReloadingProgressBar.gameObject.SetActive(value);

        public void DisplayPlayer2ReloadText(bool value) => player2ReloadingText.SetActive(value);

        public void IncrementPlayer1ReloadProgress(float newProgress) => player1ReloadingProgressBar.value += newProgress;

        public void IncrementPlayer2ReloadProgress(float newProgress) => player2ReloadingProgressBar.value += newProgress;

        public void ResetPlayer1ReloadProgressBar() => player1ReloadingProgressBar.value = 0;

        public void ResetPlayer2ReloadProgressBar() => player2ReloadingProgressBar.value = 0;

        #endregion

        public void DamageHealthBar(int damage, string playerTag)
        {
            switch (playerTag)
            {
                case "PlayerTop":
                    for (int i = _player1CurrentHealthIndex; i < damage; i++)
                    {
                        Timing.RunCoroutine(TurnOffHealthPip(player1HealthPips[i], player1Damaged));
                    }
                    break;
                case "PlayerBottom":
                    break;
            }
        }

        private void ResetPlayersHealthIndex()
        {
            _player1CurrentHealthIndex = 0;
            _player2CurrentHealthIndex = 0;
        }

        private IEnumerator<float> TurnOffHealthPip(SpriteRenderer healthPip, Color targetColor)
        {
            float flickerDuration = 0.1f;
            Sequence healthBarDrainSequence = DOTween.Sequence();

            healthBarDrainSequence.Append(healthPip.DOColor(targetColor, 0.1f));
            healthBarDrainSequence.Append(healthPip.DOColor(Color.white, 0.1f));
            healthBarDrainSequence.Append(healthPip.DOColor(targetColor, 0.1f));
            healthBarDrainSequence.Append(healthPip.DOColor(Color.white, 0.1f));
            healthBarDrainSequence.Append(healthPip.DOColor(targetColor, 0.1f));
            healthBarDrainSequence.Append(healthPip.DOColor(Color.white, 0.1f));
            healthBarDrainSequence.Append(healthPip.DOColor(targetColor, 0.1f));
            healthBarDrainSequence.Append(healthPip.DOFade(0, 0.1f));

            healthBarDrainSequence.Play();
            
            yield return 0;
        }
    }
}
