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
- The application baseline is now implemented.
- Initial project documentation and feature specifications were created first, then the baseline code was added.

## Decisions Recorded
- Target framework is currently `net10.0` per [`workitems.csproj`](/Users/john/Source/repos/xelseor/workitems/workitems.csproj).
- Final executable name: `ado-wi`
- Homebrew tap repository path: [`/Users/john/Source/repos/xelseor/homebrew-ado-wi`](/Users/john/Source/repos/xelseor/homebrew-ado-wi)
- The tool is a console-first application with both:
  - interactive menu workflows
  - direct command-line options and switches
- The application will persist configuration locally, including a path supplied by the user.
- Configuration is expected to live under `~/.ADO-WI/` with the main file stored as `~/.ADO-WI/ADO-WI.config`.
- The startup experience should always render a STARC splash before any prompts, command output, or errors.
- The CLI surface should include `--version` and `--help`.
- `--help` should explain command usage plus the interactive menu mode, and should not require existing config.
- The console UI implementation is using `Spectre.Console`.
- Default project configuration should store both project ID and project name.
- Retrieval should preserve multiple description-style fields per work item.
- Text-discovered work item references should only expand one layer deep.
- The distribution plan should include Homebrew packaging through the separate tap repository.
- The first Homebrew-packaged version should be `0.1.0`.
- Packaging work should include formula creation, artifact hash generation, and install verification.
- Public Homebrew installs must use a GitHub release asset URL rather than a machine-local `file://` path.
- The application’s core output is a markdown document containing:
  - the selected work item
  - parent work items
  - child work items
  - description-style fields for each retrieved work item
  - comments on all retrieved work items
  - additional work items referenced from comments or description text
  - separate markdown sections for parents, children, and related referenced work items
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
  - `features/006-HomebrewPackagingAndInstallation.md`
- Updated the docs and specs to reflect the explicit `ado-wi` command set and richer retrieval scope:
  - `--set-pat`
  - `--set-org`
  - `--set-project`
  - `--get`
  - `--out`
- Confirmed the final executable name as `ado-wi`
- Recorded the local Homebrew tap repository path for future formula work
- Updated the project docs to standardize the config location as `~/.ADO-WI/ADO-WI.config`
- Updated the foundation docs to make the splash mandatory on every run and to persist both default project ID and project name
- Updated retrieval and markdown docs to preserve multiple description-style fields and to cap text-reference traversal at one layer
- Updated workflow docs to add `--version` and `--help` as splash-first CLI discovery commands
- Added the Homebrew packaging feature spec covering formula creation, version `0.1.0`, hash generation, and installability
- Implemented the application baseline:
  - config persistence under `~/.ADO-WI/ADO-WI.config`
  - STARC splash-first startup flow
  - direct commands for `--help`, `--version`, `--set-pat`, `--set-org`, `--set-project`, and `--get`
  - interactive menu-driven configuration and retrieval flow
  - Azure DevOps work item retrieval with parent, child, comment, and one-layer text-reference expansion
  - markdown export with root, parent, child, and related work item sections
  - Homebrew publish/update scripts and a working local tap formula
- Verified:
  - `dotnet build`
  - `dotnet run -- --version`
  - `dotnet run -- --help`
  - direct config file creation in an isolated home directory
  - `brew install johnnydevcraft/ado-wi/ado-wi`
  - `brew test johnnydevcraft/ado-wi/ado-wi`

## Next Recommended Step
- Test the Azure DevOps retrieval flow with live credentials and confirm any project-specific description fields that should be promoted in exports.
- Decide when to switch the Homebrew formula from the local source tarball URL to a published GitHub release asset URL.
