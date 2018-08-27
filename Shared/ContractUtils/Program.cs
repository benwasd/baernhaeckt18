using ContractUtils;

namespace ConsoleApp1
{
    class Program
    {
        static void Main()
        {
            //new SampleData().SeedSamples();
            //new TagIndex().FillTagsByStiftungen();
            new SizungsratIndex().FillSizungsraeteByStiftungen();
        }
    }
}
