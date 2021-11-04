using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Valkyrie.Utils
{
    public static class WebHelper
    {
        public static async Task<string> LoadHtmlAsync(Uri url)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }

        public static async Task<string> LoadHtmlAsync(string url)
        {
            return await LoadHtmlAsync(new Uri(url));
        }
    }
}
