using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamBins6.Common
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class Class1
    {
        public Class1()
        {
        }
    }
    public class CategoryDto : NameValueItem
    {

    }

    public class NameValueItem
    {
        public string Name { set; get; }
        public string Code { set; get; }
        public int Id { set; get; }
    }

}
