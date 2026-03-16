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
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            //this.Session["UserType"] = "";
            if (this.Session["UserType"] == null)
            {
                return RedirectToAction("LogIn", "Account");

            }

            return View();
        }
        public JsonResult GetdateStockCode(string Name, string Xval, string Prod, string STKGR, string Xcus, string XvalCompany)
        {
            string CUSCOD = string.Empty;
            List<string> StockCode = new List<string>();

            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            var command = new SqlCommand("P_Search_Item_Catalog", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@inCUSCOD", Xcus);
            command.Parameters.AddWithValue("@inSearch", Name);
            command.Parameters.AddWithValue("@inProd", "");
            command.Parameters.AddWithValue("@inSTKGRP", "");
            command.Parameters.AddWithValue("@inFix ", Xval);
            command.Parameters.AddWithValue("@Company", XvalCompany);
            Connection.Open();
            //command.ExecuteNonQuery();
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                //StockCode.Add(reader.GetString(0) + "|" + reader.GetString(1));
                StockCode.Add(dr.GetString(1) + "|" + dr.GetString(2));
            }
            //S20161016                
            dr.Dispose();
            command.Dispose();
            //E20161016
            Connection.Dispose();
            Connection.Close();
            //}
            return Json(StockCode, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetdateStockCodebyval(string usrtyp, string itemsegment, string engine, string cuscod, string yrStart, string yrEnd, string itemno, string maker, string modelno, string submodel, string company, string catalogue, string brand, string textfree)
        {
            string CUSCOD = string.Empty;
            List<string> StockCode = new List<string>();

            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            var command = new SqlCommand("p_Search_item_byVehicle", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Maker", maker);
            command.Parameters.AddWithValue("@Model", modelno);
            command.Parameters.AddWithValue("@SubModel", submodel);
            command.Parameters.AddWithValue("@YrStart", yrStart);
            command.Parameters.AddWithValue("@YrEnd", yrEnd);
            command.Parameters.AddWithValue("@Engine", engine);
            command.Parameters.AddWithValue("@Category", catalogue);
            command.Parameters.AddWithValue("@Item", itemno);
            command.Parameters.AddWithValue("@Brand", brand);
            command.Parameters.AddWithValue("@inCUSCOD", cuscod);
            command.Parameters.AddWithValue("@inSearch", textfree);
            command.Parameters.AddWithValue("@Company", company);
            command.Parameters.AddWithValue("@ItemSegment", itemsegment);
            command.Parameters.AddWithValue("@UsrTyp", usrtyp);
            //command.ExecuteNonQuery();
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                //StockCode.Add(reader.GetString(0) + "|" + reader.GetString(1));
                StockCode.Add(dr.GetString(1));
            }
            //S20161016                
            dr.Dispose();
            command.Dispose();
            //E20161016
            Connection.Dispose();
            Connection.Close();
            //}
            return Json(StockCode, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Getdataitemdetail(string usrtyp, string sortby, string itemsegment, string productgroup, string productline, string engine, string cuscod, string yrStart, string yrEnd, string itemno, string maker, string modelno, string submodel, string company, string catalogue, string brand, string textfree)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            string message = "false";
            // List<SearchitemDetailGetdata> Getdata = new List<SearchitemDetailGetdata>();
            //Searchitem model = null;
            var root = @"..\IMAGE_A\";
            var Getdata = new List<object>();
            try
            {
                var command = new SqlCommand("p_Search_item_byVehicle", Connection);
                command.CommandType = CommandType.StoredProcedure;
                //command.Parameters.AddWithValue("@Maker", "BMW");
                //command.Parameters.AddWithValue("@Model", "Series 3");
                //command.Parameters.AddWithValue("@SubModel", "");
                //command.Parameters.AddWithValue("@YrStart", "");
                //command.Parameters.AddWithValue("@YrEnd", "");
                //command.Parameters.AddWithValue("@Engine", "");
                //command.Parameters.AddWithValue("@Category", "");
                //command.Parameters.AddWithValue("@Item", "");
                //command.Parameters.AddWithValue("@Brand", "");
                //command.Parameters.AddWithValue("@inCUSCOD", "");
                //command.Parameters.AddWithValue("@inSearch", "");
                //command.Parameters.AddWithValue("@Company", "AAC");
                command.Parameters.AddWithValue("@Maker", maker);
                command.Parameters.AddWithValue("@Model", modelno);
                command.Parameters.AddWithValue("@SubModel", submodel);
                command.Parameters.AddWithValue("@YrStart", yrStart);
                command.Parameters.AddWithValue("@YrEnd", yrEnd);
                command.Parameters.AddWithValue("@Engine", engine);
                command.Parameters.AddWithValue("@Category", catalogue);
                command.Parameters.AddWithValue("@OE", itemno);
                command.Parameters.AddWithValue("@Brand", brand);
                command.Parameters.AddWithValue("@inCUSCOD", cuscod);
                command.Parameters.AddWithValue("@inSearch", textfree);
                command.Parameters.AddWithValue("@Company", company);
                command.Parameters.AddWithValue("@PrdGrp", productgroup);
                command.Parameters.AddWithValue("@PrdLne", productline);
                command.Parameters.AddWithValue("@ItemSegment", itemsegment);
                command.Parameters.AddWithValue("@SortBy", sortby);
                command.Parameters.AddWithValue("@UsrTyp", usrtyp);
                command.ExecuteNonQuery();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    //model = new Searchitem();

                    //model.ItemNo = dr["STKCOD"].ToString();
                    //model.Description = dr["Stkdes"].ToString();
                    Getdata.Add(new
                    {
                        ItemNo = dr["STKCOD"].ToString(),
                        Description = dr["Stkdes"].ToString(),
                        Brand = dr["Brand"].ToString(),
                        Company = dr["Company"].ToString(),
                        Price = dr["Price"].ToString(),//ราคาตามเงื่อนไข//
                        SalePrice = dr["PrcPrice"].ToString(),//ราคาโครงสร้าง//
                        PlcPrice = dr["PlcPrice"].ToString(),//ราคาป้าย//
                        SpcPrice = dr["SpcPrice"].ToString(),//ราคาพิเศษ//
                        PromotionCode = dr["PromotionCode"].ToString(),//PromotionCode//	
                        PromoDesc = dr["PromoDesc"].ToString(),
                        PromoPrice = dr["PromoPrice"].ToString(),//ราคาโปรโมชั่น//	
                        Available_Stock = dr["Available_Stock"].ToString(),
                        NewItem = dr["New Item"].ToString(),
                        FavoriteItem = dr["Favorite Item"].ToString(),
                        // PATH = Path.Combine(root, dr["Company"].ToString() + "_" + dr["STKCOD"].ToString()+".png")
                        PATH = Path.Combine(root, dr["IMAGE_NAME"].ToString()),
                        Inactive = dr["Inactive"].ToString(),
                        minord = dr["minord"].ToString(),
                        maxord = dr["maxord"].ToString(),
                        spackuom = dr["SPackUOM"].ToString(),
                        prclst_no = dr["prclst_no"].ToString(),
                        Stock = dr["Stock"].ToString(),
                        ItemClass = dr["ItemClass"].ToString()

                    });

                    //Getdata.Add(new SearchitemDetailGetdata { val = model });
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

    }
}
