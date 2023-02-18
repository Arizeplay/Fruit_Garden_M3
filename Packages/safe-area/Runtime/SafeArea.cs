using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace AS.SafeArea
{
    [ExecuteAlways]
    public class SafeArea : MonoBehaviour
    {
        public Variant variant;
        public bool left = true;
        public bool right = true;
        public bool top = true;
        public bool bottom = true;
        public FloatRectOffset padding, minBorder;
        [Space]
        public float fixIphoneBottomFactor = 0.41666f;

        public delegate void OnChange(Rect rect);
        public event OnChange onSafeAreaChange;


        RectTransform rectTransform;
        LayoutGroup targetLayoutGroup;

        Rect safeArea = new Rect();
        Vector2 screenSize = Vector2.zero;
        ScreenOrientation screenOrientation = ScreenOrientation.AutoRotation;

        FloatRectOffset lastPadding, lastMinBorder;
        bool lastLeft, lastRight, lastTop, lastBottom;
        Variant lastVariant;

        bool isIphone = false;
        float oldFixIphoneBottomFactor = 0;

        public enum Variant
        {
            disable,
            anchorPosition,
            padding
        }

#if UNITY_EDITOR
        [MenuItem("GameObject/UI/Safe area", false, 5000)]
        static public void CreateSafeArea(MenuCommand menuCommand)
        {
            GameObject parent = menuCommand.context as GameObject;
            if (parent)
            {
                GameObject go = new GameObject("SafeArea");
                Undo.RegisterCreatedObjectUndo(go, "Create safe area");
                Undo.SetTransformParent(go.transform, parent.transform, "Parent safe area");
                GameObjectUtility.SetParentAndAlign(go, parent);

                RectTransform rectTransform = go.AddComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.sizeDelta = Vector2.zero;
                rectTransform.anchoredPosition = Vector2.zero;

                go.AddComponent<SafeArea>();

                Selection.activeGameObject = go;
            }
        }

        [MenuItem("GameObject/UI/Safe area", true, 5000)]
        static public bool CreateSafeAreaValidate()
        {
            return Selection.activeGameObject?.GetComponentInParent<Canvas>() != null;
        }
#endif

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            targetLayoutGroup = GetComponent<LayoutGroup>();
            isIphone = SystemInfo.deviceModel.Contains("iPhone");
        }

        void OnEnable()
        {
            Refresh();
        }

        void Update()
        {
            Refresh();

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                isIphone = SystemInfo.deviceModel.Contains("iPhone");
                Rect r = ModifySafeArea(safeArea);
                ApplySafeArea(r);
            }
#endif
        }

        void Refresh()
        {
            Rect currentSafeArea = Screen.safeArea;

            if (currentSafeArea != safeArea ||
                Screen.width != screenSize.x ||
                Screen.height != screenSize.y ||
                Screen.orientation != screenOrientation || Changed())
            {
                screenSize.x = Screen.width;
                screenSize.y = Screen.height;
                lastPadding = new FloatRectOffset(padding);
                lastMinBorder = new FloatRectOffset(minBorder);

                lastLeft = left;
                lastRight = right;
                lastTop = top;
                lastBottom = bottom;

                lastVariant = variant;

                screenOrientation = Screen.orientation;

                safeArea = currentSafeArea;
                oldFixIphoneBottomFactor = fixIphoneBottomFactor;

                Rect r = ModifySafeArea(safeArea);
                ApplySafeArea(r);

                onSafeAreaChange?.Invoke(r);

            }
        }

        bool Changed()
        {
            if (lastBottom != bottom || lastLeft != left || lastRight != right || lastTop != top) return true;
            if (!lastPadding.Equals(padding) || !lastMinBorder.Equals(minBorder)) return true;
            if (lastVariant != variant) return true;
            if (oldFixIphoneBottomFactor != fixIphoneBottomFactor) return true;

            return false;
        }

        Rect ModifySafeArea(Rect r)
        {
            if (fixIphoneBottomFactor != 1 && isIphone)
            {
                switch (screenOrientation)
                {
                    case ScreenOrientation.Portrait:
                        r.yMin = r.yMin * fixIphoneBottomFactor;
                        break;
                    case ScreenOrientation.PortraitUpsideDown:
                        r.yMax = Screen.height + (r.yMax - Screen.height) * fixIphoneBottomFactor;
                        break;
                    case ScreenOrientation.LandscapeRight:
                        r.xMin = r.xMin * fixIphoneBottomFactor;
                        break;
                    case ScreenOrientation.LandscapeLeft:
                        r.xMax = Screen.width + (r.xMax - Screen.width) * fixIphoneBottomFactor;
                        break;
                }

            }

            Vector2 size = GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;


            float xScale = size.x / (1.0f * Screen.width);
            float yScale = size.y / (1.0f * Screen.height);

            r.yMax = Mathf.Min(r.yMax, Screen.height - minBorder.top / yScale) - padding.top / yScale;
            r.yMin = Mathf.Max(r.yMin, minBorder.bottom / yScale) + padding.bottom / yScale;
            r.xMax = Mathf.Min(r.xMax, Screen.width - minBorder.right / yScale) - padding.right / yScale;
            r.xMin = Mathf.Max(r.xMin, minBorder.left / yScale) + padding.left / yScale;


            return r;
        }

        void ApplySafeArea(Rect r)
        {
            if (!top)
                r.yMax = Screen.height;

            if (!bottom)
                r.yMin = 0;

            if (!right)
                r.xMax = Screen.width;

            if (!left)
                r.xMin = 0;

            if (variant == Variant.anchorPosition)
            {
                SetRectAnchored(r);
            }
            else if (variant == Variant.padding)
            {
                SetRectPadding(r);
            }
        }

        void SetRectAnchored(Rect r)
        {
            if (!rectTransform) rectTransform = GetComponent<RectTransform>();

            Vector2 anchorMin = r.position;
            Vector2 anchorMax = r.position + r.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }

        void SetRectPadding(Rect r)
        {
            if (!targetLayoutGroup) targetLayoutGroup = GetComponent<LayoutGroup>();
            if (!targetLayoutGroup)
            {
                this.enabled = false;
                throw new System.Exception("LayoutGroup not found");
            }


            Vector2 size = GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;


            float xScale = size.x / (1.0f * Screen.width);
            float yScale = size.y / (1.0f * Screen.height);

            targetLayoutGroup.padding.top = Mathf.RoundToInt((Screen.height - r.yMax) * xScale);
            targetLayoutGroup.padding.bottom = Mathf.RoundToInt(r.yMin * yScale);
            targetLayoutGroup.padding.left = Mathf.RoundToInt(r.xMin * xScale);
            targetLayoutGroup.padding.right = Mathf.RoundToInt((Screen.width - r.xMax) * xScale);

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);

        }
    }
}
