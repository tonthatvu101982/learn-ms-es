﻿using CQRS.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.Core.Domains;

public interface IEventStoreRepository
{
    Task SaveAsync(EventModel @event);

    Task<List<EventModel>> FindByAggregateId(Guid aggregateId);
}
