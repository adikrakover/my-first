import React, { useState, useEffect } from 'react';
import { getTasks, createTask, updateTask, deleteTask } from './service.js';

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
        setTodos(data);
        setIsLoading(false);
      } catch (err) {
        setError("Failed to fetch tasks.");
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
      const newTask = await createTask(input);
      setTodos([...todos, newTask]);
      setInput('');
    } catch (err) {
      setError("Failed to create task.");
      console.error("Error creating todo:", err);
    }
  };

  // 3. עדכון מצב השלמה (PUT) - תיקון שגיאת 400
  const updateCompleted = async (item) => {
    try {
      // *** התיקון הקריטי: הוספת ה-ID לאובייקט הנשלח לשרת ***
      const updatedItem = {
        id: item.id, // זה התיקון שמונע שגיאת 400
        name: item.name,
        isComplete: !item.isComplete,
      };

      await updateTask(item.id, updatedItem);

      // עדכון ה-UI
      setTodos(
        todos.map((todo) =>
          todo.id === item.id ? { ...todo, isComplete: !todo.isComplete } : todo
        )
      );
    } catch (err) {
      setError("Failed to update task status.");
      console.error("Error updating completed status:", err);
    }
  };

  // 4. מחיקת משימה (DELETE)
  const deleteTodo = async (id) => {
    try {
      await deleteTask(id);
      setTodos(todos.filter((todo) => todo.id !== id));
    } catch (err) {
      setError("Failed to delete task.");
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
        {todos.map((todo) => (
          <li key={todo.id} className={todo.isComplete ? 'completed' : ''}>
            <span
              className="task-text"
              onClick={() => updateCompleted(todo)}
            >
              {todo.name}
            </span>
            <div className="actions">
                <button
                    className="complete-btn"
                    onClick={() => updateCompleted(todo)}
                >
                    {todo.isComplete ? '✅' : '☐'}
                </button>
                <button
                    className="delete-btn"
                    onClick={() => deleteTodo(todo.id)}
                >
                    ❌
                </button>
            </div>
          </li>
        ))}
      </ul>
      {todos.length === 0 && !isLoading && (
        <p className="no-tasks">אין משימות להצגה. הוסף משימה!</p>
      )}
    </div>
  );
}

export default App;