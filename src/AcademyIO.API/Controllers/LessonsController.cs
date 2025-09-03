﻿using AcademyIO.Core.Enums;
using AcademyIO.Core.Interfaces.Services;
using AcademyIO.ManagementCourses.Application.Commands;
using AcademyIO.ManagementCourses.Application.Queries;
using AcademyIO.ManagementCourses.Application.Queries.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AcademyIO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonsController(IMediator _mediator,
                                ILessonQuery lessonQuery,
                                INotifier notifier) : MainController(notifier)
    {
        /// <summary>
        /// Retorna todas as aulas registradas
        /// </summary>
        /// <returns><see cref="IEnumerable{LessonViewModel}"/>Retorna uma lista de aulas e suas informações</returns>
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LessonViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LessonViewModel>>> GetAll()
        {
            var lessons = await lessonQuery.GetAll();
            return CustomResponse(lessons);
        }

        /// <summary>
        /// Retorna aulas referentes ao CourseId do parametro
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns><see cref="IEnumerable{LessonViewModel}"/>Retorna uma lista de aulas e suas informações</returns>
        [AllowAnonymous]
        [HttpGet("get-by-courseId")]
        [ProducesResponseType(typeof(IEnumerable<LessonViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LessonViewModel>>> GetByCourseId([FromQuery] Guid courseId)
        {
            var lessons = await lessonQuery.GetByCourseId(courseId);
            return CustomResponse(lessons);
        }

        /// <summary>
        /// Retorna o progresso do aluno dentro das aulas
        /// </summary>
        /// <returns><see cref="IEnumerable{LessonProgressViewModel}"/>Retorna uma lista de progresso de aulas</returns>
        [Authorize(Roles = "STUDENT")]
        [HttpGet("get-progress")]
        [ProducesResponseType(typeof(IEnumerable<LessonProgressViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProgress()
        {
            var progress = await lessonQuery.GetProgress(UserId);

            return CustomResponse(progress);
        }

        /// <summary>
        /// Recebe um modelo de aula para ser adicionada
        /// </summary>
        /// <param name="lesson"></param>
        /// <returns>Retorna sucesso 201</returns>
        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ProducesResponseType(typeof(LessonViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Add(LessonViewModel lesson)
        {
            var command = new AddLessonCommand(lesson.Name, lesson.Subject, lesson.CourseId, lesson.TotalHours);
            await _mediator.Send(command);

            return CustomResponse(HttpStatusCode.Created); ;
        }

        /// <summary>
        /// Registra que o aluno iniciou a aula
        /// </summary>
        /// <param name="lessonId"></param>
        /// <returns>Se aluno estiver matriculdo e nao concluiu, sucesso 204</returns>
        [Authorize(Roles = "STUDENT")]
        [HttpPost("{lessonId:guid}/start-class")]
        [ProducesResponseType(typeof(CourseViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> StartClass(Guid lessonId)
        {
            if (!lessonQuery.ExistsProgress(lessonId, UserId))
                return NotFound("Você ainda não está matriculado a essa aula.");

            var status = lessonQuery.GetProgressStatusLesson(lessonId, UserId);

            if (status == EProgressLesson.Completed)
                return NotFound("Você já concluiu essa aula.");

            var command = new StartLessonCommand(lessonId, UserId);
            await _mediator.Send(command);

            return CustomResponse(HttpStatusCode.NoContent); ;
        }

        /// <summary>
        /// Finaliza aula
        /// </summary>
        /// <param name="lessonId"></param>
        /// <returns>Se o aluno já se matriculou e iniciou, retorna sucesso 204</returns>
        [Authorize(Roles = "STUDENT")]
        [HttpPost("{lessonId:guid}/finish-class")]
        [ProducesResponseType(typeof(CourseViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> FinishClass(Guid lessonId)
        {
            if (!lessonQuery.ExistsProgress(lessonId, UserId))
                return NotFound("Você ainda não está matriculado a essa aula.");

            var status = lessonQuery.GetProgressStatusLesson(lessonId, UserId);

            if (status == EProgressLesson.NotStarted)
                return NotFound("Você ainda não teve progresso nesta aula.");

            var command = new FinishLessonCommand(lessonId, UserId);
            await _mediator.Send(command);

            return CustomResponse(HttpStatusCode.NoContent); ;
        }
    }
}
