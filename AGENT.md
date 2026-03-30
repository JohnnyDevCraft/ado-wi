# AGENT

## Project
- Name: `workitems`
- Type: .NET console application
- Purpose: Retrieve Azure DevOps work item details and generate markdown handoff documents that can be refined with ChatGPT or other coding agents before implementation.

## Current State
- Repository scaffold exists with:
  - `Program.cs`
  - `ConsoleUi/`
  - `Models/`
  - `Services/`
  - `Workflows/`
  - `scripts/`
  - `features/`
- No implementation work has been started in this thread.
- Initial project documentation and feature specifications were created first, per project rules.

## Decisions Recorded
- Target framework is currently `net10.0` per [`workitems.csproj`](/Users/john/Source/repos/xelseor/workitems/workitems.csproj).
- Final executable name: `ado-wi`
- The tool is a console-first application with both:
  - interactive menu workflows
  - direct command-line options and switches
- The application will persist configuration locally, including a path supplied by the user.
- The application’s core output is a markdown document containing:
  - the selected work item
  - parent work items
  - child work items
  - comments on all retrieved work items
  - additional work items referenced from comments or description text
  - enough normalized context to support downstream AI-assisted specification refinement

## Open Questions
- The request specifies “Subtray” for console menus, while the existing precontext standardizes on Spectre.Console. This has been captured as an explicit architecture decision to confirm before coding.
- Azure DevOps authentication details are not finalized yet. PAT-based auth is the current assumed baseline.
- The exact markdown output template and front matter shape will be finalized during feature design approval.

## Work Completed By Agent
- Reviewed repository scaffold and precontext.
- Created required root context documents:
  - `AGENT.md`
  - `README.md`
  - `MODEL.md`
  - `THEME.md`
  - `ARCH.md`
  - `DISCOVERY.md`
- Created initial feature specifications:
  - `features/001-ProjectFoundationAndConfiguration.md`
  - `features/002-AzureDevOpsWorkItemRetrieval.md`
  - `features/003-MarkdownHandoffDocumentGeneration.md`
  - `features/004-InteractiveAndCommandLineWorkflows.md`
- Updated the docs and specs to reflect the explicit `ado-wi` command set and richer retrieval scope:
  - `--set-pat`
  - `--set-org`
  - `--set-project`
  - `--get`
  - `--out`
- Confirmed the final executable name as `ado-wi`

## Next Recommended Step
- Review and approve the documentation baseline and feature specs.
- Resolve the console UI package decision before implementation begins.
