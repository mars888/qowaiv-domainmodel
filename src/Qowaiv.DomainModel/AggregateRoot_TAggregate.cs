﻿using Qowaiv.DomainModel.Dynamic;
using Qowaiv.DomainModel.Collections;
using Qowaiv.Validation.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Qowaiv.DomainModel
{
    /// <summary>Represents an (domain-driven design) aggregate root that is based on event sourcing.</summary>
    /// <typeparam name="TAggregate">
    /// The type of the aggregate root itself.
    /// </typeparam>
    public abstract class AggregateRoot<TAggregate>
        where TAggregate : AggregateRoot<TAggregate>
    {
        /// <summary>Initializes a new instance of the <see cref="AggregateRoot{TAggregate}"/> class.</summary>
        /// <param name="validator">
        /// A custom <paramref name="validator"/> to validate the aggregate.
        /// </param>
        protected AggregateRoot(IValidator<TAggregate> validator)
        {
            Validator = Guard.NotNull(validator, nameof(validator));
            Dynamic = new DynamicEventDispatcher<TAggregate>((TAggregate)this);
        }

        /// <summary>The validator that ensures that after applying events the
        /// aggregate is still valid.
        /// </summary>
        protected IValidator<TAggregate> Validator { get; }

        /// <summary>Gets an <see cref="ImmutableCollection.Empty"/> collection.</summary>
        protected static ImmutableCollection Events => ImmutableCollection.Empty;

        /// <summary>Represents the aggregate root as a dynamic.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected virtual dynamic Dynamic { get; }

        /// <summary>Adds the events to the linked event buffer.</summary>
        /// <param name="events">
        /// The events to add to the event buffer.
        /// </param>
        /// <remarks>
        /// This method is only called if after applying the events, the aggregate
        /// is still valid.
        /// </remarks>
        protected abstract void AddEventsToBuffer(IEnumerable<object> events);

        /// <summary>Applies a single event.</summary>
        protected Result<TAggregate> ApplyEvent(object @event) => ApplyEvents(@event);

        /// <summary>Applies the events.</summary>
        protected Result<TAggregate> ApplyEvents(params object[] events)
            => Apply(events);

        /// <summary>Applies the events.</summary>
        protected Result<TAggregate> Apply(IEnumerable<object> events)
        {
            Guard.HasAny(events, nameof(events));

            lock (locker)
            {
                foreach (var @event in events)
                {
                    Dynamic.When(@event);
                }

                var result = Validator.Validate((TAggregate)this);

                if (result.IsValid)
                {
                    AddEventsToBuffer(events);
                }
                return result;
            }
        }

        /// <summary>Loads the state of the aggregate root by replaying events.</summary>
        protected void Replay(IEnumerable<object> events)
        {
            Guard.NotNull(events, nameof(events));

            lock (locker)
            {
                foreach (var @event in events)
                {
                    Dynamic.When(@event);
                }
            }
        }

        /// <summary>To be thread-safe, we lock.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly object locker = new object();
    }
}
