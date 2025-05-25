import React, { useState } from "react";

const TopicManager = () => {
  const [newTopic, setNewTopic] = useState("");
  const [editValue, setEditValue] = useState("");
  const [topics, setTopics] = useState([]);

  const handleAdd = (e) => {
    e.preventDefault();
    if (!newTopic) return;
    setTopics([...topics, { id: Date.now(), name: newTopic }]);
    setNewTopic("");
  };

  const handleEdit = (id) => {
    const updatedTopics = topics.map((topic) =>
      topic.id === id ? { ...topic, name: editValue } : topic
    );
    setTopics(updatedTopics);
    setEditValue("");
  };

  return (
    <div>
      <form onSubmit={handleAdd} style={{ marginBottom: "1.5rem" }}>
        <input
          id="add-topic-input"
          name="add-topic"
          type="text"
          value={newTopic}
          onChange={(e) => setNewTopic(e.target.value)}
          placeholder="Add new topic"
          style={{ padding: "0.5rem", width: "250px", marginRight: "0.5rem" }}
        />
        <button type="submit" style={{ padding: "0.5rem 1rem" }}>Add</button>
      </form>

      <ul>
        {topics.map((topic) => (
          <li key={topic.id} style={{ marginBottom: "0.5rem" }}>
            <span>{topic.name}</span>
            <input
              id={`edit-topic-input-${topic.id}`}
              name="edit-topic"
              value={editValue}
              onChange={(e) => setEditValue(e.target.value)}
              style={{ padding: "0.4rem", marginRight: "0.5rem" }}
            />
            <button onClick={() => handleEdit(topic.id)} style={{ padding: "0.4rem 0.8rem" }}>
              Edit
            </button>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default TopicManager;