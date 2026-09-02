namespace TechStore.Models
{
    public static class SupportTicketStatuses
    {
        public const string Open = "Açık";
        public const string Pending = "Beklemede";
        public const string Closed = "Kapandı";

        private static readonly HashSet<string> ValidStatuses = [Open, Pending, Closed];

        public static bool IsValid(string status) => ValidStatuses.Contains(status);
    }
}
