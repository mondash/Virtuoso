using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Virtuoso.Data;
using Virtuoso.Event;

namespace Virtuoso.Behaviour;

[RequireComponent(typeof(BugleSFX))]
internal abstract class BugleBehaviour : MonoBehaviour, IBugleListener
{
    private static T Connect<T>(BugleSFX sfx) where T : BugleBehaviour =>
        sfx.TryGetComponent<T>(out var behaviour)
            ? behaviour : sfx.gameObject.AddComponent<T>();

    public static void Connect(BugleSFX sfx)
    {
        Connect<BugleInput>(sfx);
        Connect<BugleAudio>(sfx);
        Connect<BugleSync>(sfx);
        Connect<BugleDrop>(sfx);
    }

    protected static void Disconnect(BugleSFX sfx) =>
        sfx.GetComponents<BugleBehaviour>()
            .ToList()
            .ForEach(Destroy);

    public static void DisconnectAll() =>
        FindObjectsByType<BugleBehaviour>(FindObjectsSortMode.None)
            .ToList()
            .ForEach(Destroy);

    private Action? _unsubscribe;
    private bool _implementsOnTootStarted, _implementsOnTootStopped, _implementsOnFrame, _implementsOnUpdate;

    protected string Name { get; private set; } = null!;
    protected BugleSFX Sfx { get; private set; } = null!;
    protected BugleHub Hub { get; private set; } = null!;

    protected bool IsMine => Sfx.photonView.IsMine;
    protected int ViewID => Sfx.photonView.ViewID;

    private static bool ImplementsHandler(Type type, string name)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        var method = type.GetMethod(name, flags);
        return method != null && method.DeclaringType != typeof(BugleBehaviour);
    }

    private void Awake()
    {
        var type = GetType();
        _implementsOnTootStarted = ImplementsHandler(type, nameof(OnTootStarted));
        _implementsOnTootStopped = ImplementsHandler(type, nameof(OnTootStopped));
        _implementsOnFrame = ImplementsHandler(type, nameof(OnFrame));
        _implementsOnUpdate = ImplementsHandler(type, nameof(OnUpdate));
        Name = type.Name;
        Sfx = GetComponent<BugleSFX>();
        Hub = BugleHub.Get(Sfx);
    }

    protected virtual void OnEnable()
    {
        _unsubscribe ??= Hub.Subscribe(this);
        Plugin.Log.LogInfo($"Enabled {Name} for view {ViewID}");
    }

    protected virtual void OnDisable()
    {
        _unsubscribe?.Invoke();
        _unsubscribe = null;
        Plugin.Log.LogInfo($"Disabled {Name} for view {ViewID}");
    }

    protected virtual bool ShouldHandleTootStarted => true;
    protected virtual void OnTootStarted() { }

    protected virtual bool ShouldHandleTootStopped => true;
    protected virtual void OnTootStopped() { }

    protected virtual bool ShouldHandleFrame => true;
    protected virtual void OnFrame(BuglePitchFrame f) { }

    protected virtual bool ShouldUpdate => Sfx.hold && Time.timeScale > 0;
    protected virtual void OnUpdate() {}

    void IBugleListener.HandleTootStarted()
    {
        if (!_implementsOnTootStarted || !ShouldHandleTootStarted) return;
        Plugin.Log.LogDebug($"{Name} handling toot start for view {ViewID}");
        OnTootStarted();
    }

    void IBugleListener.HandleTootStopped()
    {
        if (!_implementsOnTootStopped || !ShouldHandleTootStopped) return;
        Plugin.Log.LogDebug($"{Name} handling toot stop for view {ViewID}");
        OnTootStopped();
    }

    void IBugleListener.HandleFrame(BuglePitchFrame frame)
    {
        if (!_implementsOnFrame || !ShouldHandleFrame) return;
        Plugin.Log.LogDebug($"{Name} handling frame for view {ViewID}");
        OnFrame(frame);
    }

    private void Update()
    {
        if (!_implementsOnUpdate || !ShouldUpdate) return;
        // TODO Additional flag for logging update?
        Plugin.Log.LogDebug($"{Name} handling update for view {ViewID}");
        OnUpdate();
    }
}
