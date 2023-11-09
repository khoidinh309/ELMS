namespace LeaveDayAPI.Permissions;

public static class LeaveDayAPIPermissions
{
    public const string LeaveRequestManagement = "LeaveRequestManagement";

    public const string CreatePermission = LeaveRequestManagement + ".Create";
    public const string ViewPermission = LeaveRequestManagement + ".View";
    public const string UpdatePermission = LeaveRequestManagement + ".Update";
    public const string DeletePermission = LeaveRequestManagement + ".Delete";
    public const string ApprovePermission = LeaveRequestManagement + ".Approve";
}
