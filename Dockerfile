FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ExamPortal-net10/ExamPortal/ExamPortal/ExamPortal.csproj ExamPortal-net10/ExamPortal/ExamPortal/
RUN dotnet restore ExamPortal-net10/ExamPortal/ExamPortal/ExamPortal.csproj
COPY ExamPortal-net10/ ExamPortal-net10/
RUN dotnet publish ExamPortal-net10/ExamPortal/ExamPortal/ExamPortal.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_HTTP_PORTS=10000
EXPOSE 10000
ENTRYPOINT ["dotnet", "ExamPortal.dll"]
