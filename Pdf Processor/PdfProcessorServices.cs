
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Pdf_Processor.Services.CategoryService;
using Pdf_Processor.Services.PdfService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pdf_Processor
{
    public static class PdfProcessorServices
    {
        public static IServiceCollection AddPdfProcessorServicesSettings(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddTransient<IPdfService, PdfService>();
            services.AddTransient<ICategoryService, CategoryService>(); 


            return services;
        }
    }
}
