using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Valkyria
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
