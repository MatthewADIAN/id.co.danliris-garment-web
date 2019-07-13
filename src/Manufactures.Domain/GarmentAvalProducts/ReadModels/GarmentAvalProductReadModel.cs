﻿using Infrastructure.Domain.ReadModels;
using System;
using System.Collections.Generic;

namespace Manufactures.Domain.GarmentAvalProducts.ReadModels
{
    public class GarmentAvalProductReadModel : ReadModelBase
    {
        public GarmentAvalProductReadModel(Guid identity) : base(identity)
        {

        }

        public string RONo { get; internal set; }
        public string Article { get; internal set; }
        public DateTimeOffset? AvalDate { get; internal set; }
        public virtual List<GarmentAvalProductItemReadModel> GarmentAvalProductItem { get; internal set; }
    }
}