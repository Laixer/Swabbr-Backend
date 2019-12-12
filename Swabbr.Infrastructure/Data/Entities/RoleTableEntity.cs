﻿using Microsoft.Azure.Cosmos.Table;
using System;

namespace Swabbr.Infrastructure.Data.Entities
{
    public class RoleTableEntity : TableEntity
    {
        public Guid RoleId { get; set; }

        public string Name { get; set; }

        public string NormalizedName { get; set; }
    }
}
