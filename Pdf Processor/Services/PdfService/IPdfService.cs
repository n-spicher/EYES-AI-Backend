using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pdf_Processor.Services.PdfService
{
    public interface IPdfService
    {
        public Task<object> Search(string fileName, string query);
    }
}
