using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniRequestAPI.DbContexts;
using UniRequestAPI.Models.Comments;
using UniRequestAPI.Models.DTO;
using System.IO;

namespace UniRequestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "employee, student")]
    public class UserRequestController : ControllerBase
    {
        private ApplicationContext _context;

        public UserRequestController()
        {
            _context = new ApplicationContext();
        }

        [HttpPost("message/create")]
        public IActionResult CreateMessage([FromBody] MessageRequestDto messageDto)
        {
            var userLogin = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.Login == userLogin);

            if (user == null)
                return BadRequest("User not found");

            var person = _context.People.FirstOrDefault(p => p.UserId == user.Id);
            if (person == null)
                return BadRequest("Person not found");

            var allowedTypes = new[] { "request", "appeal", "question", "suggestion", "complaint" };
            if (!allowedTypes.Contains(messageDto.Type.ToLower()))
                return BadRequest("Type must be 'request', 'appeal', 'question', 'suggestion' or 'complaint'");

            var tempMessage = new MessageRequest
            {
                Type = messageDto.Type.ToLower(),
                Title = messageDto.Title,
                Content = messageDto.Content,
                CreatedAt = DateTime.UtcNow,
                PersonId = person.Id,
                Status = "pending"
            };
            _context.MessageRequests.Add(tempMessage);
            _context.SaveChanges();

            return Ok(new
            {
                message = "Message submitted successfully. Waiting for dispatch approval.",
                messageId = tempMessage.Id,
                type = tempMessage.Type,
                status = tempMessage.Status
            });
        }

        [HttpGet("message/read")]
        public IActionResult GetMyMessages()
        {
            var userLogin = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.Login == userLogin);

            if (user == null)
                return BadRequest("User not found");

            var person = _context.People.FirstOrDefault(p => p.UserId == user.Id);
            if (person == null)
                return BadRequest("Person not found");

            var requests = _context.Requests
                .Where(r => r.PersonId == person.Id)
                .ToList()
                .Select(r => new
                {
                    r.Id,
                    r.Title,
                    r.Description,
                    r.CreatedAt,
                    r.Status,
                    r.ResponseText,
                    r.RespondedAt,
                    PersonId = r.PersonId
                });

            var appeals = _context.Appeals
                .Where(a => a.PersonId == person.Id)
                .ToList()
                .Select(a => new
                {
                    a.Id,
                    a.Title,
                    a.Content,
                    a.CreatedAt,
                    a.Status,
                    a.ResponseText,
                    a.RespondedAt,
                    PersonId = a.PersonId
                });

            var questions = _context.Questions
                .Where(q => q.PersonId == person.Id)
                .ToList()
                .Select(q => new
                {
                    q.Id,
                    q.Title,
                    q.Content,
                    q.CreatedAt,
                    q.Status,
                    q.ResponseText,
                    q.RespondedAt,
                    PersonId = q.PersonId
                });

            var suggestions = _context.Suggestions
                .Where(s => s.PersonId == person.Id)
                .ToList()
                .Select(s => new
                {
                    s.Id,
                    s.Title,
                    s.Content,
                    s.CreatedAt,
                    s.Status,
                    s.ResponseText,
                    s.RespondedAt,
                    PersonId = s.PersonId
                });

            var complaints = _context.Complaints
                .Where(c => c.PersonId == person.Id)
                .ToList()
                .Select(c => new
                {
                    c.Id,
                    c.Title,
                    c.Content,
                    c.CreatedAt,
                    c.Status,
                    c.ResponseText,
                    c.RespondedAt,
                    PersonId = c.PersonId
                });

            var result = new
            {
                requests = requests.ToList(),
                appeals = appeals.ToList(),
                questions = questions.ToList(),
                suggestions = suggestions.ToList(),
                complaints = complaints.ToList()
            };

            return Ok(result);
        }

        [HttpPut("message/update")]
        public IActionResult UpdateMessage([FromBody] UpdateRequestDto updateRequestDto)
        {
            var userLogin = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.Login == userLogin);

            if (user == null)
                return BadRequest("User not found");

            var person = _context.People.FirstOrDefault(p => p.UserId == user.Id);
            if (person == null)
                return BadRequest("Person not found");

            var tempMessage = _context.MessageRequests.FirstOrDefault(m => m.Id == updateRequestDto.Id && m.PersonId == person.Id && m.Status == "pending");
            if (tempMessage == null)
                return BadRequest("Message not found or already processed");

            tempMessage.Title = updateRequestDto.Title;
            tempMessage.Content = updateRequestDto.Content;
            _context.SaveChanges();

            return Ok(new
            {
                message = "Message updated successfully",
                messageId = tempMessage.Id,
                tempMessage.Title,
                tempMessage.Content,
                tempMessage.Status
            });
        }

        [HttpDelete("message/delete")]
        public IActionResult DeleteMessage([FromBody] DeleteRequestDto deleteRequestDto)
        {
            var userLogin = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.Login == userLogin);

            if (user == null)
                return BadRequest("User not found");

            var person = _context.People.FirstOrDefault(p => p.UserId == user.Id);
            if (person == null)
                return BadRequest("Person not found");

            var tempMessage = _context.MessageRequests.FirstOrDefault(m => m.Id == deleteRequestDto.Id && m.PersonId == person.Id && m.Status == "pending");
            if (tempMessage == null)
                return BadRequest("Message not found or already processed");

            var files = _context.UserFiles.Where(f => f.ObjectId == tempMessage.Id && f.ObjectType == tempMessage.Type).ToList();
            foreach (var file in files)
            {
                if (System.IO.File.Exists(file.FilePath))
                    System.IO.File.Delete(file.FilePath);
                _context.UserFiles.Remove(file);
            }

            _context.MessageRequests.Remove(tempMessage);
            _context.SaveChanges();

            return Ok("Message deleted successfully");
        }

        [HttpPost("file/upload")]
        public IActionResult UploadFile([FromBody] UploadFileDto uploadFileDto)
        {
            var userLogin = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.Login == userLogin);

            if (user == null)
                return BadRequest("User not found");

            var person = _context.People.FirstOrDefault(p => p.UserId == user.Id);
            if (person == null)
                return BadRequest("Person not found");

            var tempMessage = _context.MessageRequests.FirstOrDefault(m => m.Id == uploadFileDto.ObjectId && m.PersonId == person.Id);
            if (tempMessage == null)
                return BadRequest("Message not found or access denied");

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string fileExtension = Path.GetExtension(uploadFileDto.FileName);
            string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            byte[] fileBytes = Convert.FromBase64String(uploadFileDto.FileBase64);
            System.IO.File.WriteAllBytes(filePath, fileBytes);

            var userFile = new UserFile
            {
                FileName = uploadFileDto.FileName,
                FilePath = filePath,
                FileType = fileExtension,
                FileSize = fileBytes.Length,
                UploadedAt = DateTime.UtcNow,
                ObjectType = uploadFileDto.ObjectType.ToLower(),
                ObjectId = uploadFileDto.ObjectId
            };
            _context.UserFiles.Add(userFile);
            _context.SaveChanges();

            return Ok(new
            {
                message = "File uploaded successfully",
                fileId = userFile.Id,
                fileName = userFile.FileName,
                fileSize = userFile.FileSize,
                uploadedAt = userFile.UploadedAt
            });
        }
    }
}
