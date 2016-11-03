using System;

namespace TeamBins.Common.ViewModels
{
    public class UploadDto
    {
        public int Id { set; get; }
        public int ParentId { set; get; }
        public string FileName { set; get; }
        public DateTime CreatedDate { set; get; }
        public int CreatedById { set; get; }
        public string Url { get; set; }
        public string Type { get; set; }

        public string FileExtn { set; get; }
    }
}