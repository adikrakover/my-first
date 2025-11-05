// // import axios from 'axios';

// // const apiUrl = "https://localhost:7271"

// // export default {
// //   getTasks: async () => {
// //     const result = await axios.get(`${apiUrl}/items`)    
// //     return result.data;
// //   },

// //   addTask: async(name)=>{
// //     console.log('addTask', name)
// //     //TODO
// //     return {};
// //   },

// //   setCompleted: async(id, isComplete)=>{
// //     console.log('setCompleted', {id, isComplete})
// //     //TODO
// //     return {};
// //   },

// //   deleteTask:async()=>{
// //     console.log('deleteTask')
// //   }
// // };


// import axios from 'axios';

// // ===============================================
// // *** הגדרה 1: Config Defaults (כתובת בסיס) ***
// // ודאי שהכתובת הזו מוגדרת כ-5002:
// axios.defaults.headers.post['Content-Type'] = 'application/json';// ===============================================

// // ===============================================
// // *** הגדרה 2: Interceptor לטיפול בשגיאות ***
// // מיירט את כל התגובות מהשרת. אם יש שגיאה, הוא רושם אותה לקונסול.
// axios.interceptors.response.use(
//     (response) => {
//         // מחזיר תגובה תקינה
//         return response;
//     },
//     (error) => {
//         // טיפול בשגיאות HTTP
//         if (error.response) {
//             console.error('API Error Details:', {
//                 status: error.response.status,
//                 data: error.response.data,
//                 message: error.message
//             });
//         } else if (error.request) {
//             // השרת לא הגיב
//             console.error('No response received from API server.', error.request);
//         } else {
//             // שגיאה בהגדרת הבקשה
//             console.error('Error in request setup:', error.message);
//         }
        
//         // מעביר את השגיאה הלאה לטיפול נוסף (אם יש צורך)
//         return Promise.reject(error);
//     }
// );
// // ===============================================


// // --- פונקציות CRUD ---

// // (R)ead - שליפת כל המשימות
// export const getTasks = async () => {
//     // שולח GET ל-http://localhost:5002/api/items
//     const response = await axios.get('/'); 
//     return response.data;
// };

// // (C)reate - יצירת משימה חדשה (POST)
// export const createTask = async (task) => {
//     // שולח POST ל-http://localhost:5002/api/items
//     const response = await axios.post('/', task);
//     return response.data; // מחזיר את המשימה שנוצרה (כולל ה-ID)
// };

// // (U)pdate - עדכון משימה קיימת (PUT)
// export const updateTask = async (id, task) => {
//     // שולח PUT ל-http://localhost:5002/api/items/{id}
//     const response = await axios.put(`/${id}`, task); 
//     return response.status; 
// };

// // (D)elete - מחיקת משימה (DELETE)
// export const deleteTask = async (id) => {
//     // שולח DELETE ל-http://localhost:5002/api/items/{id}
//     const response = await axios.delete(`/${id}`);
//     return response.status; // ה-API מחזיר 204 No Content
// };


import axios from 'axios';

// 1. הגדרת כתובת ה-API כ-default
axios.defaults.baseURL = 'http://localhost:5002/api/items';

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
  // נשלח את האובייקט המלא לעדכון
  const response = await axios.put(`/${id}`, updatedTask);
  return response.data;
};

// DELETE (מחיקת משימה)
export const deleteTask = async (id) => {
  await axios.delete(`/${id}`);
};