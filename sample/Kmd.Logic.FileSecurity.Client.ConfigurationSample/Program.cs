using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Kmd.Logic.FileSecurity.Client.ServiceMessages;
using Kmd.Logic.Identity.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Kmd.Logic.FileSecurity.Client.ConfigurationSample
{
    /// <summary>
    /// Sample class to use the client.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main method to start the sample.
        /// </summary>
        /// <param name="args">Array of arguments.</param>
        public static async Task Main(string[] args)
        {
            InitLogger();

            try
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args)
                    .Build()
                    .Get<AppConfiguration>();

                await Run(config).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Caught a fatal unhandled exception");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void InitLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
        }

        private static async Task Run(AppConfiguration configuration)
        {
            var validator = new ConfigurationValidator(configuration);
            if (!validator.Validate())
            {
                return;
            }

            using (var httpClient = new HttpClient())
            using (var tokenProviderFactory = new LogicTokenProviderFactory(configuration.TokenProvider))
            {
                var fileSecurityClient = new FileSecurityClient(httpClient, tokenProviderFactory, configuration.FileSecurityOptions);
                var certificateId = configuration.CertificateDetails.CertificateId;

                var path = GetCertificateStream("kmd-df-key.p12");
                //var memoryStream = new MemoryStream();

                //path.Position = 0;
                //path.CopyTo(memoryStream);

                //var ms = new MemoryStream();
                //path.CopyTo(ms);

                //IFormFile file = new FormFile(ms, 0, ms.Length, "name", "kmd-df-key.p12");

                var value = new CertificateRequestDetails(Guid.NewGuid(), "Test", path, "Qwer123!");

                var certificateResponse = await fileSecurityClient.CreateCertificate(value).ConfigureAwait(false);


                Log.Information("Fetching certificate details for certificate id {CertificateId} ", configuration.CertificateDetails.CertificateId);
                var result = await fileSecurityClient.GetCertificate(certificateId).ConfigureAwait(false);

                if (result == null)
                {
                    Log.Error("Invalid certificate id {Id}", configuration.CertificateDetails.CertificateId);
                    return;
                }

                Console.WriteLine("Certificate ID: {0} \nCertificate Name: {1}\nSubscription ID : {2}", result.CertificateId, result.Name, result.SubscriptionId);
            }
        }

        private static Stream GetCertificateStream(string certificateFileName)
        {
            return File.OpenRead(Path.Combine(Directory.GetCurrentDirectory(), "Certificates", certificateFileName));
        }
    }
}