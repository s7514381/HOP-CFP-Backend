using System;
using System.ComponentModel.DataAnnotations;
using TableAttribute = Dapper.Contrib.Extensions.TableAttribute;

namespace HOP_CFP_Backend.Library.Models.ImageCrop
{

    [Table(nameof(CropData))]
    public class CropData
    {
        [Key]
        public Guid UploadFileId { get; set; }
        public double width { get; set; }
        public double height { get; set; }
        public double rotate { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public double scaleX { get; set; }
        public double scaleY { get; set; }
    }
}
