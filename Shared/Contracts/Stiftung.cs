﻿using System;
using Nest;

namespace Contracts
{
    public class Stiftung
    {
        public Stiftung()
        {
            this.id = Guid.NewGuid();
            this.timestamp = DateTime.Now;
        }

        public Guid id { get; set; }

        public string sourceId { get; set; }

        public string name { get; set; }

        public string nameshort { get; set; }

        public string zweck { get; set; }

        public string[] tags { get; set; }

        public string adresse { get; set; }

        public string jahresbericht { get; set; }

        public decimal bilanzsumme { get; set; }

        public decimal? bilanzsumme26 { get; set; }

        public string handelsregisterUID { get; set; }

        public string handelsregisterCHNR { get; set; }

        public string handelsregisterAmt { get; set; }

        public string kanton { get; set; }

        public string url { get; set; }

        public Stiftungsratsmitglied[] stiftungsratsmitglieder { get; set; }

        [Date(Name = "@timestamp")]
        public DateTime timestamp { get; set; }
    }

    public class Stiftungsratsmitglied
    {
        public string name { get; set; }

        public string funktion { get; set; }

        public string berechtigung { get; set; }
    }
}
