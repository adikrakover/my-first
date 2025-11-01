
using System;
using System.Linq;
using System.Threading.Tasks; // *** חובה: מאפשר async/await ***

namespace lesson2
{
    class Program
    {
        // *** חובה: שינוי ל-async Task Main כדי לאפשר קריאות רשת ***
        static async Task Main(string[] args)
        {
            // טוען את תגיות ה-HTML לפני הכל (Singleton)
            var helper = HtmlHelper.Instance;

            var parser = new HtmlParser();

            // 🎯 הגדר/י אתר לבדיקה. example.com הוא אתר בסיסי ובטוח.
            string urlToCrawl = "http://books.toscrape.com";

            try
            {
                Console.WriteLine($"--- 1. טעינה וניתוח של: {urlToCrawl} ---");

                // טעינה וניתוח של הדף מהרשת
                var htmlRoot = await parser.ParseUrl(urlToCrawl);

                if (htmlRoot == null)
                {
                    Console.WriteLine("❌ התהליך נכשל. לא נבנה עץ.");
                    return;
                }

                Console.WriteLine($"✅ העץ נבנה בהצלחה מ-URL.");

                // --- 2. ביצוע שאילתה אמיתית ---

                // דוגמה: חיפוש כל אלמנט <p> בדף
                string query = "h3";
                var elements = parser.FindElementsByQuery(htmlRoot, query);

                Console.WriteLine($"\n--- 2. חיפוש כל ה'{query}' ({elements.Count} נמצאו) ---");

                // הצגת התוצאות
                foreach (var el in elements.Take(5))
                {
                    var snippet = el.InnerHtml?.Trim() ?? "(ריק)";
                    Console.WriteLine($"  - <{el.TagName}> | טקסט: {snippet.Substring(0, Math.Min(snippet.Length, 40))}...");
                }

                Console.WriteLine("\n--- הפרויקט פועל באופן מלא! ---");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ שגיאה בלתי צפויה בריצה: {ex.Message}");
            }
        }
    }
}