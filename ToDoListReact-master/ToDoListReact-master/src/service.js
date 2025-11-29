import axios from 'axios';

// כתובת ה-API הבסיסית: נקראת ממשתנה הסביבה שהגדרנו בקובץ .env
// הערה: נשתמש ב-base URL הכללי (https://todo-api-krakover.onrender.com)
const BASE_URL = process.env.REACT_APP_API_URL;
// *** תיקון סופי וודאי: משנים ל-'/api/items' בהתאם ל-Program.cs ***
const ENDPOINT = '/api/items'; 

// 1. הגדרת כתובת ה-API כ-default
// התיקון: נגדיר את baseURL רק לכתובת השרת (BASE_URL)
// ונוסיף את ה-ENDPOINT בפונקציות ה-CRUD למטה.
axios.defaults.baseURL = BASE_URL; 

// *** זו השורה המתוקנת! ***
// הגדרת Content-Type עבור POST ו-PUT (כדי למנוע שגיאת 415 בשניהם)
axios.defaults.headers.common['Content-Type'] = 'application/json';


// 2. הוספת interceptor לטיפול בשגיאות
axios.interceptors.response.use(
  (response) => {
    // אם הסטטוס תקין, מחזירים את התשובה
    return response;
  },
  (error) => {
    // תופס שגיאות ומדפיס ללוג
    if (error.response) {
      console.error('API Response Error:', error.response.status, error.response.data);
    } else if (error.request) {
      console.error('API Request Error: No response received', error.request);
    } else {
      console.error('API Error:', error.message);
    }
    return Promise.reject(error);
  }
);

// CRUD Operations

// GET (שליפת כל המשימות)
export const getTasks = async () => {
  // עכשיו הקריאה היא לנתיב: BASE_URL + ENDPOINT -> https://...render.com/api/items
  const response = await axios.get(ENDPOINT); 
  return response.data;
};

// POST (יצירת משימה חדשה)
export const createTask = async (taskName) => {
  const newTask = {
    // שינוי שם השדה מ-name ל-Title, בהתאם לקובץ Program.cs שהשתמש ב-Title
    title: taskName, 
    isCompleted: false, // שינוי מ-isComplete ל-isCompleted
  };
  const response = await axios.post(ENDPOINT, newTask);
  return response.data;
};

// PUT (עדכון משימה קיימת)
export const updateTask = async (id, updatedTask) => {
  // הנתיב: BASE_URL + ENDPOINT + id -> https://...render.com/api/items/{id}
  const response = await axios.put(`${ENDPOINT}/${id}`, updatedTask);
  return response.data;
};

// DELETE (מחיקת משימה)
export const deleteTask = async (id) => {
  // הנתיב: BASE_URL + ENDPOINT + id -> https://...render.com/api/items/{id}
  await axios.delete(`${ENDPOINT}/${id}`);
};