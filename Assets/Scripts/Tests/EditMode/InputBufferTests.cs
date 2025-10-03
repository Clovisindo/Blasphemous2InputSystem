using Game.Input;
using Game.Input.Commands;
using NUnit.Framework;
using UnityEngine;
using static Utilities;

public class InputBufferTests
{
    [Test]
    public void AddAndDetectCombo()
    {
        var buf = new InputBuffer(10, 1f);
        float ts = Time.unscaledTime;

        buf.AddCommand(new AttackCommand(AttackType.Light, ts));
        buf.AddCommand(new AttackCommand(AttackType.Light, ts + 0.05f));
        buf.AddCommand(new AttackCommand(AttackType.Heavy, ts + 0.1f));

        var combo = buf.DetectCombo();
        Assert.AreEqual(ComboType.LightLightHeavy, combo);
    }
}
