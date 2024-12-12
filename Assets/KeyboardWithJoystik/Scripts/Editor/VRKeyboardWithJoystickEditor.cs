using UnityEditor;
using UnityEngine;

namespace BNG
{
    [CustomEditor(typeof(VRKeyboardWithJoystick))]
    public class VRKeyboardWithJoystickEditor : Editor
    {
        private VRKeyboardWithJoystick keyboardWithJoystick;

        private void OnEnable()
        {
            keyboardWithJoystick = (VRKeyboardWithJoystick)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Toggle Joystick"))
            {
                keyboardWithJoystick.ToggleJoystickInput();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Toggle Left Handed"))
            {
                keyboardWithJoystick.ToggleLeftHanded();
            }

            if (GUILayout.Button("Switch Navigation Mode"))
            {
                keyboardWithJoystick.SwitchNavigationMode();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Next Keyboard"))
            {
                keyboardWithJoystick.NextKeyboard();
            }

            if (GUILayout.Button("Previous Keyboard"))
            {
                keyboardWithJoystick.PreviousKeyboard();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("Next Key"))
            {
                keyboardWithJoystick.NextKey();
            }

            if (GUILayout.Button("Move Down"))
            {
                keyboardWithJoystick.MoveDown();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("Previous Key"))
            {
                keyboardWithJoystick.PreviousKey();
            }

            if (GUILayout.Button("Move Up"))
            {
                keyboardWithJoystick.MoveUp();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
    }
}