import axios from 'axios';

// כתובת ה-API הבסיסית
const BASE_URL = process.env.REACT_APP_API_URL || 'https://todo-api-krakover.onrender.com';
const ENDPOINT = '/api/items'; 

// הגדרת baseURL
axios.defaults.baseURL = BASE_URL; 
axios.defaults.headers.common['Content-Type'] = 'application/json';

// Interceptor לטיפול בשגיאות
axios.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
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
  const response = await axios.get(ENDPOINT); 
  return response.data;
};

// POST (יצירת משימה חדשה)
export const createTask = async (taskName) => {
  const newTask = {
    name: taskName,        // ✅ שינוי מ-title ל-name
    isComplete: false,     // ✅ שינוי מ-isCompleted ל-isComplete
  };
  const response = await axios.post(ENDPOINT, newTask);
  return response.data;
};

// PUT (עדכון משימה קיימת)
export const updateTask = async (id, updatedTask) => {
  const response = await axios.put(`${ENDPOINT}/${id}`, updatedTask);
  return response.data;
};

// DELETE (מחיקת משימה)
export const deleteTask = async (id) => {
  await axios.delete(`${ENDPOINT}/${id}`);
};