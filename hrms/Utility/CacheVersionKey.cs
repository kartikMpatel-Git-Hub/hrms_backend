namespace hrms.Utility
{
    public static class CacheVersionKey
    {
        public static string For(string domain) => $"{domain}:version";
        public static string ForJobInfo(int jobId) => $"CacheVersion:JobInfo:JobId:{jobId}";
        public static string ForJobDetails(int jobId) => $"CacheVersion:JobDetails:JobId:{jobId}";
        public static string ForJobReferrals(int jobId) => $"CacheVersion:JobReferrals:JobId:{jobId}";
        public static string ForJobShares(int jobId) => $"CacheVersion:JobShares:JobId:{jobId}";
        public static string ForDepartmentDetails(int departmentId) => $"CacheVersion:DepartmentDetails:DepartmentId:{departmentId}";
        public static string ForUserNotifications(int userId) => $"CacheVersion:Notifications:User:{userId}";
        public static string ForUserInfo(int userId) => $"CacheVersion:UserInfo:{userId}";
        public static string ForHrInfo() => $"CacheVersion:HrInfo";
        public static string ForManagerInfo() => $"CacheVersion:ManagerInfo";
        public static string ForManagerTeam() => $"CacheVersion:ManagerTeam";
        public static string ForEmployeeInfo() => $"CacheVersion:EmployeeInfo";
        public static string ForUserDetails(int userId) => $"CacheVersion:UserDetails:UserId:{userId}";
        public static string ForUserProfile(int userId) => $"CacheVersion:UserProfile:UserId:{userId}";
        public static string ForGameInfo(int gameId) => $"CacheVersion:GameInfo:GameId:{gameId}";
        public static string ForGameSlots(int gameId) => $"CacheVersion:GameSlots:GameId:{gameId}";
        public static string ForGameOperationWindows(int gameId) => $"CacheVersion:GameOperationWindows:GameId:{gameId}";
        public static string ForTravelInfo(int travelId) => $"CacheVersion:TravelInfo:TravelId:{travelId}";
        public static string ForTravelTravelers(int travelId) => $"CacheVersion:TravelTravelers:TravelId:{travelId}";
        public static string ForTravelerTravels(int travelerId) => $"CacheVersion:TravelerTravels:TravelerId:{travelerId}";
        public static string ForTravelDocuments(int travelId) => $"CacheVersion:TravelDocuments:TravelId:{travelId}";
        public static string ForTravelExpenses(int travelId) => $"CacheVersion:TravelExpenses:TravelId:{travelId}";
        public static string ForPostInfo(int postId) => $"CacheVersion:PostInfo:PostId:{postId}";
        public static string ForPostComments(int postId) => $"CacheVersion:PostComments:PostId:{postId}";
        public static string ForUserPosts(int userId) => $"CacheVersion:UserPosts:UserId:{userId}";
    }
}