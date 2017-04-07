# AzureMultipartFormDataStreamProvider
Store uploaded files to azure blob storage

Usage

        [HttpPost]
        [Route("api/media")]
        public async Task<IHttpActionResult> UploadFile()
        {
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var provider = new AzureMultipartFormDataStreamProvider("Container-Name");
            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);
                foreach (MultipartFileData file in provider.FileData)
                {
                  Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                  Trace.WriteLine("Server file path: " + file.LocalFileName);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error has occured. Details: {ex.Message}");
            }

            var filename = provider.FileData.FirstOrDefault()?.LocalFileName;
            if (string.IsNullOrEmpty(filename))
            {
                return BadRequest("An error has occured while uploading your file. Please try again.");
            }
            return Ok($"File: {filename} has successfully uploaded");
        }
