using System.ComponentModel.DataAnnotations;

namespace HOP_CFP_Backend.Library.Models
{
    public enum EDynamicFormFieldType
    {
        [Display(Name = "文字輸入")]
        Input = 1,

        [Display(Name = "多行輸入")]
        Textarea = 2,

        [Display(Name = "單一選擇")]
        Select = 3,

        [Display(Name = "多重選擇")]
        MultiSelect = 4,

        [Display(Name = "日期")]
        Date = 5,

        [Display(Name = "數字")]
        Number = 6,
    }

    public static class EDynamicFormFieldTypeExtension
    {
        public static string ColorClass(this EDynamicFormFieldType type)
        {
            switch (type)
            {
                case EDynamicFormFieldType.Input:
                    return "blue";
                case EDynamicFormFieldType.Date:
                    return "green";
                case EDynamicFormFieldType.Select:
                    return "red";
                case EDynamicFormFieldType.Number:
                    return "yellow";
                default:
                    return "";
            }
        }

    }

}