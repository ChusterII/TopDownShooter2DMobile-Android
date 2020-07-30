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

        [Header("Reloading fields")]
        [SerializeField] private GameObject player1ReloadingText;
        [SerializeField] private GameObject player2ReloadingText;
        [SerializeField] private Slider player1ReloadingProgressBar;
        [SerializeField] private Slider player2ReloadingProgressBar;
        [SerializeField] private float progressBarFillSpeed = 0.5f;

        [Header("Start animations")]
        [SerializeField] private GameObject tacticalModeGrid;
        [SerializeField] private float gridSpeed;
        [SerializeField] private Light2D backgroundLight;
        [SerializeField] private GameObject player1;
        [SerializeField] private GameObject player2;
        [SerializeField] private GameObject blakeSprite;
        [SerializeField] private GameObject zinjiSprite;
        

        private float _targetProgress = 0;
        private Vector3 _gridOriginalPosition;
        private float _backgroundLightOriginalIntensity;

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

        public void StartTacticalMode()
        {
            print("Started Tactical Mode");
            Timing.RunCoroutine(TacticalModeGridMovement());
        }

        private IEnumerator<float> TacticalModeGridMovement()
        {
            yield return Timing.WaitForSeconds(1f);
            tacticalModeGrid.transform.DOMoveY(-_gridOriginalPosition.y, gridSpeed);
            DOTween.To(() => backgroundLight.intensity, intensity => backgroundLight.intensity = intensity, 0.15f, gridSpeed);
            yield return Timing.WaitForSeconds(gridSpeed);
            GameEventMessage.SendEvent("Tactical Mode Animation Done");
        }

        public void SetWeaponButtonText(string weaponText)
        {
            currentWeaponButtonText.text = "Equipped: " + weaponText;
        }

        public void SetPlayer1CurrentAmmoText(int ammoCount)
        {
            player1CurrentAmmoText.text = ammoCount.ToString();
        }

        public void SetPlayer1MaxAmmoText(int ammoMax)
        {
            player1MaxAmmoText.text = " | " + ammoMax;
        }
        
        public void SetPlayer2CurrentAmmoText(int ammoCount)
        {
            player2CurrentAmmoText.text = ammoCount.ToString();
        }

        public void SetPlayer2MaxAmmoText(int ammoMax)
        {
            player2MaxAmmoText.text = " | " + ammoMax;
        }

        public void DisplayPlayer1ReloadProgressBar(bool value)
        {
            player1ReloadingProgressBar.gameObject.SetActive(value);
        }
        
        public void DisplayPlayer1ReloadText(bool value)
        {
            player1ReloadingText.SetActive(value);
        }
        
        public void DisplayPlayer2ReloadProgressBar(bool value)
        {
            player2ReloadingProgressBar.gameObject.SetActive(value);
        }
        
        public void DisplayPlayer2ReloadText(bool value)
        {
            player2ReloadingText.SetActive(value);
        }

        public void IncrementPlayer1ReloadProgress(float newProgress)
        {
            player1ReloadingProgressBar.value += newProgress;
        }
        
        public void IncrementPlayer2ReloadProgress(float newProgress)
        {
            player2ReloadingProgressBar.value += newProgress;
        }

        public void ResetPlayer1ReloadProgressBar()
        {
            player1ReloadingProgressBar.value = 0;
        }
        
        public void ResetPlayer2ReloadProgressBar()
        {
            player2ReloadingProgressBar.value = 0;
        }
    }
}
