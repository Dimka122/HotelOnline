using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;

namespace HotelBooking.Web.TagHelpers
{
    [HtmlTargetElement("ul", Attributes = "paged-model")]
    public class PagedListTagHelper : TagHelper
    {
        private readonly IUrlHelperFactory _urlHelperFactory;

        public PagedListTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;
        }

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("paged-model")]
        public dynamic PagedModel { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
            var model = PagedModel;
            
            if (model.TotalPages <= 1)
                return;

            output.TagName = "nav";
            output.Attributes.Add("aria-label", "Page navigation");

            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination justify-content-center");

            // Previous
            var prevLi = CreatePageItem(
                model.HasPreviousPage ? urlHelper.Action(model.ActionName, new { page = model.CurrentPage - 1 }) : null,
                "Предыдущая",
                "page-item",
                "page-link",
                !model.HasPreviousPage
            );
            ul.InnerHtml.AppendHtml(prevLi);

            // Page numbers
            for (int i = 1; i <= model.TotalPages; i++)
            {
                if (i == 1 || i == model.TotalPages || (i >= model.CurrentPage - 1 && i <= model.CurrentPage + 1))
                {
                    var active = i == model.CurrentPage ? "active" : "";
                    var pageLi = CreatePageItem(
                        urlHelper.Action(model.ActionName, new { page = i }),
                        i.ToString(),
                        "page-item " + active,
                        "page-link",
                        false
                    );
                    ul.InnerHtml.AppendHtml(pageLi);
                }
                else if (i == model.CurrentPage - 2 || i == model.CurrentPage + 2)
                {
                    var ellipsisLi = new TagBuilder("li");
                    ellipsisLi.AddCssClass("page-item disabled");
                    ellipsisLi.InnerHtml.AppendHtml("<span class=\"page-link\">...</span>");
                    ul.InnerHtml.AppendHtml(ellipsisLi);
                }
            }

            // Next
            var nextLi = CreatePageItem(
                model.HasNextPage ? urlHelper.Action(model.ActionName, new { page = model.CurrentPage + 1 }) : null,
                "Следующая",
                "page-item",
                "page-link",
                !model.HasNextPage
            );
            ul.InnerHtml.AppendHtml(nextLi);

            output.Content.AppendHtml(ul);
        }

        private TagBuilder CreatePageItem(string url, string text, string liClass, string linkClass, bool disabled)
        {
            var li = new TagBuilder("li");
            li.AddCssClass(liClass);

            if (disabled)
            {
                li.AddCssClass("disabled");
                li.InnerHtml.AppendHtml($"<span class=\"{linkClass}\">{text}</span>");
            }
            else
            {
                var a = new TagBuilder("a");
                a.AddCssClass(linkClass);
                a.Attributes["href"] = url;
                a.InnerHtml.AppendHtml(text);
                li.InnerHtml.AppendHtml(a);
            }

            return li;
        }
    }
}
