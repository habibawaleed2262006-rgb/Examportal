using ExamPortal.Models;

namespace ExamPortal.Services;

// Singleton: in-memory data shared by admin and student (no database, kept simple).
public class ExamService
{
    private readonly List<Course> _courses = new();
    private int _nextCourseId = 1;
    private int _nextQuestionId = 1;

    public ExamService()
    {
        AddCourse("C#");
    }

    public IReadOnlyList<Course> GetCourses()
    {
        lock (_courses) return _courses.ToList();
    }

    public Course? GetCourse(int id)
    {
        lock (_courses) return _courses.FirstOrDefault(c => c.Id == id);
    }

    public Course AddCourse(string name)
    {
        lock (_courses)
        {
            var course = new Course { Id = _nextCourseId++, Name = name.Trim() };
            _courses.Add(course);
            return course;
        }
    }

    public Question? AddQuestion(int courseId, string text, string correctAnswer)
    {
        lock (_courses)
        {
            var course = _courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return null;
            var q = new Question { Id = _nextQuestionId++, Text = text.Trim(), CorrectAnswer = correctAnswer };
            course.Questions.Add(q);
            return q;
        }
    }

    public void DeleteQuestion(int courseId, int questionId)
    {
        lock (_courses)
        {
            _courses.FirstOrDefault(c => c.Id == courseId)?.Questions.RemoveAll(q => q.Id == questionId);
        }
    }
}
