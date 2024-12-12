using System;
using UnityEngine;

namespace BNG
{
    public class SpecialVRKeyboardKey : VRKeyboardKey
    {
        public Action<KeyType> OnSpecialKeyHit;
        
        [SerializeField] private KeyType keyType;

        public override void OnKeyHit()
        {
            OnSpecialKeyHit?.Invoke(keyType);
        }
    }
}