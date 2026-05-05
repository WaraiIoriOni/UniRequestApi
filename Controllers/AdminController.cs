using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniRequestAPI.DbContexts;
using UniRequestAPI.Models.DTO;
using UniRequestAPI.Models.People;
using UniRequestAPI.Models.Users;
using UniRequestAPI.Utils;
using System.IO;

namespace UniRequestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {
        private ApplicationContext _context;

        public AdminController()
        {
            _context = new ApplicationContext();
        }

        [HttpGet("registration/requests")]
        public IActionResult GetRegistrationRequests()
        {
            var requests = _context.RegistrationRequests
                .Where(r => r.Status == "pending")
                .Select(r => new
                {
                    r.Id,
                    r.Login,
                    r.FirstName,
                    r.LastName,
                    r.MiddleName,
                    r.Email,
                    r.PhoneNumber,
                    r.Department,
                    r.CreatedAt,
                    r.Status
                })
                .ToList();

            return Ok(requests);
        }

        [HttpPost("registration/approve")]
        public IActionResult ApproveRegistration([FromBody] ApproveRegistrationDto approveDto)
        {
            var regRequest = _context.RegistrationRequests.FirstOrDefault(r => r.Id == approveDto.RequestId);
            if (regRequest == null)
                return BadRequest("Registration request not found");

            if (regRequest.Status != "pending")
                return BadRequest("Registration request already processed");

            if (!approveDto.Approve)
            {
                regRequest.Status = "rejected";
                _context.SaveChanges();
                return Ok(new { message = "Registration request rejected" });
            }

            var existingUser = _context.Users.FirstOrDefault(u => u.Login == regRequest.Login);
            if (existingUser != null)
            {
                regRequest.Status = "rejected";
                _context.SaveChanges();
                return BadRequest("User with this login already exists");
            }

            var user = new User
            {
                Login = regRequest.Login,
                Password = regRequest.Password,
                Role = "student"
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            var person = new Person
            {
                FirstName = regRequest.FirstName,
                LastName = regRequest.LastName,
                MiddleName = regRequest.MiddleName,
                Email = regRequest.Email,
                PhoneNumber = regRequest.PhoneNumber,
                Department = regRequest.Department,
                UserId = user.Id
            };
            _context.People.Add(person);
            _context.SaveChanges();

            regRequest.Status = "approved";
            _context.SaveChanges();

            return Ok(new
            {
                message = "Registration request approved. User created successfully.",
                userId = user.Id,
                personId = person.Id
            });
        }

        [HttpPost("person/create")]
        public IActionResult AddPerson([FromBody] PersonDto personDto)
        {
            var allowedRoles = new[] { "employee", "student", "dispatch" };
            if (!allowedRoles.Contains(personDto.Role.ToLower()))
                return BadRequest("Role must be 'employee', 'student' or 'dispatch'");

            var existingUser = _context.Users.FirstOrDefault(u => u.Login == personDto.Login);
            if (existingUser != null)
                return BadRequest("User with this login already exists");

            var user = new User
            {
                Login = personDto.Login,
                Password = AuthUtils.HashPassword(personDto.Password),
                Role = personDto.Role.ToLower()
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            var person = new Person
            {
                FirstName = personDto.FirstName,
                LastName = personDto.LastName,
                MiddleName = personDto.MiddleName,
                Email = personDto.Email,
                PhoneNumber = personDto.PhoneNumber,
                Department = personDto.Department,
                UserId = user.Id
            };
            _context.People.Add(person);
            _context.SaveChanges();

            return Ok(person);
        }

        [HttpGet("people/read")]
        public IActionResult GetAllPeople()
        {
            var people = _context.People
                .Include(p => p.User)
                .Select(p => new
                {
                    p.Id,
                    FullName = $"{p.MiddleName} {p.FirstName} {p.LastName}".Trim(),
                    p.Email,
                    p.PhoneNumber,
                    p.Department,
                    User = new
                    {
                        p.User.Id,
                        p.User.Login,
                        p.User.Role
                    },
                    Statistics = new
                    {
                        TotalRequests = _context.Requests.Count(r => r.PersonId == p.Id),
                        TotalAppeals = _context.Appeals.Count(a => a.PersonId == p.Id),
                        TotalQuestions = _context.Questions.Count(q => q.PersonId == p.Id),
                        TotalSuggestions = _context.Suggestions.Count(s => s.PersonId == p.Id),
                        TotalComplaints = _context.Complaints.Count(c => c.PersonId == p.Id)
                    }
                })
                .ToList();

            return Ok(new
            {
                Total = people.Count,
                Employees = people.Where(p => p.User.Role == "employee").ToList(),
                Students = people.Where(p => p.User.Role == "student").ToList(),
                All = people
            });
        }

        [HttpPut("person/update")]
        public IActionResult UpdatePerson([FromBody] UpdatePersonDto updatePersonDto)
        {
            var person = _context.People.FirstOrDefault(p => p.Id == updatePersonDto.Id);
            if (person == null)
                return BadRequest("Person not found");

            person.FirstName = updatePersonDto.FirstName;
            person.LastName = updatePersonDto.LastName;
            person.MiddleName = updatePersonDto.MiddleName;
            person.Email = updatePersonDto.Email;
            person.PhoneNumber = updatePersonDto.PhoneNumber;
            person.Department = updatePersonDto.Department;

            _context.SaveChanges();
            return Ok(person);
        }

        [HttpDelete("person/delete")]
        public IActionResult DeletePerson(int id)
        {
            var person = _context.People.FirstOrDefault(p => p.Id == id);
            if (person == null)
                return BadRequest("Person not found");

            var user = _context.Users.FirstOrDefault(u => u.Id == person.UserId);

            var requests = _context.Requests.Where(r => r.PersonId == person.Id).ToList();
            foreach (var request in requests)
            {
                var files = _context.UserFiles.Where(f => f.ObjectType == "request" && f.ObjectId == request.Id).ToList();
                foreach (var file in files)
                {
                    if (System.IO.File.Exists(file.FilePath))
                        System.IO.File.Delete(file.FilePath);
                    _context.UserFiles.Remove(file);
                }
            }

            var appeals = _context.Appeals.Where(a => a.PersonId == person.Id).ToList();
            foreach (var appeal in appeals)
            {
                var files = _context.UserFiles.Where(f => f.ObjectType == "appeal" && f.ObjectId == appeal.Id).ToList();
                foreach (var file in files)
                {
                    if (System.IO.File.Exists(file.FilePath))
                        System.IO.File.Delete(file.FilePath);
                    _context.UserFiles.Remove(file);
                }
            }

            var questions = _context.Questions.Where(q => q.PersonId == person.Id).ToList();
            foreach (var question in questions)
            {
                var files = _context.UserFiles.Where(f => f.ObjectType == "question" && f.ObjectId == question.Id).ToList();
                foreach (var file in files)
                {
                    if (System.IO.File.Exists(file.FilePath))
                        System.IO.File.Delete(file.FilePath);
                    _context.UserFiles.Remove(file);
                }
            }

            var suggestions = _context.Suggestions.Where(s => s.PersonId == person.Id).ToList();
            foreach (var suggestion in suggestions)
            {
                var files = _context.UserFiles.Where(f => f.ObjectType == "suggestion" && f.ObjectId == suggestion.Id).ToList();
                foreach (var file in files)
                {
                    if (System.IO.File.Exists(file.FilePath))
                        System.IO.File.Delete(file.FilePath);
                    _context.UserFiles.Remove(file);
                }
            }

            var complaints = _context.Complaints.Where(c => c.PersonId == person.Id).ToList();
            foreach (var complaint in complaints)
            {
                var files = _context.UserFiles.Where(f => f.ObjectType == "complaint" && f.ObjectId == complaint.Id).ToList();
                foreach (var file in files)
                {
                    if (System.IO.File.Exists(file.FilePath))
                        System.IO.File.Delete(file.FilePath);
                    _context.UserFiles.Remove(file);
                }
            }

            _context.People.Remove(person);

            if (user != null)
                _context.Users.Remove(user);

            _context.SaveChanges();
            return Ok("Person and all related data deleted successfully");
        }
    }
}
