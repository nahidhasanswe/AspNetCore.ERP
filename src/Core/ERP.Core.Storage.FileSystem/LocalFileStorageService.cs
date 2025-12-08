using Microsoft.Extensions.Hosting;

namespace ERP.Core.Storage.FileSystem;

public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _basePath;

        public LocalFileStorageService(IHostEnvironment environment)
        {
            // Set a base path, e.g., "wwwroot/uploads" or a specific disk path
            _basePath = Path.Combine(environment.ContentRootPath, "Storage");
            Directory.CreateDirectory(_basePath); // Ensure the directory exists
        }

        private string GetFullPath(string fileName, string? containerName)
        {
            var folderPath = containerName is not null
                ? Path.Combine(_basePath, containerName)
                : _basePath;
            
            Directory.CreateDirectory(folderPath); // Ensure container directory exists
            
            return Path.Combine(folderPath, fileName);
        }

        public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string? containerName = null)
        {
            var fullPath = GetFullPath(fileName, containerName);

            await using (var outputStream = new FileStream(fullPath, FileMode.Create))
            {
                // Reset stream position if needed and copy content
                fileStream.Seek(0, SeekOrigin.Begin);
                await fileStream.CopyToAsync(outputStream);
            }

            // Return the path relative to the base directory or just the filename/key
            return Path.Combine(containerName ?? string.Empty, fileName).Replace('\\', '/');
        }

        public Task<Stream?> GetFileStreamAsync(string fileName, string? containerName = null)
        {
            var fullPath = GetFullPath(fileName, containerName);

            return !File.Exists(fullPath) ? Task.FromResult<Stream?>(null) :
                // FileStream must be disposed of by the consumer (e.g., the controller)
                Task.FromResult<Stream?>(new FileStream(fullPath, FileMode.Open, FileAccess.Read));
        }

        public Task<bool> DeleteFileAsync(string fileName, string? containerName = null)
        {
            var fullPath = GetFullPath(fileName, containerName);

            if (!File.Exists(fullPath)) 
                return Task.FromResult(true); // Treat as successful if file doesn't exist
            
            File.Delete(fullPath);
            return Task.FromResult(true);
        }
        
        // This is a simplified example. For public access, you'd need to configure Kestrel/IIS
        // or a static file middleware to serve files from the "Storage" folder.
        public string GetFileUrl(string fileName, string? containerName = null)
        {
            return $"/{containerName ?? "Storage"}/{fileName}"; 
        }
    }