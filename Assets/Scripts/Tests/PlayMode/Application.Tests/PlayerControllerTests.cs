using Game.Application;
using Game.Core;
using Game.Input;
using Game.Input.Commands;
using Game.Services.Application;
using NSubstitute;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerControllerTests : MonoBehaviour
{

    PlayerController _controller;
    IInputService _inputService;
    IPlayerApplicationService _playerApplicationService;
    InputCommand moveCmd;

    [SetUp]
    public void Setup()
    {
        Bootstrapper.InitializeForTests();//No queremos arrancar el bootstrapper completo, lo configuramos para cada test
        _inputService = Substitute.For<IInputService>();
        _playerApplicationService = Substitute.For<IPlayerApplicationService>();
        Bootstrapper.Container.RegisterSingleton<IInputService>(_inputService);
        Bootstrapper.Container.RegisterSingleton<IPlayerApplicationService>(_playerApplicationService);

        var go = new GameObject("PlayerControllerTestObj");
        _controller = go.AddComponent<PlayerController>();
        _controller.enabled = false;//no queremos que ejecute update
    }

    [UnityTest]
    public IEnumerator PlayerController_WhenReceiveCommand_ThenTryDequeueAndProcess()
    {
        moveCmd = new MovementCommand(Vector2.right, 1f);
        InputCommand dummy = null;
        _inputService.TryDequeue(out dummy)
        .Returns(ci =>
        {
            ci[0] = moveCmd;
            return true;
        });

        _controller.Tick(1f); 

        _playerApplicationService.Received(1).ProcessInputCommands(moveCmd, Arg.Any<float>());
        _playerApplicationService.Received(1).Update(Arg.Any<float>());
        yield break;
    }


    // la cola esta vacia y no procesamos pero si se hace update
    [UnityTest]
    public IEnumerator PlayerController_WhenEmptyCommand_ThenNotDequeueButUpdate()
    {
        _controller.Tick(1f);
        
        _playerApplicationService.Received(0).ProcessInputCommands(Arg.Any<InputCommand>(), Arg.Any<float>());
        _playerApplicationService.Received(1).Update(Arg.Any<float>());
        yield break;
    }
}
