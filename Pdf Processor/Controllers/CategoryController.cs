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
                CategoryId = item.CategoryId
            };
            this.context.CategoryItems.Add(obj);

            await this.context.SaveChangesAsync();

            return obj;
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

        [HttpDelete("[action]/{id}")]
        public async Task<object> DeleteCategoryItem(int id)
        {
            var categoryItem = await this.context.CategoryItems.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (categoryItem != null)
            {
                this.context.Entry(categoryItem).State = EntityState.Deleted;

                await this.context.SaveChangesAsync();
                return new { Status = true };
            }

            return new { Status = false };
        }
    }
}
