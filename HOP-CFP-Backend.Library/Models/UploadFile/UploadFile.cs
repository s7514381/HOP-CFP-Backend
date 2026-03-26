using Dapper.Contrib.Extensions;
using System;
using TableAttribute = Dapper.Contrib.Extensions.TableAttribute;

namespace HOP_CFP_Backend.Library.Models.UploadFile
{
    [Table(nameof(UploadFile))]
    public class UploadFile : ModelBase
    {
        [ExplicitKey]
        public Guid UploadFileId { get; set; } = Guid.NewGuid();

        public string ContentType { get; set; }

        public string Url { get; set; }

        public string OriginalFileName { get; set; }
    }
}