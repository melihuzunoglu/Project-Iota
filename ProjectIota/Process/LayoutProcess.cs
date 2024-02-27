using ProjectIota.Data;

namespace ProjectIota.Process
{
    public class LayoutProcess
    {
        public string GetLogoutButtonVisibilityStyle(HttpContext httpContext)
        {
            if (httpContext.Session.GetString("UserId") != null)
            {
                return "display:block;";
            }
            else
            {
                return "display:none;";
            }
        }
    }
}
