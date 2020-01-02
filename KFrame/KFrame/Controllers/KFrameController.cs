using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;

namespace KFrame.Controllers
{
    public class KFrameController : ApiController
    {
        private const String SRC_PARTTERN = "src=\"";
        private const String HREF_PARTERN = "href=\"";
        private const String DATA_IMAGE = "data-img=\"";
        private const String DATA_BG = "data-background=\"";
        private const String SRCSET_PARTERN = "srcset=\"";
        // GET: kframe/url
        public async System.Threading.Tasks.Task<HttpResponseMessage> GetAsync(String URL)
        {
            String mURL = URL;
            String data = String.Empty;
            try
            {
                using (var wb = new WebClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    wb.UseDefaultCredentials = true;
                    wb.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                    var mresponse = wb.DownloadData(mURL);
                    var htmlCode = Encoding.UTF8.GetString(mresponse);
                    data = htmlCode.ToString();           
                    if(data.ToLower().IndexOf("html") == -1)
                    {
                        var client = new RestClient(mURL);
                        var request = new RestRequest(Method.GET);
                        IRestResponse mresponse1 = client.Execute(request);
                        data = mresponse1.Content;
                    }
                }
            }
            catch
            {
                //analysiswithHttpClientAsync();
                var client = new RestClient(mURL);
                var request = new RestRequest(Method.GET);
                IRestResponse mresponse = client.Execute(request);
                data = mresponse.Content;
            }
            data = replaceTagContent(SRC_PARTTERN, data, mURL);
            data = replaceTagContent(HREF_PARTERN, data, mURL);
            data = replaceTagContent(DATA_IMAGE, data, mURL);
            data = replaceTagContent(DATA_BG, data, mURL);
            data = replaceTagContent(SRCSET_PARTERN, data, mURL);

            var response = new HttpResponseMessage();
            response.Content = new StringContent(data);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        private String replaceTagContent(String mSrcPattern, String orgContent, String concatPattern)
        {
            //progress - pattern
            if (concatPattern.EndsWith("/"))
                concatPattern = concatPattern.Substring(0, concatPattern.Length - 1);

            int orgContentLen = orgContent.Length;
            String newContent = String.Empty;
            String srcPattern = mSrcPattern/*"src=\""*/;
            //include https b'c https start with http. Exclude
            String httpPartern = "http";
            while (orgContent.Length > 0)
            {
                int indexOfSrc = orgContent.IndexOf(srcPattern);
                //found index
                if (indexOfSrc != -1)
                {
                    String subContent = orgContent.Substring(0, indexOfSrc + srcPattern.Length);
                    newContent = newContent + subContent;
                    orgContent = orgContent.Substring(subContent.Length, orgContent.Length - subContent.Length);
                    if (!orgContent.StartsWith(httpPartern))
                    {
                        if (!orgContent.StartsWith("//"))
                        {
                            if (!orgContent.StartsWith("/"))
                            {
                                orgContent = "/" + orgContent;
                            }
                            orgContent = concatPattern + orgContent;
                        }
                        else
                        {
                            orgContent = "https:" + orgContent;
                        }
                    }
                }
                else
                {
                    //Cannot found anyindex
                    newContent = newContent + orgContent;
                    orgContent = String.Empty;
                }
            }
            return newContent;
        }



    }
}
