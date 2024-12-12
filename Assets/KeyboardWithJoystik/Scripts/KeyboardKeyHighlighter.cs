using UnityEngine;

namespace BNG
{
    public class KeyboardKeyHighlighter
    {
        private VRKeyboardKey currentKey;

        public KeyboardKeyHighlighter(VRKeyboardKey currentKey)
        {
            this.currentKey = currentKey;
            HighlightKey(currentKey);
        }

        public void HighlightKey(VRKeyboardKey key)
        {
            if (currentKey != null)
                currentKey.ThisButton.image.color = Color.white;

            currentKey = key;
            key.ThisButton.image.color = Color.yellow;
            
        }

        public void ResetHighlight()
        {
            if (currentKey)
                currentKey.ThisButton.image.color = Color.white;
            currentKey = null;
        }
    }
}