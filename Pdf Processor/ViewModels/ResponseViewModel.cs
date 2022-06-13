using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pdf_Processor.ViewModels
{
    public class ResponseViewModel<T>
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string StatusCode { get; set; }
        public T Data { get; set; }
    }
}
