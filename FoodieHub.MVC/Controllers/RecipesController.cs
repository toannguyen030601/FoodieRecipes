﻿using FoodieHub.MVC.Configurations;
using FoodieHub.MVC.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using FoodieHub.MVC.Models.Favorite;
using FoodieHub.MVC.Models.Recipe;
using FoodieHub.MVC.Helpers;
using FoodieHub.MVC.Models.Comment;
using FoodieHub.MVC.Models.QueryModel;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace FoodieHub.MVC.Controllers
{
    public class RecipesController : Controller
    {
        private readonly IRecipeService _recipeService;
        private readonly IFavoriteService _favoriteService;
        private readonly ICommentService _commentService;
        private readonly INotyfService _notyf;

        public RecipesController(IRecipeService recipeService, IFavoriteService favoriteService, ICommentService commentService, INotyfService notyf)
        {
            _recipeService = recipeService;
            _favoriteService = favoriteService;
            _commentService = commentService;
            _notyf = notyf;
        }
        [ValidateTokenForUser]
        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateRecipeDTO();
            return View(model);
        }
        [ValidateTokenForUser]
        [HttpPost]
        public async Task<IActionResult> Create(CreateRecipeDTO recipe)
        {
            if (ModelState.IsValid)
            {
                if (!recipe.Ingredients.Any() || !recipe.RecipeSteps.Any())
                {
                    /*NotificationHelper.SetErrorNotification(this,"List ingredient and step is required");*/
                    _notyf.Error("List ingredient and step is required");
                    return View();
                }
                bool result = await _recipeService.Create(recipe);
                if (result)
                {
                    /*NotificationHelper.SetSuccessNotification(this);*/
                    _notyf.Success("Create success");
                    return Redirect("/account/recipes");
                }
                else
                {
                    /*NotificationHelper.SetErrorNotification(this);*/
                    _notyf.Error("Create fail");
                }
            }         
            return View(recipe);
        }

        [ValidateTokenForUser]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _recipeService.GetByID(id);
            if(response != null)
            {
                var edit = new UpdateRecipeDTO
                {
                    RecipeID = id,
                    Title = response.Title,
                    CookTime = response.CookTime,
                    Serves = response.Serves,
                    Description = response.Description,
                    IsActive = response.IsActive,
                    ImageURL = response.ImageURL,
                    CategoryID= response.CategoryID,
                    RecipeSteps = response.Steps,
                    Ingredients = response.Ingredients,
                };
                return View(edit);
            }
            /*NotificationHelper.SetErrorNotification(this);*/
            _notyf.Error("Not found this recipe");
            return RedirectToAction("Recipes", "Account");
        }
        [ValidateTokenForUser]
        [HttpPost]
        public async Task<IActionResult> Edit(UpdateRecipeDTO update)
        {
            if (update.Ingredients.Count() == 0 || update.RecipeSteps.Count() == 0)
            {
                /*NotificationHelper.SetErrorNotification(this,"List step and ingredient is required");*/
                _notyf.Error("List step and ingredient is required");
                return View(update);
            }
            if (ModelState.IsValid)
            {
                var result = await _recipeService.Update(update);
                if (result)
                {
                    /*NotificationHelper.SetSuccessNotification(this);*/
                    _notyf.Success("Update success");
                    return RedirectToAction("Recipes","Account");
                }
                /*NotificationHelper.SetErrorNotification(this);*/
                _notyf.Error("Update fail");
            }
            return View(update);
        }


        public async Task<IActionResult> Index(QueryRecipeModel query)
        {
            query.IsActive = true;
            var recipes = await _recipeService.GetAll(query);
            ViewBag.Query = query;
            return View(recipes);
        }



        // Action hiển thị chi tiết bài viết
        public async Task<IActionResult> Detail(int id)
        {
            var data = await _recipeService.GetByID(id);
            if (data == null || !data.IsActive)
            {
                NotificationHelper.SetErrorNotification(this, "Not found this recipe");
                return RedirectToAction("Index");
            }

            // Lấy danh sách bài viết liên quan
            var relatedRecipesResult = await _recipeService.GetAll(new QueryRecipeModel
            {
                CategoryID = data.CategoryID,
                IsActive = true,
            });

            // Lọc các bài viết liên quan, loại trừ bài viết hiện tại
            var relatedRecipes = relatedRecipesResult.Items
                .Where(r => r.RecipeID != id)
                .Select(r => new
                {
                    r.RecipeID,
                    r.Title,
                    r.ImageURL,
                    r.CookTime,
                    r.CategoryName,
                    r.Avatar,
                    r.FullName,
                    r.RatingAverage,
                    r.TotalFavorites // Thêm thuộc tính TotalFavorites vào đây
                })
                .ToList();

            ViewBag.RelatedRecipes = relatedRecipes;
            ViewBag.UserID = Request.GetCookie("UserID");
            return View(data);
        }


        [ValidateTokenForUser]
        public async Task<IActionResult> Favorite(int id)
        {
            var newFavorite = new FavoriteDTO { RecipeID = id };
            bool result = await _favoriteService.Create(newFavorite);
            var message = result ? "Favorite success" : "Favorite fail";
            var notificationType = result ? "success" : "error";
            return Json(new { success = result, message, notificationType });
        }

        [ValidateTokenForUser]
        public async Task<IActionResult> UnFavorite(int id)
        {
            bool result = await _favoriteService.Delete(new FavoriteDTO { RecipeID = id });
            var message = result ? "UnFavorite success" : "UnFavorite fail";
            var notificationType = result ? "success" : "error";
            return Json(new { success = result, message, notificationType }); 
        }
        [ValidateTokenForUser]
        public async Task<IActionResult> Rating(int recipeID, int ratingValue)
        {
            var rating = new CreateRatingDTO
            {
                RecipeID = recipeID,
                RatingValue = ratingValue
            };
            bool result = await _recipeService.Rating(rating);
            if (result)
            {
                /*NotificationHelper.SetSuccessNotification(this);*/
                _notyf.Success("Rating success");
            }

            else
            {
                /*NotificationHelper.SetErrorNotification(this);*/
                _notyf.Error("Rating fail");
            }
            return Redirect("/Recipes/Detail/" + recipeID);
        }
        [ValidateTokenForUser]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _recipeService.Delete(id);
            if (result)
            {
                /*NotificationHelper.SetSuccessNotification(this);*/
                _notyf.Success("Delete success");
            }
            else
            {
                /*NotificationHelper.SetErrorNotification(this);*/
                _notyf.Error("Delete fail");
            }
            return RedirectToAction("Recipes", "Account");
        }
        [ValidateTokenForUser]
        public async Task<IActionResult> CreateComment(CommentDTO comment)
        {
            var avatarUrl = Request.Cookies["Avatar"];
            var userId = Request.Cookies["UserID"];
            var fullName = Request.Cookies["FullName"];
            var commentedAt = DateTime.Now;

            bool result = await _commentService.Create(comment);
            if (result)
            {
                return Json(new { success = true, message = "Comment submitted", fullName });
            }
            else
            {
                return Json(new { success = false, message = "Failed to submit comment" });
            }
        }
        [ValidateTokenForUser]
        public async Task<IActionResult> DeleteComment(int id,int recipeID)
        {
            bool result = await _commentService.Delete(id);
            if (result)
            {
                return Json(new { success = true, message = "Delete success" });
            }
            else
            {
                return Json(new { success = false, message = "Delete fail" });
            }
        }
        [ValidateTokenForUser]
        public async Task<IActionResult> EditComment(int CommentID,string CommentContent,int RecipeID)
        {
            bool result = await _commentService.Edit(CommentID,new CommentDTO
            {
                CommentContent = CommentContent
            });
            if (result) NotificationHelper.SetSuccessNotification(this);
            else NotificationHelper.SetErrorNotification(this);
            return RedirectToAction("Detail", new { id = RecipeID });
        }

        public async Task<IActionResult> GetCommentRecipe(int id)
        {
            var comments = await _commentService.GetCommentRecipe(id);
            return Json(comments);
        }
    }
}
