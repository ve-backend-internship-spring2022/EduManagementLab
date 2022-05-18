using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Entities.client;
using EduManagementLab.Core.Interfaces;
using EduManagementLab.Core.Services;
using IdentityServer4.Models;
using IDS4Secret = IdentityServer4.Models.Secret;

namespace EduManagementLab.EfRepository
{
    public class DevTestData
    {
        public static void Seed(DataContext dataContext)
        {
            var unitOfWork = new UnitOfWork(dataContext);
            unitOfWork.Users.AddRange(GetDevTestUsers(unitOfWork).ToList());
            unitOfWork.Courses.AddRange(GetDevTestCourses().ToList());
            unitOfWork.OAuthClients.AddRange(GetClients().ToList());
            unitOfWork.Complete();
        }

        public static IEnumerable<User> GetDevTestUsers(IUnitOfWork unitOfWork)
        {
            UserService _userService = new UserService(unitOfWork);
            return new List<User>
            {
                new User() { Id = Guid.Parse("{106CD9D2-3065-47A8-AC34-85343037534A}"), UserName = "John123" , PasswordHash = _userService.GenerateHashPassword("John123!") , Displayname = "Mr John", FirstName = "John", LastName = "Doe", Email = "john.doe@myemail.test" },
                new User() { Id = Guid.Parse("{BF6DA545-C0A1-49FB-86AC-4BD6B890C3F5}"), UserName = "Anders123" , PasswordHash = _userService.GenerateHashPassword("Anders123!") , Displayname = "Anders S", FirstName = "Anders", LastName = "Svensson", Email = "anders.svensson@myemail.test" },
                new User() { Id = Guid.Parse("{22C4D8D0-8683-4116-B0FD-8C55B20E900D}"), UserName = "Danni123" , PasswordHash = _userService.GenerateHashPassword("Danni123!") , Displayname = "Danni L", FirstName = "Danni", LastName = "Lorenzo", Email = "dlorenzo2@privacy.gov.au" },
                new User() { Id = Guid.Parse("{BCCEC9A8-4531-46CA-9D3B-E98229622C11}"), UserName = "Genny123" , PasswordHash = _userService.GenerateHashPassword("Genny123!") , Displayname = "Genny H", FirstName = "Genny", LastName = "Headey", Email = "	gheadey3@google.com.br" },
                new User() { Id = Guid.Parse("{54915E7F-9CD1-40B6-AD9A-992F6A87A28A}"), UserName = "Connie123" , PasswordHash = _userService.GenerateHashPassword("Connie123!") , Displayname = "Connie A", FirstName = "Connie", LastName = "Annets", Email = "cannets0@adobe.com" },
                new User() { Id = Guid.Parse("{853EF2AB-C86A-4ECD-A1CD-68B6A15DFE02}"), UserName = "Nevile123" , PasswordHash = _userService.GenerateHashPassword("Nevile123!") , Displayname = "Nevile B", FirstName = "Nevile", LastName = "Brimming", Email = "nbrimming2@bigcartel.com" },
                new User() { Id = Guid.Parse("{F10C5A5B-51D2-4115-9421-6B4D6A6BA60F}"), UserName = "Bo123" , PasswordHash = _userService.GenerateHashPassword("Bo123!") , Displayname = "Bo B", FirstName = "Bo", LastName = "Brombell", Email = "bbrombell1@amazon.co.uk" },
                new User() { Id = Guid.Parse("{67A0843F-7262-4F05-BB4B-0045F5A6BA96}"), UserName = "Jillayne123" , PasswordHash = _userService.GenerateHashPassword("Jillayne123!") , Displayname = "Jillayne P", FirstName = "Jillayne", LastName = "Presnell", Email = "jpresnell5@godaddy.com" },
                new User() { Id = Guid.Parse("{9F60A30C-8BFA-46BB-9F3B-7D32E590FFBE}"), UserName = "Katrinka123" , PasswordHash = _userService.GenerateHashPassword("Katrinka123!") , Displayname = "Katrinka R", FirstName = "Katrinka", LastName = "Rupert", Email = "krupert9@wordpress.org" },
                new User() { Id = Guid.Parse("{A764A5A6-C67C-452E-81BF-11984FB538D3}"), UserName = "Seymour123" , PasswordHash = _userService.GenerateHashPassword("Seymour123!") , Displayname = "Seymour M", FirstName = "Seymour", LastName = "MacMechan", Email = "smacmechan0@dailymotion.com" }
            };
        }

        public static IEnumerable<Course> GetDevTestCourses()
        {


            return new List<Course>
            {
                new Course() { Id = Guid.Parse("{56E96EA1-90D5-47E9-A1FA-93073614B7FB}"), Code = "ECO1", Name = "Economy 1", Description = "Introduction course to economy", StartDate = DateTime.Now.AddDays(-60), EndDate = DateTime.Now.AddDays(-30), Memperships = new List<Course.Membership>() {
                    new Course.Membership() { Id = Guid.NewGuid(), CourseId = Guid.Parse("{56E96EA1-90D5-47E9-A1FA-93073614B7FB}"), UserId = Guid.Parse("{106CD9D2-3065-47A8-AC34-85343037534A}"), EnrolledDate = DateTime.Now.AddDays(-60) } } },

                new Course() { Id = Guid.Parse("{1BFE9361-8CEE-42A8-881D-28BAC263B6B9}"), Code = "ECO2", Name = "Economy 2", StartDate = DateTime.Now.AddDays(-29), EndDate = DateTime.Now.AddDays(40), Memperships = new List<Course.Membership>() {
                    new Course.Membership() { Id = Guid.NewGuid(), CourseId = Guid.Parse("{1BFE9361-8CEE-42A8-881D-28BAC263B6B9}"), UserId = Guid.Parse("{BF6DA545-C0A1-49FB-86AC-4BD6B890C3F5}"), EnrolledDate = DateTime.Now.AddDays(-29) } }  },

                new Course() { Id = Guid.Parse("{A94CC958-3777-4814-BCB7-5DB3F7A25D74}"), Code = "MAN1", Name = "Management 1", Description = "Introduction course to management", StartDate = DateTime.Now.AddDays(-25), EndDate = DateTime.Now.AddDays(30), Memperships = new List<Course.Membership>() {
                    new Course.Membership() { Id = Guid.NewGuid(), CourseId = Guid.Parse("{A94CC958-3777-4814-BCB7-5DB3F7A25D74}"), UserId = Guid.Parse("{22C4D8D0-8683-4116-B0FD-8C55B20E900D}"), EnrolledDate = DateTime.Now.AddDays(-25) } }  },

                new Course() { Id = Guid.Parse("{18B9CDAE-BED0-468A-9491-E2394212CA75}"), Code = "MAN2", Name = "Management 2", StartDate = DateTime.Now.AddDays(5), EndDate = DateTime.Now.AddDays(45), Memperships = new List<Course.Membership>() {
                    new Course.Membership() { Id = Guid.NewGuid(), CourseId = Guid.Parse("{18B9CDAE-BED0-468A-9491-E2394212CA75}"), UserId = Guid.Parse("{BCCEC9A8-4531-46CA-9D3B-E98229622C11}"), EnrolledDate = DateTime.Now.AddDays(5) } }  },

                new Course() { Id = Guid.Parse("{B2D5A544-5015-4821-8D68-7E3658DCFBA2}"), Code = "ACC1", Name = "Accounting 1", Description = "Introduction course to management", StartDate = DateTime.Now.AddDays(-34), EndDate = DateTime.Now.AddDays(83), Memperships = new List<Course.Membership>() {
                    new Course.Membership() { Id = Guid.NewGuid(), CourseId = Guid.Parse("{B2D5A544-5015-4821-8D68-7E3658DCFBA2}"), UserId = Guid.Parse("{54915E7F-9CD1-40B6-AD9A-992F6A87A28A}"), EnrolledDate = DateTime.Now.AddDays(-34) } }  },

                new Course() { Id = Guid.Parse("{88C51C50-4571-45AF-9381-B12B9E0845A7}"), Code = "ENT1", Name = "Entrepreneurship 1", Description = "Course to help entrepreneurs grow their business.", StartDate = DateTime.Now.AddDays(-14), EndDate = DateTime.Now.AddDays(22), Memperships = new List<Course.Membership>() {
                    new Course.Membership() { Id = Guid.NewGuid(), CourseId = Guid.Parse("{88C51C50-4571-45AF-9381-B12B9E0845A7}"), UserId = Guid.Parse("{853EF2AB-C86A-4ECD-A1CD-68B6A15DFE02}"), EnrolledDate = DateTime.Now.AddDays(-14) } }  },

                new Course() { Id = Guid.Parse("{B5C95752-B067-406E-9758-C701DCE58EA9}"), Code = "RED1", Name = "Real Estate Development 1", Description = "EDP-Sales and Marketing Management for Real Estate", StartDate = DateTime.Now.AddDays(-33), EndDate = DateTime.Now.AddDays(11), Memperships = new List<Course.Membership>() {
                    new Course.Membership() { Id = Guid.NewGuid(), CourseId = Guid.Parse("{B5C95752-B067-406E-9758-C701DCE58EA9}"), UserId = Guid.Parse("{F10C5A5B-51D2-4115-9421-6B4D6A6BA60F}"), EnrolledDate = DateTime.Now.AddDays(-33) } }  },

                new Course() { Id = Guid.Parse("{B9F20A7E-7362-4E49-ACC2-58C7F9D517B6}"), Code = "PRM1", Name = "Project Management 1", Description = "This course is the first in a series of six to equip you with the skills you need to apply to introductory-level roles in project management", StartDate = DateTime.Now.AddDays(-5), EndDate = DateTime.Now.AddDays(41), Memperships = new List<Course.Membership>() {
                    new Course.Membership() { Id = Guid.NewGuid(), CourseId = Guid.Parse("{B9F20A7E-7362-4E49-ACC2-58C7F9D517B6}"), UserId = Guid.Parse("{67A0843F-7262-4F05-BB4B-0045F5A6BA96}"), EnrolledDate = DateTime.Now.AddDays(-5) } }  },

                new Course() { Id = Guid.Parse("{BF641F31-9C3C-4690-9928-6A1F58FB49AF}"), Code = "AGT1", Name = "Agile Testing 1", Description = "ISTQB (BCS) Certified Tester Foundation", StartDate = DateTime.Now.AddDays(-29), EndDate = DateTime.Now.AddDays(12), Memperships = new List<Course.Membership>() {
                    new Course.Membership() { Id = Guid.NewGuid(), CourseId = Guid.Parse("{BF641F31-9C3C-4690-9928-6A1F58FB49AF}"), UserId = Guid.Parse("{9F60A30C-8BFA-46BB-9F3B-7D32E590FFBE}"), EnrolledDate = DateTime.Now.AddDays(-29) } }  },

                new Course() { Id = Guid.Parse("{C68EA325-9527-4343-934E-67D7998B7B16}"), Code = "TDD1", Name = "Test Driven Development 1", Description = "The fundamentals of Test-Driven Development", StartDate = DateTime.Now.AddDays(-24), EndDate = DateTime.Now.AddDays(31), Memperships = new List<Course.Membership>() {
                    new Course.Membership() { Id = Guid.NewGuid(), CourseId = Guid.Parse("{C68EA325-9527-4343-934E-67D7998B7B16}"), UserId = Guid.Parse("{853EF2AB-C86A-4ECD-A1CD-68B6A15DFE02}"), EnrolledDate = DateTime.Now.AddDays(-24) } }  },

                new Course() { Id = Guid.Parse("{F8C2613F-6133-41F8-A224-6DAA65DBFAC2}"), Code = "SAL1", Name = "Sales 1", Description = "Introduction course to sales", StartDate = DateTime.Now.AddDays(-31), EndDate = DateTime.Now.AddDays(19), Memperships = new List<Course.Membership>() {
                    new Course.Membership() { Id = Guid.NewGuid(), CourseId = Guid.Parse("{F8C2613F-6133-41F8-A224-6DAA65DBFAC2}"), UserId = Guid.Parse("{22C4D8D0-8683-4116-B0FD-8C55B20E900D}"), EnrolledDate = DateTime.Now.AddDays(-31) },
                    new Course.Membership() { Id = Guid.NewGuid(), CourseId = Guid.Parse("{F8C2613F-6133-41F8-A224-6DAA65DBFAC2}"), UserId = Guid.Parse("{BF6DA545-C0A1-49FB-86AC-4BD6B890C3F5}"), EnrolledDate = DateTime.Now.AddDays(-31) } }  },

                new Course() { Id = Guid.Parse("{CE38E160-EC70-4A34-82D3-F42099F67327}"), Code = "SAL2", Name = "Sales 2", StartDate = DateTime.Now.AddDays(19), EndDate = DateTime.Now.AddDays(75), Memperships = new List<Course.Membership>() {
                    new Course.Membership() { Id = Guid.NewGuid(), CourseId = Guid.Parse("{CE38E160-EC70-4A34-82D3-F42099F67327}"), UserId = Guid.Parse("{106CD9D2-3065-47A8-AC34-85343037534A}"), EnrolledDate = DateTime.Now.AddDays(19) } }  },

                new Course() { Id = Guid.Parse("{DDA58C7C-8105-4377-89AD-AB504E87C89D}"), Code = "EMC1", Name = "Embedded C 1", Description = "Course in the Embedded C language", StartDate = DateTime.Now.AddDays(-19), EndDate = DateTime.Now.AddDays(44), Memperships = new List<Course.Membership>() {
                    new Course.Membership() { Id = Guid.NewGuid(), CourseId = Guid.Parse("{DDA58C7C-8105-4377-89AD-AB504E87C89D}"), UserId = Guid.Parse("{67A0843F-7262-4F05-BB4B-0045F5A6BA96}"), EnrolledDate = DateTime.Now.AddDays(-19) } }  },

                new Course() { Id = Guid.Parse("{B8EEEF95-B579-45D4-8E86-2862C6FEB1C6}"), Code = "QUC1", Name = "Quality Control 1", Description = "Comprehensive course which provides a systematic development of skills and knowledge of QA", StartDate = DateTime.Now.AddDays(-14), EndDate = DateTime.Now.AddDays(31), Memperships = new List<Course.Membership>() {
                    new Course.Membership() { Id = Guid.NewGuid(), CourseId = Guid.Parse("{B8EEEF95-B579-45D4-8E86-2862C6FEB1C6}"), UserId = Guid.Parse("{F10C5A5B-51D2-4115-9421-6B4D6A6BA60F}"), EnrolledDate = DateTime.Now.AddDays(-14) } }  },

                new Course() { Id = Guid.Parse("{7659C805-D968-4910-BDF6-6936FAB38FC3}"), Code = "POL1", Name = "Public Administration 1", Description = "Course specializing in public management and administration.", StartDate = DateTime.Now.AddDays(-27), EndDate = DateTime.Now.AddDays(26), }
            };

        }
        public static IEnumerable<OAuthClient> GetClients()
        {
            return new List<OAuthClient>
            {
                new OAuthClient
                {
                    //OAuth2 
                    ClientId = "eduManagementLabApi",
                    ClientName = "ASP.NET Core EduManagementLab Api",
                    AllowedGrantTypes = new List<ClientGrantType>
                    {
                        new ClientGrantType { GrantType = "client_credentials" },
                    },
                    ClientSecrets = new List<ClientSecret>
                    {
                        new ClientSecret { Value = "TestEduApi".Sha256() }
                    },
                    AllowedScopes = new List<ClientScope>()
                    {
                        new ClientScope { Scope = "eduManagementLabApi.read" },
                        new ClientScope { Scope = "eduManagementLabApi.write" },
                        new ClientScope { Scope = "https://purl.imsglobal.org/spec/lti-ags/scope/lineitem" },
                        new ClientScope { Scope = "https://purl.imsglobal.org/spec/lti-ags/scope/lineitem.readonly" },
                        new ClientScope { Scope = "https://purl.imsglobal.org/spec/lti-ags/scope/result.readonly" },
                        new ClientScope { Scope = "https://purl.imsglobal.org/spec/lti-ags/scope/score" },
                        new ClientScope { Scope = "https://purl.imsglobal.org/spec/lti-ags/scope/score.readonly" },
                        new ClientScope { Scope = "https://purl.imsglobal.org/spec/lti-nrps/scope/contextmembership.readonly" },
                        new ClientScope { Scope = "openid" },
                    },
                    AllowedCorsOrigins = new List<ClientCorsOrigin>
                    {
                        new ClientCorsOrigin{Origin= "https://localhost:5001"},
                        new ClientCorsOrigin{Origin= "https://localhost:5002"},
                        new ClientCorsOrigin{Origin= "https://localhost:44308"},
                        new ClientCorsOrigin{Origin= "https://localhost:44338"},
                    },
                },
                new OAuthClient
                {
                    //OpenID Connect
                    ClientId = "oidcEduWebApp",
                    ClientName = "ASP.NET Core EduManagementLab Web",
                    ClientSecrets = new List<ClientSecret>
                    {
                        new ClientSecret { Value = "TestEduApi".Sha256() }
                    },
                    AllowedGrantTypes = new List<ClientGrantType>
                    {
                        new ClientGrantType
                        {
                            GrantType = "authorization_code"
                        }
                    },
                    AllowOfflineAccess = true,
                    RedirectUris = new List<ClientRedirectUri>
                    {
                        new ClientRedirectUri
                        {
                            RedirectUri = "https://localhost:5002/signin-oidc"
                        }
                    },
                    PostLogoutRedirectUris = new List<ClientPostLogoutRedirectUri>
                    {
                        new ClientPostLogoutRedirectUri
                        {
                            PostLogoutRedirectUri = "https://localhost:5002/signout-callback-oidc"
                        }
                    },
                    AllowedScopes = new List<ClientScope>
                    {
                        new ClientScope { Scope = "openid" },
                        new ClientScope { Scope = "profile" }
                    }
                }
            };
        }
    }
}
