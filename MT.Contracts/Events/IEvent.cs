namespace MT.Contracts.Events
{
    public interface IEvent
    {
        /// <summary>
        /// Id of an event
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Utc event created date
        /// </summary>
        DateTime CreatedAt { get; }

        /// <summary>
        /// Source entity/service that generated event
        /// </summary>
        string Source { get; }
    }
}
