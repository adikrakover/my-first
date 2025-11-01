using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lesson2
{
    internal class WebClient
    {

        public async Task<string> Load(string url)
        {
            HttpClient client = new HttpClient();
            // ודאי שאת מטפלת בשגיאות רשת פוטנציאליות
            var response = await client.GetAsync(url);
            //response.EnsureSuccessStatusCode();
            var html = await response.Content.ReadAsStringAsync();
            return html;
        }
    }
}
