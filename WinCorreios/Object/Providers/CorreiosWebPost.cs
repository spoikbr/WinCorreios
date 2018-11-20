using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WinCorreios.Object.Providers
{
    public static class CorreiosWebPost
    {
        private static readonly HttpClient client = new HttpClient();
        public static async Task<XmlDocument> GetEvents(string TrackingCode)
        {
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml"));
            string requeststr =
                String.Format
                ("<rastroObjeto><usuario></usuario><senha></senha><tipo>L</tipo><resultado>T</resultado><objetos>{0}</objetos><lingua>101</lingua><token></token></rastroObjeto>", TrackingCode);
            var values = new StringContent(requeststr, Encoding.UTF8, "application/xml");
            HttpResponseMessage response;
            try
            {
                response = await client.PostAsync("http://webservice.correios.com.br/service/rest/rastro/rastroMobile", values);
            }
            catch (System.Net.Http.HttpRequestException)
            {
                throw new Exceptions.ConnectionProblemException();
            }

            var responseString = await response.Content.ReadAsStringAsync();
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(responseString);
            if (xml.SelectSingleNode("rastro/objeto/categoria").InnerText.Contains("ERRO:"))
            {
                throw new Exceptions.ObjectNotFoundException(TrackingCode);
            }
            return xml;

            
        }
    }
}
