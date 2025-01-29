using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Web.Pages.Shared;

public abstract class BasePageModel : PageModel
{
    // The page's ViewData. Injectable for unit testing, of course.
    private IDictionary<string, object?>? _viewData;

    public BasePageModel(IDictionary<string, object?>? viewDataDictionary = null)
    {
        _viewData = viewDataDictionary;
    }

    public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        // Can't do this in the constructor, because ViewData is null
        if (_viewData == null)
        {
            _viewData = base.ViewData;
        }
        
        base.OnPageHandlerExecuting(context);
    }

    new public IDictionary<string, object?> ViewData
    {
        get { return _viewData; }
        set { _viewData = value; }
    }
}
