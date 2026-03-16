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
using System.Text.RegularExpressions;
using System.Globalization;
using NewShopTAM.Models;
using NewShopTAM_TAM.Models;

namespace NewShopTAM.Controllers
{
    public class PrintStatementController : Controller
    {
        //
        // GET: /Otherlink/

        Entities db = new Entities();
        [HttpGet]
        public ActionResult Index(string CODE)
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

                List<ListdataReward> GetdataReward = new List<ListdataReward>();
                List<ListCus> GetdataCus = new List<ListCus>();
                List<Listdata> GetdatatoDisplay = new List<Listdata>();
                Item data = null;
                ItemReward dataReward = null;
                Cus dataCus = null;
                string supslmcode = string.Empty;
                string strsupCus = string.Empty;
                string strsupSLM = string.Empty;
                string[] result = Regex.Split(CODE, "/");
                if (result.Length == 2)
                {
                    supslmcode = result[1].ToString();
                    var CustomerSup = db.v_SupSlm_Customer.Where(c => c.SUP == supslmcode).OrderBy(s => s.CUSCOD).ToArray();
                    for (int x = 0; x < CustomerSup.Length; x++)
                    {
                        strsupCus = CustomerSup[x].CUSCOD.ToString().Trim();
                        strsupSLM = CustomerSup[x].SLMCOD.ToString().Trim();
                        var Customer = db.v_Promo_Statement_Customer.Where(c => c.CUSCOD == strsupCus).ToArray();
                        for (int t = 0; t < Customer.Length; t++)
                        {
                            dataCus = new Cus();
                            dataCus.CUSCOD = Customer[t].CUSCOD.ToString().Trim();
                            dataCus.CUSNAM = Customer[t].CUSNAM;
                            dataCus.TELNUM = Customer[t].TELNUM;
                            dataCus.ADDR_01 = Customer[t].ADDR_01;

                            string Cus = Customer[t].CUSCOD;
                            string SLM = Customer[t].SLMCOD;
                            //if (caltyp == 2)
                            // {
                            // dataCus.CUSCOD = Customer[t].CUSCOD;
                            // dataCus.CUSNAM = Customer[t].CUSNAM;

                            dataCus.SLMCOD = Customer[t].SLMCOD;
                            dataCus.SLMNAM = Customer[t].SLMNAM;
                            DateTime dateAsof = Convert.ToDateTime(Customer[t].Asof);
                            String formatAsof = "dd/MMM/yyyy";
                            String date = dateAsof.ToString(formatAsof);
                            dataCus.Asof = date;

                            SqlCommand cmdCUSOCD = new SqlCommand("select * from v_Promo_Statement_Reward where CUSCOD =N'" + Cus + "'and SLMCOD ='" + strsupSLM + "' and PYear ='" + DateTime.Now.ToString("yy") + "'", Connection);
                            SqlDataReader reCUSOCD = cmdCUSOCD.ExecuteReader();

                            while (reCUSOCD.Read())
                            {
                                dataReward = new ItemReward();
                                // GroupName = reCUSOCD.GetValue(0).ToString();
                                dataReward.CUSCODReward = reCUSOCD.GetValue(0).ToString();
                                dataReward.Promotion_Code = reCUSOCD.GetValue(4).ToString();
                                string CPromotion = reCUSOCD.GetValue(5).ToString();
                                dataReward.CUSCOD = reCUSOCD.GetValue(1).ToString();
                                dataReward.CUSNAM = reCUSOCD.GetValue(2).ToString();
                                DateTime dateSd = Convert.ToDateTime(reCUSOCD.GetValue(12).ToString());
                                String format = "dd/MMM/yyyy";
                                String dateStr = dateSd.ToString(format);
                                dataReward.Startdate = dateStr;

                                DateTime dateEd = Convert.ToDateTime(reCUSOCD.GetValue(13).ToString());
                                String formatEd = "dd/MMM/yyyy";
                                String dateE = dateEd.ToString(formatEd);
                                dataReward.Enddate = dateE;
                                if (CPromotion != null)
                                {
                                    dataReward.Promotion_Name = CPromotion;

                                }
                                else
                                {
                                    dataReward.Promotion_Name = "-";
                                }

                                Double SAMT = Convert.ToDouble(reCUSOCD.GetValue(6).ToString());
                                if (SAMT >= 1000 || SAMT <= -1000)
                                {
                                    dataReward.SAMT = string.Format("{0:0,000.00}", reCUSOCD.GetValue(6), CultureInfo.InvariantCulture);
                                }
                                else { dataReward.SAMT = Convert.ToString(reCUSOCD.GetValue(6).ToString()); }
                                Double SPaidAMT = Convert.ToDouble(reCUSOCD.GetValue(7).ToString());
                                if (SPaidAMT >= 1000 || SPaidAMT <= -1000)
                                {
                                    dataReward.SPaidAMT = string.Format("{0:0,000.00}", reCUSOCD.GetValue(7), CultureInfo.InvariantCulture);
                                }
                                else { dataReward.SPaidAMT = Convert.ToString(reCUSOCD.GetValue(7)); }


                                //int summy = s2 - 21;
                                Double s1 = Convert.ToDouble(reCUSOCD.GetValue(6));
                                Double s2 = Convert.ToDouble(reCUSOCD.GetValue(8));
                                Double summy = s2 - s1;
                                Double StrSummy = Convert.ToDouble(summy);
                                if (summy > 0)
                                {
                                    if (StrSummy >= 1000 || StrSummy <= -1000)
                                    {
                                        dataReward.Sum = string.Format("{0:0,000.00}", summy, CultureInfo.InvariantCulture);
                                    }
                                    else { dataReward.Sum = Convert.ToString(summy); }
                                }
                                else
                                {

                                    dataReward.Sum = "0";
                                }
                                Double Condition = Convert.ToDouble(reCUSOCD.GetValue(8));
                                if (Condition >= 1000 || Condition <= -1000)
                                {
                                    dataReward.Condition = string.Format("{0:0,000.00}", reCUSOCD.GetValue(8), CultureInfo.InvariantCulture);
                                }
                                else { dataReward.Condition = Convert.ToString(reCUSOCD.GetValue(8)); }
                                dataReward.Reward = reCUSOCD.GetValue(9).ToString();
                                GetdataReward.Add(new ListdataReward { val = dataReward });
                                //GroupCus.Add(new SelectListItem() { Value = reCUSOCD.GetValue(1).ToString(), Text = reCUSOCD.GetValue(1).ToString() });
                            }
                            reCUSOCD.Dispose();

                            var rd = db.v_Promo_Statement_Inv.Where(c => c.CUSKEY == strsupCus).ToArray();
                            // if (rs.Length > 1)
                            // {
                            for (int i = 0; i < rd.Length; i++)
                            {

                                data = new Item();
                                data.COMP = rd[i].COMP;
                                data.CUSCOD = rd[i].CUSKEY.ToString().Trim();
                                int Y = Convert.ToInt32(rd[i].StYr) - 1;
                                data.LastStYr = Convert.ToString(Y);
                                data.StYr = Convert.ToString(rd[i].StYr);


                                Double LastTotal = Convert.ToDouble(rd[i].LastTotal);
                                if (LastTotal >= 1000 || LastTotal <= -1000)
                                {

                                    data.LastTotal = String.Format("{0:0,000.00}", rd[i].LastTotal, CultureInfo.InvariantCulture);

                                }
                                else { data.LastTotal = Convert.ToString(rd[i].LastTotal); }



                                Double A1 = Convert.ToDouble(rd[i].Jan);
                                if (A1 >= 1000 || A1 <= -1000)
                                {

                                    data.Jan = String.Format("{0:0,000.00}", rd[i].Jan, CultureInfo.InvariantCulture);

                                }
                                else { data.Jan = Convert.ToString(rd[i].Jan); }

                                Double A2 = Convert.ToDouble(rd[i].Feb);
                                if (A2 >= 1000 || A2 <= -1000)
                                {

                                    data.Feb = string.Format("{0:0,000.00}", rd[i].Feb, CultureInfo.InvariantCulture);
                                }
                                else { data.Feb = Convert.ToString(rd[i].Feb); }

                                Double A3 = Convert.ToDouble(rd[i].Mar);
                                if (A3 >= 1000 || A3 <= -1000)
                                {

                                    data.Mar = string.Format("{0:0,000.00}", rd[i].Mar, CultureInfo.InvariantCulture);
                                }
                                else { data.Mar = Convert.ToString(rd[i].Mar); }

                                Double A4 = Convert.ToDouble(rd[i].Apr);
                                if (A4 >= 1000 || A4 <= -1000)
                                {

                                    data.Apr = string.Format("{0:0,000.00}", rd[i].Apr, CultureInfo.InvariantCulture);
                                }
                                else { data.Apr = Convert.ToString(rd[i].Apr); }

                                Double A5 = Convert.ToDouble(rd[i].May);
                                if (A5 >= 1000 || A5 <= -1000)
                                {

                                    data.May = string.Format("{0:0,000.00}", rd[i].May, CultureInfo.InvariantCulture);
                                }
                                else { data.May = Convert.ToString(rd[i].May); }


                                Double A6 = Convert.ToDouble(rd[i].Jun);
                                if (A6 >= 1000 || A6 <= -1000)
                                {

                                    data.Jun = string.Format("{0:0,000.00}", rd[i].Jun, CultureInfo.InvariantCulture);
                                }
                                else { data.Jun = Convert.ToString(rd[i].Jun); }

                                Double A7 = Convert.ToDouble(rd[i].Jul);
                                if (A7 >= 1000 || A7 <= -1000)
                                {

                                    data.Jul = string.Format("{0:0,000.00}", rd[i].Jul, CultureInfo.InvariantCulture);
                                }
                                else { data.Jul = Convert.ToString(rd[i].Jul); }

                                Double A8 = Convert.ToDouble(rd[i].Aug);
                                if (A8 >= 1000 || A8 <= -1000)
                                {

                                    data.Aug = string.Format("{0:0,000.00}", rd[i].Aug, CultureInfo.InvariantCulture);
                                }
                                else { data.Aug = Convert.ToString(rd[i].Aug); }

                                Double A9 = Convert.ToDouble(rd[i].Sep);
                                if (A9 >= 1000 || A9 <= -1000)
                                {
                                    data.Sep = string.Format("{0:0,000.00}", rd[i].Sep, CultureInfo.InvariantCulture);
                                }
                                else { data.Sep = Convert.ToString(rd[i].Sep); }

                                Double A10 = Convert.ToDouble(rd[i].Oct);
                                if (A10 >= 1000 || A10 <= -1000)
                                {
                                    data.Oct = string.Format("{0:0,000.00}", rd[i].Oct, CultureInfo.InvariantCulture);
                                }
                                else { data.Oct = Convert.ToString(rd[i].Oct); }

                                Double A11 = Convert.ToDouble(rd[i].Nov);
                                if (A11 >= 1000 || A11 <= -1000)
                                {
                                    data.Nov = string.Format("{0:0,000.00}", rd[i].Nov, CultureInfo.InvariantCulture);
                                }
                                else { data.Nov = Convert.ToString(rd[i].Nov); }

                                Double A12 = Convert.ToDouble(rd[i].Dec);
                                if (A12 >= 1000 || A12 <= -1000)
                                {
                                    data.Dec = string.Format("{0:0,000.00}", rd[i].Dec, CultureInfo.InvariantCulture);
                                }
                                else { data.Dec = Convert.ToString(rd[i].Dec); }

                                Double Total = Convert.ToDouble(rd[i].Total);
                                if (Total >= 1000 || Total <= -1000)
                                {
                                    data.Total = string.Format("{0:0,000.00}", rd[i].Total, CultureInfo.InvariantCulture);
                                }
                                else { data.Total = Convert.ToString(rd[i].Total); }


                                if (rd.Length > 1)
                                {

                                    Double SumA1 = Convert.ToDouble(rd[0].Jan + rd[1].Jan, CultureInfo.InvariantCulture);
                                    if (SumA1 >= 1000 || SumA1 <= -1000)
                                    {
                                        data.SumJan = string.Format("{0:0,000.00}", (rd[i].Jan + rd[1].Jan), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumJan = Convert.ToString(rd[i].Jan + rd[1].Jan); }
                                }
                                else
                                {

                                    Double SumA1 = Convert.ToDouble(rd[i].Jan);
                                    if (SumA1 >= 1000 || SumA1 <= -1000)
                                    {
                                        data.SumJan = string.Format("{0:0,000.00}", (rd[i].Jan), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumJan = Convert.ToString(rd[i].Jan); }

                                }

                                if (rd.Length > 1)
                                {
                                    Double SumA2 = Convert.ToDouble(rd[i].Feb + rd[1].Feb);
                                    if (SumA2 >= 1000 || SumA2 <= -1000)
                                    {
                                        data.SumFeb = string.Format("{0:0,000.00}", (rd[i].Feb + rd[1].Feb), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumFeb = Convert.ToString(rd[i].Feb + rd[1].Feb); }
                                }
                                else
                                {
                                    Double SumA2 = Convert.ToDouble(rd[i].Feb);
                                    if (SumA2 >= 1000 || SumA2 <= -1000)
                                    {
                                        data.SumFeb = string.Format("{0:0,000.00}", (rd[i].Feb), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumFeb = Convert.ToString(rd[i].Feb); }
                                }

                                if (rd.Length > 1)
                                {
                                    Double SumA3 = Convert.ToDouble(rd[i].Mar + rd[1].Mar);
                                    if (SumA3 >= 1000 || SumA3 <= -1000)
                                    {

                                        data.SumMar = string.Format("{0:0,000.00}", (rd[i].Mar + rd[1].Mar), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumMar = Convert.ToString(rd[i].Mar + rd[1].Mar); }
                                }
                                else
                                {
                                    Double SumA3 = Convert.ToDouble(rd[i].Mar);
                                    if (SumA3 >= 1000 || SumA3 <= -1000)
                                    {

                                        data.SumMar = string.Format("{0:0,000.00}", (rd[i].Mar), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumMar = Convert.ToString(rd[i].Mar); }

                                }
                                if (rd.Length > 1)
                                {
                                    Double SumA4 = Convert.ToDouble(rd[i].Apr + rd[1].Apr);
                                    if (SumA4 >= 1000 || SumA4 <= -1000)
                                    {

                                        data.SumApr = string.Format("{0:0,000.00}", (rd[i].Apr + rd[1].Apr), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumApr = Convert.ToString(rd[i].Apr + rd[1].Apr); }
                                }
                                else
                                {
                                    Double SumA4 = Convert.ToDouble(rd[i].Apr);
                                    if (SumA4 >= 1000 || SumA4 <= -1000)
                                    {

                                        data.SumApr = string.Format("{0:0,000.00}", (rd[i].Apr), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumApr = Convert.ToString(rd[i].Apr); }
                                }
                                if (rd.Length > 1)
                                {
                                    Double SumA5 = Convert.ToDouble(rd[i].May + rd[1].May);
                                    if (SumA5 >= 1000 || SumA5 <= -1000)
                                    {

                                        data.SumMay = string.Format("{0:0,000.00}", (rd[i].May + rd[1].May), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumMay = Convert.ToString(rd[i].May + rd[1].May); }
                                }
                                else
                                {
                                    Double SumA5 = Convert.ToDouble(rd[i].May);
                                    if (SumA5 >= 1000 || SumA5 <= -1000)
                                    {

                                        data.SumMay = string.Format("{0:0,000.00}", (rd[i].May), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumMay = Convert.ToString(rd[i].May); }
                                }
                                if (rd.Length > 1)
                                {
                                    Double SumA6 = Convert.ToDouble(rd[i].Jun + rd[1].Jun);
                                    if (SumA6 >= 1000 || SumA6 <= -1000)
                                    {

                                        data.SumJun = string.Format("{0:0,000.00}", (rd[i].Jun + rd[1].Jun), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumJun = Convert.ToString(rd[i].Jun + rd[1].Jun); }
                                }
                                else
                                {
                                    Double SumA6 = Convert.ToDouble(rd[i].Jun);
                                    if (SumA6 >= 1000 || SumA6 <= -1000)
                                    {

                                        data.SumJun = string.Format("{0:0,000.00}", (rd[i].Jun), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumJun = Convert.ToString(rd[i].Jun); }
                                }
                                if (rd.Length > 1)
                                {
                                    Double SumA7 = Convert.ToDouble(rd[i].Jul + rd[1].Jul);
                                    if (SumA7 >= 1000 || SumA7 <= -1000)
                                    {

                                        data.SumJul = string.Format("{0:0,000.00}", (rd[i].Jul + rd[1].Jul), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumJul = Convert.ToString(rd[0].Jul + rd[1].Jul); }
                                }
                                else
                                {
                                    Double SumA7 = Convert.ToDouble(rd[i].Jul);
                                    if (SumA7 >= 1000 || SumA7 <= -1000)
                                    {

                                        data.SumJul = string.Format("{0:0,000.00}", (rd[i].Jul), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumJul = Convert.ToString(rd[0].Jul); }

                                }
                                if (rd.Length > 1)
                                {
                                    Double SumA8 = Convert.ToDouble(rd[0].Aug + rd[1].Aug);
                                    if (SumA8 >= 1000 || SumA8 <= -1000)
                                    {

                                        data.SumAug = string.Format("{0:0,000.00}", (rd[0].Aug + rd[1].Aug), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumAug = Convert.ToString(rd[0].Aug + rd[1].Aug); }
                                }
                                else
                                {

                                    Double SumA8 = Convert.ToDouble(rd[0].Aug);
                                    if (SumA8 >= 1000 || SumA8 <= -1000)
                                    {

                                        data.SumAug = string.Format("{0:0,000.00}", (rd[0].Aug), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumAug = Convert.ToString(rd[0].Aug); }
                                }
                                if (rd.Length > 1)
                                {
                                    Double SumA9 = Convert.ToDouble(rd[0].Sep + rd[1].Sep);
                                    if (SumA9 >= 1000 || SumA9 <= -1000)
                                    {

                                        data.SumSep = string.Format("{0:0,000.00}", (rd[0].Sep + rd[1].Sep), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumSep = Convert.ToString(rd[0].Sep + rd[1].Sep); }
                                }
                                else
                                {
                                    Double SumA9 = Convert.ToDouble(rd[0].Sep);
                                    if (SumA9 >= 1000 || SumA9 <= -1000)
                                    {

                                        data.SumSep = string.Format("{0:0,000.00}", (rd[0].Sep), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumSep = Convert.ToString(rd[0].Sep); }

                                }
                                if (rd.Length > 1)
                                {
                                    Double SumA10 = Convert.ToDouble(rd[0].Oct + rd[1].Oct);
                                    if (SumA10 >= 1000 || SumA10 <= -1000)
                                    {

                                        data.SumOct = string.Format("{0:0,000.00}", (rd[0].Oct + rd[1].Oct), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumOct = Convert.ToString(rd[0].Oct + rd[1].Oct); }
                                }
                                else
                                {
                                    Double SumA10 = Convert.ToDouble(rd[0].Oct);
                                    if (SumA10 >= 1000 || SumA10 <= -1000)
                                    {

                                        data.SumOct = string.Format("{0:0,000.00}", (rd[0].Oct), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumOct = Convert.ToString(rd[0].Oct); }


                                }
                                if (rd.Length > 1)
                                {
                                    Double SumA11 = Convert.ToDouble(rd[0].Nov + rd[1].Nov);
                                    if (SumA11 >= 1000 || SumA11 <= -1000)
                                    {

                                        data.SumNov = string.Format("{0:0,000.00}", (rd[0].Nov + rd[1].Nov), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumNov = Convert.ToString(rd[0].Nov + rd[1].Nov); }
                                }
                                else
                                {

                                    Double SumA11 = Convert.ToDouble(rd[0].Nov);
                                    if (SumA11 >= 1000 || SumA11 <= -1000)
                                    {

                                        data.SumNov = string.Format("{0:0,000.00}", (rd[0].Nov), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumNov = Convert.ToString(rd[0].Nov); }
                                }
                                if (rd.Length > 1)
                                {
                                    Double SumA12 = Convert.ToDouble(rd[0].Dec + rd[1].Dec);
                                    if (SumA12 >= 1000 || SumA12 <= -1000)
                                    {

                                        data.SumDec = string.Format("{0:0,000.00}", (rd[0].Dec + rd[1].Dec), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumDec = Convert.ToString(rd[0].Dec + rd[1].Dec); }
                                }
                                else
                                {
                                    Double SumA12 = Convert.ToDouble(rd[0].Dec);
                                    if (SumA12 >= 1000 || SumA12 <= -1000)
                                    {

                                        data.SumDec = string.Format("{0:0,000.00}", (rd[0].Dec), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumDec = Convert.ToString(rd[0].Dec); }
                                }
                                if (rd.Length > 1)
                                {
                                    Double SumTotal = Convert.ToDouble(rd[0].Total + rd[1].Total);
                                    if (SumTotal >= 1000 || SumTotal <= -1000)
                                    {

                                        data.SumTotal = string.Format("{0:0,000.00}", (rd[0].Total + rd[1].Total), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumTotal = Convert.ToString(rd[0].Total + rd[1].Total); }

                                }
                                else
                                {
                                    Double SumTotal = Convert.ToDouble(rd[0].Total);
                                    if (SumTotal >= 1000 || SumTotal <= -1000)
                                    {

                                        data.SumTotal = string.Format("{0:0,000.00}", (rd[0].Total), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumTotal = Convert.ToString(rd[0].Total); }

                                }
                                if (rd.Length > 1)
                                {
                                    Double SumLastTotal = Convert.ToDouble(rd[0].LastTotal + rd[1].LastTotal);
                                    if (SumLastTotal >= 1000 || SumLastTotal <= -1000)
                                    {

                                        data.SumLastTotal = string.Format("{0:0,000.00}", (rd[0].LastTotal + rd[1].LastTotal), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumLastTotal = Convert.ToString(rd[0].LastTotal + rd[1].LastTotal); }
                                }
                                else
                                {
                                    Double SumLastTotal = Convert.ToDouble(rd[0].LastTotal);
                                    if (SumLastTotal >= 1000 || SumLastTotal <= -1000)
                                    {

                                        data.SumLastTotal = string.Format("{0:0,000.00}", (rd[0].LastTotal), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumLastTotal = Convert.ToString(rd[0].LastTotal); }


                                }
                                if (rd.Length > 1)
                                {
                                    Double SumpayA1 = Convert.ToDouble(rd[0].PayJan + rd[1].PayJan);
                                    if (SumpayA1 >= 1000 || SumpayA1 <= -1000)
                                    {
                                        data.SumpayJan = string.Format("{0:0,000.00}", (rd[0].PayJan + rd[1].PayJan), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumpayJan = Convert.ToString(rd[0].PayJan + rd[1].PayJan); }
                                }
                                else
                                {

                                    Double SumpayA1 = Convert.ToDouble(rd[0].PayJan);
                                    if (SumpayA1 >= 1000 || SumpayA1 <= -1000)
                                    {
                                        data.SumpayJan = string.Format("{0:0,000.00}", (rd[0].PayJan), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumpayJan = Convert.ToString(rd[0].PayJan); }


                                }
                                if (rd.Length > 1)
                                {
                                    Double SumpayA2 = Convert.ToDouble(rd[0].PayFeb + rd[1].PayFeb);
                                    if (SumpayA2 >= 1000 || SumpayA2 <= -1000)
                                    {
                                        data.SumpayFeb = string.Format("{0:0,000.00}", (rd[0].PayFeb + rd[1].PayFeb), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumpayFeb = Convert.ToString(rd[0].PayFeb + rd[1].PayFeb); }
                                }
                                else
                                {
                                    Double SumpayA2 = Convert.ToDouble(rd[0].PayFeb);
                                    if (SumpayA2 >= 1000 || SumpayA2 <= -1000)
                                    {
                                        data.SumpayFeb = string.Format("{0:0,000.00}", (rd[0].PayFeb), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumpayFeb = Convert.ToString(rd[0].PayFeb); }

                                }
                                if (rd.Length > 1)
                                {
                                    Double SumpayA3 = Convert.ToDouble(rd[0].PayMar + rd[1].PayMar);
                                    if (SumpayA3 >= 1000 || SumpayA3 <= -1000)
                                    {
                                        data.SumpayMar = string.Format("{0:0,000.00}", (rd[0].PayMar + rd[1].PayMar), CultureInfo.InvariantCulture);
                                    }
                                    else
                                    { //data.SumpayMar = Convert.ToString(rd[0].PayMar + rd[1].PayMar); 
                                        data.SumpayMar = Convert.ToString(rd[0].PayMar + rd[1].PayMar);
                                    }
                                }
                                else
                                {
                                    Double SumpayA3 = Convert.ToDouble(rd[0].PayMar);
                                    if (SumpayA3 >= 1000 || SumpayA3 <= -1000)
                                    {
                                        data.SumpayMar = string.Format("{0:0,000.00}", (rd[0].PayMar), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumpayMar = Convert.ToString(rd[0].PayMar); }

                                }
                                if (rd.Length > 1)
                                {
                                    Double SumpayA4 = Convert.ToDouble(rd[0].PayApr + rd[1].PayApr);
                                    if (SumpayA4 >= 1000 || SumpayA4 <= -1000)
                                    {
                                        data.SumpayApr = string.Format("{0:0,000.00}", (rd[0].PayApr + rd[1].PayApr), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumpayApr = Convert.ToString(rd[0].PayApr + rd[1].PayApr); }
                                }
                                else
                                {
                                    Double SumpayA4 = Convert.ToDouble(rd[0].PayApr);
                                    if (SumpayA4 >= 1000 || SumpayA4 <= -1000)
                                    {
                                        data.SumpayApr = string.Format("{0:0,000.00}", (rd[0].PayApr), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumpayApr = Convert.ToString(rd[0].PayApr); }
                                }
                                if (rd.Length > 1)
                                {
                                    Double SumpayA5 = Convert.ToDouble(rd[0].PayMay + rd[1].PayMay);
                                    if (SumpayA5 >= 1000 || SumpayA5 <= -1000)
                                    {
                                        data.SumpayMay = string.Format("{0:0,000.00}", (rd[0].PayMay + rd[1].PayMay), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumpayMay = Convert.ToString(rd[0].PayMay + rd[1].PayMay); }
                                }
                                else
                                {
                                    Double SumpayA5 = Convert.ToDouble(rd[0].PayMay);
                                    if (SumpayA5 >= 1000 || SumpayA5 <= -1000)
                                    {
                                        data.SumpayMay = string.Format("{0:0,000.00}", (rd[0].PayMay), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumpayMay = Convert.ToString(rd[0].PayMay); }
                                }
                                if (rd.Length > 1)
                                {
                                    Double SumpayA6 = Convert.ToDouble(rd[0].PayJun + rd[1].PayJun);
                                    if (SumpayA6 >= 1000 || SumpayA6 <= -1000)
                                    {
                                        data.SumpayJun = string.Format("{0:0,000.00}", (rd[0].PayJun + rd[1].PayJun), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumpayJun = Convert.ToString(rd[0].PayJun + rd[1].PayJun); }
                                }
                                else
                                {
                                    Double SumpayA6 = Convert.ToDouble(rd[0].PayJun);
                                    if (SumpayA6 >= 1000 || SumpayA6 <= -1000)
                                    {
                                        data.SumpayJun = string.Format("{0:0,000.00}", (rd[0].PayJun), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumpayJun = Convert.ToString(rd[0].PayJun); }

                                }
                                if (rd.Length > 1)
                                {
                                    Double SumpayA7 = Convert.ToDouble(rd[0].PayJul + rd[1].PayJul);
                                    if (SumpayA7 >= 1000 || SumpayA7 <= -1000)
                                    {
                                        data.SumpayJul = string.Format("{0:0,000.00}", (rd[0].PayJul + rd[1].PayJul), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumpayJul = Convert.ToString(rd[0].PayJul + rd[1].PayJul); }
                                }
                                else
                                {
                                    Double SumpayA7 = Convert.ToDouble(rd[0].PayJul);
                                    if (SumpayA7 >= 1000 || SumpayA7 <= -1000)
                                    {
                                        data.SumpayJul = string.Format("{0:0,000.00}", (rd[0].PayJul), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumpayJul = Convert.ToString(rd[0].PayJul); }

                                }
                                if (rd.Length > 1)
                                {
                                    Double SumpayA8 = Convert.ToDouble(rd[0].PayAug + rd[1].PayAug);
                                    if (SumpayA8 >= 1000 || SumpayA8 <= -1000)
                                    {
                                        data.SumpayAug = string.Format("{0:0,000.00}", (rd[0].PayAug + rd[1].PayAug), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumpayAug = Convert.ToString(rd[0].PayAug + rd[1].PayAug); }
                                }
                                else
                                {
                                    Double SumpayA8 = Convert.ToDouble(rd[0].PayAug);
                                    if (SumpayA8 >= 1000 || SumpayA8 <= -1000)
                                    {
                                        data.SumpayAug = string.Format("{0:0,000.00}", (rd[0].PayAug), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumpayAug = Convert.ToString(rd[0].PayAug); }
                                }
                                if (rd.Length > 1)
                                {
                                    Double SumpayA9 = Convert.ToDouble(rd[0].PaySep + rd[1].PaySep);
                                    if (SumpayA9 >= 1000 || SumpayA9 <= -1000)
                                    {
                                        data.SumpaySep = string.Format("{0:0,000.00}", (rd[0].PaySep + rd[1].PaySep), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumpaySep = Convert.ToString(rd[0].PaySep + rd[1].PaySep); }
                                }
                                else
                                {
                                    Double SumpayA9 = Convert.ToDouble(rd[0].PaySep);
                                    if (SumpayA9 >= 1000 || SumpayA9 <= -1000)
                                    {
                                        data.SumpaySep = string.Format("{0:0,000.00}", (rd[0].PaySep));
                                    }
                                    else { data.SumpaySep = Convert.ToString(rd[0].PaySep); }
                                }
                                if (rd.Length > 1)
                                {
                                    Double SumpayA10 = Convert.ToDouble(rd[0].PayOct + rd[1].PayOct);
                                    if (SumpayA10 >= 1000 || SumpayA10 <= -1000)
                                    {
                                        data.SumpayOct = string.Format("{0:0,000.00}", (rd[0].PayOct + rd[1].PayOct), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumpayOct = Convert.ToString(rd[0].PayOct + rd[1].PayOct); }
                                }
                                else
                                {
                                    Double SumpayA10 = Convert.ToDouble(rd[0].PayOct);
                                    if (SumpayA10 >= 1000 || SumpayA10 <= -1000)
                                    {
                                        data.SumpayOct = string.Format("{0:0,000.00}", (rd[0].PayOct), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumpayOct = Convert.ToString(rd[0].PayOct); }

                                }
                                if (rd.Length > 1)
                                {
                                    Double SumpayA11 = Convert.ToDouble(rd[0].PayNov + rd[1].PayNov);
                                    if (SumpayA11 >= 1000 || SumpayA11 <= -1000)
                                    {
                                        data.SumpayNov = string.Format("{0:0,000.00}", (rd[0].PayNov + rd[1].PayNov), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumpayNov = Convert.ToString(rd[0].PayNov + rd[1].PayNov); }
                                }
                                else
                                {
                                    Double SumpayA11 = Convert.ToDouble(rd[0].PayNov);
                                    if (SumpayA11 >= 1000 || SumpayA11 <= -1000)
                                    {
                                        data.SumpayNov = string.Format("{0:0,000.00}", (rd[0].PayNov), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumpayNov = Convert.ToString(rd[0].PayNov); }
                                }
                                if (rd.Length > 1)
                                {
                                    Double SumpayA12 = Convert.ToDouble(rd[0].PayDec + rd[1].PayDec);
                                    if (SumpayA12 >= 1000 || SumpayA12 <= -1000)
                                    {
                                        data.SumpayDec = string.Format("{0:0,000.00}", (rd[0].PayDec + rd[1].PayDec), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumpayDec = Convert.ToString(rd[0].PayDec + rd[1].PayDec); }
                                }
                                else
                                {
                                    Double SumpayA12 = Convert.ToDouble(rd[0].PayDec);
                                    if (SumpayA12 >= 1000 || SumpayA12 <= -1000)
                                    {

                                        data.SumpayDec = string.Format("{0:0,000.00}", (rd[0].PayDec), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumpayDec = Convert.ToString(rd[0].PayDec); }


                                }
                                if (rd.Length > 1)
                                {
                                    Double SumpayTotal = Convert.ToDouble(rd[0].PayTotal + rd[1].PayTotal);
                                    if (SumpayTotal >= 1000 || SumpayTotal <= -1000)
                                    {
                                        data.SumpayTotal = string.Format("{0:0,000.00}", (rd[0].PayTotal + rd[1].PayTotal), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumpayTotal = Convert.ToString(rd[0].PayTotal + rd[1].PayTotal); }
                                }
                                else
                                {

                                    Double SumpayTotal = Convert.ToDouble(rd[0].PayTotal);
                                    if (SumpayTotal >= 1000 || SumpayTotal <= -1000)
                                    {
                                        data.SumpayTotal = string.Format("{0:0,000.00}", (rd[0].PayTotal), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumpayTotal = Convert.ToString(rd[0].PayTotal); }

                                }
                                if (rd.Length > 1)
                                {
                                    Double SumLastpayTotal = Convert.ToDouble(rd[0].LastPayTotal + rd[1].LastPayTotal);
                                    if (SumLastpayTotal >= 1000 || SumLastpayTotal <= -1000)
                                    {
                                        data.SumLastpayTotal = string.Format("{0:0,000.00}", (rd[0].LastPayTotal + rd[1].LastPayTotal), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumLastpayTotal = Convert.ToString(rd[0].LastPayTotal + rd[1].LastPayTotal); }
                                }
                                else
                                {
                                    Double SumLastpayTotal = Convert.ToDouble(rd[0].LastPayTotal);
                                    if (SumLastpayTotal >= 1000 || SumLastpayTotal <= -1000)
                                    {
                                        data.SumLastpayTotal = string.Format("{0:0,000.00}", (rd[0].LastPayTotal), CultureInfo.InvariantCulture);
                                    }
                                    else { data.SumLastpayTotal = Convert.ToString(rd[0].LastPayTotal); }

                                }

                                GetdatatoDisplay.Add(new Listdata { val = data });

                            }
                            GetdataCus.Add(new ListCus { val = dataCus });
                        }//For//


                    }

                }
                else
                {
                    int caltyp = 0;
                    string strcalstr = string.Empty;
                    caltyp = Convert.ToInt32(result[0]);
                    if (caltyp == 0) { strcalstr = "ALL"; } else { strcalstr = Convert.ToString(caltyp); }

                    string cuscode = result[1].ToString();
                    string slmcode = string.Empty;

                    slmcode = result[2].ToString();
                    //supslmcode = result[3].ToString();
                    if (slmcode == "ALLS") { slmcode = "ALL"; } else { slmcode = slmcode; }
                    string timeyear = DateTime.Now.ToString("yy");
                    List<GenItem> _ItemList = new JavaScriptSerializer().Deserialize<List<GenItem>>(cuscode);
                    string chcustomer = _ItemList[0].ItemID;
                    if (chcustomer != "ALL")
                    {
                        string srt = string.Empty;

                        for (int r = 0; r < _ItemList.Count; r++)
                        {

                            srt = _ItemList[r].ItemID;
                            if (srt != "CUSCOD")
                            {
                                var tea = db.v_Promo_Statement_Reward.Where(n => n.CUSCOD == srt && n.PYear == timeyear).ToArray();
                                // var tea = db.v_Promo_Statement_Reward.Where(n => n.CalTyp == caltyp && n.CUSCOD == srt && n.PYear == timeyear).ToArray();
                                if (tea.Length > 0)
                                {
                                    var Customer = db.v_Promo_Statement_Customer.Where(c => c.CUSCOD == srt).ToArray();
                                    for (int t = 0; t < Customer.Length; t++)
                                    {
                                        dataCus = new Cus();
                                        dataCus.CUSCOD = Customer[t].CUSCOD.ToString().Trim();
                                        dataCus.CUSNAM = Customer[t].CUSNAM;
                                        dataCus.TELNUM = Customer[t].TELNUM;
                                        dataCus.ADDR_01 = Customer[t].ADDR_01;
                                        dataCus.SLMCOD = Customer[t].SLMCOD;
                                        dataCus.SLMNAM = Customer[t].SLMNAM;
                                        DateTime dateAsof = Convert.ToDateTime(Customer[t].Asof);
                                        String formatAsof = "dd/MMM/yyyy";
                                        String date = dateAsof.ToString(formatAsof);
                                        dataCus.Asof = date;
                                        string Cus = Customer[t].CUSCOD;
                                        GetdataCus.Add(new ListCus { val = dataCus });
                                    }

                                    if (slmcode != "ALL")
                                    {

                                        //SqlCommand cmdCUSOCD = new SqlCommand("select * from v_Promo_Statement_Reward where CUSCOD =N'" + srt + "'and CalTyp ='" + caltyp + "'and SLMCOD ='" + slmcode + "' and PYear ='" + DateTime.Now.ToString("yy") + "'", Connection);
                                        SqlCommand cmdCUSOCD = new SqlCommand("select * from v_Promo_Statement_Reward where CUSCOD =N'" + srt + "'and SLMCOD ='" + slmcode + "' and PYear ='" + DateTime.Now.ToString("yy") + "'", Connection);
                                        SqlDataReader reCUSOCD = cmdCUSOCD.ExecuteReader();
                                        while (reCUSOCD.Read())
                                        {
                                            dataReward = new ItemReward();
                                            // GroupName = reCUSOCD.GetValue(0).ToString();
                                            dataReward.CUSCODReward = reCUSOCD.GetValue(0).ToString();
                                            dataReward.Promotion_Code = reCUSOCD.GetValue(4).ToString();
                                            string CPromotion = reCUSOCD.GetValue(5).ToString();
                                            dataReward.CUSCOD = reCUSOCD.GetValue(1).ToString();
                                            dataReward.CUSNAM = reCUSOCD.GetValue(2).ToString();
                                            DateTime dateSd = Convert.ToDateTime(reCUSOCD.GetValue(12).ToString());
                                            String format = "dd/MMM/yyyy";
                                            String dateStr = dateSd.ToString(format);
                                            dataReward.Startdate = dateStr;

                                            DateTime dateEd = Convert.ToDateTime(reCUSOCD.GetValue(13).ToString());
                                            String formatEd = "dd/MMM/yyyy";
                                            String dateE = dateEd.ToString(formatEd);
                                            dataReward.Enddate = dateE;
                                            if (CPromotion != null)
                                            {
                                                dataReward.Promotion_Name = CPromotion;

                                            }
                                            else
                                            {
                                                dataReward.Promotion_Name = "-";
                                            }

                                            Double SAMT = Convert.ToDouble(reCUSOCD.GetValue(6).ToString());
                                            if (SAMT >= 1000 || SAMT <= -1000)
                                            {
                                                dataReward.SAMT = string.Format("{0:0,000.00}", reCUSOCD.GetValue(6), CultureInfo.InvariantCulture);
                                            }
                                            else { dataReward.SAMT = Convert.ToString(reCUSOCD.GetValue(6).ToString()); }
                                            Double SPaidAMT = Convert.ToDouble(reCUSOCD.GetValue(7).ToString());
                                            if (SPaidAMT >= 1000 || SPaidAMT <= -1000)
                                            {
                                                dataReward.SPaidAMT = string.Format("{0:0,000.00}", reCUSOCD.GetValue(7), CultureInfo.InvariantCulture);
                                            }
                                            else { dataReward.SPaidAMT = Convert.ToString(reCUSOCD.GetValue(7)); }


                                            //int summy = s2 - 21;
                                            Double s1 = Convert.ToDouble(reCUSOCD.GetValue(6));
                                            Double s2 = Convert.ToDouble(reCUSOCD.GetValue(8));
                                            Double summy = s2 - s1;
                                            Double StrSummy = Convert.ToDouble(summy);
                                            if (summy > 0)
                                            {
                                                if (StrSummy >= 1000 || StrSummy <= -1000)
                                                {
                                                    dataReward.Sum = string.Format("{0:0,000}", summy, CultureInfo.InvariantCulture);
                                                }
                                                else { //dataReward.Sum = Convert.ToString(summy); 

                                                    dataReward.Sum = String.Format("{0:0}", summy);
                                                }
                                            }
                                            else
                                            {

                                                dataReward.Sum = "0";
                                            }
                                            Double Condition = Convert.ToDouble(reCUSOCD.GetValue(8));
                                            if (Condition >= 1000 || Condition <= -1000)
                                            {
                                                dataReward.Condition = string.Format("{0:0,000.00}", reCUSOCD.GetValue(8), CultureInfo.InvariantCulture);
                                            }
                                            else { dataReward.Condition = Convert.ToString(reCUSOCD.GetValue(8)); }
                                            dataReward.Reward = reCUSOCD.GetValue(9).ToString();
                                            GetdataReward.Add(new ListdataReward { val = dataReward });
                                            //GroupCus.Add(new SelectListItem() { Value = reCUSOCD.GetValue(1).ToString(), Text = reCUSOCD.GetValue(1).ToString() });
                                        }
                                        var rd = db.v_Promo_Statement_Inv.Where(c => c.CUSKEY == srt).ToArray();
                                        // if (rs.Length > 1)
                                        // {
                                        for (int i = 0; i < rd.Length; i++)
                                        {

                                            data = new Item();
                                            data.COMP = rd[i].COMP;
                                            data.CUSCOD = rd[i].CUSKEY.ToString().Trim();
                                            int Y = Convert.ToInt32(rd[i].StYr) - 1;
                                            data.LastStYr = Convert.ToString(Y);
                                            data.StYr = Convert.ToString(rd[i].StYr);


                                            Double LastTotal = Convert.ToDouble(rd[i].LastTotal);
                                            if (LastTotal >= 1000 || LastTotal <= -1000)
                                            {

                                                data.LastTotal = String.Format("{0:0,000.00}", rd[i].LastTotal, CultureInfo.InvariantCulture);

                                            }
                                            else { data.LastTotal = Convert.ToString(rd[i].LastTotal); }



                                            Double A1 = Convert.ToDouble(rd[i].Jan);
                                            if (A1 >= 1000 || A1 <= -1000)
                                            {

                                                data.Jan = String.Format("{0:0,000.00}", rd[i].Jan, CultureInfo.InvariantCulture);

                                            }
                                            else { data.Jan = Convert.ToString(rd[i].Jan); }

                                            Double A2 = Convert.ToDouble(rd[i].Feb);
                                            if (A2 >= 1000 || A2 <= -1000)
                                            {

                                                data.Feb = string.Format("{0:0,000.00}", rd[i].Feb, CultureInfo.InvariantCulture);
                                            }
                                            else { data.Feb = Convert.ToString(rd[i].Feb); }

                                            Double A3 = Convert.ToDouble(rd[i].Mar);
                                            if (A3 >= 1000 || A3 <= -1000)
                                            {

                                                data.Mar = string.Format("{0:0,000.00}", rd[i].Mar, CultureInfo.InvariantCulture);
                                            }
                                            else { data.Mar = Convert.ToString(rd[i].Mar); }

                                            Double A4 = Convert.ToDouble(rd[i].Apr);
                                            if (A4 >= 1000 || A4 <= -1000)
                                            {

                                                data.Apr = string.Format("{0:0,000.00}", rd[i].Apr, CultureInfo.InvariantCulture);
                                            }
                                            else { data.Apr = Convert.ToString(rd[i].Apr); }

                                            Double A5 = Convert.ToDouble(rd[i].May);
                                            if (A5 >= 1000 || A5 <= -1000)
                                            {

                                                data.May = string.Format("{0:0,000.00}", rd[i].May, CultureInfo.InvariantCulture);
                                            }
                                            else { data.May = Convert.ToString(rd[i].May); }


                                            Double A6 = Convert.ToDouble(rd[i].Jun);
                                            if (A6 >= 1000 || A6 <= -1000)
                                            {

                                                data.Jun = string.Format("{0:0,000.00}", rd[i].Jun, CultureInfo.InvariantCulture);
                                            }
                                            else { data.Jun = Convert.ToString(rd[i].Jun); }

                                            Double A7 = Convert.ToDouble(rd[i].Jul);
                                            if (A7 >= 1000 || A7 <= -1000)
                                            {

                                                data.Jul = string.Format("{0:0,000.00}", rd[i].Jul, CultureInfo.InvariantCulture);
                                            }
                                            else { data.Jul = Convert.ToString(rd[i].Jul); }

                                            Double A8 = Convert.ToDouble(rd[i].Aug);
                                            if (A8 >= 1000 || A8 <= -1000)
                                            {

                                                data.Aug = string.Format("{0:0,000.00}", rd[i].Aug, CultureInfo.InvariantCulture);
                                            }
                                            else { data.Aug = Convert.ToString(rd[i].Aug); }

                                            Double A9 = Convert.ToDouble(rd[i].Sep);
                                            if (A9 >= 1000 || A9 <= -1000)
                                            {
                                                data.Sep = string.Format("{0:0,000.00}", rd[i].Sep, CultureInfo.InvariantCulture);
                                            }
                                            else { data.Sep = Convert.ToString(rd[i].Sep); }

                                            Double A10 = Convert.ToDouble(rd[i].Oct);
                                            if (A10 >= 1000 || A10 <= -1000)
                                            {
                                                data.Oct = string.Format("{0:0,000.00}", rd[i].Oct, CultureInfo.InvariantCulture);
                                            }
                                            else { data.Oct = Convert.ToString(rd[i].Oct); }

                                            Double A11 = Convert.ToDouble(rd[i].Nov);
                                            if (A11 >= 1000 || A11 <= -1000)
                                            {
                                                data.Nov = string.Format("{0:0,000.00}", rd[i].Nov, CultureInfo.InvariantCulture);
                                            }
                                            else { data.Nov = Convert.ToString(rd[i].Nov); }

                                            Double A12 = Convert.ToDouble(rd[i].Dec);
                                            if (A12 >= 1000 || A12 <= -1000)
                                            {
                                                data.Dec = string.Format("{0:0,000.00}", rd[i].Dec, CultureInfo.InvariantCulture);
                                            }
                                            else { data.Dec = Convert.ToString(rd[i].Dec); }

                                            Double Total = Convert.ToDouble(rd[i].Total);
                                            if (Total >= 1000 || Total <= -1000)
                                            {
                                                data.Total = string.Format("{0:0,000.00}", rd[i].Total, CultureInfo.InvariantCulture);
                                            }
                                            else { data.Total = Convert.ToString(rd[i].Total); }


                                            if (rd.Length > 1)
                                            {

                                                Double SumA1 = Convert.ToDouble(rd[0].Jan + rd[1].Jan, CultureInfo.InvariantCulture);
                                                if (SumA1 >= 1000 || SumA1 <= -1000)
                                                {
                                                    data.SumJan = string.Format("{0:0,000.00}", (rd[i].Jan + rd[1].Jan), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumJan = Convert.ToString(rd[i].Jan + rd[1].Jan); }
                                            }
                                            else
                                            {

                                                Double SumA1 = Convert.ToDouble(rd[i].Jan);
                                                if (SumA1 >= 1000 || SumA1 <= -1000)
                                                {
                                                    data.SumJan = string.Format("{0:0,000.00}", (rd[i].Jan), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumJan = Convert.ToString(rd[i].Jan); }

                                            }

                                            if (rd.Length > 1)
                                            {
                                                Double SumA2 = Convert.ToDouble(rd[i].Feb + rd[1].Feb);
                                                if (SumA2 >= 1000 || SumA2 <= -1000)
                                                {
                                                    data.SumFeb = string.Format("{0:0,000.00}", (rd[i].Feb + rd[1].Feb), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumFeb = Convert.ToString(rd[i].Feb + rd[1].Feb); }
                                            }
                                            else
                                            {
                                                Double SumA2 = Convert.ToDouble(rd[i].Feb);
                                                if (SumA2 >= 1000 || SumA2 <= -1000)
                                                {
                                                    data.SumFeb = string.Format("{0:0,000.00}", (rd[i].Feb), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumFeb = Convert.ToString(rd[i].Feb); }
                                            }

                                            if (rd.Length > 1)
                                            {
                                                Double SumA3 = Convert.ToDouble(rd[i].Mar + rd[1].Mar);
                                                if (SumA3 >= 1000 || SumA3 <= -1000)
                                                {

                                                    data.SumMar = string.Format("{0:0,000.00}", (rd[i].Mar + rd[1].Mar), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumMar = Convert.ToString(rd[i].Mar + rd[1].Mar); }
                                            }
                                            else
                                            {
                                                Double SumA3 = Convert.ToDouble(rd[i].Mar);
                                                if (SumA3 >= 1000 || SumA3 <= -1000)
                                                {

                                                    data.SumMar = string.Format("{0:0,000.00}", (rd[i].Mar), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumMar = Convert.ToString(rd[i].Mar); }

                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumA4 = Convert.ToDouble(rd[i].Apr + rd[1].Apr);
                                                if (SumA4 >= 1000 || SumA4 <= -1000)
                                                {

                                                    data.SumApr = string.Format("{0:0,000.00}", (rd[i].Apr + rd[1].Apr), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumApr = Convert.ToString(rd[i].Apr + rd[1].Apr); }
                                            }
                                            else
                                            {
                                                Double SumA4 = Convert.ToDouble(rd[i].Apr);
                                                if (SumA4 >= 1000 || SumA4 <= -1000)
                                                {

                                                    data.SumApr = string.Format("{0:0,000.00}", (rd[i].Apr), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumApr = Convert.ToString(rd[i].Apr); }
                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumA5 = Convert.ToDouble(rd[i].May + rd[1].May);
                                                if (SumA5 >= 1000 || SumA5 <= -1000)
                                                {

                                                    data.SumMay = string.Format("{0:0,000.00}", (rd[i].May + rd[1].May), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumMay = Convert.ToString(rd[i].May + rd[1].May); }
                                            }
                                            else
                                            {
                                                Double SumA5 = Convert.ToDouble(rd[i].May);
                                                if (SumA5 >= 1000 || SumA5 <= -1000)
                                                {

                                                    data.SumMay = string.Format("{0:0,000.00}", (rd[i].May), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumMay = Convert.ToString(rd[i].May); }
                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumA6 = Convert.ToDouble(rd[i].Jun + rd[1].Jun);
                                                if (SumA6 >= 1000 || SumA6 <= -1000)
                                                {

                                                    data.SumJun = string.Format("{0:0,000.00}", (rd[i].Jun + rd[1].Jun), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumJun = Convert.ToString(rd[i].Jun + rd[1].Jun); }
                                            }
                                            else
                                            {
                                                Double SumA6 = Convert.ToDouble(rd[i].Jun);
                                                if (SumA6 >= 1000 || SumA6 <= -1000)
                                                {

                                                    data.SumJun = string.Format("{0:0,000.00}", (rd[i].Jun), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumJun = Convert.ToString(rd[i].Jun); }
                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumA7 = Convert.ToDouble(rd[i].Jul + rd[1].Jul);
                                                if (SumA7 >= 1000 || SumA7 <= -1000)
                                                {

                                                    data.SumJul = string.Format("{0:0,000.00}", (rd[i].Jul + rd[1].Jul), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumJul = Convert.ToString(rd[0].Jul + rd[1].Jul); }
                                            }
                                            else
                                            {
                                                Double SumA7 = Convert.ToDouble(rd[i].Jul);
                                                if (SumA7 >= 1000 || SumA7 <= -1000)
                                                {

                                                    data.SumJul = string.Format("{0:0,000.00}", (rd[i].Jul), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumJul = Convert.ToString(rd[0].Jul); }

                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumA8 = Convert.ToDouble(rd[0].Aug + rd[1].Aug);
                                                if (SumA8 >= 1000 || SumA8 <= -1000)
                                                {

                                                    data.SumAug = string.Format("{0:0,000.00}", (rd[0].Aug + rd[1].Aug), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumAug = Convert.ToString(rd[0].Aug + rd[1].Aug); }
                                            }
                                            else
                                            {

                                                Double SumA8 = Convert.ToDouble(rd[0].Aug);
                                                if (SumA8 >= 1000 || SumA8 <= -1000)
                                                {

                                                    data.SumAug = string.Format("{0:0,000.00}", (rd[0].Aug), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumAug = Convert.ToString(rd[0].Aug); }
                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumA9 = Convert.ToDouble(rd[0].Sep + rd[1].Sep);
                                                if (SumA9 >= 1000 || SumA9 <= -1000)
                                                {

                                                    data.SumSep = string.Format("{0:0,000.00}", (rd[0].Sep + rd[1].Sep), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumSep = Convert.ToString(rd[0].Sep + rd[1].Sep); }
                                            }
                                            else
                                            {
                                                Double SumA9 = Convert.ToDouble(rd[0].Sep);
                                                if (SumA9 >= 1000 || SumA9 <= -1000)
                                                {

                                                    data.SumSep = string.Format("{0:0,000.00}", (rd[0].Sep), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumSep = Convert.ToString(rd[0].Sep); }

                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumA10 = Convert.ToDouble(rd[0].Oct + rd[1].Oct);
                                                if (SumA10 >= 1000 || SumA10 <= -1000)
                                                {

                                                    data.SumOct = string.Format("{0:0,000.00}", (rd[0].Oct + rd[1].Oct), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumOct = Convert.ToString(rd[0].Oct + rd[1].Oct); }
                                            }
                                            else
                                            {
                                                Double SumA10 = Convert.ToDouble(rd[0].Oct);
                                                if (SumA10 >= 1000 || SumA10 <= -1000)
                                                {

                                                    data.SumOct = string.Format("{0:0,000.00}", (rd[0].Oct), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumOct = Convert.ToString(rd[0].Oct); }


                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumA11 = Convert.ToDouble(rd[0].Nov + rd[1].Nov);
                                                if (SumA11 >= 1000 || SumA11 <= -1000)
                                                {

                                                    data.SumNov = string.Format("{0:0,000.00}", (rd[0].Nov + rd[1].Nov), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumNov = Convert.ToString(rd[0].Nov + rd[1].Nov); }
                                            }
                                            else
                                            {

                                                Double SumA11 = Convert.ToDouble(rd[0].Nov);
                                                if (SumA11 >= 1000 || SumA11 <= -1000)
                                                {

                                                    data.SumNov = string.Format("{0:0,000.00}", (rd[0].Nov), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumNov = Convert.ToString(rd[0].Nov); }
                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumA12 = Convert.ToDouble(rd[0].Dec + rd[1].Dec);
                                                if (SumA12 >= 1000 || SumA12 <= -1000)
                                                {

                                                    data.SumDec = string.Format("{0:0,000.00}", (rd[0].Dec + rd[1].Dec), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumDec = Convert.ToString(rd[0].Dec + rd[1].Dec); }
                                            }
                                            else
                                            {
                                                Double SumA12 = Convert.ToDouble(rd[0].Dec);
                                                if (SumA12 >= 1000 || SumA12 <= -1000)
                                                {

                                                    data.SumDec = string.Format("{0:0,000.00}", (rd[0].Dec), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumDec = Convert.ToString(rd[0].Dec); }
                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumTotal = Convert.ToDouble(rd[0].Total + rd[1].Total);
                                                if (SumTotal >= 1000 || SumTotal <= -1000)
                                                {

                                                    data.SumTotal = string.Format("{0:0,000.00}", (rd[0].Total + rd[1].Total), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumTotal = Convert.ToString(rd[0].Total + rd[1].Total); }

                                            }
                                            else
                                            {
                                                Double SumTotal = Convert.ToDouble(rd[0].Total);
                                                if (SumTotal >= 1000 || SumTotal <= -1000)
                                                {

                                                    data.SumTotal = string.Format("{0:0,000.00}", (rd[0].Total), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumTotal = Convert.ToString(rd[0].Total); }

                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumLastTotal = Convert.ToDouble(rd[0].LastTotal + rd[1].LastTotal);
                                                if (SumLastTotal >= 1000 || SumLastTotal <= -1000)
                                                {

                                                    data.SumLastTotal = string.Format("{0:0,000.00}", (rd[0].LastTotal + rd[1].LastTotal), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumLastTotal = Convert.ToString(rd[0].LastTotal + rd[1].LastTotal); }
                                            }
                                            else
                                            {
                                                Double SumLastTotal = Convert.ToDouble(rd[0].LastTotal);
                                                if (SumLastTotal >= 1000 || SumLastTotal <= -1000)
                                                {

                                                    data.SumLastTotal = string.Format("{0:0,000.00}", (rd[0].LastTotal), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumLastTotal = Convert.ToString(rd[0].LastTotal); }


                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumpayA1 = Convert.ToDouble(rd[0].PayJan + rd[1].PayJan);
                                                if (SumpayA1 >= 1000 || SumpayA1 <= -1000)
                                                {
                                                    data.SumpayJan = string.Format("{0:0,000.00}", (rd[0].PayJan + rd[1].PayJan), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayJan = Convert.ToString(rd[0].PayJan + rd[1].PayJan); }
                                            }
                                            else
                                            {

                                                Double SumpayA1 = Convert.ToDouble(rd[0].PayJan);
                                                if (SumpayA1 >= 1000 || SumpayA1 <= -1000)
                                                {
                                                    data.SumpayJan = string.Format("{0:0,000.00}", (rd[0].PayJan), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayJan = Convert.ToString(rd[0].PayJan); }


                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumpayA2 = Convert.ToDouble(rd[0].PayFeb + rd[1].PayFeb);
                                                if (SumpayA2 >= 1000 || SumpayA2 <= -1000)
                                                {
                                                    data.SumpayFeb = string.Format("{0:0,000.00}", (rd[0].PayFeb + rd[1].PayFeb), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayFeb = Convert.ToString(rd[0].PayFeb + rd[1].PayFeb); }
                                            }
                                            else
                                            {
                                                Double SumpayA2 = Convert.ToDouble(rd[0].PayFeb);
                                                if (SumpayA2 >= 1000 || SumpayA2 <= -1000)
                                                {
                                                    data.SumpayFeb = string.Format("{0:0,000.00}", (rd[0].PayFeb), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayFeb = Convert.ToString(rd[0].PayFeb); }

                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumpayA3 = Convert.ToDouble(rd[0].PayMar + rd[1].PayMar);
                                                if (SumpayA3 >= 1000 || SumpayA3 <= -1000)
                                                {
                                                    data.SumpayMar = string.Format("{0:0,000.00}", (rd[0].PayMar + rd[1].PayMar), CultureInfo.InvariantCulture);
                                                }
                                                else
                                                { //data.SumpayMar = Convert.ToString(rd[0].PayMar + rd[1].PayMar); 
                                                    data.SumpayMar = Convert.ToString(rd[0].PayMar + rd[1].PayMar);
                                                }
                                            }
                                            else
                                            {
                                                Double SumpayA3 = Convert.ToDouble(rd[0].PayMar);
                                                if (SumpayA3 >= 1000 || SumpayA3 <= -1000)
                                                {
                                                    data.SumpayMar = string.Format("{0:0,000.00}", (rd[0].PayMar), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayMar = Convert.ToString(rd[0].PayMar); }

                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumpayA4 = Convert.ToDouble(rd[0].PayApr + rd[1].PayApr);
                                                if (SumpayA4 >= 1000 || SumpayA4 <= -1000)
                                                {
                                                    data.SumpayApr = string.Format("{0:0,000.00}", (rd[0].PayApr + rd[1].PayApr), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayApr = Convert.ToString(rd[0].PayApr + rd[1].PayApr); }
                                            }
                                            else
                                            {
                                                Double SumpayA4 = Convert.ToDouble(rd[0].PayApr);
                                                if (SumpayA4 >= 1000 || SumpayA4 <= -1000)
                                                {
                                                    data.SumpayApr = string.Format("{0:0,000.00}", (rd[0].PayApr), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayApr = Convert.ToString(rd[0].PayApr); }
                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumpayA5 = Convert.ToDouble(rd[0].PayMay + rd[1].PayMay);
                                                if (SumpayA5 >= 1000 || SumpayA5 <= -1000)
                                                {
                                                    data.SumpayMay = string.Format("{0:0,000.00}", (rd[0].PayMay + rd[1].PayMay), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayMay = Convert.ToString(rd[0].PayMay + rd[1].PayMay); }
                                            }
                                            else
                                            {
                                                Double SumpayA5 = Convert.ToDouble(rd[0].PayMay);
                                                if (SumpayA5 >= 1000 || SumpayA5 <= -1000)
                                                {
                                                    data.SumpayMay = string.Format("{0:0,000.00}", (rd[0].PayMay), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayMay = Convert.ToString(rd[0].PayMay); }
                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumpayA6 = Convert.ToDouble(rd[0].PayJun + rd[1].PayJun);
                                                if (SumpayA6 >= 1000 || SumpayA6 <= -1000)
                                                {
                                                    data.SumpayJun = string.Format("{0:0,000.00}", (rd[0].PayJun + rd[1].PayJun), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayJun = Convert.ToString(rd[0].PayJun + rd[1].PayJun); }
                                            }
                                            else
                                            {
                                                Double SumpayA6 = Convert.ToDouble(rd[0].PayJun);
                                                if (SumpayA6 >= 1000 || SumpayA6 <= -1000)
                                                {
                                                    data.SumpayJun = string.Format("{0:0,000.00}", (rd[0].PayJun), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayJun = Convert.ToString(rd[0].PayJun); }

                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumpayA7 = Convert.ToDouble(rd[0].PayJul + rd[1].PayJul);
                                                if (SumpayA7 >= 1000 || SumpayA7 <= -1000)
                                                {
                                                    data.SumpayJul = string.Format("{0:0,000.00}", (rd[0].PayJul + rd[1].PayJul), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayJul = Convert.ToString(rd[0].PayJul + rd[1].PayJul); }
                                            }
                                            else
                                            {
                                                Double SumpayA7 = Convert.ToDouble(rd[0].PayJul);
                                                if (SumpayA7 >= 1000 || SumpayA7 <= -1000)
                                                {
                                                    data.SumpayJul = string.Format("{0:0,000.00}", (rd[0].PayJul), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayJul = Convert.ToString(rd[0].PayJul); }

                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumpayA8 = Convert.ToDouble(rd[0].PayAug + rd[1].PayAug);
                                                if (SumpayA8 >= 1000 || SumpayA8 <= -1000)
                                                {
                                                    data.SumpayAug = string.Format("{0:0,000.00}", (rd[0].PayAug + rd[1].PayAug), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayAug = Convert.ToString(rd[0].PayAug + rd[1].PayAug); }
                                            }
                                            else
                                            {
                                                Double SumpayA8 = Convert.ToDouble(rd[0].PayAug);
                                                if (SumpayA8 >= 1000 || SumpayA8 <= -1000)
                                                {
                                                    data.SumpayAug = string.Format("{0:0,000.00}", (rd[0].PayAug), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayAug = Convert.ToString(rd[0].PayAug); }
                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumpayA9 = Convert.ToDouble(rd[0].PaySep + rd[1].PaySep);
                                                if (SumpayA9 >= 1000 || SumpayA9 <= -1000)
                                                {
                                                    data.SumpaySep = string.Format("{0:0,000.00}", (rd[0].PaySep + rd[1].PaySep), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpaySep = Convert.ToString(rd[0].PaySep + rd[1].PaySep); }
                                            }
                                            else
                                            {
                                                Double SumpayA9 = Convert.ToDouble(rd[0].PaySep);
                                                if (SumpayA9 >= 1000 || SumpayA9 <= -1000)
                                                {
                                                    data.SumpaySep = string.Format("{0:0,000.00}", (rd[0].PaySep));
                                                }
                                                else { data.SumpaySep = Convert.ToString(rd[0].PaySep); }
                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumpayA10 = Convert.ToDouble(rd[0].PayOct + rd[1].PayOct);
                                                if (SumpayA10 >= 1000 || SumpayA10 <= -1000)
                                                {
                                                    data.SumpayOct = string.Format("{0:0,000.00}", (rd[0].PayOct + rd[1].PayOct), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayOct = Convert.ToString(rd[0].PayOct + rd[1].PayOct); }
                                            }
                                            else
                                            {
                                                Double SumpayA10 = Convert.ToDouble(rd[0].PayOct);
                                                if (SumpayA10 >= 1000 || SumpayA10 <= -1000)
                                                {
                                                    data.SumpayOct = string.Format("{0:0,000.00}", (rd[0].PayOct), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayOct = Convert.ToString(rd[0].PayOct); }

                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumpayA11 = Convert.ToDouble(rd[0].PayNov + rd[1].PayNov);
                                                if (SumpayA11 >= 1000 || SumpayA11 <= -1000)
                                                {
                                                    data.SumpayNov = string.Format("{0:0,000.00}", (rd[0].PayNov + rd[1].PayNov), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayNov = Convert.ToString(rd[0].PayNov + rd[1].PayNov); }
                                            }
                                            else
                                            {
                                                Double SumpayA11 = Convert.ToDouble(rd[0].PayNov);
                                                if (SumpayA11 >= 1000 || SumpayA11 <= -1000)
                                                {
                                                    data.SumpayNov = string.Format("{0:0,000.00}", (rd[0].PayNov), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayNov = Convert.ToString(rd[0].PayNov); }
                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumpayA12 = Convert.ToDouble(rd[0].PayDec + rd[1].PayDec);
                                                if (SumpayA12 >= 1000 || SumpayA12 <= -1000)
                                                {
                                                    data.SumpayDec = string.Format("{0:0,000.00}", (rd[0].PayDec + rd[1].PayDec), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayDec = Convert.ToString(rd[0].PayDec + rd[1].PayDec); }
                                            }
                                            else
                                            {
                                                Double SumpayA12 = Convert.ToDouble(rd[0].PayDec);
                                                if (SumpayA12 >= 1000 || SumpayA12 <= -1000)
                                                {

                                                    data.SumpayDec = string.Format("{0:0,000.00}", (rd[0].PayDec), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayDec = Convert.ToString(rd[0].PayDec); }


                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumpayTotal = Convert.ToDouble(rd[0].PayTotal + rd[1].PayTotal);
                                                if (SumpayTotal >= 1000 || SumpayTotal <= -1000)
                                                {
                                                    data.SumpayTotal = string.Format("{0:0,000.00}", (rd[0].PayTotal + rd[1].PayTotal), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayTotal = Convert.ToString(rd[0].PayTotal + rd[1].PayTotal); }
                                            }
                                            else
                                            {

                                                Double SumpayTotal = Convert.ToDouble(rd[0].PayTotal);
                                                if (SumpayTotal >= 1000 || SumpayTotal <= -1000)
                                                {
                                                    data.SumpayTotal = string.Format("{0:0,000.00}", (rd[0].PayTotal), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayTotal = Convert.ToString(rd[0].PayTotal); }

                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumLastpayTotal = Convert.ToDouble(rd[0].LastPayTotal + rd[1].LastPayTotal);
                                                if (SumLastpayTotal >= 1000 || SumLastpayTotal <= -1000)
                                                {
                                                    data.SumLastpayTotal = string.Format("{0:0,000.00}", (rd[0].LastPayTotal + rd[1].LastPayTotal), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumLastpayTotal = Convert.ToString(rd[0].LastPayTotal + rd[1].LastPayTotal); }
                                            }
                                            else
                                            {
                                                Double SumLastpayTotal = Convert.ToDouble(rd[0].LastPayTotal);
                                                if (SumLastpayTotal >= 1000 || SumLastpayTotal <= -1000)
                                                {
                                                    data.SumLastpayTotal = string.Format("{0:0,000.00}", (rd[0].LastPayTotal), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumLastpayTotal = Convert.ToString(rd[0].LastPayTotal); }

                                            }

                                            GetdatatoDisplay.Add(new Listdata { val = data });

                                        }
                                    }
                                    else
                                    {
                                        SqlCommand cmdCUSOCD = new SqlCommand("select * from v_Promo_Statement_Reward where CUSCOD =N'" + srt + "' and PYear ='" + DateTime.Now.ToString("yy") + "'", Connection);
                                        SqlDataReader reCUSOCD = cmdCUSOCD.ExecuteReader();

                                        while (reCUSOCD.Read())
                                        {
                                            dataReward = new ItemReward();
                                            // GroupName = reCUSOCD.GetValue(0).ToString();
                                            dataReward.CUSCODReward = reCUSOCD.GetValue(0).ToString();
                                            dataReward.Promotion_Code = reCUSOCD.GetValue(4).ToString();
                                            string CPromotion = reCUSOCD.GetValue(5).ToString();
                                            dataReward.CUSCOD = reCUSOCD.GetValue(1).ToString();
                                            dataReward.CUSNAM = reCUSOCD.GetValue(2).ToString();
                                            DateTime dateSd = Convert.ToDateTime(reCUSOCD.GetValue(12).ToString());
                                            String format = "dd/MMM/yyyy";
                                            String dateStr = dateSd.ToString(format);
                                            dataReward.Startdate = dateStr;

                                            DateTime dateEd = Convert.ToDateTime(reCUSOCD.GetValue(13).ToString());
                                            String formatEd = "dd/MMM/yyyy";
                                            String dateE = dateEd.ToString(formatEd);
                                            dataReward.Enddate = dateE;
                                            if (CPromotion != null)
                                            {
                                                dataReward.Promotion_Name = CPromotion;

                                            }
                                            else
                                            {
                                                dataReward.Promotion_Name = "-";
                                            }

                                            Double SAMT = Convert.ToDouble(reCUSOCD.GetValue(6).ToString());
                                            if (SAMT >= 1000 || SAMT <= -1000)
                                            {
                                                dataReward.SAMT = string.Format("{0:0,000.00}", reCUSOCD.GetValue(6), CultureInfo.InvariantCulture);
                                            }
                                            else { dataReward.SAMT = Convert.ToString(reCUSOCD.GetValue(6).ToString()); }
                                            Double SPaidAMT = Convert.ToDouble(reCUSOCD.GetValue(7).ToString());
                                            if (SPaidAMT >= 1000 || SPaidAMT <= -1000)
                                            {
                                                dataReward.SPaidAMT = string.Format("{0:0,000.00}", reCUSOCD.GetValue(7), CultureInfo.InvariantCulture);
                                            }
                                            else { dataReward.SPaidAMT = Convert.ToString(reCUSOCD.GetValue(7)); }


                                            //int summy = s2 - 21;
                                            Double s1 = Convert.ToDouble(reCUSOCD.GetValue(6));
                                            Double s2 = Convert.ToDouble(reCUSOCD.GetValue(8));
                                            Double summy = s2 - s1;
                                            Double StrSummy = Convert.ToDouble(summy);
                                            if (summy > 0)
                                            {
                                                if (StrSummy >= 1000 || StrSummy <= -1000)
                                                {
                                                    dataReward.Sum = string.Format("{0:0,000.00}", summy, CultureInfo.InvariantCulture);
                                                }
                                                else { dataReward.Sum = Convert.ToString(summy); }
                                            }
                                            else
                                            {

                                                dataReward.Sum = "0";
                                            }
                                            Double Condition = Convert.ToDouble(reCUSOCD.GetValue(8));
                                            if (Condition >= 1000 || Condition <= -1000)
                                            {
                                                dataReward.Condition = string.Format("{0:0,000.00}", reCUSOCD.GetValue(8), CultureInfo.InvariantCulture);
                                            }
                                            else { dataReward.Condition = Convert.ToString(reCUSOCD.GetValue(8)); }
                                            dataReward.Reward = reCUSOCD.GetValue(9).ToString();
                                            GetdataReward.Add(new ListdataReward { val = dataReward });
                                            //GroupCus.Add(new SelectListItem() { Value = reCUSOCD.GetValue(1).ToString(), Text = reCUSOCD.GetValue(1).ToString() });
                                        }
                                        var rd = db.v_Promo_Statement_Inv.Where(c => c.CUSKEY == srt).ToArray();
                                        string ckCom = string.Empty;
                                        for (int i = 0; i < rd.Length; i++)
                                        {

                                            data = new Item();
                                            data.COMP = rd[i].COMP;
                                            data.CUSCOD = rd[i].CUSKEY.ToString().Trim();
                                            int Y = Convert.ToInt32(rd[i].StYr) - 1;
                                            data.LastStYr = Convert.ToString(Y);
                                            data.StYr = Convert.ToString(rd[i].StYr);


                                            Double LastTotal = Convert.ToDouble(rd[i].LastTotal);
                                            if (LastTotal >= 1000 || LastTotal <= -1000)
                                            {

                                                data.LastTotal = String.Format("{0:0,000.00}", rd[i].LastTotal, CultureInfo.InvariantCulture);

                                            }
                                            else { data.LastTotal = Convert.ToString(rd[i].LastTotal); }



                                            Double A1 = Convert.ToDouble(rd[i].Jan);
                                            if (A1 >= 1000 || A1 <= -1000)
                                            {

                                                data.Jan = String.Format("{0:0,000.00}", rd[i].Jan, CultureInfo.InvariantCulture);

                                            }
                                            else { data.Jan = Convert.ToString(rd[i].Jan); }

                                            Double A2 = Convert.ToDouble(rd[i].Feb);
                                            if (A2 >= 1000 || A2 <= -1000)
                                            {

                                                data.Feb = string.Format("{0:0,000.00}", rd[i].Feb, CultureInfo.InvariantCulture);
                                            }
                                            else { data.Feb = Convert.ToString(rd[i].Feb); }

                                            Double A3 = Convert.ToDouble(rd[i].Mar);
                                            if (A3 >= 1000 || A3 <= -1000)
                                            {

                                                data.Mar = string.Format("{0:0,000.00}", rd[i].Mar, CultureInfo.InvariantCulture);
                                            }
                                            else { data.Mar = Convert.ToString(rd[i].Mar); }

                                            Double A4 = Convert.ToDouble(rd[i].Apr);
                                            if (A4 >= 1000 || A4 <= -1000)
                                            {

                                                data.Apr = string.Format("{0:0,000.00}", rd[i].Apr, CultureInfo.InvariantCulture);
                                            }
                                            else { data.Apr = Convert.ToString(rd[i].Apr); }

                                            Double A5 = Convert.ToDouble(rd[i].May);
                                            if (A5 >= 1000 || A5 <= -1000)
                                            {

                                                data.May = string.Format("{0:0,000.00}", rd[i].May, CultureInfo.InvariantCulture);
                                            }
                                            else { data.May = Convert.ToString(rd[i].May); }


                                            Double A6 = Convert.ToDouble(rd[i].Jun);
                                            if (A6 >= 1000 || A6 <= -1000)
                                            {

                                                data.Jun = string.Format("{0:0,000.00}", rd[i].Jun, CultureInfo.InvariantCulture);
                                            }
                                            else { data.Jun = Convert.ToString(rd[i].Jun); }

                                            Double A7 = Convert.ToDouble(rd[i].Jul);
                                            if (A7 >= 1000 || A7 <= -1000)
                                            {

                                                data.Jul = string.Format("{0:0,000.00}", rd[i].Jul, CultureInfo.InvariantCulture);
                                            }
                                            else { data.Jul = Convert.ToString(rd[i].Jul); }

                                            Double A8 = Convert.ToDouble(rd[i].Aug);
                                            if (A8 >= 1000 || A8 <= -1000)
                                            {

                                                data.Aug = string.Format("{0:0,000.00}", rd[i].Aug, CultureInfo.InvariantCulture);
                                            }
                                            else { data.Aug = Convert.ToString(rd[i].Aug); }

                                            Double A9 = Convert.ToDouble(rd[i].Sep);
                                            if (A9 >= 1000 || A9 <= -1000)
                                            {
                                                data.Sep = string.Format("{0:0,000.00}", rd[i].Sep, CultureInfo.InvariantCulture);
                                            }
                                            else { data.Sep = Convert.ToString(rd[i].Sep); }

                                            Double A10 = Convert.ToDouble(rd[i].Oct);
                                            if (A10 >= 1000 || A10 <= -1000)
                                            {
                                                data.Oct = string.Format("{0:0,000.00}", rd[i].Oct, CultureInfo.InvariantCulture);
                                            }
                                            else { data.Oct = Convert.ToString(rd[i].Oct); }

                                            Double A11 = Convert.ToDouble(rd[i].Nov);
                                            if (A11 >= 1000 || A11 <= -1000)
                                            {
                                                data.Nov = string.Format("{0:0,000.00}", rd[i].Nov, CultureInfo.InvariantCulture);
                                            }
                                            else { data.Nov = Convert.ToString(rd[i].Nov); }

                                            Double A12 = Convert.ToDouble(rd[i].Dec);
                                            if (A12 >= 1000 || A12 <= -1000)
                                            {
                                                data.Dec = string.Format("{0:0,000.00}", rd[i].Dec, CultureInfo.InvariantCulture);
                                            }
                                            else { data.Dec = Convert.ToString(rd[i].Dec); }

                                            Double Total = Convert.ToDouble(rd[i].Total);
                                            if (Total >= 1000 || Total <= -1000)
                                            {
                                                data.Total = string.Format("{0:0,000.00}", rd[i].Total, CultureInfo.InvariantCulture);
                                            }
                                            else { data.Total = Convert.ToString(rd[i].Total); }


                                            if (rd.Length > 1)
                                            {

                                                Double SumA1 = Convert.ToDouble(rd[0].Jan + rd[1].Jan, CultureInfo.InvariantCulture);
                                                if (SumA1 >= 1000 || SumA1 <= -1000)
                                                {
                                                    data.SumJan = string.Format("{0:0,000.00}", (rd[i].Jan + rd[1].Jan), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumJan = Convert.ToString(rd[i].Jan + rd[1].Jan); }
                                            }
                                            else
                                            {

                                                Double SumA1 = Convert.ToDouble(rd[i].Jan);
                                                if (SumA1 >= 1000 || SumA1 <= -1000)
                                                {
                                                    data.SumJan = string.Format("{0:0,000.00}", (rd[i].Jan), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumJan = Convert.ToString(rd[i].Jan); }

                                            }

                                            if (rd.Length > 1)
                                            {
                                                Double SumA2 = Convert.ToDouble(rd[i].Feb + rd[1].Feb);
                                                if (SumA2 >= 1000 || SumA2 <= -1000)
                                                {
                                                    data.SumFeb = string.Format("{0:0,000.00}", (rd[i].Feb + rd[1].Feb), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumFeb = Convert.ToString(rd[i].Feb + rd[1].Feb); }
                                            }
                                            else
                                            {
                                                Double SumA2 = Convert.ToDouble(rd[i].Feb);
                                                if (SumA2 >= 1000 || SumA2 <= -1000)
                                                {
                                                    data.SumFeb = string.Format("{0:0,000.00}", (rd[i].Feb), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumFeb = Convert.ToString(rd[i].Feb); }
                                            }

                                            if (rd.Length > 1)
                                            {
                                                Double SumA3 = Convert.ToDouble(rd[i].Mar + rd[1].Mar);
                                                if (SumA3 >= 1000 || SumA3 <= -1000)
                                                {

                                                    data.SumMar = string.Format("{0:0,000.00}", (rd[i].Mar + rd[1].Mar), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumMar = Convert.ToString(rd[i].Mar + rd[1].Mar); }
                                            }
                                            else
                                            {
                                                Double SumA3 = Convert.ToDouble(rd[i].Mar);
                                                if (SumA3 >= 1000 || SumA3 <= -1000)
                                                {

                                                    data.SumMar = string.Format("{0:0,000.00}", (rd[i].Mar), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumMar = Convert.ToString(rd[i].Mar); }

                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumA4 = Convert.ToDouble(rd[i].Apr + rd[1].Apr);
                                                if (SumA4 >= 1000 || SumA4 <= -1000)
                                                {

                                                    data.SumApr = string.Format("{0:0,000.00}", (rd[i].Apr + rd[1].Apr), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumApr = Convert.ToString(rd[i].Apr + rd[1].Apr); }
                                            }
                                            else
                                            {
                                                Double SumA4 = Convert.ToDouble(rd[i].Apr);
                                                if (SumA4 >= 1000 || SumA4 <= -1000)
                                                {

                                                    data.SumApr = string.Format("{0:0,000.00}", (rd[i].Apr), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumApr = Convert.ToString(rd[i].Apr); }
                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumA5 = Convert.ToDouble(rd[i].May + rd[1].May);
                                                if (SumA5 >= 1000 || SumA5 <= -1000)
                                                {

                                                    data.SumMay = string.Format("{0:0,000.00}", (rd[i].May + rd[1].May), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumMay = Convert.ToString(rd[i].May + rd[1].May); }
                                            }
                                            else
                                            {
                                                Double SumA5 = Convert.ToDouble(rd[i].May);
                                                if (SumA5 >= 1000 || SumA5 <= -1000)
                                                {

                                                    data.SumMay = string.Format("{0:0,000.00}", (rd[i].May), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumMay = Convert.ToString(rd[i].May); }
                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumA6 = Convert.ToDouble(rd[i].Jun + rd[1].Jun);
                                                if (SumA6 >= 1000 || SumA6 <= -1000)
                                                {

                                                    data.SumJun = string.Format("{0:0,000.00}", (rd[i].Jun + rd[1].Jun), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumJun = Convert.ToString(rd[i].Jun + rd[1].Jun); }
                                            }
                                            else
                                            {
                                                Double SumA6 = Convert.ToDouble(rd[i].Jun);
                                                if (SumA6 >= 1000 || SumA6 <= -1000)
                                                {

                                                    data.SumJun = string.Format("{0:0,000.00}", (rd[i].Jun), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumJun = Convert.ToString(rd[i].Jun); }
                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumA7 = Convert.ToDouble(rd[i].Jul + rd[1].Jul);
                                                if (SumA7 >= 1000 || SumA7 <= -1000)
                                                {

                                                    data.SumJul = string.Format("{0:0,000.00}", (rd[i].Jul + rd[1].Jul), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumJul = Convert.ToString(rd[0].Jul + rd[1].Jul); }
                                            }
                                            else
                                            {
                                                Double SumA7 = Convert.ToDouble(rd[i].Jul);
                                                if (SumA7 >= 1000 || SumA7 <= -1000)
                                                {

                                                    data.SumJul = string.Format("{0:0,000.00}", (rd[i].Jul), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumJul = Convert.ToString(rd[0].Jul); }

                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumA8 = Convert.ToDouble(rd[0].Aug + rd[1].Aug);
                                                if (SumA8 >= 1000 || SumA8 <= -1000)
                                                {

                                                    data.SumAug = string.Format("{0:0,000.00}", (rd[0].Aug + rd[1].Aug), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumAug = Convert.ToString(rd[0].Aug + rd[1].Aug); }
                                            }
                                            else
                                            {

                                                Double SumA8 = Convert.ToDouble(rd[0].Aug);
                                                if (SumA8 >= 1000 || SumA8 <= -1000)
                                                {

                                                    data.SumAug = string.Format("{0:0,000.00}", (rd[0].Aug), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumAug = Convert.ToString(rd[0].Aug); }
                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumA9 = Convert.ToDouble(rd[0].Sep + rd[1].Sep);
                                                if (SumA9 >= 1000 || SumA9 <= -1000)
                                                {

                                                    data.SumSep = string.Format("{0:0,000.00}", (rd[0].Sep + rd[1].Sep), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumSep = Convert.ToString(rd[0].Sep + rd[1].Sep); }
                                            }
                                            else
                                            {
                                                Double SumA9 = Convert.ToDouble(rd[0].Sep);
                                                if (SumA9 >= 1000 || SumA9 <= -1000)
                                                {

                                                    data.SumSep = string.Format("{0:0,000.00}", (rd[0].Sep), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumSep = Convert.ToString(rd[0].Sep); }

                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumA10 = Convert.ToDouble(rd[0].Oct + rd[1].Oct);
                                                if (SumA10 >= 1000 || SumA10 <= -1000)
                                                {

                                                    data.SumOct = string.Format("{0:0,000.00}", (rd[0].Oct + rd[1].Oct), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumOct = Convert.ToString(rd[0].Oct + rd[1].Oct); }
                                            }
                                            else
                                            {
                                                Double SumA10 = Convert.ToDouble(rd[0].Oct);
                                                if (SumA10 >= 1000 || SumA10 <= -1000)
                                                {

                                                    data.SumOct = string.Format("{0:0,000.00}", (rd[0].Oct), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumOct = Convert.ToString(rd[0].Oct); }


                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumA11 = Convert.ToDouble(rd[0].Nov + rd[1].Nov);
                                                if (SumA11 >= 1000 || SumA11 <= -1000)
                                                {

                                                    data.SumNov = string.Format("{0:0,000.00}", (rd[0].Nov + rd[1].Nov), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumNov = Convert.ToString(rd[0].Nov + rd[1].Nov); }
                                            }
                                            else
                                            {

                                                Double SumA11 = Convert.ToDouble(rd[0].Nov);
                                                if (SumA11 >= 1000 || SumA11 <= -1000)
                                                {

                                                    data.SumNov = string.Format("{0:0,000.00}", (rd[0].Nov), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumNov = Convert.ToString(rd[0].Nov); }
                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumA12 = Convert.ToDouble(rd[0].Dec + rd[1].Dec);
                                                if (SumA12 >= 1000 || SumA12 <= -1000)
                                                {

                                                    data.SumDec = string.Format("{0:0,000.00}", (rd[0].Dec + rd[1].Dec), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumDec = Convert.ToString(rd[0].Dec + rd[1].Dec); }
                                            }
                                            else
                                            {
                                                Double SumA12 = Convert.ToDouble(rd[0].Dec);
                                                if (SumA12 >= 1000 || SumA12 <= -1000)
                                                {

                                                    data.SumDec = string.Format("{0:0,000.00}", (rd[0].Dec), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumDec = Convert.ToString(rd[0].Dec); }
                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumTotal = Convert.ToDouble(rd[0].Total + rd[1].Total);
                                                if (SumTotal >= 1000 || SumTotal <= -1000)
                                                {

                                                    data.SumTotal = string.Format("{0:0,000.00}", (rd[0].Total + rd[1].Total), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumTotal = Convert.ToString(rd[0].Total + rd[1].Total); }

                                            }
                                            else
                                            {
                                                Double SumTotal = Convert.ToDouble(rd[0].Total);
                                                if (SumTotal >= 1000 || SumTotal <= -1000)
                                                {

                                                    data.SumTotal = string.Format("{0:0,000.00}", (rd[0].Total), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumTotal = Convert.ToString(rd[0].Total); }

                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumLastTotal = Convert.ToDouble(rd[0].LastTotal + rd[1].LastTotal);
                                                if (SumLastTotal >= 1000 || SumLastTotal <= -1000)
                                                {

                                                    data.SumLastTotal = string.Format("{0:0,000.00}", (rd[0].LastTotal + rd[1].LastTotal), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumLastTotal = Convert.ToString(rd[0].LastTotal + rd[1].LastTotal); }
                                            }
                                            else
                                            {
                                                Double SumLastTotal = Convert.ToDouble(rd[0].LastTotal);
                                                if (SumLastTotal >= 1000 || SumLastTotal <= -1000)
                                                {

                                                    data.SumLastTotal = string.Format("{0:0,000.00}", (rd[0].LastTotal), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumLastTotal = Convert.ToString(rd[0].LastTotal); }


                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumpayA1 = Convert.ToDouble(rd[0].PayJan + rd[1].PayJan);
                                                if (SumpayA1 >= 1000 || SumpayA1 <= -1000)
                                                {
                                                    data.SumpayJan = string.Format("{0:0,000.00}", (rd[0].PayJan + rd[1].PayJan), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayJan = Convert.ToString(rd[0].PayJan + rd[1].PayJan); }
                                            }
                                            else
                                            {

                                                Double SumpayA1 = Convert.ToDouble(rd[0].PayJan);
                                                if (SumpayA1 >= 1000 || SumpayA1 <= -1000)
                                                {
                                                    data.SumpayJan = string.Format("{0:0,000.00}", (rd[0].PayJan), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayJan = Convert.ToString(rd[0].PayJan); }


                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumpayA2 = Convert.ToDouble(rd[0].PayFeb + rd[1].PayFeb);
                                                if (SumpayA2 >= 1000 || SumpayA2 <= -1000)
                                                {
                                                    data.SumpayFeb = string.Format("{0:0,000.00}", (rd[0].PayFeb + rd[1].PayFeb), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayFeb = Convert.ToString(rd[0].PayFeb + rd[1].PayFeb); }
                                            }
                                            else
                                            {
                                                Double SumpayA2 = Convert.ToDouble(rd[0].PayFeb);
                                                if (SumpayA2 >= 1000 || SumpayA2 <= -1000)
                                                {
                                                    data.SumpayFeb = string.Format("{0:0,000.00}", (rd[0].PayFeb), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayFeb = Convert.ToString(rd[0].PayFeb); }

                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumpayA3 = Convert.ToDouble(rd[0].PayMar + rd[1].PayMar);
                                                if (SumpayA3 >= 1000 || SumpayA3 <= -1000)
                                                {
                                                    data.SumpayMar = string.Format("{0:0,000.00}", (rd[0].PayMar + rd[1].PayMar), CultureInfo.InvariantCulture);
                                                }
                                                else
                                                { //data.SumpayMar = Convert.ToString(rd[0].PayMar + rd[1].PayMar); 
                                                    data.SumpayMar = Convert.ToString(rd[0].PayMar + rd[1].PayMar);
                                                }
                                            }
                                            else
                                            {
                                                Double SumpayA3 = Convert.ToDouble(rd[0].PayMar);
                                                if (SumpayA3 >= 1000 || SumpayA3 <= -1000)
                                                {
                                                    data.SumpayMar = string.Format("{0:0,000.00}", (rd[0].PayMar), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayMar = Convert.ToString(rd[0].PayMar); }

                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumpayA4 = Convert.ToDouble(rd[0].PayApr + rd[1].PayApr);
                                                if (SumpayA4 >= 1000 || SumpayA4 <= -1000)
                                                {
                                                    data.SumpayApr = string.Format("{0:0,000.00}", (rd[0].PayApr + rd[1].PayApr), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayApr = Convert.ToString(rd[0].PayApr + rd[1].PayApr); }
                                            }
                                            else
                                            {
                                                Double SumpayA4 = Convert.ToDouble(rd[0].PayApr);
                                                if (SumpayA4 >= 1000 || SumpayA4 <= -1000)
                                                {
                                                    data.SumpayApr = string.Format("{0:0,000.00}", (rd[0].PayApr), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayApr = Convert.ToString(rd[0].PayApr); }
                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumpayA5 = Convert.ToDouble(rd[0].PayMay + rd[1].PayMay);
                                                if (SumpayA5 >= 1000 || SumpayA5 <= -1000)
                                                {
                                                    data.SumpayMay = string.Format("{0:0,000.00}", (rd[0].PayMay + rd[1].PayMay), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayMay = Convert.ToString(rd[0].PayMay + rd[1].PayMay); }
                                            }
                                            else
                                            {
                                                Double SumpayA5 = Convert.ToDouble(rd[0].PayMay);
                                                if (SumpayA5 >= 1000 || SumpayA5 <= -1000)
                                                {
                                                    data.SumpayMay = string.Format("{0:0,000.00}", (rd[0].PayMay), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayMay = Convert.ToString(rd[0].PayMay); }
                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumpayA6 = Convert.ToDouble(rd[0].PayJun + rd[1].PayJun);
                                                if (SumpayA6 >= 1000 || SumpayA6 <= -1000)
                                                {
                                                    data.SumpayJun = string.Format("{0:0,000.00}", (rd[0].PayJun + rd[1].PayJun), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayJun = Convert.ToString(rd[0].PayJun + rd[1].PayJun); }
                                            }
                                            else
                                            {
                                                Double SumpayA6 = Convert.ToDouble(rd[0].PayJun);
                                                if (SumpayA6 >= 1000 || SumpayA6 <= -1000)
                                                {
                                                    data.SumpayJun = string.Format("{0:0,000.00}", (rd[0].PayJun), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayJun = Convert.ToString(rd[0].PayJun); }

                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumpayA7 = Convert.ToDouble(rd[0].PayJul + rd[1].PayJul);
                                                if (SumpayA7 >= 1000 || SumpayA7 <= -1000)
                                                {
                                                    data.SumpayJul = string.Format("{0:0,000.00}", (rd[0].PayJul + rd[1].PayJul), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayJul = Convert.ToString(rd[0].PayJul + rd[1].PayJul); }
                                            }
                                            else
                                            {
                                                Double SumpayA7 = Convert.ToDouble(rd[0].PayJul);
                                                if (SumpayA7 >= 1000 || SumpayA7 <= -1000)
                                                {
                                                    data.SumpayJul = string.Format("{0:0,000.00}", (rd[0].PayJul), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayJul = Convert.ToString(rd[0].PayJul); }

                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumpayA8 = Convert.ToDouble(rd[0].PayAug + rd[1].PayAug);
                                                if (SumpayA8 >= 1000 || SumpayA8 <= -1000)
                                                {
                                                    data.SumpayAug = string.Format("{0:0,000.00}", (rd[0].PayAug + rd[1].PayAug), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayAug = Convert.ToString(rd[0].PayAug + rd[1].PayAug); }
                                            }
                                            else
                                            {
                                                Double SumpayA8 = Convert.ToDouble(rd[0].PayAug);
                                                if (SumpayA8 >= 1000 || SumpayA8 <= -1000)
                                                {
                                                    data.SumpayAug = string.Format("{0:0,000.00}", (rd[0].PayAug), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayAug = Convert.ToString(rd[0].PayAug); }
                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumpayA9 = Convert.ToDouble(rd[0].PaySep + rd[1].PaySep);
                                                if (SumpayA9 >= 1000 || SumpayA9 <= -1000)
                                                {
                                                    data.SumpaySep = string.Format("{0:0,000.00}", (rd[0].PaySep + rd[1].PaySep), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpaySep = Convert.ToString(rd[0].PaySep + rd[1].PaySep); }
                                            }
                                            else
                                            {
                                                Double SumpayA9 = Convert.ToDouble(rd[0].PaySep);
                                                if (SumpayA9 >= 1000 || SumpayA9 <= -1000)
                                                {
                                                    data.SumpaySep = string.Format("{0:0,000.00}", (rd[0].PaySep));
                                                }
                                                else { data.SumpaySep = Convert.ToString(rd[0].PaySep); }
                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumpayA10 = Convert.ToDouble(rd[0].PayOct + rd[1].PayOct);
                                                if (SumpayA10 >= 1000 || SumpayA10 <= -1000)
                                                {
                                                    data.SumpayOct = string.Format("{0:0,000.00}", (rd[0].PayOct + rd[1].PayOct), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayOct = Convert.ToString(rd[0].PayOct + rd[1].PayOct); }
                                            }
                                            else
                                            {
                                                Double SumpayA10 = Convert.ToDouble(rd[0].PayOct);
                                                if (SumpayA10 >= 1000 || SumpayA10 <= -1000)
                                                {
                                                    data.SumpayOct = string.Format("{0:0,000.00}", (rd[0].PayOct), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayOct = Convert.ToString(rd[0].PayOct); }

                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumpayA11 = Convert.ToDouble(rd[0].PayNov + rd[1].PayNov);
                                                if (SumpayA11 >= 1000 || SumpayA11 <= -1000)
                                                {
                                                    data.SumpayNov = string.Format("{0:0,000.00}", (rd[0].PayNov + rd[1].PayNov), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayNov = Convert.ToString(rd[0].PayNov + rd[1].PayNov); }
                                            }
                                            else
                                            {
                                                Double SumpayA11 = Convert.ToDouble(rd[0].PayNov);
                                                if (SumpayA11 >= 1000 || SumpayA11 <= -1000)
                                                {
                                                    data.SumpayNov = string.Format("{0:0,000.00}", (rd[0].PayNov), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayNov = Convert.ToString(rd[0].PayNov); }
                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumpayA12 = Convert.ToDouble(rd[0].PayDec + rd[1].PayDec);
                                                if (SumpayA12 >= 1000 || SumpayA12 <= -1000)
                                                {
                                                    data.SumpayDec = string.Format("{0:0,000.00}", (rd[0].PayDec + rd[1].PayDec), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayDec = Convert.ToString(rd[0].PayDec + rd[1].PayDec); }
                                            }
                                            else
                                            {
                                                Double SumpayA12 = Convert.ToDouble(rd[0].PayDec);
                                                if (SumpayA12 >= 1000 || SumpayA12 <= -1000)
                                                {

                                                    data.SumpayDec = string.Format("{0:0,000.00}", (rd[0].PayDec), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayDec = Convert.ToString(rd[0].PayDec); }


                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumpayTotal = Convert.ToDouble(rd[0].PayTotal + rd[1].PayTotal);
                                                if (SumpayTotal >= 1000 || SumpayTotal <= -1000)
                                                {
                                                    data.SumpayTotal = string.Format("{0:0,000.00}", (rd[0].PayTotal + rd[1].PayTotal), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayTotal = Convert.ToString(rd[0].PayTotal + rd[1].PayTotal); }
                                            }
                                            else
                                            {

                                                Double SumpayTotal = Convert.ToDouble(rd[0].PayTotal);
                                                if (SumpayTotal >= 1000 || SumpayTotal <= -1000)
                                                {
                                                    data.SumpayTotal = string.Format("{0:0,000.00}", (rd[0].PayTotal), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumpayTotal = Convert.ToString(rd[0].PayTotal); }

                                            }
                                            if (rd.Length > 1)
                                            {
                                                Double SumLastpayTotal = Convert.ToDouble(rd[0].LastPayTotal + rd[1].LastPayTotal);
                                                if (SumLastpayTotal >= 1000 || SumLastpayTotal <= -1000)
                                                {
                                                    data.SumLastpayTotal = string.Format("{0:0,000.00}", (rd[0].LastPayTotal + rd[1].LastPayTotal), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumLastpayTotal = Convert.ToString(rd[0].LastPayTotal + rd[1].LastPayTotal); }
                                            }
                                            else
                                            {
                                                Double SumLastpayTotal = Convert.ToDouble(rd[0].LastPayTotal);
                                                if (SumLastpayTotal >= 1000 || SumLastpayTotal <= -1000)
                                                {
                                                    data.SumLastpayTotal = string.Format("{0:0,000.00}", (rd[0].LastPayTotal), CultureInfo.InvariantCulture);
                                                }
                                                else { data.SumLastpayTotal = Convert.ToString(rd[0].LastPayTotal); }

                                            }

                                            GetdatatoDisplay.Add(new Listdata { val = data });

                                        }
                                    }

                                }



                            }
                        }

                    }
                    else
                    {


                        string User = string.Empty;
                        string CUser = string.Empty;
                        var command = new SqlCommand("P_Find_CUSCOD_Statement", Connection);
                        command.CommandType = CommandType.StoredProcedure;
                        //int intcal = Convert.ToInt32(strcalstr);
                        command.Parameters.AddWithValue("@SLMCOD", slmcode);
                        command.Parameters.AddWithValue("@CUSCOD ", chcustomer);
                        //command.Parameters.AddWithValue("@CALTYP", intcal);
                        command.ExecuteNonQuery();
                        SqlDataReader drd = command.ExecuteReader();
                        while (drd.Read())
                        {
                            User = drd.GetValue(0).ToString().Trim();

                            var Customer = db.v_Promo_Statement_Customer.Where(c => c.CUSCOD == User).ToArray();
                            for (int t = 0; t < Customer.Length; t++)
                            {
                                dataCus = new Cus();
                                dataCus.CUSCOD = Customer[t].CUSCOD.ToString().Trim();
                                dataCus.CUSNAM = Customer[t].CUSNAM;
                                dataCus.TELNUM = Customer[t].TELNUM;
                                dataCus.ADDR_01 = Customer[t].ADDR_01;

                                string Cus = Customer[t].CUSCOD;
                                string SLM = Customer[t].SLMCOD;
                                //if (caltyp == 2)
                                // {
                                // dataCus.CUSCOD = Customer[t].CUSCOD;
                                // dataCus.CUSNAM = Customer[t].CUSNAM;

                                dataCus.SLMCOD = Customer[t].SLMCOD;
                                dataCus.SLMNAM = Customer[t].SLMNAM;
                                DateTime dateAsof = Convert.ToDateTime(Customer[t].Asof);
                                String formatAsof = "dd/MMM/yyyy";
                                String date = dateAsof.ToString(formatAsof);
                                dataCus.Asof = date;
                                //  Connection.Open();
                                if (slmcode != "ALL")
                                {
                                    //var supslm = db.v_SLMTAB_SupSlm.Where(s => s.SUP == slmcode).ToArray();

                                    SqlCommand cmdCUSOCD = new SqlCommand("select * from v_Promo_Statement_Reward where CUSCOD =N'" + Cus + "'and SLMCOD ='" + slmcode + "' and PYear ='" + DateTime.Now.ToString("yy") + "'", Connection);
                                    SqlDataReader reCUSOCD = cmdCUSOCD.ExecuteReader();

                                    while (reCUSOCD.Read())
                                    {
                                        dataReward = new ItemReward();
                                        // GroupName = reCUSOCD.GetValue(0).ToString();
                                        dataReward.CUSCODReward = reCUSOCD.GetValue(0).ToString();
                                        dataReward.Promotion_Code = reCUSOCD.GetValue(4).ToString();
                                        string CPromotion = reCUSOCD.GetValue(5).ToString();
                                        dataReward.CUSCOD = reCUSOCD.GetValue(1).ToString();
                                        dataReward.CUSNAM = reCUSOCD.GetValue(2).ToString();
                                        DateTime dateSd = Convert.ToDateTime(reCUSOCD.GetValue(12).ToString());
                                        String format = "dd/MMM/yyyy";
                                        String dateStr = dateSd.ToString(format);
                                        dataReward.Startdate = dateStr;

                                        DateTime dateEd = Convert.ToDateTime(reCUSOCD.GetValue(13).ToString());
                                        String formatEd = "dd/MMM/yyyy";
                                        String dateE = dateEd.ToString(formatEd);
                                        dataReward.Enddate = dateE;
                                        if (CPromotion != null)
                                        {
                                            dataReward.Promotion_Name = CPromotion;

                                        }
                                        else
                                        {
                                            dataReward.Promotion_Name = "-";
                                        }

                                        Double SAMT = Convert.ToDouble(reCUSOCD.GetValue(6).ToString());
                                        if (SAMT >= 1000 || SAMT <= -1000)
                                        {
                                            dataReward.SAMT = string.Format("{0:0,000.00}", reCUSOCD.GetValue(6), CultureInfo.InvariantCulture);
                                        }
                                        else { dataReward.SAMT = Convert.ToString(reCUSOCD.GetValue(6).ToString()); }
                                        Double SPaidAMT = Convert.ToDouble(reCUSOCD.GetValue(7).ToString());
                                        if (SPaidAMT >= 1000 || SPaidAMT <= -1000)
                                        {
                                            dataReward.SPaidAMT = string.Format("{0:0,000.00}", reCUSOCD.GetValue(7), CultureInfo.InvariantCulture);
                                        }
                                        else { dataReward.SPaidAMT = Convert.ToString(reCUSOCD.GetValue(7)); }


                                        //int summy = s2 - 21;
                                        Double s1 = Convert.ToDouble(reCUSOCD.GetValue(6));
                                        Double s2 = Convert.ToDouble(reCUSOCD.GetValue(8));
                                        Double summy = s2 - s1;
                                        Double StrSummy = Convert.ToDouble(summy);
                                        if (summy > 0)
                                        {
                                            if (StrSummy >= 1000 || StrSummy <= -1000)
                                            {
                                                dataReward.Sum = string.Format("{0:0,000.00}", summy, CultureInfo.InvariantCulture);
                                            }
                                            else { dataReward.Sum = Convert.ToString(summy); }
                                        }
                                        else
                                        {

                                            dataReward.Sum = "0";
                                        }
                                        Double Condition = Convert.ToDouble(reCUSOCD.GetValue(8));
                                        if (Condition >= 1000 || Condition <= -1000)
                                        {
                                            dataReward.Condition = string.Format("{0:0,000.00}", reCUSOCD.GetValue(8), CultureInfo.InvariantCulture);
                                        }
                                        else { dataReward.Condition = Convert.ToString(reCUSOCD.GetValue(8)); }
                                        dataReward.Reward = reCUSOCD.GetValue(9).ToString();
                                        GetdataReward.Add(new ListdataReward { val = dataReward });
                                        //GroupCus.Add(new SelectListItem() { Value = reCUSOCD.GetValue(1).ToString(), Text = reCUSOCD.GetValue(1).ToString() });
                                    }
                                    reCUSOCD.Dispose();
                                    var rd = db.v_Promo_Statement_Inv.Where(c => c.CUSKEY == User).ToArray();
                                    // if (rs.Length > 1)
                                    // {
                                    for (int i = 0; i < rd.Length; i++)
                                    {

                                        data = new Item();
                                        data.COMP = rd[i].COMP;
                                        data.CUSCOD = rd[i].CUSKEY.ToString().Trim();
                                        int Y = Convert.ToInt32(rd[i].StYr) - 1;
                                        data.LastStYr = Convert.ToString(Y);
                                        data.StYr = Convert.ToString(rd[i].StYr);


                                        Double LastTotal = Convert.ToDouble(rd[i].LastTotal);
                                        if (LastTotal >= 1000 || LastTotal <= -1000)
                                        {

                                            data.LastTotal = String.Format("{0:0,000.00}", rd[i].LastTotal, CultureInfo.InvariantCulture);

                                        }
                                        else { data.LastTotal = Convert.ToString(rd[i].LastTotal); }



                                        Double A1 = Convert.ToDouble(rd[i].Jan);
                                        if (A1 >= 1000 || A1 <= -1000)
                                        {

                                            data.Jan = String.Format("{0:0,000.00}", rd[i].Jan, CultureInfo.InvariantCulture);

                                        }
                                        else { data.Jan = Convert.ToString(rd[i].Jan); }

                                        Double A2 = Convert.ToDouble(rd[i].Feb);
                                        if (A2 >= 1000 || A2 <= -1000)
                                        {

                                            data.Feb = string.Format("{0:0,000.00}", rd[i].Feb, CultureInfo.InvariantCulture);
                                        }
                                        else { data.Feb = Convert.ToString(rd[i].Feb); }

                                        Double A3 = Convert.ToDouble(rd[i].Mar);
                                        if (A3 >= 1000 || A3 <= -1000)
                                        {

                                            data.Mar = string.Format("{0:0,000.00}", rd[i].Mar, CultureInfo.InvariantCulture);
                                        }
                                        else { data.Mar = Convert.ToString(rd[i].Mar); }

                                        Double A4 = Convert.ToDouble(rd[i].Apr);
                                        if (A4 >= 1000 || A4 <= -1000)
                                        {

                                            data.Apr = string.Format("{0:0,000.00}", rd[i].Apr, CultureInfo.InvariantCulture);
                                        }
                                        else { data.Apr = Convert.ToString(rd[i].Apr); }

                                        Double A5 = Convert.ToDouble(rd[i].May);
                                        if (A5 >= 1000 || A5 <= -1000)
                                        {

                                            data.May = string.Format("{0:0,000.00}", rd[i].May, CultureInfo.InvariantCulture);
                                        }
                                        else { data.May = Convert.ToString(rd[i].May); }


                                        Double A6 = Convert.ToDouble(rd[i].Jun);
                                        if (A6 >= 1000 || A6 <= -1000)
                                        {

                                            data.Jun = string.Format("{0:0,000.00}", rd[i].Jun, CultureInfo.InvariantCulture);
                                        }
                                        else { data.Jun = Convert.ToString(rd[i].Jun); }

                                        Double A7 = Convert.ToDouble(rd[i].Jul);
                                        if (A7 >= 1000 || A7 <= -1000)
                                        {

                                            data.Jul = string.Format("{0:0,000.00}", rd[i].Jul, CultureInfo.InvariantCulture);
                                        }
                                        else { data.Jul = Convert.ToString(rd[i].Jul); }

                                        Double A8 = Convert.ToDouble(rd[i].Aug);
                                        if (A8 >= 1000 || A8 <= -1000)
                                        {

                                            data.Aug = string.Format("{0:0,000.00}", rd[i].Aug, CultureInfo.InvariantCulture);
                                        }
                                        else { data.Aug = Convert.ToString(rd[i].Aug); }

                                        Double A9 = Convert.ToDouble(rd[i].Sep);
                                        if (A9 >= 1000 || A9 <= -1000)
                                        {
                                            data.Sep = string.Format("{0:0,000.00}", rd[i].Sep, CultureInfo.InvariantCulture);
                                        }
                                        else { data.Sep = Convert.ToString(rd[i].Sep); }

                                        Double A10 = Convert.ToDouble(rd[i].Oct);
                                        if (A10 >= 1000 || A10 <= -1000)
                                        {
                                            data.Oct = string.Format("{0:0,000.00}", rd[i].Oct, CultureInfo.InvariantCulture);
                                        }
                                        else { data.Oct = Convert.ToString(rd[i].Oct); }

                                        Double A11 = Convert.ToDouble(rd[i].Nov);
                                        if (A11 >= 1000 || A11 <= -1000)
                                        {
                                            data.Nov = string.Format("{0:0,000.00}", rd[i].Nov, CultureInfo.InvariantCulture);
                                        }
                                        else { data.Nov = Convert.ToString(rd[i].Nov); }

                                        Double A12 = Convert.ToDouble(rd[i].Dec);
                                        if (A12 >= 1000 || A12 <= -1000)
                                        {
                                            data.Dec = string.Format("{0:0,000.00}", rd[i].Dec, CultureInfo.InvariantCulture);
                                        }
                                        else { data.Dec = Convert.ToString(rd[i].Dec); }

                                        Double Total = Convert.ToDouble(rd[i].Total);
                                        if (Total >= 1000 || Total <= -1000)
                                        {
                                            data.Total = string.Format("{0:0,000.00}", rd[i].Total, CultureInfo.InvariantCulture);
                                        }
                                        else { data.Total = Convert.ToString(rd[i].Total); }


                                        if (rd.Length > 1)
                                        {

                                            Double SumA1 = Convert.ToDouble(rd[0].Jan + rd[1].Jan, CultureInfo.InvariantCulture);
                                            if (SumA1 >= 1000 || SumA1 <= -1000)
                                            {
                                                data.SumJan = string.Format("{0:0,000.00}", (rd[i].Jan + rd[1].Jan), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumJan = Convert.ToString(rd[i].Jan + rd[1].Jan); }
                                        }
                                        else
                                        {

                                            Double SumA1 = Convert.ToDouble(rd[i].Jan);
                                            if (SumA1 >= 1000 || SumA1 <= -1000)
                                            {
                                                data.SumJan = string.Format("{0:0,000.00}", (rd[i].Jan), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumJan = Convert.ToString(rd[i].Jan); }

                                        }

                                        if (rd.Length > 1)
                                        {
                                            Double SumA2 = Convert.ToDouble(rd[i].Feb + rd[1].Feb);
                                            if (SumA2 >= 1000 || SumA2 <= -1000)
                                            {
                                                data.SumFeb = string.Format("{0:0,000.00}", (rd[i].Feb + rd[1].Feb), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumFeb = Convert.ToString(rd[i].Feb + rd[1].Feb); }
                                        }
                                        else
                                        {
                                            Double SumA2 = Convert.ToDouble(rd[i].Feb);
                                            if (SumA2 >= 1000 || SumA2 <= -1000)
                                            {
                                                data.SumFeb = string.Format("{0:0,000.00}", (rd[i].Feb), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumFeb = Convert.ToString(rd[i].Feb); }
                                        }

                                        if (rd.Length > 1)
                                        {
                                            Double SumA3 = Convert.ToDouble(rd[i].Mar + rd[1].Mar);
                                            if (SumA3 >= 1000 || SumA3 <= -1000)
                                            {

                                                data.SumMar = string.Format("{0:0,000.00}", (rd[i].Mar + rd[1].Mar), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumMar = Convert.ToString(rd[i].Mar + rd[1].Mar); }
                                        }
                                        else
                                        {
                                            Double SumA3 = Convert.ToDouble(rd[i].Mar);
                                            if (SumA3 >= 1000 || SumA3 <= -1000)
                                            {

                                                data.SumMar = string.Format("{0:0,000.00}", (rd[i].Mar), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumMar = Convert.ToString(rd[i].Mar); }

                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumA4 = Convert.ToDouble(rd[i].Apr + rd[1].Apr);
                                            if (SumA4 >= 1000 || SumA4 <= -1000)
                                            {

                                                data.SumApr = string.Format("{0:0,000.00}", (rd[i].Apr + rd[1].Apr), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumApr = Convert.ToString(rd[i].Apr + rd[1].Apr); }
                                        }
                                        else
                                        {
                                            Double SumA4 = Convert.ToDouble(rd[i].Apr);
                                            if (SumA4 >= 1000 || SumA4 <= -1000)
                                            {

                                                data.SumApr = string.Format("{0:0,000.00}", (rd[i].Apr), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumApr = Convert.ToString(rd[i].Apr); }
                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumA5 = Convert.ToDouble(rd[i].May + rd[1].May);
                                            if (SumA5 >= 1000 || SumA5 <= -1000)
                                            {

                                                data.SumMay = string.Format("{0:0,000.00}", (rd[i].May + rd[1].May), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumMay = Convert.ToString(rd[i].May + rd[1].May); }
                                        }
                                        else
                                        {
                                            Double SumA5 = Convert.ToDouble(rd[i].May);
                                            if (SumA5 >= 1000 || SumA5 <= -1000)
                                            {

                                                data.SumMay = string.Format("{0:0,000.00}", (rd[i].May), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumMay = Convert.ToString(rd[i].May); }
                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumA6 = Convert.ToDouble(rd[i].Jun + rd[1].Jun);
                                            if (SumA6 >= 1000 || SumA6 <= -1000)
                                            {

                                                data.SumJun = string.Format("{0:0,000.00}", (rd[i].Jun + rd[1].Jun), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumJun = Convert.ToString(rd[i].Jun + rd[1].Jun); }
                                        }
                                        else
                                        {
                                            Double SumA6 = Convert.ToDouble(rd[i].Jun);
                                            if (SumA6 >= 1000 || SumA6 <= -1000)
                                            {

                                                data.SumJun = string.Format("{0:0,000.00}", (rd[i].Jun), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumJun = Convert.ToString(rd[i].Jun); }
                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumA7 = Convert.ToDouble(rd[i].Jul + rd[1].Jul);
                                            if (SumA7 >= 1000 || SumA7 <= -1000)
                                            {

                                                data.SumJul = string.Format("{0:0,000.00}", (rd[i].Jul + rd[1].Jul), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumJul = Convert.ToString(rd[0].Jul + rd[1].Jul); }
                                        }
                                        else
                                        {
                                            Double SumA7 = Convert.ToDouble(rd[i].Jul);
                                            if (SumA7 >= 1000 || SumA7 <= -1000)
                                            {

                                                data.SumJul = string.Format("{0:0,000.00}", (rd[i].Jul), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumJul = Convert.ToString(rd[0].Jul); }

                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumA8 = Convert.ToDouble(rd[0].Aug + rd[1].Aug);
                                            if (SumA8 >= 1000 || SumA8 <= -1000)
                                            {

                                                data.SumAug = string.Format("{0:0,000.00}", (rd[0].Aug + rd[1].Aug), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumAug = Convert.ToString(rd[0].Aug + rd[1].Aug); }
                                        }
                                        else
                                        {

                                            Double SumA8 = Convert.ToDouble(rd[0].Aug);
                                            if (SumA8 >= 1000 || SumA8 <= -1000)
                                            {

                                                data.SumAug = string.Format("{0:0,000.00}", (rd[0].Aug), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumAug = Convert.ToString(rd[0].Aug); }
                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumA9 = Convert.ToDouble(rd[0].Sep + rd[1].Sep);
                                            if (SumA9 >= 1000 || SumA9 <= -1000)
                                            {

                                                data.SumSep = string.Format("{0:0,000.00}", (rd[0].Sep + rd[1].Sep), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumSep = Convert.ToString(rd[0].Sep + rd[1].Sep); }
                                        }
                                        else
                                        {
                                            Double SumA9 = Convert.ToDouble(rd[0].Sep);
                                            if (SumA9 >= 1000 || SumA9 <= -1000)
                                            {

                                                data.SumSep = string.Format("{0:0,000.00}", (rd[0].Sep), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumSep = Convert.ToString(rd[0].Sep); }

                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumA10 = Convert.ToDouble(rd[0].Oct + rd[1].Oct);
                                            if (SumA10 >= 1000 || SumA10 <= -1000)
                                            {

                                                data.SumOct = string.Format("{0:0,000.00}", (rd[0].Oct + rd[1].Oct), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumOct = Convert.ToString(rd[0].Oct + rd[1].Oct); }
                                        }
                                        else
                                        {
                                            Double SumA10 = Convert.ToDouble(rd[0].Oct);
                                            if (SumA10 >= 1000 || SumA10 <= -1000)
                                            {

                                                data.SumOct = string.Format("{0:0,000.00}", (rd[0].Oct), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumOct = Convert.ToString(rd[0].Oct); }


                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumA11 = Convert.ToDouble(rd[0].Nov + rd[1].Nov);
                                            if (SumA11 >= 1000 || SumA11 <= -1000)
                                            {

                                                data.SumNov = string.Format("{0:0,000.00}", (rd[0].Nov + rd[1].Nov), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumNov = Convert.ToString(rd[0].Nov + rd[1].Nov); }
                                        }
                                        else
                                        {

                                            Double SumA11 = Convert.ToDouble(rd[0].Nov);
                                            if (SumA11 >= 1000 || SumA11 <= -1000)
                                            {

                                                data.SumNov = string.Format("{0:0,000.00}", (rd[0].Nov), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumNov = Convert.ToString(rd[0].Nov); }
                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumA12 = Convert.ToDouble(rd[0].Dec + rd[1].Dec);
                                            if (SumA12 >= 1000 || SumA12 <= -1000)
                                            {

                                                data.SumDec = string.Format("{0:0,000.00}", (rd[0].Dec + rd[1].Dec), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumDec = Convert.ToString(rd[0].Dec + rd[1].Dec); }
                                        }
                                        else
                                        {
                                            Double SumA12 = Convert.ToDouble(rd[0].Dec);
                                            if (SumA12 >= 1000 || SumA12 <= -1000)
                                            {

                                                data.SumDec = string.Format("{0:0,000.00}", (rd[0].Dec), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumDec = Convert.ToString(rd[0].Dec); }
                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumTotal = Convert.ToDouble(rd[0].Total + rd[1].Total);
                                            if (SumTotal >= 1000 || SumTotal <= -1000)
                                            {

                                                data.SumTotal = string.Format("{0:0,000.00}", (rd[0].Total + rd[1].Total), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumTotal = Convert.ToString(rd[0].Total + rd[1].Total); }

                                        }
                                        else
                                        {
                                            Double SumTotal = Convert.ToDouble(rd[0].Total);
                                            if (SumTotal >= 1000 || SumTotal <= -1000)
                                            {

                                                data.SumTotal = string.Format("{0:0,000.00}", (rd[0].Total), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumTotal = Convert.ToString(rd[0].Total); }

                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumLastTotal = Convert.ToDouble(rd[0].LastTotal + rd[1].LastTotal);
                                            if (SumLastTotal >= 1000 || SumLastTotal <= -1000)
                                            {

                                                data.SumLastTotal = string.Format("{0:0,000.00}", (rd[0].LastTotal + rd[1].LastTotal), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumLastTotal = Convert.ToString(rd[0].LastTotal + rd[1].LastTotal); }
                                        }
                                        else
                                        {
                                            Double SumLastTotal = Convert.ToDouble(rd[0].LastTotal);
                                            if (SumLastTotal >= 1000 || SumLastTotal <= -1000)
                                            {

                                                data.SumLastTotal = string.Format("{0:0,000.00}", (rd[0].LastTotal), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumLastTotal = Convert.ToString(rd[0].LastTotal); }


                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumpayA1 = Convert.ToDouble(rd[0].PayJan + rd[1].PayJan);
                                            if (SumpayA1 >= 1000 || SumpayA1 <= -1000)
                                            {
                                                data.SumpayJan = string.Format("{0:0,000.00}", (rd[0].PayJan + rd[1].PayJan), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayJan = Convert.ToString(rd[0].PayJan + rd[1].PayJan); }
                                        }
                                        else
                                        {

                                            Double SumpayA1 = Convert.ToDouble(rd[0].PayJan);
                                            if (SumpayA1 >= 1000 || SumpayA1 <= -1000)
                                            {
                                                data.SumpayJan = string.Format("{0:0,000.00}", (rd[0].PayJan), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayJan = Convert.ToString(rd[0].PayJan); }


                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumpayA2 = Convert.ToDouble(rd[0].PayFeb + rd[1].PayFeb);
                                            if (SumpayA2 >= 1000 || SumpayA2 <= -1000)
                                            {
                                                data.SumpayFeb = string.Format("{0:0,000.00}", (rd[0].PayFeb + rd[1].PayFeb), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayFeb = Convert.ToString(rd[0].PayFeb + rd[1].PayFeb); }
                                        }
                                        else
                                        {
                                            Double SumpayA2 = Convert.ToDouble(rd[0].PayFeb);
                                            if (SumpayA2 >= 1000 || SumpayA2 <= -1000)
                                            {
                                                data.SumpayFeb = string.Format("{0:0,000.00}", (rd[0].PayFeb), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayFeb = Convert.ToString(rd[0].PayFeb); }

                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumpayA3 = Convert.ToDouble(rd[0].PayMar + rd[1].PayMar);
                                            if (SumpayA3 >= 1000 || SumpayA3 <= -1000)
                                            {
                                                data.SumpayMar = string.Format("{0:0,000.00}", (rd[0].PayMar + rd[1].PayMar), CultureInfo.InvariantCulture);
                                            }
                                            else
                                            { //data.SumpayMar = Convert.ToString(rd[0].PayMar + rd[1].PayMar); 
                                                data.SumpayMar = Convert.ToString(rd[0].PayMar + rd[1].PayMar);
                                            }
                                        }
                                        else
                                        {
                                            Double SumpayA3 = Convert.ToDouble(rd[0].PayMar);
                                            if (SumpayA3 >= 1000 || SumpayA3 <= -1000)
                                            {
                                                data.SumpayMar = string.Format("{0:0,000.00}", (rd[0].PayMar), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayMar = Convert.ToString(rd[0].PayMar); }

                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumpayA4 = Convert.ToDouble(rd[0].PayApr + rd[1].PayApr);
                                            if (SumpayA4 >= 1000 || SumpayA4 <= -1000)
                                            {
                                                data.SumpayApr = string.Format("{0:0,000.00}", (rd[0].PayApr + rd[1].PayApr), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayApr = Convert.ToString(rd[0].PayApr + rd[1].PayApr); }
                                        }
                                        else
                                        {
                                            Double SumpayA4 = Convert.ToDouble(rd[0].PayApr);
                                            if (SumpayA4 >= 1000 || SumpayA4 <= -1000)
                                            {
                                                data.SumpayApr = string.Format("{0:0,000.00}", (rd[0].PayApr), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayApr = Convert.ToString(rd[0].PayApr); }
                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumpayA5 = Convert.ToDouble(rd[0].PayMay + rd[1].PayMay);
                                            if (SumpayA5 >= 1000 || SumpayA5 <= -1000)
                                            {
                                                data.SumpayMay = string.Format("{0:0,000.00}", (rd[0].PayMay + rd[1].PayMay), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayMay = Convert.ToString(rd[0].PayMay + rd[1].PayMay); }
                                        }
                                        else
                                        {
                                            Double SumpayA5 = Convert.ToDouble(rd[0].PayMay);
                                            if (SumpayA5 >= 1000 || SumpayA5 <= -1000)
                                            {
                                                data.SumpayMay = string.Format("{0:0,000.00}", (rd[0].PayMay), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayMay = Convert.ToString(rd[0].PayMay); }
                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumpayA6 = Convert.ToDouble(rd[0].PayJun + rd[1].PayJun);
                                            if (SumpayA6 >= 1000 || SumpayA6 <= -1000)
                                            {
                                                data.SumpayJun = string.Format("{0:0,000.00}", (rd[0].PayJun + rd[1].PayJun), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayJun = Convert.ToString(rd[0].PayJun + rd[1].PayJun); }
                                        }
                                        else
                                        {
                                            Double SumpayA6 = Convert.ToDouble(rd[0].PayJun);
                                            if (SumpayA6 >= 1000 || SumpayA6 <= -1000)
                                            {
                                                data.SumpayJun = string.Format("{0:0,000.00}", (rd[0].PayJun), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayJun = Convert.ToString(rd[0].PayJun); }

                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumpayA7 = Convert.ToDouble(rd[0].PayJul + rd[1].PayJul);
                                            if (SumpayA7 >= 1000 || SumpayA7 <= -1000)
                                            {
                                                data.SumpayJul = string.Format("{0:0,000.00}", (rd[0].PayJul + rd[1].PayJul), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayJul = Convert.ToString(rd[0].PayJul + rd[1].PayJul); }
                                        }
                                        else
                                        {
                                            Double SumpayA7 = Convert.ToDouble(rd[0].PayJul);
                                            if (SumpayA7 >= 1000 || SumpayA7 <= -1000)
                                            {
                                                data.SumpayJul = string.Format("{0:0,000.00}", (rd[0].PayJul), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayJul = Convert.ToString(rd[0].PayJul); }

                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumpayA8 = Convert.ToDouble(rd[0].PayAug + rd[1].PayAug);
                                            if (SumpayA8 >= 1000 || SumpayA8 <= -1000)
                                            {
                                                data.SumpayAug = string.Format("{0:0,000.00}", (rd[0].PayAug + rd[1].PayAug), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayAug = Convert.ToString(rd[0].PayAug + rd[1].PayAug); }
                                        }
                                        else
                                        {
                                            Double SumpayA8 = Convert.ToDouble(rd[0].PayAug);
                                            if (SumpayA8 >= 1000 || SumpayA8 <= -1000)
                                            {
                                                data.SumpayAug = string.Format("{0:0,000.00}", (rd[0].PayAug), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayAug = Convert.ToString(rd[0].PayAug); }
                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumpayA9 = Convert.ToDouble(rd[0].PaySep + rd[1].PaySep);
                                            if (SumpayA9 >= 1000 || SumpayA9 <= -1000)
                                            {
                                                data.SumpaySep = string.Format("{0:0,000.00}", (rd[0].PaySep + rd[1].PaySep), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpaySep = Convert.ToString(rd[0].PaySep + rd[1].PaySep); }
                                        }
                                        else
                                        {
                                            Double SumpayA9 = Convert.ToDouble(rd[0].PaySep);
                                            if (SumpayA9 >= 1000 || SumpayA9 <= -1000)
                                            {
                                                data.SumpaySep = string.Format("{0:0,000.00}", (rd[0].PaySep));
                                            }
                                            else { data.SumpaySep = Convert.ToString(rd[0].PaySep); }
                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumpayA10 = Convert.ToDouble(rd[0].PayOct + rd[1].PayOct);
                                            if (SumpayA10 >= 1000 || SumpayA10 <= -1000)
                                            {
                                                data.SumpayOct = string.Format("{0:0,000.00}", (rd[0].PayOct + rd[1].PayOct), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayOct = Convert.ToString(rd[0].PayOct + rd[1].PayOct); }
                                        }
                                        else
                                        {
                                            Double SumpayA10 = Convert.ToDouble(rd[0].PayOct);
                                            if (SumpayA10 >= 1000 || SumpayA10 <= -1000)
                                            {
                                                data.SumpayOct = string.Format("{0:0,000.00}", (rd[0].PayOct), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayOct = Convert.ToString(rd[0].PayOct); }

                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumpayA11 = Convert.ToDouble(rd[0].PayNov + rd[1].PayNov);
                                            if (SumpayA11 >= 1000 || SumpayA11 <= -1000)
                                            {
                                                data.SumpayNov = string.Format("{0:0,000.00}", (rd[0].PayNov + rd[1].PayNov), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayNov = Convert.ToString(rd[0].PayNov + rd[1].PayNov); }
                                        }
                                        else
                                        {
                                            Double SumpayA11 = Convert.ToDouble(rd[0].PayNov);
                                            if (SumpayA11 >= 1000 || SumpayA11 <= -1000)
                                            {
                                                data.SumpayNov = string.Format("{0:0,000.00}", (rd[0].PayNov), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayNov = Convert.ToString(rd[0].PayNov); }
                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumpayA12 = Convert.ToDouble(rd[0].PayDec + rd[1].PayDec);
                                            if (SumpayA12 >= 1000 || SumpayA12 <= -1000)
                                            {
                                                data.SumpayDec = string.Format("{0:0,000.00}", (rd[0].PayDec + rd[1].PayDec), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayDec = Convert.ToString(rd[0].PayDec + rd[1].PayDec); }
                                        }
                                        else
                                        {
                                            Double SumpayA12 = Convert.ToDouble(rd[0].PayDec);
                                            if (SumpayA12 >= 1000 || SumpayA12 <= -1000)
                                            {

                                                data.SumpayDec = string.Format("{0:0,000.00}", (rd[0].PayDec), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayDec = Convert.ToString(rd[0].PayDec); }


                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumpayTotal = Convert.ToDouble(rd[0].PayTotal + rd[1].PayTotal);
                                            if (SumpayTotal >= 1000 || SumpayTotal <= -1000)
                                            {
                                                data.SumpayTotal = string.Format("{0:0,000.00}", (rd[0].PayTotal + rd[1].PayTotal), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayTotal = Convert.ToString(rd[0].PayTotal + rd[1].PayTotal); }
                                        }
                                        else
                                        {

                                            Double SumpayTotal = Convert.ToDouble(rd[0].PayTotal);
                                            if (SumpayTotal >= 1000 || SumpayTotal <= -1000)
                                            {
                                                data.SumpayTotal = string.Format("{0:0,000.00}", (rd[0].PayTotal), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayTotal = Convert.ToString(rd[0].PayTotal); }

                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumLastpayTotal = Convert.ToDouble(rd[0].LastPayTotal + rd[1].LastPayTotal);
                                            if (SumLastpayTotal >= 1000 || SumLastpayTotal <= -1000)
                                            {
                                                data.SumLastpayTotal = string.Format("{0:0,000.00}", (rd[0].LastPayTotal + rd[1].LastPayTotal), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumLastpayTotal = Convert.ToString(rd[0].LastPayTotal + rd[1].LastPayTotal); }
                                        }
                                        else
                                        {
                                            Double SumLastpayTotal = Convert.ToDouble(rd[0].LastPayTotal);
                                            if (SumLastpayTotal >= 1000 || SumLastpayTotal <= -1000)
                                            {
                                                data.SumLastpayTotal = string.Format("{0:0,000.00}", (rd[0].LastPayTotal), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumLastpayTotal = Convert.ToString(rd[0].LastPayTotal); }

                                        }

                                        GetdatatoDisplay.Add(new Listdata { val = data });

                                    }
                                }
                                else
                                {
                                    SqlCommand cmdCUSOCD = new SqlCommand("select * from v_Promo_Statement_Reward where CUSCOD =N'" + Cus + "'and PYear ='" + DateTime.Now.ToString("yy") + "'", Connection);
                                    SqlDataReader reCUSOCD = cmdCUSOCD.ExecuteReader();

                                    while (reCUSOCD.Read())
                                    {
                                        dataReward = new ItemReward();
                                        // GroupName = reCUSOCD.GetValue(0).ToString();
                                        dataReward.CUSCODReward = reCUSOCD.GetValue(0).ToString();
                                        dataReward.Promotion_Code = reCUSOCD.GetValue(4).ToString();
                                        string CPromotion = reCUSOCD.GetValue(5).ToString();
                                        dataReward.CUSCOD = reCUSOCD.GetValue(1).ToString();
                                        dataReward.CUSNAM = reCUSOCD.GetValue(2).ToString();
                                        DateTime dateSd = Convert.ToDateTime(reCUSOCD.GetValue(12).ToString());
                                        String format = "dd/MMM/yyyy";
                                        String dateStr = dateSd.ToString(format);
                                        dataReward.Startdate = dateStr;

                                        DateTime dateEd = Convert.ToDateTime(reCUSOCD.GetValue(13).ToString());
                                        String formatEd = "dd/MMM/yyyy";
                                        String dateE = dateEd.ToString(formatEd);
                                        dataReward.Enddate = dateE;
                                        if (CPromotion != null)
                                        {
                                            dataReward.Promotion_Name = CPromotion;

                                        }
                                        else
                                        {
                                            dataReward.Promotion_Name = "-";
                                        }

                                        Double SAMT = Convert.ToDouble(reCUSOCD.GetValue(6).ToString());
                                        if (SAMT >= 1000 || SAMT <= -1000)
                                        {
                                            dataReward.SAMT = string.Format("{0:0,000.00}", reCUSOCD.GetValue(6), CultureInfo.InvariantCulture);
                                        }
                                        else { dataReward.SAMT = Convert.ToString(reCUSOCD.GetValue(6).ToString()); }
                                        Double SPaidAMT = Convert.ToDouble(reCUSOCD.GetValue(7).ToString());
                                        if (SPaidAMT >= 1000 || SPaidAMT <= -1000)
                                        {
                                            dataReward.SPaidAMT = string.Format("{0:0,000.00}", reCUSOCD.GetValue(7), CultureInfo.InvariantCulture);
                                        }
                                        else { dataReward.SPaidAMT = Convert.ToString(reCUSOCD.GetValue(7)); }


                                        //int summy = s2 - 21;
                                        Double s1 = Convert.ToDouble(reCUSOCD.GetValue(6));
                                        Double s2 = Convert.ToDouble(reCUSOCD.GetValue(8));
                                        Double summy = s2 - s1;
                                        Double StrSummy = Convert.ToDouble(summy);
                                        if (summy > 0)
                                        {
                                            if (StrSummy >= 1000 || StrSummy <= -1000)
                                            {
                                                dataReward.Sum = string.Format("{0:0,000.00}", summy, CultureInfo.InvariantCulture);
                                            }
                                            else { dataReward.Sum = Convert.ToString(summy); }
                                        }
                                        else
                                        {

                                            dataReward.Sum = "0";
                                        }
                                        Double Condition = Convert.ToDouble(reCUSOCD.GetValue(8));
                                        if (Condition >= 1000 || Condition <= -1000)
                                        {
                                            dataReward.Condition = string.Format("{0:0,000.00}", reCUSOCD.GetValue(8), CultureInfo.InvariantCulture);
                                        }
                                        else { dataReward.Condition = Convert.ToString(reCUSOCD.GetValue(8)); }
                                        dataReward.Reward = reCUSOCD.GetValue(9).ToString();
                                        GetdataReward.Add(new ListdataReward { val = dataReward });
                                        //GroupCus.Add(new SelectListItem() { Value = reCUSOCD.GetValue(1).ToString(), Text = reCUSOCD.GetValue(1).ToString() });
                                    }

                                    var rd = db.v_Promo_Statement_Inv.Where(c => c.CUSKEY == User).ToArray();
                                    // if (rs.Length > 1)
                                    // {
                                    for (int i = 0; i < rd.Length; i++)
                                    {

                                        data = new Item();
                                        data.COMP = rd[i].COMP;
                                        data.CUSCOD = rd[i].CUSKEY.ToString().Trim();
                                        int Y = Convert.ToInt32(rd[i].StYr) - 1;
                                        data.LastStYr = Convert.ToString(Y);
                                        data.StYr = Convert.ToString(rd[i].StYr);


                                        Double LastTotal = Convert.ToDouble(rd[i].LastTotal);
                                        if (LastTotal >= 1000 || LastTotal <= -1000)
                                        {

                                            data.LastTotal = String.Format("{0:0,000.00}", rd[i].LastTotal, CultureInfo.InvariantCulture);

                                        }
                                        else { data.LastTotal = Convert.ToString(rd[i].LastTotal); }



                                        Double A1 = Convert.ToDouble(rd[i].Jan);
                                        if (A1 >= 1000 || A1 <= -1000)
                                        {

                                            data.Jan = String.Format("{0:0,000.00}", rd[i].Jan, CultureInfo.InvariantCulture);

                                        }
                                        else { data.Jan = Convert.ToString(rd[i].Jan); }

                                        Double A2 = Convert.ToDouble(rd[i].Feb);
                                        if (A2 >= 1000 || A2 <= -1000)
                                        {

                                            data.Feb = string.Format("{0:0,000.00}", rd[i].Feb, CultureInfo.InvariantCulture);
                                        }
                                        else { data.Feb = Convert.ToString(rd[i].Feb); }

                                        Double A3 = Convert.ToDouble(rd[i].Mar);
                                        if (A3 >= 1000 || A3 <= -1000)
                                        {

                                            data.Mar = string.Format("{0:0,000.00}", rd[i].Mar, CultureInfo.InvariantCulture);
                                        }
                                        else { data.Mar = Convert.ToString(rd[i].Mar); }

                                        Double A4 = Convert.ToDouble(rd[i].Apr);
                                        if (A4 >= 1000 || A4 <= -1000)
                                        {

                                            data.Apr = string.Format("{0:0,000.00}", rd[i].Apr, CultureInfo.InvariantCulture);
                                        }
                                        else { data.Apr = Convert.ToString(rd[i].Apr); }

                                        Double A5 = Convert.ToDouble(rd[i].May);
                                        if (A5 >= 1000 || A5 <= -1000)
                                        {

                                            data.May = string.Format("{0:0,000.00}", rd[i].May, CultureInfo.InvariantCulture);
                                        }
                                        else { data.May = Convert.ToString(rd[i].May); }


                                        Double A6 = Convert.ToDouble(rd[i].Jun);
                                        if (A6 >= 1000 || A6 <= -1000)
                                        {

                                            data.Jun = string.Format("{0:0,000.00}", rd[i].Jun, CultureInfo.InvariantCulture);
                                        }
                                        else { data.Jun = Convert.ToString(rd[i].Jun); }

                                        Double A7 = Convert.ToDouble(rd[i].Jul);
                                        if (A7 >= 1000 || A7 <= -1000)
                                        {

                                            data.Jul = string.Format("{0:0,000.00}", rd[i].Jul, CultureInfo.InvariantCulture);
                                        }
                                        else { data.Jul = Convert.ToString(rd[i].Jul); }

                                        Double A8 = Convert.ToDouble(rd[i].Aug);
                                        if (A8 >= 1000 || A8 <= -1000)
                                        {

                                            data.Aug = string.Format("{0:0,000.00}", rd[i].Aug, CultureInfo.InvariantCulture);
                                        }
                                        else { data.Aug = Convert.ToString(rd[i].Aug); }

                                        Double A9 = Convert.ToDouble(rd[i].Sep);
                                        if (A9 >= 1000 || A9 <= -1000)
                                        {
                                            data.Sep = string.Format("{0:0,000.00}", rd[i].Sep, CultureInfo.InvariantCulture);
                                        }
                                        else { data.Sep = Convert.ToString(rd[i].Sep); }

                                        Double A10 = Convert.ToDouble(rd[i].Oct);
                                        if (A10 >= 1000 || A10 <= -1000)
                                        {
                                            data.Oct = string.Format("{0:0,000.00}", rd[i].Oct, CultureInfo.InvariantCulture);
                                        }
                                        else { data.Oct = Convert.ToString(rd[i].Oct); }

                                        Double A11 = Convert.ToDouble(rd[i].Nov);
                                        if (A11 >= 1000 || A11 <= -1000)
                                        {
                                            data.Nov = string.Format("{0:0,000.00}", rd[i].Nov, CultureInfo.InvariantCulture);
                                        }
                                        else { data.Nov = Convert.ToString(rd[i].Nov); }

                                        Double A12 = Convert.ToDouble(rd[i].Dec);
                                        if (A12 >= 1000 || A12 <= -1000)
                                        {
                                            data.Dec = string.Format("{0:0,000.00}", rd[i].Dec, CultureInfo.InvariantCulture);
                                        }
                                        else { data.Dec = Convert.ToString(rd[i].Dec); }

                                        Double Total = Convert.ToDouble(rd[i].Total);
                                        if (Total >= 1000 || Total <= -1000)
                                        {
                                            data.Total = string.Format("{0:0,000.00}", rd[i].Total, CultureInfo.InvariantCulture);
                                        }
                                        else { data.Total = Convert.ToString(rd[i].Total); }


                                        if (rd.Length > 1)
                                        {

                                            Double SumA1 = Convert.ToDouble(rd[0].Jan + rd[1].Jan, CultureInfo.InvariantCulture);
                                            if (SumA1 >= 1000 || SumA1 <= -1000)
                                            {
                                                data.SumJan = string.Format("{0:0,000.00}", (rd[i].Jan + rd[1].Jan), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumJan = Convert.ToString(rd[i].Jan + rd[1].Jan); }
                                        }
                                        else
                                        {

                                            Double SumA1 = Convert.ToDouble(rd[i].Jan);
                                            if (SumA1 >= 1000 || SumA1 <= -1000)
                                            {
                                                data.SumJan = string.Format("{0:0,000.00}", (rd[i].Jan), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumJan = Convert.ToString(rd[i].Jan); }

                                        }

                                        if (rd.Length > 1)
                                        {
                                            Double SumA2 = Convert.ToDouble(rd[i].Feb + rd[1].Feb);
                                            if (SumA2 >= 1000 || SumA2 <= -1000)
                                            {
                                                data.SumFeb = string.Format("{0:0,000.00}", (rd[i].Feb + rd[1].Feb), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumFeb = Convert.ToString(rd[i].Feb + rd[1].Feb); }
                                        }
                                        else
                                        {
                                            Double SumA2 = Convert.ToDouble(rd[i].Feb);
                                            if (SumA2 >= 1000 || SumA2 <= -1000)
                                            {
                                                data.SumFeb = string.Format("{0:0,000.00}", (rd[i].Feb), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumFeb = Convert.ToString(rd[i].Feb); }
                                        }

                                        if (rd.Length > 1)
                                        {
                                            Double SumA3 = Convert.ToDouble(rd[i].Mar + rd[1].Mar);
                                            if (SumA3 >= 1000 || SumA3 <= -1000)
                                            {

                                                data.SumMar = string.Format("{0:0,000.00}", (rd[i].Mar + rd[1].Mar), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumMar = Convert.ToString(rd[i].Mar + rd[1].Mar); }
                                        }
                                        else
                                        {
                                            Double SumA3 = Convert.ToDouble(rd[i].Mar);
                                            if (SumA3 >= 1000 || SumA3 <= -1000)
                                            {

                                                data.SumMar = string.Format("{0:0,000.00}", (rd[i].Mar), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumMar = Convert.ToString(rd[i].Mar); }

                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumA4 = Convert.ToDouble(rd[i].Apr + rd[1].Apr);
                                            if (SumA4 >= 1000 || SumA4 <= -1000)
                                            {

                                                data.SumApr = string.Format("{0:0,000.00}", (rd[i].Apr + rd[1].Apr), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumApr = Convert.ToString(rd[i].Apr + rd[1].Apr); }
                                        }
                                        else
                                        {
                                            Double SumA4 = Convert.ToDouble(rd[i].Apr);
                                            if (SumA4 >= 1000 || SumA4 <= -1000)
                                            {

                                                data.SumApr = string.Format("{0:0,000.00}", (rd[i].Apr), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumApr = Convert.ToString(rd[i].Apr); }
                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumA5 = Convert.ToDouble(rd[i].May + rd[1].May);
                                            if (SumA5 >= 1000 || SumA5 <= -1000)
                                            {

                                                data.SumMay = string.Format("{0:0,000.00}", (rd[i].May + rd[1].May), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumMay = Convert.ToString(rd[i].May + rd[1].May); }
                                        }
                                        else
                                        {
                                            Double SumA5 = Convert.ToDouble(rd[i].May);
                                            if (SumA5 >= 1000 || SumA5 <= -1000)
                                            {

                                                data.SumMay = string.Format("{0:0,000.00}", (rd[i].May), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumMay = Convert.ToString(rd[i].May); }
                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumA6 = Convert.ToDouble(rd[i].Jun + rd[1].Jun);
                                            if (SumA6 >= 1000 || SumA6 <= -1000)
                                            {

                                                data.SumJun = string.Format("{0:0,000.00}", (rd[i].Jun + rd[1].Jun), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumJun = Convert.ToString(rd[i].Jun + rd[1].Jun); }
                                        }
                                        else
                                        {
                                            Double SumA6 = Convert.ToDouble(rd[i].Jun);
                                            if (SumA6 >= 1000 || SumA6 <= -1000)
                                            {

                                                data.SumJun = string.Format("{0:0,000.00}", (rd[i].Jun), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumJun = Convert.ToString(rd[i].Jun); }
                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumA7 = Convert.ToDouble(rd[i].Jul + rd[1].Jul);
                                            if (SumA7 >= 1000 || SumA7 <= -1000)
                                            {

                                                data.SumJul = string.Format("{0:0,000.00}", (rd[i].Jul + rd[1].Jul), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumJul = Convert.ToString(rd[0].Jul + rd[1].Jul); }
                                        }
                                        else
                                        {
                                            Double SumA7 = Convert.ToDouble(rd[i].Jul);
                                            if (SumA7 >= 1000 || SumA7 <= -1000)
                                            {

                                                data.SumJul = string.Format("{0:0,000.00}", (rd[i].Jul), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumJul = Convert.ToString(rd[0].Jul); }

                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumA8 = Convert.ToDouble(rd[0].Aug + rd[1].Aug);
                                            if (SumA8 >= 1000 || SumA8 <= -1000)
                                            {

                                                data.SumAug = string.Format("{0:0,000.00}", (rd[0].Aug + rd[1].Aug), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumAug = Convert.ToString(rd[0].Aug + rd[1].Aug); }
                                        }
                                        else
                                        {

                                            Double SumA8 = Convert.ToDouble(rd[0].Aug);
                                            if (SumA8 >= 1000 || SumA8 <= -1000)
                                            {

                                                data.SumAug = string.Format("{0:0,000.00}", (rd[0].Aug), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumAug = Convert.ToString(rd[0].Aug); }
                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumA9 = Convert.ToDouble(rd[0].Sep + rd[1].Sep);
                                            if (SumA9 >= 1000 || SumA9 <= -1000)
                                            {

                                                data.SumSep = string.Format("{0:0,000.00}", (rd[0].Sep + rd[1].Sep), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumSep = Convert.ToString(rd[0].Sep + rd[1].Sep); }
                                        }
                                        else
                                        {
                                            Double SumA9 = Convert.ToDouble(rd[0].Sep);
                                            if (SumA9 >= 1000 || SumA9 <= -1000)
                                            {

                                                data.SumSep = string.Format("{0:0,000.00}", (rd[0].Sep), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumSep = Convert.ToString(rd[0].Sep); }

                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumA10 = Convert.ToDouble(rd[0].Oct + rd[1].Oct);
                                            if (SumA10 >= 1000 || SumA10 <= -1000)
                                            {

                                                data.SumOct = string.Format("{0:0,000.00}", (rd[0].Oct + rd[1].Oct), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumOct = Convert.ToString(rd[0].Oct + rd[1].Oct); }
                                        }
                                        else
                                        {
                                            Double SumA10 = Convert.ToDouble(rd[0].Oct);
                                            if (SumA10 >= 1000 || SumA10 <= -1000)
                                            {

                                                data.SumOct = string.Format("{0:0,000.00}", (rd[0].Oct), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumOct = Convert.ToString(rd[0].Oct); }


                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumA11 = Convert.ToDouble(rd[0].Nov + rd[1].Nov);
                                            if (SumA11 >= 1000 || SumA11 <= -1000)
                                            {

                                                data.SumNov = string.Format("{0:0,000.00}", (rd[0].Nov + rd[1].Nov), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumNov = Convert.ToString(rd[0].Nov + rd[1].Nov); }
                                        }
                                        else
                                        {

                                            Double SumA11 = Convert.ToDouble(rd[0].Nov);
                                            if (SumA11 >= 1000 || SumA11 <= -1000)
                                            {

                                                data.SumNov = string.Format("{0:0,000.00}", (rd[0].Nov), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumNov = Convert.ToString(rd[0].Nov); }
                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumA12 = Convert.ToDouble(rd[0].Dec + rd[1].Dec);
                                            if (SumA12 >= 1000 || SumA12 <= -1000)
                                            {

                                                data.SumDec = string.Format("{0:0,000.00}", (rd[0].Dec + rd[1].Dec), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumDec = Convert.ToString(rd[0].Dec + rd[1].Dec); }
                                        }
                                        else
                                        {
                                            Double SumA12 = Convert.ToDouble(rd[0].Dec);
                                            if (SumA12 >= 1000 || SumA12 <= -1000)
                                            {

                                                data.SumDec = string.Format("{0:0,000.00}", (rd[0].Dec), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumDec = Convert.ToString(rd[0].Dec); }
                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumTotal = Convert.ToDouble(rd[0].Total + rd[1].Total);
                                            if (SumTotal >= 1000 || SumTotal <= -1000)
                                            {

                                                data.SumTotal = string.Format("{0:0,000.00}", (rd[0].Total + rd[1].Total), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumTotal = Convert.ToString(rd[0].Total + rd[1].Total); }

                                        }
                                        else
                                        {
                                            Double SumTotal = Convert.ToDouble(rd[0].Total);
                                            if (SumTotal >= 1000 || SumTotal <= -1000)
                                            {

                                                data.SumTotal = string.Format("{0:0,000.00}", (rd[0].Total), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumTotal = Convert.ToString(rd[0].Total); }

                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumLastTotal = Convert.ToDouble(rd[0].LastTotal + rd[1].LastTotal);
                                            if (SumLastTotal >= 1000 || SumLastTotal <= -1000)
                                            {

                                                data.SumLastTotal = string.Format("{0:0,000.00}", (rd[0].LastTotal + rd[1].LastTotal), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumLastTotal = Convert.ToString(rd[0].LastTotal + rd[1].LastTotal); }
                                        }
                                        else
                                        {
                                            Double SumLastTotal = Convert.ToDouble(rd[0].LastTotal);
                                            if (SumLastTotal >= 1000 || SumLastTotal <= -1000)
                                            {

                                                data.SumLastTotal = string.Format("{0:0,000.00}", (rd[0].LastTotal), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumLastTotal = Convert.ToString(rd[0].LastTotal); }


                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumpayA1 = Convert.ToDouble(rd[0].PayJan + rd[1].PayJan);
                                            if (SumpayA1 >= 1000 || SumpayA1 <= -1000)
                                            {
                                                data.SumpayJan = string.Format("{0:0,000.00}", (rd[0].PayJan + rd[1].PayJan), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayJan = Convert.ToString(rd[0].PayJan + rd[1].PayJan); }
                                        }
                                        else
                                        {

                                            Double SumpayA1 = Convert.ToDouble(rd[0].PayJan);
                                            if (SumpayA1 >= 1000 || SumpayA1 <= -1000)
                                            {
                                                data.SumpayJan = string.Format("{0:0,000.00}", (rd[0].PayJan), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayJan = Convert.ToString(rd[0].PayJan); }


                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumpayA2 = Convert.ToDouble(rd[0].PayFeb + rd[1].PayFeb);
                                            if (SumpayA2 >= 1000 || SumpayA2 <= -1000)
                                            {
                                                data.SumpayFeb = string.Format("{0:0,000.00}", (rd[0].PayFeb + rd[1].PayFeb), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayFeb = Convert.ToString(rd[0].PayFeb + rd[1].PayFeb); }
                                        }
                                        else
                                        {
                                            Double SumpayA2 = Convert.ToDouble(rd[0].PayFeb);
                                            if (SumpayA2 >= 1000 || SumpayA2 <= -1000)
                                            {
                                                data.SumpayFeb = string.Format("{0:0,000.00}", (rd[0].PayFeb), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayFeb = Convert.ToString(rd[0].PayFeb); }

                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumpayA3 = Convert.ToDouble(rd[0].PayMar + rd[1].PayMar);
                                            if (SumpayA3 >= 1000 || SumpayA3 <= -1000)
                                            {
                                                data.SumpayMar = string.Format("{0:0,000.00}", (rd[0].PayMar + rd[1].PayMar), CultureInfo.InvariantCulture);
                                            }
                                            else
                                            { //data.SumpayMar = Convert.ToString(rd[0].PayMar + rd[1].PayMar); 
                                                data.SumpayMar = Convert.ToString(rd[0].PayMar + rd[1].PayMar);
                                            }
                                        }
                                        else
                                        {
                                            Double SumpayA3 = Convert.ToDouble(rd[0].PayMar);
                                            if (SumpayA3 >= 1000 || SumpayA3 <= -1000)
                                            {
                                                data.SumpayMar = string.Format("{0:0,000.00}", (rd[0].PayMar), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayMar = Convert.ToString(rd[0].PayMar); }

                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumpayA4 = Convert.ToDouble(rd[0].PayApr + rd[1].PayApr);
                                            if (SumpayA4 >= 1000 || SumpayA4 <= -1000)
                                            {
                                                data.SumpayApr = string.Format("{0:0,000.00}", (rd[0].PayApr + rd[1].PayApr), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayApr = Convert.ToString(rd[0].PayApr + rd[1].PayApr); }
                                        }
                                        else
                                        {
                                            Double SumpayA4 = Convert.ToDouble(rd[0].PayApr);
                                            if (SumpayA4 >= 1000 || SumpayA4 <= -1000)
                                            {
                                                data.SumpayApr = string.Format("{0:0,000.00}", (rd[0].PayApr), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayApr = Convert.ToString(rd[0].PayApr); }
                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumpayA5 = Convert.ToDouble(rd[0].PayMay + rd[1].PayMay);
                                            if (SumpayA5 >= 1000 || SumpayA5 <= -1000)
                                            {
                                                data.SumpayMay = string.Format("{0:0,000.00}", (rd[0].PayMay + rd[1].PayMay), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayMay = Convert.ToString(rd[0].PayMay + rd[1].PayMay); }
                                        }
                                        else
                                        {
                                            Double SumpayA5 = Convert.ToDouble(rd[0].PayMay);
                                            if (SumpayA5 >= 1000 || SumpayA5 <= -1000)
                                            {
                                                data.SumpayMay = string.Format("{0:0,000.00}", (rd[0].PayMay), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayMay = Convert.ToString(rd[0].PayMay); }
                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumpayA6 = Convert.ToDouble(rd[0].PayJun + rd[1].PayJun);
                                            if (SumpayA6 >= 1000 || SumpayA6 <= -1000)
                                            {
                                                data.SumpayJun = string.Format("{0:0,000.00}", (rd[0].PayJun + rd[1].PayJun), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayJun = Convert.ToString(rd[0].PayJun + rd[1].PayJun); }
                                        }
                                        else
                                        {
                                            Double SumpayA6 = Convert.ToDouble(rd[0].PayJun);
                                            if (SumpayA6 >= 1000 || SumpayA6 <= -1000)
                                            {
                                                data.SumpayJun = string.Format("{0:0,000.00}", (rd[0].PayJun), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayJun = Convert.ToString(rd[0].PayJun); }

                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumpayA7 = Convert.ToDouble(rd[0].PayJul + rd[1].PayJul);
                                            if (SumpayA7 >= 1000 || SumpayA7 <= -1000)
                                            {
                                                data.SumpayJul = string.Format("{0:0,000.00}", (rd[0].PayJul + rd[1].PayJul), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayJul = Convert.ToString(rd[0].PayJul + rd[1].PayJul); }
                                        }
                                        else
                                        {
                                            Double SumpayA7 = Convert.ToDouble(rd[0].PayJul);
                                            if (SumpayA7 >= 1000 || SumpayA7 <= -1000)
                                            {
                                                data.SumpayJul = string.Format("{0:0,000.00}", (rd[0].PayJul), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayJul = Convert.ToString(rd[0].PayJul); }

                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumpayA8 = Convert.ToDouble(rd[0].PayAug + rd[1].PayAug);
                                            if (SumpayA8 >= 1000 || SumpayA8 <= -1000)
                                            {
                                                data.SumpayAug = string.Format("{0:0,000.00}", (rd[0].PayAug + rd[1].PayAug), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayAug = Convert.ToString(rd[0].PayAug + rd[1].PayAug); }
                                        }
                                        else
                                        {
                                            Double SumpayA8 = Convert.ToDouble(rd[0].PayAug);
                                            if (SumpayA8 >= 1000 || SumpayA8 <= -1000)
                                            {
                                                data.SumpayAug = string.Format("{0:0,000.00}", (rd[0].PayAug), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayAug = Convert.ToString(rd[0].PayAug); }
                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumpayA9 = Convert.ToDouble(rd[0].PaySep + rd[1].PaySep);
                                            if (SumpayA9 >= 1000 || SumpayA9 <= -1000)
                                            {
                                                data.SumpaySep = string.Format("{0:0,000.00}", (rd[0].PaySep + rd[1].PaySep), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpaySep = Convert.ToString(rd[0].PaySep + rd[1].PaySep); }
                                        }
                                        else
                                        {
                                            Double SumpayA9 = Convert.ToDouble(rd[0].PaySep);
                                            if (SumpayA9 >= 1000 || SumpayA9 <= -1000)
                                            {
                                                data.SumpaySep = string.Format("{0:0,000.00}", (rd[0].PaySep));
                                            }
                                            else { data.SumpaySep = Convert.ToString(rd[0].PaySep); }
                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumpayA10 = Convert.ToDouble(rd[0].PayOct + rd[1].PayOct);
                                            if (SumpayA10 >= 1000 || SumpayA10 <= -1000)
                                            {
                                                data.SumpayOct = string.Format("{0:0,000.00}", (rd[0].PayOct + rd[1].PayOct), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayOct = Convert.ToString(rd[0].PayOct + rd[1].PayOct); }
                                        }
                                        else
                                        {
                                            Double SumpayA10 = Convert.ToDouble(rd[0].PayOct);
                                            if (SumpayA10 >= 1000 || SumpayA10 <= -1000)
                                            {
                                                data.SumpayOct = string.Format("{0:0,000.00}", (rd[0].PayOct), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayOct = Convert.ToString(rd[0].PayOct); }

                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumpayA11 = Convert.ToDouble(rd[0].PayNov + rd[1].PayNov);
                                            if (SumpayA11 >= 1000 || SumpayA11 <= -1000)
                                            {
                                                data.SumpayNov = string.Format("{0:0,000.00}", (rd[0].PayNov + rd[1].PayNov), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayNov = Convert.ToString(rd[0].PayNov + rd[1].PayNov); }
                                        }
                                        else
                                        {
                                            Double SumpayA11 = Convert.ToDouble(rd[0].PayNov);
                                            if (SumpayA11 >= 1000 || SumpayA11 <= -1000)
                                            {
                                                data.SumpayNov = string.Format("{0:0,000.00}", (rd[0].PayNov), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayNov = Convert.ToString(rd[0].PayNov); }
                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumpayA12 = Convert.ToDouble(rd[0].PayDec + rd[1].PayDec);
                                            if (SumpayA12 >= 1000 || SumpayA12 <= -1000)
                                            {
                                                data.SumpayDec = string.Format("{0:0,000.00}", (rd[0].PayDec + rd[1].PayDec), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayDec = Convert.ToString(rd[0].PayDec + rd[1].PayDec); }
                                        }
                                        else
                                        {
                                            Double SumpayA12 = Convert.ToDouble(rd[0].PayDec);
                                            if (SumpayA12 >= 1000 || SumpayA12 <= -1000)
                                            {

                                                data.SumpayDec = string.Format("{0:0,000.00}", (rd[0].PayDec), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayDec = Convert.ToString(rd[0].PayDec); }


                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumpayTotal = Convert.ToDouble(rd[0].PayTotal + rd[1].PayTotal);
                                            if (SumpayTotal >= 1000 || SumpayTotal <= -1000)
                                            {
                                                data.SumpayTotal = string.Format("{0:0,000.00}", (rd[0].PayTotal + rd[1].PayTotal), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayTotal = Convert.ToString(rd[0].PayTotal + rd[1].PayTotal); }
                                        }
                                        else
                                        {

                                            Double SumpayTotal = Convert.ToDouble(rd[0].PayTotal);
                                            if (SumpayTotal >= 1000 || SumpayTotal <= -1000)
                                            {
                                                data.SumpayTotal = string.Format("{0:0,000.00}", (rd[0].PayTotal), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumpayTotal = Convert.ToString(rd[0].PayTotal); }

                                        }
                                        if (rd.Length > 1)
                                        {
                                            Double SumLastpayTotal = Convert.ToDouble(rd[0].LastPayTotal + rd[1].LastPayTotal);
                                            if (SumLastpayTotal >= 1000 || SumLastpayTotal <= -1000)
                                            {
                                                data.SumLastpayTotal = string.Format("{0:0,000.00}", (rd[0].LastPayTotal + rd[1].LastPayTotal), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumLastpayTotal = Convert.ToString(rd[0].LastPayTotal + rd[1].LastPayTotal); }
                                        }
                                        else
                                        {
                                            Double SumLastpayTotal = Convert.ToDouble(rd[0].LastPayTotal);
                                            if (SumLastpayTotal >= 1000 || SumLastpayTotal <= -1000)
                                            {
                                                data.SumLastpayTotal = string.Format("{0:0,000.00}", (rd[0].LastPayTotal), CultureInfo.InvariantCulture);
                                            }
                                            else { data.SumLastpayTotal = Convert.ToString(rd[0].LastPayTotal); }

                                        }

                                        GetdatatoDisplay.Add(new Listdata { val = data });

                                    }
                                    //}
                                    reCUSOCD.Dispose();
                                }
                                GetdataCus.Add(new ListCus { val = dataCus });
                            }

                        }

                    }
                }


                Connection.Close();
                ViewBag.PromoStatementReward = GetdataReward;
                ViewBag.Customer = GetdataCus;
                ViewBag.GetLocation = GetdatatoDisplay;



                string yeardt = DateTime.Now.ToString("yy");
                ViewBag.year = yeardt;

                string dt = DateTime.Now.ToString("dd/MMM/yyyy");

                ViewBag.datetime = dt;
                return View();
            }
        }
        public ActionResult SomeImage(string imageName)
        {
            // var root = @"H:\IT\Public\Promotion_Image\";
            var root = @"\\tac01srv-sha01\AACTAC\IT\Public\Promotion_Image\";
            var path = Path.Combine(root, "advertise.png");
            path = Path.GetFullPath(path);
            if (!path.StartsWith(root))
            {
                // Ensure that we are serving file only inside the root folder
                // and block requests outside like "../web.config"
                throw new HttpException(403, "Forbidden");
            }

            return File(path, "image/png");
        }


    }
    public class Item
    {

        public string COMP { get; set; }
        public string CUSKEY { get; set; }
        public string LastTotal { get; set; }
        public string LastPayTotal { get; set; }
        public string Jan { get; set; }
        public string Feb { get; set; }
        public string Mar { get; set; }
        public string Apr { get; set; }
        public string May { get; set; }
        public string Jun { get; set; }
        public string Jul { get; set; }
        public string Aug { get; set; }
        public string Sep { get; set; }
        public string Oct { get; set; }
        public string Nov { get; set; }
        public string Dec { get; set; }
        public string PayTotal { get; set; }
        public string PayJan { get; set; }
        public string PayFeb { get; set; }
        public string PayMar { get; set; }
        public string PayApr { get; set; }
        public string PayMay { get; set; }
        public string PayJun { get; set; }
        public string PayJul { get; set; }
        public string PayAug { get; set; }
        public string PaySep { get; set; }
        public string PayOct { get; set; }
        public string PayNov { get; set; }
        public string PayDec { get; set; }
        public string SumJan { get; set; }
        public string SumFeb { get; set; }
        public string SumMar { get; set; }
        public string SumApr { get; set; }
        public string SumMay { get; set; }
        public string SumJun { get; set; }
        public string SumJul { get; set; }
        public string SumAug { get; set; }
        public string SumSep { get; set; }
        public string SumOct { get; set; }
        public string SumNov { get; set; }
        public string SumDec { get; set; }
        public string SumpayJan { get; set; }
        public string SumpayFeb { get; set; }
        public string SumpayMar { get; set; }
        public string SumpayApr { get; set; }
        public string SumpayMay { get; set; }
        public string SumpayJun { get; set; }
        public string SumpayJul { get; set; }
        public string SumpayAug { get; set; }
        public string SumpaySep { get; set; }
        public string SumpayOct { get; set; }
        public string SumpayNov { get; set; }
        public string SumpayDec { get; set; }


        public string SumLastTotal { get; set; }
        public string SumLastpayTotal { get; set; }
        public string StYr { get; set; }
        public string SumTotal { get; set; }
        public string SumpayTotal { get; set; }
        public string Total { get; set; }
        public string LastStYr { get; set; }



        public string Promotion_Code { get; set; }
        public string Promotion_Name { get; set; }
        public string SAMT { get; set; }
        public string SPaidAMT { get; set; }
        public string Condition { get; set; }
        public string Reward { get; set; }
        public string Sum { get; set; }


        public string CUSCOD { get; set; }
        public string CUSNAM { get; set; }
        public string ADDR_01 { get; set; }
        public string TELNUM { get; set; }

    }

    public class Listdata
    {
        public Item val { get; set; }
    }

    public class Cus
    {

        public string CUSCOD { get; set; }
        public string CUSNAM { get; set; }
        public string ADDR_01 { get; set; }
        public string TELNUM { get; set; }
        public string SLMNAM { get; set; }
        public string SLMCOD { get; set; }
        public string Asof { get; set; }

    }

    public class ListCus
    {
        public Cus val { get; set; }
    }

    public class ItemReward
    {
        public string CUSCOD { get; set; }
        public string Promotion_Code { get; set; }
        public string Promotion_Name { get; set; }
        public string SAMT { get; set; }
        public string SPaidAMT { get; set; }
        public string Condition { get; set; }
        public string Reward { get; set; }
        public string Sum { get; set; }
        public string Startdate { get; set; }
        public string Enddate { get; set; }
        public string CUSCODReward { get; set; }
        public string CUSNAM { get; set; }
        public string ADDR_01 { get; set; }
        public string TELNUM { get; set; }
    }

    public class ListdataReward
    {
        public ItemReward val { get; set; }
    }

    public class GenItem
    {
        public string ItemID { get; set; }
    }
}
