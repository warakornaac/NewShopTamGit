using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewShop.Controllers;
using System.Data;
using System.IO;
using System.Web.Script.Serialization;
using NewShop.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NewShop.Controllers
{
    public class TechdocController : Controller
    {
        //
        // GET: /Techdoc/

        public ActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> getArticles(string Partno, string dataSupplierId)
        {
            //var url = "https://mst.aac.co.th/APIService/Post/PushMessage";
            var url = "https://localhost:44361/Post/Articles";
            string status = string.Empty;
            string dataResponse = string.Empty;
            string encodeDocno = string.Empty;
            string encodeUrlDetail = string.Empty;

            var post = new
            {
                Partno = Partno,
                DataSupplierId = dataSupplierId,
            };
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var handler = new HttpClientHandler();
                var client = new HttpClient(handler);
                string jsonContent = JsonConvert.SerializeObject(post);
                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(url, content).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    dataResponse = responseContent;
                    status = response.StatusCode.ToString();
                }
                else
                {
                    status = response.StatusCode.ToString();
                }
            }
            catch (Exception ex)
            {
                dataResponse = ex.Message;
            }
            JObject jsonResponse = JObject.Parse(dataResponse);
            var articleIdToken = jsonResponse["ArticleId"];
            var articleNumberToken = jsonResponse["ArticleNumber"];
            var additionalDescriptionsToken = jsonResponse["AdditionalDescriptions"];
            var ArticleStatusDescriptionToken = jsonResponse["ArticleStatusDescription"];
            var mfrNamesToken = jsonResponse["MfrNames"];

            var articleId = articleIdToken != null && articleIdToken.Type != JTokenType.Null
                ? articleIdToken.ToObject<List<string>>()
                : null;
            var articleNumber = articleNumberToken != null && articleNumberToken.Type != JTokenType.Null
                ? articleNumberToken.ToObject<List<string>>()
                : null;
            var additionalDescriptions = additionalDescriptionsToken != null && additionalDescriptionsToken.Type != JTokenType.Null
                ? additionalDescriptionsToken.ToObject<List<string>>()
                : null;
            var articleStatusDescription = ArticleStatusDescriptionToken != null && ArticleStatusDescriptionToken.Type != JTokenType.Null
                ? ArticleStatusDescriptionToken.ToObject<List<string>>()
                : null;
            var mfrNames = mfrNamesToken != null && mfrNamesToken.Type != JTokenType.Null
                ? mfrNamesToken.ToObject<List<string>>()
                : null;
            @ViewBag.articleId = articleId;
            @ViewBag.articleNumber = articleNumber;
            @ViewBag.additionalDescriptions = additionalDescriptions;
            @ViewBag.articleStatusDescription = articleStatusDescription;
            @ViewBag.mfrName = mfrNames;
            ArticleData articleResponse = JsonConvert.DeserializeObject<ArticleData>(dataResponse);
            @ViewBag.dataResponse = articleResponse;
            return PartialView("_listResultArticles", new
            {
                @ViewBag.articleId,
                @ViewBag.articleNumber,
                @ViewBag.additionalDescriptions,
                @ViewBag.articleStatusDescription,
                @ViewBag.mfrName,
                @ViewBag.dataResponse
            });
        }
        public class GenericArticles
        {
            public int genericArticleId { get; set; }
            public string genericArticleDescription { get; set; }
            public int assemblyGroupNodeId { get; set; }
            public string assemblyGroupName { get; set; }
            public int legacyArticleId { get; set; }
            public List<string> linkageTargetTypes { get; set; }
        }
        public class Image
        {
            public string imageURL50 { get; set; }
            public string imageURL100 { get; set; }
            public string imageURL200 { get; set; }
            public string imageURL400 { get; set; }
            public string imageURL800 { get; set; }
            public string imageURL1600 { get; set; }
            public string imageURL3200 { get; set; }
            public string fileName { get; set; }
            public string typeDescription { get; set; }
            public int typeKey { get; set; }
            public string headerDescription { get; set; }
            public int headerKey { get; set; }
            public int sortNumber { get; set; }
            public string assetSource { get; set; }
        }
        public class OemNumber
        {
            public string articleNumber { get; set; }
            public int mfrId { get; set; }
            public string mfrName { get; set; }
            public bool matchesSearchQuery { get; set; }
        }

        public class ArticleCriteria
        {
            public int criteriaId { get; set; }
            public string criteriaDescription { get; set; }
            public string criteriaAbbrDescription { get; set; }
            public string criteriaUnitDescription { get; set; }
            public string criteriaType { get; set; }
            public string rawValue { get; set; }
            public string formattedValue { get; set; }
            public bool immediateDisplay { get; set; }
            public bool isMandatory { get; set; }
            public bool isInterval { get; set; }
        }
        public class Pdf
        {
            public string url { get; set; }
            public string fileName { get; set; }
            public string typeDescription { get; set; }
            public string headerDescription { get; set; }
            public int sortNumber { get; set; }
            public string assetSource { get; set; }
        }
        public class TradeNumberDetail
        {
            public string tradeNumber { get; set; }
            public bool isImmediateDisplay { get; set; }
        }
        public class ArticleData
        {
            public List<GenericArticles> GenericArticles { get; set; }
            public List<Image> Images { get; set; }
            public List<OemNumber> OemNumbers { get; set; }
            public List<ArticleCriteria> ArticleCriteria { get; set; }
            public List<Pdf> Pdf { get; set; }
            public List<TradeNumberDetail> TradeNumberDetail { get; set; }

        }
        public class ArticleDataProductBrand
        {
            public string MfrName { get; set; }
            public string ArticleNumber { get; set; }
            public string AdditionalDescription { get; set; }
            public string ArticleStatusDescription { get; set; }
            public List<GenericArticles> GenericArticles { get; set; }
            public List<Image> Images { get; set; }
            public List<OemNumber> OemNumbers { get; set; }
            public List<ArticleCriteria> ArticleCriteria { get; set; }
            public List<Pdf> Pdf { get; set; }
            public List<TradeNumberDetail> TradeNumberDetail { get; set; }

        }
        public async Task<ActionResult> getLinkageTargets(string ArticleId)
        {
            var url = "https://localhost:44361/Post/ArticlesLinkedAll";
            string status = string.Empty;
            string dataResponse = string.Empty;
            string encodeDocno = string.Empty;
            string encodeUrlDetail = string.Empty;

            var post = new
            {
                ArticleId = ArticleId,
            };
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var handler = new HttpClientHandler();
                var client = new HttpClient(handler);
                string jsonContent = JsonConvert.SerializeObject(post);
                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(url, content).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    dataResponse = responseContent;
                    status = response.StatusCode.ToString();
                }
                else
                {
                    status = response.StatusCode.ToString();
                }
            }
            catch (Exception ex)
            {
                dataResponse = ex.Message;
            }

            LinkageData articleResponse = JsonConvert.DeserializeObject<LinkageData>(dataResponse);
            @ViewBag.dataResponse = articleResponse;
            return PartialView("_listResultCar", new
            {
                @ViewBag.dataResponse
            });
        }
        public class LinkageData
        {
            public List<LinkageDetails> LinkageDetails { get; set; }
        }
        public class LinkageDetails
        {
            public int? LinkageTargetId { get; set; }
            public string LinkageTargetType { get; set; }
            public string SubLinkageTargetType { get; set; }
            public string MfrName { get; set; }
            public string VehicleModelSeriesName { get; set; }
            public string BeginYearMonth { get; set; }
            public string EndYearMonth { get; set; }
            public string ImageURL { get; set; }
            public string DriveType { get; set; }
            public string BodyStyle { get; set; }
            public string FuelMixtureFormationType { get; set; }
            public string FuelType { get; set; }
            public string EngineType { get; set; }
            public string Engines { get; set; }
        }
        public async Task<ActionResult> getArticlesPartNumberNearby(string Partno)
        {
            var url = "https://localhost:44361/Post/ArticlesPartNumberNearby";
            string status = string.Empty;
            string dataResponse = string.Empty;
            string encodeDocno = string.Empty;
            string encodeUrlDetail = string.Empty;

            var post = new
            {
                Partno = Partno,
            };
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var handler = new HttpClientHandler();
                var client = new HttpClient(handler);
                string jsonContent = JsonConvert.SerializeObject(post);
                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(url, content).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    dataResponse = responseContent;
                    status = response.StatusCode.ToString();
                }
                else
                {
                    status = response.StatusCode.ToString();
                }
            }
            catch (Exception ex)
            {
                dataResponse = ex.Message;
            }

            PartNumberNearbyData articleResponse = JsonConvert.DeserializeObject<PartNumberNearbyData>(dataResponse);
            @ViewBag.dataResponse = articleResponse;
            return PartialView("_listResultPartNumberNearby", new
            {
                @ViewBag.dataResponse
            });
        }
        public class PartNumberNearbyData
        {
            public List<PartNumberNearby> PartNumberNearby { get; set; }
        }
        public class PartNumberNearby
        {
            public int? articleId { get; set; }
            public string articleName { get; set; }
            public string articleNo { get; set; }
            public string articleSearchNo { get; set; }
            public int articleStateId { get; set; }
            public string brandName { get; set; }
            public int brandNo { get; set; }
            public int numberType { get; set; }
        }
        //
        //Vehicle
        //
        public async Task<ActionResult> GetVehicleAll()
        {
            var url = "https://localhost:44361/Post/VehicleAll";
            string status = string.Empty;
            string dataResponse = string.Empty;

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var handler = new HttpClientHandler();
                var client = new HttpClient(handler);
                string jsonContent = JsonConvert.SerializeObject("");
                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content); // using await properly now
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    dataResponse = responseContent;
                    status = response.StatusCode.ToString();
                }
                else
                {
                    status = response.StatusCode.ToString();
                }
            }
            catch (Exception ex)
            {
                dataResponse = ex.Message;
            }

            VehicleAllData articleResponse = JsonConvert.DeserializeObject<VehicleAllData>(dataResponse);
            ViewBag.dataResponse = articleResponse; // no @ needed here
            return View("IndexVehicle");
        }
        public class VehicleAllData
        {
            public List<VehicleListData> VehicleListData { get; set; }
        }
        public class VehicleListData
        {
            public int? manuId { get; set; }
            public string manuName { get; set; }
        }
        public async Task<ActionResult> GetModelSeries(string BrandId)
        {
            var url = "https://localhost:44361/Post/VehicleModel";
            string status = string.Empty;
            string dataResponse = string.Empty;
            string encodeDocno = string.Empty;
            string encodeUrlDetail = string.Empty;
            var post = new
            {
                manuId = BrandId,
            };

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var handler = new HttpClientHandler();
                var client = new HttpClient(handler);
                string jsonContent = JsonConvert.SerializeObject(post);
                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(url, content).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    dataResponse = responseContent;
                    status = response.StatusCode.ToString();
                }
                else
                {
                    status = response.StatusCode.ToString();
                }
            }
            catch (Exception ex)
            {
                dataResponse = ex.Message;
            }

            VehicleModelAllData articleResponse = JsonConvert.DeserializeObject<VehicleModelAllData>(dataResponse);
            @ViewBag.dataResponse = articleResponse;
            @ViewBag.manuId = BrandId;
            return PartialView("_listResultModel", new
            {
                @ViewBag.dataResponse
            });
        }
        public class VehicleModelAllData
        {
            public List<VehicleModelListData> VehicleModelListData { get; set; }
        }
        public class VehicleModelListData
        {
            public int? modelId { get; set; }
            public string modelname { get; set; }
            public string yearOfConstrFrom { get; set; }
            public string yearOfConstrTo { get; set; }
        }
        public async Task<ActionResult> GetLinkageTargetsModel(string manuId, string vehicleModelSeriesIds)
        {
            var url = "https://localhost:44361/Post/LinkageTargetsModel";
            string status = string.Empty;
            string dataResponse = string.Empty;
            string encodeDocno = string.Empty;
            string encodeUrlDetail = string.Empty;

            var post = new
            {
                mfrIds = manuId,
                vehicleModelSeriesIds = vehicleModelSeriesIds
            };
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var handler = new HttpClientHandler();
                var client = new HttpClient(handler);
                string jsonContent = JsonConvert.SerializeObject(post);
                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(url, content).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    dataResponse = responseContent;
                    status = response.StatusCode.ToString();
                }
                else
                {
                    status = response.StatusCode.ToString();
                }
            }
            catch (Exception ex)
            {
                dataResponse = ex.Message;
            }

            LinkageModelData articleResponse = JsonConvert.DeserializeObject<LinkageModelData>(dataResponse);
            @ViewBag.dataResponse = articleResponse;
            return PartialView("_listResultModelCar", new
            {
                @ViewBag.dataResponse
            });
        }
        public class LinkageModelData
        {
            public List<TargetsModelListData> TargetsModelListData { get; set; }
        }
        public class TargetsModelListData
        {
            public int? LinkageTargetId { get; set; }
            public string LinkageTargetType { get; set; }
            public string SubLinkageTargetType { get; set; }
            public string MfrName { get; set; }
            public string VehicleModelSeriesName { get; set; }
            public string BeginYearMonth { get; set; }
            public string EndYearMonth { get; set; }
            public string ImageURL { get; set; }
            public string DriveType { get; set; }
            public string BodyStyle { get; set; }
            public string FuelMixtureFormationType { get; set; }
            public string FuelType { get; set; }
            public string EngineType { get; set; }
            public string Engines { get; set; }
        }
        //
        //Search Product Brand
        //
        public async Task<ActionResult> IndexSearchBrand()
        {
            var url = "https://localhost:44361/Post/SupplierAll";
            string status = string.Empty;
            string dataResponse = string.Empty;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var handler = new HttpClientHandler();
                var client = new HttpClient(handler);
                string jsonContent = JsonConvert.SerializeObject("");
                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content); // using await properly now
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    dataResponse = responseContent;
                    status = response.StatusCode.ToString();
                }
                else
                {
                    status = response.StatusCode.ToString();
                }
            }
            catch (Exception ex)
            {
                dataResponse = ex.Message;
            }
            SupplierAllData dataSupplierResponse = JsonConvert.DeserializeObject<SupplierAllData>(dataResponse);
            var dataProductAll = await FetchGroupProductAllData();

            ViewBag.dataSupplier = dataSupplierResponse;
            ViewBag.dataProduct = dataProductAll;

            return View("IndexSearchBrand");
        }
        public class SupplierAllData
        {
            public List<SupplierListData> SupplierListData { get; set; }
        }
        public class SupplierListData
        {
            public int? brandId { get; set; }
            public string brandName { get; set; }
        }
        private async Task<GroupProductAllData> FetchGroupProductAllData()
        {
            var url = "https://localhost:44361/Post/GroupProductAll";
            string dataResponse = string.Empty;

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var handler = new HttpClientHandler();
                var client = new HttpClient(handler);
                string jsonContent = JsonConvert.SerializeObject("");
                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    dataResponse = responseContent;
                }
                else
                {
                    // Optional: log or handle unsuccessful status
                }
            }
            catch (Exception ex)
            {
                dataResponse = ex.Message;
            }

            return JsonConvert.DeserializeObject<GroupProductAllData>(dataResponse);
        }
        public class GroupProductAllData
        {
            public List<GroupProductAllListData> GroupProductListData { get; set; }
        }
        public class GroupProductAllListData
        {
            public string assemblyGroup { get; set; }
            public string designation { get; set; }
            public string genericArticleId { get; set; }
            public string masterDesignation { get; set; }
        }
        public async Task<ActionResult> GetGroupProductAll()
        {
            var dataSupplierResponse = await FetchGroupProductAllData();
            return Json(new
            {
                dataSupplierResponse
            });
        }
        public async Task<ActionResult> getProductByBrandGroup(string brandId, string groupId)
        {
            var url = "https://localhost:44361/Post/ProductByBrandGroup";
            string status = string.Empty;
            string dataResponse = string.Empty;
            string encodeDocno = string.Empty;
            string encodeUrlDetail = string.Empty;
            List<ArticleDataProductBrand> articleResponse = new List<ArticleDataProductBrand>();
            var post = new
            {
                brandId = brandId,
                groupId = groupId
            };
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var handler = new HttpClientHandler();
                var client = new HttpClient(handler);
                string jsonContent = JsonConvert.SerializeObject(post);
                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(url, content).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    articleResponse = JsonConvert.DeserializeObject<List<ArticleDataProductBrand>>(responseContent);
                    status = response.StatusCode.ToString();
                    @ViewBag.dataResponse = responseContent;
                }
                else
                {
                    status = response.StatusCode.ToString();
                }
            }
            catch (Exception ex)
            {
                dataResponse = ex.Message;
            }

            //return PartialView("_listResultBrandSupplier", new
            //{
            //    @ViewBag.dataResponse
            //});
            return PartialView("_listResultBrandSupplier", articleResponse);
        }
    }
}