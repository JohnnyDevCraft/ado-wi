# ARCH

## Overview
`workitems` is planned as a small, single-purpose .NET console application that retrieves Azure DevOps work item context, traverses parent/child relationships, and exports an AI-ready markdown document.

The final executable name is `ado-wi`.

## Architectural Goals
- Keep the codebase small and straightforward.
- Separate console interaction, Azure DevOps retrieval, configuration, and markdown generation cleanly.
- Support both interactive workflows and non-interactive command execution without duplicating business logic.
- Persist local state in files, not a database, unless later requirements justify a change.

## Expected Layers

### Entry / Command Surface
- `Program.cs`
- Parses command-line switches/options
- Decides whether to launch menu mode or direct execution mode
- Planned direct surface:
  - `ado-wi --set-pat <PAT>`
  - `ado-wi --set-org <OrgName>`
  - `ado-wi --set-project <ProjectId || ProjectName>`
  - `ado-wi --set-project`
  - `ado-wi --get <WorkItemId>`
  - `ado-wi --get <WorkItemId> --out <OutputPath>`

### Console UI
- `ConsoleUi/`
- Menus, prompts, status displays, summaries
- Should depend on orchestration services, not on low-level Azure DevOps calls directly

### Workflow / Orchestration
- `Workflows/`
- Use-case level flows such as:
  - configure app
  - select project and work item
  - retrieve work item graph
  - export markdown

### Services
- `Services/`
- Path/config service
- File store service
- Azure DevOps API client
- Work item graph builder
- Comment retrieval and textual reference resolver
- Markdown document builder

### Models
- `Models/`
- Config models
- Azure DevOps response normalization models
- Export document models

## Key Flows

### Interactive Flow
1. Launch app
2. Show splash
3. Show main menu
4. Let user configure path and defaults
5. Prompt for project and work item
6. Retrieve work item graph
7. Retrieve comments and resolve referenced work items
8. Generate markdown
8. Show output location/result

### Direct CLI Flow
1. Launch app with switches/options
2. Load config
3. Resolve project/work item inputs
4. Retrieve work item graph
5. Retrieve comments and resolve referenced work items
6. Generate markdown
7. Exit with clear success/failure code

## Integration Assumptions
- Azure DevOps REST APIs will be used.
- Authentication is assumed to be PAT-based initially.
- Relationship traversal for v1 includes:
  - parent links
  - child links
  - textual references discovered in descriptions and comments

## File Storage Direction
- Config and state under a user-scoped hidden folder such as `~/.workitems/`
- Exports written under a configured output path
- Raw or normalized retrieval snapshots may also be persisted for traceability and offline review

## Risks
- Azure DevOps relation/link shapes can vary by work item type and project conventions.
- CLI and menu mode can drift if command handling is not centralized.
- Console formatting libraries can misrender or throw when external text is treated as markup.
- Secret storage needs a deliberate choice before shipping.
- Textual work item reference detection may produce false positives if parsing rules are too loose.

## Decisions To Confirm Before Coding
- Console UI package:
  - user request says `Subtray`
  - project precontext says `Spectre.Console`
- Command-line parsing approach and package selection
- Exact export template structure
- Secret storage strategy
