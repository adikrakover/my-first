----------------------------------------------------

    שלב 1: שלב ה-BUILD (בנייה)
    
    משתמש בתמונת בסיס של .NET SDK כדי לבנות את הקוד
    
    ----------------------------------------------------
    
    FROM https://www.google.com/url?sa=E&source=gmail&q=mcr.microsoft.com/dotnet/sdk:8.0 AS build
    WORKDIR /src
    
    העתק את קבצי הפרויקט, כולל קובץ ה-CSPROJ
    
    COPY ["TodoApi/TodoApi.csproj", "TodoApi/"]
    
    שחזר את התלויות
    
    RUN dotnet restore "TodoApi/TodoApi.csproj"
    
    העתק את שאר הקוד
    
    COPY . .
    
    בנה את הפרויקט והוצא את הקבצים הנחוצים לתיקיית 'publish'
    
    WORKDIR /src/TodoApi
    RUN dotnet publish "TodoApi.csproj" -c Release -o /app/publish
    
    ----------------------------------------------------
    
    שלב 2: שלב ה-FINAL (סופי)
    
    משתמש בתמונת בסיס קטנה יותר (ASP.NET) להרצה בלבד
    
    ----------------------------------------------------
    
    FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
    WORKDIR /app
    EXPOSE 8080
    
    העתק את הקבצים המפורסמים משלב ה-build
    
    COPY --from=build /app/publish .
    
    פקודת ההפעלה של היישום
    
    ENTRYPOINT ["dotnet", "TodoApi.dll"]