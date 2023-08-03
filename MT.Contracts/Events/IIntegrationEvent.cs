namespace MT.Contracts.Events
{
    [Obsolete("Use IVersionedIntegrationEvent instead for new events")]
    public interface IIntegrationEvent : IEvent
    {

    }
    
    public interface IVersionedIntegrationEvent :  IIntegrationEvent, IVersionedEvent
    {
        string SourceId { get; }
    }
}
