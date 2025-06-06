# Contribution Guidelines for MiniBBS

## Environment Setup
- Install the .NET SDK (version 8 or newer). The main application targets .NET 6 while the tests target .NET 8.
- Run `dotnet --list-sdks` to ensure you have the required version.

## Building the Project
- Restore dependencies and build the solution from the repository root:
  ```bash
  dotnet restore
  dotnet build
  ```
- You can run the web application locally with:
  ```bash
  dotnet run --project MiniBBS
  ```

## Running Tests
- Execute the full test suite before submitting a PR:
  ```bash
  dotnet test
  ```
- The command restores, builds and runs all tests under `MiniBBS.Tests`.

## Coding Style
- Use four spaces for indentation; do not use tabs.
- Follow C# naming conventions: `PascalCase` for types and methods, `camelCase` for variables and parameters.
- Keep line length under 120 characters where possible.
- Prefer expression-bodied members for simple getters or methods.
- Add XML documentation comments for public classes and methods.

## Commit and Pull Request Guidelines
- Write commit messages in the imperative mood ("Add feature" not "Added feature").
- Each commit should contain a logical unit of work.
- Ensure `dotnet test` succeeds before committing.
- Pull requests should include a short summary of changes and describe how tests were run.
- If you make significant design decisions, document them in the PR description.
