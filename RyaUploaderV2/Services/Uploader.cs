﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using RyaUploaderV2.Facade;
using Serilog;

namespace RyaUploaderV2.Services
{
    public interface IUploader
    {
        /// <summary>
        /// Uploads the last 8 matches to an online endpoint
        /// </summary>
        /// <param name="shareCodes">List of sharecodes you want to upload</param>
        /// <returns>status message</returns>
        Task<bool> UploadShareCodes(IEnumerable<string> shareCodes);
    }

    public class Uploader : IUploader
    {
        private readonly IHttpClient _client;
        
        public Uploader(IHttpClient client)
        {
            _client = client;
        }


        public async Task<bool> UploadShareCodes(IEnumerable<string> shareCodes)
        {
            if (shareCodes == null) return false;

            await Task.WhenAll(shareCodes.Select(UploadAsync)).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Try to upload a specific match to csgostats.gg
        /// </summary>
        /// <param name="shareCode"></param>
        private async Task UploadAsync(string shareCode)
        {
            try
            {
                if (string.IsNullOrEmpty(shareCode)) return;

                var form = new MultipartFormDataContent { { new StringContent(shareCode), "sharecode" } };

                var response = await _client.PostAsync("https://csgostats.gg/match/upload", form);

                response.EnsureSuccessStatusCode();

                Log.Information($"Uploaded: {shareCode}.");
            }
            catch (Exception exception)
            {
                Log.Error(exception, $"Could not upload: {shareCode}.");
            }
        }

    }
}
