using System;
using UnityEngine;
namespace SH.API
{
    public static partial class Extension
    {
        public static void UpdateRectForSafeArea(this RectTransform rect, bool SetAnchor = true)
        {
            if (SetAnchor)
            {
                Vector2 anchorMin = Screen.safeArea.position;
                Vector2 anchorMax = Screen.safeArea.position + Screen.safeArea.size;
                anchorMin.x /= Screen.width;
                anchorMin.y /= Screen.height;
                anchorMax.x /= Screen.width;
                anchorMax.y /= Screen.height;

                rect.anchorMin = anchorMin;
                rect.anchorMax = anchorMax;
            }
            else
            {
                Debug.LogError("Not Implemented...!");
            }
        }
    }
}