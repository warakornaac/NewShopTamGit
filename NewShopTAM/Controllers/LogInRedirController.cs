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
using System.Web.Services.Protocols;
using System.Security.Principal;
using System.DirectoryServices;
using System.Web.Security;
using System.Runtime.InteropServices;

namespace NewShopTAM.Controllers
{
    public class LogInRedirController : Controller
    {
        //
        // GET: /LogInRedir/

        public ActionResult Index()
        {
            return View();
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
                SqlCommand cmdcus = new SqlCommand("select * From v_UsrTbl_catalog where UsrID =N'" + User + "'and [dbo].F_decrypt([Password])='" + password + "' and  [LoginFail] <> 3", Connection);
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
            // return View(User);
            //  return User.Usre;
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
    }
}
