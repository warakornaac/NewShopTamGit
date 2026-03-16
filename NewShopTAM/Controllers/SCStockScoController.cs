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

namespace NewShopTAM.Controllers
{
    public class SCStockScoController : Controller
    {
        //
        // GET: /SCStockSco/

        public ActionResult Index()
        {
            //this.Session["UserType"] = "";
            if (this.Session["UserType"] == "")
            {
                return RedirectToAction("LogIn", "Account");
            }
            else
            {
                if (this.Session["UserType"] == null)
                {
                    return RedirectToAction("LogIn", "Account");
                }
                else
                {
                    string Docdisplay = string.Empty;
                    string CUSCOD = string.Empty;

                    Docdisplay = Request.QueryString["numcuber"];
                    if (Docdisplay != null)
                    {

                        byte[] data = System.Convert.FromBase64String(Docdisplay);
                        CUSCOD = System.Text.ASCIIEncoding.ASCII.GetString(data);

                    }



                    ViewBag.Nodisplay = CUSCOD;


                }
            }
            return View();
        }
        public JsonResult Savedatatemp(string item, string SLM, string CUS, string User)
        {
            string message = "false";
            string strstk = string.Empty;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString))
                {
                    connection.Open();
                    List<ItemListtemp> _ItemList = new JavaScriptSerializer().Deserialize<List<ItemListtemp>>(item);
                    for (int i = 0; i < _ItemList.Count; i++)
                    {
                        strstk = _ItemList[i].item_t;
                        SqlCommand command = new SqlCommand("P_SaveOrder_Temp", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@inSLMCODE", SLM);
                        command.Parameters.AddWithValue("@inCUSCOD", CUS);
                        command.Parameters.AddWithValue("@inItem", strstk);
                        command.Parameters.AddWithValue("@inUsrID", User);
                        command.Parameters.AddWithValue("@Mode", "");
                        command.Parameters.AddWithValue("@CRM_Status", "");


                        //command.ExecuteNonQuery();
                    }
                    connection.Close();
                    message = "true";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(new { message }, JsonRequestBehavior.AllowGet);
        }
    }
}
