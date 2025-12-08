namespace ERP.Core.Storage;

public interface IFileStorageService
{
    /// <summary>
    /// Saves a file stream to the specified path/key.
    /// </summary>
    /// <param name="fileStream">The stream of the file content.</param>
    /// <param name="fileName">The desired name/key for the file (including any virtual path).</param>
    /// <param name="containerName">The container/bucket name (optional).</param>
    /// <returns>A unique identifier or URL for the stored file.</returns>
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string? containerName = null);

    /// <summary>
    /// Retrieves a file stream from the specified path/key.
    /// </summary>
    /// <param name="fileName">The name/key of the file.</param>
    /// <param name="containerName">The container/bucket name (optional).</param>
    /// <returns>The file content stream, or null if the file is not found.</returns>
    Task<Stream?> GetFileStreamAsync(string fileName, string? containerName = null);

    /// <summary>
    /// Deletes a file from the specified path/key.
    /// </summary>
    /// <param name="fileName">The name/key of the file to delete.</param>
    /// <param name="containerName">The container/bucket name (optional).</param>
    /// <returns>True if the file was deleted or did not exist, false otherwise.</returns>
    Task<bool> DeleteFileAsync(string fileName, string? containerName = null);

    /// <summary>
    /// Gets the public URL for the stored file (if applicable).
    /// </summary>
    /// <param name="fileName">The name/key of the file.</param>
    /// <param name="containerName">The container/bucket name (optional).</param>
    /// <returns>The public URL as a string.</returns>
    string GetFileUrl(string fileName, string? containerName = null);
}