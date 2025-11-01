
using lesson2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json; // דרוש לסריאליזציה
using System.Text.RegularExpressions;

public class HtmlHelper
{
    // משתנים שיאכלסו את רשימות התגיות
    private HashSet<string> ValidTags { get; set; } = new HashSet<string>();
    private HashSet<string> VoidTags { get; set; } = new HashSet<string>(); // לשעבר: selfClosingTags

    // --- מנגנון Singleton ---
    private static HtmlHelper _instance;
    public static HtmlHelper Instance => _instance ??= new HtmlHelper();

    // קונסטרוקטור פרטי (כדי לוודא שמופעל רק פעם אחת)
    private HtmlHelper()
    {
        LoadTagsFromFile();
    }

    /// <summary>
    /// **פונקציה מתוקנת:** טוענת את קובצי ה-JSON באמצעות בניית נתיב מלא.
    /// </summary>
    private void LoadTagsFromFile()
    {
        // 1. **התיקון לנתיב:** קבלת הנתיב המלא לתיקייה בה נמצא קובץ ה-EXE
        string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        // **התיקון לשמות הקבצים:**
        string validTagsPath = Path.Combine(directory, "HtmlTags.json");
        string voidTagsPath = Path.Combine(directory, "HtmlVoidTags.json");

        try
        {
            // --- טעינת HtmlTags.json ---
            string validTagsJson = File.ReadAllText(validTagsPath);
            // מומלץ: שינוי מ-string[] ל-HashSet<string> לשיפור ביצועי בדיקה
            ValidTags = JsonSerializer.Deserialize<HashSet<string>>(validTagsJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // --- טעינת HtmlVoidTags.json ---
            string voidTagsJson = File.ReadAllText(voidTagsPath);
            VoidTags = JsonSerializer.Deserialize<HashSet<string>>(voidTagsJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Console.WriteLine("✅ טעינת קובצי תגיות בוצעה בהצלחה.");
        }
        catch (FileNotFoundException)
        {
            // במקרה של כישלון (כדי לא לעצור את התוכנית, אבל להציף שגיאה)
            Console.WriteLine("❌ שגיאה: אחד מקובצי התגיות (HtmlTags.json או HtmlVoidTags.json) לא נמצא. ודא/י שהגדרת 'Copy to Output Directory' ל-'Copy always'.");
            // שימוש ברשימות ברירת מחדל כדי לאפשר לתוכנה להמשיך
            ValidTags = new HashSet<string> { "html", "head", "body", "div", "p", "a", "img", "br" };
            VoidTags = new HashSet<string> { "img", "br", "input", "meta", "link" };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ שגיאה בלתי צפויה בטעינת קבצי תגיות: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// בודק אם התגית היא תגית קנונית (תקפה).
    /// </summary>
    public bool IsTagValid(string tagName)
    {
        return ValidTags.Contains(tagName.ToLower());
       
    }

    /// <summary>
    /// בודק אם התגית היא תגית סגירה עצמית (Void Tag).
    /// </summary>
    public bool IsSelfClosing(string tagName)
    {
        return VoidTags.Contains(tagName.ToLower());
    }
    /// <summary>
    /// מנתח מחרוזת תגית פותחת (למשל: <div id="main" class="container">) 
    /// ויוצר ממנה אובייקט HtmlElement.
    /// </summary>
    public HtmlElement CreateElement(string htmlTag)
    {
        var element = new lesson2.HtmlElement(); // שימוש ב-namespace המלא אם יש בעיה

        // 1. ניקוי התגית והסרת הסוגריים (<, >, /)
        string cleanTag = htmlTag.Trim('<', '>', '/').Trim();

        // 2. חילוץ שם התגית (TagName)
        int firstSpace = cleanTag.IndexOf(' ');
        string tagName = firstSpace > 0 ? cleanTag.Substring(0, firstSpace) : cleanTag;

        element.TagName = tagName.ToLower();

        // בדיקה האם זו תגית סגירה עצמית
        element.IsSelfClosing = IsSelfClosing(element.TagName);

        // 3. חילוץ מאפיינים (Attributes) - אם קיימים
        if (firstSpace > 0)
        {
            string attributesString = cleanTag.Substring(firstSpace).Trim();

            // Regex לחילוץ מאפיינים (attr="val" או attr='val' או attr=val או attr)
            // זהו Regex מורכב ויעיל עבור רוב המקרים.
            var attributeRegex = new Regex(@"(\w+)=[""']?([^""'\s>]+)[""']?|(\w+)", RegexOptions.Compiled);

            foreach (Match match in attributeRegex.Matches(attributesString))
            {
                string key;
                string value;

                if (match.Groups[1].Success) // מצא מפתח=ערך
                {
                    key = match.Groups[1].Value.ToLower();
                    value = match.Groups[2].Value;
                }
                else // מצא מפתח בלבד (כמו 'disabled' או 'checked')
                {
                    key = match.Groups[3].Value.ToLower();
                    value = string.Empty; // או ערך אחר המייצג בוליאני
                }

                // חילוץ מזהה (ID) ומחלקות (Classes)
                if (key == "id")
                {
                    element.Id = value;
                }
                else if (key == "class")
                {
                    element.Classes.AddRange(value.Split(' ', StringSplitOptions.RemoveEmptyEntries));
                }
                else
                {
                    // הוספת כל שאר המאפיינים למילון
                    element.Attributes[key] = value;
                }
            }
        }

        return element;
    }
}

