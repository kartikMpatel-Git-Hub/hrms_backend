namespace hrms.Utility
{
    public static class CacheVersionKey
    {
        public static string For(string domain) => $"{domain}:version";
        public static string ForJobDetails(int jobId) => $"CacheVersion:JobDetails:JobId:{jobId}";
        public static string ForJobReferrals(int jobId) => $"CacheVersion:JobReferrals:JobId:{jobId}";
        public static string ForJobShares(int jobId) => $"CacheVersion:JobShares:JobId:{jobId}";
        public static string ForDepartmentDetails(int departmentId) => $"CacheVersion:DepartmentDetails:DepartmentId:{departmentId}";
        public static string ForUserNotifications(int userId) => $"CacheVersion:Notifications:User:{userId}";
    }
}