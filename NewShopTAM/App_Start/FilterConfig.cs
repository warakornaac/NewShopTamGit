using System.Web;
using System.Web.Mvc;
using NewShopTAM.Filters;

namespace NewShopTAM
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //ทุก request ต้องผ่าน SystemAuthorize
            //filters.Add(new SystemAuthorizeAttribute());
        }
    }
}