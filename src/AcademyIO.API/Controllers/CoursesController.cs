using AcademyIO.Core.Interfaces.Services;
using AcademyIO.ManagementCourses.Aplication.Commands;
using AcademyIO.ManagementCourses.Application.Commands;
using AcademyIO.ManagementCourses.Application.Queries;
using AcademyIO.ManagementCourses.Application.Queries.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaEducacao.Api.DTOs;
using System.Net;

//TO DO, FINALIZAR O CURSO gerar certificado se todas as aulas foram finalziadas

namespace AcademyIO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController(IMediator _mediator,
                                ICourseQuery courseQuery,
                                INotifier notifier) : MainController(notifier)
    {

        /// <summary>
        /// Retorna todos os cursos registrados
        /// </summary>
        /// <returns><see cref="IEnumerable{CourseViewModel}"/> Retorna uma lista de CourseViewModel</returns>
        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(typeof(IEnumerable<CourseViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CourseViewModel>>> GetAll()
        {
            var courses = await courseQuery.GetAll();
            return CustomResponse(courses);
        }

        /// <summary>
        /// Retorna curso referente ao Id do parametro
        /// </summary>
        /// <param name="id"></param>
        /// <returns><see cref="CourseViewModel"/>Retorna os dados do curso</returns>
        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(CourseViewModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CourseViewModel>>> GetById(Guid id)
        {
            var course = await courseQuery.GetById(id);
            return CustomResponse(course);
        }

        /// <summary>
        /// Cria um novo curso
        /// </summary>
        /// <param name="course"></param>
        /// <returns>Retorna que o curso foi criado, status 201</returns>
        [Authorize(Roles = "ADMIN")]
        [HttpPost("create")]
        [ProducesResponseType(typeof(CourseViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create(CourseViewModel course)
        {
            var command = new AddCourseCommand(course.Name, course.Description, UserId, course.Price);
            await _mediator.Send(command);

            return CustomResponse(HttpStatusCode.Created);
        }

        /// <summary>
        /// Faz o pagamento do curso referenciado nos parametro 
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="paymentViewModel"></param>
        /// <returns>Retorna que o pagamento foi feito, status 201</returns>
        [Authorize(Roles = "STUDENT")]
        [HttpPost("{courseId:guid}/make-payment")]
        public async Task<IActionResult> MakePayment(Guid courseId, [FromBody] PaymentViewModel paymentViewModel)
        {
            var command = new ValidatePaymentCourseCommand(courseId, UserId, paymentViewModel.CardName,
                                                        paymentViewModel.CardNumber, paymentViewModel.CardExpirationDate,
                                                        paymentViewModel.CardCVV);
            await _mediator.Send(command);

            return CustomResponse(HttpStatusCode.Created);
        }
    }
}