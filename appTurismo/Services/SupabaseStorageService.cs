using System;
using System.Collections.Generic;
using System.Text;

namespace appTurismo.Services
{
    public class SupabaseStorageService: IStorageService
    {
        private readonly Supabase.Client _supabaseClient;

        // Flag to track initialization state
        private bool _isInitialized = false;

        public SupabaseStorageService(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }

        public async Task UploadFileAsync(string localFullPath, string fileName, string supabaseBucket)
        {
            await _supabaseClient.Storage
              .From(supabaseBucket)
              .Upload(localFullPath, fileName);
        }

        public async Task UpdateFileAsync(string localFilePath, string supabaseFilePath, string supabaseBucket)
        {
            await _supabaseClient.Storage.From(supabaseBucket)
                .Update(localFilePath, supabaseFilePath);
        }

        public async Task DeleteFileAsync(List<string> fileItems, string supabaseBucket)
        {
            await _supabaseClient.Storage.From(supabaseBucket).Remove(fileItems);
        }

        public async Task<byte[]> DownloadFileAsync(string filePath, string supabaseBucket)
        {
            var bytes = await _supabaseClient.Storage.From(supabaseBucket).Download(filePath, null);
            return bytes;
        }

        // Initialize method to be called explicitly, can be awaited
        public async Task InitializeAsync()
        {
            if (!_isInitialized)
            {
                try
                {
                    await _supabaseClient.InitializeAsync();
                    _isInitialized = true; // Set initialization flag
                }
                catch (Exception ex)
                {
                    // Handle any initialization errors here
                    Console.WriteLine($"Initialization failed: {ex.Message}");
                    throw new InvalidOperationException("Failed to initialize Supabase client.", ex);
                }
            }
        }
    }
}
