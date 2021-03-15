using Xunit.Abstractions;

namespace MAR.API.MortgageCalculator.UnitTests
{
    public class ProviderBaseTestClass : AllTestsBaseClass
    {
        public ProviderBaseTestClass(ITestOutputHelper consoleLogger)
            : base(consoleLogger)
        {
        }
    }
}
