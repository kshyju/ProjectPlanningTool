namespace TeamBins.Common
{
    public class UserAccountDto : UserDto
    {
        
        public string Password { set; get; }
        public string GravatarUrl { get; set; }

        public int? DefaultTeamId { set; get; }
    }

    public class UserDto
    {
        public int Id { set; get; }
        public string EmailAddress { set; get; }
        public string Name { set; get; }
        public string GravatarUrl { get; set; }
    }
}