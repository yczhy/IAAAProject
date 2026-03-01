using UnityEngine;
using UnityEngine.UI;

namespace Duskvern
{
    public static class UIUtils
    {
        private static Canvas Canvas => UIModule.Instance.UICanvas;

        private static Camera UICamera
        {
            get
            {
                if (Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                    return null;

                return Canvas.worldCamera != null
                    ? Canvas.worldCamera
                    : Camera.main;
            }
        }

        #region 世界坐标 <-> 屏幕坐标

        /// <summary>
        /// 世界坐标 -> 屏幕坐标
        /// </summary>
        public static Vector2 WorldToScreen(Vector3 worldPos, Camera worldCamera = null)
        {
            if (worldCamera == null)
                worldCamera = Camera.main;

            return worldCamera.WorldToScreenPoint(worldPos);
        }

        /// <summary>
        /// 屏幕坐标 -> 世界坐标
        /// </summary>
        public static Vector3 ScreenToWorld(Vector2 screenPos, Camera worldCamera = null, float depth = 10f)
        {
            if (worldCamera == null)
                worldCamera = Camera.main;

            Vector3 pos = new Vector3(screenPos.x, screenPos.y, depth);
            return worldCamera.ScreenToWorldPoint(pos);
        }

        #endregion

        #region 屏幕坐标 <-> UI本地坐标

        /// <summary>
        /// 屏幕坐标 -> UI局部坐标
        /// </summary>
        public static Vector2 ScreenToUILocal(RectTransform target, Vector2 screenPos)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                target,
                screenPos,
                UICamera,
                out Vector2 localPoint);

            return localPoint;
        }

        /// <summary>
        /// UI局部坐标 -> 屏幕坐标
        /// </summary>
        public static Vector2 UILocalToScreen(RectTransform target, Vector2 localPos)
        {
            Vector3 world = target.TransformPoint(localPos);
            return RectTransformUtility.WorldToScreenPoint(UICamera, world);
        }

        #endregion

        #region 世界坐标 <-> UI坐标

        /// <summary>
        /// 世界坐标 -> UI anchoredPosition
        /// </summary>
        public static Vector2 WorldToUIAnchoredPosition(RectTransform uiTarget, Vector3 worldPos, Camera worldCamera = null)
        {
            Vector2 screenPos = WorldToScreen(worldPos, worldCamera);
            return ScreenToUILocal(uiTarget, screenPos);
        }

        /// <summary>
        /// UI anchoredPosition -> 世界坐标
        /// </summary>
        public static Vector3 UIToWorld(RectTransform uiTarget, Vector2 anchoredPos, float depth = 10f)
        {
            Vector2 screenPos = UILocalToScreen(uiTarget, anchoredPos);
            return ScreenToWorld(screenPos, Camera.main, depth);
        }

        #endregion

        #region 工具方法

        /// <summary>
        /// 判断世界点是否在屏幕内
        /// </summary>
        public static bool IsWorldPointVisible(Vector3 worldPos, Camera worldCamera = null)
        {
            if (worldCamera == null)
                worldCamera = Camera.main;

            Vector3 viewport = worldCamera.WorldToViewportPoint(worldPos);
            return viewport.z > 0 &&
                   viewport.x > 0 && viewport.x < 1 &&
                   viewport.y > 0 && viewport.y < 1;
        }

        #endregion
    }
}