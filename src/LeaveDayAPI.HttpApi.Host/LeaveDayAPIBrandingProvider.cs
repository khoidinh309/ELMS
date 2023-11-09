using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace LeaveDayAPI;

[Dependency(ReplaceServices = true)]
public class LeaveDayAPIBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "LeaveDayAPI";
}
