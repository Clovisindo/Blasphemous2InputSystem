using Game.Input;
using Game.Input.Commands;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class InputAdapterTests 
{
    InputAdapter _adapter;
    IInputStrategy _keyboardStrategy;
    IInputStrategy _gamepadStrategy;
    IInputDeviceWatcher _deviceWatcher;

    [SetUp]
    public void Setup()
    {
        _keyboardStrategy = Substitute.For<IInputStrategy>();
        _gamepadStrategy = Substitute.For<IInputStrategy>();
        _deviceWatcher = Substitute.For<IInputDeviceWatcher>();
        _adapter = new InputAdapter(_keyboardStrategy, _gamepadStrategy, _deviceWatcher);
    }

    [TearDown]
    public void TearDown()
    {
        _adapter.ShutDown();
    }

    [Test]
    public void Update_WhenHasStrategy_ThenPollAndEnqueueInputCommand()
    {
        _adapter.SetStrategy(_keyboardStrategy);
        List<InputCommand> inputCommands = new List<InputCommand>() { new MovementCommand(Vector2.right, 1f) };
        _keyboardStrategy.Poll(Arg.Any<float>()).Returns(inputCommands);

        _adapter.Update(1f);

        _keyboardStrategy.Received(1).Poll(Arg.Any<float>());
        Assert.IsTrue(_adapter.TryDequeue(out _));//comprobamos que se ha añadido algo a _queue private
    }

    [Test]
    public void Update_WhenNoStrategy_ThenDoNothing()
    {
        _adapter.Update(1f);

        _keyboardStrategy.Received(0).Poll(Arg.Any<float>());
        Assert.IsTrue(!_adapter.TryDequeue(out _));
    }

    [Test]
    public void SetStrategy_WhenHasPrevStrategy_ThenShutdownAndInitialize()
    {
        _adapter.SetStrategy(_keyboardStrategy);

        _adapter.SetStrategy(_gamepadStrategy);

        _keyboardStrategy.Received(1).ShutDown();
        _gamepadStrategy.Received(1).Initialize(Arg.Any<PlayerInputActions>());
    }

    [Test]
    public void SetStrategy_WhenNoPrevStrategy_ThenInitialize()
    {
        _adapter.SetStrategy(_keyboardStrategy);

        _keyboardStrategy.Received(0).ShutDown();
        _gamepadStrategy.Received(0).ShutDown();
        _keyboardStrategy.Received(1).Initialize(Arg.Any<PlayerInputActions>());
    }

    [Test]
    public void TryDequeue_WhenHasElements_ThenDequeue()
    {
        InputCommand dequeueCommand;
        InputCommand moveCommand = new MovementCommand(Vector2.right, 1f);
        _adapter.SetStrategy(_keyboardStrategy);
        List<InputCommand> inputCommands = new List<InputCommand>() { moveCommand };
        _keyboardStrategy.Poll(Arg.Any<float>()).Returns(inputCommands);
        _adapter.Update(1f);

        bool result = _adapter.TryDequeue(out dequeueCommand);

        Assert.AreEqual(moveCommand, dequeueCommand);
        Assert.IsTrue(result);
    }

    [Test]
    public void TryDequeue_WhenNoElements_ThenTryDequeue()
    {
        InputCommand dequeueCommand;

        bool result =_adapter.TryDequeue(out dequeueCommand);

        Assert.IsNull(dequeueCommand);
        Assert.IsFalse(result);
    }

    [Test]
    public void Enqueue_WhenEnqueue_ThenVerify()
    {
        InputCommand dequeueCommand;
        InputCommand command = new MovementCommand(Vector2.right, 1f);

        _adapter.Enqueue(command);

        bool result = _adapter.TryDequeue(out dequeueCommand);
        Assert.AreEqual(command, dequeueCommand);
        Assert.IsTrue(result);
    }

    [Test]
    public void Shutdown_WhenHasPrevStrategy_ThenShutdownAndClear()
    {
        _adapter.SetStrategy(_keyboardStrategy);

        _adapter.ShutDown();
        _deviceWatcher.OnDeviceChanged += Raise.Event<Action<InputDeviceType>>(InputDeviceType.Gamepad);

        _keyboardStrategy.Received(1).ShutDown();
        Assert.IsFalse(_adapter.TryDequeue(out _));
        _gamepadStrategy.Received(0).Initialize(Arg.Any<PlayerInputActions>());
    }

    [Test]
    public void OnDeviceChange_WhenIsDifferentStrategy_ThenVerifyChanges()
    {
        _adapter.SetStrategy(_keyboardStrategy);

        _deviceWatcher.OnDeviceChanged += Raise.Event<Action<InputDeviceType>>(InputDeviceType.Gamepad);

        _keyboardStrategy.Received(1).ShutDown();
        _gamepadStrategy.Received(1).Initialize(Arg.Any<PlayerInputActions>());//asi sabemos que el currentStrategy y currentDevice cambiaron
    }

    [Test]
    public void OnDeviceChange_WhenIsSameStrategy_ThenDoNothing()
    {
        _adapter.SetStrategy(_keyboardStrategy);

        _deviceWatcher.OnDeviceChanged += Raise.Event<Action<InputDeviceType>>(InputDeviceType.KeyboardMouse);

        _keyboardStrategy.Received(0).ShutDown();
        _gamepadStrategy.Received(0).Initialize(Arg.Any<PlayerInputActions>());
    }
}
