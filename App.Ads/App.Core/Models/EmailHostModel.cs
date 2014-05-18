using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Core.Services;

namespace App.Core.Models
{
    public class EmailHostModel
    {
        //private readonly IConfigService configService;

        //public EmailHostModel(IConfigService configService)
        //{
        //    this.configService = configService;
        //}

        public string EmailHost { get; set; }
        public string EmailUserName { get; set; }
        public string EmailPassword { get; set; }
        public string EmailPort { get; set; }
        public string EmailEnableSSL { get; set; }
        public string EmailFrom { get; set; }

        //public string EmailHost
        //{
        //    get
        //    {
        //        return this.configService.GetValue(ConfigName.EmailHost);
        //    }
        //}

        //public string EmailUsername
        //{
        //    get
        //    {
        //        return this.configService.GetValue(ConfigName.EmailUsername);
        //    }
        //}

        //public string EmailPassword
        //{
        //    get
        //    {
        //        return this.configService.GetValue(ConfigName.EmailPassword);
        //    }
        //}

        //public string EmailPort
        //{
        //    get
        //    {
        //        return this.configService.GetValue(ConfigName.EmailPort);
        //    }
        //}

        //public string EmailEnableSSL
        //{
        //    get
        //    {
        //        return this.configService.GetValue(ConfigName.EmailEnableSSL);
        //    }
        //}

        //public string EmailFrom
        //{
        //    get
        //    {
        //        return this.configService.GetValue(ConfigName.EmailFrom);
        //    }
        //}

    }
}
