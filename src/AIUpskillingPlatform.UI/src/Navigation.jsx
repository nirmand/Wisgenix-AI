import React from "react";
import "./Navigation.css";

const Navigation = ({ onNavigate }) => (
  <nav className="navigation">
    <ul>
      <li onClick={() => onNavigate("home")}>Home</li>
      <li onClick={() => onNavigate("topics")}>Topics</li>
      <li onClick={() => onNavigate("subjects")}>Subjects</li>
      <li onClick={() => onNavigate("questions")}>Questions</li>
    </ul>
  </nav>
);

export default Navigation;
