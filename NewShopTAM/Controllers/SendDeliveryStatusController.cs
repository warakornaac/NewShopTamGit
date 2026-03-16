using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewShopTAM.Controllers;
using System.Data;
using System.IO;
using System.Web.Script.Serialization;
using NewShopTAM.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NewShopTAM.Controllers
{
    public class SendDeliveryStatusController : Controller
    {
        // GET: /SendDeliveryStatus/
        public ActionResult Index()
        {
            //var getDateInput = Utils.PushMessage();
            List<ListSendDelivery> List = new List<ListSendDelivery>();
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            var command = new SqlCommand("p_Order_Notify", Connection);
            command.CommandType = CommandType.StoredProcedure;
            Connection.Open();
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                List.Add(new NewShopTAM.Models.ListSendDelivery()
                {
                    Uid = dr["Uid"].ToString(),
                    Docno = dr["Ord_DocNo"].ToString(),
                    Docdate = dr["ORD_Date"].ToString(),
                    Cusname = dr["CUSNAM"].ToString(),
                    Delivery = dr["Notify"].ToString(),
                    User = "System",
                });
            }

            ViewBag.listData = List;
            return View();
        }
        public ActionResult DashboardSendDeliveryStatus()
        {
            int numSuccess = 0;
            int numError = 0;
            string[] arrSuccess = new string[2];
            string[] arrError = new string[2];


            ViewBag.listData = ""; // GetStoreSearchOrderNotify();
            ViewBag.countlistDataAll = "";// GetStoreSearchOrderNotify();
            ViewBag.numError = numError;
            ViewBag.arrError = arrError;
            ViewBag.numSuccess = numSuccess;
            ViewBag.arrSuccess = arrSuccess;

            return View();
        }
        [HttpPost]
        public JsonResult GetSendDeliveryCount()
        {
            var getNotifyCount = GetStoreSearchOrderNotifyCount();
            return Json(getNotifyCount, JsonRequestBehavior.AllowGet);
        }
        //list order
        public JsonResult GetStoreSearchOrderNotify()
        {
            int numSuccess = 0;
            int numError = 0;
            var encodeDocno = string.Empty;
            var encodeCuskey = string.Empty;
            var encodeUrlDetail = string.Empty;
           

            List<ListSendDelivery> ListSendDelivery = new List<ListSendDelivery>();
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            var command = new SqlCommand("p_Order_Notify", Connection);
            command.CommandTimeout = 240; //60 sec = 1 min
            command.CommandType = CommandType.StoredProcedure;
            Connection.Open();
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                ListSendDelivery.Add(new NewShopTAM.Models.ListSendDelivery()
                {
                    Uid = dr["Uid"].ToString(),
                    Docno = dr["Ord_DocNo"].ToString(),
                    Docdate = dr["ORD_Date"].ToString(),
                    Cusname = dr["CUSNAM"].ToString(),
                    Delivery = dr["Notify"].ToString(),
                    DeliveryId = dr["NotifyID"].ToString(),
                    UrlDocno = dr["Docno"].ToString(),
                    UrlCuskey = dr["CUSKEY"].ToString(),
                    User = "System",
                });
            }
            //get data by stored
            if (ListSendDelivery.Any())
            {
                foreach (var rowList in ListSendDelivery)
                {
                    if (!string.IsNullOrEmpty(rowList.UrlDocno)){
                        //encode docno
                        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(rowList.UrlDocno);
                        encodeDocno = System.Convert.ToBase64String(plainTextBytes);    
                        //encode cuscode
                        var plainTextBytes2 = System.Text.Encoding.UTF8.GetBytes(rowList.UrlCuskey);
                        encodeCuskey = System.Convert.ToBase64String(plainTextBytes2);
                        encodeUrlDetail = "https://mst.aac.co.th/MobileCatalog/CustomerDashboard/GetDeliveryDetailByDocno?getDocno=" + encodeDocno + "&getCuskey=" + encodeCuskey;
                    }

                    var statusApi = ApiPushMessage(rowList.Uid, rowList.Docno, rowList.Docdate, rowList.Cusname, rowList.Delivery, rowList.DeliveryId, encodeUrlDetail, rowList.User);
                    string json = JsonConvert.SerializeObject(statusApi.Result.Data);
                    ResultApi dto = JsonConvert.DeserializeObject<ResultApi>(json);
                    //send fail
                    if (dto.status != "OK")
                    {
                        ++numError;
                        //arrError[numError] = rowList.Docno;
                    }
                    else
                    {
                        ++numSuccess;
                        //arrSuccess[numSuccess] = rowList.Docno;
                    }
                }
            }
            return Json(ListSendDelivery, JsonRequestBehavior.AllowGet);
        }
        //notify count
        [HttpPost]
        public JsonResult GetStoreSearchOrderNotifyCount()
        {
            List<ListSendDeliveryCount> ListSendDeliveryCount = new List<ListSendDeliveryCount>();
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            var command = new SqlCommand("p_Order_Notify_Count", Connection);
            command.CommandType = CommandType.StoredProcedure;
            Connection.Open();
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                ListSendDeliveryCount.Add(new ListSendDeliveryCount()
                {
                    sumOrderAll = dr["sumOrderAll"].ToString(),
                    sumOrderCurrentDate = dr["sumOrderCurrentDate"].ToString(),
                    sumOrderByMonth = dr["sumOrderByMonth"].ToString(),
                    sumOrderStatus1 = dr["sumOrderStatus1"].ToString(),
                    sumOrderStatus2 = dr["sumOrderStatus2"].ToString(),
                    sumOrderStatus3 = dr["sumOrderStatus3"].ToString(),
                    sumOrderStatus4 = dr["sumOrderStatus4"].ToString(),
                    sumDateCurrent = DateTime.Now.ToString(),
                });
            }
            dr.Close();
            dr.Dispose();
            command.Dispose();
            Connection.Close();
            //return Json(List, JsonRequestBehavior.AllowGet);
            //return Json(new { data = ListSendDeliveryCount });
            return Json(ListSendDeliveryCount, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ApiPushMessage(string Uid, string Docno, string Docdate, string Cusname, string Delivery, string DeliveryId, string Urldetail, string User)
        {
            var url = "https://mst.aac.co.th/APIService/Post/PushMessage";
            //var url = "https://localhost:44361/Post/PushMessage";
            string status = string.Empty;
            string message = string.Empty;
            string encodeDocno = string.Empty;
            string encodeUrlDetail = string.Empty;
          
            var post = new ListSendDelivery
            {
                Uid = Uid,
                Docno = Docno,
                Docdate = Docdate,
                Cusname = Cusname,
                Delivery = Delivery,
                DeliveryId = DeliveryId,
                Urldetail = Urldetail,
                User = User
            };
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var handler = new HttpClientHandler();
                var client = new HttpClient(handler);
                string jsonContent = JsonConvert.SerializeObject(post);
                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(url, content).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    message = responseContent;
                    status = response.StatusCode.ToString();
                }
                else
                {
                    status =  response.StatusCode.ToString();
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(new { status = status, message = message}, JsonRequestBehavior.AllowGet);
        }
        public class ResultApi
        {
            public string status { get; set; }
            public string message { get; set; }
        }

    }
}