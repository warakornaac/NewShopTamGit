using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
namespace NewShopTAM.Controllers
{
    public class ProductimageController : Controller
    {
        public ActionResult Index()
        {
           
            return View();
        }
        [HttpPost]
        public ActionResult Indexnew(FormCollection formCollection)
        {
            foreach (string item in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[item] as HttpPostedFileBase;
                if (file.ContentLength == 0)
                    continue;
                if (file.ContentLength > 0)
                {
                    string name = formCollection["uploadername"];
                    string incompany = formCollection["incompany"];
                    string instk = formCollection["instk"];
                    // width + height will force size, care for distortion
                    //Exmaple: ImageUpload imageUpload = new ImageUpload { Width = 800, Height = 700 };

                    // height will increase the width proportionally
                    //Example: ImageUpload imageUpload = new ImageUpload { Height= 600 };

                    // width will increase the height proportionally
                    //ImageUpload imageUpload = new ImageUpload { Width = 600 };
                    ImageUpload imageUpload = new ImageUpload { Width = 350 };
                    ImageUploadB imageUpload_B = new ImageUploadB { Width = 600 };
                    ImageUploadC imageUpload_C = new ImageUploadC { Width = 900 };
                    // rename, resize, and upload
                    //return object that contains {bool Success,string ErrorMessage,string ImageName}
                    ImageResult imageResult = imageUpload.RenameUploadFile(file, name, incompany, instk); // Width = 350
                    if (imageResult.Success)
                    {
                        //TODO: write the filename to the db
                        Console.WriteLine(imageResult.ImageName);
                    }
                    else
                    {
                        // use imageResult.ErrorMessage to show the error
                        ViewBag.Error = imageResult.ErrorMessage;
                    }

                    ImageResult imageUploadB = imageUpload_B.RenameUploadFileB(file, name, incompany, instk); //Width = 600
                    if (imageUploadB.Success)
                    {
                        //TODO: write the filename to the db
                        Console.WriteLine(imageUploadB.ImageName);
                    }
                    else
                    {
                        // use imageResult.ErrorMessage to show the error
                        ViewBag.Error = imageResult.ErrorMessage;
                    }
                    ImageResult imageUploadC = imageUpload_C.RenameUploadFileC(file, name, incompany, instk); //Width = 900
                    if (imageUploadC.Success)
                    {
                        //TODO: write the filename to the db
                        Console.WriteLine(imageUploadC.ImageName);
                    }
                    else
                    {
                        // use imageResult.ErrorMessage to show the error
                        ViewBag.Error = imageResult.ErrorMessage;
                    }
                }
            }

            return View();
        }
        [HttpPost]
        public ActionResult Index(FormCollection formCollection)
        {
            foreach (string item in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[item] as HttpPostedFileBase;
                if (file.ContentLength == 0)
                    continue;
                if (file.ContentLength > 0)
                {
                    string name = formCollection["uploadername"];
                    string incompany = formCollection["incompany"];
                    string instk = formCollection["instk"];
                  
                    ImageUpload imageUpload = new ImageUpload { Width = 350 };
                   
                    ImageResult imageResult = imageUpload.RenameUploadFile(file, name, incompany, instk); // Width = 350
                    if (imageResult.Success)
                    {
                        //TODO: write the filename to the db
                        Console.WriteLine(imageResult.ImageName);
                    }
                    else
                    {
                        // use imageResult.ErrorMessage to show the error
                        ViewBag.Error = imageResult.ErrorMessage;
                    }

                }
            }

            return View();
        }
       
        public class ImageUpload
        {
            // set default size here
            public int Width { get; set; }

            public int Height { get; set; }

            // folder for the upload, you can put this in the web.config
            private readonly string UploadPath = "~/IMAGE_A/";
            //private readonly string UploadPath_B = "~/IMAGE_B/";
            //private readonly string UploadPath_C = "~/IMAGE_C/";
            public ImageResult RenameUploadFile(HttpPostedFileBase file,string name,string incompany,string instk, Int32 counter = 0)
            {
                var fileName = Path.GetFileName(file.FileName);
               
                string prepend = "item_";
               
                //string name = formCollection["uploadername"];
                //string incompany = formCollection["incompany"];
                //string instk = formCollection["instk"];
               // string finalFileName = prepend + ((counter).ToString()) + "_" + fileName;
                string finalFileName = incompany + "_" + instk +".png";
               

                if (System.IO.File.Exists(System.Web.HttpContext.Current.Request.MapPath(UploadPath + finalFileName)))
                    {
                        //file exists => add country try again
                        //return RenameUploadFile(file, name, incompany, instk, ++counter);
                        return UploadFile(file, finalFileName);
                    }
                
                    //file doesn't exist, upload item but validate first
                    return UploadFile(file, finalFileName);
                
            }

            private ImageResult UploadFile(HttpPostedFileBase file, string fileName)
            {
                ImageResult imageResult = new ImageResult { Success = true, ErrorMessage = null };
                //string UploadPath = string.Empty;
                
                    var path =
                  Path.Combine(System.Web.HttpContext.Current.Request.MapPath(UploadPath), fileName);
                    string extension = Path.GetExtension(file.FileName);
                
               
                //make sure the file is valid
                if (!ValidateExtension(extension))
                {
                    imageResult.Success = false;
                    imageResult.ErrorMessage = "Invalid Extension";
                    return imageResult;
                }

                try
                {

                    file.SaveAs(path);
                    

                    Image imgOriginal = Image.FromFile(path);
                   
                    //pass in whatever value you want
                    Image imgActual = Scale(imgOriginal);
                    imgOriginal.Dispose();
                    imgActual.Save(path);
                    imgActual.Dispose();

                    imageResult.ImageName = fileName;

                    return imageResult;
                }
                catch (Exception ex)
                {
                    // you might NOT want to show the exception error for the user
                    // this is generally logging or testing

                    imageResult.Success = false;
                    imageResult.ErrorMessage = ex.Message;
                    return imageResult;
                }
            }

            private bool ValidateExtension(string extension)
            {
                extension = extension.ToLower();
                switch (extension)
                {
                    case ".jpg":
                        return true;
                    case ".png":
                        return true;
                    case ".gif":
                        return true;
                    case ".jpeg":
                        return true;
                    default:
                        return false;
                }
            }

            private Image Scale(Image imgPhoto)
            {
                float sourceWidth = imgPhoto.Width;
                float sourceHeight = imgPhoto.Height;
                float destHeight = 0;
                float destWidth = 0;
                int sourceX = 0;
                int sourceY = 0;
                int destX = 0;
                int destY = 0;

                // force resize, might distort image
                if (Width != 0 && Height != 0)
                {
                    destWidth = Width;
                    destHeight = Height;
                }
                // change size proportially depending on width or height
                else if (Height != 0)
                {
                    destWidth = (float)(Height * sourceWidth) / sourceHeight;
                    destHeight = Height;
                }
                else
                {
                    destWidth = Width;
                    destHeight = (float)(sourceHeight * Width / sourceWidth);
                }

                Bitmap bmPhoto = new Bitmap((int)destWidth, (int)destHeight,
                                            PixelFormat.Format32bppPArgb);
                bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

                Graphics grPhoto = Graphics.FromImage(bmPhoto);
                grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

                grPhoto.DrawImage(imgPhoto,
                    new Rectangle(destX, destY, (int)destWidth, (int)destHeight),
                    new Rectangle(sourceX, sourceY, (int)sourceWidth, (int)sourceHeight),
                    GraphicsUnit.Pixel);

                grPhoto.Dispose();

                return bmPhoto;
            }
        }
        public class ImageUploadB
        {
            // set default size here
            public int Width { get; set; }

            public int Height { get; set; }

            // folder for the upload, you can put this in the web.config
           
            private readonly string UploadPath_B = "~/IMAGE_B/";

            public ImageResult RenameUploadFileB(HttpPostedFileBase file, string name, string incompany, string instk, Int32 counter = 0)
            {
                var fileName = Path.GetFileName(file.FileName);

                string prepend = "item_";

                //string finalFileName = prepend + ((counter).ToString()) + "_" + fileName;
                string finalFileName = incompany + "_" + instk + ".png";


                if (System.IO.File.Exists(System.Web.HttpContext.Current.Request.MapPath(UploadPath_B + finalFileName)))
                {
                    //file exists => add country try again
                  // return RenameUploadFileB(file, name, incompany, instk, ++counter);
                    return UploadFileB(file, finalFileName);
                }

                //file doesn't exist, upload item but validate first
                return UploadFileB(file, finalFileName);

            }

            private ImageResult UploadFileB(HttpPostedFileBase file, string fileName)
            {
                ImageResult imageResult = new ImageResult { Success = true, ErrorMessage = null };
                string UploadPath = string.Empty;

                var path =
              Path.Combine(System.Web.HttpContext.Current.Request.MapPath(UploadPath_B), fileName);
                string extension = Path.GetExtension(file.FileName);


                //make sure the file is valid
                if (!ValidateExtension(extension))
                {
                    imageResult.Success = false;
                    imageResult.ErrorMessage = "Invalid Extension";
                    return imageResult;
                }

                try
                {
                    file.SaveAs(path);


                    Image imgOriginal = Image.FromFile(path);

                    //pass in whatever value you want
                    Image imgActual = Scale(imgOriginal);
                    imgOriginal.Dispose();
                    imgActual.Save(path);
                    imgActual.Dispose();

                    imageResult.ImageName = fileName;

                    return imageResult;
                }
                catch (Exception ex)
                {
                    // you might NOT want to show the exception error for the user
                    // this is generally logging or testing

                    imageResult.Success = false;
                    imageResult.ErrorMessage = ex.Message;
                    return imageResult;
                }
            }

            private bool ValidateExtension(string extension)
            {
                extension = extension.ToLower();
                switch (extension)
                {
                    case ".jpg":
                        return true;
                    case ".png":
                        return true;
                    case ".gif":
                        return true;
                    case ".jpeg":
                        return true;
                    default:
                        return false;
                }
            }

            private Image Scale(Image imgPhoto)
            {
                float sourceWidth = imgPhoto.Width;
                float sourceHeight = imgPhoto.Height;
                float destHeight = 0;
                float destWidth = 0;
                int sourceX = 0;
                int sourceY = 0;
                int destX = 0;
                int destY = 0;

                // force resize, might distort image
                if (Width != 0 && Height != 0)
                {
                    destWidth = Width;
                    destHeight = Height;
                }
                // change size proportially depending on width or height
                else if (Height != 0)
                {
                    destWidth = (float)(Height * sourceWidth) / sourceHeight;
                    destHeight = Height;
                }
                else
                {
                    destWidth = Width;
                    destHeight = (float)(sourceHeight * Width / sourceWidth);
                }

                Bitmap bmPhoto = new Bitmap((int)destWidth, (int)destHeight,
                                            PixelFormat.Format32bppPArgb);
                bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

                Graphics grPhoto = Graphics.FromImage(bmPhoto);
                grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

                grPhoto.DrawImage(imgPhoto,
                    new Rectangle(destX, destY, (int)destWidth, (int)destHeight),
                    new Rectangle(sourceX, sourceY, (int)sourceWidth, (int)sourceHeight),
                    GraphicsUnit.Pixel);

                grPhoto.Dispose();

                return bmPhoto;
            }
        }
        public class ImageUploadC
        {
            // set default size here
            public int Width { get; set; }

            public int Height { get; set; }

            // folder for the upload, you can put this in the web.config

            private readonly string UploadPath_C = "~/IMAGE_C/";

            public ImageResult RenameUploadFileC(HttpPostedFileBase file, string name, string incompany, string instk, Int32 counter = 0)
            {
                var fileName = Path.GetFileName(file.FileName);

                string prepend = "item_";

               // string finalFileName = prepend + ((counter).ToString()) + "_" + fileName;
                string finalFileName = incompany + "_" + instk + ".png";


                if (System.IO.File.Exists(System.Web.HttpContext.Current.Request.MapPath(UploadPath_C + finalFileName)))
                {
                    //file exists => add country try again
                   // return RenameUploadFileC(file, name, incompany,instk, ++counter);
                    return UploadFileC(file, finalFileName, incompany, instk);
                }

                //file doesn't exist, upload item but validate first
                return UploadFileC(file, finalFileName, incompany, instk);

            }

            private ImageResult UploadFileC(HttpPostedFileBase file, string fileName, string incompany, string instk)
            {
                ImageResult imageResult = new ImageResult { Success = true, ErrorMessage = null };
               

                var path =
              Path.Combine(System.Web.HttpContext.Current.Request.MapPath(UploadPath_C), fileName);
                string extension = Path.GetExtension(file.FileName);


                //make sure the file is valid
                if (!ValidateExtension(extension))
                {
                    imageResult.Success = false;
                    imageResult.ErrorMessage = "Invalid Extension";
                    return imageResult;
                }

                try
                {
                    file.SaveAs(path);
                    var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
                    SqlConnection Connection = new SqlConnection(connectionString);
                    Connection.Open();
                    var command = new SqlCommand("P_Save_PathImage", Connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@inname", fileName);
                    command.Parameters.AddWithValue("@incom", incompany);
                    command.Parameters.AddWithValue("@instk", instk);
                    command.ExecuteNonQuery();

                    command.Dispose();
                    Connection.Close();

                    Image imgOriginal = Image.FromFile(path);

                    //pass in whatever value you want
                    Image imgActual = Scale(imgOriginal);
                    imgOriginal.Dispose();
                    imgActual.Save(path);
                    imgActual.Dispose();

                    imageResult.ImageName = fileName;

                    return imageResult;
                }
                catch (Exception ex)
                {
                    // you might NOT want to show the exception error for the user
                    // this is generally logging or testing

                    imageResult.Success = false;
                    imageResult.ErrorMessage = ex.Message;
                    return imageResult;
                }
            }

            private bool ValidateExtension(string extension)
            {
                extension = extension.ToLower();
                switch (extension)
                {
                    case ".jpg":
                        return true;
                    case ".png":
                        return true;
                    case ".gif":
                        return true;
                    case ".jpeg":
                        return true;
                    default:
                        return false;
                }
            }

            private Image Scale(Image imgPhoto)
            {
                float sourceWidth = imgPhoto.Width;
                float sourceHeight = imgPhoto.Height;
                float destHeight = 0;
                float destWidth = 0;
                int sourceX = 0;
                int sourceY = 0;
                int destX = 0;
                int destY = 0;

                // force resize, might distort image
                if (Width != 0 && Height != 0)
                {
                    destWidth = Width;
                    destHeight = Height;
                }
                // change size proportially depending on width or height
                else if (Height != 0)
                {
                    destWidth = (float)(Height * sourceWidth) / sourceHeight;
                    destHeight = Height;
                }
                else
                {
                    destWidth = Width;
                    destHeight = (float)(sourceHeight * Width / sourceWidth);
                }

                Bitmap bmPhoto = new Bitmap((int)destWidth, (int)destHeight,
                                            PixelFormat.Format32bppPArgb);
                bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

                Graphics grPhoto = Graphics.FromImage(bmPhoto);
                grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

                grPhoto.DrawImage(imgPhoto,
                    new Rectangle(destX, destY, (int)destWidth, (int)destHeight),
                    new Rectangle(sourceX, sourceY, (int)sourceWidth, (int)sourceHeight),
                    GraphicsUnit.Pixel);

                grPhoto.Dispose();

                return bmPhoto;
            }
        }
        public class ImageResult
        {
            public bool Success { get; set; }
            public string ImageName { get; set; }
            public string ErrorMessage { get; set; }
        }
        
        //[HttpPost]
        //public ActionResult Savepathfiles(FormCollection formCollection)
        //{
        //    //int pussend = 0;
        //    //int pus = 0;
        //    string uname = string.Empty;
        //    string Pathimg = string.Empty;
        //    // string path = Server.MapPath(@"~\IMAGE_A\");
        //    HttpFileCollectionBase files = Request.Files;
        //    var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
        //    SqlConnection Connection = new SqlConnection(connectionString);
        //    Connection.Open();
        //    for (int i = 0; i < files.Count; i++)
        //    {


        //        string name = formCollection["uploadername"];
        //        string incompany = formCollection["incompany"];
        //        string instk = formCollection["instk"];


        //        //Pathimg = name + "-" + pussend + ".png";
        //        var command = new SqlCommand("P_Save_PathImage", Connection);
        //        command.CommandType = CommandType.StoredProcedure;
        //        command.Parameters.AddWithValue("@inname", "");
        //        command.Parameters.AddWithValue("@incom", incompany);
        //        command.Parameters.AddWithValue("@instk", instk);
        //        command.ExecuteNonQuery();
              
        //        command.Dispose();
               


        //    }
        //    Connection.Close();
        //    return Json(files.Count + " Files Uploaded!");
        //}


        [HttpPost]
        public ActionResult UploadFilesA(FormCollection formCollection)
        {
            //int pussend = 0;
            //int pus = 0;
            string uname = string.Empty;
            string Pathimg = string.Empty;
            string imgname = string.Empty;
            int RNo = 0;
            string path = Server.MapPath(@"~\IMAGE_A\");
            HttpFileCollectionBase files = Request.Files;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();

            //foreach (string item in Request.Files)
            //{
            //    HttpPostedFileBase file = Request.Files[item] as HttpPostedFileBase;
            //    if (file.ContentLength == 0)
            //        continue;
            //    if (file.ContentLength > 0)
            //    {
            //        string name = formCollection["uploadername"];
            //        string incompany = formCollection["incompany"];
            //        string instk = formCollection["instk"];

            //        ImageUpload imageUpload = new ImageUpload { Width = 350 };

            //        ImageResult imageResult = imageUpload.RenameUploadFile(file, name, incompany, instk); // Width = 350
            //        if (imageResult.Success)
            //        {
            //            //TODO: write the filename to the db
            //            Console.WriteLine(imageResult.ImageName);
            //        }
            //        else
            //        {
            //            // use imageResult.ErrorMessage to show the error
            //            ViewBag.Error = imageResult.ErrorMessage;
            //        }

            //    }
            //}
            for (int i = 0; i < files.Count; i++)
            {
            

                string name = formCollection["uploadername"];
                string incompany = formCollection["incompany"];
                string instk = formCollection["instk"];
                RNo = i + 1;
                imgname = incompany + "_" + RNo + "_" + instk + ".png";
              
                //Pathimg = name + "-" + pussend + ".png";
                var command = new SqlCommand("P_Save_PathImage", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@inname", imgname);
                command.Parameters.AddWithValue("@incom", incompany);
                command.Parameters.AddWithValue("@instk", instk);
                command.Parameters.AddWithValue("@inno", RNo);
                SqlParameter returnValuedoc = new SqlParameter("@outimagename", SqlDbType.NVarChar, 100);
                returnValuedoc.Direction = System.Data.ParameterDirection.Output;
                command.Parameters.Add(returnValuedoc);
                command.ExecuteNonQuery();
                uname = returnValuedoc.Value.ToString();
                command.Dispose();

                
                //pus = i;
                //pussend = (pus + 1);
                HttpPostedFileBase file = files[i];
                string fileName = file.FileName;
                // file.SaveAs(path + file.FileName);
                file.SaveAs(Server.MapPath(@"~\IMAGE_A\" + uname));


            }
            Connection.Close();
            return Json(files.Count + " Files Uploaded!");
        }
        //public JsonResult Upload(HttpPostedFileBase filex, string UserID)
        //{
        //    string message = string.Empty;
        //    string imgname = string.Empty;
        //    try
        //    {
        //        string ImageName = Guid.NewGuid().ToString();
        //        for (int i = 0; i < Request.Files.Count; i++)
        //        {
        //            HttpPostedFileBase file = Request.Files[i]; //Uploaded file

        //            //Use the following properties to get file's name, size and MIMEType
        //            int fileSize = file.ContentLength;
        //            string fileName = file.FileName;
        //            string mimeType = file.ContentType;

        //            System.IO.Stream fileContent = file.InputStream;
        //            //To save file, use SaveAs method
        //            // file.SaveAs(Server.MapPath("~/") + fileName); //File will be saved in application root
        //            file.SaveAs(Server.MapPath(@"~\ImgUpload\" + fileName + ".png")); //File will be saved in application root

        //            imgname = fileName + ".png";
        //            message = "true";

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        message = ex.Message;
        //    }

        //    return Json(new { message, imgname }, JsonRequestBehavior.AllowGet);
        //    //return Json("Uploaded " + Request.Files.Count + " files");
        //}
    }
}
