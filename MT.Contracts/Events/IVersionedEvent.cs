namespace MT.Contracts.Events
{
    public interface IVersionedEvent : IEvent
    {
        long Version { get; }
    }
}
