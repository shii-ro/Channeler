using Channeler.Model;
using Channeler.View;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace Channeler.ViewModel.Helpers
{
    internal class ApiHelper
    {
        private const string BASE_URL = "https://a.4cdn.org/";
        private const string BOARDS_ENDPOINT = "boards.json";
        private const string CATALOG_ENDPOINT = "/catalog.json";


        public static BoardList GetBoards()
        {
            BoardList boards = new BoardList();

            string url = BASE_URL + BOARDS_ENDPOINT;

            using (HttpClient client = new HttpClient())
            {
                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync(url).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content;
                    BoardList myDeserializedClass = JsonConvert.DeserializeObject<BoardList>(response.Content.ReadAsStringAsync().Result);
                    boards = myDeserializedClass;
                }
                else
                {
                    // Show error in the window later
                    // Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);

                }
            }
            return boards;
        }
        public static async Task<List<BoardPage>> GetBoardCatalog(string boardName)
        {
            string url = BASE_URL + boardName + CATALOG_ENDPOINT;

            using (HttpClient client = new HttpClient())
            {
                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(url);  // Blocking call! Program will wait here until a response is received or a timeout occurs.
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
                else
                {
                    return null;
                }
            }
        }
        public static async Task<ThreadPosts> GetThreadPosts(string boardName, string opId)
        {

            string url = BASE_URL + boardName + "/thread/" + opId + ".json";

            using (HttpClient client = new HttpClient())
            {
                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var myDeserializedClass = JsonConvert.DeserializeObject<ThreadPosts>(content);

                    /* get the thumbs and images url */
                    /* put the replies */
                    foreach (Post p in myDeserializedClass.posts)
                    {
                        if (p.tim is not null)
                        {
                            p.imageThumbUrl = $"{"https://i.4cdn.org"}/{boardName}/{p.tim}s.jpg";
                            p.imageUrl = $"{"https://i.4cdn.org"}/{boardName}/{p.tim}{p.ext}";
                        }
                        else
                        {
                            p.imageThumbUrl = null;
                            p.imageUrl = null;
                        }
                        p.RepliesPosts = new List<RepliesPosts>();

                        myDeserializedClass.posts.ForEach(post =>
                        {
                            if (!post.Equals(p))
                            {
                                if (post.com is not null)
                                    if (post.com.Contains(p.no.ToString()))
                                    {
                                        p.RepliesPosts.Add(new RepliesPosts { replyNo = post.no, replyPost = post });
                                    }
                            }
                        });
                    }
                    return myDeserializedClass;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
