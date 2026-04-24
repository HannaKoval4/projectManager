using ProjectManager.Models;

namespace ProjectManager.Services
{
    public static class SessionService
    {
        public static User CurrentUser { get; private set; }

        public static bool IsAuthenticated => CurrentUser != null;

        public static void SetCurrentUser(User user)
        {
            CurrentUser = user;
        }

        public static void Clear()
        {
            CurrentUser = null;
        }
    }
}

