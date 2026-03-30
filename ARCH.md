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
  - `ado-wi --version`
  - `ado-wi --help`
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
- Implemented with `Spectre.Console`

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
2. Show STARC splash
3. Show main menu
4. Let user configure path and defaults
5. Prompt for project and work item
6. Retrieve work item graph
7. Retrieve comments and resolve referenced work items
8. Generate markdown
8. Show output location/result

### Direct CLI Flow
1. Launch app with switches/options
2. Show STARC splash
3. Resolve whether the request is informational (`--help`, `--version`) or operational
4. For informational commands, render output without requiring app configuration
5. For operational commands, load config
6. Resolve project/work item inputs
7. Retrieve work item graph
8. Retrieve comments and resolve referenced work items
9. Generate markdown
10. Exit with clear success/failure code

### First-Run Configuration Flow
1. Launch app
2. Show STARC splash
3. Check for configured output path
4. Prompt for output path only if it is missing
5. Check for configured organization
6. Prompt for organization only if it is missing
7. Check for configured default project
8. If the default project is missing, retrieve visible projects and show a selectable list
9. Persist both the selected project ID and project name
10. Continue into the requested workflow

## Integration Assumptions
- Azure DevOps REST APIs will be used.
- Authentication is assumed to be PAT-based initially.
- Relationship traversal for v1 includes:
  - parent links
  - child links
  - textual references discovered in descriptions and comments
  - one-layer-only expansion for text-discovered references

## Retrieval Requirements
- Each retrieved work item should normalize the same core field set.
- Description content may come from multiple Azure DevOps long-text fields and should be preserved as distinct description-style fields.
- Description-style field extraction should prefer capturing all available rich-text and long-text description fields rather than only a narrow allowlist.
- HTML description field content should be converted to Markdown before markdown export generation begins.
- Parent, child, and text-referenced related items should all carry the same normalized shape into markdown generation.
- Secondary work item loads should use each work item's actual project metadata rather than assuming the configured default project for all follow-up requests.
- The related work item section should include both one-layer text references and formal Azure DevOps `Related` relations discovered on the root, parent, and child items.

## File Storage Direction
- Config and state under the user-scoped hidden folder `~/.ADO-WI/`
- Primary config file stored as `~/.ADO-WI/ADO-WI.config`
- Exports written under a configured output path
- Raw or normalized retrieval snapshots may also be persisted for traceability and offline review

## Configuration Requirements
- Default project configuration should include both project ID and project name.
- All execution modes should render the STARC splash before any prompts, status messages, results, or errors.
- `--help` and `--version` should be available even when no configuration has been created yet.

## Packaging Direction
- Keep the application source repository separate from the Homebrew tap repository.
- Current local tap repository:
  - [`/Users/john/Source/repos/xelseor/homebrew-ado-wi`](/Users/john/Source/repos/xelseor/homebrew-ado-wi)
- Planned formula location:
  - [`/Users/john/Source/repos/xelseor/homebrew-ado-wi/Formula/ado-wi.rb`](/Users/john/Source/repos/xelseor/homebrew-ado-wi/Formula/ado-wi.rb)
- Latest published release line:
  - `0.1.1`
- Current app version target:
  - `0.1.2`
- Packaging work should include:
  - release artifact generation
  - SHA-256 hash generation for the artifact
  - formula creation/update
  - Homebrew install verification
- The default formula URL strategy should use a GitHub release asset for public installs.
- Local `file://` formula URLs are acceptable only for machine-local validation.
- Current local packaging scripts:
  - [`/Users/john/Source/repos/xelseor/workitems/scripts/publish.sh`](/Users/john/Source/repos/xelseor/workitems/scripts/publish.sh)
  - [`/Users/john/Source/repos/xelseor/workitems/scripts/update-homebrew-formula.sh`](/Users/john/Source/repos/xelseor/workitems/scripts/update-homebrew-formula.sh)
- Current local formula verification uses a generated source tarball at [`/Users/john/Source/repos/xelseor/workitems/dist/ado-wi-0.1.0.tar.gz`](/Users/john/Source/repos/xelseor/workitems/dist/ado-wi-0.1.0.tar.gz)

## Risks
- Azure DevOps relation/link shapes can vary by work item type and project conventions.
- CLI and menu mode can drift if command handling is not centralized.
- Console formatting libraries can misrender or throw when external text is treated as markup.
- Secret storage needs a deliberate choice before shipping.
- Textual work item reference detection may produce false positives if parsing rules are too loose.

## Decisions To Confirm Before Further Hardening
- Secret storage strategy beyond plain local config storage
- When to switch the Homebrew formula from a local build artifact URL to a GitHub release asset URL
