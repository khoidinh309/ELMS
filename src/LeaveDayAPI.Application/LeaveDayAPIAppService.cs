using System;
using System.Collections.Generic;
using System.Text;
using LeaveDayAPI.Localization;
using Volo.Abp.Application.Services;

namespace LeaveDayAPI;

/* Inherit your application services from this class.
 */
public abstract class LeaveDayAPIAppService : ApplicationService
{
    protected LeaveDayAPIAppService()
    {
        LocalizationResource = typeof(LeaveDayAPIResource);
    }
}
