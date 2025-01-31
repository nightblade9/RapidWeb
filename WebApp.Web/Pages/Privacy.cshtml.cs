using Microsoft.AspNetCore.Mvc.Filters;
using WebApp.Web.Pages.Shared;

namespace WebApp.Web.Pages;

public class PrivacyModel : BasePageModel
{
    public PrivacyModel(Dictionary<string, object?>? viewData = null)
    : base(viewData)
    {
    }
    
    public override void OnPageHandlerExecuted(PageHandlerExecutedContext context)
    {
        base.OnPageHandlerExecuted(context);
        ViewData["Title"] = "Privacy Policy";
    }
}

