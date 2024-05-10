using HtmlAgilityPack;
using ItemFinder_WPF.Backend.Enums;
using ItemFinder_WPF.Backend.Objects;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

namespace ItemFinder_WPF.Backend
{
    public static class NativeMethods
    {
        [DllImport("WinInet.dll", PreserveSig = true, SetLastError = true)]
        public static extern void DeleteUrlCacheEntry(string url);
    }

    public class Scraper
    {
        private string _keywords;
        private bool _searchInTitle;
        private uint _numberOfPages;
        private uint _msAverageDelayBetweenRequests;
        private Proxy _proxy;

        public Scraper(string keywords, bool searchInTitle , uint numberOfPages, uint msAverageDelayBetweenRequests, Proxy proxy = null)
        {
            _keywords = keywords;
            _searchInTitle = searchInTitle;
            _numberOfPages = numberOfPages;
            _msAverageDelayBetweenRequests = msAverageDelayBetweenRequests;
            _proxy = proxy;
        }

        private string _buildLBCUrl(uint page)
        {
            UriBuilder uriBuilder = new UriBuilder()
            {
                Scheme = "https",
                Host = "leboncoin.fr",
                Path = "recherche"
            };

            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query["category"] = "14";
            query["text"] = _keywords;


            if (_searchInTitle)
            {
                query["search_in"] = "subject";
            }

            query["sort"] = "time";


            query["page"] = page.ToString();
            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }

        public string _getRandomUserAgent()
        {
            var lines = File.ReadAllLines("UserAgents.txt");
            var r = new Random();
            var randomLineNumber = r.Next(0, lines.Length - 1);
            var line = lines[randomLineNumber];

            return line;
        }

        private string _getHtmlFromPage(uint page)
        {
            NativeMethods.DeleteUrlCacheEntry("https://leboncoin.fr");
            NativeMethods.DeleteUrlCacheEntry("https://www.leboncoin.fr");
            string url = _buildLBCUrl(page);
            NativeMethods.DeleteUrlCacheEntry(url);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpRequestCachePolicy policy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Default);
            HttpWebRequest.DefaultCachePolicy = policy;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url, UriKind.Absolute));

            CookieContainer sessionCookie = new CookieContainer();
            //sessionCookie.Add(new Cookie("datadome", "4LmsSN5ZYI7vUalWF7dehNk3iMN3Z3nIvCtUyRPt2BoAxzQWolIqaO6Qdk8qats7pCs3qXSwkyx0YX4xcX0pc4fkgZDeUM0WVV08_su-r~0PTHpzdLFhydiP-ck10p8D") { Domain = "leboncoin.fr" });
            request.CookieContainer = sessionCookie;
            HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            request.CachePolicy = noCachePolicy;

            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36 OPR/94.0.0.0";
            request.Headers.Add("Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            request.Headers.Add("accept-language: fr-FR,fr;q=0.9,en-US;q=0.8,en;q=0.7");
            request.Headers.Add(HttpRequestHeader.CacheControl, "max-age=0");
            request.Host = "www.leboncoin.fr";
            request.KeepAlive= true;


            if (_proxy!= null)
            {
                request.Proxy = _proxy.GetWebProxy();
            }


            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string html = new StreamReader(response.GetResponseStream()).ReadToEnd();
            
            return html;
        }

        private bool _scrapeValidation(params HtmlNode[] htmlNodes)
        {
            bool isValidated = true;

            foreach(HtmlNode node in htmlNodes)
            {
                if(node == null)
                {
                    isValidated = false;
                }
            }

            return isValidated;
        }

        private string getAttributeFromNode(HtmlNode node, string attributeName)
        {
            return HttpUtility.HtmlDecode(node.Attributes[attributeName].Value);
        }

        public List<Ad> _scrapeFromPage(uint page)
        {
            var ads = new List<Ad>();

            HtmlDocument htmlDocument = new HtmlDocument();
            string html = _getHtmlFromPage(page);
            htmlDocument.LoadHtml(html);

            HtmlNodeCollection scrapedAds = htmlDocument.DocumentNode.SelectNodes("//a[@data-qa-id='aditem_container']");
            HtmlNode scrapedScript = htmlDocument.DocumentNode.SelectSingleNode("//script[@id='__NEXT_DATA__']");

            List<string> imageUrlList = new List<string>();
            JToken myDeserializedClass = JsonConvert.DeserializeObject<JToken>(scrapedScript.InnerHtml);

            for (int i = 0; i < myDeserializedClass["props"]["pageProps"]["searchData"]["ads"].Count(); i++)
            {
                imageUrlList.Add(myDeserializedClass["props"]["pageProps"]["searchData"]["ads"][i]["url"].ToString().Replace("https://www.leboncoin.fr", ""));
            }

            foreach (HtmlNode scrapedAd in scrapedAds)
            {
                UnparsedAd unparsedAd = new UnparsedAd();
                Ad ad = new Ad();

                HtmlNode htmlAdName = scrapedAd.SelectSingleNode(".//p[@data-qa-id='aditem_title']");
                HtmlNode htmlAdPrice = scrapedAd.SelectSingleNode(".//p[@data-test-id='price']");
                HtmlNode htmlAdState = scrapedAd.SelectSingleNode(".//p[@data-test-id='price']/following::div[1]/span");
                HtmlNode htmlAdDelivery = scrapedAd.SelectSingleNode(".//div[@display='flex']/div[@color='black']");
                HtmlNode htmlAdLocation = scrapedAd.SelectSingleNode(".//div[@display='flex']/div/p/span/span[1]");
                HtmlNode htmlAdDate = scrapedAd.SelectSingleNode(".//div[@display='flex']/div/p/span/span[2]");
                HtmlNode htmlAdSellerName = scrapedAd.SelectSingleNode("./div[@display='flex']/span");
                HtmlNode htmlAdReviewCount = scrapedAd.SelectSingleNode("./div[@display='flex']/div/span");
                HtmlNode htmlAdReviewGrade = scrapedAd.SelectSingleNode("./div[@display='flex']/div/div/div[2]/span");

                bool nodesValidation = _scrapeValidation(htmlAdName, htmlAdPrice, htmlAdState, htmlAdLocation, htmlAdDate, htmlAdSellerName);

                if (!nodesValidation) { continue; }

                unparsedAd.Name = getAttributeFromNode(htmlAdName, "title");
                unparsedAd.Price = getAttributeFromNode(htmlAdPrice, "aria-label");
                unparsedAd.State = HttpUtility.HtmlDecode(htmlAdState.InnerText);
                unparsedAd.IsDeliveryAvailable = string.Empty;
                unparsedAd.Url = getAttributeFromNode(scrapedAd, "href");
                unparsedAd.Location = HttpUtility.HtmlDecode(htmlAdLocation.InnerText);
                unparsedAd.SellerName = HttpUtility.HtmlDecode(htmlAdSellerName.InnerText);
                unparsedAd.SellerReviewCount = string.Empty;
                unparsedAd.SellerReviewGrade = string.Empty;
                unparsedAd.ImageUrl = null;

                int currentParsedJsonIndex = imageUrlList.IndexOf(unparsedAd.Url);
                if (currentParsedJsonIndex != -1)
                {
                    if (myDeserializedClass["props"]["pageProps"]["searchData"]["ads"][currentParsedJsonIndex]["images"]["small_url"] != null)
                    {
                        unparsedAd.ImageUrl = myDeserializedClass["props"]["pageProps"]["searchData"]["ads"][currentParsedJsonIndex]["images"]["small_url"].ToString();

                    }

                    if (myDeserializedClass["props"]["pageProps"]["searchData"]["ads"][currentParsedJsonIndex]["index_date"] != null)
                    {
                        unparsedAd.Date = myDeserializedClass["props"]["pageProps"]["searchData"]["ads"][currentParsedJsonIndex]["index_date"].ToString();

                    }

                }

                if (htmlAdDelivery != null)
                {
                    unparsedAd.IsDeliveryAvailable = HttpUtility.HtmlDecode(htmlAdDelivery.InnerText);
                }

                if (htmlAdReviewCount != null)
                {
                    unparsedAd.SellerReviewCount = HttpUtility.HtmlDecode(htmlAdReviewCount.InnerText);
                }

                if (htmlAdReviewGrade != null)
                {
                    unparsedAd.SellerReviewGrade = HttpUtility.HtmlDecode(htmlAdReviewGrade.InnerText);
                }

                Parser parser = new Parser(unparsedAd);
                ad = parser.parseAd();
                ads.Add(ad);
            }

            return ads;
        }

        public List<Ad> scrape()
        {
            var ads = new List<Ad>();
            Random rnd = new Random();

            for(uint page = 1; page <= _numberOfPages; page++)
            {
                int minDelay = Convert.ToInt32(_msAverageDelayBetweenRequests - 0.5 * _msAverageDelayBetweenRequests);
                int maxDelay = Convert.ToInt32(_msAverageDelayBetweenRequests + 0.5 * _msAverageDelayBetweenRequests);

                int delay = rnd.Next(minDelay, maxDelay);
                ads.AddRange(_scrapeFromPage(page));
                Thread.Sleep(delay);
            }

            return ads;
        }
    }
}
