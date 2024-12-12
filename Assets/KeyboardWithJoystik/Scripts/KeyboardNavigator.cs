using System.Linq;
using UnityEngine;

namespace BNG
{
    public class KeyboardNavigator
    {
        private readonly Keyboard currentKeyboard;
        private readonly KeyboardKeyHighlighter keyboardKeyHighlighter;
        private readonly VRKeyboardWithJoystick keyboardWithJoystick;
        private VRKeyboardKey currentKey;
        private KeyboardRow currentRow;
        private bool isJoystickInput;
        private bool isCursorInput; 

        public KeyboardNavigator(VRKeyboardKey currentKey,
            KeyboardRow currentRow, Keyboard currentKeyboard,
            KeyboardKeyHighlighter keyboardKeyHighlighter, VRKeyboardWithJoystick keyboardWithJoystick)
        {
            this.currentKey = currentKey;
            this.currentRow = currentRow;
            this.currentKeyboard = currentKeyboard;
            this.keyboardKeyHighlighter = keyboardKeyHighlighter;
            this.keyboardWithJoystick = keyboardWithJoystick;
            keyboardWithJoystick.InputTypeChanged += OnInputTypeChanged;
            keyboardWithJoystick.NavigationModeChanged += OnNavigationModeChanged;
        }

        private void OnNavigationModeChanged(VRKeyboardWithJoystick.NavigationMode obj)
        {
            if (obj != VRKeyboardWithJoystick.NavigationMode.Key )
                keyboardKeyHighlighter.ResetHighlight();
        }

        private void OnInputTypeChanged(bool obj)
        {
            isJoystickInput = obj;

            if (!isJoystickInput)
                keyboardKeyHighlighter.ResetHighlight();
        }

        public void NavigateKeys(Vector2 thumbstickAxis)
        {
            if (!isJoystickInput || isCursorInput)
                return;

            if (thumbstickAxis == Vector2.zero)
                return;
            
            if (Mathf.Abs(thumbstickAxis.x) > Mathf.Abs(thumbstickAxis.y))
            {
                if (thumbstickAxis.x > 0) NextKey();
                else PreviousKey();
            }
            else
            {
                if (thumbstickAxis.y > 0) MoveUp();
                else MoveDown();
            }
        }

        public void NextKey() => ChangeKey(1);
        public void PreviousKey() => ChangeKey(-1);
        public void MoveUp() => MoveVertically(-1);
        public void MoveDown() => MoveVertically(1);

        private void MoveVertically(int direction)
        {
            if (currentRow == null || currentKey == null)
                return;

            var keyboardKeys = currentRow.keyboardKeys;
            var currentKeyIndex = keyboardKeys.IndexOf(currentKey);
            if (currentKeyIndex == -1)
                currentKeyIndex = 0;

            var keyboardRows = currentKeyboard.keyboardRows;
            var currentRowIndex = keyboardRows.IndexOf(currentRow);
            var newRowIndex = (currentRowIndex + direction + keyboardRows.Count) % keyboardRows.Count;
            currentRow = keyboardRows[newRowIndex];

            keyboardKeys = currentRow.keyboardKeys;

            if (keyboardKeys.Count > 0)
            {
                var ratio = (float)currentKeyIndex / keyboardKeys.Count;
                var newKeyIndex = Mathf.RoundToInt(ratio * (keyboardKeys.Count - 1));
                newKeyIndex = Mathf.Clamp(newKeyIndex, 0, keyboardKeys.Count - 1);

                keyboardWithJoystick.SetSelectedKey(keyboardKeys[newKeyIndex]);
                keyboardKeyHighlighter.HighlightKey(keyboardKeys[newKeyIndex]);
            }
        }

        private void ChangeKey(int direction)
        {
            if (currentKey == null)
                return;

            var keysCurrentRow = currentRow.keyboardKeys;

            var currentKeyIndex = keysCurrentRow.IndexOf(currentKey);
            var newKeyIndex = (currentKeyIndex + direction + keysCurrentRow.Count) % keysCurrentRow.Count;
            currentKey = keysCurrentRow[newKeyIndex];

            keyboardWithJoystick.SetSelectedKey(keysCurrentRow[newKeyIndex]);
            keyboardKeyHighlighter.HighlightKey(keysCurrentRow.ElementAt(newKeyIndex));
        }
    }
}