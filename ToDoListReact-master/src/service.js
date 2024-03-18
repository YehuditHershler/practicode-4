import axios from 'axios';

const apiUrl = "http://localhost:5115/api/todos";

export default {
    getTasks: async () => {
    try {
      const result = await axios.get(apiUrl);
      return result.data;
    } catch (error) {
      console.error("Error getting tasks:", error);
      throw error;
    }
  },

  addTask: async (name) => {
    try {
      const result = await axios.post(apiUrl, { name });
      return result.data;
    } catch (error) {
      console.error("Error adding task:", error);
      throw error;
    }
  },

setCompleted: async(id, isComplete)=> {
    try {
      await axios.put(`${apiUrl}/${id}`, { isComplete }, { headers: { 'Content-Type': 'application/json' } });
      //await getTodos();
    } catch (error) {
      console.error(error.message);
    }
  },
  
  deleteTask: async (id) => {
    try {
      const result = await axios.delete(`${apiUrl}/${id}`);
      return result.data;
    } catch (error) {
      console.error("Error deleting task:", error);
      throw error;
    }
  }
};
