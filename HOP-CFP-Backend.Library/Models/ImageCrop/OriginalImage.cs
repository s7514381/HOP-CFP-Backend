using System;
using System.ComponentModel.DataAnnotations;
using TableAttribute = Dapper.Contrib.Extensions.TableAttribute;

namespace HOP_CFP_Backend.Library.Models.ImageCrop
{
    [Table(nameof(OriginalImage))]
    public class OriginalImage
    {
        [Key]
        public Guid CroppedUploadFileId { get; set; }

        public Guid OriginalUploadFileId { get; set; }
    }
}
