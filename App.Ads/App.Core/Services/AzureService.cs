using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.Models;
using System.Configuration;
using System.IO;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace App.Core.Services
{
    public interface IAzureService
    {
        void UploadImage(string filePath, byte[] byteArray);
        bool DeleteImage(string filePath);
    }

    public class AzureService : IAzureService
    {
        private AzureStorageModel blob = new AzureStorageModel();

        public AzureService()
        {
            this.blob.AccountName = ConfigurationManager.AppSettings["StorageAccountName"].ToString();
            this.blob.AccessKey = ConfigurationManager.AppSettings["StorageAccountAccessKey"].ToString();
            this.blob.EndPoint = new Uri(string.Format("https://{0}.blob.core.windows.net", blob.AccountName));
        }

        void IAzureService.UploadImage(string filePath, byte[] byteArray)
        {
            string containerName = "images";

            CloudBlobClient blobClient = new CloudBlobClient(blob.EndPoint, new StorageCredentials(blob.AccountName, blob.AccessKey));
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            // create container if not exists
            container.CreateIfNotExists();

            // Enable anonymous read access to BLOBs.
            container.SetPermissions(
                new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });

            // Upload image to Blob Storage
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(filePath);
            blockBlob.Properties.ContentType = "image/jpeg";

            //blockBlob.Properties.ContentType = photoToUpload.ContentType;

            using (MemoryStream memoryStream = new MemoryStream(byteArray))
            {
                blockBlob.UploadFromStream(memoryStream);
                memoryStream.Flush();
            }
        }

        bool IAzureService.DeleteImage(string filePath)
        {
            string containerName = "images";
            CloudBlobClient blobClient = new CloudBlobClient(blob.EndPoint, new StorageCredentials(blob.AccountName, blob.AccessKey));
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            // Retrieve reference to a blob named "myblob.txt".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(filePath);

            // Delete the blob.
            try
            {
                blockBlob.Delete();
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
