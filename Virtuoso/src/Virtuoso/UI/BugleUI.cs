using UnityEngine;
using Virtuoso.Config;
using Virtuoso.Input;

namespace Virtuoso.UI;

// TODO Leave guide lines at initial bend horizontal?

internal class BugleUI : MonoBehaviour
{
    private static BugleUI? _instance;
    private static bool _visible = true;
    private static KeyCode ToggleUIKey => BugleConfig.ToggleUIKey.Value;

    public static void Initialize(GameObject gameObject)
    {
        if (_instance) return;
        _instance = gameObject.AddComponent<BugleUI>();
        Plugin.Log.LogInfo("BugleUI initialized");
    }

    public static void DestroyInstance()
    {
        if (!_instance) return;
        Destroy(_instance);
        _instance = null;
        Plugin.Log.LogInfo("BugleUI destroyed");
    }

    private void Update()
    {
        if (UnityEngine.Input.GetKeyDown(ToggleUIKey)) _visible = !_visible;
    }

    private static bool IsPaused => GUIManager.instance.pauseMenu.isOpen;

    private void OnGUI()
    {
        if (!_visible || IsPaused) return;

        var character = Character.localCharacter;
        if (!character) return;

        var currentItem = character.data.currentItem;
        if (!currentItem || !currentItem.TryGetComponent<BugleSFX>(out _)) return;

        // TODO Cache camera?
        var cam = Camera.main;
        if (!cam) return;

        const float lineLength = 20f;
        var maxAngle = BuglePartial.MaxAngle;
        var partials = BuglePartial.Partials;
        var divisions = partials - 1;

        var verticalAngle = character.data.lookValues.y;
        var fovDeg = cam.fieldOfView;
        var halfFovRad = fovDeg * 0.5f * Mathf.Deg2Rad;
        float screenHeight = Screen.height;
        float screenWidth = Screen.width;

        for (var i = 1; i <= divisions; i++)
        {
            // Line is placed at the boundary between two partials
            var percent = (float)i / partials;
            var angle = Mathf.Lerp(-maxAngle, maxAngle, percent);

            var angleDiff = angle - verticalAngle;
            var angleDiffRad = angleDiff * Mathf.Deg2Rad;

            // If outside of vertical FOV, skip
            if (Mathf.Abs(angleDiffRad) > halfFovRad) continue;

            // Perspective projection onto screen Y
            var normalizedY = 0.5f - Mathf.Tan(angleDiffRad) / Mathf.Tan(halfFovRad);
            var screenY = normalizedY * screenHeight;
            var screenX = (screenWidth - lineLength) * 0.5f;

            DrawLine(screenX, screenY, lineLength);
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
