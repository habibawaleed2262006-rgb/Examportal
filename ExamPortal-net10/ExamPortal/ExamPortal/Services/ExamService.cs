using System.Text.Json;
using ExamPortal.Models;

namespace ExamPortal.Services;

// Singleton: in-memory data shared by admin and student (no database, kept simple).
public class ExamService
{
    private readonly List<Course> _courses = new();
    private int _nextCourseId = 1;
    private int _nextQuestionId = 1;

    private readonly string _file;
    private DateTime _loadedAt = DateTime.MinValue;

    private class Store
    {
        public int NextCourseId { get; set; } = 1;
        public int NextQuestionId { get; set; } = 1;
        public List<CourseDto> Courses { get; set; } = new();
    }
    private class CourseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public List<QuestionDto> Questions { get; set; } = new();
    }
    private class QuestionDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = "";
        public string CorrectAnswer { get; set; } = "True";
    }

    public ExamService(IWebHostEnvironment env)
    {
        string dir;
        try
        {
            dir = Path.Combine(env.ContentRootPath, "App_Data");
            Directory.CreateDirectory(dir);
        }
        catch
        {
            dir = Path.GetTempPath();
        }
        _file = Path.Combine(dir, "exam-data.json");
        lock (_courses)
        {
            Reload();
            if (_courses.Count == 0)
            {
                _courses.Add(new Course { Id = _nextCourseId++, Name = "C#" });
                Save();
            }
        }
    }

    // Data is saved to a file so it survives app restarts, and re-read when the file changes
    // (covers hosts that run more than one worker process).
    private void Reload()
    {
        try
        {
            if (!File.Exists(_file)) return;
            var stamp = File.GetLastWriteTimeUtc(_file);
            if (stamp == _loadedAt) return;
            var store = JsonSerializer.Deserialize<Store>(File.ReadAllText(_file));
            if (store == null) return;
            _courses.Clear();
            foreach (var c in store.Courses)
            {
                var course = new Course { Id = c.Id, Name = c.Name };
                foreach (var q in c.Questions)
                    course.Questions.Add(new Question { Id = q.Id, Text = q.Text, CorrectAnswer = q.CorrectAnswer });
                _courses.Add(course);
            }
            _nextCourseId = store.NextCourseId;
            _nextQuestionId = store.NextQuestionId;
            _loadedAt = stamp;
        }
        catch { }
    }

    private void Save()
    {
        try
        {
            var store = new Store { NextCourseId = _nextCourseId, NextQuestionId = _nextQuestionId };
            foreach (var c in _courses)
                store.Courses.Add(new CourseDto
                {
                    Id = c.Id, Name = c.Name,
                    Questions = c.Questions.Select(q => new QuestionDto { Id = q.Id, Text = q.Text, CorrectAnswer = q.CorrectAnswer }).ToList()
                });
            File.WriteAllText(_file, JsonSerializer.Serialize(store));
            _loadedAt = File.GetLastWriteTimeUtc(_file);
        }
        catch { }
    }

    public IReadOnlyList<Course> GetCourses()
    {
        lock (_courses) { Reload(); return _courses.ToList(); }
    }

    public Course? GetCourse(int id)
    {
        lock (_courses) { Reload(); return _courses.FirstOrDefault(c => c.Id == id); }
    }

    public Course AddCourse(string name)
    {
        lock (_courses)
        {
            Reload();
            var course = new Course { Id = _nextCourseId++, Name = name.Trim() };
            _courses.Add(course);
            Save();
            return course;
        }
    }

    public Question? AddQuestion(int courseId, string text, string correctAnswer)
    {
        lock (_courses)
        {
            Reload();
            var course = _courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return null;
            var q = new Question { Id = _nextQuestionId++, Text = text.Trim(), CorrectAnswer = correctAnswer };
            course.Questions.Add(q);
            Save();
            return q;
        }
    }

    public void DeleteQuestion(int courseId, int questionId)
    {
        lock (_courses)
        {
            Reload();
            _courses.FirstOrDefault(c => c.Id == courseId)?.Questions.RemoveAll(q => q.Id == questionId);
            Save();
        }
    }
}
