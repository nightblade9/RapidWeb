using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Web.Pages.Shared;

public abstract class BasePageModel : PageModel
{
    // The page's ViewData. Injectable for unit testing, of course.
    private IDictionary<string, object?>? _viewData;

    protected BasePageModel(IDictionary<string, object?>? viewDataDictionary = null)
    {
        _viewData = viewDataDictionary;
    }

    new public IDictionary<string, object?> ViewData
    {
        get { return _viewData ?? base.ViewData; }
        set { _viewData = value; }
    }
}
