﻿using Qowaiv.Validation.Abstractions;
using System.Collections.Generic;

namespace Qowaiv.DomainModel
{
    /// <summary>Represents an (domain-driven design) aggregate root that is based on event sourcing.</summary>
    /// <typeparam name="TAggregate">
    /// The type of the aggregate root itself.
    /// </typeparam>
    /// <typeparam name="TId">
    /// The type of the identifier.
    /// </typeparam>
    public class AggregateRoot<TAggregate, TId> : AggregateRoot<TAggregate>
        where TAggregate : AggregateRoot<TAggregate>
    {
        /// <summary>Initializes a new instance of the <see cref="AggregateRoot{TAggregate, TId}"/> class.</summary>
        /// <param name="validator">
        /// A custom <paramref name="validator"/> to validate the aggregate.
        /// </param>
        /// <param name="aggregateId">
        /// The identifier of the aggregate.
        /// </param>
        protected AggregateRoot(TId aggregateId, IValidator<TAggregate> validator) : base(validator)
        {
            Buffer = new EventBuffer<TId>(aggregateId);
        }

        /// <summary>Gets the identifier.</summary>
        public TId Id => Buffer.AggregateId;

        /// <summary>Gets the version of aggregate root.</summary>
        public int Version => Buffer.Version;

        /// <summary>Gets the buffer with the recently added events.</summary>
        public EventBuffer<TId> Buffer { get; protected set; }

        /// <inheritdoc/>
        protected override void AddEventsToBuffer(IEnumerable<object> events)
            => Buffer = Buffer.Add(events);

        /// <summary>Loads the state of the aggregate root by replaying events.</summary>
        internal void Replay(EventBuffer<TId> eventBuffer)
        {
            Buffer = new EventBuffer<TId>(eventBuffer.AggregateId, eventBuffer.CommittedVersion);
            Replay(eventBuffer.Committed);
        }
    }
}
