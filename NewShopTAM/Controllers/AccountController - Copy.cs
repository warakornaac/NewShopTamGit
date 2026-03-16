using NewShop.Models;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Services.Description;
using System.Web.UI.WebControls;
using UAParser;
using Newtonsoft.Json;
using System.IO;
using System.Web.Hosting;
using NewShop.Filters;

namespace NewShop.Controllers
{
    public class AccountController : Controller
    {
        //global variable
        string _Userlineid = string.Empty;
        // GET: /Account/
        public ActionResult Index()
        {
            this.Session["LoginSystem"] = "";
            if (this.Session["UserType"] == null)
            {
                this.Session["UserType"] = "";
            }


            return View();

        }
        [HttpGet]
        public ActionResult LoginCus()
        {
            if (this.Session["UserType"] == null)
            {
                this.Session["UserType"] = "";
            }
            ViewBag.Userlineid = _Userlineid;
            return View();
        }
        public ActionResult ChangePasswordPortal()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult LogIn()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            this.Session["LoginSystem"] = "";
            if (this.Session["UserType"] == null)
            {
                this.Session["UserType"] = "";
            }
            return View();
            //}
        }
        [HttpGet]
        public ActionResult CheckLoginExternal()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CheckDataLoginExternal(string userId, string email, string displayName, string page)
        {
            string message = string.Empty;
            string SLM = string.Empty;
            _Userlineid = userId;
            this.Session["Line"] = userId;
            this.Session["UserPassword"] = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            try
            {
                Connection.Open();
                var command = new SqlCommand("P_Check_Login_External", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@email", email.Trim());
                command.Parameters.AddWithValue("@displayName", displayName);

                SqlParameter returnValuedoc = new SqlParameter("@outGenstatus", SqlDbType.NVarChar, 100);
                SqlParameter Slm = new SqlParameter("@outSLM", SqlDbType.NVarChar, 100);

                returnValuedoc.Direction = System.Data.ParameterDirection.Output;
                Slm.Direction = System.Data.ParameterDirection.Output;
                command.Parameters.Add(returnValuedoc);
                command.Parameters.Add(Slm);
                command.ExecuteNonQuery();
                message = returnValuedoc.Value.ToString();
                SLM = Slm.Value.ToString();
                if (message == "Y")
                {

                    this.Session["slmcode"] = SLM.ToString();

                }
                command.Dispose();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            Connection.Close();

            return Json(new { message = message, page = page }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDataLoginExternal(string email, string page)
        {
            this.Session["UserID"] = string.Empty;
            //this.Session["Email"] = string.Empty;
            this.Session["UserType"] = string.Empty;
            this.Session["DisplayName"] = string.Empty;
            this.Session["CUSCOD"] = string.Empty;
            string UsrType = string.Empty;
            string Username = string.Empty;
            string message = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("select * From UsrTbl_Portal where Email =N'" + email + "'", Connection);
                SqlDataReader rev = cmd.ExecuteReader();
                while (rev.Read())
                {
                    if (!string.IsNullOrEmpty(rev["Email"].ToString()))
                    {
                        this.Session["UserID"] = rev["Email"].ToString();
                        this.Session["DisplayName"] = rev["CusName"].ToString();
                        this.Session["UserType"] = rev["UsrTyp"].ToString();
                        this.Session["CUSCOD"] = rev["CusCode"].ToString();
                        //if (rev["slmcode"] != DBNull.Value)
                        //{
                        //    this.Session["slmcode"] = rev["slmcode"].ToString();
                        //}
                        //get sesssion
                        string sessionId = string.Empty;
                        string httpCookie = string.Empty;
                        //set session id

                        var command = new SqlCommand("P_logSingin_customer_mobileStatus", Connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UsrID", email);
                        command.Parameters.AddWithValue("@SessionId", sessionId);
                        command.Parameters.AddWithValue("@flag", "external");
                        command.ExecuteReader();
                        command.Dispose();
                    }
                }
                rev.Close();
                rev.Dispose();
                cmd.Dispose();
                Connection.Close();
                message = "Y";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            var returnField = new { UserId = this.Session["UserID"], UserType = this.Session["UserType"], ID = this.Session["ID"], page = page, message = message, CUDCOD = this.Session["CUSCOD"] };
            return Json(returnField, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult LogIn(LoginUserViewModel User)
        {
            string Userlog = string.Empty;
            string Usertype = string.Empty;
            string dateexpire = string.Empty;
            string UsrClmStaff = string.Empty;
            //string appEnv = string.Empty;
            int intdateexpire = 0;
            //appEnv = ConfigurationManager.AppSettings["Environment"];
            //this.Session["appEnv"] = appEnv;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            //Connection.Open();
            // GenericIdentity identity = null;
            Connection.Open();
            try
            {
                this.Session["DisplayName"] = string.Empty;
                this.Session["UserType"] = null;
                this.Session["UserID"] = User.Usre;
                this.Session["UserPassword"] = User.Password;
                this.Session["UsrGrpspecial"] = 0;
                this.Session["DatetoExpire"] = "..";
                this.Session["UsrClmStaff"] = "0";
                this.Session["LoginSystem"] = ConfigurationManager.AppSettings["SystemCode"];

                string UserType = string.Empty;
                string sessionId = Request["http_cookie"];
                string secCodeArr = string.Empty;
                SqlCommand cmdcus = new SqlCommand("select * From v_UsrTbl_catalog where UsrID = @inUsername and [dbo].F_decrypt([Password])= @inPassword and  [LoginFail] <> 3", Connection);
                cmdcus.Parameters.AddWithValue("@inUsername", User.Usre);
                cmdcus.Parameters.AddWithValue("@inPassword", User.Password);
                SqlDataReader revcus = cmdcus.ExecuteReader();
                while (revcus.Read())
                {

                    this.Session["UserType"] = revcus["UsrTyp"].ToString();
                    this.Session["Department"] = revcus["Department"].ToString();
                    this.Session["CUSCOD"] = revcus["CUSCOD"].ToString();
                    UserType = Session["UserType"].ToString();
                    this.Session["UsrClmStaff"] = revcus["UsrClmStaff"].ToString();

                    sessionId = sessionId.Substring(sessionId.Length - 24);
                    this.Session["ID"] = sessionId;
                }

                revcus.Close();
                revcus.Dispose();
                cmdcus.Dispose();
                FormsAuthentication.SetAuthCookie(User.Usre, false);


                if (UserType == null)
                {
                    //ADSRV01
                    DirectoryEntry entry = new DirectoryEntry("LDAP://ADSRV2016-01/dc=Automotive,dc=com", User.Usre, User.Password);
                    DirectorySearcher search = new DirectorySearcher(entry);
                    search.Filter = "(SAMAccountName=" + User.Usre + ")";
                    search.PropertiesToLoad.Add("cn");

                    SearchResult result = search.FindOne();
                    //result.GetDirectoryEntry();
                    // Connection.Open();
                    if (null == result)
                    {
                        if (IsValid(User.Usre, User.Password))
                        {

                        }
                        else
                        {
                            ModelState.AddModelError("", "Login details are wrong.");
                        }
                        //throw new SoapException("Error authenticating user.",SoapException.ClientFaultCode);
                    }
                    else
                    {
                        this.Session["UserID"] = User.Usre;
                        this.Session["UserPassword"] = User.Password;
                        this.Session["UsrGrpspecial"] = 0;
                        SqlCommand cmd = new SqlCommand("select * From v_UsrTbl where UsrID =N'" + User.Usre + "'", Connection);
                        SqlDataReader rev = cmd.ExecuteReader();
                        while (rev.Read())
                        {

                            dateexpire = rev["Date to Expire"].ToString();
                            //dateexpire = "2";
                            this.Session["UserType"] = rev["UsrTyp"].ToString();
                            this.Session["CUSCOD"] = "";
                            this.Session["Department"] = rev["Department"].ToString();
                        }
                        rev.Close();
                        rev.Dispose();
                        cmd.Dispose();

                        intdateexpire = Convert.ToInt32(dateexpire);
                        this.Session["expdatecal"] = intdateexpire;
                        if (intdateexpire <= 15)
                        {
                            this.Session["DatetoExpire"] = "Passwords expire '" + intdateexpire + "' days";
                        }
                        else if (intdateexpire == 0)
                        {
                            this.Session["DatetoExpire"] = "The user's password must be changed password  Changed password on Citrix";
                        }
                        else
                        {

                            this.Session["DatetoExpire"] = "..";
                        }

                        FormsAuthentication.SetAuthCookie(User.Usre, false);

                        //return RedirectToAction("Index", "SeleScrCustomer");
                        UserType = Session["UserType"].ToString();
                        if (UserType == "5")
                        {
                            // return RedirectToAction("Index", "Home");
                            return RedirectToAction("Index", "PriceApproval");
                        }
                        else
                        {
                            return RedirectToAction("dashboard", "SeleScrCustomer");
                        }
                    }

                    // ModelState.AddModelError("", "Login details are wrong.");
                }
                else if (UserType == "")
                {
                    //ADSRV01
                    DirectoryEntry entry = new DirectoryEntry("LDAP://ADSRV2016-01/dc=Automotive,dc=com", User.Usre, User.Password);
                    DirectorySearcher search = new DirectorySearcher(entry);
                    search.Filter = "(SAMAccountName=" + User.Usre + ")";
                    search.PropertiesToLoad.Add("cn");

                    SearchResult result = search.FindOne();
                    //result.GetDirectoryEntry();
                    // Connection.Open();
                    if (null == result)
                    {
                        if (IsValid(User.Usre, User.Password))
                        {

                        }
                        else
                        {
                            ModelState.AddModelError("", "Login details are wrong.");
                        }
                        //throw new SoapException("Error authenticating user.",SoapException.ClientFaultCode);
                    }
                    else
                    {
                        this.Session["UserID"] = User.Usre;
                        this.Session["UserPassword"] = User.Password;
                        this.Session["UsrGrpspecial"] = 0;
                        SqlCommand cmd = new SqlCommand("select * From v_UsrTbl where UsrID =N'" + User.Usre + "'", Connection);
                        SqlDataReader rev = cmd.ExecuteReader();
                        while (rev.Read())
                        {

                            dateexpire = rev["Date to Expire"].ToString();
                            //dateexpire = "2";
                            this.Session["UserType"] = rev["UsrTyp"].ToString();
                            this.Session["CUSCOD"] = "";
                            this.Session["Department"] = rev["Department"].ToString();
                        }
                        rev.Close();
                        rev.Dispose();
                        cmd.Dispose();

                        intdateexpire = Convert.ToInt32(dateexpire);
                        this.Session["expdatecal"] = intdateexpire;
                        if (intdateexpire <= 15)
                        {
                            this.Session["DatetoExpire"] = "Passwords expire '" + intdateexpire + "' days";
                        }
                        else if (intdateexpire == 0)
                        {
                            this.Session["DatetoExpire"] = "The user's password must be changed password  Changed password on Citrix";
                        }
                        else
                        {

                            this.Session["DatetoExpire"] = "..";
                        }

                        FormsAuthentication.SetAuthCookie(User.Usre, false);

                        //return RedirectToAction("Index", "SeleScrCustomer");
                        UserType = Session["UserType"].ToString();
                        if (UserType == "5")
                        {
                            var infoUser = GetUserInfo();
                            var command = new SqlCommand("P_LoginMobile_log", Connection);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@UsrID", User.Usre);
                            command.Parameters.AddWithValue("@UsrType", UserType);
                            command.Parameters.AddWithValue("@OS", infoUser.OS);
                            command.Parameters.AddWithValue("@Browser", infoUser.Browser);
                            command.Parameters.AddWithValue("@IpAddress", infoUser.Ip_Addresss);
                            command.Parameters.AddWithValue("@Latitude", User.Latitude);
                            command.Parameters.AddWithValue("@Longitude", User.Longitude);
                            command.ExecuteNonQuery();
                            command.Dispose();

                            // return RedirectToAction("Index", "Home");
                            return RedirectToAction("Index", "PriceApproval");
                        }
                        else
                        {
                            var infoUser = GetUserInfo();
                            var command = new SqlCommand("P_LoginMobile_log", Connection);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@UsrID", User.Usre);
                            command.Parameters.AddWithValue("@UsrType", UserType);
                            command.Parameters.AddWithValue("@OS", infoUser.OS);
                            command.Parameters.AddWithValue("@Browser", infoUser.Browser);
                            command.Parameters.AddWithValue("@IpAddress", infoUser.Ip_Addresss);
                            command.Parameters.AddWithValue("@Latitude", User.Latitude);
                            command.Parameters.AddWithValue("@Longitude", User.Longitude);
                            command.ExecuteNonQuery();
                            command.Dispose();
                            return RedirectToAction("dashboard", "SeleScrCustomer");
                        }
                    }
                    // ModelState.AddModelError("", "Login details are wrong.");
                }
                else if (UserType == "6") //customer
                {
                    var command = new SqlCommand("P_logSingin", Connection);
                    command.CommandType = CommandType.StoredProcedure;
                    //command.Parameters.AddWithValue("@pCusCod", strcustome);
                    command.Parameters.AddWithValue("@UsrID", User.Usre);
                    command.Parameters.AddWithValue("@SessionId", sessionId);
                    command.ExecuteReader();
                    command.Dispose();
                    //get date expire
                    SqlCommand cmd = new SqlCommand("select * From v_UsrTbl where UsrID =N'" + User.Usre + "'", Connection);
                    SqlDataReader rev = cmd.ExecuteReader();
                    while (rev.Read())
                    {
                        dateexpire = rev["Date to Expire"].ToString();
                    }
                    rev.Close();
                    rev.Dispose();
                    cmd.Dispose();
                    intdateexpire = Convert.ToInt32(dateexpire);
                    this.Session["expdatecal"] = intdateexpire;
                    if (intdateexpire <= 0)
                    {
                        ModelState.AddModelError("", "Your password expire.");
                    }
                    else
                    {
                        return RedirectToAction("dashboard", "SeleScrCustomer");
                    }
                    //end get date expire
                    //return RedirectToAction("Index", "Home");
                }
                else if (UserType == "2")//sales
                {
                    var infoUser = GetUserInfo();
                    var command = new SqlCommand("P_LoginMobile_log", Connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UsrID", User.Usre);
                    command.Parameters.AddWithValue("@UsrType", UserType);
                    command.Parameters.AddWithValue("@OS", infoUser.OS);
                    command.Parameters.AddWithValue("@Browser", infoUser.Browser);
                    command.Parameters.AddWithValue("@IpAddress", infoUser.Ip_Addresss);
                    command.Parameters.AddWithValue("@Latitude", User.Latitude);
                    command.Parameters.AddWithValue("@Longitude", User.Longitude);
                    command.ExecuteNonQuery();
                    command.Dispose();
                    return RedirectToAction("dashboard", "SeleScrCustomer");
                }
                else if (UserType == "1")//salesco
                {
                    var infoUser = GetUserInfo();
                    var command = new SqlCommand("P_LoginMobile_log", Connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UsrID", User.Usre);
                    command.Parameters.AddWithValue("@UsrType", UserType);
                    command.Parameters.AddWithValue("@OS", infoUser.OS);
                    command.Parameters.AddWithValue("@Browser", infoUser.Browser);
                    command.Parameters.AddWithValue("@IpAddress", infoUser.Ip_Addresss);
                    command.Parameters.AddWithValue("@Latitude", User.Latitude);
                    command.Parameters.AddWithValue("@Longitude", User.Longitude);
                    command.ExecuteNonQuery();
                    command.Dispose();
                    return RedirectToAction("dashboard", "SeleScrCustomer");
                }
                else if (UserType == "5")//pm
                {
                    var infoUser = GetUserInfo();
                    var command = new SqlCommand("P_LoginMobile_log", Connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UsrID", User.Usre);
                    command.Parameters.AddWithValue("@UsrType", UserType);
                    command.Parameters.AddWithValue("@OS", infoUser.OS);
                    command.Parameters.AddWithValue("@Browser", infoUser.Browser);
                    command.Parameters.AddWithValue("@IpAddress", infoUser.Ip_Addresss);
                    command.Parameters.AddWithValue("@Latitude", User.Latitude);
                    command.Parameters.AddWithValue("@Longitude", User.Longitude);
                    command.ExecuteNonQuery();
                    command.Dispose();
                    return RedirectToAction("Index", "PriceApproval");
                }
            }
            //catch (COMException ex)
            //{
            //    ModelState.AddModelError("", "Login details are wrong.");
            //}
            catch (SqlException ex)
            {
                if (ex.Number == -2)
                {
                    TempData["ErrorMessage"] = "Database connection timed out. Please try again.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Database error: " + ex.Message;
                }
            }
            catch (COMException)
            {
                TempData["ErrorMessage"] = "Login details are wrong.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
            }
            finally
            {
                Connection.Close();
            }
            //Connection.Close();
            return View("Login", User);
            //return View();
        }

        public LoginuserInfo GetUserInfo()
        {
            var uaString = Request.Headers["User-Agent"].ToString();
            var uaParser = Parser.GetDefault();
            string ipAddress = GetIp();
            ClientInfo clientInfo = uaParser.Parse(uaString);

            var os = clientInfo.OS.ToString();
            var browser = clientInfo.UserAgent.ToString();

            return new LoginuserInfo
            {
                OS = os,
                Browser = browser,
                Ip_Addresss = ipAddress,
            };
        }
        public string GetIp()
        {
            string ip =
            System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip;
        }



        // [HttpPost]
        public ActionResult LogInRedir(string User, string password)
        {
            string Docdisplay = string.Empty;
            string Userlog = string.Empty;
            string Usertype = string.Empty;
            string dateexpire = string.Empty;
            // string User = string.Empty;
            // string password = string.Empty;
            int intdateexpire = 0;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            //Connection.Open();
            // GenericIdentity identity = null;
            Connection.Open();
            try
            {


                //string basePack = Pack;

                //byte[] data = System.Convert.FromBase64String(Docdisplay);

                //ADSRV01
                DirectoryEntry entry = new DirectoryEntry("LDAP://ADSRV2016-01/dc=Automotive,dc=com", User, password);
                DirectorySearcher search = new DirectorySearcher(entry);
                search.Filter = "(SAMAccountName=" + User + ")";
                search.PropertiesToLoad.Add("cn");

                SearchResult result = search.FindOne();
                //result.GetDirectoryEntry();
                // Connection.Open();
                if (null == result)
                {
                    if (IsValid(User, password))
                    {

                    }
                    else
                    {
                        ModelState.AddModelError("", "Login details are wrong.");
                    }
                    //throw new SoapException("Error authenticating user.",SoapException.ClientFaultCode);
                }
                else
                {
                    this.Session["UserID"] = User;
                    this.Session["UserPassword"] = password;
                    this.Session["UsrGrpspecial"] = 0;
                    SqlCommand cmd = new SqlCommand("select * From v_UsrTbl where UsrID =N'" + User + "'", Connection);
                    SqlDataReader rev = cmd.ExecuteReader();
                    while (rev.Read())
                    {

                        dateexpire = rev["Date to Expire"].ToString();
                        this.Session["UserType"] = rev["UsrTyp"].ToString();
                        this.Session["CUSCOD"] = "";
                        this.Session["Department"] = rev["Department"].ToString();
                    }
                    rev.Close();
                    rev.Dispose();
                    cmd.Dispose();

                    intdateexpire = Convert.ToInt32(dateexpire);
                    this.Session["expdatecal"] = intdateexpire;
                    if (intdateexpire <= 15)
                    {
                        this.Session["DatetoExpire"] = "Passwords expire '" + intdateexpire + "' days";
                    }
                    else if (intdateexpire == 0)
                    {
                        this.Session["DatetoExpire"] = "The user's password must be changed password  Changed password on Citrix";
                    }
                    else
                    {

                        this.Session["DatetoExpire"] = "..";
                    }

                    FormsAuthentication.SetAuthCookie(User, false);

                    //return RedirectToAction("Index", "SeleScrCustomer");
                    string UserType = Session["UserType"].ToString();
                    if (UserType == "5")
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Index", "SeleScrCustomer");
                    }
                }

            }
            catch (COMException ex)
            {
                this.Session["UserType"] = null;
                this.Session["UserID"] = User;
                this.Session["UserPassword"] = password;
                this.Session["UsrGrpspecial"] = 0;
                string UserType = string.Empty;
                SqlCommand cmdcus = new SqlCommand("select * From v_UsrTbl_catalog where UsrID = @inUser and [dbo].F_decrypt([Password])= @inPassword and  [LoginFail] <> 3", Connection);
                cmdcus.Parameters.AddWithValue("@inUser", User);
                cmdcus.Parameters.AddWithValue("@inPassword", password);
                SqlDataReader revcus = cmdcus.ExecuteReader();
                while (revcus.Read())
                {

                    this.Session["UserType"] = revcus["UsrTyp"].ToString();
                    this.Session["Department"] = revcus["Department"].ToString();
                    this.Session["CUSCOD"] = revcus["CUSCOD"].ToString();
                    UserType = Session["UserType"].ToString();
                }

                revcus.Close();
                revcus.Dispose();
                cmdcus.Dispose();
                FormsAuthentication.SetAuthCookie(User, false);

                if (UserType == null)
                {
                    ModelState.AddModelError("", "Login details are wrong.");
                }
                else if (UserType == "")
                {
                    ModelState.AddModelError("", "Login details are wrong.");
                }
                else if (UserType == "6") //customer
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (UserType == "2")//sales
                {
                    return RedirectToAction("Index", "SeleScrCustomer");
                }
                else if (UserType == "1")//salesco
                {
                    return RedirectToAction("Index", "SeleScrCustomer");
                }
                else if (UserType == "5")//pm
                {
                    return RedirectToAction("Index", "Home");
                }


            }
            Connection.Close();

            return View();
        }
        /*
        //External LogIn
        [HttpPost]
        public ActionResult CheckDataLoginExternal(string userId, string email, string displayName)
        {
            string message = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            try
            {
                Connection.Open();
                var command = new SqlCommand("สโตเช็ค Login", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@displayName", displayName);

                SqlParameter returnValuedoc = new SqlParameter("@outGenstatus", SqlDbType.NVarChar, 100);
                returnValuedoc.Direction = System.Data.ParameterDirection.Output;
                command.Parameters.Add(returnValuedoc);

                command.ExecuteNonQuery();
                message = returnValuedoc.Value.ToString();
                command.Dispose();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            Connection.Close();

            return Json(new { message }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDataLoginExternal(string userId, string page)
        {
            this.Session["UserID"] = string.Empty;
            this.Session["UserType"] = string.Empty;
            string message = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {
                //SqlCommand cmd = new SqlCommand("select * From UsrTbl_Line where UserIdLine =N'" + userId + "' and  [LoginFail] <> 3", Connection);
                SqlCommand cmd = new SqlCommand("select * From UsrTbl_Line where UserIdLine =N'" + userId, Connection);
                SqlDataReader rev = cmd.ExecuteReader();
                while (rev.Read())
                {
                    if (!string.IsNullOrEmpty(rev["UsrID"].ToString()))
                    {
                        message = "Y";
                        this.Session["UserID"] = rev["UsrID"].ToString();
                        this.Session["UserType"] = rev["UsrTyp"].ToString();
                        //get sesssion
                        string sessionId = string.Empty;
                        string httpCookie = string.Empty;
                        if (Request.ServerVariables["HTTP_COOKIE"] != null)
                        {
                            httpCookie = Request.ServerVariables["HTTP_COOKIE"].Substring(0, (Request.ServerVariables["HTTP_COOKIE"].Length > 399) ? 399 : Request.ServerVariables["HTTP_COOKIE"].Length);
                        }
                        sessionId = httpCookie;

                        if (sessionId != null)
                        {
                            sessionId = sessionId.Substring(sessionId.Length - 24);
                            this.Session["ID"] = sessionId;
                        }
                        else
                        {
                            this.Session["ID"] = "775.333";
                        }
                        //set session id
                        var command = new SqlCommand("สโตคัสตอมเมอร์ Login", Connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UsrID", userId);
                        command.Parameters.AddWithValue("@SessionId", sessionId);
                        command.Parameters.AddWithValue("@flag", "external");
                        command.ExecuteReader();
                        command.Dispose();
                    }
                }
                rev.Close();
                rev.Dispose();
                cmd.Dispose();
                Connection.Close();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            var returnField = new { UserId = this.Session["UserID"], UserType = this.Session["UserType"], ID = this.Session["ID"], page = page, message = message };
            return Json(returnField, JsonRequestBehavior.AllowGet);
        }


        */

        [HttpPost]
        public ActionResult LoginCus(string User, string password, string page)
        {
            string message = string.Empty;
            string phoneNum = string.Empty;
            string cuscode = string.Empty;
            string email = string.Empty;
            string UserType = string.Empty;
            string SLM = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {
                this.Session["DisplayName"] = string.Empty;
                this.Session["UserType"] = null;
                this.Session["UserID"] = null;
                this.Session["UserPassword"] = password;
                this.Session["UsrGrpspecial"] = 0;
                this.Session["DatetoExpire"] = "..";
                this.Session["UsrClmStaff"] = "0";

                string secCodeArr = string.Empty;
                var sqlString = "select * From UsrTbl_Portal where Username = @user and [dbo].F_decrypt([Password]) = @pass";
                //SqlCommand cmdcus = new SqlCommand("select * From UsrTbl_Portal where Username =N'" + User + "'and [dbo].F_decrypt([Password])='" + password + "'", Connection);
                SqlCommand cmdcus = new SqlCommand(sqlString, Connection);
                cmdcus.Parameters.AddWithValue("user", User);
                cmdcus.Parameters.AddWithValue("pass", password);
                SqlDataReader revcus = cmdcus.ExecuteReader();
                while (revcus.Read())
                {
                    phoneNum = revcus["Tel"].ToString();
                    this.Session["UserType"] = revcus["UsrTyp"].ToString();
                    this.Session["CUSCOD"] = revcus["CusCode"].ToString();
                    this.Session["DisplayName"] = revcus["CusName"].ToString();
                    email = revcus["Email"].ToString();
                    cuscode = revcus["CusCode"].ToString();
                    message = revcus["VerifyFlag"].ToString();
                    UserType = Session["UserType"].ToString();
                    //if (revcus["slmcode"] != DBNull.Value)
                    //{
                    //    this.Session["slmcode"] = revcus["slmcode"].ToString();
                    //}
                    //SLM = revcus["slmcode"] != DBNull.Value ? revcus["slmcode"].ToString() : string.Empty;
                }



                revcus.Close();
                revcus.Dispose();
                cmdcus.Dispose();
                if (message == "Y")
                {
                    message = "N";
                }
                if (UserType == "2")
                {
                    var cmd = new SqlCommand("P_Check_User_Active_AD", Connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@InUsrType", UserType);
                    cmd.Parameters.AddWithValue("@InUserName", User);
                    SqlParameter p = new SqlParameter("@OutGenstatus", SqlDbType.NVarChar, 100);
                    SqlParameter Slm = new SqlParameter("@OutSlm", SqlDbType.NVarChar, 20);
                    p.Direction = ParameterDirection.Output;
                    Slm.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(p);
                    cmd.Parameters.Add(Slm);
                    cmd.ExecuteNonQuery();
                    var status = cmd.Parameters["@OutGenstatus"].Value.ToString();
                    var sqlSLM = cmd.Parameters["@OutSlm"].Value.ToString();
                    if (!string.IsNullOrEmpty(status))
                    {
                        if (status == "Y")
                        {
                            message = "N";
                            this.Session["slmcode"] = sqlSLM;
                        }
                        else
                        {
                            message = "B";
                        }
                    }
                    else
                    {
                        message = "B";
                    }
                }

                Connection.Close();
            }
            catch (Exception ex)
            {
                message = ex.Message;
                ViewData["ErrorMessage"] = "Login details are wrong.";
            }

            return Json(new { message = message, tel = phoneNum, page = page, cuscod = cuscode, email = email, UserType = UserType }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ChangePassword(string userName, string oldPassword, string newPassword)
        {
            //string userName = System.Environment.UserName;
            //string userName = "Deploy";
            //currentPassword = "Happy1002";
            //newPassword = "Happy1003";
            string messageSave = "You password has been changed.";
            string statusSave = "success";
            try
            {
                DirectoryEntry directionEntry = new DirectoryEntry("LDAP://ADSRV2016-01/dc=Automotive,dc=com", userName, oldPassword);
                if (directionEntry != null)
                {
                    DirectorySearcher search = new DirectorySearcher(directionEntry);
                    search.Filter = "(SAMAccountName=" + userName + ")";
                    SearchResult result = search.FindOne();
                    if (result != null)
                    {
                        DirectoryEntry userEntry = result.GetDirectoryEntry();
                        if (userEntry != null)
                        {
                            userEntry.Invoke("ChangePassword", new object[] { oldPassword, newPassword });
                            // userEntry.Invoke("SetPassword", new object[] { newPassword }); กรณี reset pass สิทธิ์ admin
                            userEntry.CommitChanges();
                            userEntry.Close();
                            userEntry.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Exception has been thrown by the target of an invocation / passwprd new ไม่ผ่าน
                statusSave = "error";
                messageSave = ex.Message.ToString();//ex.Message.ToString();
            }
            return Json(new { status = statusSave, message = messageSave }, JsonRequestBehavior.AllowGet);
        }
        private bool IsValid(string user, string Password)
        {

            bool IsValid = false;
            if (user == null || Password == null) { IsValid = false; }
            else
            {


            }
            return IsValid;
        }

        public ActionResult Register()
        {
            if (Session["UserType"] == null)
            {
                return RedirectToAction("LogIn", "Account");
            }
            return View();
        }
        [HttpPost]
        public ActionResult AddUser(string email, string username, string pass, string tel, string cuscos, string cusname, string user, string slmcode, string usertype)
        {

            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            string lindId = " ";
            string displayName = "";
            string status = string.Empty;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("P_Register_customer", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inemail", email.Trim());
                    cmd.Parameters.AddWithValue("@inusername", username.Trim());
                    cmd.Parameters.AddWithValue("@inpassword", pass.Trim());
                    cmd.Parameters.AddWithValue("@intel", tel.Trim()); ;
                    cmd.Parameters.AddWithValue("@inuserId", lindId.Trim());
                    cmd.Parameters.AddWithValue("@indisplayName", displayName.Trim());
                    cmd.Parameters.AddWithValue("@incuscod", cuscos.Trim());
                    cmd.Parameters.AddWithValue("@incusname", cusname);
                    cmd.Parameters.AddWithValue("@inUser", user.Trim());
                    cmd.Parameters.AddWithValue("@inUserType", usertype.Trim());
                    cmd.Parameters.AddWithValue("@inSLM", slmcode.Trim());
                    SqlParameter p = new SqlParameter("@outGenstatus", SqlDbType.NVarChar, 100);
                    p.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(p);
                    int INSID = cmd.ExecuteNonQuery();
                    if (INSID > 0)
                    {

                    }
                    status = cmd.Parameters["@outGenstatus"].Value.ToString();

                }
                return Json(new { status = status, message = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = ex.ToString() });
            }
            // return Json(new { status = "success", });
        }
        public JsonResult Log_Login(string username)
        {
            string message = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {
                var cmd = new SqlCommand("P_CustomerPortal_Login_log", Connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inUser", username.Trim());
                SqlParameter p = new SqlParameter("@outGenstatus", SqlDbType.NVarChar, 100);
                p.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p);
                cmd.ExecuteNonQuery();
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
        public JsonResult CheckUsername(string username)
        {
            string message = string.Empty;
            string phone = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {
                var cmd = new SqlCommand("P_CustomerPortal_CheckUserName", Connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inUser", username.Trim());
                SqlParameter p = new SqlParameter("@outGenstatus", SqlDbType.NVarChar, 100);
                SqlParameter Tel = new SqlParameter("@outPhone", SqlDbType.NVarChar, 100);
                p.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p);
                Tel.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(Tel);
                cmd.ExecuteNonQuery();
                message = cmd.Parameters["@outGenstatus"].Value.ToString();
                phone = cmd.Parameters["@outPhone"].Value.ToString();

                cmd.Dispose();
                Connection.Close();

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return Json(new { message = message, phone = phone }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangeNewPasswordPortal(string USER, string password, string newpassword, string confirmpassword)
        {
            string message = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {
                var cmd = new SqlCommand("P_CustomerPortal_ChangePassword", Connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inUser", USER);
                cmd.Parameters.AddWithValue("@password", password);
                cmd.Parameters.AddWithValue("@newPassword", newpassword);
                cmd.Parameters.AddWithValue("@confirmPassword", confirmpassword);
                SqlParameter p = new SqlParameter("@outGenstatus", SqlDbType.NVarChar, 100);
                p.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p);
                cmd.ExecuteNonQuery();
                message = cmd.Parameters["@outGenstatus"].Value.ToString();

                cmd.Dispose();
                Connection.Close();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }
    }

}
