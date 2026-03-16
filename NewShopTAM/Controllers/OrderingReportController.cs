using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewShopTAM.Models;
namespace NewShopTAM.Controllers
{
    public class OrderingReportController : Controller
    {
        //
        // GET: /OrderingReport/

        //public ActionResult Index()
        //{
        //    return View();
        //}
        //
        // GET: /OrderingReport/
        // MobileOrderEntities db = new MobileOrderEntities();
        public ActionResult Index(string slm, string encodedCus)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            if (Session["UserID"] == null && Session["UserPassword"] == null)
            {
                return RedirectToAction("LogIn", "Account");
            }
            else
            {

                string usre = Session["UserID"].ToString();
                string Password = Session["UserPassword"].ToString();

                string Slmcodname = string.Empty;
                string Customername = string.Empty;
                string base64CusDecoded = string.Empty;
                string base64Cus = string.Empty;
                string base64slmDecoded = string.Empty;
                string getusertype = string.Empty;
                if (encodedCus != null)
                {
                    string base64slm = slm;


                    byte[] data = System.Convert.FromBase64String(base64slm);
                    base64slmDecoded = System.Text.ASCIIEncoding.ASCII.GetString(data);


                    base64Cus = encodedCus;

                    byte[] dataCus = System.Convert.FromBase64String(base64Cus);
                    base64CusDecoded = System.Text.ASCIIEncoding.ASCII.GetString(dataCus);
                }

                ViewBag.UserId = usre;
                // ViewBag.UserInsert = GroupUserInsert;
                if (this.Session["UserType"] == null)
                {
                    return RedirectToAction("LogIn", "Account");

                }
                else
                {
                    getusertype = Session["UserType"].ToString();
                }
                //var usertype = db.UsrTbl.Where(c => c.UsrID == usre).ToArray();
                //if (usertype.Length == 1)
                //{
                //    getusertype = Convert.ToString(usertype[0].UsrTyp);
                //}
                //else
                //{
                //    var us = db.UsrTbl_History.Where(c => c.UsrID == usre).ToArray();
                //    if (us.Length == 1)
                //    {
                //        getusertype = Convert.ToString(us[0].UsrTyp);
                //    }
                //}
                //get stkgrp
                List<SelectListItem> GroupStkGrp = new List<SelectListItem>();
                SqlCommand cmdstkgrp = new SqlCommand("select * from  v_Mst_StkGrp ", Connection);
                SqlDataReader rev_stkgrp = cmdstkgrp.ExecuteReader();
                while (rev_stkgrp.Read())
                {
                    GroupStkGrp.Add(new SelectListItem()
                    {
                        Value = rev_stkgrp["STKGRP"].ToString(),
                        Text = rev_stkgrp["STKGRP"].ToString() + "/" + rev_stkgrp["GRPNAM"].ToString(),
                    });
                }
                ViewBag.StkGrp = GroupStkGrp;
                //get section
                List<SelectListItem> ListSection = new List<SelectListItem>();
                SqlCommand cmdSection = new SqlCommand("select SEC, SECNAM from  v_MST_SECTION ", Connection);
                SqlDataReader revSection = cmdSection.ExecuteReader();
                while (revSection.Read())
                {
                    ListSection.Add(new SelectListItem()
                    {
                        Value = revSection["SEC"].ToString(),
                        Text = revSection["SEC"].ToString() + "/" + revSection["SECNAM"].ToString(),
                    });
                }
                ViewBag.ListSection = ListSection;

                List<SelectListItem> GroupDYK_Question = new List<SelectListItem>();
                SqlCommand cmddyk = new SqlCommand("select * from  v_DYK_Question order by code", Connection);
                SqlDataReader rev_dyk = cmddyk.ExecuteReader();
                while (rev_dyk.Read())
                {
                    GroupDYK_Question.Add(new SelectListItem()
                    {
                        Value = rev_dyk["Code"].ToString(),
                        Text = rev_dyk["Description"].ToString(),
                    });
                }
                cmddyk.Dispose();
                rev_dyk.Close();
                rev_dyk.Dispose();

                ViewBag.Xdyk = GroupDYK_Question;


                ViewBag.Xgetusertype = getusertype;
                ViewBag.Xcus = base64CusDecoded;
                ViewBag.Xslmcodid = base64slmDecoded;
                ViewBag.XCustomername = Customername;
                ViewBag.XSul = Slmcodname;
                //var link = db.LinkConcern.ToArray();
                //ViewBag.link = link;
                Connection.Close();
                return View();
            }
        }
        public JsonResult Getslmlogin()
        {
            string usre = Session["UserID"].ToString();
            string Password = Session["UserPassword"].ToString();
            List<SLM> SlmList = new List<SLM>();

            SLMc SlmListcount = null;
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

            Connection.Dispose();
            command.Dispose();
            Connection.Close();


            return Json(new { SlmList, SlmListcount }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetdataSlm(string Cus)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            List<CUS> CUSList = new List<CUS>();
            string Slm = string.Empty;
            string Slmcodname = string.Empty;
            string Telcus = string.Empty;
            SqlCommand cmd = new SqlCommand("select Pm.SLMCOD ,Pm.SLMNAM from  v_CUSPROV Pc INNER JOIN v_SLMTAB_SM Pm ON Pc.SLMCOD = Pm.SLMCOD where Pc.CUSCOD =N'" + Cus + "'", Connection);
            SqlDataReader rev_CUSPROV = cmd.ExecuteReader();
            while (rev_CUSPROV.Read())
            {

                Slm = rev_CUSPROV["SLMCOD"].ToString();
                Slmcodname = rev_CUSPROV["SLMNAM"].ToString();

            }

            rev_CUSPROV.Close();
            rev_CUSPROV.Dispose();
            cmd.Dispose();

            Connection.Close();
            return Json(new { Slm, Slmcodname }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetCondition(string dyk)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            List<Condition> CondiList = new List<Condition>();

            SqlCommand cmd = new SqlCommand("select * from v_DYK_Condition_1 where Q_Code =N'" + dyk + "'", Connection);
            SqlDataReader rev_Condi = cmd.ExecuteReader();
            while (rev_Condi.Read())
            {
                CondiList.Add(new Condition()
                {
                    Code = rev_Condi["Code"].ToString(),
                    Description = rev_Condi["Description"].ToString()
                });
            }
            rev_Condi.Close();
            rev_Condi.Dispose();

            cmd.Dispose();
            Connection.Close();
            return Json(CondiList, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetCondition1(string dyk)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            List<Condition> CondiList = new List<Condition>();

            SqlCommand cmd = new SqlCommand("select * from v_DYK_Condition_2 where Q_Code =N'" + dyk + "'", Connection);
            SqlDataReader rev_Condi = cmd.ExecuteReader();
            while (rev_Condi.Read())
            {
                CondiList.Add(new Condition()
                {
                    Code = rev_Condi["Code"].ToString(),
                    Description = rev_Condi["Description"].ToString()
                });
            }
            rev_Condi.Close();
            rev_Condi.Dispose();

            cmd.Dispose();
            Connection.Close();
            return Json(CondiList, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetdataDidYouKnowL(string dyk, string con1, string con2, string con3, string con4, string User)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();

            p_DidYouKnowList Model = null;

            List<List_DidYouKnow> DidYouKnowList = new List<List_DidYouKnow>();
            var cmd = new SqlCommand("p_DidYouKnow", Connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Q_id", dyk);
            cmd.Parameters.AddWithValue("@C1_id", con1);
            cmd.Parameters.AddWithValue("@C2_id", con2);
            cmd.Parameters.AddWithValue("@C3_id", con3);
            cmd.Parameters.AddWithValue("@C4_id", con4);
            cmd.Parameters.AddWithValue("@usrlogin", User);
            SqlDataReader rev_ = cmd.ExecuteReader();
            while (rev_.Read())
            {
                Model = new p_DidYouKnowList();
                Model.Company = rev_["Company"].ToString();
                Model.STKCOD = rev_["STKCOD"].ToString();
                Model.Description = rev_["Description"].ToString();
                // Model.Description = "";
                Model.Stock = rev_["Stock"].ToString();
                Model.SalesPrice = rev_["SalesPrice"].ToString();
                Model.EndPrice = rev_["End Price"].ToString();
                DidYouKnowList.Add(new List_DidYouKnow { val = Model });
            }
            //rev_CUSPROV.Dispose();
            //S20161016
            rev_.Close();
            rev_.Dispose();
            cmd.Dispose();
            //E20161016
            Connection.Close();
            return Json(DidYouKnowList, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetdateStockCode(string Name, string Prod, string STKGR, string Xcus, string XvalCompany)
        {
            string CUSCOD = string.Empty;
            List<string> StockCode = new List<string>();
            if (Xcus == null)
            {

                return Json(new
                {
                    redirectUrl = Url.Action("LogIn", "Account"),
                    isRedirect = true
                });

            }
            else
            {
                // CUSCOD = Session["CUSID"].ToString();
                CUSCOD = Xcus;
                //string query = string.Format("select distinct pc.PEOPLE,pc.PEOPLE +' | ' + CS.CUSNAM as CUSNAM,CS.SLMCOD from prccusgrp pc inner join (select cuscod,cusnam,slmcod from customer cs inner join usrsalepermit sp on cs.slmcod = sp.salepermit  where sp.ulogin =N'chavalit' union (select cuscod,cusnam,slmcod from customer cs where exists (select salepermit from usrsalepermit sp where sp.ulogin = N'chavalit' and salepermit = '*'))) CS on pc.people = CS.CUSCOD where pc.PEOPLE  LIKE '%{0}'", Name);
                //string query = string.Format("select distinct pc.Stkcod,pc.Stkcod + ' | ' + pc.Stkdes as STKNAM from Item pc  where pc.Stkcod LIKE '%{0}%'or pc.Stkdes  LIKE '%{0}%'", Name);
                var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
                SqlConnection Connection = new SqlConnection(connectionString);
                var command = new SqlCommand("P_Search_Item", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@inCUSCOD", CUSCOD);
                command.Parameters.AddWithValue("@inSearch", Name);
                command.Parameters.AddWithValue("@inProd", Prod);
                command.Parameters.AddWithValue("@inSTKGRP", STKGR);
                command.Parameters.AddWithValue("@inFix ", 1);
                command.Parameters.AddWithValue("@Company", XvalCompany);
                Connection.Open();
                command.ExecuteNonQuery();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {

                    //StockCode.Add(reader.GetString(0) + "|" + reader.GetString(1));
                    StockCode.Add(dr.GetString(1));

                }
                //dr.Close();
                //S20161016
                dr.Close();
                dr.Dispose();
                command.Dispose();
                //E20161016
                Connection.Close();
            }
            return Json(StockCode, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDataStkgrpBySec(string sec)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            List<Condition> stkgrpList = new List<Condition>();

            SqlCommand cmd = new SqlCommand("select * from  v_Mst_StkGrp where SEC =N'" + sec + "'", Connection);
            SqlDataReader rev_Condi = cmd.ExecuteReader();
            while (rev_Condi.Read())
            {
                stkgrpList.Add(new Condition()
                {
                    Code = rev_Condi["STKGRP"].ToString(),
                    Description = rev_Condi["STKGRP"].ToString() + "/" + rev_Condi["GRPNAM"].ToString(),
                });
            }
            rev_Condi.Close();
            rev_Condi.Dispose();

            cmd.Dispose();
            Connection.Close();
            return Json(stkgrpList, JsonRequestBehavior.AllowGet);

        }
    }

    public class p_DidYouKnowList
    {
        public string Company { get; set; }
        public string STKCOD { get; set; }
        public string Description { get; set; }
        public string Stock { get; set; }
        public string SalesPrice { get; set; }
        public string EndPrice { get; set; }
    }
    public class List_DidYouKnow
    {
        public p_DidYouKnowList val { get; set; }

    }

    public class Condition
    {
        public string Code { get; set; }
        public string Description { get; set; }

    }
    public class List_Condition
    {
        public Condition val { get; set; }

    }


}

