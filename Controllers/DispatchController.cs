using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniRequestAPI.DbContexts;
using UniRequestAPI.Models.Comments;
using UniRequestAPI.Models.DTO;
using System.IO;

namespace UniRequestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "dispatch")]
    public class DispatchController : ControllerBase
    {
        private ApplicationContext _context;

        public DispatchController()
        {
            _context = new ApplicationContext();
        }

        [HttpGet("messages/const/get")]
        public IActionResult GetAllMessages()
        {
            var requests = _context.Requests
                .Include(r => r.Person)
                .ThenInclude(p => p.User)
                .Select(r => new
                {
                    Type = "Request",
                    r.Id,
                    r.Title,
                    r.Description,
                    r.CreatedAt,
                    r.Status,
                    r.ResponseText,
                    r.RespondedAt,
                    Person = new
                    {
                        r.Person.Id,
                        FullName = $"{r.Person.LastName} {r.Person.FirstName} {r.Person.MiddleName}".Trim(),
                        r.Person.PhoneNumber,
                        r.Person.Department,
                        UserLogin = r.Person.User.Login
                    }
                })
                .ToList();

            var appeals = _context.Appeals
                .Include(a => a.Person)
                .ThenInclude(p => p.User)
                .Select(a => new
                {
                    Type = "Appeal",
                    a.Id,
                    a.Title,
                    a.Content,
                    a.CreatedAt,
                    a.Status,
                    a.ResponseText,
                    a.RespondedAt,
                    Person = new
                    {
                        a.Person.Id,
                        FullName = $"{a.Person.LastName} {a.Person.FirstName} {a.Person.MiddleName}".Trim(),
                        a.Person.PhoneNumber,
                        a.Person.Department,
                        UserLogin = a.Person.User.Login
                    }
                })
                .ToList();

            var questions = _context.Questions
                .Include(q => q.Person)
                .ThenInclude(p => p.User)
                .Select(q => new
                {
                    Type = "Question",
                    q.Id,
                    q.Title,
                    q.Content,
                    q.CreatedAt,
                    q.Status,
                    q.ResponseText,
                    q.RespondedAt,
                    Person = new
                    {
                        q.Person.Id,
                        FullName = $"{q.Person.LastName} {q.Person.FirstName} {q.Person.MiddleName}".Trim(),
                        q.Person.PhoneNumber,
                        q.Person.Department,
                        UserLogin = q.Person.User.Login
                    }
                })
                .ToList();

            var suggestions = _context.Suggestions
                .Include(s => s.Person)
                .ThenInclude(p => p.User)
                .Select(s => new
                {
                    Type = "Suggestion",
                    s.Id,
                    s.Title,
                    s.Content,
                    s.CreatedAt,
                    s.Status,
                    s.ResponseText,
                    s.RespondedAt,
                    Person = new
                    {
                        s.Person.Id,
                        FullName = $"{s.Person.LastName} {s.Person.FirstName} {s.Person.MiddleName}".Trim(),
                        s.Person.PhoneNumber,
                        s.Person.Department,
                        UserLogin = s.Person.User.Login
                    }
                })
                .ToList();

            var complaints = _context.Complaints
                .Include(c => c.Person)
                .ThenInclude(p => p.User)
                .Select(c => new
                {
                    Type = "Complaint",
                    c.Id,
                    c.Title,
                    c.Content,
                    c.CreatedAt,
                    c.Status,
                    c.ResponseText,
                    c.RespondedAt,
                    Person = new
                    {
                        c.Person.Id,
                        FullName = $"{c.Person.LastName} {c.Person.FirstName} {c.Person.MiddleName}".Trim(),
                        c.Person.PhoneNumber,
                        c.Person.Department,
                        UserLogin = c.Person.User.Login
                    }
                })
                .ToList();

            var result = new
            {
                requests,
                appeals,
                questions,
                suggestions,
                complaints
            };

            return Ok(result);
        }

        [HttpGet("messages/user/const/get")]
        public IActionResult GetUserMessages(int personId)
        {
            var person = _context.People.FirstOrDefault(p => p.Id == personId);
            if (person == null)
                return BadRequest("Person not found");

            var requests = _context.Requests
                .Where(r => r.PersonId == person.Id)
                .Select(r => new
                {
                    Type = "Request",
                    r.Id,
                    r.Title,
                    r.Description,
                    r.CreatedAt,
                    r.Status,
                    r.ResponseText,
                    r.RespondedAt
                })
                .ToList();

            var appeals = _context.Appeals
                .Where(a => a.PersonId == person.Id)
                .Select(a => new
                {
                    Type = "Appeal",
                    a.Id,
                    a.Title,
                    a.Content,
                    a.CreatedAt,
                    a.Status,
                    a.ResponseText,
                    a.RespondedAt
                })
                .ToList();

            var questions = _context.Questions
                .Where(q => q.PersonId == person.Id)
                .Select(q => new
                {
                    Type = "Question",
                    q.Id,
                    q.Title,
                    q.Content,
                    q.CreatedAt,
                    q.Status,
                    q.ResponseText,
                    q.RespondedAt
                })
                .ToList();

            var suggestions = _context.Suggestions
                .Where(s => s.PersonId == person.Id)
                .Select(s => new
                {
                    Type = "Suggestion",
                    s.Id,
                    s.Title,
                    s.Content,
                    s.CreatedAt,
                    s.Status,
                    s.ResponseText,
                    s.RespondedAt
                })
                .ToList();

            var complaints = _context.Complaints
                .Where(c => c.PersonId == person.Id)
                .Select(c => new
                {
                    Type = "Complaint",
                    c.Id,
                    c.Title,
                    c.Content,
                    c.CreatedAt,
                    c.Status,
                    c.ResponseText,
                    c.RespondedAt
                })
                .ToList();

            return Ok(new
            {
                personId = person.Id,
                fullName = $"{person.LastName} {person.FirstName} {person.MiddleName}".Trim(),
                requests,
                appeals,
                questions,
                suggestions,
                complaints
            });
        }

        [HttpGet("messages/temp/get")]
        public IActionResult GetTempMessages()
        {
            var messages = _context.MessageRequests
                .Where(m => m.Status == "pending")
                .Select(m => new
                {
                    m.Id,
                    m.Type,
                    m.Title,
                    m.Content,
                    m.CreatedAt,
                    m.PersonId,
                    m.Status
                })
                .ToList();

            return Ok(messages);
        }

        [HttpPost("message/temp/process")]
        public IActionResult ProcessTempMessage([FromBody] ProcessRequestMessageDto processDto)
        {
            var tempMessage = _context.MessageRequests.FirstOrDefault(m => m.Id == processDto.MessageId);
            if (tempMessage == null)
                return BadRequest("Temp message not found");

            if (tempMessage.Status != "pending")
                return BadRequest("Message already processed");

            if (!processDto.Accept)
            {
                tempMessage.Status = "rejected";
                _context.SaveChanges();
                return Ok(new { message = "Message rejected and removed from queue" });
            }

            if (tempMessage.Type.ToLower() == "request")
            {
                var request = new Request
                {
                    Title = tempMessage.Title,
                    Description = tempMessage.Content,
                    CreatedAt = tempMessage.CreatedAt,
                    Status = "pending",
                    PersonId = tempMessage.PersonId
                };
                _context.Requests.Add(request);
            }
            else if (tempMessage.Type.ToLower() == "appeal")
            {
                var appeal = new Appeal
                {
                    Title = tempMessage.Title,
                    Content = tempMessage.Content,
                    CreatedAt = tempMessage.CreatedAt,
                    Status = "pending",
                    PersonId = tempMessage.PersonId
                };
                _context.Appeals.Add(appeal);
            }
            else if (tempMessage.Type.ToLower() == "question")
            {
                var question = new Question
                {
                    Title = tempMessage.Title,
                    Content = tempMessage.Content,
                    CreatedAt = tempMessage.CreatedAt,
                    Status = "pending",
                    PersonId = tempMessage.PersonId
                };
                _context.Questions.Add(question);
            }
            else if (tempMessage.Type.ToLower() == "suggestion")
            {
                var suggestion = new Suggestion
                {
                    Title = tempMessage.Title,
                    Content = tempMessage.Content,
                    CreatedAt = tempMessage.CreatedAt,
                    Status = "pending",
                    PersonId = tempMessage.PersonId
                };
                _context.Suggestions.Add(suggestion);
            }
            else if (tempMessage.Type.ToLower() == "complaint")
            {
                var complaint = new Complaint
                {
                    Title = tempMessage.Title,
                    Content = tempMessage.Content,
                    CreatedAt = tempMessage.CreatedAt,
                    Status = "pending",
                    PersonId = tempMessage.PersonId
                };
                _context.Complaints.Add(complaint);
            }
            else
            {
                return BadRequest("Unknown message type");
            }

            tempMessage.Status = "accepted";
            _context.SaveChanges();

            return Ok(new { message = $"Message accepted and moved to {tempMessage.Type} table" });
        }

        [HttpGet("file/download")]
        public IActionResult DownloadFile(string type, int id)
        {
            if (type.ToLower() == "request")
            {
                var request = _context.Requests.FirstOrDefault(r => r.Id == id);
                if (request == null)
                    return BadRequest("Request not found");
            }
            else if (type.ToLower() == "appeal")
            {
                var appeal = _context.Appeals.FirstOrDefault(a => a.Id == id);
                if (appeal == null)
                    return BadRequest("Appeal not found");
            }
            else if (type.ToLower() == "question")
            {
                var question = _context.Questions.FirstOrDefault(q => q.Id == id);
                if (question == null)
                    return BadRequest("Question not found");
            }
            else if (type.ToLower() == "suggestion")
            {
                var suggestion = _context.Suggestions.FirstOrDefault(s => s.Id == id);
                if (suggestion == null)
                    return BadRequest("Suggestion not found");
            }
            else if (type.ToLower() == "complaint")
            {
                var complaint = _context.Complaints.FirstOrDefault(c => c.Id == id);
                if (complaint == null)
                    return BadRequest("Complaint not found");
            }
            else
            {
                return BadRequest("Type must be 'request', 'appeal', 'question', 'suggestion' or 'complaint'");
            }

            var file = _context.UserFiles.FirstOrDefault(f => f.ObjectType == type.ToLower() && f.ObjectId == id);
            if (file == null)
                return BadRequest("File not found");

            if (!System.IO.File.Exists(file.FilePath))
                return BadRequest("File not found on server");

            byte[] fileBytes = System.IO.File.ReadAllBytes(file.FilePath);
            return File(fileBytes, "application/octet-stream", file.FileName);
        }

        [HttpPut("message/status/update")]
        public IActionResult UpdateMessageStatus([FromBody] UpdateStatusDto updateStatusDto)
        {
            if (updateStatusDto.Type.ToLower() == "request")
            {
                var request = _context.Requests.FirstOrDefault(r => r.Id == updateStatusDto.Id);
                if (request == null)
                    return BadRequest("Request not found");

                request.Status = updateStatusDto.Status;
                request.ResponseText = updateStatusDto.ResponseText;
                request.RespondedAt = DateTime.UtcNow;
                _context.SaveChanges();
                return Ok(request);
            }
            else if (updateStatusDto.Type.ToLower() == "appeal")
            {
                var appeal = _context.Appeals.FirstOrDefault(a => a.Id == updateStatusDto.Id);
                if (appeal == null)
                    return BadRequest("Appeal not found");

                appeal.Status = updateStatusDto.Status;
                appeal.ResponseText = updateStatusDto.ResponseText;
                appeal.RespondedAt = DateTime.UtcNow;
                _context.SaveChanges();
                return Ok(appeal);
            }
            else if (updateStatusDto.Type.ToLower() == "question")
            {
                var question = _context.Questions.FirstOrDefault(q => q.Id == updateStatusDto.Id);
                if (question == null)
                    return BadRequest("Question not found");

                question.Status = updateStatusDto.Status;
                question.ResponseText = updateStatusDto.ResponseText;
                question.RespondedAt = DateTime.UtcNow;
                _context.SaveChanges();
                return Ok(question);
            }
            else if (updateStatusDto.Type.ToLower() == "suggestion")
            {
                var suggestion = _context.Suggestions.FirstOrDefault(s => s.Id == updateStatusDto.Id);
                if (suggestion == null)
                    return BadRequest("Suggestion not found");

                suggestion.Status = updateStatusDto.Status;
                suggestion.ResponseText = updateStatusDto.ResponseText;
                suggestion.RespondedAt = DateTime.UtcNow;
                _context.SaveChanges();
                return Ok(suggestion);
            }
            else if (updateStatusDto.Type.ToLower() == "complaint")
            {
                var complaint = _context.Complaints.FirstOrDefault(c => c.Id == updateStatusDto.Id);
                if (complaint == null)
                    return BadRequest("Complaint not found");

                complaint.Status = updateStatusDto.Status;
                complaint.ResponseText = updateStatusDto.ResponseText;
                complaint.RespondedAt = DateTime.UtcNow;
                _context.SaveChanges();
                return Ok(complaint);
            }

            return BadRequest("Type must be 'request', 'appeal', 'question', 'suggestion' or 'complaint'");
        }
    }
}
