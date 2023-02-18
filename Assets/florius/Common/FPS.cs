using UnityEngine;

namespace florius.Common
{
    public class FPS
    {
        [RuntimeInitializeOnLoadMethod]
        private static void SetFPS()
        {
            Application.targetFrameRate = 120;
        }
    }
}