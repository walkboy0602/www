using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Models
{
    public class AzureStorageModel
    {
        public string AccountName { get; set; }
        public string AccessKey { get; set; }
        public Uri EndPoint { get; set; }
        public string ConnectionString { get; set; }


    }
}
