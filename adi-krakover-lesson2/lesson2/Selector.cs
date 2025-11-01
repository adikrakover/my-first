
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text; // נשמר ליתר ביטחון, למרות שאינו נחוץ ישירות כאן

namespace lesson2
{
    public class Selector
    {
        // --- מאפייני חיפוש ---
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; } = new List<string>();

        // --- מאפייני מבנה היררכיית הסלקטורים (שרשרת) ---
        public Selector Parent { get; set; }
        public Selector Child { get; set; }

        /// <summary>
        /// פונקציה סטטית להמרת מחרוזת שאילתה לעץ Selector.
        /// </summary>
        /// <param name="selectorQuery">מחרוזת הסלקטור (לדוגמה: "div .class1 #id1")</param>
        /// <returns>הסלקטור הראשון בשרשרת.</returns>
        public static Selector ParseSelector(string selectorQuery)
        {
            if (string.IsNullOrWhiteSpace(selectorQuery))
            {
                return null;
            }

            // 1. פיצול לפי רווח (לכדי רמות היררכיה - חיפוש בצאצאים)
            // .Trim() ינקה רווחים מיותרים משני הקצוות של המחרוזת כולה.
            var parts = selectorQuery.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // יצירת אוביקט שורש זמני (Dummy Root) כדי להתחיל את ההיררכיה
            var root = new Selector();
            var currentSelector = root; // מצביע לסלקטור הנוכחי שאליו נוסיף את הבן

            foreach (var part in parts)
            {
                // אם החלק ריק (למרות ה-StringSplitOptions, זו בדיקה בטוחה)
                if (string.IsNullOrWhiteSpace(part)) continue;

                var newSelector = new Selector();

                // 2. פירוק החלק לפי המפרידים # ו-.
                // Regex.Split מפריד ומשאיר את התחיליות # ו-.
                var tokens = Regex.Split(part, @"(?=[#\.])")
                                     .Where(t => !string.IsNullOrEmpty(t))
                                     .ToList();

                foreach (var token in tokens)
                {
                    if (token.StartsWith('#'))
                    {
                        // תיקון: מפעילים Trim() כדי לנקות רווחים נגררים
                        newSelector.Id = token.Substring(1).Trim();
                    }
                    else if (token.StartsWith('.'))
                    {
                        // תיקון: מפעילים Trim() כדי לנקות רווחים נגררים
                        newSelector.Classes.Add(token.Substring(1).Trim());
                    }
                    else
                    {
                        // אם זה לא # או ., זה אמור להיות שם תגית. 
                        var tagName = token.Trim(); // מנקים רווחים משם התגית

                        // בדיקת תקינות באמצעות HtmlHelper
                        if (HtmlHelper.Instance.IsTagValid(tagName))
                        {
                            newSelector.TagName = tagName.ToLower();
                        }
                    }
                }

                // 3. **יצירת היררכיה (שרשור)**: חיבור הסלקטור החדש כבן של הנוכחי
                currentSelector.Child = newSelector;
                newSelector.Parent = currentSelector;

                // מקדם את currentSelector לבא (הבן החדש)
                currentSelector = newSelector;
            }

            // החזרת הסלקטור הראשון בשרשרת (דילוג על ה-root הריק)
            return root.Child;
        }
    }
}
