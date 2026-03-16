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
    public class SelectPrintStatementController : Controller
    {
        //
        // GET: /Otherlink/
       
        [HttpGet]

        public ActionResult Index()
        {

            if (this.Session["UserType"] == null)
            {
                return RedirectToAction("LogIn", "Account");
            }
            else
            {
                string usre = Session["UserID"].ToString();
                string Password = Session["UserPassword"].ToString();
                List<SLM> SlmList = new List<SLM>();


                var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
                SqlConnection Connection = new SqlConnection(connectionString);
                Connection.Open();
                var command = new SqlCommand("P_Chk_user", Connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UsrID", usre);
                command.Parameters.AddWithValue("@Password", Password);

                SqlDataReader dr = command.ExecuteReader();

                while (dr.Read())
                {
                    SlmList.Add(new SLM()
                    {
                        SLMCOD = dr.GetValue(1).ToString(),
                        SLMNAM = dr.GetValue(2).ToString()
                    });
                }
                dr.Close();
                dr.Dispose();

                var _User = this.Session["UsrTyp"];
                ViewBag.typeUs = _User;


                ViewBag.GroupSLMTAB = SlmList;

                List<SelectListItem> GroupSLMTSupSlm = new List<SelectListItem>();
                GroupSLMTSupSlm.Add(new SelectListItem() { Text = "เลือก SUP", Value = "ALL" });
                //var getdataSLMTSupSlm = db.v_SLMTAB_SupSlm.OrderBy(s => s.SUP).GroupBy(s => s.SUP).ToArray();

                //for (int i = 0; i < getdataSLMTSupSlm.Length; i++)
                //{

                //    GroupSLMTSupSlm.Add(new SelectListItem() { Value = getdataSLMTSupSlm[i].Key.ToString(), Text = getdataSLMTSupSlm[i].Key.ToString() + "/" + getdataSLMTSupSlm[i].Key });
                //}
                ViewBag.GroupSLMTSupSlm = GroupSLMTSupSlm;
                Connection.Dispose();
                command.Dispose();
                Connection.Close();
            }
            return View();
        }
        public JsonResult GetCustomer(string SLMTAB_CODE, string culated)
        {

            //SqlCommand cmdCustomer = new SqlCommand("select * from v_CUSPROV where SLMCOD =N'" + SLMTAB_CODE + "'", Connection);
            //SqlDataReader reCustomer = cmdCustomer.ExecuteReader();
            List<GroupItem> GroupCus = new List<GroupItem>();
            CUSto Model = null;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            if (SLMTAB_CODE != "ALLS")
            {


                SqlCommand cmdCUSOCD = new SqlCommand("select  CUSCOD,CusNam from v_Promo_Statement_Reward where SLMCOD =N'" + SLMTAB_CODE + "' and PYear ='" + DateTime.Now.ToString("yy") + "' GROUP BY  CUSCOD,CusNam", Connection);
                SqlDataReader dr = cmdCUSOCD.ExecuteReader();
                Model = new CUSto();
                Model.Cuscode = "ALL";
                Model.Cusname = "ALL Customer";
                GroupCus.Add(new GroupItem { val = Model });
                while (dr.Read())
                {
                    Model = new CUSto();
                    Model.Cuscode = dr.GetValue(0).ToString();
                    Model.Cusname = dr.GetValue(1).ToString();

                    GroupCus.Add(new GroupItem { val = Model });
                }

                //}
            }
            else
            {

                SqlCommand cmdCUSOCD = new SqlCommand("select  CUSCOD,CusNam from v_Promo_Statement_Reward GROUP BY  CUSCOD,CusNam", Connection);
                SqlDataReader dr = cmdCUSOCD.ExecuteReader();
                Model = new CUSto();
                Model.Cuscode = "ALL";
                Model.Cusname = "ALL Customer";
                GroupCus.Add(new GroupItem { val = Model });
                while (dr.Read())
                {
                    Model = new CUSto();
                    Model.Cuscode = dr.GetValue(0).ToString();
                    Model.Cusname = dr.GetValue(1).ToString();

                    GroupCus.Add(new GroupItem { val = Model });
                }


            }

            //string User = string.Empty;
            //var command = new SqlCommand("P_Find_CUSCOD_Statement", Connection);
            //command.CommandType = CommandType.StoredProcedure;

            //command.Parameters.AddWithValue("@SLMCOD", SLMTAB_CODE);
            //command.Parameters.AddWithValue("@CUSCOD ", "ALL");
            //command.Parameters.AddWithValue("@CALTYP", 4);       
            //command.ExecuteNonQuery();
            //SqlDataReader dr = command.ExecuteReader();
            //Model = new CUS();
            //Model.Cuscode = "ALL";
            //Model.Cusname = "ALL Customer";
            //GroupCus.Add(new GroupItem { val = Model });
            //while (dr.Read())
            //{
            //    Model = new CUS();
            //    Model.Cuscode = dr.GetValue(0).ToString();
            //    Model.Cusname = dr.GetValue(1).ToString();

            //    GroupCus.Add(new GroupItem { val = Model });
            //}
            //GroupCus.Add(new SelectListItem() { Text = "เลือก Customer", Value = "0" });
            //if (reCustomer.FieldCount != 0)
            //{
            //    while (reCustomer.Read())
            //    {

            //        GroupCus.Add(new SelectListItem() { Value = reCustomer.GetValue(1).ToString(), Text = reCustomer.GetValue(1).ToString() + "/" + reCustomer.GetValue(2).ToString() });
            //    }
            //}
            //reCustomer.Dispose();


            //List<SelectListItem> GroupCus = new List<SelectListItem>();
            //GroupCus.Add(new SelectListItem() { Text = "เลือก Customer", Value = "0" });
            //v_SLMTAB modalSLMTAB = new v_SLMTAB();
            //var getCus = db.v_CUSPROV.Where(p => p.SLMCOD == SLMTAB_CODE).ToArray();

            //if (getCus.Length != 0)
            //{
            //    for (int i = 0; i < getCus.Length; i++)
            //    {

            //        GroupCus.Add(new SelectListItem() { Value = getCus[i].CUSCOD.ToString(), Text = getCus[i].CUSCOD.ToString() + "/" + getCus[i].CUSNAM });
            //    }
            //}
            //else { }
            Connection.Close();
            return Json(GroupCus, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCustomerbycustome(string cuscode)
        {

            //SqlCommand cmdCustomer = new SqlCommand("select * from v_CUSPROV where SLMCOD =N'" + SLMTAB_CODE + "'", Connection);
            //SqlDataReader reCustomer = cmdCustomer.ExecuteReader();
            List<GroupItem> GroupCus = new List<GroupItem>();
            CUSto Model = null;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            
                var command = new SqlCommand("P_Search_Promo_Statement_ByCustomer", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Code", cuscode);
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    Model = new CUSto();
                    
                         Model.Cuscode = dr["CUSCOD"].ToString();
                         Model.Cusname = dr["CUSNAM"].ToString();
                      GroupCus.Add(new GroupItem { val = Model });

               }
           
            Connection.Close();
            return Json(GroupCus, JsonRequestBehavior.AllowGet);
        }
    }
    public class CUSto
    {


        public string Cuscode { get; set; }
        public string Cusname { get; set; }
        //public string EmployeeID { get; set; }
        // public string FullName { get; set; }
        //public string GroupName { get; set; }

    }
    public class GroupItem
    {
        public CUSto val { get; set; }
    }
}
