"use client";

import React from "react";
import { FiSearch } from "react-icons/fi";

const navLinks = [
  { href: "/subjects", label: "Subjects" },
  { href: "/topics", label: "Topics" },
  { href: "/questions", label: "Questions" },
];

const Header: React.FC = () => {
  return (
    <header className="sticky top-0 z-30 w-full bg-white/80 backdrop-blur border-b border-gray-200 shadow-sm">
      <div className="max-w-7xl mx-auto flex items-center justify-between px-4 py-3">
        <div className="flex items-center gap-4">
          <span className="text-xl font-bold text-blue-700 tracking-tight select-none">AIUpskill Admin</span>
          <nav className="hidden md:flex gap-2 ml-6">
            {navLinks.map((link) => (
              <a
                key={link.href}
                href={link.href}
                className="px-4 py-2 rounded-lg text-gray-700 hover:bg-blue-50 hover:text-blue-700 transition-colors font-medium"
              >
                {link.label}
              </a>
            ))}
          </nav>
        </div>
        <div className="flex items-center gap-2">
          <button className="p-2 rounded-full hover:bg-blue-100 transition-colors" aria-label="Search">
            <FiSearch size={20} color="#2563eb" />
          </button>
          <button className="ml-2 px-4 py-2 rounded-lg bg-blue-600 text-white font-semibold shadow hover:bg-blue-700 transition-colors text-sm" onClick={() => {/* TODO: Add social login redirect */}}>
            Log In
          </button>
        </div>
      </div>
      {/* Mobile nav */}
      <nav className="flex md:hidden justify-center gap-2 pb-2">
        {navLinks.map((link) => (
          <a
            key={link.href}
            href={link.href}
            className="px-3 py-1 rounded text-gray-700 hover:bg-blue-50 hover:text-blue-700 transition-colors text-sm"
          >
            {link.label}
          </a>
        ))}
      </nav>
    </header>
  );
};

export default Header; 