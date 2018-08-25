using System;

namespace Contracts
{
    public class Stiftung
    {
        public Stiftung()
        {
            this.id = Guid.NewGuid();
        }

        public Guid id { get; set; }

        public string name { get; set; }

        public string zweck { get; set; }

        public string[] tags { get; set; }

        public string adresse { get; set; }

        public string jahresbericht { get; set; }

        public decimal bilanzsumme { get; set; }

        public string handelsregisterUID { get; set; }

        public string handelsregisterCHNR { get; set; }

        public string handelsregisterAmt { get; set; }

        public string url { get; set; }
    }
}
