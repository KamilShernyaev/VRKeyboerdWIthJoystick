using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BNG
{
    public class VRKeyboardWithJoystick : VRKeyboard
    {
        public Action<bool> InputTypeChanged;
        public Action<bool> LeftHandedChanged;
        public Action<NavigationMode> NavigationModeChanged;

        [SerializeField] private GameObject parent;
        [SerializeField] private CharacterController player;
        [SerializeField] private List<Keyboard> keyboards;
        [SerializeField] private float speed;
        [SerializeField] private float inputDelay = 0.03f;

        private KeyboardNavigator keyboardNavigator;
        private CursorNavigator cursorNavigator;
        private KeyboardKeyHighlighter keyboardKeyHighlighter;

        private NavigationMode navigationMode;
        private VRKeyboardKey currentKey;
        private KeyboardRow currentRow;
        private Keyboard currentKeyboard;
        private float lastThumbstickInputTime;
        private SmoothLocomotion smoothLocomotion;
        private PlayerRotation playerRotation;
        private UIPointer[] pointers;

        public enum NavigationMode
        {
            Cursor,
            Key
        }

        public string Mode => navigationMode.ToString();
        public bool IsLeftHanded { get; private set; }
        public bool IsJoystickInput { get; private set; }

        private void OnValidate() => keyboards = GetComponentsInChildren<Keyboard>(includeInactive: true).ToList();

        protected void Awake()
        {
            smoothLocomotion = player.GetComponent<SmoothLocomotion>();
            playerRotation = player.GetComponent<PlayerRotation>();
            pointers = player.GetComponentsInChildren<UIPointer>();

            var canvasRectTransform = GetComponent<RectTransform>();
            var canvasSizeDelta = canvasRectTransform.sizeDelta;
            var localScale = canvasRectTransform.localScale;

            currentKeyboard = keyboards[0];
            currentRow = keyboards[0].keyboardRows[0];
            currentKey = currentRow.keyboardKeys[0];

            keyboardKeyHighlighter = new KeyboardKeyHighlighter(currentKey);
            keyboardNavigator =
                new KeyboardNavigator(currentKey, currentRow, currentKeyboard, keyboardKeyHighlighter, this);
            cursorNavigator =
                new CursorNavigator(CreateCursor(), canvasSizeDelta, localScale, currentKeyboard,
                    keyboardKeyHighlighter, this);
            SubscribeToSpecialKeys();

            NavigationModeChanged?.Invoke(navigationMode);
        }

        private GameObject CreateCursor()
        {
            var cursor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            cursor.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            cursor.transform.SetParent(parent.transform);
            cursor.transform.localPosition = Vector3.zero;
            return cursor;
        }

        private void Update()
        {
            if (!IsJoystickInput)
                return;

            var thumbstickAxis = IsLeftHanded
                ? InputBridge.Instance.LeftThumbstickAxis
                : InputBridge.Instance.RightThumbstickAxis;

            var thumbstickInputDown = IsLeftHanded
                ? InputBridge.Instance.LeftTriggerDown
                : InputBridge.Instance.RightTriggerDown;

            switch (navigationMode)
            {
                case NavigationMode.Cursor:
                    cursorNavigator.UpdateCursorPosition(thumbstickAxis * speed);
                    break;
                case NavigationMode.Key:
                    keyboardNavigator.NavigateKeys(thumbstickAxis * speed);
                    break;
            }

            if (!thumbstickInputDown || !(Time.time - lastThumbstickInputTime > inputDelay))
                return;

            lastThumbstickInputTime = Time.time;
            if (currentKey)
            {
                currentKey.ThisButton.onClick.Invoke();
            }
        }

        private void SubscribeToSpecialKeys()
        {
            foreach (var key in currentKeyboard.keyboardRows.SelectMany(row =>
                         row.keyboardKeys.OfType<SpecialVRKeyboardKey>()))
            {
                key.OnSpecialKeyHit += HandleSpecialKeyPressed;
            }
        }

        private void HandleSpecialKeyPressed(KeyType keyType)
        {
            switch (keyType)
            {
                case KeyType.SwitchNavigationMode:
                    SwitchNavigationMode();
                    break;
                case KeyType.SwitchKeyboard:
                    NextKeyboard();
                    break;
                case KeyType.ToggleJoystick:
                    ToggleJoystickInput();
                    break;
                case KeyType.ToggleLeftHanded:
                    ToggleLeftHanded();
                    break;
            }
        }

        private void SwitchKeyboard(int direction)
        {
            if (keyboards.Count == 0) return;

            var currentKeyboardIndex = keyboards.IndexOf(currentKeyboard);
            var newKeyboardIndex = (currentKeyboardIndex + direction + keyboards.Count) % keyboards.Count;
            currentKeyboard.gameObject.SetActive(false);
            currentKeyboard = keyboards[newKeyboardIndex];
            currentKeyboard.gameObject.SetActive(true);
            if (navigationMode == NavigationMode.Key)
            {
                currentRow = currentKeyboard.keyboardRows[0];
                currentKey = currentRow.keyboardKeys[0];
            }

            keyboardKeyHighlighter.HighlightKey(currentKey);
        }

        public void ToggleJoystickInput()
        {
            IsJoystickInput = !IsJoystickInput;
            InputTypeChanged?.Invoke(IsJoystickInput);

            smoothLocomotion.AllowInput = !IsJoystickInput;
            playerRotation.AllowInput = !IsJoystickInput;

            foreach (var pointer in pointers)
                pointer.gameObject.SetActive(!IsJoystickInput);
        }

        public void ToggleLeftHanded()
        {
            IsLeftHanded = !IsLeftHanded;
            LeftHandedChanged?.Invoke(IsLeftHanded);
        }

        public void SwitchNavigationMode()
        {
            navigationMode =
                (NavigationMode)(((int)navigationMode + 1) % Enum.GetNames(typeof(NavigationMode)).Length);
            NavigationModeChanged?.Invoke(navigationMode);
        }

        public void NextKey() => keyboardNavigator.NextKey();

        public void PreviousKey() => keyboardNavigator.PreviousKey();

        public void MoveDown() => keyboardNavigator.MoveDown();

        public void MoveUp() => keyboardNavigator.MoveUp();

        public void NextKeyboard() => SwitchKeyboard(1);

        public void PreviousKeyboard() => SwitchKeyboard(-1);

        public void SetSelectedKey(VRKeyboardKey keyboardKey) => currentKey = keyboardKey;
        public void SetCurrentKeyboard(Keyboard keyboard) => currentKeyboard = keyboard;
    }
}