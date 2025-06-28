namespace PunchInAndOut.Model
{
    public class PersonalLogInfo
    {
        public required DateTime LogDate { get; set; }
        public required string UserName { get; set; }
        public required string UserId { get; set; }
        public required string ProjectId { get; set; }
        public ZealogicsTimeLogDto To()
        {
            return new ZealogicsTimeLogDto
            {
                username = UserName,
                userId = UserId,
                projectid = ProjectId,
                entrydat = LogDate.ToString("yyyy-MM-dd"),
            };
        }
    }
}
