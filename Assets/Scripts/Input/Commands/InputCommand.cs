namespace Game.Input.Commands
{
    public abstract class InputCommand
    {
        public readonly float Timestamp;
        protected InputCommand(float timestamp) => Timestamp = timestamp;
    }
}
