using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.ViewFeatures;
using Microsoft.AspNet.Razor.TagHelpers;

namespace TeamBins6.Infrastrucutre.TagHelpers
{
    [HtmlTargetElement("div", Attributes = DurationAttributeName)]
    public class AlertMessagesTagHelper : TagHelper
    {
        private const string DurationAttributeName = "message-duration";

        /// <summary>
        /// Message duration in milli seconds
        /// </summary>
        [HtmlAttributeName(DurationAttributeName)]
        public int Duration { get; set; }



        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ViewContext s = new ViewContext();
            StringBuilder str = new StringBuilder();

            var messages = ViewContext.TempData["AlertMessages"] as Dictionary<string, string>;
            if (messages != null)
            {
                foreach (var message in messages)
                {
                    str.AppendFormat("<div class='alert alert-success' data-duration='{1}' role='alert'>{0}</div>", message.Value, Duration);

                }
            }

            output.Content.AppendHtml(str.ToString());

        }
    }




}
