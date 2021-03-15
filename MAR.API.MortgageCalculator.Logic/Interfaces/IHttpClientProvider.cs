using System.Net.Http;
using System.Threading.Tasks;

namespace MAR.API.MortgageCalculator.Logic.Interfaces
{
    public interface IHttpClientProvider
    {
        HttpClient HttpClient { get; set; }
        Task<string> GetAsync(string url);
    }
}
