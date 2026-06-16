# Documentation Implementation Prompt

Your task is to create and improve the project documentation only. **Do not implement or modify any application functionality.**

Analyze the entire repository, the attached requirements document, and the `spec` folder. The documentation must describe **only what is currently implemented**.

## Files to create/update

### README.md

Create a complete project documentation including:

* Project overview
* Technology stack
* Architecture overview
* Folder structure and purpose of each major folder
* Implemented features
* Authentication and security overview
* Database and seed strategy
* Configuration (`appsettings`, logging, CORS, etc.)
* Testing strategy
* Development guidelines
* Useful Mermaid diagrams where appropriate

The README should be sufficient for onboarding a new developer.

---

### QUICKSTART.md

Create a quick start guide containing:

* Prerequisites
* Backend setup
* Frontend setup
* Build commands
* Run commands
* Test commands
* Swagger URL
* Frontend URL
* Common development commands

Focus on getting the project running quickly.

---

### USERCASES.md

Create a lightweight overview of the implemented user cases.

For each user case include only:

* Title
* Short description
* Primary actor
* Expected outcome
* Reference to the corresponding file(s) in the `spec` folder

Do not duplicate acceptance criteria, business rules, or detailed flows already documented in the specifications.

Include a short introduction explaining that the `spec` folder is the source of truth for detailed functional requirements.

---

### AGENTS.md

Review and improve the existing file with project-specific guidance for AI agents.

Include:

* Architecture overview
* Folder responsibilities
* Coding conventions
* Clean Architecture rules
* Testing expectations
* Documentation standards
* Rules to avoid breaking architecture boundaries
* Rule that only functionality defined in the `spec` folder should be implemented

---

## Folder Documentation

Document the purpose of every important folder in the project, explaining its responsibility and the type of content it contains.

---

## Requirements Validation

Analyze the attached requirements document and ensure the documentation satisfies its expectations.

If a requirement is not implemented, explicitly state that it is **not implemented** instead of documenting nonexistent functionality.

## Constraints

* Documentation only.
* Do not modify source code.
* Do not implement new features.
* Do not document functionality that does not exist.
* Produce professional, consistent, and developer-friendly documentation suitable for project delivery and onboarding.
