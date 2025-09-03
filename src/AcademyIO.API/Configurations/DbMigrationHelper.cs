using AcademyIO.ManagementCourses.Data;
using AcademyIO.ManagementCourses.Domain;
using AcademyIO.ManagementStudents.Data;
using AcademyIO.ManagementStudents.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace AcademyIO.API.Configurations
{
    [ExcludeFromCodeCoverage]
    public static class DbMigrationHelperExtension
    {
        public static void UseDbMigrationHelper(this WebApplication app)
        {
            DbMigrationHelper.EnsureSeedData(app).Wait();
        }
    }

    [ExcludeFromCodeCoverage]
    public static class DbMigrationHelper
    {
        public static async Task EnsureSeedData(WebApplication application)
        {
            var services = application.Services.CreateScope().ServiceProvider;
            await EnsureSeedData(services);
        }

        public static async Task EnsureSeedData(IServiceProvider serviceProvider)
        {
            var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

            var studentsContext = scope.ServiceProvider.GetRequiredService<StudentsContext>();
            var courseContext = scope.ServiceProvider.GetRequiredService<CoursesContext>();

            if (env.IsDevelopment())
            {
                await studentsContext.Database.MigrateAsync();
                await courseContext.Database.MigrateAsync();
                await EnsureSeedData(studentsContext, courseContext);
            }
        }

        private static async Task EnsureSeedData(StudentsContext studentsContext, CoursesContext courseContext)
        {
            await SeedUsers(studentsContext);
            await SeedCourses(courseContext);
            await SeedLessons(courseContext);
            await SeedCertifications(studentsContext, courseContext);
        }

        public static async Task SeedUsers(StudentsContext context)
        {
            if (context.Users.Any()) return;

            #region ADMIN SEED
            var ADMIN_ROLE_ID = Guid.NewGuid();
            await context.Roles.AddAsync(new IdentityRole<Guid>
            {
                Name = "ADMIN",
                NormalizedName = "ADMIN",
                Id = ADMIN_ROLE_ID,
                ConcurrencyStamp = ADMIN_ROLE_ID.ToString()
            });

            var STUDENT_ROLE_ID = Guid.NewGuid();
            await context.Roles.AddAsync(new IdentityRole<Guid>
            {
                Name = "STUDENT",
                NormalizedName = "STUDENT",
                Id = STUDENT_ROLE_ID,
                ConcurrencyStamp = STUDENT_ROLE_ID.ToString()
            });

            var ADMIN_ID = Guid.NewGuid();
            var adminUser = new IdentityUser<Guid>
            {
                Id = ADMIN_ID,
                Email = "admin@AcademyIO.com",
                EmailConfirmed = true,
                UserName = "admin@AcademyIO.com",
                NormalizedUserName = "admin@AcademyIO.com".ToUpper(),
                NormalizedEmail = "admin@AcademyIO.com".ToUpper(),
                LockoutEnabled = true,
                SecurityStamp = ADMIN_ROLE_ID.ToString(),
            };

            //set user password
            PasswordHasher<IdentityUser<Guid>> ph = new PasswordHasher<IdentityUser<Guid>>();
            adminUser.PasswordHash = ph.HashPassword(adminUser, "Teste@123");
            await context.Users.AddAsync(adminUser);

            var user = new User(adminUser.Id, adminUser.UserName, "Admin", "Admin", adminUser.Email, DateTime.Now.AddYears(-20), true);
            await context.SystemUsers.AddAsync(user);

            await context.UserRoles.AddAsync(new IdentityUserRole<Guid>
            {
                RoleId = ADMIN_ROLE_ID,
                UserId = ADMIN_ID
            });

            context.SaveChanges();
            #endregion

            #region NON-ADMIN USERS SEED
            var user1Id = Guid.NewGuid();
            var user1 = new IdentityUser<Guid>
            {
                Id = user1Id,
                Email = "user1@AcademyIO.com",
                EmailConfirmed = true,
                UserName = "user1@AcademyIO.com",
                NormalizedUserName = "user1@AcademyIO.com".ToUpper(),
                NormalizedEmail = "user1@AcademyIO.com".ToUpper(),
                LockoutEnabled = true,
                SecurityStamp = user1Id.ToString(),
            };
            user1.PasswordHash = ph.HashPassword(user1, "Teste@123");
            await context.Users.AddAsync(user1);

            var systemUser1 = new User(user1.Id, user1.UserName, "User1", "User1", user1.Email, DateTime.Now.AddYears(21), false);
            await context.SystemUsers.AddAsync(systemUser1);

            var user2Id = Guid.NewGuid();
            var user2 = new IdentityUser<Guid>
            {
                Id = user2Id,
                Email = "user2@AcademyIO.com",
                EmailConfirmed = true,
                UserName = "user2@AcademyIO.com",
                NormalizedUserName = "user2@AcademyIO.com".ToUpper(),
                NormalizedEmail = "user2@AcademyIO.com".ToUpper(),
                LockoutEnabled = true,
                SecurityStamp = user2Id.ToString(),
            };
            user2.PasswordHash = ph.HashPassword(user2, "Teste@123");
            await context.Users.AddAsync(user2);

            await context.UserRoles.AddAsync(new IdentityUserRole<Guid>
            {
                RoleId = STUDENT_ROLE_ID,
                UserId = user1Id
            });

            await context.UserRoles.AddAsync(new IdentityUserRole<Guid>
            {
                RoleId = STUDENT_ROLE_ID,
                UserId = user2Id
            });


            var systemUser2 = new User(user2.Id, user2.UserName, "User2", "User2", user2.Email, DateTime.Now.AddYears(-22), false);
            await context.SystemUsers.AddAsync(systemUser2);

            context.SaveChanges();
            #endregion
        }

        public static async Task SeedCertifications(StudentsContext studentContext, CoursesContext courseContext)
        {
            if (!studentContext.Certifications.Any())
            {
                var user1 = studentContext.SystemUsers.FirstOrDefault(u => u.UserName.Equals("user1@AcademyIO.com"));
                var user2 = studentContext.SystemUsers.FirstOrDefault(u => u.UserName.Equals("user2@AcademyIO.com"));
                var courseDominios = courseContext.Courses.FirstOrDefault(u => u.Name.Equals("Modelagem de Dominios Ricos"));
                var courseTests = courseContext.Courses.FirstOrDefault(u => u.Name.Equals("Dominando Testes de Software"));

                var certifications = new List<Certification>
                {
                    new()
                    {
                        CourseId = courseDominios.Id,
                        StudentId = user1.Id,
                        CreatedDate = DateTime.Now,
                        Deleted = false,
                        UpdatedDate = DateTime.Now
                    },
                    new()
                    {
                        CourseId = courseTests.Id,
                        StudentId = user2.Id,
                        CreatedDate = DateTime.Now,
                        Deleted = false,
                        UpdatedDate = DateTime.Now
                    },
                };

                await studentContext.Certifications.AddRangeAsync(certifications);
                studentContext.SaveChanges();
            }
        }

        public static async Task SeedCourses(CoursesContext context)
        {
            if (!context.Courses.Any())
            {
                var courses = new List<Course>
                {
                    new()
                    {
                        Name = "Modelagem de Dominios Ricos",
                        CreatedDate = DateTime.Now,
                        Deleted = false,
                        UpdatedDate = DateTime.Now,
                        Description = "Este e um curso sobre modelagem de dominios ricos",
                        Price = 500
                    },
                   new()
                    {
                        Name = "Dominando Testes de Software",
                        CreatedDate = DateTime.Now,
                        Deleted = false,
                        UpdatedDate = DateTime.Now,
                        Description = "Este e um curso sobre testes de software",
                        Price = 350
                    },
                };

                await context.Courses.AddRangeAsync(courses);
                context.SaveChanges();
            }
        }

        public static async Task SeedLessons(CoursesContext context)
        {
            if (!context.Lessons.Any())
            {
                var courseDominios = context.Courses.FirstOrDefault(u => u.Name.Equals("Modelagem de Dominios Ricos"));
                var courseTests = context.Courses.FirstOrDefault(u => u.Name.Equals("Dominando Testes de Software"));
                if (courseDominios != null)
                {
                    var lessons = new List<Lesson>
                    {
                        new("Lesson 1", "Aula de dominios ricos 1", 80, courseDominios.Id),
                       new("Lesson 2", "Aulas de dominios ricos 2", 75, courseDominios.Id),
                    };

                    await context.Lessons.AddRangeAsync(lessons);
                    context.SaveChanges();
                }

                if (courseTests != null)
                {
                    var lessons = new List<Lesson>
                    {
                       new("Lesson 1", "Aula de testes 1", 60, courseTests.Id),
                       new("Lesson 2", "Aula de testes 2", 55, courseTests.Id)
                    };

                    await context.Lessons.AddRangeAsync(lessons);
                    context.SaveChanges();
                }
            }
        }
    }
}