using System;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

namespace NoteTakingApp
{
    internal class Pro
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter your note:");
            string note = Console.ReadLine();

            if (!string.IsNullOrEmpty(note))
            {
                CreateNoteOnGoogleDrive(note);
            }
        }

        private static void CreateNoteOnGoogleDrive(string note)
        {
            var clientSecrets = new ClientSecrets
            {
                ClientId = "967136322635-ud83273ou7sumpuq3rl5fipke0prstvr.apps.googleusercontent.com",
                ClientSecret = "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"
            };

            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                clientSecrets,
                new[] { DriveService.Scope.Drive },
                "user",
                CancellationToken.None).Result;

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Note Taking App",
            });

            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = $"Note-{DateTime.Now.ToString("yyyy-MM-dd")}.txt",
                MimeType = "text/plain"
            };

            FilesResource.CreateMediaUpload request;

            using (var stream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(note)))
            {
                request = service.Files.Create(fileMetadata, stream, "text/plain");
                request.Fields = "id";
                request.Upload();
            }

            var file = request.ResponseBody;

            Console.WriteLine($"Note created on Google Drive. File ID: {file.Id}");
        }
    }
}