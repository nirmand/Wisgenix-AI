import { dirname } from "path";
import { fileURLToPath } from "url";
import { FlatCompat } from "@eslint/eslintrc";

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

const compat = new FlatCompat({
  baseDirectory: __dirname,
});

const eslintConfig = [
  ...compat.extends("next/core-web-vitals", "next/typescript"),
  // Your custom rules overrides go here
  {
    // Apply these rules to all relevant files in your project
    // This is generally safe as the 'extends' configs already define file types
    // but being explicit helps understanding.
    files: ["**/*.{js,jsx,ts,tsx}"],
    rules: {
      // Treat unused variables as errors
      "@typescript-eslint/no-unused-vars": "error",

      // Ensure <a> tag for navigation is an error
      // (This rule is typically already 'error' in 'next/core-web-vitals',
      // but explicitly setting it here ensures it's an error.)
      "@next/next/no-html-link-for-pages": "error",

      // If you want to disable specific rules, you can do so here:
      // "no-console": "warn", // Example: show console.log as a warning
      // "react/react-in-jsx-scope": "off" // Another common one for Next.js app router
    },
  },
];

export default eslintConfig;
