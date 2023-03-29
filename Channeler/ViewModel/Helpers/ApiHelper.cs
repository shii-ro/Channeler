using Channeler.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Channeler.ViewModel.Helpers
{
    public class ApiHelper
    {
        private static readonly HttpClient client = new HttpClient();
        private const string BASE_URL = "https://a.4cdn.org/";
        private const string BOARDS_ENDPOINT = "boards.json";
        private const string CATALOG_ENDPOINT = "/catalog.json";


        public static async Task<BoardList?> GetBoards()
        {
            BoardList boards = new BoardList();

            string url = $"{BASE_URL}{BOARDS_ENDPOINT}";

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<BoardList>(content);
            }

            return null;
        }

        public static async Task<List<BoardPage>?> GetBoardCatalog(string boardName)
        {
            string url = $"{BASE_URL}{boardName}{CATALOG_ENDPOINT}";

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var myDeserializedClass = JsonConvert.DeserializeObject<List<BoardPage>>(content);

                foreach (BoardPage bp in myDeserializedClass)
                {
                    foreach (Thread t in bp.threads)
                    {
                        t.imageThumbUrl = $"{"https://i.4cdn.org"}/{boardName}/{t.tim}s.jpg";
                        t.imageUrl = $"{"https://i.4cdn.org"}/{boardName}/{t.tim}{t.ext}";
                    }
                }
                return myDeserializedClass;
            }

            return null;
        }

        public static async Task<ThreadPosts?> GetThreadPosts(string boardName, string opId)
        {

            string url = $"{BASE_URL}{boardName}/thread/{opId}.json";

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await client.GetAsync(url).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var myDeserializedClass = JsonConvert.DeserializeObject<ThreadPosts>(content);

                /* get the thumbs and images url */
                foreach (Post currentPost in myDeserializedClass.posts)
                {
                    if (currentPost.tim is not null)
                    {
                        currentPost.imageThumbUrl = $"{"https://i.4cdn.org"}/{boardName}/{currentPost.tim}s.jpg";
                        currentPost.imageUrl = $"{"https://i.4cdn.org"}/{boardName}/{currentPost.tim}{currentPost.ext}";
                    }

                    // add the replies and quotes
                    foreach (Post otherPost in myDeserializedClass.posts)
                    {
                        if (otherPost.com is not null)
                            if (otherPost.com.Contains(currentPost.no.ToString()))
                            {
                                currentPost.RepliesPosts.Add(otherPost);
                                otherPost.QuotesPosts.Add(currentPost);
                            }
                    }

                }
                return myDeserializedClass;
            }
            return null;
        }
    }
}
