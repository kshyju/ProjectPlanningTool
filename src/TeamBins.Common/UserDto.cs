using System;

namespace TeamBins.Common.ViewModels
{
    public class UserDto
    {
        public int Id { set; get; }
        public string EmailAddress { set; get; }
        public string Name { set; get; }
        public string GravatarUrl { get; set; }

        public DateTime? LastLoginDate { set; get; }

        public DateTime CreatedDate { set; get; }

    }
}