namespace OfficePlantCare.Areas.AdminQL.Models
{
    public class NotificationViewModel
    {
        public int NotificationId { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string TimeAgo { get; set; }
        public bool IsRead { get; set; }
        public bool IsDeleted { get; set; }
        public string Icon { get; set; }
        public string Link { get; set; }
    }
}