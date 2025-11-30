import axios from 'axios';

// כתובת ה-API המלאה - זה התיקון הקריטי!
const API_URL = 'https://todo-api-krakover.onrender.com/api/items';

// הגדרת headers
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
  const response = await axios.get(API_URL); 
  return response.data;
};

// POST (יצירת משימה חדשה)
export const createTask = async (taskName) => {
  const newTask = {
    name: taskName,
    isComplete: false,
  };
  const response = await axios.post(API_URL, newTask);
  return response.data;
};

// PUT (עדכון משימה קיימת)
export const updateTask = async (id, updatedTask) => {
  const response = await axios.put(`${API_URL}/${id}`, updatedTask);
  return response.data;
};

// DELETE (מחיקת משימה)
export const deleteTask = async (id) => {
  await axios.delete(`${API_URL}/${id}`);
};