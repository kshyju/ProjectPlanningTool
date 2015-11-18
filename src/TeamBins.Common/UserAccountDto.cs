namespace TeamBins.Common
{
    public class UserAccountDto
    {
        public int Id { set; get; }
        public string EmailAddress { set; get; }
        public string Name { set; get; }
        public string Password { set; get; }
        public string GravatarUrl { get; set; }

        public int? DefaultTeamId { set; get; }
    }
}