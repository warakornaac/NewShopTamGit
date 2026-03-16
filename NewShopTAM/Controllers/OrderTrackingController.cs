using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewShopTAM.Controllers
{
    public class OrderTrackingController : Controller
    {
        //
        // GET: /OrderTracking/

        public ActionResult Index(string act, string exparrive, string Pack, string CarID, string Lat, string Long, string Sta, string StartWork, string EndWork, string Remark)
        {
            string @inact = string.Empty;
            string Docdisplay = string.Empty;
            string CUSCOD = string.Empty;
            string @inPack = string.Empty;
            string @inCarID = string.Empty;
            string @inLat = string.Empty;
            string @inLong = string.Empty;
            string @inSta = string.Empty;
            string @inStartWork = string.Empty;
            string @inEndWork = string.Empty;
            string @inRemark = string.Empty;
            string @inexparrive = string.Empty;
            string messagereturn = string.Empty;
            // Docdisplay = Request.QueryString["encode"];
            if (Docdisplay != null)
            {
                //string basePack = Pack;

                //byte[] data = System.Convert.FromBase64String(Docdisplay);
                @inPack = Pack;
                @inCarID = CarID;
                @inLat = Lat;
                @inLong = Long;
                @inSta = Sta;
                @inStartWork = StartWork;
                @inEndWork = EndWork;
                @inact = act;
                @inRemark = Remark;
                @inexparrive = exparrive;

                ///
                // @inact =1 Add
                // @inact=2 Update
                //  @inact =3 read
                ///

                //List<OrderListGetdata> Getdata = new List<OrderListGetdata>();
                // List<OrderListrackingor> GetdataTrackingOrder = new List<OrderListrackingor>();
                // List<SaleOrderList> Getdatadrive = new List<SaleOrderList>();
                List<trackingor> GetdataTrackingOrder = new List<trackingor>();
                // SaleOrderList model = null;
                //trackingor modeltrackingor = null;
                var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
                SqlConnection Connection = new SqlConnection(connectionString);
                Connection.Open();
                if (@inact == "1")
                {
                    SqlCommand cmdupdate = new SqlCommand("P_AddTrackingorder", Connection);
                    cmdupdate.Connection = Connection;
                    cmdupdate.CommandType = CommandType.StoredProcedure;

                    cmdupdate.Parameters.AddWithValue("@inPackingList", @inPack);
                    cmdupdate.Parameters.AddWithValue("@inCarID", @inCarID);
                    cmdupdate.Parameters.AddWithValue("@inLatitude", @inLat);
                    cmdupdate.Parameters.AddWithValue("@inLongitude", @inLong);
                    cmdupdate.Parameters.AddWithValue("@inExptoarrive", @inexparrive);
                    cmdupdate.Parameters.AddWithValue("@inStatus", "1");
                    //cmdupdate.Parameters.AddWithValue("@inStartWork", @inStartWork);
                    //cmdupdate.Parameters.AddWithValue("@inEndWork", @inEndWork);
                    //cmdupdate.Parameters.Add(returnValue);
                    cmdupdate.ExecuteReader();
                    cmdupdate.Dispose();
                }
                else if (@inact == "2")
                {
                    SqlCommand cmdupdate = new SqlCommand("P_UpdateTrackingLatLong", Connection);
                    cmdupdate.Connection = Connection;
                    cmdupdate.CommandType = CommandType.StoredProcedure;

                    // cmdupdate.Parameters.AddWithValue("@inPackingList", @inPack);
                    cmdupdate.Parameters.AddWithValue("@inCarID", @inCarID);
                    cmdupdate.Parameters.AddWithValue("@inLatitude", @inLat);
                    cmdupdate.Parameters.AddWithValue("@inLongitude", @inLong);
                    //cmdupdate.Parameters.AddWithValue("@inStatus", @inSta);
                    //cmdupdate.Parameters.AddWithValue("@inStartWork", @inStartWork);
                    //cmdupdate.Parameters.AddWithValue("@inEndWork", @inEndWork);
                    //cmdupdate.Parameters.Add(returnValue);
                    cmdupdate.ExecuteReader();
                    cmdupdate.Dispose();
                }
                else if (@inact == "3")
                {
                    SqlCommand cmdupdate = new SqlCommand("P_UpdateTrackingOrder", Connection);
                    cmdupdate.Connection = Connection;
                    cmdupdate.CommandType = CommandType.StoredProcedure;

                    cmdupdate.Parameters.AddWithValue("@inPackingList", @inPack);
                    cmdupdate.Parameters.AddWithValue("@inCarID", @inCarID);
                    cmdupdate.Parameters.AddWithValue("@inLatitude", @inLat);
                    cmdupdate.Parameters.AddWithValue("@inLongitude", @inLong);
                    cmdupdate.Parameters.AddWithValue("@inStatus", @inSta);
                    //cmdupdate.Parameters.AddWithValue("@inStartWork", @inStartWork);
                    //cmdupdate.Parameters.AddWithValue("@inEndWork", @inEndWork);
                    //cmdupdate.Parameters.Add(returnValue);
                    cmdupdate.ExecuteReader();
                    cmdupdate.Dispose();
                }
                else if (@inact == "4")
                {
                    SqlCommand cmdupdate = new SqlCommand("P_UpdateTrackingOrderEnd", Connection);
                    cmdupdate.Connection = Connection;
                    cmdupdate.CommandType = CommandType.StoredProcedure;

                    cmdupdate.Parameters.AddWithValue("@inPackingList", @inPack);
                    cmdupdate.Parameters.AddWithValue("@inCarID", @inCarID);
                    cmdupdate.Parameters.AddWithValue("@inLatitude", @inLat);
                    cmdupdate.Parameters.AddWithValue("@inLongitude", @inLong);
                    cmdupdate.Parameters.AddWithValue("@inStatus", @inSta);
                    //cmdupdate.Parameters.AddWithValue("@inStartWork", @inStartWork);
                    //cmdupdate.Parameters.AddWithValue("@inEndWork", @inEndWork);
                    //cmdupdate.Parameters.Add(returnValue);
                    cmdupdate.ExecuteReader();

                    cmdupdate.Dispose();
                }
                else if (@inact == "5")
                {
                    SqlCommand cmdupdate = new SqlCommand("P_UpdateTrackingOrderCancel", Connection);
                    cmdupdate.Connection = Connection;
                    cmdupdate.CommandType = CommandType.StoredProcedure;

                    cmdupdate.Parameters.AddWithValue("@inPackingList", @inPack);
                    cmdupdate.Parameters.AddWithValue("@inCarID", @inCarID);
                    cmdupdate.Parameters.AddWithValue("@inLatitude", @inLat);
                    cmdupdate.Parameters.AddWithValue("@inLongitude", @inLong);
                    cmdupdate.Parameters.AddWithValue("@inRemark", @inRemark);
                    cmdupdate.Parameters.AddWithValue("@inStatus", @inSta);
                    SqlParameter returnValue = new SqlParameter("@outResult", SqlDbType.NVarChar, 100);
                    returnValue.Direction = System.Data.ParameterDirection.Output;
                    cmdupdate.Parameters.Add(returnValue);
                    cmdupdate.ExecuteNonQuery();
                    messagereturn = returnValue.Value.ToString();

                    GetdataTrackingOrder.Add(new trackingor()
                    {
                        Status = messagereturn


                    });

                    cmdupdate.Dispose();
                    Connection.Close();
                }
                else if (@inact == "6")
                {
                    string Name = string.Empty;
                    var command = new SqlCommand("P_Get_Driver", Connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@inCarID", @inCarID);


                    SqlDataReader dr = command.ExecuteReader();

                    while (dr.Read())
                    {
                        //Getdatadrive.Add(new SaleOrderList()
                        //{
                        //    Customer = dr["Name"].ToString(),
                        //    CarID = dr["Car-ID"].ToString(),
                        //    StrStatus = dr["StrStatus"].ToString(),
                        //    Status = dr["Status"].ToString(),

                        //});
                        //model = new SaleOrderList();
                        //model.Customer = dr["Name"].ToString();
                        //model.DType = dr["DType"].ToString();
                        //model.RowNo = dr["RowNo"].ToString();
                        //model.ORD_ID = dr["ORD_ID"].ToString();
                        //model.ORD_DocNo = dr["ORD_DocNo"].ToString();
                        //model.ORD_Date = dr["ORD_Date"].ToString();
                        //model.ORD_TotalAmt = dr["ORD_TotalAmt"].ToString();
                        //model.QTY = dr["ORD_TotalQty"].ToString();
                        //model.Picking = dr["Picking"].ToString();
                        //model.Picking_Date = dr["Picking_Date"].ToString();
                        //model.Invoice = dr["Invoice"].ToString();
                        //model.Invoice_Date = dr["Invoice_Date"].ToString();
                        //model.InsertDate = dr["InsertDate"].ToString();
                        //model.Customer = dr["Customer"].ToString();
                        //model.SlmCod = dr["SlmCod"].ToString();
                        //model.Prn_ORD = dr["Prn_ORD"].ToString();
                        //model.Order_Status = dr["Order_Status"].ToString();
                        //model.PrintID = dr["PrintID"].ToString();
                        //model.StatusID = dr["StatusID"].ToString();
                        //Getdata.Add(new OrderListGetdata { val = model });
                    }

                    dr.Close();
                    dr.Dispose();
                    command.Dispose();
                }
                else if (@inact == "7")
                {
                    string Name = string.Empty;
                    var command = new SqlCommand("P_Get_TrackingOrder", Connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@inCarID", @inCarID);
                    command.Parameters.AddWithValue("@inStatus", @inSta);

                    SqlDataReader dr = command.ExecuteReader();

                    while (dr.Read())
                    {

                        GetdataTrackingOrder.Add(new trackingor()
                        {
                            PackingList = dr["Packing List"].ToString(),
                            CarID = dr["Car-ID"].ToString(),
                            StrStatus = dr["StrStatus"].ToString(),
                            Status = dr["Status"].ToString(),
                            NoOfBox = dr["NoOfBox"].ToString(),
                            Exptoarrive = dr["Exptoarrive"].ToString(),
                            customer = dr["customer"].ToString(),
                            customer_name = dr["customer_name"].ToString(),
                            Address = dr["Address"].ToString(),
                            Contact = dr["Contact"].ToString(),
                            Contact_Phone = dr["Contact_Phone"].ToString(),
                            StartWork = dr["StartWork"].ToString(),
                            EndWork = dr["EndWork"].ToString(),
                            order_no = dr["order_no"].ToString(),
                            invoice_no = dr["invoice_no"].ToString(),
                        });
                        //modeltrackingor = new trackingor();
                        //modeltrackingor.PackingList = dr["Packing List"].ToString();
                        //modeltrackingor.CarID = dr["Car-ID"].ToString();
                        //modeltrackingor.StrStatus = dr["StrStatus"].ToString();
                        //modeltrackingor.Status = dr["Status"].ToString();
                        //model.RowNo = dr["RowNo"].ToString();
                        //model.ORD_ID = dr["ORD_ID"].ToString();
                        //model.ORD_DocNo = dr["ORD_DocNo"].ToString();
                        //model.ORD_Date = dr["ORD_Date"].ToString();
                        //model.ORD_TotalAmt = dr["ORD_TotalAmt"].ToString();
                        //model.QTY = dr["ORD_TotalQty"].ToString();
                        //model.Picking = dr["Picking"].ToString();
                        //model.Picking_Date = dr["Picking_Date"].ToString();
                        //model.Invoice = dr["Invoice"].ToString();
                        //model.Invoice_Date = dr["Invoice_Date"].ToString();
                        //model.InsertDate = dr["InsertDate"].ToString();
                        //model.Customer = dr["Customer"].ToString();
                        //model.SlmCod = dr["SlmCod"].ToString();
                        //model.Prn_ORD = dr["Prn_ORD"].ToString();
                        //model.Order_Status = dr["Order_Status"].ToString();
                        //model.PrintID = dr["PrintID"].ToString();
                        //model.StatusID = dr["StatusID"].ToString();
                        // GetdataTrackingOrder.Add(new OrderListrackingor { val = modeltrackingor });                                         
                    }
                    dr.Close();
                    dr.Dispose();
                    command.Dispose();
                }
                Connection.Close();
                // Doc = Docdisplay;
                // Docsub = Docdisplay;
                //v Getdata
                //ViewBag.Getdata = Getdata;
                ViewBag.GetdataTrackingOrder = GetdataTrackingOrder;
                return Json(GetdataTrackingOrder, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        public ActionResult Tatrackdetail(string act, string Pack, string CarID, string Lat, string Long, string Sta, string StartWork, string EndWork, string Remark)
        {
            string @inact = string.Empty;
            string Docdisplay = string.Empty;
            string CUSCOD = string.Empty;
            string @inPack = string.Empty;
            string @inCarID = string.Empty;
            string @inLat = string.Empty;
            string @inLong = string.Empty;
            string @inSta = string.Empty;
            string @inStartWork = string.Empty;
            string @inEndWork = string.Empty;
            string @inRemark = string.Empty;
            // Docdisplay = Request.QueryString["encode"];
            if (Docdisplay != null)
            {
                //string basePack = Pack;

                //byte[] data = System.Convert.FromBase64String(Docdisplay);
                @inPack = Pack;
                @inCarID = CarID;
                @inLat = Lat;
                @inLong = Long;
                @inSta = Sta;
                @inStartWork = StartWork;
                @inEndWork = EndWork;
                @inact = act;
                @inRemark = Remark;

                ///
                // @inact =1 Add
                // @inact=2 Update
                //  @inact =3 read
                ///

                List<trackingordetail> Getdatatrackdetail = new List<trackingordetail>();

                var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
                SqlConnection Connection = new SqlConnection(connectionString);
                Connection.Open();
                if (@inact == "8")
                {
                    // string Name = string.Empty;
                    var command = new SqlCommand("P_Get_TrackingOrder_Detail", Connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@inpacking_no", @inPack);

                    SqlDataReader dr = command.ExecuteReader();

                    while (dr.Read())
                    {
                        Getdatatrackdetail.Add(new trackingordetail()
                        {
                            packing_no = dr["packing_no"].ToString(),
                            sales_order = dr["sales_order"].ToString(),
                            customer = dr["customer"].ToString(),
                            customer_name = dr["customer_name"].ToString(),
                            client = dr["client"].ToString(),
                            //maker=dr["maker"].ToString(),
                            //status=dr["status"].ToString(),
                            dlvr_code = dr["dlvr_code"].ToString(),
                            Latitude = dr["Latitude"].ToString(),
                            Longitude = dr["Longitude"].ToString(),
                            Address = dr["Address"].ToString(),
                            Contact = dr["Contact"].ToString(),
                            NoOfBox = dr["NoOfBox"].ToString(),
                            Contact_Phone = dr["Contact_Phone"].ToString(),
                        });
                    }
                    dr.Close();
                    dr.Dispose();
                    command.Dispose();
                }
                else if (@inact == "9")
                {
                    var command = new SqlCommand("P_Check_TrackingNo", Connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@inpacking_no", @inPack);
                    command.Parameters.AddWithValue("@inCarID", @inCarID);

                    SqlDataReader dr = command.ExecuteReader();

                    while (dr.Read())
                    {
                        Getdatatrackdetail.Add(new trackingordetail()
                        {

                            packing_no = dr["packing_no"].ToString(),
                            sales_order = dr["sales_order"].ToString(),
                            customer = dr["customer"].ToString(),
                            customer_name = dr["customer_name"].ToString(),
                            client = dr["client"].ToString(),
                            //maker=dr["maker"].ToString(),
                            //status=dr["status"].ToString(),
                            dlvr_code = dr["dlvr_code"].ToString(),
                            Latitude = dr["Latitude"].ToString(),
                            Longitude = dr["Longitude"].ToString(),
                            Address = dr["Address"].ToString(),
                            Contact = dr["Contact"].ToString(),
                            NoOfBox = dr["NoOfBox"].ToString(),
                            Contact_Phone = dr["Contact_Phone"].ToString(),
                        });
                    }
                    dr.Close();
                    dr.Dispose();
                    command.Dispose();
                }
                Connection.Close();
                return Json(Getdatatrackdetail, JsonRequestBehavior.AllowGet);
            }
            return View();
        }


        public ActionResult Driver(string versio, string act, string Pack, string CarID, string Lat, string Long, string Sta, string StartWork, string EndWork, string Remark)
        {
            string @inact = string.Empty;
            string Docdisplay = string.Empty;
            string CUSCOD = string.Empty;
            string @inPack = string.Empty;
            string @inCarID = string.Empty;
            string @inLat = string.Empty;
            string @inLong = string.Empty;
            string @inSta = string.Empty;
            string @inStartWork = string.Empty;
            string @inEndWork = string.Empty;
            string @inRemark = string.Empty;
            string @inversio = string.Empty;
            string Requeststr = string.Empty;
            // Docdisplay = Request.QueryString["encode"];
            if (Docdisplay != null)
            {

                //string basePack = Pack;

                //byte[] data = System.Convert.FromBase64String(Docdisplay);
                @inPack = Pack;
                @inCarID = CarID;
                @inLat = Lat;
                @inLong = Long;
                @inSta = Sta;
                @inStartWork = StartWork;
                @inEndWork = EndWork;
                @inact = act;
                @inRemark = Remark;
                @inversio = versio;
                ////
                // @inact =1 Add
                // @inact=2 Update
                //  @inact =3 read
                ///



                List<driver> Getdatadrive = new List<driver>();


                var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
                SqlConnection Connection = new SqlConnection(connectionString);
                Connection.Open();
                if (@inact == "6")
                {
                    string Name = string.Empty;
                    var command = new SqlCommand("P_Get_Driver", Connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@inCarID", @inCarID);


                    SqlDataReader dr = command.ExecuteReader();

                    while (dr.Read())
                    {
                        Getdatadrive.Add(new driver()
                        {
                            type = dr["type"].ToString(),
                            CarID = dr["Car-ID"].ToString(),
                            DriverName = dr["Driver-Name"].ToString(),


                        });

                    }

                    dr.Close();
                    dr.Dispose();
                    command.Dispose();



                }
                else if (@inact == "0")
                {
                    string Name = string.Empty;
                    var command = new SqlCommand("P_Check_Version", Connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@inVerID", @inversio);
                    SqlParameter returnValue = new SqlParameter("@out", SqlDbType.NVarChar, 100);
                    returnValue.Direction = System.Data.ParameterDirection.Output;
                    command.Parameters.Add(returnValue);
                    command.ExecuteNonQuery();
                    Requeststr = returnValue.Value.ToString();
                    Getdatadrive.Add(new driver()
                    {
                        type = Requeststr,
                        CarID = "",
                        DriverName = "",


                    });
                    command.Dispose();
                }
                Connection.Close();
                return Json(Getdatadrive, JsonRequestBehavior.AllowGet);
            }
            return View();
        }


        public JsonResult GetReason(string flag)
        {
            if (string.IsNullOrWhiteSpace(flag))
            {
                return Json(new
                {
                    StatusId = "400",
                    Error = "Bad Request",
                    Message = "flag parameter is required."
                }, JsonRequestBehavior.AllowGet);
            }
            var Getdata = new List<object>();
            string message = string.Empty;

            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("P_Get_OrderTracking", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inflag", flag);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Getdata.Add(new
                                {
                                    Id = reader["Id"]?.ToString(),
                                    Value = reader["Value"]?.ToString()
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        StatusId = "500",
                        Error = "Internal Server Error",
                        Message = ex.Message
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(Getdata, JsonRequestBehavior.AllowGet);
        }


    }
}
