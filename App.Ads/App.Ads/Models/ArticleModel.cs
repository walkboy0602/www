using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using App.Core.Data;
using System.Web.Mvc;

namespace App.Ads.Models
{
    //[MetadataTypeAttribute(typeof(Article.ArticleMetaData))]
    //public partial class Article 
    //{
    //     internal sealed class ArticleMetaData
    //     {
    //        [Required]
    //        public string Title { get; set; }
    //     }
    //}

    public class CreateArticleModel
    {
        public int Id { get; set; }
     
        [Required]
        public string Title { get; set; }
        public string InlineTitle { get; set; }
        
        [Required, AllowHtml]
        public string Content { get; set; }
        public int CreateBy { get; set; }
        public Nullable<int> UpdateBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> LastUpdate { get; set; }
        public bool IsActive { get; set; }
    }
}