

using System;
using System.Collections.Generic;
using System.Linq; // חובה עבור LINQ כמו Prepend ו-ToList
using System.Text;
using System.Threading.Tasks;

namespace lesson2
{
    // המחלקה חייבת להיות סטטית כדי להכיל פונקציות הרחבה
    public static class HtmlElementExtensions
    {
        /// <summary>
        /// פונקציית הרחבה שמחפשת אלמנטים בעץ התואמים לסלקטור הנתון.
        /// זוהי נקודת הכניסה לחיפוש.
        /// </summary>
        public static HashSet<HtmlElement> QuerySelector(this HtmlElement root, Selector selector)
        {
            var results = new HashSet<HtmlElement>();

            // *** התיקון הקריטי: ***
            // אנו קוראים לפונקציית FindMatches הסטטית רק פעם אחת, מהשורש (root).
            // הלוגיקה של FindMatches (שכוללת Descendants().Prepend(currentElement))
            // מטפלת הן בסלקטור בודד והן בשרשור (רווח).

            // יש לוודא שהפונקציה FindMatches היא סטטית בתוך מחלקת HtmlElement, 
            // ולכן אנו קוראים לה דרך שם המחלקה.
            HtmlElement.FindMatches(root, selector, results);

            return results;
        }


        // *** הערה: פונקציית FindMatches צריכה להיות ממוקמת כ-private static בתוך מחלקת HtmlElement. ***
        /*
        // דוגמה ל-FindMatches הנכונה (שבמחלקת HtmlElement):
        private static void FindMatches(HtmlElement currentElement, Selector currentSelector, HashSet<HtmlElement> results)
        {
            // ... (הקוד שהבאת עם התיקון של Descendants().Prepend(currentElement))
            // בתוך הלולאה הרקורסיבית, יש לקרוא שוב דרך שם המחלקה: HtmlElement.FindMatches(...)
        }
        */
    }
}