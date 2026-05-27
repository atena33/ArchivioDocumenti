namespace ArchivioDocumenti.Middlewares
{
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient; // Fondamentale per riconoscere gli errori di SQL Server
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

    
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Si è verificato un errore non gestito nel sistema: {Message}", exception.Message);

            var dettagliErrore = new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError, 
                Title = "Errore Interno del Server",
                Instance = httpContext.Request.Path 
            };

            if (exception is SqlException sqlException)
            {
                dettagliErrore.Detail = "Impossibile comunicare con il database o completare l'operazione richiesta. Riprovare più tardi.";

            }
            else
            {
                dettagliErrore.Detail = "Si è verificato un errore imprevisto durante l'elaborazione della richiesta.";
            }

            httpContext.Response.StatusCode = dettagliErrore.Status.Value;
            httpContext.Response.ContentType = "application/problem+json"; 

            await httpContext.Response.WriteAsJsonAsync(dettagliErrore, cancellationToken);

            return true;
        }
    }
}

