using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace WarKiwiCode.Game_Files.Scripts.Managers.Player
{
    public class PlayerInputManager : MonoBehaviour
    {
        [System.Serializable]
        public class Vector2Event : UnityEvent<Vector2>{}

        //public Vector2Event onTouchTop;
        //public Vector2Event onTouchBottom;
        public UnityEvent onTouchTop;
        public UnityEvent onTouchBottom;

        // TEST OPTION
        [SerializeField] private bool holdAllowed;
        
        
        private Camera _camera;
        private Vector2 _touchedPosition;
        private readonly float _screenMiddle = Screen.height / 2f; 
        private bool _touchedTop;
        private int _topTouchesTotal, _bottomTouchesTotal;
        private int _topTouchesFirstFinger = 0;
        private int _bottomTouchesFirstFinger = 0;
        private int _topTouchesSecondFinger = 0;
        private int _bottomTouchesSecondFinger = 0;
        private Vector2 _startTouchPositionFirstFinger, _endTouchPositionFirstFinger;
        private Vector2 _startTouchPositionSecondFinger, _endTouchPositionSecondFinger;
        private bool _inputDisabled;
        private bool _topInputDisabled;
        private bool _bottomInputDisabled;
        
        void Start()
        {
            _camera = Camera.main;
        }

        void Update()
        {
            if (!_inputDisabled)
            {
                RegisterTouch();
            }
            
        }

        private void RegisterTouch()
        {
            // IsPointerOverGO is FOR TESTING ONLY!!!!
            if (Input.touchCount > 0 && !EventSystem.current.IsPointerOverGameObject(0) && !EventSystem.current.IsPointerOverGameObject(1))
            {
                foreach (Touch touch in Input.touches)
                {
                    if (touch.phase == TouchPhase.Began && AllowedFingers(touch))
                    {
                        InitializeFingersPositions(touch);
                        CalculateTouchPosition(touch);
                        ExecuteTap(touch);
                    }

                    if (holdAllowed && (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved) &&
                        AllowedFingers(touch))
                    {
                        CalculateTouchPosition(touch);
                        ExecuteHold(touch);
                    }

                    if (touch.phase == TouchPhase.Ended && AllowedFingers(touch))
                    {
                        CheckForSwipes(touch);
                        ResetTouches(touch);
                    }
                }
            }
        }

        /// <summary>
        /// Get the start position depending on which finger was used so they don't mix up.
        /// </summary>
        /// <param name="touch">The current touch</param>
        private void InitializeFingersPositions(Touch touch)
        {
            switch (touch.fingerId)
            {
                // First finger case
                case 0:
                    _startTouchPositionFirstFinger = touch.position;
                    break;
                
                // Second finger case
                case 1:
                    _startTouchPositionSecondFinger = touch.position;
                    break;
            }
        }

        #region Touch Execution

        private void ExecuteTap(Touch touch)
        {
            // Check where we touched (top or bottom)
            if (touch.position.y >= _screenMiddle)
            {
                if (_topInputDisabled) return;
                
                // We touched the upper part of the screen
                switch (touch.fingerId)
                {
                    // First finger case
                    case 0:
                        _topTouchesFirstFinger++;
                        break;
                    
                    // Second finger case
                    case 1:
                        _topTouchesSecondFinger++;
                        break;
                }

                // Increase total touches
                _topTouchesTotal++;

                // Register only one touch per half of the screen
                if (_topTouchesTotal == 1)
                {
                    _touchedTop = true;
                    onTouchTop.Invoke();
                }
            }
            else
            {
                if (_bottomInputDisabled) return;
                
                // We touched the lower part of the screen
                switch (touch.fingerId)
                {
                    // First finger case
                    case 0:
                        _bottomTouchesFirstFinger++;
                        break;
                    
                    // Second finger case
                    case 1:
                        _bottomTouchesSecondFinger++;
                        break;
                }

                // Increase total touches
                _bottomTouchesTotal++;

                // Register only one touch per half of the screen
                if (_bottomTouchesTotal == 1)
                {
                    _touchedTop = false;
                    onTouchBottom.Invoke();
                }
            }
        }

        private void ExecuteHold(Touch touch)
        {
            // Check where we touched (top or bottom)
            if (touch.position.y >= _screenMiddle)
            {
                if (_topInputDisabled) return;
                
                // We touched the upper part of the screen
                switch (touch.fingerId)
                {
                    case 0:
                        if (_topTouchesTotal == 1)
                        {
                            _touchedTop = true;
                            onTouchTop.Invoke();
                        }
                        else
                        {
                            print("Cannot execute hold: " + _topTouchesTotal + " top touches");
                        }
                        break;
                    
                    case 1:
                        if (_topTouchesTotal == 1)
                        {
                            _touchedTop = true;
                            onTouchTop.Invoke();
                        }
                        else
                        {
                            print("Cannot execute hold: " + _topTouchesTotal + " top touches");
                        }
                        break;
                }
            }
            else
            {
                if (_bottomInputDisabled) return;
                
                switch (touch.fingerId)
                {
                    case 0:
                        if (_bottomTouchesTotal == 1)
                        {
                            _touchedTop = false;
                            onTouchBottom.Invoke();
                        }
                        else
                        {
                            print("Cannot execute hold: " + _bottomTouchesTotal + " bot touches");
                        }
                        break;
                    
                    case 1:
                        if (_bottomTouchesTotal == 1)
                        {
                            _touchedTop = false;
                            onTouchBottom.Invoke();
                        }
                        else
                        {
                            print("Cannot execute hold: " + _bottomTouchesTotal + " bot touches");
                        }
                        break;
                }
            }
        }

        #endregion
        
        #region Ended Phase

        /// <summary>
        /// After the finger is released, this method is called to decrease the amounts of touches in the respective
        /// half of the screen, to a minimum of 0 touches.
        /// </summary>
        /// <param name="touch">The current touch</param>
        private void ResetTouches(Touch touch)
        {
            // Upper half of the screen
            if (touch.position.y >= _screenMiddle)
            {
                // Allow another touch
                switch (touch.fingerId)
                {
                    // First finger case
                    case 0:
                        _topTouchesFirstFinger--;
                        if (_topTouchesFirstFinger < 0)
                        {
                            _topTouchesFirstFinger = 0;
                        }
                        break;
                    
                    // Second finger case
                    case 1:
                        _topTouchesSecondFinger--;
                        if (_topTouchesSecondFinger < 0)
                        {
                            _topTouchesSecondFinger = 0;
                        }
                        break;
                }
                
                // Total touches
                --_topTouchesTotal;
                if (_topTouchesTotal < 0)
                {
                    _topTouchesTotal = 0;
                }
                
            }
            else
            {
                // Lower half of the screen
                // Allow another touch
                switch (touch.fingerId)
                {
                    // First finger case
                    case 0:
                        _bottomTouchesFirstFinger--;
                        if (_bottomTouchesFirstFinger < 0)
                        {
                            _bottomTouchesFirstFinger = 0;
                        }
                        break;
                    
                    // Second finger case
                    case 1:
                        _bottomTouchesSecondFinger--;
                        if (_bottomTouchesSecondFinger < 0)
                        {
                            _bottomTouchesSecondFinger = 0;
                        }
                        break;
                }

                // Total touches
                --_bottomTouchesTotal;
                if (_bottomTouchesTotal < 0)
                {
                    _bottomTouchesTotal = 0;
                }
            }
        }
        
        /// <summary>
        /// Checks if the player swiped across the half of the screen in either direction with one finger or several
        /// and resets the value of the number of touches on the half of the screen where the swipe started.
        /// </summary>
        /// <param name="touch">The current touch</param>
        private void CheckForSwipes(Touch touch)
        {
            switch (touch.fingerId)
            {
                // First finger case
                case 0:
                    // Get the end touch position
                    _endTouchPositionFirstFinger = touch.position;
                    
                    if (_startTouchPositionFirstFinger.y >= _screenMiddle && _endTouchPositionFirstFinger.y < _screenMiddle)
                    {
                        // Swiped down, 1st finger
                        --_topTouchesFirstFinger;
                        _topTouchesTotal--;
                    }
                    else if (_startTouchPositionFirstFinger.y < _screenMiddle &&
                             _endTouchPositionFirstFinger.y >= _screenMiddle)
                    {
                        // Swiped up, 1st finger
                        --_bottomTouchesFirstFinger;
                        _bottomTouchesTotal--;
                    }
                    break;

                // Second finger case
                case 1:
                    // Get the end touch position
                    _endTouchPositionSecondFinger = touch.position;
                    
                    if (_startTouchPositionSecondFinger.y >= _screenMiddle && _endTouchPositionSecondFinger.y < _screenMiddle)
                    {
                        // Swiped down, 2nd finger
                        --_topTouchesSecondFinger;
                        _topTouchesTotal--;
                    }
                    else if (_startTouchPositionSecondFinger.y < _screenMiddle &&
                             _endTouchPositionSecondFinger.y >= _screenMiddle)
                    {
                        // Swiped up, 2nd finger
                        --_bottomTouchesSecondFinger;
                        _bottomTouchesTotal--;
                    }
                    break;
            }
        }

        #endregion

        #region Utilities

        private void CalculateTouchPosition(Touch touch)
        {
            // Get the touched position in World Coordinates
            _touchedPosition = _camera.ScreenToWorldPoint(touch.position);
        }
        
        private static bool AllowedFingers(Touch touch)
        {
            return touch.fingerId == 0 || touch.fingerId == 1;
        }

        public Vector2 GetTouchedPosition()
        {
            return _touchedPosition;
        }

        public bool TouchedTop()
        {
            return _touchedTop;
        }

        public void SetHold(bool value)
        {
            holdAllowed = value;
        }

        public void DisableInput(bool value)
        {
            _inputDisabled = value;
        }
        public void DisableTopInput(bool value)
        {
            _topInputDisabled = value;
        }
        public void DisableBottomInput(bool value)
        {
            _bottomInputDisabled = value;
        }

        #endregion
    }
}
