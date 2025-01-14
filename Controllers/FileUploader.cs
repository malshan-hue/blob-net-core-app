using blob_net_core_app.Models;
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;

namespace blob_net_core_app.Controllers
{
    public class FileUploader : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _containerName;

        public FileUploader(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetValue<string>("AzureBlobStorage:ConnectionString");
            _containerName = _configuration.GetValue<string>("AzureBlobStorage:ContainerName");
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new FileUploadViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest("No files uploaded.");
            }

            var uploadedFileUrls = new List<string>();

            foreach (var file in files)
            {
                var url = await UploadToBlobAsync(file);
                uploadedFileUrls.Add(url);
            }

            return Json(new { fileUrls = uploadedFileUrls });
        }

        private async Task<string> UploadToBlobAsync(IFormFile file)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(file.FileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            return blobClient.Uri.ToString(); 
        }


    }
}
