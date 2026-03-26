using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using HOP_CFP_Backend.Library.Models;

namespace HOP_CFP_Backend.Library.Utility
{
    public static class CommentGenerator
    {
        public static IEnumerable<Type> GetTableTypes(Type type)
        {
            return from property in type.GetProperties()
                   where property.PropertyType.IsGenericType
                   where property.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)
                   select property.PropertyType.GetGenericArguments().First();
        }

        public static bool NeedGenerateComment(MemberInfo memberInfo, out string generatedComment)
        {
            generatedComment = string.Empty;
            if (memberInfo.GetCustomAttribute<CommentAttribute>() is not null)
            {
                return false;
            }

            var display = memberInfo.GetCustomAttribute<DisplayAttribute>();
            if (display != null)
            {
                generatedComment = display.Name;
            }

            if (memberInfo is PropertyInfo property)
            {
                if (property.PropertyType.IsEnum)
                {
                    generatedComment += $" {GetEnumAdditionalComment(property)}";
                }
            }

            generatedComment = generatedComment.Trim();

            return !string.IsNullOrEmpty(generatedComment);
        }

        public static string GetEnumAdditionalComment(PropertyInfo propertyInfo)
        {
            List<string> values = new();
            var enumType = propertyInfo.PropertyType;
            var enumValues = enumType.GetEnumValues();
            foreach (var e in enumValues)
            {
                var value = enumType.GetField("value__").GetValue(e);
                var name = e.ToString();

                if ((e is Enum enumE)) name = enumE.GetDisplayName();
                values.Add($"({value}){name}");
            }
            return string.Join(' ', values);
        }
    }
}
