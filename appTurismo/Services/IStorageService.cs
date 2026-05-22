using System;
using System.Collections.Generic;
using System.Text;

namespace appTurismo.Services
{
    public interface IStorageService
    {
        Task UploadFileAsync(string localFilePath, string fileName, string remoteFilePath);
        Task UpdateFileAsync(string localFilePath, string remoteFilePath, string bucketName);
        Task<byte[]> DownloadFileAsync(string remoteFilePath, string bucketName);
        Task DeleteFileAsync(List<string> fileItems, string supabaseBucket);
    }
}
