using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using App.Core.Data;
using App.Ads.Code.Security;
using App.Ads.Models;
using AutoMapper;
using App.Ads.Code.Helpers;

namespace App.Ads.Controllers
{
    public class ArticleController : BaseController
    {
        private AdsDBEntities db = new AdsDBEntities();

        // GET: Article
        public ActionResult Index()
        {
            var article = db.Articles.ToList();
            return View(article);
        }

        // GET: Article/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Article/Create
        [UserRoleAuthorize(Roles = "SuperAdmin, Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Article/Create
        [HttpPost]
        [UserRoleAuthorize(Roles = "SuperAdmin, Admin")]
        public ActionResult Create(CreateArticleModel model)
        {
            if (ModelState.IsValid)
            {
                Article article = Mapper.Map<CreateArticleModel, Article>(model);

                article.CreateBy = CurrentUser.CustomIdentity.UserId;
                article.CreateDate = DateTime.Now;

                db.Articles.Add(article);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Article/Edit/5
        [UserRoleAuthorize(Roles = "SuperAdmin, Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }

            CreateArticleModel model = Mapper.Map<Article, CreateArticleModel>(article);

            return View(model);
        }


        // POST: Article/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        [UserRoleAuthorize(Roles = "SuperAdmin, Admin")]
        public ActionResult Edit(CreateArticleModel model)
        {
            if (ModelState.IsValid)
            {
                Article article = Mapper.Map<CreateArticleModel, Article>(model);

                db.Entry(article).State = System.Data.Entity.EntityState.Modified;
                article.LastUpdate = DateTime.Now;
                article.UpdateBy = CurrentUser.CustomIdentity.UserId;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Detail(int id, string title)
        {
            Article article = this.db.Articles.Find(id);

            if (article == null)
            {
                return HttpNotFound();
            }

            string expectedTitle = article.Title.ToSeoUrl();
            string actualName = (title ?? "").ToLower();

            if (expectedTitle != actualName)
            {
                return RedirectToActionPermanent("Detail", "Article", new { id = article.Id, title = expectedTitle });
            }

           
            return View(article);
        }

        // GET: Article/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Article/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
