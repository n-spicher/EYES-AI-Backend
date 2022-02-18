﻿using Microsoft.EntityFrameworkCore;
using Pdf_Processor.Data;
using Pdf_Processor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pdf_Processor.Services.CategoryService
{
    public class CategoryService: ICategoryService
    {
        public readonly ApplicationDbContext context;
        public CategoryService(
            ApplicationDbContext context
            )
        {
            this.context = context;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            try
            {
                var categories = await this.context.Categories
                    .Include(x => x.CategoryItems)
                    .Include(x => x.CategoryCodes)
                    .ToListAsync();

                return categories;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
