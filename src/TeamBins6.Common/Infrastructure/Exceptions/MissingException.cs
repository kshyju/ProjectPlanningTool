using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamBins6.Common.Infrastructure.Exceptions
{
    public class MissingSettingsException : Exception
    {
        public MissingSettingsException(string message, string missingSettingName)
            : base(message)
        {
            MissingSettingName = missingSettingName;
        }
        public string MissingSettingName { set; get; }
    }
}
