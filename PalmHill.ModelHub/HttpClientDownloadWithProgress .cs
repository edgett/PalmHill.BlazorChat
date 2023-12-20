using System.Diagnostics;

namespace PalmHill.ModelHub
{
    public class HttpClientDownloadWithProgress : IDisposable
    {
        private readonly string _downloadUrl;
        private readonly string _destinationFilePath;
        private readonly HttpClient _httpClient;

        // Delegate and event to include bandwidth.
        public delegate void ProgressChangedHandler(long? totalFileSize, long totalBytesDownloaded, double? progressPercentage, double speed);

        public event ProgressChangedHandler? ProgressChanged;

        public HttpClientDownloadWithProgress(string downloadUrl, string destinationFilePath)
        {
            _downloadUrl = downloadUrl;
            _destinationFilePath = destinationFilePath;
            _httpClient = new HttpClient { Timeout = TimeSpan.FromDays(1) };
        }

        public async Task StartDownload()
        {
            using (var response = await _httpClient.GetAsync(_downloadUrl, HttpCompletionOption.ResponseHeadersRead))
                await DownloadFileFromHttpResponseMessage(response);
        }

        private async Task DownloadFileFromHttpResponseMessage(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength;

            using (var contentStream = await response.Content.ReadAsStreamAsync())
                await ProcessContentStream(totalBytes, contentStream);
        }

        private async Task ProcessContentStream(long? totalDownloadSize, Stream contentStream)
        {
            var totalBytesRead = 0L;
            var readCount = 0L;
            var buffer = new byte[8192];
            var isMoreToRead = true;
            var stopwatch = new Stopwatch();

            using (var fileStream = new FileStream(_destinationFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
            {
                do
                {
                    stopwatch.Start();
                    var bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                    stopwatch.Stop();

                    if (bytesRead == 0)
                    {
                        isMoreToRead = false;
                        TriggerProgressChanged(totalDownloadSize, totalBytesRead, 0);
                        continue;
                    }

                    await fileStream.WriteAsync(buffer, 0, bytesRead);

                    totalBytesRead += bytesRead;
                    readCount += 1;

                    if (readCount % 100 == 0 || bytesRead < buffer.Length)
                    {
                        var speed = bytesRead / (stopwatch.ElapsedMilliseconds / 1000.0); // Speed in bytes/sec
                        TriggerProgressChanged(totalDownloadSize, totalBytesRead, speed);
                    }

                    stopwatch.Reset();
                }
                while (isMoreToRead);
            }
        }

        private void TriggerProgressChanged(long? totalDownloadSize, long totalBytesRead, double speed)
        {
            if (ProgressChanged == null)
                return;

            double? progressPercentage = null;
            if (totalDownloadSize.HasValue)
                progressPercentage = Math.Round((double)totalBytesRead / totalDownloadSize.Value * 100, 2);

            ProgressChanged(totalDownloadSize, totalBytesRead, progressPercentage, speed);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }

}
