using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureMultipartFormDataStreamProvider
{
    public class AzureMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        private readonly CloudBlobContainer _blobContainer;

        public AzureMultipartFormDataStreamProvider(string containerName) : base("azureBlobStorage")
        {
            var storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            var blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            _blobContainer = blobClient.GetContainerReference(containerName);
        }

        public override Stream GetStream(HttpContent parent, HttpContentHeaders headers)
        {
            if (parent == null || headers == null)
                throw new ArgumentException();

            //Check to see if we are reading a file, if it is other form data return emtpy stream
            if (string.IsNullOrEmpty(headers.ContentDisposition.FileName))
                return new MemoryStream();

            var filename = Guid.NewGuid().ToString();

            var blob = _blobContainer.GetBlockBlobReference(filename);
            blob.Properties.ContentType = headers.ContentType?.MediaType;

            FileData.Add(new MultipartFileData(headers, blob.Name));

            return blob.OpenWrite();
        }
    }
}