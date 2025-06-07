As an expert front-end developer, generate a React (Next.js) application with TypeScript, and a Tailwind CSS-based UI for an ASP.NET Core Web API project.

Goal: Create an intuitive, appealing, and highly responsive front-end for an Admin site to manage Assessments.

Key Pages/Flows:

Home Page: Clean layout with navigation for pages managing Subjects, Topics, Questions. There also needs to be LogIn button which should redirct users to login using social logins (Gmail, Facebook and LinkedIn to begin with).
Subjects Listing page: Displays subjects from /api/content/subjects in a tabular/ grid format with paging. Default page size is 10 but should be configurable. User should also be able to change page size. Display subjectName field with lable "Subject", and a link to "Topics" button for each subject. "Topics" button should navigate users to "/api/content/topics-by-subject/{subjectId}" where "subjectId" is a value in "Id" field. Each row should also have "Delete" button calling "/api/content/delete-subject/{id}".
Subject List page should also have abilities to allow users to create a new subject, edit an existing subject. Use the best practices for modern UI in 2025 and decide whether a new page will need to be created for add/edit or should it be part of List page itself. Implement functionality for the same. Creating a new subject is possible through "/api/content/create-subject", and Editing an existing subject can be done through "/api/content/{update-subject}/{id}"


Modern, minimalist design with a clean aesthetic.
Use a consistent color palette with subtle blue accents.
Prioritize user-friendliness and clear navigation.
Ensure full responsiveness for desktop, tablet, and mobile devices.
Include subtle hover effects and transition animations for a polished feel.
Implement a sticky header with navigation and a search icon.

Technical Details:
This React app needs to be created in "E:\Workspace\AIUpskillingPlatform\src\AIUpskillingPlatform.UI" folder.
Utilize Next.js for server-side rendering (SSR) or static site generation (SSG) where appropriate (e.g., subjects listing).
Use Tailwind CSS for all styling.
Integrate with the provided ASP.NET Core Web API endpoints. "https://localhost:7118/" is for development environment whereas also keep provision for QA and production environments with the help of configurations so there shouldnt be any code changes required.
Manage client-side state efficiently (e.g., using React Context API or a library like Zustand/Redux if complexity warrants).
Provide placeholder data initially if API integration details are not fully defined yet.
Structure the project with clear folders for components, pages, hooks, and utilities.
Start by generating the pages/index.tsx (or app/page.tsx for Next.js 13+) and components/Header.tsx components, focusing on the basic layout and navigation structure.
Make the application as configurable with maximum reusability of code. 
Also, make app as testable as possible so unit tests can be added in future.