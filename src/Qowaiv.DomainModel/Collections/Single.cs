﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Qowaiv.DomainModel.Collections
{
    /// <summary>Represents a read-only collection of events.</summary>
    public partial class ImmutableCollection
    {
        /// <summary><see cref="ImmutableCollection"/> implementation for containing a single of item.</summary>
        private class Single : NotEmpty
        {
            /// <summary>Initializes a new instance of the <see cref="Single"/> class.</summary>
            public Single(object item, ImmutableCollection predecessor)
                : base(predecessor) => Item = item;

            /// <summary>Item placeholder.</summary>
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private object Item { get; }

            /// <inheritdoc />
            internal override IEnumerable<object> Enumerate()
                => base.Enumerate().Append(Item);
        }
    }
}
