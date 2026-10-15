namespace HomeHub.Models
{
    public static class Recurrence
    {
        public static DateTime Next(DateTime date, string recurrence)
        {
            return recurrence switch
            {
                "daily" => date.AddDays(1),
                "weekly" => date.AddDays(7),
                "monthly" => date.AddMonths(1),
                _ => date
            };
        }
    }
}
