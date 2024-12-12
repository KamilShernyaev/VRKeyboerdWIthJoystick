using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BNG
{
    public class KeyboardRow : MonoBehaviour
    {
        public List<VRKeyboardKey> keyboardKeys { get; private set; }

        private void OnValidate()
        {
            keyboardKeys = GetComponentsInChildren<VRKeyboardKey>(includeInactive: true).ToList();
        }
    }
}