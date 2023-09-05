using OnlinePractice.API.Repository.Interfaces;
using System.Text.RegularExpressions;
using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Imagekit;

namespace OnlinePractice.API.Repository.Services
{
    public class AzureBolbRepository : IAzureBolbRepository
    {
        private readonly IConfiguration _configuration;

        //private static readonly string azureBlobContainer = ConfigurationManager.AppSettings["AzureBlobContainer"];
        //private static readonly string azureBlobURL = ConfigurationManager.AppSettings["AzureBlobBaseURL"];
        public AzureBolbRepository(IConfiguration configuration) { _configuration = configuration; }

        public string CreateBlob()
        {
            string connectionString = "your_connection_string";
            string containerName = "your_container_name";

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            containerClient.CreateIfNotExists();
            return string.Empty;
        }
        public async Task<string> UploadBlob()
        {
            string connectionString = "your_connection_string";
            string containerName = "your_container_name";
            string blobName = "your_blob_name";
            string filePath = "path_to_your_file";

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.UploadAsync(filePath);
            return string.Empty;
        }
        public async Task<string> DownloadBlob()
        {
            string connectionString = "your_connection_string";
            string containerName = "your_container_name";
            string blobName = "your_blob_name";
            string destinationPath = "path_to_save_downloaded_file";

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DownloadToAsync(destinationPath);
            return string.Empty;
        }
        public string ListBlobContainers()
        {

            string connectionString = "your_connection_string";
            string containerName = "your_container_name";

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            foreach (BlobItem blobItem in containerClient.GetBlobs())
            {
                Console.WriteLine(blobItem.Name);
            }
            return string.Empty.Trim();

        }
        public async Task<string> DeleteBlob()
        {
            string connectionString = "your_connection_string";
            string containerName = "your_container_name";
            string blobName = "your_blob_name";

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteAsync();
            return string.Empty.Trim();
        }
        //public string UploadFileOnAzure(byte[] byteArray, string fileName, string contentType)
        //{
        //    try
        //    {
        //        string azureBlobConnectionString = _configuration.GetValue<string>("Azure:AzureBlobConnectionString");
        //        // convert string to stream
        //        Stream stream = new MemoryStream(byteArray);
        //        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(azureBlobConnectionString);

        //        //Create the blob client.
        //        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
        //        //Retrieve a reference to a container.
        //        string azureBlobContainer = _configuration.GetValue<string>("Azure:AzureBlobConnectionString");

        //        CloudBlobContainer container = blobClient.GetContainerReference(azureBlobContainer);

        //        CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
        //        //string ext = fileName.Split('.')[1];
        //        blockBlob.Properties.ContentType = contentType;

        //        fileName = System.Web.HttpUtility.UrlEncode(fileName);
        //        blockBlob.Metadata["filename"] = fileName;
        //        blockBlob.UploadFromStreamAsync(stream);

        //        return blockBlob.Uri.AbsoluteUri;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}
        //public  string GetSASToken()
        //{
        //    try
        //    {
        //        string azureBlobConnectionString = _configuration.GetValue<string>("Azure:AzureBlobConnectionString")

        //        var storageAccount = CloudStorageAccount.Parse(azureBlobConnectionString);
        //        var blobClient = storageAccount.CreateCloudBlobClient();

        //        SharedAccessAccountPolicy policy = new SharedAccessAccountPolicy()
        //        {
        //            Permissions = SharedAccessAccountPermissions.Read,
        //            Services = SharedAccessAccountServices.Blob,
        //            ResourceTypes = SharedAccessAccountResourceTypes.Container | SharedAccessAccountResourceTypes.Object,
        //            SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(60),
        //            Protocols = SharedAccessProtocol.HttpsOnly,
        //        };
        //        return storageAccount.GetSharedAccessSignature(policy);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }

        //}

        //public static string GetCSVBlobData(string fileURL, string filename, string folderName)
        //{
        //    try
        //    {
        //        //Removing Special Characters from File Name
        //        Regex reg = new Regex("[*'\",_&#^@?]");
        //        filename = reg.Replace(filename, string.Empty);
        //        StringBuilder sb = new StringBuilder();
        //        //string pdfURL = string.Empty;
        //    //    string azureBlobConnectionString = _configuration.GetValue<string>("Azure:AzureBlobConnectionString")

        //        CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(azureBlobConnectionString);
        //        CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
        //        CloudBlobContainer cloudBlobContainer = blobClient.GetRootContainerReference();
        //        var blob = new CloudBlockBlob(new Uri(fileURL), cloudStorageAccount.Credentials);

        //        // Read content
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            //pdfURL = string.Format("~/SavePDF/{0}", folderName);
        //            sb = new StringBuilder(string.Format("~/SavePDF/{0}", folderName));
        //            blob.DownloadToStreamAsync(ms);
        //            //if (!Directory.Exists(System.Web.Hosting.HostingEnvironment.MapPath(sb.ToString())))
        //            //    Directory.CreateDirectory(System.Web.Hosting.HostingEnvironment.MapPath(sb.ToString()));
        //            //sb = new StringBuilder(string.Format("{0}/{1}.pdf", sb.ToString(), filename));
        //            //File.WriteAllBytes(System.Web.Hosting.HostingEnvironment.MapPath(sb.ToString()), ms.ToArray());
        //            sb = sb.Replace("~", "");
        //            //pdfURL = pdfURL.Replace("~", "");
        //        }
        //        return sb.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //}

        public void DeleteCSVBlobData(string fileURL)
        {
            //try
            //{
            //    string azureBlobConnectionString = _configuration.GetValue<string>("Azure:AzureBlobConnectionString")
            //    CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(azureBlobConnectionString);
            //    CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
            //    CloudBlobContainer cloudBlobContainer = blobClient.GetRootContainerReference();
            //    var blob = new CloudBlockBlob(new Uri(fileURL), cloudStorageAccount.Credentials);
            //    blob.DeleteIfExistsAsync();


            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}

        }

        // SDK initialization

}
}
