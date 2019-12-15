using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MMSystem.Models
{
    public class Image
    {
        // POST: api/image
        [HttpPost]
        public void Post(byte[] file, string filename)
        {
            // Don't trust the file name sent by the client. Use
            // Path.GetRandomFileName to generate a safe random
            // file name. _targetFilePath receives a value
            // from configuration (the appsettings.json file in
            // the sample app).
            var trustedFileName = filename;//Path.GetRandomFileName();
            var filePath = Path.Combine("Image location here", trustedFileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                return;
            }

            System.IO.File.WriteAllBytes(filePath, file);
        }

    }
}
