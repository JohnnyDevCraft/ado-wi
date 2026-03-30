# workitems

`workitems` is a console application for pulling Azure DevOps work item context and turning it into a structured markdown document that can be refined with ChatGPT or another coding agent before implementation work begins.

The final executable name will be `ado-wi`.

## Goals
- Retrieve a target Azure DevOps work item by project and work item ID.
- Include related parent and child work items in the retrieved context.
- Include comments for all retrieved work items.
- Resolve additional referenced work items found inside comments or description text.
- Persist local configuration, including a user-supplied working/output path.
- Support both interactive menu-based usage and direct command-line execution via switches and options.
- Produce clean markdown output intended for AI-assisted specification refinement and implementation planning.

## Intended Workflow
1. User configures the tool with Azure DevOps connection details and a local output path.
2. User selects or supplies:
   - organization
   - project
   - work item ID
3. The tool retrieves:
   - target work item
   - parent work items
   - child work items
   - comments for all retrieved items
   - additional referenced work items discovered in comments or description text
4. The tool normalizes the retrieved data into a local document model.
5. The tool writes a markdown file that can be used as a starting point for further planning with an AI agent.

## Planned Commands
- `ado-wi --set-pat <PAT>`
- `ado-wi --set-org <OrgName>`
- `ado-wi --set-project <ProjectId || ProjectName>`
- `ado-wi --set-project`
- `ado-wi --get <WorkItemId>`
- `ado-wi --get <WorkItemId> --out <OutputPath>`

## Command Behavior Notes
- `--set-pat` stores the PAT in local application configuration.
- `--set-org` stores the Azure DevOps organization in local application configuration.
- `--set-project <ProjectId || ProjectName>` stores the default project directly.
- `--set-project` with no value should show a selectable list of projects available to the current PAT.
- `--get <WorkItemId>` retrieves the target work item, related work items, comments, and referenced work items, then writes a single markdown document to the default configured output path.
- `--out <OutputPath>` overrides the default output path for that export only.

## Planned Feature Areas
- Local configuration and path persistence
- Azure DevOps authentication and lookup
- Work item relationship traversal
- Comment retrieval and reference expansion
- Markdown export
- Interactive menu workflows
- Switch and option based command execution

## Non-Goals
- Direct implementation of work items in source repositories
- Rich project management features beyond retrieval and export
- Background synchronization of Azure DevOps projects
- Local database storage unless justified later

## Current Status
- Documentation baseline created
- Feature specifications drafted
- No application code implemented yet

## Repository Structure
- [`Program.cs`](/Users/john/Source/repos/xelseor/workitems/Program.cs)
- [`ConsoleUi/`](/Users/john/Source/repos/xelseor/workitems/ConsoleUi)
- [`Models/`](/Users/john/Source/repos/xelseor/workitems/Models)
- [`Services/`](/Users/john/Source/repos/xelseor/workitems/Services)
- [`Workflows/`](/Users/john/Source/repos/xelseor/workitems/Workflows)
- [`features/`](/Users/john/Source/repos/xelseor/workitems/features)

## Planning Docs
- [`ARCH.md`](/Users/john/Source/repos/xelseor/workitems/ARCH.md)
- [`DISCOVERY.md`](/Users/john/Source/repos/xelseor/workitems/DISCOVERY.md)
- [`MODEL.md`](/Users/john/Source/repos/xelseor/workitems/MODEL.md)
- [`THEME.md`](/Users/john/Source/repos/xelseor/workitems/THEME.md)
- [`AGENT.md`](/Users/john/Source/repos/xelseor/workitems/AGENT.md)
