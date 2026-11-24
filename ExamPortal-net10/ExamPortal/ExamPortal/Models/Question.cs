namespace ExamPortal.Models;

public class Question
{
    public int Id { get; set; }
    public string Text { get; set; } = "";
    // Choices are always "True" and "False"
    public List<string> Choices { get; } = new() { "True", "False" };
    public string CorrectAnswer { get; set; } = "True";
}
