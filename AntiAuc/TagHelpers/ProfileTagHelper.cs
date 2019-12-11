using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using AntiAuc.Models;

namespace AntiAuc.TagHelpers
{
    public class ProfileTagHelper : TagHelper
    {
        public User of { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("class","text-muted");
            output.Attributes.SetAttribute("href",$"/Account/Profile/{of.Id}");
            //output.Attributes.SetAttribute("asp-action", "Profile");
            //output.Attributes.SetAttribute("asp-route-id",$"{of.Id}");
            output.Content.SetContent(of.Email);

        }
    }
}
