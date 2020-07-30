using System.Collections.Generic;
using Doozy.Engine;
using MEC;
using TMPro;
using UnityEngine;

namespace WarKiwiCode.Game_Files.Scripts.Utilities
{
    public class TacticalModeText : MonoBehaviour
    {
        
        [SerializeField] private string textToDisplay;
        [SerializeField] private float typingSpeed;

        private TextMeshProUGUI _tacticalModeText;
        private const string Underscore = "_";
        private int _blinkCount = 0;
        
        // Start is called before the first frame update
        void Start()
        {
            _tacticalModeText = GetComponent<TextMeshProUGUI>();
            _tacticalModeText.text = "";
            Timing.RunCoroutine(Type());
        }

        private IEnumerator<float> Type()
        {
            yield return Timing.WaitForSeconds(0.2f);
            while (_blinkCount < 3)
            {
                _tacticalModeText.text = "";
                yield return Timing.WaitForSeconds(0.2f);
                _tacticalModeText.text = Underscore;
                yield return Timing.WaitForSeconds(0.2f);
                _blinkCount++;
            }
            
            foreach (char letter in textToDisplay.ToCharArray())
            {
                _tacticalModeText.text += letter;
                yield return Timing.WaitForSeconds(typingSpeed);
            }

            yield return Timing.WaitForSeconds(1f);
            GameEventMessage.SendEvent("Tactical Text Done");
        }
    }
}
