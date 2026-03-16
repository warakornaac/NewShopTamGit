using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using NewShopTAM.Models;
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
using static System.Net.Mime.MediaTypeNames;

namespace NewShopTAM.Controllers
{
    public class VerificationController : Controller
    {
        //
        // GET: /Verification/

        private readonly string _otpChars = "0123456789";
        private readonly Random _random = new Random();

        public ActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> SendOtp(string phone, string user, string refer)
        {
            string otp = new string(Enumerable.Repeat(_otpChars, 6)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
            string status = string.Empty;
            string message = string.Empty;
            string TextConfirmOtp = string.Empty;
            //string statusApi = string.Empty;
            string Api = string.Empty;
            var connectString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectString);
            try
            {
                conn.Open();
                var command = new SqlCommand("P_Add_Otp", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@user", user.Trim());
                command.Parameters.AddWithValue("@Phone", phone.Trim());
                command.Parameters.AddWithValue("@ref", refer.Trim());
                command.Parameters.AddWithValue("@OTP", otp.Trim());
                SqlParameter p = new SqlParameter("@outGenstatus", SqlDbType.NVarChar, 100);
                p.Direction = ParameterDirection.Output;
                SqlParameter m = new SqlParameter("@outColumn", SqlDbType.NVarChar, 100);
                m.Direction = ParameterDirection.Output;
                command.Parameters.Add(p);
                command.Parameters.Add(m);
                command.ExecuteNonQuery();
                message = command.Parameters["@outColumn"].Value.ToString();
                status = command.Parameters["@outGenstatus"].Value.ToString();
                command.Dispose();
                if (!string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(refer) && !string.IsNullOrEmpty(otp) && !string.IsNullOrEmpty(user))
                {
                    TextConfirmOtp = "OTP = " + otp + " [Ref:" + refer + "] สำหรับ Customer Portal จะหมดอายุภายใน 5 นาที";
                    var statusApi = Apiservice(phone, TextConfirmOtp, user);
                    Api = await statusApi;
                    //Api = "YES";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            conn.Close();

            return Json(new { message = message, status = status, Apisend = Api }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Verify(string lineid, string phone, string user, string otp, string refer, string page, string UserType)
        {
            var message = string.Empty;
            this.Session["UserID"] = null;
            var connectString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectString);
            try
            {
                conn.Open();
                var command = new SqlCommand("P_Check_Otp", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@lineid", lineid.Trim());
                command.Parameters.AddWithValue("@user", user.Trim());
                command.Parameters.AddWithValue("@Phone", phone.Trim());
                command.Parameters.AddWithValue("@OTP", otp.Trim());
                command.Parameters.AddWithValue("@ref", refer.Trim());
                SqlParameter p = new SqlParameter("@outGenstatus", SqlDbType.NVarChar, 100);
                p.Direction = ParameterDirection.Output;
                command.Parameters.Add(p);
                command.ExecuteNonQuery();
                message = command.Parameters["@outGenstatus"].Value.ToString();
                if (message == "Y")
                {
                    this.Session["UserID"] = user;
                }
                else if (message == "R")
                {
                    this.Session["UserID"] = user;
                }
                else
                {
                    this.Session["UserID"] = null;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(new { message = message, page = page, UserType = UserType }, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ForgetPasswordCustomer(string username, string phone, string new_password)
        {
            string message = string.Empty;
            string textResetSMS = string.Empty;
            string API = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            if (username == null || username == "")
            {
                SqlCommand command = new SqlCommand("select Username from UsrTbl_Portal where Tel = '" + phone + "'", Connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    username = reader["Username"].ToString();
                }
            }
            try
            {
                var cmd = new SqlCommand("P_CustomerPortal_ForgetPassword", Connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inUser", username);
                cmd.Parameters.AddWithValue("@inPhone", phone);
                cmd.Parameters.AddWithValue("@resetPassword", new_password.Trim());
                SqlParameter p = new SqlParameter("@outGenstatus", SqlDbType.NVarChar, 100);
                p.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p);
                int INTCM = cmd.ExecuteNonQuery();
                if (INTCM > 0)
                {
                    textResetSMS = $"รหัสผ่านใหม่สำหรับบัญชีบน Customer Portal \nUsername: < {username} >\nPassword: < {new_password} >";
                    //textResetSMS = "รหัสผ่านใหม่สำหรับบัญชีบน Customer Portal  \nUsername: {username} \nPassword: {new_password}";
                    var statusAPI = Apiservice(phone, textResetSMS, username);
                    API = await statusAPI;
                }
                message = cmd.Parameters["@outGenstatus"].Value.ToString();

                cmd.Dispose();
                Connection.Close();

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(new { message = message }, JsonRequestBehavior.AllowGet);
        }
        public string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random random = new Random();
            StringBuilder stringBuilder = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(chars[random.Next(chars.Length)]);
            }
            return stringBuilder.ToString();
        }
        private async Task<string> Apiservice(string phone, string Text, string user)
        {
            var urlAPI = "https://mst.aac.co.th/APIService/Post/SendSms";
            var post = new SmsModels
            {
                Phone = phone,
                Text = Text,
                User = user
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
    }
}
