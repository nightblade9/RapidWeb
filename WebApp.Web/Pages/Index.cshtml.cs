using Microsoft.AspNetCore.Mvc.Filters;
using WebApp.Web.Pages.Shared;

namespace WebApp.Web.Pages;

public class IndexModel : BasePageModel
{
    internal readonly IConfiguration Configuration;

    public IndexModel(IConfiguration configuration, Dictionary<string, object?> viewData = null)
    : base(viewData)
    {
        Configuration = configuration;
    }

    public override void OnPageHandlerExecuted(PageHandlerExecutedContext context)
    {
        base.OnPageHandlerExecuted(context);
        ViewData["Title"] = "Home";
    }
}
