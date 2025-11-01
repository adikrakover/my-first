

using System;
using System.Collections.Generic;
using System.Linq; // נדרש עבור LINQ (Prepend, Any, ToList)
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace lesson2
{
    public class HtmlElement
    {
        // מאפייני האלמנט
        public string Id { get; set; }
        public string TagName { get; set; }
        public List<string> Classes { get; set; } = new List<string>();
        public string InnerHtml { get; set; }

        // מאפייני העץ
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; } = new List<HtmlElement>();
        public bool IsSelfClosing { get; set; } // נדרש לזיהוי תגיות שנסגרות בעצמן

        // מאפייני חיפוש
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();


        /// <summary>
        /// מחזירה את כל הצאצאים של האלמנט הזה (צאצאי עומק), ללא האלמנט עצמו.
        /// </summary>
        public IEnumerable<HtmlElement> Descendants()
        {
            var queue = new Queue<HtmlElement>();

            // מוסיפים את הבנים הישירים לתור
            foreach (var child in this.Children)
            {
                queue.Enqueue(child);
            }

            while (queue.Any())
            {
                var element = queue.Dequeue();
                yield return element;

                // מוסיפים את הצאצאים של האלמנט הנוכחי לתור
                foreach (var child in element.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }

        /// <summary>
        /// בונה את עץ האלמנטים מרשימת המחרוזות שפוצלו.
        /// </summary>
        public HtmlElement BuildTree(List<string> htmlParts)
        {
            HtmlElement tempRoot = new HtmlElement { TagName = "html" };
            HtmlElement current = tempRoot;

            // המצאת תגית שורש שתחזיק את האלמנטים ברמה העליונה
            var htmlRoot = new HtmlElement { TagName = "html" };
            htmlRoot.Children.Add(tempRoot);
            tempRoot.Parent = htmlRoot;
            current = tempRoot;


            foreach (var part in htmlParts)
            {
                if (part.StartsWith("<") && part.EndsWith(">"))
                {
                    // טיפול בתגית
                    if (part.StartsWith("</"))
                    {
                        // תגית סוגרת (</tag>)
                        if (current.Parent != null)
                        {
                            current = current.Parent;
                        }
                    }
                    else
                    {
                        // תגית פותחת (<tag ...>)
                        HtmlElement newElement = HtmlHelper.Instance.CreateElement(part);
                   
                        // אם זו תגית עצמית סוגרת, נכנסים ויוצאים מיד
                        if (newElement.IsSelfClosing)
                        {
                            newElement.Parent = current;
                            current.Children.Add(newElement);
                        }
                        else
                        {
                            // תגית רגילה: ממקמים ומעבירים את current אליה
                            newElement.Parent = current;
                            current.Children.Add(newElement);
                            current = newElement;
                        }
                    }
                }
                else
                {
                    // טיפול בטקסט פנימי (InnerHtml)
                    if (current.TagName != "html") // לא מוסיפים טקסט לשורש הזמני
                    {
                        current.InnerHtml = (current.InnerHtml ?? "") + part;
                    }
                }
            }

            // חזרה לשורש ה-<html> האמיתי (הבן היחיד של השורש הזמני)
            return tempRoot.Children.FirstOrDefault(e => e.TagName == "html") ?? tempRoot;
        }

        /// <summary>
        /// בודקת האם האלמנט תואם לסלקטור הנתון.
        /// </summary>
        public bool IsMatch(Selector selector)
        {
            // בדיקת שם התגית (TagName) - חייבת להיות Case-Insensitive!
            bool tagMatch = string.IsNullOrEmpty(selector.TagName) ||
                            this.TagName.Equals(selector.TagName, StringComparison.OrdinalIgnoreCase);

            if (!tagMatch) return false;

            // בדיקת ID
            if (!string.IsNullOrEmpty(selector.Id) &&
                !this.Id.Equals(selector.Id, StringComparison.OrdinalIgnoreCase)) return false;

            // בדיקת מחלקות (Classes)
            if (selector.Classes.Any())
            {
                // ודא/י שכל המחלקות הנדרשות בסלקטור קיימות באלמנט.
                if (!selector.Classes.All(c => this.Classes.Contains(c))) return false;
            }

            return true;
        }

        // ------------------------------------------------------------------
        // *** לוגיקת החיפוש הסטטית שמופעלת על ידי QuerySelector ***
        // ------------------------------------------------------------------

        /// <summary>
        /// פונקציה רקורסיבית סטטית המבצעת את החיפוש בעץ ה-HTML.
        /// </summary>
        internal static void FindMatches(HtmlElement currentElement, Selector currentSelector, HashSet<HtmlElement> results)
        {
            // תנאי עצירה
            if (currentSelector == null)
            {
                return;
            }

            // 1. קבלת כל המועמדים לחיפוש: האלמנט הנוכחי + כל הצאצאים שלו (עומק).
            // זה מבטיח שהחיפוש יתבצע בכל רמות העומק עבור הסלקטור הנוכחי (מאפשר דילוג על רמות).
            // **זה התיקון הקריטי לבעיית "0 נמצאו" בסלקטור בודד או משרשור**
            var candidates = currentElement.Descendants().Prepend(currentElement).ToList();

            // 2. סינון המועמדים מול הסלקטור הנוכחי
            var matchingCandidates = candidates.Where(e => e.IsMatch(currentSelector)).ToList();

            if (currentSelector.Child == null)
            {
                // 3. תנאי סיום שרשרת הסלקטורים: הוסף את התואמים לתוצאות הסופיות
                foreach (var element in matchingCandidates)
                {
                    results.Add(element);
                }

                // עצירה: מכיוון שזו התאמה סופית, אין צורך להמשיך בחיפוש הצאצאים.
                return;
            }
            else
            {
                // 4. יש סלקטורים נוספים (currentSelector.Child)
                var nextSelector = currentSelector.Child;

                // הפעל רקורסיה על כל מועמד שנמצא תואם.
                // האלמנט התואם (element) הופך להיות נקודת ההתחלה של החיפוש הבא.
                foreach (var element in matchingCandidates)
                {
                    // קריאה רקורסיבית לפונקציה הסטטית דרך שם המחלקה!
                    HtmlElement.FindMatches(element, nextSelector, results);
                }
            }
        }
    }
}
