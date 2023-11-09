using Volo.Abp.Settings;

namespace LeaveDayAPI.Settings;

public class LeaveDayAPISettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(LeaveDayAPISettings.MySetting1));
    }
}
