using System;
using System.Collections.Generic;
using System.Linq;
using florius.Common;
using UnityEditor;
using UnityEngine;

namespace florius.UI
{
    [CreateAssetMenu(fileName = "AutoRotation Settings", menuName = "UI/AutoRotation Settings", order = 0)]
    public class DeviceBasedAutoRotationSettings : ScriptableObject
    {
        [Flags]
        public enum Orientations
        {
            None = 0,
            Portrait = 1,
            PortraitUpsideDown = 2,
            LandscapeLeft = 4,
            LandscapeRight = 8,
            Everything = 15
        }

        public List<float> Diagonals;
        public List<Orientations> AllowedOrientations;

        public static List<DeviceBasedAutoRotationSettings> AllSettings =>
            Resources.FindObjectsOfTypeAll<DeviceBasedAutoRotationSettings>().ToList();

        [RuntimeInitializeOnLoadMethod]
        private static void ApplySettings()
        {
            var d = new List<(float diagonal, Orientations orientations)>();
            foreach (var setting in AllSettings)
                d.AddRange(setting.Diagonals.Zip(setting.AllowedOrientations, (x, y) => (x, y)).ToList());

            var h = Screen.height;
            var w = Screen.width;
            var diagonal = Mathf.Sqrt(h * h + w * w) / Screen.dpi;

            foreach (var v in d.DistinctBy(t => t.diagonal).Where(v => diagonal < v.diagonal))
            {
                Screen.autorotateToPortrait = v.orientations.HasFlag(Orientations.Portrait);
                Screen.autorotateToPortraitUpsideDown = v.orientations.HasFlag(Orientations.PortraitUpsideDown);
                Screen.autorotateToLandscapeLeft = v.orientations.HasFlag(Orientations.LandscapeLeft);
                Screen.autorotateToLandscapeRight = v.orientations.HasFlag(Orientations.LandscapeRight);
                return;
            }
        }

#if UNITY_EDITOR
        [ContextMenu("OnValidate")]
        private void OnValidate()
        {
            AllowedOrientations = AllowedOrientations.Select(x => x == Orientations.None ? Orientations.Portrait : x)
                .ToList();
        }

        private static void AddToPreloadedAssets()
        {
            var assets = PlayerSettings.GetPreloadedAssets()
                .Where(x => !(x is DeviceBasedAutoRotationSettings || x is null))
                .ToList();
            assets.AddRange(AllSettings);
            PlayerSettings.SetPreloadedAssets(assets.ToArray());
        }
#endif
    }
}