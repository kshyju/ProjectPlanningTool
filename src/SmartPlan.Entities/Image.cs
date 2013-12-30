using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechiesWeb.TeamBins.Entities
{
    public class Image:Document
    {


    }
    public class Document
    {
        public int DocID { set; get; }
        public string DocName { set; get; }
        public string DocKey { set;get;}
        public int ParentID { set; get; }
        public string UploadType { set; get; }
        public string Extension { set; get; }
        public int CreatedByID { set; get; }
    }
}
