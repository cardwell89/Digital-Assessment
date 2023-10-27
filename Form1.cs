using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;



using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;


using Google.Apis.Download;

namespace NoteTakingApp
{
    public partial class NoteTake : Form
    {
        DataTable notes = new DataTable();
        bool editing = false;
        public NoteTake()
        {
            InitializeComponent();
        }

        private void NoteTake_Load(object sender, EventArgs e)
        {
            notes.Columns.Add("Tittle");
            notes.Columns.Add("Note");

            previousNotes.DataSource = notes;


        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            tittleBox.Text = notes.Rows[previousNotes.CurrentCell.RowIndex].ItemArray[0].ToString();
            noteBox.Text = notes.Rows[previousNotes.CurrentCell.RowIndex].ItemArray[1].ToString();
            editing = true;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                notes.Rows[previousNotes.CurrentCell.RowIndex].Delete();
            }
            catch (Exception ex) { Console.WriteLine("Note is not valit"); }

        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (editing)
            {
                notes.Rows[previousNotes.CurrentCell.RowIndex]["Tittle"] = tittleBox.Text;
                notes.Rows[previousNotes.CurrentCell.RowIndex]["Note"] = noteBox.Text;


            }
            else
            {
                notes.Rows.Add(tittleBox.Text, noteBox.Text);
            }
            tittleBox.Text = "";
            noteBox.Text = "";
            editing = false;
        }




        private void newNoteButton_Click(object sender, EventArgs e)
        {
            tittleBox.Text = "";
            noteBox.Text = "";

        }

        private void previousNotes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            tittleBox.Text = notes.Rows[previousNotes.CurrentCell.RowIndex].ItemArray[0].ToString();
            noteBox.Text = notes.Rows[previousNotes.CurrentCell.RowIndex].ItemArray[1].ToString();
            editing = true;

        }

     

       


        }
    }


    namespace GoogleDriveApiIntegration
    {
        class Program
        {
            // If modifying these scopes, delete your previously saved credentials
            // at ~/.credentials/drive-dotnet-quickstart.json
            static string[] Scopes = { DriveService.Scope.DriveFile };
            static string ApplicationName = "NoteTakingApp";

            static void Main(string[] args)
            {
                UserCredential credential;

                using (var stream = new FileStream("967136322635-ud83273ou7sumpuq3rl5fipke0prstvr.apps.googleusercontent.com", FileMode.Open, FileAccess.Read))
                {
                    string credPath = "token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                    Console.WriteLine("Credential file saved to: " + credPath);
                }

                // Create Drive API service.
                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                // Upload a note file.
                string noteTitle = "Test Note";
                string noteText = "This is a test note for Google Drive API integration.";
                string filePath = "Note.txt";

                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    using (var streamWriter = new StreamWriter(fileStream))
                    {
                        streamWriter.Write(noteText);
                    }
                }

                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = noteTitle
                };
                FilesResource.CreateMediaUpload request;

                using (var fileStream = new FileStream(filePath, FileMode.Open))
                {
                    request = service.Files.Create(fileMetadata, fileStream, "text/plain");
                    request.Fields = "id";
                    request.Upload();
                }

                var file = request.ResponseBody;
                Console.WriteLine("File ID: " + file.Id);

                // Download a note file.
                var fileId = "FILE_ID";
                var requestDownload = service.Files.Get(fileId);
                using (var fileStream = new FileStream("Note_Download.txt", FileMode.Create, FileAccess.Write))
                {
                    requestDownload.MediaDownloader.ProgressChanged += (IDownloadProgress progress) =>
                    {
                        switch (progress.Status)
                        {
                            case DownloadStatus.Downloading:
                                {
                                    Console.WriteLine(progress.BytesDownloaded);
                                    break;
                                }
                            case DownloadStatus.Completed:
                                {
                                    Console.WriteLine("Download complete.");
                                    break;
                                }
                            case DownloadStatus.Failed:
                                {
                                    Console.WriteLine("Download failed.");
                                    break;
                                }
                        }
                    };
                    requestDownload.Download(fileStream);
                }

                // Delete a note file.
                service.Files.Delete(fileId).Execute();
                Console.WriteLine("Note deleted.");

                Console.Read();
            }
        
    





}
    }

    // Starting from here need some work

 



    // Inside here

   
    

    




    

