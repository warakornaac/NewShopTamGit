using NewShop.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NewShop.Controllers
{
    public class ApiServiceController : Controller
    {
        //
        // GET: /ApiService/

        public ActionResult Index()
        {
            return View();
        }
        private async Task<string> PushMessage(string Uid, string Docno, string Docdate, string Cusname, string Delivery, string User)
        {
            var urlAPI = "https://mst.aac.co.th/APIService/Post/PushMessage";
            var post = new ListSendDelivery
            {
                Uid = Uid,
                Docno = Docno,
                Docdate = Docdate,
                Cusname = Cusname,
                Delivery = Delivery,
                User = User
            };
            try
            {
                // ตั้งค่าการตรวจสอบใบรับรอง SSL/TLS
                var handler = new HttpClientHandler();
                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                var client = new HttpClient(handler);
                string jsonContent = JsonConvert.SerializeObject(post);
                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(urlAPI, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                }
                else
                {
                    return response.StatusCode.ToString();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> SendNotificateLine(string Uid, string Toppic, string ToppicCo, string stkcod, string stkdes, string Price, string Qty, string cuscod, string cusnam, string apprvby, string apprvdat, string user)
        {
            var urlAPI = "https://localhost:44361/Post/PushMessageSale";
            var post = new ListPMNotificate
            {
                Uid = Uid,
                Topic = Toppic,
                Topiccod = ToppicCo,
                stkcod = stkcod,
                stkdes = stkdes,
                SPrice = Price,
                Qty = Qty,
                Cuscod = cuscod,
                cusnam = cusnam,
                ApprvBy = apprvby,
                ApprvDate = apprvdat,
                user = user
            }
            try
            {
                var handler = new HttpClientHandler();
                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                var client = new HttpClient(handler);
                string jsonContent = JsonConvert.SerializeObject(post);
                HttpContent content = new StringContent(jsonContent, Encoder.UTF8, "application/json");
                HttpResponeMessage respone = await client.PostAsync(urlAPI, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                }
                else
                {
                    return response.StatusCode.ToString();
                }
            }
            catch (Exception ex) { return ex.Message; }

        }

    }
}
