using Pdf_Processor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pdf_Processor.Services.CategoryService
{
    public interface ICategoryService
    {
        public  Task<List<Category>> GetCategoriesAsync();
    }
}
