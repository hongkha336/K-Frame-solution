//khaDNH crate 30/12/2019
/*
 * Description: The demo tool have a problem that we cannot get the Title 
 */ 
using RestSharp;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Web.Http.Results;

namespace KFrame.Controllers
{
    public class KTitleController : ApiController
    {

        // GET: /ktitle?url=
        public JsonResult<String> Get(String url)
        {
            String mURL = url;
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
                    if (data.ToLower().IndexOf("html") == -1)
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
            //var response = new HttpResponseMessage();
            //response.Content = new StringContent(getTitle(data));
            //response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");


            //return response;
            ////return Newtonsoft.Json.JsonConvert.SerializeObject(getTitle(data));

            return Json(getTitle(data));
        }

 

        private const String TITLE_TAG = "<title>";
        private const String TITLE_TAG_END = "</title>";
       /// <summary>
       /// Get the Title form the response
       /// </summary>
       /// <param name="res"></param>
       /// <returns></returns>
        private String getTitle(String res)
        {
            String _bkres = res;
            String _lowcaseResource = res.ToLower();
            int tagIndex = _lowcaseResource.IndexOf(TITLE_TAG);
            String title = String.Empty;
            if(tagIndex != -1)
            {
                int endTagIndex = _lowcaseResource.IndexOf(TITLE_TAG_END);
                title = _bkres.Substring(0, endTagIndex);
                title = title.Substring(tagIndex + TITLE_TAG.Length, title.Length - tagIndex - TITLE_TAG.Length);
            }
            if (title.Equals(String.Empty))
            {
                title = "Demo page";
            }
            return title;
        }


      
    }
}
