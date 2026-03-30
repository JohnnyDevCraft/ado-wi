# Console App Precontext

Use this file as a starting context document for future small .NET console tools that follow the same patterns as Commentor.

## Project Shape
- Build as a small, single-purpose C# console application.
- Do not structure it as a modular monolith.
- Prefer a small number of focused classes over deep abstraction.
- Use `.NET 10` unless there is a specific reason to target something else.
- Use `Spectre.Console` for the console interface.

## Standard Root Files
Create these files at project start:
- `AGENT.md`
- `README.md`
- `MODEL.md`
- `THEME.md`
- `ARCH.md`
- `DISCOVERY.md`
- `features/` for feature specs

## Interface Conventions
- Show a branded ASCII art splash screen on startup.
- Use color sparingly and keep the splash readable in plain text terminals.
- Start on a main menu.
- Include a `Return to Main Menu` option in every workflow.
- Clear the screen on exit.
- Treat arbitrary external text as plain text when rendering in Spectre.Console. Do not assume text from APIs, comments, or prompts is valid Spectre markup.

## Console Framework
- Use `Spectre.Console` prompts, panels, rules, tables, and status spinners.
- Prefer `SelectionPrompt<T>` for menu flows.
- Prefer `TextPrompt<string>` for user-entered values.
- Use `Status()` for network-backed loading states.
- Avoid depending on syntax/theme tokens that can throw if an unknown style name appears.

## Local Storage Pattern
- Store app state under the user home directory in a hidden folder.
- Use a config file in the form:
  - `~/.<appname>/<appname>.config`
- Persist operational state as JSON files instead of introducing a database unless requirements clearly justify it.
- Keep one JSON file per main work item when that maps cleanly to the problem space.
- Save reusable prompt output or exports to a user-configured folder path.

## Config Design Pattern
The config should typically include:
- `storageRootPath`
- primary credential or token if the project chooses direct local storage
- `promptFolderPath` if the tool generates AI handoff prompts or exports
- arrays for known organizations/projects/repos or equivalent domain references

General shape:

```json
{
  "storageRootPath": "~/.appname",
  "pat": "token-if-app-uses-one",
  "promptFolderPath": "~/Source/prompts",
  "organizations": [
    { "name": "my-org" }
  ],
  "projects": [
    {
      "organization": "my-org",
      "name": "ProjectA",
      "repos": [
        {
          "name": "RepoA",
          "path": "~/Source/repos/RepoA",
          "workItems": []
        }
      ]
    }
  ]
}
```

Adjust object names to match the domain of the new app.

## Startup Requirements
- On startup, initialize the local storage folder and config file if they do not exist.
- If required config values are missing, prompt immediately before entering the main menu.
- If the user cancels a required startup prompt, exit cleanly.

## Workflow Pattern
- Splash screen
- Ensure required configuration
- Main menu
- Guided workflow screens
- Persist state after meaningful user decisions
- Generate any final report/prompt/output
- Write final output to disk when useful

## Persistence Approach
- Use strongly typed C# models with `System.Text.Json.Serialization` attributes when field names matter.
- Use a small file-store service for `LoadOrCreate`, `Save`, and plain text export helpers.
- Keep path-building logic centralized in a path service.
- Sanitize file/folder keys for filesystem safety.

## API Client Pattern
- Treat the remote system as the source of truth.
- Use a dedicated service class for API calls.
- Normalize remote responses into local models before the UI uses them.
- Keep the UI/workflow layer responsible for orchestration, not raw HTTP details.

## Prompt Generation Pattern
- When the tool generates prompts for ChatGPT, Copilot, or other coding agents:
  - include the core record metadata
  - include the actionable items selected by the user
  - include local code context when available
  - include developer notes when captured
  - tell the coding agent to produce a plan first and wait for approval before implementation
- Write generated prompts to:
  - `<promptFolderPath>/<primary>_<secondary>_<id>.txt`

## Code Context Pattern
- If the tool works against source repositories, store a required local repo path per repo, not per project.
- When a repo is first added, prompt for the local path and persist it.
- Load a small excerpt around the relevant line when source is available.
- If the file cannot be resolved locally, continue without blocking and show a clear fallback like `Code excerpt unavailable`.

## Versioning and Packaging
- Keep the project version in the `.csproj`.
- Keep release packaging defaults aligned in scripts like `scripts/publish.sh`.
- Plan cross-platform publish targets:
  - `osx-arm64`
  - `osx-x64`
  - `linux-x64`
  - `linux-arm64`
  - `win-x64`
- Plan distribution through:
  - Homebrew
  - `.deb`
  - winget

## Homebrew Pattern
- Keep the source app repo and tap repo separate.
- Use a tap repo named like `homebrew-<appname>`.
- Publish GitHub release artifacts from the app repo.
- Point the formula at public release asset URLs.
- Verify:
  - `brew install <tap>/<formula>`
  - `<app> --version`
  - `brew test <tap>/<formula>`
  - `brew audit --strict <tap>/<formula>`

## Recommended Service Layout
- `Program.cs`
- `Models/`
- `Services/`
- `ConsoleUi/`
- `Workflows/`

## Recommended Services
- `Paths` service for all filesystem layout logic
- JSON file store service
- API client service
- code excerpt/context service when source files are relevant
- prompt builder service when the tool emits AI handoff prompts

## Documentation Pattern
Keep these updated after changes:
- `README.md`
- `AGENT.md`
- `MODEL.md`
- `THEME.md`
- `ARCH.md`
- `DISCOVERY.md`
- active feature specs in `features/`

## Notes for ADO-Based Tools
For Azure DevOps console apps specifically:
- support a stored PAT
- support known organizations
- allow project and repo selection from ADO
- persist repo local paths
- normalize ADO entities into local models
- preserve the ability to generate AI-ready outputs from stored work item data
