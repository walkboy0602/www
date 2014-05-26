using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Models
{
    public class S3AssetsModel
    {
        public string S3AccessKey { get; set; }
        public string S3SecretKey { get; set; }
        public string S3Bucket { get; set; }
        public string S3Env { get; set; }
        public string S3Url { get; set; }
        public string S3ServiceUrl { get; set; }
    }
}
