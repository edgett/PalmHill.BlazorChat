using System.Net;

namespace PalmHill.ModelHub.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestDownloadeWithProgressBasic()
        {
            var url = "https://httpbin.org/ip";

            var destinationFilePathWebClient = "ip_webclient.json";
            var webClient = new WebClient();
            webClient.DownloadFile(url, destinationFilePathWebClient);
            webClient.Dispose();

            var webClientHash = FileHasher.ComputeSHA256Hash(destinationFilePathWebClient);
            Assert.IsNotNull(webClientHash);

            var destinationFilePathHttpClient = "ip_httpclient.json";
            var httpClient = new HttpClientDownloadWithProgress(url, destinationFilePathHttpClient);
            httpClient.StartDownload().GetAwaiter().GetResult();
            httpClient.Dispose();

            var httpClientHash = FileHasher.ComputeSHA256Hash(destinationFilePathHttpClient);
            Assert.IsNotNull(httpClientHash);

            Assert.AreEqual(webClientHash, httpClientHash);

            Assert.Pass();
        }


        [Test]
        public void TestDownloadeWithProgressEvents()
        {
            var url = "https://httpbin.org/ip";

            var destinationFilePathWebClient = "ip_webclient.json";
            var webClient = new WebClient();
            webClient.DownloadFile(url, destinationFilePathWebClient);
            webClient.Dispose();

            var webClientHash = FileHasher.ComputeSHA256Hash(destinationFilePathWebClient);
            Assert.IsNotNull(webClientHash);

            var destinationFilePathHttpClient = "ip_httpclient.json";
            var httpClient = new HttpClientDownloadWithProgress(url, destinationFilePathHttpClient);
            var lastProgressPercentage = 0.0;
            httpClient.ProgressChanged += (totalFileSize, totalBytesDownloaded, progressPercentage, speed) =>
            {
                lastProgressPercentage = progressPercentage ?? lastProgressPercentage;
                Console.WriteLine($"{progressPercentage}% ({speed} Mbps) downloaded");
            };
            httpClient.StartDownload().GetAwaiter().GetResult();
            httpClient.Dispose();

            var httpClientHash = FileHasher.ComputeSHA256Hash(destinationFilePathHttpClient);
            Assert.IsNotNull(httpClientHash);

            Assert.That(webClientHash, Is.EqualTo(httpClientHash));
            Assert.That(100.0, Is.EqualTo(lastProgressPercentage));

            Assert.Pass();
        }
    }
}