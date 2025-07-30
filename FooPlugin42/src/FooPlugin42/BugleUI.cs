using System.Collections;
using FooPlugin42.BuglePitch;
using UnityEngine;

namespace FooPlugin42;

internal class BugleUI : MonoBehaviour
{
    public static BugleUI? Instance;

    public static void Initialize(GameObject gameObject)
    {
        if (Instance) return;
        Instance = gameObject.AddComponent<BugleUI>();
        Plugin.Log.LogInfo("BugleUI initialized");
    }

    private void OnDestroy()
    {
        Instance = null;
        Plugin.Log.LogInfo("BugleUI destroyed");
    }

    private void OnGUI()
    {
        var cam = Camera.main;
        if (!cam) return;

        var character = Character.localCharacter;
        if (!character) return;

        var currentItem = character.data.currentItem;
        if (!currentItem) return;

        var bugle = currentItem.GetComponent<BugleSFX>();
        if (!bugle) return;

        const float lineLength = 20f;
        const float maxAngle = BuglePitchInput.MaxVerticalAngle;
        var partials = BuglePitchInput.HarmonicsCount;
        var divisions = partials - 1;

        var verticalView = ViewAngle.Vertical();
        var fovDeg = cam.fieldOfView;
        var halfFovRad = fovDeg * 0.5f * Mathf.Deg2Rad;
        float screenHeight = Screen.height;
        float screenWidth = Screen.width;

        for (var i = 1; i <= divisions; i++)
        {
            // Line is placed at the boundary between two partials
            var percent = (float)i / partials;
            var angle = Mathf.Lerp(-maxAngle, maxAngle, percent);

            var angleDiff = angle - verticalView;
            var angleDiffRad = angleDiff * Mathf.Deg2Rad;

            // If outside of vertical FOV, skip
            if (Mathf.Abs(angleDiffRad) > halfFovRad) continue;

            // Perspective projection onto screen Y
            var normalizedY = 0.5f - Mathf.Tan(angleDiffRad) / Mathf.Tan(halfFovRad);
            var screenY = normalizedY * screenHeight;

            DrawLine(screenWidth * 0.5f, screenY, lineLength);
        }
    }

    private static void DrawLine(float x, float y, float width)
    {
        var savedColor = GUI.color;
        GUI.color = Color.white;
        GUI.DrawTexture(new Rect(x, y, width, 5f), Texture2D.whiteTexture);
        GUI.color = savedColor;
    }
}
