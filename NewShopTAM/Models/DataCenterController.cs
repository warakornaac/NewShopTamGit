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
using System.Threading.Tasks;
using Dapper;
using System.Runtime.Caching;

namespace NewShopTAM.Models
{
    public class DataCenterController : Controller
    {
        // GET: /DataCenter/
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult Getdiscount(string cuscod)
        {
            string discount = string.Empty;
            string Note = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            var command = new SqlCommand("P_CusDiscount", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@cuscode", cuscod);
            Connection.Open();
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                discount = dr["Discount"].ToString();
                Note = dr["Note"].ToString();
            }
            dr.Dispose();
            command.Dispose();
            Connection.Dispose();
            Connection.Close();
            return Json(new { discount, Note }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Getdate(string Name)
        {
            List<LookupVehicle> List = new List<LookupVehicle>();
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            var command = new SqlCommand("P_Search_LookupVehicle", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Type", Name);
            Connection.Open();
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                List.Add(new LookupVehicle()
                {
                    Code = dr["Maker"].ToString(),
                });
            }
            dr.Close();
            dr.Dispose();
            command.Dispose();
            Connection.Close();
            return Json(List, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Getdatalogincustomer()
        {
            List<CUS> List = new List<CUS>();
            string usre = Session["UserID"].ToString();
            string password = Session["UserPassword"].ToString();
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            var command = new SqlCommand("P_Chk_Customer", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@UsrID", usre);
            command.Parameters.AddWithValue("@Password", password);
            Connection.Open();
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                List.Add(new CUS()
                {
                    CUSCOD = dr["CUSCOD"].ToString(),
                    CUSNAM = dr["CUSNAM"].ToString(),
                    PRO = dr["PRO"].ToString(),
                    ADDR_01 = dr["ADDR_01"].ToString(),
                });
            }
            dr.Close();
            dr.Dispose();
            command.Dispose();
            Connection.Close();
            return Json(List, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCatProductGroup(string com, string cuscod)
        {
            List<CatProductGroup> List = new List<CatProductGroup>();
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            var command = new SqlCommand("P_Search_CatProductGroup", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Com", com);
            command.Parameters.AddWithValue("@inCUSCOD", cuscod);
            Connection.Open();
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                List.Add(new CatProductGroup()
                {
                    Company = dr["Company"].ToString(),
                    ProductGroup = dr["Product Group"].ToString(),
                });
            }
            dr.Close();
            dr.Dispose();
            command.Dispose();
            Connection.Close();
            return Json(List, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCatProductGroupline(string com, string progroup, string brand, string cuscod)
        {
            List<CatProductGroup> List = new List<CatProductGroup>();
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            var command = new SqlCommand("P_Search_CatProductLine", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Com", com);
            command.Parameters.AddWithValue("@progroup", progroup);
            command.Parameters.AddWithValue("@Brand", brand);
            command.Parameters.AddWithValue("@inCUSCOD", cuscod);
            Connection.Open();
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                List.Add(new CatProductGroup()
                {
                    Company = dr["Company"].ToString(),
                    ProductGroup = dr["Product Group"].ToString(),
                    ProductLine = dr["Product Line"].ToString()
                });
            }
            dr.Close();
            dr.Dispose();
            command.Dispose();
            Connection.Close();
            return Json(List, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetdateRelation(string Name, string sty)
        {
            List<LookupVehicle> List = new List<LookupVehicle>();
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            var command = new SqlCommand("P_Search_LookupVehicle", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Type", Name);
            Connection.Open();
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                List.Add(new LookupVehicle()
                {
                    Code = dr["Model"].ToString(),
                });
            }
            dr.Close();
            dr.Dispose();
            command.Dispose();
            Connection.Close();
            return Json(List, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBrand(string Name, string productLine)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            List<Brabdgrop> List = new List<Brabdgrop>();
            SqlCommand cmd = new SqlCommand("P_Search_Brand", Connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Company", Name);
            cmd.Parameters.AddWithValue("@ProductLine", productLine);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                List.Add(new Brabdgrop()
                {
                    CODE = dr["Brand"].ToString(),
                });
            }
            dr.Close();
            dr.Dispose();
            cmd.Dispose();
            Connection.Close();
            return Json(List, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSegment(string Name)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            List<Segmentgrop> List = new List<Segmentgrop>();
            SqlCommand cmd = new SqlCommand("P_Search_Segment", Connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                List.Add(new Segmentgrop()
                {
                    CODE = dr["code"].ToString(),
                    segment = dr["segment"].ToString(),
                    sort = dr["sort"].ToString(),
                });
            }
            dr.Close();
            dr.Dispose();
            cmd.Dispose();
            Connection.Close();
            return Json(List, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetdateStkgrp(string Name)
        {
            List<Stkgrop> List = new List<Stkgrop>();
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            var command = new SqlCommand("P_Search_Mst_StkGrp", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Com", Name);
            Connection.Open();
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                List.Add(new Stkgrop()
                {
                    STKGRP = dr["STKGRP"].ToString(),
                    GRPNAM = dr["GRPNAM"].ToString(),
                    SEC = dr["SEC"].ToString(),
                    PROD = dr["PROD"].ToString(),
                    DEP = dr["DEP"].ToString(),
                    COMPANY = dr["COMPANY"].ToString(),
                });
            }
            dr.Close();
            dr.Dispose();
            command.Dispose();
            Connection.Close();
            return Json(List, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPathImagemain(string inCLM_ID, string CLM_NO)
        {

            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            // var root = @"\Warranty\ImgUpload\";
            var root = @"..\IMAGE_A\";
            var command = new SqlCommand("P_GetPathImage", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@incom", inCLM_ID);
            command.Parameters.AddWithValue("@instk", CLM_NO);
            Connection.Open();
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                //model = new ImageFiles();
                //model.IMAGE_ID = dr["IMAGE_ID"].ToString();
                //model.REQ_NO = dr["REQ_NO"].ToString();
                //model.CLM_NO_SUB = dr["CLM_NO_SUB"].ToString();
                //model.IMAGE_NO = dr["IMAGE_NO"].ToString();
                //model.IMAGE_NAME = dr["IMAGE_NAME"].ToString();
                //model.PATH = dr["PATH"].ToString();
                //  model.PATH = Server.MapPath(@"~\ImgUpload\" + dr["IMAGE_NAME"].ToString());
                //model.PATH = Path.Combine(root, dr["IMAGE_NAME"].ToString());
                //model.PATH = "D:\\Projects\\work spaces\\ClaimWap\\ClaimWap\\ImgUpload\\CM18110012-GDB7224YO-CM18110012-01-01.png";
                // Getdata.Add(new ImageFilesListDetail { val = model });
            }
            //dr.Close();
            //dr.Dispose();
            //command.Dispose();
            //Connection.Close();
            //return Json(new { Getdata }, JsonRequestBehavior.AllowGet);
            return null;
        }
        public JsonResult Getdateslm()
        {
            if (Session["UserID"] == null)
            return Json(new List<SLM>(), JsonRequestBehavior.AllowGet);

            string usre = Session["UserID"].ToString();
            List<SLM> SlmList = new List<SLM>();
            SLM SlmListcount = null;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            var command = new SqlCommand("P_Chk_user", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@UsrID", usre);
            command.Parameters.AddWithValue("@Password", "");
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                SlmList.Add(new SLM()
                {
                    SLMCOD = dr["SLMCOD"].ToString(),
                    SLMNAM = dr["SLMNAM"].ToString(),
                });
            }
            dr.Close();
            dr.Dispose();
            Connection.Dispose();
            command.Dispose();
            Connection.Close();
            return Json(SlmList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Getdateslmbycustomer(string cuscod)
        {
            List<SLM> SlmList = new List<SLM>();
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            var command = new SqlCommand("P_Search_SLM_byCustomer", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@incode", cuscod);
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                SlmList.Add(new SLM()
                {
                    SLMCOD = dr["SLMCOD"].ToString(),
                    SLMNAM = dr["SLMNAM"].ToString(),
                });
            }
            dr.Close();
            dr.Dispose();
            Connection.Dispose();
            command.Dispose();
            Connection.Close();
            return Json(SlmList, JsonRequestBehavior.AllowGet);
        }
        //ดึงรายชื่อ Sale
        public async Task<JsonResult> GetdateslmbysalmcodDapper(string codeslm)
        {
            string cacheKey = $"SLM_{codeslm}";
            // 1) Check cache ก่อน
            var cache = MemoryCache.Default;
            if (cache.Contains(cacheKey))
            {
                var cached = cache.Get(cacheKey) as List<SLM>;
                return Json(cached, JsonRequestBehavior.AllowGet);
            }
            List<SLM> SlmList = new List<SLM>();
            try
            {
                string connectionString =
                    ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    // 2) Call Stored Procedure แบบ Dapper Async
                    var rows = await connection.QueryAsync<dynamic>(
                        "P_Search_SLM",
                        new { incode = codeslm },
                        commandType: System.Data.CommandType.StoredProcedure
                    );
                    // 3) Map data
                    foreach (var r in rows)
                    {
                        SlmList.Add(new SLM
                        {
                            SLMCOD = r.SLMCOD?.ToString(),
                            SLMNAM = r.SLMNAM?.ToString()
                        });
                    }
                }
                //4) Cache 10 นาที
                cache.Add(
                    cacheKey,
                    SlmList,
                    new CacheItemPolicy
                    {
                        AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30)
                    }
                );
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(SlmList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Getdateslmbysalmcod(string codeslm)
        {
            List<SLM> SlmList = new List<SLM>();
            SLM SlmListcount = null;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            var command = new SqlCommand("P_Search_SLM", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@incode", codeslm);
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                SlmList.Add(new SLM()
                {
                    SLMCOD = dr["SLMCOD"].ToString(),
                    SLMNAM = dr["SLMNAM"].ToString(),
                });
            }
            dr.Close();
            dr.Dispose();
            Connection.Dispose();
            command.Dispose();
            Connection.Close();
            //transaction.Commit();
            return Json(SlmList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Getdatabyslm(string SLXX, string SLMNAM)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            List<CUS> CUSList = new List<CUS>();

            SqlCommand cmd = new SqlCommand(@"
            SELECT CUSCOD, CUSNAM, PRO, ADDR_01, CUSTYP,
                   ISNULL(TAMCRLINE,0)   AS TAMCRLINE,
                   ISNULL(TAMBAL,0)      AS TAMBAL,
                   ISNULL(VELOXCRLINE,0) AS VELOXCRLINE,
                   ISNULL(VELOXBAL,0)    AS VELOXBAL
            FROM v_CUSPROV 
            WHERE SLMCOD = @SLMCOD 
            ORDER BY SLMCOD", Connection);

            cmd.Parameters.AddWithValue("@SLMCOD", SLXX);

            this.Session["SLM"] = SLXX;
            this.Session["SLMCOD"] = SLMNAM;

            SqlDataReader rev_CUSPROV = cmd.ExecuteReader();
            while (rev_CUSPROV.Read())
            {
                CUSList.Add(new CUS()
                {
                    CUSCOD = rev_CUSPROV["CUSCOD"].ToString(),
                    CUSNAM = rev_CUSPROV["CUSNAM"].ToString(),
                    PRO = rev_CUSPROV["PRO"].ToString(),
                    ADDR_01 = rev_CUSPROV["ADDR_01"].ToString(),
                    CUSTYP = rev_CUSPROV["CUSTYP"].ToString(),
                    TAMCRLINE = rev_CUSPROV["TAMCRLINE"].ToString(),
                    TAMBAL = rev_CUSPROV["TAMBAL"].ToString(),
                    VELOXCRLINE = rev_CUSPROV["VELOXCRLINE"].ToString(),
                    VELOXBAL = rev_CUSPROV["VELOXBAL"].ToString(),
                });
            }

            rev_CUSPROV.Close();
            rev_CUSPROV.Dispose();
            cmd.Dispose();
            Connection.Close();
            return Json(CUSList, JsonRequestBehavior.AllowGet);
        }
        //แก้ที่ SP
        public JsonResult GetdCustomerCredit(string cusel)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            List<CUS> CUSList = new List<CUS>();
            var cmd = new SqlCommand("P_Customer_credit", Connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CUSCOD", cusel);
            string cusstr = string.Empty;
            SqlDataReader rev_CUSPROV = cmd.ExecuteReader();
            while (rev_CUSPROV.Read())
            {
                CUSList.Add(new CUS()
                {
                    CUSCOD = rev_CUSPROV["CUSCOD"].ToString(),
                    CUSNAM = rev_CUSPROV["CUSNAM"].ToString(),
                    PRO = rev_CUSPROV["PRO"].ToString(),
                    ADDR_01 = rev_CUSPROV["ADDR_01"].ToString(),
                    ADDR_02 = rev_CUSPROV["ADDR_02"].ToString(),
                    CUSTYP = rev_CUSPROV["CUSTYP"].ToString(),
                    TAMCRLINE = rev_CUSPROV["TAMCRLINE"].ToString(),
                    TAMBAL = rev_CUSPROV["TAMBAL"].ToString(),
                    VELOXCRLINE = rev_CUSPROV["VELOXCRLINE"].ToString(),
                    VELOXBAL = rev_CUSPROV["VELOXBAL"].ToString(),
                    SLMCOD = rev_CUSPROV["SLMCOD"].ToString(),
                    INACTIVE = rev_CUSPROV["INACTIVE"].ToString(),
                    BLOCKED = rev_CUSPROV["Blocked"].ToString(),
                    TAMPAYTRM = rev_CUSPROV["TAMPAYTRM"].ToString(),
                    VELOXPAYTRM = rev_CUSPROV["VELOXPAYTRM"].ToString(),
                    TELNUM = rev_CUSPROV["TELNUM"].ToString(),
                });
            }
            rev_CUSPROV.Close();
            rev_CUSPROV.Dispose();
            cmd.Dispose();
            Connection.Close();
            return Json(CUSList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetdateCus(string Cus, string Slm)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            List<CUS> CUSList = new List<CUS>();
            string SLMCOD = string.Empty;
            string query = string.Empty;
            if (Slm == "0" || Slm == null || Slm == "(ALL)")
            {
                SLMCOD = "";
                query = string.Format("select distinct pc.CUSCOD,pc.CUSCOD + ' | ' + pc.CUSNAM from v_CUSPROV pc    where  pc.CUSCOD LIKE '%{0}%'or pc.CUSNAM  LIKE '%{0}%'", Cus);
            }
            else
            {
                SLMCOD = Slm;
                query = string.Format("select distinct pc.CUSCOD,pc.CUSCOD + ' | ' + pc.CUSNAM from v_CUSPROV pc    where  pc.SLMCOD ='" + SLMCOD + "' and (pc.CUSCOD LIKE '%{0}%'or pc.CUSNAM  LIKE '%{0}%')", Cus);
            }
            List<string> Code = new List<string>();
            using (SqlCommand cmd = new SqlCommand(query, Connection))
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Code.Add(reader.GetString(1));
                }
                reader.Close();
                reader.Dispose();
                cmd.Dispose();
            }
            Connection.Close();
            return Json(Code, JsonRequestBehavior.AllowGet);
        }
        //ดึงข้อมูลร้านค้า
        public async Task<JsonResult> GetdatabyCusDapper(string cusel)
        {
            string cacheKey = $"GetdatabyCus_{cusel}";
            // 1) Check Cache ก่อน
            var cache = MemoryCache.Default;
            if (cache.Contains(cacheKey))
            {
                var cached = cache.Get(cacheKey) as List<CUS>;
                return Json(cached, JsonRequestBehavior.AllowGet);
            }
            List<CUS> CUSList = new List<CUS>();
            string connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    // 2) Query แบบ Dapper Async ปลอดภัยกว่า string concat
                    var rows = await connection.QueryAsync<dynamic>(
                    @"SELECT 
                        CUSCOD, CUSNAM, PRO, ADDR_01, ADDR_02, CUSTYP,
                        TAMCRLINE, TAMBAL, VELOXCRLINE, VELOXBAL, SLMCOD, 
                        INACTIVE, Blocked, TAMPAYTRM, VELOXPAYTRM, TELNUM, Rating,
                        [Hierarchy1 (Market Segment)] AS H1,
                        [Hierarchy2 (Channel)] AS H2,
                        [Hierarchy3 (Bussiness Type)] AS H3
                      FROM v_CUSPROV 
                      WHERE CUSCOD = @CUSCOD
                      ORDER BY SLMCOD",
                    new { CUSCOD = cusel }
                );
                    foreach (var r in rows)
                    {
                        CUSList.Add(new CUS
                        {
                            CUSCOD = r.CUSCOD?.ToString(),
                            CUSNAM = r.CUSNAM?.ToString(),
                            PRO = r.PRO?.ToString(),
                            ADDR_01 = r.ADDR_01?.ToString(),
                            ADDR_02 = r.ADDR_02?.ToString(),
                            CUSTYP = r.CUSTYP?.ToString(),
                            TAMCRLINE = r.TAMCRLINE?.ToString(),
                            TAMBAL = r.TAMBAL?.ToString(),
                            VELOXCRLINE = r.VELOXCRLINE?.ToString(),
                            VELOXBAL = r.VELOXBAL?.ToString(),
                            BLOCKED = r.Blocked?.ToString(),
                            TAMPAYTRM = r.TAMPAYTRM?.ToString(),
                            VELOXPAYTRM = r.VELOXPAYTRM?.ToString(),
                            SLMCOD = r.SLMCOD?.ToString(),
                            INACTIVE = r.INACTIVE?.ToString(),
                            TELNUM = r.TELNUM?.ToString(),
                            RATING = r.Rating?.ToString(),
                            Hierarchy1_Market_Segment = r.H1?.ToString(),
                            Hierarchy2_Channel = r.H2?.ToString(),
                            Hierarchy3_Bussiness_Type = r.H3?.ToString()
                        });
                    }
                }
                // 3) Save Cache 10 นาที
                cache.Add(
                    cacheKey,
                    CUSList,
                    new CacheItemPolicy
                    {
                        AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(10)
                    }
                );
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(CUSList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetdatabyCus(string cusel)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            List<CUS> CUSList = new List<CUS>();
            SqlCommand cmd = new SqlCommand(@"
        SELECT CUSCOD, CUSNAM, PRO, ADDR_01, ADDR_02, CUSTYP, 
               TAMCRLINE, TAMBAL, VELOXCRLINE, VELOXBAL, 
               SLMCOD, INACTIVE, Blocked,
               TAMPAYTRM, VELOXPAYTRM, TELNUM, Rating,
               [Hierarchy1 (Market Segment)], 
               [Hierarchy2 (Channel)], 
               [Hierarchy3 (Bussiness Type)]
        FROM v_CUSPROV 
        WHERE CUSCOD = @CUSCOD 
        ORDER BY SLMCOD", Connection);

            cmd.Parameters.AddWithValue("@CUSCOD", (object)cusel ?? DBNull.Value);

            SqlDataReader rev_CUSPROV = cmd.ExecuteReader();
            while (rev_CUSPROV.Read())
            {
                CUSList.Add(new CUS()
                {
                    CUSCOD = rev_CUSPROV["CUSCOD"].ToString(),
                    CUSNAM = rev_CUSPROV["CUSNAM"].ToString(),
                    PRO = rev_CUSPROV["PRO"].ToString(),
                    ADDR_01 = rev_CUSPROV["ADDR_01"].ToString(),
                    ADDR_02 = rev_CUSPROV["ADDR_02"].ToString(),
                    CUSTYP = rev_CUSPROV["CUSTYP"].ToString(),
                    TAMCRLINE = rev_CUSPROV["TAMCRLINE"].ToString(),
                    TAMBAL = rev_CUSPROV["TAMBAL"].ToString(),
                    VELOXCRLINE = rev_CUSPROV["VELOXCRLINE"].ToString(),
                    VELOXBAL = rev_CUSPROV["VELOXBAL"].ToString(),
                    BLOCKED = rev_CUSPROV["Blocked"].ToString(),
                    TAMPAYTRM = rev_CUSPROV["TAMPAYTRM"].ToString(),
                    VELOXPAYTRM = rev_CUSPROV["VELOXPAYTRM"].ToString(),
                    SLMCOD = rev_CUSPROV["SLMCOD"].ToString(),
                    INACTIVE = rev_CUSPROV["INACTIVE"].ToString(),
                    TELNUM = rev_CUSPROV["TELNUM"].ToString(),
                    RATING = rev_CUSPROV["Rating"].ToString(),
                    Hierarchy1_Market_Segment = rev_CUSPROV["Hierarchy1 (Market Segment)"].ToString(),
                    Hierarchy2_Channel = rev_CUSPROV["Hierarchy2 (Channel)"].ToString(),
                    Hierarchy3_Bussiness_Type = rev_CUSPROV["Hierarchy3 (Bussiness Type)"].ToString(),
                });
            }
            rev_CUSPROV.Close();
            rev_CUSPROV.Dispose();
            cmd.Dispose();
            Connection.Close();
            return Json(CUSList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetdateCusCode(string Name, string Slm)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            List<CUS> CUSList = new List<CUS>();
            string SLMCOD = string.Empty;
            string query = string.Empty;
            if (Slm == "ALL")
            {
                SLMCOD = "";
                query = string.Format("select distinct pc.CUSCOD,pc.CUSCOD + ' | ' + pc.CUSNAM + '----Address ' +  pc.ADDR_01+ ' | ' + pc.PRO from v_CUSPROV pc    where  pc.CUSCOD LIKE '%{0}%'or pc.CUSNAM  LIKE '%{0}%'", Name);
            }
            else
            {
                SLMCOD = Slm;
                query = string.Format("select distinct pc.CUSCOD,pc.CUSCOD + ' | ' + pc.CUSNAM+ ' ----Address' +  pc.ADDR_01+ ' | ' + pc.PRO from v_CUSPROV pc    where  pc.SLMCOD ='" + SLMCOD + "' and (pc.CUSCOD LIKE '%{0}%'or pc.CUSNAM  LIKE '%{0}%')", Name);
            }
            List<string> Code = new List<string>();
            using (SqlCommand cmd = new SqlCommand(query, Connection))
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Code.Add(reader.GetString(1));
                }
                reader.Close();
                reader.Dispose();
                cmd.Dispose();
                Connection.Close();
            }
            return Json(Code, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetdataCuslogincus(string cusel)
        {
            List<logincutomer> List = new List<logincutomer>();
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            var command = new SqlCommand("P_Search_CusByCustomer", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@inCUS", cusel);
            Connection.Open();
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                List.Add(new logincutomer()
                {
                    EmpID = dr["EmpID"].ToString(),
                    company = dr["company"].ToString(),
                    UsrID = dr["UsrID"].ToString(),
                    initials = dr["initials"].ToString(),
                    Department = dr["Department"].ToString(),
                    Position = dr["Position"].ToString(),
                    EMail = dr["EMail"].ToString(),
                    CUSCOD = dr["CUSCOD"].ToString(),
                    SUP = dr["SUP"].ToString(),
                    UsrTyp = dr["UsrTyp"].ToString(),
                    SLMCOD = dr["SLMCOD"].ToString(),
                    SLMNAM = dr["SLMNAM"].ToString(),
                    PasswordExpiredDate = dr["Password Expired Date"].ToString(),
                    DatetoExpire = dr["Date to Expire"].ToString(),
                    SLMPhone = dr["SLMPhone"].ToString(),
                    SalesCo = dr["SalesCo"].ToString(),
                    SalesCoPhone = dr["SalesCoPhone"].ToString(),
                });
            }
            dr.Close();
            dr.Dispose();
            command.Dispose();
            Connection.Close();
            return Json(List, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetdataCompanyInformation(string Slm)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            var Getdata = new List<object>();
            SqlCommand cmd = new SqlCommand("select *    from dbo.v_SLMTAB  where [SLMCOD] ='" + Slm + "' order by [SLMCOD] ", Connection);
            SqlDataReader rev_Mod = cmd.ExecuteReader();
            while (rev_Mod.Read())
            {
                Getdata.Add(new
                {
                    SLMCOD = rev_Mod["SLMCOD"].ToString(),
                    SLMNAM = rev_Mod["SLMNAM"].ToString(),
                    Phone = rev_Mod["Phone"].ToString(),
                    SalesCo = rev_Mod["SalesCo"].ToString(),
                    SalesCoPhone = rev_Mod["SalesCoPhone"].ToString(),
                });
            }
            rev_Mod.Close();
            rev_Mod.Dispose();
            cmd.Dispose();
            Connection.Close();
            return Json(Getdata, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetdataCompanyInfo()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            var Getdata = new List<object>();
            SqlCommand cmd = new SqlCommand("select *    from dbo.Company ", Connection);
            SqlDataReader rev_Mod = cmd.ExecuteReader();
            while (rev_Mod.Read())
            {
                Getdata.Add(new
                {
                    Company = rev_Mod["Company"].ToString(),
                    Name = rev_Mod["Name"].ToString(),
                    Address1 = rev_Mod["Address1"].ToString(),
                    Address2 = rev_Mod["Address2"].ToString(),
                    Phone = rev_Mod["Phone"].ToString(),
                    Fax = rev_Mod["Fax"].ToString(),
                    Tax = rev_Mod["Tax"].ToString(),
                    Bank1 = rev_Mod["Bank1"].ToString(),
                    Bank2 = rev_Mod["Bank2"].ToString(),

                });
            }
            rev_Mod.Close();
            rev_Mod.Dispose();
            cmd.Dispose();
            Connection.Close();
            return Json(Getdata, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetdateContorder(string Slm, string cus, string Usertype, string UserIn)
        {
            List<Listslmcount> Getdata = new List<Listslmcount>();
            SLMc SlmListcount = null;
            string SLMc = string.Empty;
            string Cusc = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            var commandCount_Orde = new SqlCommand("P_Count_Order_By_Sales_catalog", Connection);
            commandCount_Orde.CommandType = CommandType.StoredProcedure;
            commandCount_Orde.Parameters.AddWithValue("@Salecode", (object)Slm ?? DBNull.Value);
            commandCount_Orde.Parameters.AddWithValue("@Cuscode", (object)cus ?? DBNull.Value);
            commandCount_Orde.Parameters.AddWithValue("@Usertype", (object)Usertype ?? DBNull.Value);
            commandCount_Orde.Parameters.AddWithValue("@UserIn", (object)UserIn ?? DBNull.Value);
            SqlDataReader drCount_Orde = commandCount_Orde.ExecuteReader();
            while (drCount_Orde.Read())
            {
                SlmListcount = new SLMc();
                SlmListcount.SumQty = drCount_Orde["SumQty"].ToString();
                SlmListcount.Countrow = drCount_Orde["Countrow"].ToString();
                SlmListcount.CountPN = drCount_Orde["CountPN"].ToString();
                SlmListcount.Status = drCount_Orde["Status"].ToString();
                Getdata.Add(new Listslmcount { val = SlmListcount });
            }
            drCount_Orde.Dispose();
            commandCount_Orde.Dispose();
            Connection.Dispose();
            Connection.Close();
            return Json(new { Getdata }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetdataPlusItem(string Nodisplay, string strcustome)
        {
            string com = string.Empty;
            string substkgrp = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            string message = "false";
            var Getdata = new List<object>();
            try
            {
                var root = @"..\IMAGE_A\";
                var command = new SqlCommand("p_Search_Item_Byvehicle_PlusItem", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@pCusCod", strcustome);
                command.Parameters.AddWithValue("@pStkcod", Nodisplay);
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    Getdata.Add(new
                    {
                        Company = dr["Company"].ToString(),
                        STKCOD = dr["STKCOD"].ToString(),
                        Description = dr["Description"].ToString(),
                        EndPrice = dr["End Price"].ToString(),
                        PATH = Path.Combine(root, dr["IMAGE_NAME"].ToString())
                    });
                }
                dr.Close();
                dr.Dispose();
                command.Dispose();
                Connection.Close();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(Getdata, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetdataContinue_Newitem(string strcustome)
        {
            string com = string.Empty;
            string substkgrp = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            string message = "false";
            var Getdata = new List<object>();
            try
            {
                var root = @"..\IMAGE_A\";
                var command = new SqlCommand("p_Search_NewItem", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@pCusCod", strcustome);
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    Getdata.Add(new
                    {
                        Company = dr["Company"].ToString(),
                        STKCOD = dr["STKCOD"].ToString(),
                        Description = dr["STKDES"].ToString(),
                        EndPrice = dr["End Price"].ToString(),
                        PATH = Path.Combine(root, dr["IMAGE_NAME"].ToString())
                    });
                }
                dr.Close();
                dr.Dispose();
                command.Dispose();
                Connection.Close();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(Getdata, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetdataContinue_Stkgrp(string Nodisplay, string strcustome)
        {
            string com = string.Empty;
            string substkgrp = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            string message = "false";
            var Getdata = new List<object>();
            try
            {
                var root = @"..\IMAGE_A\";
                var command = new SqlCommand("p_Search_Item_Continue_Stkgrp", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@pCusCod", strcustome);
                command.Parameters.AddWithValue("@pStkcod", Nodisplay);
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    Getdata.Add(new
                    {
                        Company = dr["Company"].ToString(),
                        STKCOD = dr["STKCOD"].ToString(),
                        Description = dr["Description"].ToString(),
                        Stock = dr["Stock"].ToString(),
                        EndPrice = dr["End Price"].ToString(),
                        PATH = Path.Combine(root, dr["IMAGE_NAME"].ToString())
                    });
                }
                dr.Close();
                dr.Dispose();
                command.Dispose();
                Connection.Close();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(Getdata, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Getdatailorder(string codval)
        {
            List<ListsDetailSLM> Getdata = new List<ListsDetailSLM>();
            DetailSLM DetailLists = null;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            var command = new SqlCommand("P_detail_Order_By_Sales_catalog", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Salecode", codval);
            SqlDataReader dr_Orde = command.ExecuteReader();
            while (dr_Orde.Read())
            {
                DetailLists = new DetailSLM();
                DetailLists.salmman = dr_Orde["SLMCODE"].ToString();
                DetailLists.salmmanname = dr_Orde["SLMNAM"].ToString();
                DetailLists.customer = dr_Orde["CUSCOD"].ToString();
                DetailLists.customername = dr_Orde["CUSNAM"].ToString();
                DetailLists.Countrow = dr_Orde["Countrow"].ToString();
                DetailLists.sumqty = dr_Orde["SumQty"].ToString();
                Getdata.Add(new ListsDetailSLM { val = DetailLists });
            }
            dr_Orde.Dispose();
            command.Dispose();
            Connection.Dispose();
            Connection.Close();
            return Json(new { Getdata }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Getdatailorderwaitapprv(string slmcod, string cuscod)
        {
            List<DetailApprvSLM> Getdata = new List<DetailApprvSLM>();
            DetailSLM DetailLists = null;
            string message = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {
                var command = new SqlCommand("P_detail_Order_waitApprov_By_Sales_catalog", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Salecode", slmcod);
                command.Parameters.AddWithValue("@Cuscod", cuscod);
                SqlDataReader dr_Orde = command.ExecuteReader();
                while (dr_Orde.Read())
                {
                    Getdata.Add(new DetailApprvSLM
                    {
                        CUSCOD = dr_Orde["CUSCOD"] != DBNull.Value ? dr_Orde["CUSCOD"].ToString() : string.Empty,
                        CUSNAM = dr_Orde["CUSNAM"] != DBNull.Value ? dr_Orde["CUSNAM"].ToString() : string.Empty,
                        SLMCOD = dr_Orde["SLMCOD"] != DBNull.Value ? dr_Orde["SLMCOD"].ToString() : string.Empty,
                        SLMNAM = dr_Orde["SLMNAM"] != DBNull.Value ? dr_Orde["SLMNAM"].ToString() : string.Empty,
                        STKCOD = dr_Orde["STKCOD"] != DBNull.Value ? dr_Orde["STKCOD"].ToString() : string.Empty,
                        STKDES = dr_Orde["STKDES"] != DBNull.Value ? dr_Orde["STKDES"].ToString() : string.Empty,
                        qty = dr_Orde["qty"] != DBNull.Value ? dr_Orde["qty"].ToString() : string.Empty,
                        Item_typ = dr_Orde["Item_Type"] != DBNull.Value ? dr_Orde["Item_Type"].ToString() : string.Empty
                    });
                }
                dr_Orde.Dispose();
                command.Dispose();
                Connection.Dispose();
                message = "Y";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            Connection.Close();
            return Json(new { message = message, Getdata }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Getdatailorderbycustomer(string codval)
        {
            List<ListsDetailSLM> Getdata = new List<ListsDetailSLM>();
            DetailSLM DetailLists = null;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            var command = new SqlCommand("P_detail_Order_By_Cus_catalog", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Cuscod", codval);
            SqlDataReader dr_Orde = command.ExecuteReader();
            while (dr_Orde.Read())
            {
                DetailLists = new DetailSLM();
                DetailLists.salmman = dr_Orde["SLMCODE"].ToString();
                DetailLists.salmmanname = dr_Orde["SLMNAM"].ToString();
                DetailLists.customer = dr_Orde["CUSCOD"].ToString();
                DetailLists.customername = dr_Orde["CUSNAM"].ToString();
                DetailLists.Countrow = dr_Orde["Countrow"].ToString();
                DetailLists.sumqty = dr_Orde["SumQty"].ToString();
                Getdata.Add(new ListsDetailSLM { val = DetailLists });
            }
            dr_Orde.Dispose();
            command.Dispose();
            Connection.Dispose();
            Connection.Close();
            return Json(new { Getdata }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetdataCusShipping(string XXcus)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            List<ItemListshipto> cusshipping = new List<ItemListshipto>();
            shipto model = null;
            SqlCommand cmd = new SqlCommand("select *    from dbo.v_NVcust_ShiptoAddr_TAM  where [customer No_] ='" + XXcus + "' order by [Code] ", Connection);
            SqlDataReader rev_Mod = cmd.ExecuteReader();
            while (rev_Mod.Read())
            {
                model = new shipto();
                model.customer = rev_Mod["Customer No_"].ToString();
                model.code = rev_Mod["Code"].ToString();
                model.name = rev_Mod["Name"].ToString();
                model.name2 = rev_Mod["Name 2"].ToString();
                model.address = rev_Mod["Address"].ToString();
                model.address2 = rev_Mod["Address 2"].ToString();
                model.city = rev_Mod["City"].ToString();
                model.postcode = rev_Mod["Post code"].ToString();
                cusshipping.Add(new ItemListshipto { val = model });
            }
            rev_Mod.Close();
            rev_Mod.Dispose();
            cmd.Dispose();
            Connection.Close();
            return Json(cusshipping, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetdateStockCodedropdownlist(string Prod, string STKGR, string Xcus, string XvalCompany, string Xval, string names)
        {
            string CUSCOD = string.Empty;
            List<ItemListdropList> StockCode = new List<ItemListdropList>();
            ItemListdrop DetailLists = null;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            var command = new SqlCommand("P_Search_Item_dropdownlist", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@inProd", Prod);
            command.Parameters.AddWithValue("@inSTKGRP", STKGR);
            command.Parameters.AddWithValue("@inFix", Xval);
            command.Parameters.AddWithValue("@Company", XvalCompany);
            command.Parameters.AddWithValue("@inName", names);
            Connection.Open();
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                DetailLists = new ItemListdrop();
                DetailLists.No = dr["STKCOD"].ToString();
                DetailLists.STKDES = dr["STKDES"].ToString();
                StockCode.Add(new ItemListdropList { val = DetailLists });
            }
            dr.Close();
            dr.Dispose();
            command.Dispose();
            Connection.Close();
            return Json(StockCode, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProd()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            List<Prod> List = new List<Prod>();
            SqlCommand cmd = new SqlCommand("P_Search_PROD", Connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                List.Add(new Prod()
                {
                    CODE = dr["PROD"].ToString(),
                    NAME = dr["PRODNAME"].ToString()
                });
            }
            dr.Close();
            dr.Dispose();
            cmd.Dispose();
            Connection.Close();
            return Json(List, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetdateStkgrpByProd(string Name)
        {
            List<Stkgrop> List = new List<Stkgrop>();
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            var command = new SqlCommand("P_Search_Mst_StkGrp_ByPROD", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@PROD", Name);
            Connection.Open();
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                List.Add(new Stkgrop()
                {
                    STKGRP = dr["STKGRP"].ToString(),
                    GRPNAM = dr["GRPNAM"].ToString(),
                    SEC = dr["SEC"].ToString(),
                    PROD = dr["PROD"].ToString(),
                    DEP = dr["DEP"].ToString(),
                    COMPANY = dr["COMPANY"].ToString(),
                });
            }
            dr.Close();
            dr.Dispose();
            command.Dispose();
            Connection.Close();
            return Json(List, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetdataCusAmt(string strcustome)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            string message = "false";
            var Getdata = new List<object>();
            try
            {
                var command = new SqlCommand("p_Search_CusAmt", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@inCUSCOD", strcustome);
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    Getdata.Add(new
                    {
                        Cuscod = dr["Cuscod"].ToString(),
                        Company = dr["Company"].ToString(),
                        AmtYTD = dr["Amt YTD"].ToString(),
                        AmtMTD = dr["Amt MTD"].ToString(),
                        Jan = dr["Jan"].ToString(),
                        Feb = dr["Feb"].ToString(),
                        Mar = dr["Mar"].ToString(),
                        Apr = dr["Apr"].ToString(),
                        May = dr["May"].ToString(),
                        Jun = dr["Jun"].ToString(),
                        Jul = dr["Jul"].ToString(),
                        Aug = dr["Aug"].ToString(),
                        Sep = dr["Sep"].ToString(),
                        Oct = dr["Oct"].ToString(),
                        Nov = dr["Nov"].ToString(),
                        Dec = dr["Dec"].ToString(),
                    });
                }
                dr.Close();
                dr.Dispose();
                command.Dispose();
                Connection.Close();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(Getdata, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetdataCusAmtchild(string strcustome)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            string message = "false";
            var Getdata = new List<object>();
            try
            {
                var command = new SqlCommand("p_Search_CusAmt_child", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@inCUSCOD", strcustome);
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    Getdata.Add(new
                    {
                        Cuscod = dr["Cuscod"].ToString(),
                        Company = dr["Company"].ToString(),
                        AmtYTD = dr["Amt YTD"].ToString(),
                        AmtMTD = dr["Amt MTD"].ToString(),
                        Jan = dr["Jan"].ToString(),
                        Feb = dr["Feb"].ToString(),
                        Mar = dr["Mar"].ToString(),
                        Apr = dr["Apr"].ToString(),
                        May = dr["May"].ToString(),
                        Jun = dr["Jun"].ToString(),
                        Jul = dr["Jul"].ToString(),
                        Aug = dr["Aug"].ToString(),
                        Sep = dr["Sep"].ToString(),
                        Oct = dr["Oct"].ToString(),
                        Nov = dr["Nov"].ToString(),
                        Dec = dr["Dec"].ToString(),
                    });
                }
                dr.Close();
                dr.Dispose();
                command.Dispose();
                Connection.Close();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(Getdata, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetdataCusAmtTop20(string cuscod)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            string message = "false";
            var Getdata = new List<object>();
            try
            {
                var command = new SqlCommand("p_Search_CusItmGrp", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@inCUSCOD", cuscod);
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    Getdata.Add(new
                    {
                        Rowid = dr["Rowid"].ToString(),
                        Company = dr["Company"].ToString(),
                        Cuscod = dr["CUSCOD"].ToString(),
                        Stkcod = dr["STKCOD"].ToString(),
                        Stkdes = dr["STKDES"].ToString(),
                        Qty = dr["Qty"].ToString(),
                        Amt = dr["AMT"].ToString()
                    });
                }
                dr.Close();
                dr.Dispose();
                command.Dispose();
                Connection.Close();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(Getdata, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetdataPromotion_Cus(string strcustome, string period)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            string message = "false";
            var Getdata = new List<object>();
            try
            {
                var command = new SqlCommand("P_Search_Promotion_Cus", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@inCUSCOD", strcustome);
                command.Parameters.AddWithValue("@Period", period);
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    Getdata.Add(new
                    {
                        CUSCOD = dr["CUSCOD"].ToString(),
                        company = dr["company"].ToString(),
                        Promotion_Name = dr["Promotion_Name"].ToString(),
                        StartDate = dr["StartDate"].ToString(),
                        EndDate = dr["EndDate"].ToString(),
                        Condition = dr["Condition"].ToString(),
                        INVAMT = dr["Invoice Amount"].ToString(),
                        PaidAmt = dr["Invoice Paid"].ToString(),
                        Reward = dr["Reward"].ToString(),
                        RemainAmt = dr["Remaining Amount"].ToString(),
                    });
                }
                dr.Close();
                dr.Dispose();
                command.Dispose();
                Connection.Close();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(Getdata, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetdataWarrantyClaim_Cus_count(string strcustome)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            string message = "false";
            var Getdata = new List<object>();
            try
            {
                var command = new SqlCommand("P_Search_WarrantyClaim_Cus_count", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@inCUSCOD", strcustome);
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    Getdata.Add(new
                    {
                        A = dr["Pending"].ToString(),
                        B = dr["Under Review"].ToString(),
                        C = dr["Awaiting Replacement"].ToString(),
                        D = dr["Replacement Sent"].ToString(),
                    });
                }
                dr.Close();
                dr.Dispose();
                command.Dispose();
                Connection.Close();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(Getdata, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetdataWarrantyClaim_Cus(string strcustome, string tap)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            string message = "false";
            var Getdata = new List<object>();
            try
            {
                var command = new SqlCommand("P_Search_WarrantyClaim_Cus", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@inCUSCOD", strcustome);
                command.Parameters.AddWithValue("@instatus", tap);
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    Getdata.Add(new
                    {
                        REQ_NO = dr["REQ_NO"].ToString(),
                        CLM_NO_SUB = dr["CLM_NO_SUB"].ToString(),
                        REQ_DATE = dr["REQ_DATE"].ToString(),
                        ReceiveDate = dr["FormatReceiveDate"].ToString(),
                        CLM_COMPANY = dr["CLM_COMPANY"].ToString(),
                        CUSCOD = dr["CUSCOD"].ToString(),
                        STKCOD = dr["STKCOD"].ToString(),
                        STKDES = dr["STKDES"].ToString(),
                        Qty = dr["Qty"].ToString(),
                        InvoiceNo = dr["Invoice No"].ToString(),
                        InvoiceDate = dr["Invoice Date"].ToString(),
                        Symptom = dr["Symptom"].ToString(),
                        Request = dr["Request"].ToString(),
                        DueDate = dr["Due Date"].ToString(),
                        Checking = dr["Checking"].ToString(),
                        ApproveDate = dr["Approve Date"].ToString(),
                        Status = dr["Status"].ToString(),
                    });
                }
                dr.Close();
                dr.Dispose();
                command.Dispose();
                Connection.Close();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(Getdata, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetdataSessionlogin(string UsrID, string SessionId)
        {
            string StrStstuslogin = string.Empty;
            string message = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("P_Update_SessionId_Customer", conn);
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UsrID", UsrID);
                cmd.Parameters.AddWithValue("@SessionId", SessionId);
                SqlParameter returnValue = new SqlParameter("@outResult", SqlDbType.NVarChar, 100);
                returnValue.Direction = System.Data.ParameterDirection.Output;
                cmd.Parameters.Add(returnValue);
                cmd.ExecuteReader();
                StrStstuslogin = returnValue.Value.ToString();
                cmd.Dispose();
                conn.Close();
            }
            catch (Exception ex)
            {
                message = ex.Message + '/' + ex.Source + '/' + ex.HelpLink + '/' + ex.HResult;
            }
            return Json(new { message, StrStstuslogin }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetdataPrivilege(string UsrID, string cuscod)
        {
            string StrStstuslogin = string.Empty;
            string message = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("P_privilege_Customer", conn);
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UsrID", UsrID);
                cmd.Parameters.AddWithValue("@cuscod", cuscod);
                SqlParameter returnValue = new SqlParameter("@outResult", SqlDbType.NVarChar, 100);
                returnValue.Direction = System.Data.ParameterDirection.Output;
                cmd.Parameters.Add(returnValue);
                cmd.ExecuteReader();
                StrStstuslogin = returnValue.Value.ToString();
                cmd.Dispose();
                conn.Close();
            }
            catch (Exception ex)
            {
                message = ex.Message + '/' + ex.Source + '/' + ex.HelpLink + '/' + ex.HResult;
            }
            return Json(new { message, StrStstuslogin }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetListCompany()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            var companyList = new List<string>();
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(@"
                                            SELECT Company
                                            FROM Company
                                            ORDER BY Seq", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        companyList.Add(reader["Company"]?.ToString());
                    }
                }
            }
            companyList.Insert(0, "ALL");
            return Json(companyList, JsonRequestBehavior.AllowGet);
        }
    }
}