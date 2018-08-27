﻿using System;
using Nest;

namespace Contracts
{
    public class Stiftungsrat
    {
        public Stiftungsrat()
        {
            this.id = Guid.NewGuid();
            this.timestamp = DateTime.Now;
        }

        public Guid id { get; set; }

        public string name { get; set; }

        public string nameFull { get; set; }

        public string kanton { get; set; }

        public Guid? stiftungId { get; set; }

        public string stiftungName { get; set; }

        [Date(Name = "@timestamp")]
        public DateTime timestamp { get; set; }
    }
}
