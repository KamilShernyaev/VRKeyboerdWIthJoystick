using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BNG
{
    public class Keyboard : MonoBehaviour
    {
        public List<KeyboardRow> keyboardRows;
        public Dictionary<VRKeyboardKey, RectTransform> keyPositions;

        private void OnValidate()
        {
            keyboardRows = GetComponentsInChildren<KeyboardRow>(includeInactive: true).ToList();
        }

        private void Awake()
        {
            InitializeKeyPositions();
        }

        private void InitializeKeyPositions()
        {
            keyPositions = new Dictionary<VRKeyboardKey, RectTransform>();

            foreach (var row in keyboardRows)
            {
                foreach (var key in row.keyboardKeys)
                {
                    keyPositions[key] = key.GetComponent<RectTransform>();
                }
            }
        }
    }
}