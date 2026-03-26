using System;
using System.ComponentModel.DataAnnotations;
using HOP_CFP_Backend.Library.Attributes;
using HOP_CFP_Backend.Library.Models.ImageCrop;

namespace HOP_CFP_Backend.ViewModels
{
    public class UploadImage
    {
        [NoHistory]
        public Guid uploadFileId { get; set; }

        [Display(Name = "圖片路徑")]
        public string url { get; set; }

        [NoHistory]
        public Guid originalImageId { get; set; }

        [Display(Name = "原始圖片路徑")]
        public string originalImageUrl { get; set; }

        [NoHistory]
        public CropData cropData { get; set; }

        [NoHistory]
        public bool isChanged { get; set; } = false;
    }
}
