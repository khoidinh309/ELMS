using LeaveDayAPI.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace LeaveDayAPI.Permissions;

public class LeaveDayAPIPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var leaveRequestManagement = context.AddGroup(LeaveDayAPIPermissions.LeaveRequestManagement);
        
        leaveRequestManagement.AddPermission(LeaveDayAPIPermissions.CreatePermission, L("CreatePermission"));
        leaveRequestManagement.AddPermission(LeaveDayAPIPermissions.UpdatePermission, L("UpdatePermission"));
        leaveRequestManagement.AddPermission(LeaveDayAPIPermissions.ViewPermission, L("ViewPermission"));
        leaveRequestManagement.AddPermission(LeaveDayAPIPermissions.DeletePermission, L("DeletePermission"));
        leaveRequestManagement.AddPermission(LeaveDayAPIPermissions.ApprovePermission, L("ApprovePermission"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<LeaveDayAPIResource>(name);
    }
}
