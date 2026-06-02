using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Project.Models.Enums
{
   

    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            return value.GetType()
                .GetMember(value.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()?
                .GetName() ?? value.ToString();
        }



        public static SelectList GetEnumSelectList<TEnum>() where TEnum : Enum
        {
            var data = Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(x => new
                {
                    Value = x.ToString(),
                    Text = (x as Enum).GetDisplayName()
                });

            return new SelectList(data, "Value", "Text");
        }

    }
}

