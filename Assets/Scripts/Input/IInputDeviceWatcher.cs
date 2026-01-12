using System;

namespace Game.Input
{
    public interface IInputDeviceWatcher
    {
        event Action<InputDeviceType> OnDeviceChanged;

        void Dispose();
    }
}