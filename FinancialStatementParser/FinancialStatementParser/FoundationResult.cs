using System;

namespace FinancialStatementParser
{
    public class FoundationResult
    {
        public bool Success { get; set; }

        public decimal BalanceSheetTotal { get; set; }

        public Uri FinancialStatementUrl { get; set; }
    }
}
