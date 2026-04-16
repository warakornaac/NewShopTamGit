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
    public class ManageUserController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;

        // GET: /ManageUser/
        public ActionResult Index()
        {
            List<UsrTbl> userList = new List<UsrTbl>();

            try
            {
                SqlConnection Connection = new SqlConnection(connectionString);
                using (Connection)
                {
                    string query = @"SELECT [No], [EmpID], [company], [UsrID], [initials], 
                                           [Department], [EMail], [SLMCOD], [UsrTyp], [PASSWORD], 
                                           [UsrName], [Inserted By], [Inserted Date], [Updated By], 
                                           [Updated Date], [SALES_CO]
                                    FROM [MobileOrder_TAM].[dbo].[UsrTbl]
                                    ORDER BY [No] DESC";

                    using (SqlCommand cmd = new SqlCommand(query, Connection))
                    {
                        Connection.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UsrTbl user = new UsrTbl
                                {
                                    No = reader.GetInt32(reader.GetOrdinal("No")),
                                    EmpID = reader.IsDBNull(reader.GetOrdinal("EmpID")) ? null : reader.GetString(reader.GetOrdinal("EmpID")),
                                    company = reader.IsDBNull(reader.GetOrdinal("company")) ? null : reader.GetString(reader.GetOrdinal("company")),
                                    UsrID = reader.IsDBNull(reader.GetOrdinal("UsrID")) ? null : reader.GetString(reader.GetOrdinal("UsrID")),
                                    initials = reader.IsDBNull(reader.GetOrdinal("initials")) ? null : reader.GetString(reader.GetOrdinal("initials")),
                                    Department = reader.IsDBNull(reader.GetOrdinal("Department")) ? null : reader.GetString(reader.GetOrdinal("Department")),
                                    EMail = reader.IsDBNull(reader.GetOrdinal("EMail")) ? null : reader.GetString(reader.GetOrdinal("EMail")),
                                    SLMCOD = reader.IsDBNull(reader.GetOrdinal("SLMCOD")) ? null : reader.GetString(reader.GetOrdinal("SLMCOD")),
                                    UsrTyp = reader.IsDBNull(reader.GetOrdinal("UsrTyp")) ? 0 : reader.GetInt32(reader.GetOrdinal("UsrTyp")),
                                    PASSWORD = reader.IsDBNull(reader.GetOrdinal("PASSWORD")) ? null : reader.GetString(reader.GetOrdinal("PASSWORD")),
                                    UsrName = reader.IsDBNull(reader.GetOrdinal("UsrName")) ? null : reader.GetString(reader.GetOrdinal("UsrName")),
                                    InsertedBy = reader.IsDBNull(reader.GetOrdinal("Inserted By")) ? null : reader.GetString(reader.GetOrdinal("Inserted By")),
                                    InsertedDate = reader.IsDBNull(reader.GetOrdinal("Inserted Date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("Inserted Date")),
                                    UpdatedBy = reader.IsDBNull(reader.GetOrdinal("Updated By")) ? null : reader.GetString(reader.GetOrdinal("Updated By")),
                                    UpdatedDate = reader.IsDBNull(reader.GetOrdinal("Updated Date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("Updated Date")),
                                    SALES_CO = reader.IsDBNull(reader.GetOrdinal("SALES_CO")) ? null : reader.GetString(reader.GetOrdinal("SALES_CO"))
                                };

                                userList.Add(user);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Something went wrong: " + ex.Message;
            }

            return View(userList);
        }

        // GET: GetUserList for DataTables/jsGrid
        public JsonResult GetUserList()
        {
            List<UsrTbl> userList = new List<UsrTbl>();

            try
            {
                SqlConnection Connection = new SqlConnection(connectionString);
                using (Connection)
                {
                    string query = @"SELECT [No], [EmpID], [company], [UsrID], [initials], 
                                           [Department], [EMail], [SLMCOD], [UsrTyp], 
                                           [UsrName], [Inserted By], [Inserted Date], [Updated By], 
                                           [Updated Date], [SALES_CO]
                                    FROM [MobileOrder_TAM].[dbo].[UsrTbl]
                                    ORDER BY [No] DESC";

                    using (SqlCommand cmd = new SqlCommand(query, Connection))
                    {
                        Connection.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UsrTbl user = new UsrTbl
                                {
                                    No = reader.GetInt32(reader.GetOrdinal("No")),
                                    EmpID = reader.IsDBNull(reader.GetOrdinal("EmpID")) ? null : reader.GetString(reader.GetOrdinal("EmpID")),
                                    company = reader.IsDBNull(reader.GetOrdinal("company")) ? null : reader.GetString(reader.GetOrdinal("company")),
                                    UsrID = reader.IsDBNull(reader.GetOrdinal("UsrID")) ? null : reader.GetString(reader.GetOrdinal("UsrID")),
                                    initials = reader.IsDBNull(reader.GetOrdinal("initials")) ? null : reader.GetString(reader.GetOrdinal("initials")),
                                    Department = reader.IsDBNull(reader.GetOrdinal("Department")) ? null : reader.GetString(reader.GetOrdinal("Department")),
                                    EMail = reader.IsDBNull(reader.GetOrdinal("EMail")) ? null : reader.GetString(reader.GetOrdinal("EMail")),
                                    SLMCOD = reader.IsDBNull(reader.GetOrdinal("SLMCOD")) ? null : reader.GetString(reader.GetOrdinal("SLMCOD")),
                                    UsrTyp = reader.IsDBNull(reader.GetOrdinal("UsrTyp")) ? 0 : reader.GetInt32(reader.GetOrdinal("UsrTyp")),
                                    UsrName = reader.IsDBNull(reader.GetOrdinal("UsrName")) ? null : reader.GetString(reader.GetOrdinal("UsrName")),
                                    InsertedBy = reader.IsDBNull(reader.GetOrdinal("Inserted By")) ? null : reader.GetString(reader.GetOrdinal("Inserted By")),
                                    InsertedDate = reader.IsDBNull(reader.GetOrdinal("Inserted Date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("Inserted Date")),
                                    UpdatedBy = reader.IsDBNull(reader.GetOrdinal("Updated By")) ? null : reader.GetString(reader.GetOrdinal("Updated By")),
                                    UpdatedDate = reader.IsDBNull(reader.GetOrdinal("Updated Date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("Updated Date")),
                                    SALES_CO = reader.IsDBNull(reader.GetOrdinal("SALES_CO")) ? null : reader.GetString(reader.GetOrdinal("SALES_CO"))
                                };

                                userList.Add(user);
                            }
                        }
                    }
                }

                return Json(new { success = true, data = userList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Create(UsrTbl user)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("P_AddNewUserAD", Connection))
                    {
                        // กำหนดประเภทคำสั่งให้เป็น Stored Procedure
                        cmd.CommandType = CommandType.StoredProcedure;

                        // กำหนด Input Parameters ที่ SP ต้องการ
                        cmd.Parameters.AddWithValue("@inUser", (object)user.UsrID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@instatus", (object)user.UsrTyp ?? DBNull.Value);

                        // กำหนด Output Parameter สำหรับรับค่ากลับจาก SP
                        SqlParameter outGenStatusParam = new SqlParameter("@outGenstatus", SqlDbType.NVarChar, 100)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outGenStatusParam);

                        Connection.Open();
                        cmd.ExecuteNonQuery();

                        // ดึงค่าที่ได้จาก Output Parameter
                        string statusMessage = outGenStatusParam.Value != DBNull.Value
                        ? outGenStatusParam.Value.ToString()
                        : "";

                        // ตรวจสอบผลลัพธ์ด้วยคำว่า "Success" แทน
                        if (statusMessage == "Success")
                        {
                            return Json(new { success = true, message = "Added successfully." });
                        }
                        else
                        {
                            // กรณีนี้จะครอบคลุมทั้ง 'Already exists', 'Not UserAD' หรือ Error Message จาก Catch
                            return Json(new { success = false, message = statusMessage });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // GET: Get user by ID
        [HttpGet]
        public JsonResult GetUserById(int id)
        {
            try
            {
                SqlConnection Connection = new SqlConnection(connectionString);
                using (Connection)
                {
                    string query = @"SELECT [No], [EmpID], [company], [UsrID], [initials], 
                                           [Department], [EMail], [SLMCOD], [UsrTyp], [PASSWORD], 
                                           [UsrName], [SALES_CO]
                                    FROM [MobileOrder_TAM].[dbo].[UsrTbl]
                                    WHERE [No] = @No";

                    using (SqlCommand cmd = new SqlCommand(query, Connection))
                    {
                        cmd.Parameters.AddWithValue("@No", id);
                        Connection.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                UsrTbl user = new UsrTbl
                                {
                                    No = reader.GetInt32(reader.GetOrdinal("No")),
                                    EmpID = reader.IsDBNull(reader.GetOrdinal("EmpID")) ? null : reader.GetString(reader.GetOrdinal("EmpID")),
                                    company = reader.IsDBNull(reader.GetOrdinal("company")) ? null : reader.GetString(reader.GetOrdinal("company")),
                                    UsrID = reader.IsDBNull(reader.GetOrdinal("UsrID")) ? null : reader.GetString(reader.GetOrdinal("UsrID")),
                                    initials = reader.IsDBNull(reader.GetOrdinal("initials")) ? null : reader.GetString(reader.GetOrdinal("initials")),
                                    Department = reader.IsDBNull(reader.GetOrdinal("Department")) ? null : reader.GetString(reader.GetOrdinal("Department")),
                                    EMail = reader.IsDBNull(reader.GetOrdinal("EMail")) ? null : reader.GetString(reader.GetOrdinal("EMail")),
                                    SLMCOD = reader.IsDBNull(reader.GetOrdinal("SLMCOD")) ? null : reader.GetString(reader.GetOrdinal("SLMCOD")),
                                    UsrTyp = reader.IsDBNull(reader.GetOrdinal("UsrTyp")) ? 0 : reader.GetInt32(reader.GetOrdinal("UsrTyp")),
                                    PASSWORD = reader.IsDBNull(reader.GetOrdinal("PASSWORD")) ? null : reader.GetString(reader.GetOrdinal("PASSWORD")),
                                    UsrName = reader.IsDBNull(reader.GetOrdinal("UsrName")) ? null : reader.GetString(reader.GetOrdinal("UsrName")),
                                    SALES_CO = reader.IsDBNull(reader.GetOrdinal("SALES_CO")) ? null : reader.GetString(reader.GetOrdinal("SALES_CO"))
                                };

                                return Json(new { success = true, data = user }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new { success = false, message = "Not found" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Update user
        [HttpPost]
        public JsonResult Update(UsrTbl user)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("P_UpdateUserAD", Connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // ส่ง UsrID เป็นตัวอ้างอิง และ UsrTyp เป็นค่าที่จะเปลี่ยน
                        cmd.Parameters.AddWithValue("@inUser", user.UsrID);
                        cmd.Parameters.AddWithValue("@instatus", user.UsrTyp);
                        //cmd.Parameters.AddWithValue("@EmpID", user.EmpID);
                        //cmd.Parameters.AddWithValue("@company", user.company);
                        //cmd.Parameters.AddWithValue("@Department", user.Department);
                        //cmd.Parameters.AddWithValue("@EMail", user.EMail);
                        //cmd.Parameters.AddWithValue("@UsrTyp", user.UsrTyp);
                        cmd.Parameters.AddWithValue("@SLMCOD", (object)user.SLMCOD ?? DBNull.Value);

                        SqlParameter outParam = new SqlParameter("@outGenstatus", SqlDbType.NVarChar, 100)
                        { Direction = ParameterDirection.Output };
                        cmd.Parameters.Add(outParam);

                        Connection.Open();
                        cmd.ExecuteNonQuery();

                        string result = outParam.Value.ToString();
                        if (result == "Update Success")
                        {
                            return Json(new { success = true, message = "Updated successfully." });
                        }
                        return Json(new { success = false, message = result });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Delete user
        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                SqlConnection Connection = new SqlConnection(connectionString);
                using (Connection)
                {
                    string query = @"DELETE FROM [MobileOrder_TAM].[dbo].[UsrTbl] WHERE [No] = @No";
                    using (SqlCommand cmd = new SqlCommand(query, Connection))
                    {
                        cmd.Parameters.AddWithValue("@No", id);
                        Connection.Open();
                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            return Json(new { success = true, message = "Deleted successfully." });
                        }
                        else
                        {
                            return Json(new { success = false, message = "Deleted failed." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }
    }
}