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
                    foreach (Post currentPost in myDeserializedClass.posts)
                    {
                        if (currentPost.tim is not null)
                        {
                            currentPost.imageThumbUrl = $"{"https://i.4cdn.org"}/{boardName}/{currentPost.tim}s.jpg";
                            currentPost.imageUrl = $"{"https://i.4cdn.org"}/{boardName}/{currentPost.tim}{currentPost.ext}";
                        }
                        else
                        {
                            currentPost.imageThumbUrl = null;
                            currentPost.imageUrl = null;
                        }

                        currentPost.RepliesPosts = new List<Post>();
                        currentPost.QuotesPosts = new List<Post>();

                        // Replies down
                        foreach (Post otherPost in myDeserializedClass.posts)
                        {
                            if (!otherPost.Equals(currentPost))
                            {
                                // Replies Going Down
                                if (otherPost.com is not null)
                                {
                                    // otherpost contains current p number
                                    if (otherPost.com.Contains(currentPost.no.ToString()))
                                    {
                                        currentPost.RepliesPosts.Add(otherPost);
                                    }

                                    // otherpost contains repliy to currentPost
                                    //string currentPostContent = currentPost.com;
                                    //string comparePostNo = otherPost.no.ToString();

                                    //if (currentPostContent.Contains(comparePostNo))
                                    //{
                                    //    currentPost.QuotesPosts.Add(otherPost);
                                    //}
                                    //currentPost.QuotesPosts.Add(otherPost);
                                }
                            }
                        }
                    }

                    foreach (Post currentPost in myDeserializedClass.posts)
                    {
                        if (currentPost.com is not null)
                        {
                            // if current post content contains quotes to others posts
                            foreach (Post otherPost in myDeserializedClass.posts)
                            {
                                string currentPostContent = currentPost.com;
                                string comparePostNo = otherPost.no.ToString();
                                if (currentPostContent.Contains(comparePostNo) && !currentPost.Equals(otherPost))
                                {
                                    currentPost.QuotesPosts.Add(otherPost);
                                }
                                currentPost.QuotesPosts.Add(otherPost);
                            }
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
    }
}
