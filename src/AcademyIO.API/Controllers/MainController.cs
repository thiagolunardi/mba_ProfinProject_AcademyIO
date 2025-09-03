using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using AcademyIO.Core.Interfaces.Services;
using AcademyIO.Core.Notifications;
using System.Security.Claims;

namespace AcademyIO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public abstract class MainController(INotifier notifier) : ControllerBase
    {
        private readonly INotifier _notifier = notifier;

        public Guid UserId => Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);

        protected bool IsValid()
        {
            return !_notifier.HasNotification();
        }

        protected ActionResult CustomResponse(object result = null)
        {
            if (IsValid())
            {
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }

            return BadRequest(new
            {
                success = false,
                errors = _notifier.GetNotifications().Select(n => n.Message)
            });
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            if (!modelState.IsValid) NotifieErrorInvalidModel(modelState);
            return CustomResponse();
        }

        protected void NotifieErrorInvalidModel(ModelStateDictionary modelState)
        {
            var erros = modelState.Values.SelectMany(e => e.Errors);
            foreach (var erro in erros)
            {
                var errorMsg = erro.Exception == null ? erro.ErrorMessage : erro.Exception.Message;
                NotifieError(errorMsg);
            }
        }

        protected void NotifieError(string message)
        {
            _notifier.Handle(new Notification(message));
        }
    }
}
