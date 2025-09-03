using AcademyIO.API.Extensions;
using AcademyIO.Core.Interfaces;
using AcademyIO.Core.Interfaces.Repositories;
using AcademyIO.Core.Interfaces.Services;
using AcademyIO.Core.Messages.IntegrationCommands;
using AcademyIO.Core.Notifications;
using AcademyIO.ManagementCourses.Application.Commands;
using AcademyIO.ManagementCourses.Application.Queries;
using AcademyIO.ManagementCourses.Data.Repository;
using AcademyIO.ManagementPayments.AntiCorruption;
using AcademyIO.ManagementPayments.Application.Query;
using AcademyIO.ManagementPayments.Business;
using AcademyIO.ManagementPayments.Business.Handlers;
using AcademyIO.ManagementPayments.Data.Repository;
using AcademyIO.ManagementStudents.Application.Commands;
using AcademyIO.ManagementStudents.Application.Handler;
using AcademyIO.ManagementStudents.Data.Repository;
using AcademyIO.ManagementStudents.Domain;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AcademyIO.API.Configurations
{
    public static class ServicesConfiguration
    {
        public static WebApplicationBuilder AddRepositories(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ICourseRepository, CourseRepository>();
            builder.Services.AddScoped<ILessonRepository, LessonRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IRegistrationRepository, RegistrationRepository>();

            return builder;
        }

        public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
        {
            builder.Services.TryAddScoped<IAppUserService, AppUserService>();
            builder.Services.AddScoped<INotifier, Notifier>();

            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IPaymentCreditCardFacade, PaymentCreditCardFacade>();
            builder.Services.AddScoped<IPayPalGateway, PayPalGateway>();

            builder.Services.AddScoped<ICourseQuery, CourseQuery>();
            builder.Services.AddScoped<ILessonQuery, LessonQuery>();
            builder.Services.AddScoped<IPaymentQuery, PaymentQuery>();
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<AddUserCommand>());
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<AddLessonCommand>());
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<StartLessonCommand>());
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<FinishLessonCommand>());
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<AddCourseCommand>());
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateProgressByCourseCommand>());
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<PaymentCommandHandler>());
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<RegistrationCommandHandler>());

            builder.Services.AddHttpContextAccessor();

            return builder;
        }
    }
}
