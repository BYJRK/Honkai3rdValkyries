using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Valkyrie.Utils
{
    public static class WebHelper
    {
        public static async Task<string> LoadHtml(string url)
        {
            HttpClient client = new HttpClient();
            using (var response = await client.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                    return response.Content.ReadAsStringAsync().Result;
                else
                    throw new Exception(response.ReasonPhrase);
            }
        }
    }
}
