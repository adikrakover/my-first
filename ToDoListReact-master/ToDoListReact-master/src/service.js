import axios from 'axios';

// כתובת ה-API הבסיסית: נקראת ממשתנה הסביבה שהגדרנו בקובץ .env
// הערה: נשתמש ב-base URL הכללי (https://todo-api-krakover.onrender.com)
const BASE_URL = process.env.REACT_APP_API_URL;
const ENDPOINT = '/api/Item'; // הנתיב הספציפי לטבלת המשימות

// 1. הגדרת כתובת ה-API כ-default
// axios.defaults.baseURL יהיה הכתובת המלאה כולל הנתיב
axios.defaults.baseURL = `${BASE_URL}${ENDPOINT}`; 

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
  // הקריאה היא לנתיב הבסיס שהוגדר: https://...render.com/api/Item/
  const response = await axios.get('/'); 
  return response.data;
};

// POST (יצירת משימה חדשה)
export const createTask = async (taskName) => {
  const newTask = {
    name: taskName,
    isComplete: false,
  };
  const response = await axios.post('/', newTask);
  return response.data;
};

// PUT (עדכון משימה קיימת)
export const updateTask = async (id, updatedTask) => {
  // נשלח את האובייקט המלא לעדכון. הנתיב הוא /api/Item/{id}
  const response = await axios.put(`/${id}`, updatedTask);
  return response.data;
};

// DELETE (מחיקת משימה)
export const deleteTask = async (id) => {
  // הנתיב הוא /api/Item/{id}
  await axios.delete(`/${id}`);
};