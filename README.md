# workitems

`workitems` is a console application for pulling Azure DevOps work item context and turning it into a structured markdown document that can be refined with ChatGPT or another coding agent before implementation work begins.

The final executable name will be `ado-wi`.

## Goals
- Retrieve a target Azure DevOps work item by project and work item ID.
- Include related parent and child work items in the retrieved context.
- Include comments for all retrieved work items.
- Retrieve all relevant description-style fields for each retrieved work item.
- Resolve additional referenced work items found inside comments or description text.
- Limit description/comment reference expansion to one layer deep.
- Persist local configuration, including a user-supplied working/output path.
- Store configuration under `~/.ADO-WI/` using `~/.ADO-WI/ADO-WI.config`.
- Render the STARC splash at startup for every execution path, including direct commands.
- Support both interactive menu-based usage and direct command-line execution via switches and options.
- Support Homebrew-based installation through the project tap.
- Produce clean markdown output intended for AI-assisted specification refinement and implementation planning.

## Intended Workflow
1. User sees the STARC splash when the tool starts.
2. User configures the tool with Azure DevOps connection details and a local output path.
3. User selects or supplies:
   - organization
   - project
   - work item ID
4. The tool retrieves:
   - target work item
   - parent work items
   - child work items
   - description-style fields for each retrieved work item
   - comments for all retrieved items
   - additional referenced work items discovered in comments or description text
   - only the first layer of text-discovered references
5. The tool normalizes the retrieved data into a local document model.
6. The tool writes a markdown file with parent, child, and related work item sections that can be used as a starting point for further planning with an AI agent.

## Planned Commands
- `ado-wi --version`
- `ado-wi --help`
- `ado-wi --set-pat <PAT>`
- `ado-wi --set-org <OrgName>`
- `ado-wi --set-project <ProjectId || ProjectName>`
- `ado-wi --set-project`
- `ado-wi --get <WorkItemId>`
- `ado-wi --get <WorkItemId> --out <OutputPath>`

## Command Behavior Notes
- `--version` shows the current application version after rendering the STARC splash.
- `--help` renders the STARC splash and then explains the available commands, options, and interactive menu mode.
- `--set-pat` stores the PAT in local application configuration at `~/.ADO-WI/ADO-WI.config`.
- `--set-org` stores the Azure DevOps organization in local application configuration at `~/.ADO-WI/ADO-WI.config`.
- `--set-project <ProjectId || ProjectName>` stores the default project directly and should persist both the resolved project ID and project name.
- `--set-project` with no value should show a selectable list of projects available to the current PAT and persist both the selected project ID and project name.
- `--get <WorkItemId>` retrieves the target work item, related work items, comments, and referenced work items, then writes a single markdown document to the default configured output path.
- Text-discovered referenced work items are expanded only one layer deep.
- `--out <OutputPath>` overrides the default output path for that export only.
- Every command path renders the STARC splash before showing command output, prompts, or errors.
- Running `ado-wi` with no direct command enters the interactive menu system.

## Planned Feature Areas
- Local configuration and path persistence
- Azure DevOps authentication and lookup
- Work item relationship traversal
- Comment retrieval and reference expansion
- Markdown export
- Interactive menu workflows
- Switch and option based command execution
- Homebrew packaging and installation

## Non-Goals
- Direct implementation of work items in source repositories
- Rich project management features beyond retrieval and export
- Background synchronization of Azure DevOps projects
- Local database storage unless justified later

## Current Status
- Core application baseline is implemented
- STARC splash, help/version, interactive menu flow, config persistence, retrieval/export pipeline, and Homebrew packaging scripts are in place
- Homebrew installation was verified locally against the tap formula for version `0.1.0`

## Repository Structure
- [`Program.cs`](/Users/john/Source/repos/xelseor/workitems/Program.cs)
- [`ConsoleUi/`](/Users/john/Source/repos/xelseor/workitems/ConsoleUi)
- [`Models/`](/Users/john/Source/repos/xelseor/workitems/Models)
- [`Services/`](/Users/john/Source/repos/xelseor/workitems/Services)
- [`Workflows/`](/Users/john/Source/repos/xelseor/workitems/Workflows)
- [`scripts/`](/Users/john/Source/repos/xelseor/workitems/scripts)
- [`features/`](/Users/john/Source/repos/xelseor/workitems/features)

## Packaging Repositories
- Application source repo: [`/Users/john/Source/repos/xelseor/workitems`](/Users/john/Source/repos/xelseor/workitems)
- Homebrew tap repo: [`/Users/john/Source/repos/xelseor/homebrew-ado-wi`](/Users/john/Source/repos/xelseor/homebrew-ado-wi)
- Planned formula path: [`/Users/john/Source/repos/xelseor/homebrew-ado-wi/Formula/ado-wi.rb`](/Users/john/Source/repos/xelseor/homebrew-ado-wi/Formula/ado-wi.rb)
- Planned initial packaged version: `0.1.0`
- Current packaging script creates [`dist/ado-wi-0.1.0.tar.gz`](/Users/john/Source/repos/xelseor/workitems/dist/ado-wi-0.1.0.tar.gz)
- Homebrew packaging includes the formula artifact hash and an install verification step
- Packaging helper scripts:
  - [`scripts/publish.sh`](/Users/john/Source/repos/xelseor/workitems/scripts/publish.sh)
  - [`scripts/update-homebrew-formula.sh`](/Users/john/Source/repos/xelseor/workitems/scripts/update-homebrew-formula.sh)

## Planning Docs
- [`ARCH.md`](/Users/john/Source/repos/xelseor/workitems/ARCH.md)
- [`DISCOVERY.md`](/Users/john/Source/repos/xelseor/workitems/DISCOVERY.md)
- [`MODEL.md`](/Users/john/Source/repos/xelseor/workitems/MODEL.md)
- [`THEME.md`](/Users/john/Source/repos/xelseor/workitems/THEME.md)
- [`AGENT.md`](/Users/john/Source/repos/xelseor/workitems/AGENT.md)
