using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdf_Processor.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Pdf_Processor.Helper.MyExceptionResponse;

namespace Pdf_Processor.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi =true)]
    public class ExceptionController : ControllerBase
    {
        [Route("exception")]
        public MyExceptionResponse Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context?.Error; // Your exception
            var code = 500; // Internal Server Error by default

            if (exception is HttpStatusException httpException)
            {
                code = (int)httpException.Status;
            }

            Response.StatusCode = code;

            return new MyExceptionResponse(exception); // Your error model
        }
    }
}
