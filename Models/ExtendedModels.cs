using Radzen;

namespace Re.Models
{
    public class ExtendedQuery : Query
    {
        public string Calculate { get; set; }
    }

    public class RealEstateDBServiceResult<T>
    {
        public IEnumerable<T> Value { get; set; }
        public int Count { get; set; }
        public Calculations Calculations { get; set; }
    }

    public class Calculations
    {
        public IEnumerable<string> Titles { get; set; }
        public int MaxBedrooms { get; set; }
        public int MaxBathrooms { get; set; }
        public int MaxGarages { get; set; }
        public int[] MinMaxSize { get; set; }
        public decimal[] MinMaxPrice { get; set; }
        public IEnumerable<AgentDeal> AgentDeals { get; set; }
        public IEnumerable<DealsType> DealTypes { get; set; }
        public IEnumerable<DealByMonth> DealsByMonth { get; set; }
    }

    public class AgentDeal
    {
        public string PropertyType { get; set; }
        public int Deals { get; set; }
    }

    public class DealsType
    {
        public string DealType { get; set; }
        public int Deals { get; set; }
    }

    public class DealByMonth
    {
        public string Month { get; set; }
        public int Deals { get; set; }
    }
}