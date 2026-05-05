using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniRequestAPI.DbContexts;
using UniRequestAPI.Models.DTO;

namespace UniRequestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class ReportsController : ControllerBase
    {
        private ApplicationContext _context;

        public ReportsController()
        {
            _context = new ApplicationContext();
        }

        [HttpGet("message/get")]
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
                        FullName = $"{r.Person.LastName} {r.Person.FirstName} {r.Person.MiddleName}".Trim(),
                        PhoneNumber = r.Person.PhoneNumber,
                        Status = r.Person.User.Role == "student" ? "Student" : "Employee"
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
                        FullName = $"{a.Person.LastName} {a.Person.FirstName} {a.Person.MiddleName}".Trim(),
                        PhoneNumber = a.Person.PhoneNumber,
                        Status = a.Person.User.Role == "student" ? "Student" : "Employee"
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
                        FullName = $"{q.Person.LastName} {q.Person.FirstName} {q.Person.MiddleName}".Trim(),
                        PhoneNumber = q.Person.PhoneNumber,
                        Status = q.Person.User.Role == "student" ? "Student" : "Employee"
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
                        FullName = $"{s.Person.LastName} {s.Person.FirstName} {s.Person.MiddleName}".Trim(),
                        PhoneNumber = s.Person.PhoneNumber,
                        Status = s.Person.User.Role == "student" ? "Student" : "Employee"
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
                        FullName = $"{c.Person.LastName} {c.Person.FirstName} {c.Person.MiddleName}".Trim(),
                        PhoneNumber = c.Person.PhoneNumber,
                        Status = c.Person.User.Role == "student" ? "Student" : "Employee"
                    }
                })
                .ToList();

            return Ok(new { requests, appeals, questions, suggestions, complaints });
        }

        [HttpDelete("message/delete")]
        public IActionResult DeleteMessage([FromBody] DeleteRequestDto deleteRequestDto)
        {
            if (deleteRequestDto.Type.ToLower() == "request")
            {
                var request = _context.Requests.FirstOrDefault(r => r.Id == deleteRequestDto.Id);
                if (request == null)
                    return BadRequest("Request not found");

                var files = _context.UserFiles.Where(f => f.ObjectType == "request" && f.ObjectId == request.Id).ToList();
                foreach (var file in files)
                {
                    if (System.IO.File.Exists(file.FilePath))
                        System.IO.File.Delete(file.FilePath);
                    _context.UserFiles.Remove(file);
                }

                _context.Requests.Remove(request);
                _context.SaveChanges();
                return Ok($"Request with Id={deleteRequestDto.Id} deleted successfully");
            }
            else if (deleteRequestDto.Type.ToLower() == "appeal")
            {
                var appeal = _context.Appeals.FirstOrDefault(a => a.Id == deleteRequestDto.Id);
                if (appeal == null)
                    return BadRequest("Appeal not found");

                var files = _context.UserFiles.Where(f => f.ObjectType == "appeal" && f.ObjectId == appeal.Id).ToList();
                foreach (var file in files)
                {
                    if (System.IO.File.Exists(file.FilePath))
                        System.IO.File.Delete(file.FilePath);
                    _context.UserFiles.Remove(file);
                }

                _context.Appeals.Remove(appeal);
                _context.SaveChanges();
                return Ok($"Appeal with Id={deleteRequestDto.Id} deleted successfully");
            }
            else if (deleteRequestDto.Type.ToLower() == "question")
            {
                var question = _context.Questions.FirstOrDefault(q => q.Id == deleteRequestDto.Id);
                if (question == null)
                    return BadRequest("Question not found");

                var files = _context.UserFiles.Where(f => f.ObjectType == "question" && f.ObjectId == question.Id).ToList();
                foreach (var file in files)
                {
                    if (System.IO.File.Exists(file.FilePath))
                        System.IO.File.Delete(file.FilePath);
                    _context.UserFiles.Remove(file);
                }

                _context.Questions.Remove(question);
                _context.SaveChanges();
                return Ok($"Question with Id={deleteRequestDto.Id} deleted successfully");
            }
            else if (deleteRequestDto.Type.ToLower() == "suggestion")
            {
                var suggestion = _context.Suggestions.FirstOrDefault(s => s.Id == deleteRequestDto.Id);
                if (suggestion == null)
                    return BadRequest("Suggestion not found");

                var files = _context.UserFiles.Where(f => f.ObjectType == "suggestion" && f.ObjectId == suggestion.Id).ToList();
                foreach (var file in files)
                {
                    if (System.IO.File.Exists(file.FilePath))
                        System.IO.File.Delete(file.FilePath);
                    _context.UserFiles.Remove(file);
                }

                _context.Suggestions.Remove(suggestion);
                _context.SaveChanges();
                return Ok($"Suggestion with Id={deleteRequestDto.Id} deleted successfully");
            }
            else if (deleteRequestDto.Type.ToLower() == "complaint")
            {
                var complaint = _context.Complaints.FirstOrDefault(c => c.Id == deleteRequestDto.Id);
                if (complaint == null)
                    return BadRequest("Complaint not found");

                var files = _context.UserFiles.Where(f => f.ObjectType == "complaint" && f.ObjectId == complaint.Id).ToList();
                foreach (var file in files)
                {
                    if (System.IO.File.Exists(file.FilePath))
                        System.IO.File.Delete(file.FilePath);
                    _context.UserFiles.Remove(file);
                }

                _context.Complaints.Remove(complaint);
                _context.SaveChanges();
                return Ok($"Complaint with Id={deleteRequestDto.Id} deleted successfully");
            }

            return BadRequest("Type must be 'request', 'appeal', 'question', 'suggestion' or 'complaint'");
        }

        [HttpGet("export")]
        public IActionResult ExportToDocx()
        {
            var requests = _context.Requests
                .Include(r => r.Person)
                .ThenInclude(p => p.User)
                .ToList();

            var appeals = _context.Appeals
                .Include(a => a.Person)
                .ThenInclude(p => p.User)
                .ToList();

            var questions = _context.Questions
                .Include(q => q.Person)
                .ThenInclude(p => p.User)
                .ToList();

            var suggestions = _context.Suggestions
                .Include(s => s.Person)
                .ThenInclude(p => p.User)
                .ToList();

            var complaints = _context.Complaints
                .Include(c => c.Person)
                .ThenInclude(p => p.User)
                .ToList();

            using (var stream = new MemoryStream())
            {
                using (var wordDocument = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
                {
                    var mainPart = wordDocument.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    var body = mainPart.Document.AppendChild(new Body());

                    var title = new Paragraph(
                        new Run(new Text($"Отчёт по системе учёта обращений и заявок"))
                        {
                            RunProperties = new RunProperties(new Bold(), new FontSize { Val = "32" })
                        });
                    body.AppendChild(title);

                    var dateParagraph = new Paragraph(
                        new Run(new Text($"Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm:ss}"))
                        {
                            RunProperties = new RunProperties(new Italic(), new FontSize { Val = "20" })
                        });
                    body.AppendChild(dateParagraph);

                    body.AppendChild(new Paragraph());

                    var statsParagraph = new Paragraph(
                        new Run(new Text($"Статистика:"))
                        {
                            RunProperties = new RunProperties(new Bold(), new FontSize { Val = "24" })
                        });
                    body.AppendChild(statsParagraph);

                    body.AppendChild(new Paragraph(new Run(new Text($"- Заявок (Requests): {requests.Count}"))));
                    body.AppendChild(new Paragraph(new Run(new Text($"- Обращений (Appeals): {appeals.Count}"))));
                    body.AppendChild(new Paragraph(new Run(new Text($"- Вопросов (Questions): {questions.Count}"))));
                    body.AppendChild(new Paragraph(new Run(new Text($"- Предложений (Suggestions): {suggestions.Count}"))));
                    body.AppendChild(new Paragraph(new Run(new Text($"- Жалоб (Complaints): {complaints.Count}"))));

                    body.AppendChild(new Paragraph());

                    if (requests.Count > 0)
                    {
                        var requestsTitle = new Paragraph(
                            new Run(new Text("Заявки (Requests)"))
                            {
                                RunProperties = new RunProperties(new Bold(), new FontSize { Val = "24" })
                            });
                        body.AppendChild(requestsTitle);

                        foreach (var r in requests)
                        {
                            body.AppendChild(new Paragraph(new Run(new Text($"ID: {r.Id}"))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Название: {r.Title}"))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Описание: {r.Description}"))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Дата создания: {r.CreatedAt:yyyy-MM-dd HH:mm:ss}"))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Статус: {r.Status}"))));
                            if (!string.IsNullOrEmpty(r.ResponseText))
                                body.AppendChild(new Paragraph(new Run(new Text($"Ответ: {r.ResponseText}"))));
                            if (r.RespondedAt.HasValue)
                                body.AppendChild(new Paragraph(new Run(new Text($"Дата ответа: {r.RespondedAt.Value:yyyy-MM-dd HH:mm:ss}"))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Автор: {r.Person.MiddleName}   {r.Person.FirstName}   {r.Person.LastName}".Trim()))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Телефон: {r.Person.PhoneNumber}"))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Роль: {(r.Person.User.Role == "student" ? "Студент" : "Сотрудник")}"))));
                            body.AppendChild(new Paragraph());
                        }
                    }

                    if (appeals.Count > 0)
                    {
                        var appealsTitle = new Paragraph(
                            new Run(new Text("Обращения (Appeals)"))
                            {
                                RunProperties = new RunProperties(new Bold(), new FontSize { Val = "24" })
                            });
                        body.AppendChild(appealsTitle);

                        foreach (var a in appeals)
                        {
                            body.AppendChild(new Paragraph(new Run(new Text($"ID: {a.Id}"))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Название: {a.Title}"))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Содержание: {a.Content}"))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Дата создания: {a.CreatedAt:yyyy-MM-dd HH:mm:ss}"))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Статус: {a.Status}"))));
                            if (!string.IsNullOrEmpty(a.ResponseText))
                                body.AppendChild(new Paragraph(new Run(new Text($"Ответ: {a.ResponseText}"))));
                            if (a.RespondedAt.HasValue)
                                body.AppendChild(new Paragraph(new Run(new Text($"Дата ответа: {a.RespondedAt.Value:yyyy-MM-dd HH:mm:ss}"))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Автор: {a.Person.MiddleName}   {a.Person.FirstName}   {a.Person.LastName}".Trim()))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Телефон: {a.Person.PhoneNumber}"))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Роль: {(a.Person.User.Role == "student" ? "Студент" : "Сотрудник")}"))));
                            body.AppendChild(new Paragraph());
                        }
                    }

                    if (questions.Count > 0)
                    {
                        var questionsTitle = new Paragraph(
                            new Run(new Text("Вопросы (Questions)"))
                            {
                                RunProperties = new RunProperties(new Bold(), new FontSize { Val = "24" })
                            });
                        body.AppendChild(questionsTitle);

                        foreach (var q in questions)
                        {
                            body.AppendChild(new Paragraph(new Run(new Text($"ID: {q.Id}"))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Название: {q.Title}"))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Содержание: {q.Content}"))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Дата создания: {q.CreatedAt:yyyy-MM-dd HH:mm:ss}"))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Статус: {q.Status}"))));
                            if (!string.IsNullOrEmpty(q.ResponseText))
                                body.AppendChild(new Paragraph(new Run(new Text($"Ответ: {q.ResponseText}"))));
                            if (q.RespondedAt.HasValue)
                                body.AppendChild(new Paragraph(new Run(new Text($"Дата ответа: {q.RespondedAt.Value:yyyy-MM-dd HH:mm:ss}"))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Автор: {q.Person.MiddleName}   {q.Person.FirstName}   {q.Person.LastName}".Trim()))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Телефон: {q.Person.PhoneNumber}"))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Роль: {(q.Person.User.Role == "student" ? "Студент" : "Сотрудник")}"))));
                            body.AppendChild(new Paragraph());
                        }
                    }

                    if (suggestions.Count > 0)
                    {
                        var suggestionsTitle = new Paragraph(
                            new Run(new Text("Предложения (Suggestions)"))
                            {
                                RunProperties = new RunProperties(new Bold(), new FontSize { Val = "24" })
                            });
                        body.AppendChild(suggestionsTitle);

                        foreach (var s in suggestions)
                        {
                            body.AppendChild(new Paragraph(new Run(new Text($"ID: {s.Id}"))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Название: {s.Title}"))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Содержание: {s.Content}"))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Дата создания: {s.CreatedAt:yyyy-MM-dd HH:mm:ss}"))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Статус: {s.Status}"))));
                            if (!string.IsNullOrEmpty(s.ResponseText))
                                body.AppendChild(new Paragraph(new Run(new Text($"Ответ: {s.ResponseText}"))));
                            if (s.RespondedAt.HasValue)
                                body.AppendChild(new Paragraph(new Run(new Text($"Дата ответа: {s.RespondedAt.Value:yyyy-MM-dd HH:mm:ss}"))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Автор: {s.Person.MiddleName} {s.Person.FirstName} {s.Person.LastName}".Trim()))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Телефон: {s.Person.PhoneNumber}"))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Роль: {(s.Person.User.Role == "student" ? "Студент" : "Сотрудник")}"))));
                            body.AppendChild(new Paragraph());
                        }
                    }

                    if (complaints.Count > 0)
                    {
                        var complaintsTitle = new Paragraph(
                            new Run(new Text("Жалобы (Complaints)"))
                            {
                                RunProperties = new RunProperties(new Bold(), new FontSize { Val = "24" })
                            });
                        body.AppendChild(complaintsTitle);

                        foreach (var c in complaints)
                        {
                            body.AppendChild(new Paragraph(new Run(new Text($"ID: {c.Id}"))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Название: {c.Title}"))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Содержание: {c.Content}"))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Дата создания: {c.CreatedAt:yyyy-MM-dd HH:mm:ss}"))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Статус: {c.Status}"))));
                            if (!string.IsNullOrEmpty(c.ResponseText))
                                body.AppendChild(new Paragraph(new Run(new Text($"Ответ: {c.ResponseText}"))));
                            if (c.RespondedAt.HasValue)
                                body.AppendChild(new Paragraph(new Run(new Text($"Дата ответа: {c.RespondedAt.Value:yyyy-MM-dd HH:mm:ss}"))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Автор: {c.Person.MiddleName} {c.Person.FirstName} {c.Person.LastName}".Trim()))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Телефон: {c.Person.PhoneNumber}"))));
                            body.AppendChild(new Paragraph(new Run(new Text($"Роль: {(c.Person.User.Role == "student" ? "Студент" : "Сотрудник")}"))));
                            body.AppendChild(new Paragraph());
                        }
                    }

                    body.AppendChild(new Paragraph());
                    var footer = new Paragraph(
                        new Run(new Text($"Отчёт сгенерирован автоматически. Всего записей: {requests.Count + appeals.Count + questions.Count + suggestions.Count + complaints.Count}"))
                        {
                            RunProperties = new RunProperties(new Italic(), new FontSize { Val = "18" })
                        });
                    body.AppendChild(footer);
                }

                stream.Position = 0;
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"Report_{DateTime.Now:yyyyMMdd_HHmmss}.docx");
            }
        }
    }
}
