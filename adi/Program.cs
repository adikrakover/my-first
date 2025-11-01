
using System.CommandLine;
using System.Text;

var bundleCommand = new Command("bundle", "Bundle files to one file");

// ------------------ הגדרת אפשרויות עם כינויים ------------------
var languageOption = new Option<string>("--language", "Programming language (e.g., C#)")
{
    IsRequired = true
};
languageOption.AddAlias("-l");
bundleCommand.AddOption(languageOption);

var bundleOption = new Option<FileInfo>("--output", "Output bundle file");
bundleOption.AddAlias("-o");
bundleCommand.AddOption(bundleOption);

var noteOption = new Option<bool>("--note", "Include source note in bundle");
noteOption.AddAlias("-n");
bundleCommand.AddOption(noteOption);

var sortOption = new Option<string>("--sort", () => "name", "Sort files by 'name' or 'type'");
sortOption.AddAlias("-s");
bundleCommand.AddOption(sortOption);

var removeEmptyLinesOption = new Option<bool>("--remove-empty-lines", "Remove empty lines from source files");
removeEmptyLinesOption.AddAlias("-r");
bundleCommand.AddOption(removeEmptyLinesOption);

var authorOption = new Option<string>("--author", "Author name to include in bundle");
authorOption.AddAlias("-a");
bundleCommand.AddOption(authorOption);

// ------------------ Handler של הפקודה bundle ------------------
bundleCommand.SetHandler((output, note, sort, removeEmptyLines, author, language) =>
{
    // --------- בדיקות תקינות ---------
    if (string.IsNullOrEmpty(language))
    {
        Console.WriteLine("Error: Language cannot be empty.");
        return;
    }

    if (output == null)
    {
        Console.WriteLine("Error: Output file must be specified.");
        return;
    }

    try
    {
        var test = File.Create(output.FullName);
        test.Close();
        File.Delete(output.FullName);
    }
    catch
    {
        Console.WriteLine("Error: Output file path is not valid.");
        return;
    }

    if (sort != "name" && sort != "type")
    {
        Console.WriteLine("Warning: Sort must be 'name' or 'type'. Using default 'name'.");
        sort = "name";
    }

    try
    {
        using (var fs = File.Create(output.FullName))
        {
            // --------- כתיבת הערות ----------
            if (!string.IsNullOrEmpty(author))
                fs.Write(Encoding.UTF8.GetBytes($"// Created by: {author}\n"));

            if (note)
                fs.Write(Encoding.UTF8.GetBytes("// מקור הקוד: שם קובץ המקור ונתיב יחסי\n"));

            // --------- איסוף קבצים כולל סינון תיקיות ---------
            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.*", SearchOption.AllDirectories)
                                 .Where(f => f.EndsWith(language, StringComparison.OrdinalIgnoreCase))
                                 .Where(f => !f.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar) &&
                                             !f.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar) &&
                                             !f.Contains(Path.DirectorySeparatorChar + "debug" + Path.DirectorySeparatorChar))
                                 .ToList();

            // --------- מיון הקבצים ---------
            if (sort == "name")
                files.Sort();
            else
                files = files.OrderBy(f => Path.GetExtension(f)).ThenBy(f => f).ToList();

            // --------- קריאה וכתיבה ל-bundle ---------
            foreach (var file in files)
            {
                var lines = File.ReadAllLines(file);

                if (removeEmptyLines)
                    lines = lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();

                foreach (var line in lines)
                    fs.Write(Encoding.UTF8.GetBytes(line + "\n"));
            }
        }

        Console.WriteLine("Bundle created successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }

}, bundleOption, noteOption, sortOption, removeEmptyLinesOption, authorOption, languageOption);

// ------------------ פקודת create-rsp ------------------
var createRspCommand = new Command("create-rsp", "Create a response file for the bundle command");
var rspFileOption = new Option<FileInfo>("--file", "Name of the response file to create") { IsRequired = true };
createRspCommand.AddOption(rspFileOption);

createRspCommand.SetHandler((rspFile) =>
{
    // --------- קלט מהמשתמש ---------
    Console.Write("Enter language (e.g., C#): ");
    var language = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(language))
    {
        Console.WriteLine("Error: Language cannot be empty.");
        return;
    }

    Console.Write("Enter output file name: ");
    var output = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(output))
    {
        Console.WriteLine("Error: Output file must be specified.");
        return;
    }

    Console.Write("Include note? (true/false): ");
    var note = Console.ReadLine()?.Trim().ToLower() == "true";

    Console.Write("Sort by (name/type, default name): ");
    var sort = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(sort)) sort = "name";
    if (sort != "name" && sort != "type")
    {
        Console.WriteLine("Warning: Sort must be 'name' or 'type'. Using default 'name'.");
        sort = "name";
    }

    Console.Write("Remove empty lines? (true/false): ");
    var removeEmptyLines = Console.ReadLine()?.Trim().ToLower() == "true";

    Console.Write("Author name: ");
    var author = Console.ReadLine()?.Trim();

    // --------- בניית פקודה מלאה ---------
    var commandParts = new List<string> { "bundle" };

    commandParts.Add($"--language {language}");
    commandParts.Add($"--output {output}");
    if (note) commandParts.Add("--note");
    commandParts.Add($"--sort {sort}");
    if (removeEmptyLines) commandParts.Add("--remove-empty-lines");
    if (!string.IsNullOrEmpty(author)) commandParts.Add($"--author \"{author}\"");

    var finalCommand = string.Join(" ", commandParts);

    // --------- שמירה לקובץ תגובה ---------
    try
    {
        File.WriteAllText(rspFile.FullName, finalCommand);
        Console.WriteLine($"Response file created successfully: {rspFile.FullName}");
        Console.WriteLine($"Contents:\n{finalCommand}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error writing response file: {ex.Message}");
    }

}, rspFileOption);

// ------------------ הוספה ל-rootCommand ------------------
var rootCommand = new RootCommand("Command-line utility for bundling code");
rootCommand.AddCommand(bundleCommand);
rootCommand.AddCommand(createRspCommand);

rootCommand.InvokeAsync(args);





