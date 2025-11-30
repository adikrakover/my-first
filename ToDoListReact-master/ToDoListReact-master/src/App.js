import React, { useState, useEffect } from 'react';
import { getTasks, createTask, updateTask, deleteTask } from './service.js';
import './App.css';

function App() {
  const [todos, setTodos] = useState([]);
  const [input, setInput] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);

  // 1. שליפת משימות
  useEffect(() => {
    const fetchTodos = async () => {
      try {
        const data = await getTasks();
        console.log('Raw API response:', data);
        const tasksArray = Array.isArray(data) ? data : [];
        console.log('Tasks array:', tasksArray);
        setTodos(tasksArray);
        setIsLoading(false);
      } catch (err) {
        setError("Failed to fetch tasks: " + err.message);
        setIsLoading(false);
        console.error("Error fetching todos:", err);
      }
    };
    fetchTodos();
  }, []);

  // 2. יצירת משימה חדשה (POST)
  const createTodo = async (e) => {
    e.preventDefault();
    if (!input.trim()) return;

    try {
      console.log('Creating task with name:', input);
      const newTask = await createTask(input);
      console.log('Created task:', newTask);
      setTodos([...todos, newTask]);
      setInput('');
      setError(null);
    } catch (err) {
      setError("Failed to create task: " + err.message);
      console.error("Error creating todo:", err);
    }
  };

  // 3. עדכון מצב השלמה (PUT)
  const updateCompleted = async (item) => {
    try {
      const currentIsComplete = item.isComplete ?? item.IsComplete ?? false;
      const newIsComplete = !currentIsComplete;
      
      console.log('Updating task:', item.id, 'from', currentIsComplete, 'to', newIsComplete);
      
      const updatedItem = {
        id: item.id,
        name: item.name || item.Name,
        isComplete: newIsComplete,
      };

      await updateTask(item.id, updatedItem);

      // עדכון ה-state המקומי
      setTodos(
        todos.map((todo) =>
          todo.id === item.id 
            ? { ...todo, isComplete: newIsComplete, IsComplete: newIsComplete } 
            : todo
        )
      );
      setError(null);
    } catch (err) {
      setError("Failed to update task: " + err.message);
      console.error("Error updating completed status:", err);
    }
  };

  // 4. מחיקת משימה (DELETE)
  const deleteTodo = async (id) => {
    try {
      console.log('Deleting task with id:', id);
      await deleteTask(id);
      // מחק רק את המשימה עם ה-ID הספציפי
      setTodos(todos.filter((todo) => todo.id !== id));
      setError(null);
    } catch (err) {
      setError("Failed to delete task: " + err.message);
      console.error("Error deleting todo:", err);
    }
  };
  
  // תצוגה (Rendering)

  if (isLoading) {
    return <div className="loading-state">טוען משימות...</div>;
  }

  return (
    <div className="todo-app-container">
      <h1>רשימת משימות React/ASP.NET</h1>
      {error && <div className="error-message">{error}</div>}

      <form onSubmit={createTodo} className="input-form">
        <input
          type="text"
          value={input}
          onChange={(e) => setInput(e.target.value)}
          placeholder="הכנס משימה חדשה..."
        />
        <button type="submit">הוסף</button>
      </form>

      <ul className="todo-list">
        {Array.isArray(todos) && todos.map((todo) => {
          const taskName = todo.name || todo.Name || 'משימה ללא שם';
          const isComplete = todo.isComplete ?? todo.IsComplete ?? false;
          
          console.log('Rendering task:', todo.id, 'name:', taskName, 'complete:', isComplete);
          
          return (
            <li key={todo.id} className={isComplete ? 'completed' : ''}>
              <span
                className="task-text"
                onClick={() => updateCompleted(todo)}
              >
                {taskName}
              </span>
              <div className="actions">
                  <button
                      className="complete-btn"
                      onClick={() => updateCompleted(todo)}
                  >
                      {isComplete ? '✅' : '☐'}
                  </button>
                  <button
                      className="delete-btn"
                      onClick={() => deleteTodo(todo.id)}
                  >
                      ❌
                  </button>
              </div>
            </li>
          );
        })}
      </ul>
      {todos.length === 0 && !isLoading && (
        <p className="no-tasks">אין משימות להצגה. הוסף משימה!</p>
      )}
    </div>
  );
}

export default App;