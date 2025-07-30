namespace FooPlugin42.Input;

internal static class ViewAngle
{
    public static float Vertical()
    {
        var c = Character.localCharacter;
        return c ? c.data.lookValues.y : 0;
    }

    public static float Horizontal()
    {
        var c = Character.localCharacter;
        return c ? c.data.lookValues.x : 0;
    }

    // TODO Would these be useful?
    // public static float Vertical(Component component) => component ? component.transform.eulerAngles.y : 0;
    //
    // public static float Horizontal(Component component) => component ? component.transform.eulerAngles.x : 0;
}
