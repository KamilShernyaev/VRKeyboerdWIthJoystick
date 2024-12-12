using System.Linq;
using UnityEngine;

namespace BNG
{
    public class CursorNavigator
    {
        private readonly KeyboardKeyHighlighter keyboardKeyHighlighter;
        private readonly VRKeyboardWithJoystick keyboardWithJoystick;
        private readonly Keyboard currentKeyboard;
        private readonly Vector2 scaledCanvasSize;
        private readonly GameObject cursor;
        private readonly float speed;
        
        private Vector2 cursorPosition;
        private bool isJoystickInput;

        public CursorNavigator(GameObject cursor, Vector2 canvasSizeDelta, Vector2 localScale, Keyboard currentKeyboard,
            KeyboardKeyHighlighter keyboardKeyHighlighter, VRKeyboardWithJoystick keyboardWithJoystick, float speed)
        {
            this.cursor = cursor;
            this.currentKeyboard = currentKeyboard;
            this.keyboardKeyHighlighter = keyboardKeyHighlighter;
            this.keyboardWithJoystick = keyboardWithJoystick;
            this.speed = speed;
            cursorPosition = cursor.transform.localPosition;
            scaledCanvasSize = new Vector2(canvasSizeDelta.x * localScale.x, canvasSizeDelta.y * localScale.y);
            keyboardWithJoystick.InputTypeChanged += OnInputTypeChanged;
            keyboardWithJoystick.NavigationModeChanged += OnNavigationModeChanged;
            cursor.SetActive(false);
        }

        private void OnNavigationModeChanged(VRKeyboardWithJoystick.NavigationMode obj)
        {
            if (obj != VRKeyboardWithJoystick.NavigationMode.Cursor)
            {
                keyboardKeyHighlighter.ResetHighlight();
                cursor.SetActive(false);
            }
            else
            {
                cursor.SetActive(true);
            }
        }

        private void OnInputTypeChanged(bool obj)
        {
            isJoystickInput = obj;

            cursor.SetActive(isJoystickInput);
            if (!isJoystickInput)
                keyboardKeyHighlighter.ResetHighlight();
        }

        public void UpdateCursorPosition(Vector2 thumbstickAxis)
        {
            if (!isJoystickInput)
                return;

            cursorPosition += thumbstickAxis * speed;
            cursorPosition.x = Mathf.Clamp(cursorPosition.x, -scaledCanvasSize.x / 2, scaledCanvasSize.x / 2);
            cursorPosition.y = Mathf.Clamp(cursorPosition.y, -scaledCanvasSize.y / 2, scaledCanvasSize.y / 2);
            cursor.transform.localPosition = cursorPosition;

            CheckForKeyHighlight();
        }

        private void CheckForKeyHighlight()
        {
            if (currentKeyboard == null)
                return;

            var cursorWorldPosition = cursor.transform.position;
            var highlightedKey =
                (from row in currentKeyboard.keyboardRows
                    from key in row.keyboardKeys
                    where IsCursorOverKey(cursorWorldPosition, key.GetComponent<RectTransform>())
                    select key)
                .FirstOrDefault();

            if (highlightedKey != null || !isJoystickInput)
            {
                keyboardKeyHighlighter.HighlightKey(highlightedKey);
                keyboardWithJoystick.SetSelectedKey(highlightedKey);
            }
            else
            {
                keyboardKeyHighlighter.ResetHighlight();
            }
        }

        private bool IsCursorOverKey(Vector3 worldCursorPosition, RectTransform keyRectTransform)
        {
            var localCursorPosition = keyRectTransform.InverseTransformPoint(worldCursorPosition);
            var size = keyRectTransform.rect;

            return localCursorPosition.x >= -size.width / 2 && localCursorPosition.x <= size.width / 2 &&
                   localCursorPosition.y >= -size.height / 2 && localCursorPosition.y <= size.height / 2;
        }
    }
}