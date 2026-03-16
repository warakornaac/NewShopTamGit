using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NewShopTAM.Filters 
{
    public class SystemAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var routeData = httpContext.Request.RequestContext.RouteData;
            var controller = routeData.Values["controller"]?.ToString();
            var action = routeData.Values["action"]?.ToString();

            if (controller == "Account" || controller == "Verification" || controller == "CustomerDashboard" || controller == "PortalForSales")
            {
                return true; // bypass authorize
            }

            if (httpContext?.Session == null)
                return false;

            string userId = httpContext.Session["UserID"]?.ToString();
            if (string.IsNullOrEmpty(userId))
                return false;

            string systemCode = ConfigurationManager.AppSettings["SystemCode"];
            string loginSystem = httpContext.Session["LoginSystem"]?.ToString();

            if (string.IsNullOrEmpty(systemCode))
                return false;

            return loginSystem == systemCode;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary(new
                {
                    controller = "Account",
                    action = "Login"
                })
            );
        }
    }
}
