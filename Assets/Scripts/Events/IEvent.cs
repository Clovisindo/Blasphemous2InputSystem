namespace Game.Events
{
        public interface IEvent { }
        public interface IDomainEvent : IEvent { }
        public interface IApplicationEvent : IEvent { }
}
