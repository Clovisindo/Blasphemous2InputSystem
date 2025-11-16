using Game.Input.Commands;
using System.Collections.Generic;

namespace Game.Services.Application
{
    public interface IPlayerApplicationService
    {
        void ProcessInputCommands(InputCommand command, float deltaTime);
        void Update(float deltaTime);
    }
}
