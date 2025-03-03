using App.Core.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Service
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public ImageService(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
        {
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Domain.Model.ResponseDto> Upload(IFormFile file)
        {
            // Validate that file is provided
            if (file == null)
            {
                return new Domain.Model.ResponseDto
                {
                    Status = 400,
                    Message = "No file provided",
                    Data = ""
                };
            }

            // Valid extensions
            List<string> validExtensions = new List<string> { ".jpg", ".png", ".jpeg" ,".jfif" };
            string extension = Path.GetExtension(file.FileName).ToLower(); // Normalize the extension to lowercase

            if (!validExtensions.Contains(extension))
            {
                return new Domain.Model.ResponseDto
                {
                    Status = 400,
                    Message = "Extension Not Valid",
                    Data = ""
                };
            }

            // Size check: 5MB max
            long size = file.Length;
            if (size > (5 * 1024 * 1024))
            {
                return new Domain.Model.ResponseDto
                {
                    Status = 400,
                    Message = "Maximum size can be 5Mb",
                    Data = ""
                };
            }

            // Change file name to ensure uniqueness
            string filename = Guid.NewGuid().ToString() + extension;

            // Determine the upload path (ensure the path exists)
            string uploadPath = Path.Combine(_environment.WebRootPath, "Images");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // Write the file to the server
            string filePath = Path.Combine(uploadPath, filename);
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Get the scheme and host from HttpContext
            var request = _httpContextAccessor.HttpContext.Request;
            string scheme = request.Scheme;
            string host = request.Host.Value;

            // Generate the full URL for the uploaded file
            string fileUrl = $"{scheme}://{host}/Images/{filename}";

            return new Domain.Model.ResponseDto
            {
                Status = 200,
                Message = "File Stored Successfully",
                Data = fileUrl
            };
        }
    }
}
