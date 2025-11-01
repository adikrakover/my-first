//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace lesson2
{
    public class HtmlParser
    {
        // קריאה ל-WebClient שכתבת כדי לטעון דפים מהרשת
        private readonly WebClient _webClient = new WebClient();

        // Regex לפיצול תגיות וטקסט (ששמרת)
        private static readonly Regex HtmlRegex = new Regex(@"(<[^>]+>|[^<]+)", RegexOptions.Compiled);

        // --- פונקציות עיבוד (Parsing) ---

        public List<string> SplitHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return new List<string>();
            }

            var parts = new List<string>();
            var matches = HtmlRegex.Matches(html);

            foreach (Match match in matches)
            {
                string value = match.Value.Trim();

                if (!string.IsNullOrWhiteSpace(value))
                {
                    parts.Add(value);
                }
            }

            return parts;
        }

        /// <summary>
        /// מבצעת פיצול, בניית עץ, ומחזירה את השורש האמיתי (<html>).
        /// </summary>
        public HtmlElement ParseHtml(string html)
        {
            var htmlParts = SplitHtml(html);

            // קוראים ל-BuildTree של HtmlElement
            var tempRoot = new HtmlElement();
            var actualRoot = tempRoot.BuildTree(htmlParts);

            return actualRoot;
        }

        // --- פונקציה חדשה: טעינת URL וניתוח ---

        /// <summary>
        /// טוען דף מ-URL ומבצע ניתוח מלא של ה-HTML שלו.
        /// </summary>
        public async Task<HtmlElement> ParseUrl(string url)
        {
            string htmlContent;
            try
            {
                Console.WriteLine($"🔍 מתחיל/ה לטעון נתונים מ: {url}...");
                htmlContent = await _webClient.Load(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ שגיאת רשת בטעינת URL: {ex.Message}");
                return null;
            }

            return ParseHtml(htmlContent);
        }

        // --- פונקציות חיפוש (Query) ---

        /// <summary>
        /// מבצעת שאילתה על עץ ה-HTML באמצעות סלקטור.
        /// </summary>
        public HashSet<HtmlElement> FindElementsByQuery(HtmlElement root, string selectorQuery)
        {
            var selectorChain = Selector.ParseSelector(selectorQuery);

            if (selectorChain == null)
            {
                return new HashSet<HtmlElement>();
            }

            // ביצוע החיפוש באמצעות פונקציית ההרחבה QuerySelector
            // (הפונקציה נמצאת במחלקת HtmlElementExtensions)
            return root.QuerySelector(selectorChain);
        }
    }
}