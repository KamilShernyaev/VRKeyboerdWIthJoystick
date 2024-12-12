using System;
using BNG;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardUIView : MonoBehaviour
{
    [SerializeField] private VRKeyboardWithJoystick keyboardWithJoystick;
    [SerializeField] private Text leftHandedText;
    [SerializeField] private Text navigationModeText;
    [SerializeField] private Text joystickStatusText;

    private void Start()
    {
        leftHandedText.text = $"Left Handed: {keyboardWithJoystick.IsLeftHanded}";
        navigationModeText.text = $"Navigation Mode: {keyboardWithJoystick.Mode}";
        joystickStatusText.text = $"Joystick Input: {keyboardWithJoystick.IsJoystickInput}";
        keyboardWithJoystick.InputTypeChanged += OnInputTypeChanged;
        keyboardWithJoystick.NavigationModeChanged += OnNavigationModeChanged;
        keyboardWithJoystick.LeftHandedChanged += OnLeftHandedChanged;
    }

    private void OnDisable()
    {
        keyboardWithJoystick.InputTypeChanged -= OnInputTypeChanged;
        keyboardWithJoystick.NavigationModeChanged -= OnNavigationModeChanged;
        keyboardWithJoystick.LeftHandedChanged -= OnLeftHandedChanged;
    }

    private void OnLeftHandedChanged(bool obj) =>
        leftHandedText.text = $"Left Handed: {obj}";

    private void OnNavigationModeChanged(VRKeyboardWithJoystick.NavigationMode obj) =>
        navigationModeText.text = $"Navigation Mode: {obj}";

    private void OnInputTypeChanged(bool obj) =>
        joystickStatusText.text = $"Joystick Input: {obj}";
}