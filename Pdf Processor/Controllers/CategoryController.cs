using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pdf_Processor.Data;
using Pdf_Processor.Models;
using Pdf_Processor.Services.CategoryService;
using Pdf_Processor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pdf_Processor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        public ApplicationDbContext context;
        public ICategoryService categoryService;
        public CategoryController(
            ApplicationDbContext context,
            ICategoryService categoryService
            )
        {
            this.context = context;
            this.categoryService = categoryService;
        }

        [HttpGet("[action]")]
        public async Task<List<Category>> GetCategories()
        {
            return await this.categoryService.GetCategoriesAsync();
        }

        [HttpPost("[action]")]
        public async Task<CategoryItem> AddCategoryItem(CategoryItemViewModel item)
        {
            var obj = new CategoryItem
            {
                Name = item.Name,
                Note = item.Note,
                CategoryId = item.CategoryId
            };
            this.context.CategoryItems.Add(obj);

            await this.context.SaveChangesAsync();

            var categorySyn = await this.context.CategoryItemSynonyms.Where(x => x.CategoryItemId == obj.Id).ToListAsync();

            this.context.RemoveRange(categorySyn);

            await this.context.SaveChangesAsync();

            if (item.CategoryItemSynonyms != null && item.CategoryItemSynonyms.Count > 0)
            {
                foreach (var s in item.CategoryItemSynonyms)
                {
                    var syn = new CategoryItemSynonym
                    {
                        CategoryItemId = obj.Id,
                        Name = s
                    };
                }

                await this.context.SaveChangesAsync();
            }

            return obj;
        }

        [HttpPost("[action]/{Id}")]
        public async Task<CategoryItem> UpdateCategoryItem(int Id, CategoryItemViewModel item)
        {
            var itemObj = await this.context.CategoryItems.Where(x => x.Id == Id).FirstOrDefaultAsync();

            if (item == null)
            {
                return null;
            }

            itemObj.Name = item.Name;
            itemObj.Note = item.Note;

            this.context.Entry(itemObj).Property(x => x.Name).IsModified = true;
            this.context.Entry(itemObj).Property(x => x.Note).IsModified = true;

            await this.context.SaveChangesAsync();
            
            return itemObj;
        }

        [HttpPost("[action]")]
        public async Task<List<CategoryItemSynonym>> AddCategoryItemSynonyms(List<CategoryItemSynonymViewModel> items)
        {
            List<CategoryItemSynonym> list = new List<CategoryItemSynonym>();

            foreach (var item in items)
            {
                var obj = new CategoryItemSynonym
                {
                    Name = item.Name,
                    CategoryItemId = item.CategoryItemId
                };

                this.context.CategoryItemSynonyms.Add(obj);

                await this.context.SaveChangesAsync();

                list.Add(obj);
            }

            return list;
        }

        [HttpPost("[action]")]
        public async Task<CategoryCode> AddCategoryCode(CategoryCodeViewModel item)
        {
            var obj = new CategoryCode
            {
                Code = item.Code,
                CategoryId = item.CategoryId
            };

            this.context.CategoryCodes.Add(obj);

            await this.context.SaveChangesAsync();

            return obj;
        }

        [HttpPost("[action]")]
        public async Task<Category> AddCategory(CategoryViewModel item)
        {
            var obj = new Category
            {
                Name = item.Name
            };

            this.context.Categories.Add(obj);

            await this.context.SaveChangesAsync();

            return obj;
        }

        [HttpPost("[action]")]
        public async Task<Category> UpdateCategory(CategoryViewModel category)
        {
            var cat = await this.context.Categories.Where(x => x.Id == category.Id).FirstOrDefaultAsync();

            if (cat == null)
            {
                return null;
            }

            cat.Name = category.Name;

            this.context.Entry(cat).Property(x => x.Name).IsModified = true;

            await this.context.SaveChangesAsync();

            return cat;
        }

        [HttpGet("[action]/{id}")]
        public async Task<object> DeleteCategory(int id)
        {
            var category = await this.context.Categories.Include(x => x.CategoryItems).Where(x => x.Id == id).FirstOrDefaultAsync();
            if (category != null)
            {
                if (category.CategoryItems != null)
                {
                    foreach (var item in category.CategoryItems)
                    {
                        await this.DeleteCategoryItem(item.Id);
                    }
                }
                category.CategoryItems = null;

                this.context.Remove(category);

                await this.context.SaveChangesAsync();
                return new { Status = true };
            }

            return new { Status = false };
        }

        [HttpGet("[action]/{id}")]
        public async Task<object> DeleteCategoryItem(int id)
        {
            var categoryItem = await this.context.CategoryItems.Include(x => x.CategoryItemSynonyms).Where(x => x.Id == id).FirstOrDefaultAsync();
            if (categoryItem != null)
            {
                if (categoryItem.CategoryItemSynonyms != null && categoryItem.CategoryItemSynonyms.Count > 0)
                {
                    this.context.RemoveRange(categoryItem.CategoryItemSynonyms);
                    await this.context.SaveChangesAsync();
                }


                categoryItem.CategoryItemSynonyms = null;

                this.context.Remove(categoryItem);

                await this.context.SaveChangesAsync();
                return new { Status = true };
            }

            return new { Status = false };
        }

        [HttpGet("[action]/{id}")]
        public async Task<object> DeleteCategoryItemSynonym(int id)
        {
            var categoryItem = await this.context.CategoryItemSynonyms.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (categoryItem != null)
            {
                this.context.Remove(categoryItem);

                await this.context.SaveChangesAsync();
                return new { Status = true };
            }

            return new { Status = false };
        }

        [HttpGet("[action]/{id}")]
        public async Task<object> DeleteCategoryCode(int id)
        {
            var categoryCode = await this.context.CategoryCodes.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (categoryCode != null)
            {
                this.context.Entry(categoryCode).State = EntityState.Deleted;

                await this.context.SaveChangesAsync();
                return new { Status = true };
            }

            return new { Status = false };
        }
    }
}
