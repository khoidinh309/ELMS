using LeaveDayAPI.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace LeaveDayAPI.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class LeaveDayAPIController : AbpControllerBase
{
    protected LeaveDayAPIController()
    {
        LocalizationResource = typeof(LeaveDayAPIResource);
    }
}
