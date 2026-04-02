using BlobApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlobApp.Controllers
{
    public class BlobController : Controller
    {
        private readonly BlobService _blobService;

        public BlobController(BlobService blobService)
        {
            _blobService = blobService;
        }

        // ── INDEX - List all files ────────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            var files = await _blobService.ListFilesAsync();
            return View(files);
        }

        // ── UPLOAD ────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select a file to upload.";
                return RedirectToAction("Index");
            }

            // Allowed types
            var allowedTypes = new[]
            {
                "image/jpeg", "image/png", "image/gif",
                "application/pdf",
                "application/msword",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            };

            if (!allowedTypes.Contains(file.ContentType))
            {
                TempData["Error"] = "Only images (JPG, PNG) and documents (PDF, Word) are allowed.";
                return RedirectToAction("Index");
            }

            // Max 10MB
            if (file.Length > 10 * 1024 * 1024)
            {
                TempData["Error"] = "File size must be less than 10MB.";
                return RedirectToAction("Index");
            }

            var success = await _blobService.UploadFileAsync(file);

            TempData[success ? "Success" : "Error"] = success
                ? $"'{file.FileName}' uploaded successfully!"
                : "Upload failed. Please try again.";

            return RedirectToAction("Index");
        }

        // ── DOWNLOAD ──────────────────────────────────────────────────────
        public async Task<IActionResult> Download(string fileName)
        {
            var (stream, contentType, name) = await _blobService.DownloadFileAsync(fileName);
            return File(stream, contentType, name);
        }

        // ── DELETE ────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Delete(string fileName)
        {
            var success = await _blobService.DeleteFileAsync(fileName);

            TempData[success ? "Success" : "Error"] = success
                ? $"'{fileName}' deleted successfully!"
                : "Delete failed. Please try again.";

            return RedirectToAction("Index");
        }
    }
}