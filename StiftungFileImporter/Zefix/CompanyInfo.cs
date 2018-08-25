namespace Zefix
{
    public class CompanyInfo
    {
        public string LegalSeatId { get; set; }
        public string RegisterOfficeId { get; set; }
        public string Uid { get; set; }
        public string ChId { get; set; }
        public string Canton { get; set; }

        public string CantonIso
        {
            get
            {
                return CantonMapper.CantonToCantonIso(Canton);
            }
        }
    }
}