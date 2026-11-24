Exam Portal (MudBlazor, .NET 10)

Run:
 1) Install .NET 10 SDK
 2) Open ExamPortal.sln in Visual Studio and press F5
    or in a terminal inside ExamPortal/ExamPortal run: dotnet run
 3) Open the link shown in the console (http://localhost:5128)

Accounts:
 admin   / admin123
 student / student123

Flow: login as admin -> add course -> add 3 True/False questions with the correct answer -> Logout
      login as student -> choose course -> Start Exam -> Submit -> see score and answers
(Data is kept in memory, it resets when the app restarts.)
