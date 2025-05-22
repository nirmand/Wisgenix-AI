import React, { useState } from "react";

const initialTopics = [
  { id: 1, name: "React Basics" },
  { id: 2, name: "Advanced JavaScript" },
];

const Topics = () => {
  const [topics, setTopics] = useState(initialTopics);
  const [newTopic, setNewTopic] = useState("");
  const [editId, setEditId] = useState(null);
  const [editValue, setEditValue] = useState("");

  const handleAdd = (e) => {
    e.preventDefault();
    if (!newTopic.trim()) return;
    setTopics([
      ...topics,
      { id: Date.now(), name: newTopic.trim() },
    ]);
    setNewTopic("");
  };

  const handleDelete = (id) => {
    setTopics(topics.filter((t) => t.id !== id));
  };

  const handleEdit = (id, name) => {
    setEditId(id);
    setEditValue(name);
  };

  const handleEditSave = (id) => {
    setTopics(
      topics.map((t) => (t.id === id ? { ...t, name: editValue } : t))
    );
    setEditId(null);
    setEditValue("");
  };

  return (
    <section style={{ padding: "2rem" }}>
      <h2>Topics</h2>
      <form onSubmit={handleAdd} style={{ marginBottom: "1.5rem" }}>
        <input
          type="text"
          value={newTopic}
          onChange={(e) => setNewTopic(e.target.value)}
          placeholder="Add new topic"
          style={{ padding: "0.5rem", width: "250px", marginRight: "0.5rem" }}
        />
        <button type="submit" style={{ padding: "0.5rem 1rem" }}>Add</button>
      </form>
      <ul style={{ listStyle: "none", padding: 0 }}>
        {topics.map((topic) => (
          <li key={topic.id} style={{ marginBottom: "1rem", display: "flex", alignItems: "center" }}>
            {editId === topic.id ? (
              <>
                <input
                  value={editValue}
                  onChange={(e) => setEditValue(e.target.value)}
                  style={{ padding: "0.4rem", marginRight: "0.5rem" }}
                />
                <button onClick={() => handleEditSave(topic.id)} style={{ marginRight: "0.5rem" }}>Save</button>
                <button onClick={() => { setEditId(null); setEditValue(""); }}>Cancel</button>
              </>
            ) : (
              <>
                <span style={{ flex: 1 }}>{topic.name}</span>
                <button onClick={() => handleEdit(topic.id, topic.name)} style={{ marginRight: "0.5rem" }}>Edit</button>
                <button onClick={() => handleDelete(topic.id)}>Delete</button>
              </>
            )}
          </li>
        ))}
      </ul>
    </section>
  );
};

export default Topics;
