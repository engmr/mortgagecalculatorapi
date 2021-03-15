using MAR.API.MortgageCalculator.Localization;
using Microsoft.Extensions.Localization;
using Moq;
using System.Collections.Generic;
using Xunit.Abstractions;

namespace MAR.API.MortgageCalculator.UnitTests
{
    public class AllTestsBaseClass
    {
        protected ITestOutputHelper ConsoleLogger;
        protected Mock<IStringLocalizer<ErrorMessages>> MockErrorMessagesLocalizer;
        protected Mock<IStringLocalizer<ValidationMessages>> MockValidationMessagesLocalizer;

        protected Dictionary<string, string> ErrorMessagesResxDictionary;
        protected Dictionary<string, string> ValidationMessagesResxDictionary;

        public AllTestsBaseClass(ITestOutputHelper consoleLogger)
        {
            ConsoleLogger = consoleLogger;
            MockErrorMessagesLocalizer = new Mock<IStringLocalizer<ErrorMessages>>();
            MockValidationMessagesLocalizer = new Mock<IStringLocalizer<ValidationMessages>>();

            SetupDefaultMockErrorMessagesLocalizer();
            SetupDefaultMockValidationMessagesLocalizer();
        }

        public void SetupDefaultMockErrorMessagesLocalizer()
        {
            //Reference MAR.API.MortgageCalculator.Localization ErrorMessages.cs (en-US) resx
            ErrorMessagesResxDictionary = new Dictionary<string, string>()
            {
                {"MortgageCalculationRequestNotSupported", "Mortgage calculation request is not supported." },
                {"MortgageCalculationRequestTypeNotSupported", "Mortgage calculation request type SomeFakeType is not supported." },
                {"MortgageInfoTableHadNoData", "Mortgage info table had no data in it." },
                {"MortgageInfoTableMissing", "Mortgage info table was not found." },
            };
            foreach (var kvp in ErrorMessagesResxDictionary)
            {
                MockErrorMessagesLocalizer.Setup(_ => _[kvp.Key]).Returns(new LocalizedString(kvp.Key, kvp.Value));
            }
        }

        public void SetupDefaultMockValidationMessagesLocalizer()
        {
            //Reference MAR.API.MortgageCalculator.Localization ValidationMessages.cs (en-US) resx
            ValidationMessagesResxDictionary = new Dictionary<string, string>()
            {
                {"APRIsInvalid", "APR must be greater than or equal to 0." },
            };
            foreach (var kvp in ValidationMessagesResxDictionary)
            {
                MockValidationMessagesLocalizer.Setup(_ => _[kvp.Key]).Returns(new LocalizedString(kvp.Key, kvp.Value));
            }
        }
    }
}
