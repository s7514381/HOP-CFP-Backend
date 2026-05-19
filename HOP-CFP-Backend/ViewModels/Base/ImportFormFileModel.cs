using Microsoft.AspNetCore.Http;

namespace HOP_CFP_Backend.ViewModels
{
    public class ImportFormFileModel
    {
        public IFormFile? file { get; set; }

        public bool ignoreErrors { get; set; } = true;
    }
}