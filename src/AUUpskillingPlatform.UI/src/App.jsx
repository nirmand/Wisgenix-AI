import React from "react"
import { BrowserRouter as Router, Routes, Route } from "react-router-dom"
import Navigation from "./Navigation"
import Home from "./Home"
import Topics from "./Topics"
import Subjects from "./Subjects"
import Questions from "./Questions"
import "./App.css"

function App() {
  return (
    <Router>
      <Navigation onNavigate={() => {}} />
      <main>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/topics" element={<Topics />} />
          <Route path="/subjects" element={<Subjects />} />
          <Route path="/questions" element={<Questions />} />
        </Routes>
      </main>
    </Router>
  )
}

export default App
