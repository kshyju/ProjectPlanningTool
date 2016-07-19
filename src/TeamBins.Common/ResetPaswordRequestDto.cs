using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamBins.Common.ViewModels
{
    public class ResetPaswordRequestDto
    {
        public string ActivationCode { set; get; }
        public int UserId { get; set; }
    }
}
