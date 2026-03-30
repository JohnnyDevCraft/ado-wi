# DISCOVERY

## Project Description
This project builds a console utility for gathering Azure DevOps work item context and converting it into an initial markdown specification document that can be refined with AI tools before implementation work begins.

## Problem Statement
Work item data in Azure DevOps is often not in a form that is immediately useful for handing off to an implementation-focused AI agent. The tool should collect the relevant work item details and immediate hierarchy, then render that information into a cleaner planning document.

## Primary Outcome
- Given a project and work item ID, produce a markdown file that captures:
  - the target work item
  - parent work items
  - child work items
  - relevant description-style fields for each retrieved work item
  - comments for all retrieved work items
  - additional referenced work items mentioned in comments or descriptions
  - only the first layer of text-discovered references
  - enough context to describe the work and support planning/refinement

## Applications
- Planning implementation work
- Refining requirements with ChatGPT or similar agents
- Creating an agent-ready handoff document from Azure DevOps data

## Ubiquitous Language
- Organization: Azure DevOps organization/account scope
- Project: Azure DevOps project containing the work item
- Work Item: The target Azure DevOps record being retrieved
- Parent Work Item: A higher-level item related through parent links
- Child Work Item: A lower-level item related through child links
- Retrieval: Fetching work item data from Azure DevOps
- Export: Generating the markdown handoff document
- Output Path: User-configured path where generated documents are written

## Actors
- Primary user: developer, tech lead, or planner preparing work for implementation
- Secondary user: coding agent consuming the generated markdown handoff document

## Requirements Captured So Far
- Console application
- Final executable name: `ado-wi`
- Stores a user-supplied path in local config
- Stores configuration in `~/.ADO-WI/ADO-WI.config`
- Displays the STARC splash on every run before any other output
- Supports a nice menu system
- Supports switches and options for direct execution
- Supports:
  - `--version`
  - `--help`
  - `--set-pat`
  - `--set-org`
  - `--set-project`
  - `--get`
  - `--out`
- Retrieves:
  - target work item
  - parent work items
  - child work items
  - relevant description-style fields from multiple Azure DevOps long-text fields
  - comments for all retrieved work items
  - additional referenced work items found in comments and description text
  - only one layer of text-based references
- Generates a markdown document describing the work item and intended work
- Should be downloadable and installable through Homebrew
- Homebrew packaging should start at version `0.1.0`
- Homebrew formula should include the required artifact hash
- Establish docs and feature specs before coding starts

## Modules
- Configuration
- Console UI
- Azure DevOps retrieval
- Comment and textual reference expansion
- Work item graph assembly
- Markdown export
- Command-line execution
- Packaging / distribution

## Planned Tables / Models
- App config
- Known Azure DevOps projects
- Work item snapshot
- Work item relations
- Markdown export record

## Use Cases
- Configure the application’s output path and Azure DevOps defaults
- Check the installed application version from the command line
- View command usage help and interactive mode guidance from the command line
- Retrieve a work item interactively from the main menu
- Retrieve a work item directly from command-line options
- Select a default project interactively from projects visible to the configured PAT
- Persist the default project with both project ID and project name
- Export a work item hierarchy as a markdown planning document with separate parent, child, and related work item sections
- Install `ado-wi` through Homebrew from the project tap

## External URLs / Links
- Azure DevOps REST API documentation: to be added during implementation planning
- Console UI package documentation: to be added after package decision is confirmed

## Local Repository Links
- App repo: [`/Users/john/Source/repos/xelseor/workitems`](/Users/john/Source/repos/xelseor/workitems)
- Homebrew tap repo: [`/Users/john/Source/repos/xelseor/homebrew-ado-wi`](/Users/john/Source/repos/xelseor/homebrew-ado-wi)
- Planned formula path: [`/Users/john/Source/repos/xelseor/homebrew-ado-wi/Formula/ado-wi.rb`](/Users/john/Source/repos/xelseor/homebrew-ado-wi/Formula/ado-wi.rb)

## Attachments
- None yet

## Discovery Notes
- Repository scaffold already exists with folders aligned to the expected console app structure.
- Existing `precontext.md` recommends:
  - `.NET 10`
  - a small console-first architecture
  - file-backed local storage
  - Spectre.Console for UI
- The user’s request specifies “Subtray” for menus, so this discrepancy is captured as an open decision rather than silently normalized away.
- The direct command contract is now explicit and should drive both menu wording and CLI parsing design.
- `--help` should serve as the primary CLI discovery surface and should explain both direct commands and the menu-based interactive mode.
- `--get` scope now includes comment retrieval and text-based reference discovery, not only formal parent/child relations.
- Description retrieval must account for multiple description-style fields such as acceptance criteria and reproduction steps.
- Text-based reference traversal should stop after the first discovered referenced layer.
- The startup splash is not menu-only; it is expected to render for interactive flows and direct command invocations alike.
- First-run setup should prompt only for missing configuration values, in order: output path, organization, then default project selection.
- Default project persistence must include both project ID and project name.
- Homebrew packaging is planned through the separate tap repo and should begin with version `0.1.0`.
- Formula work must include the artifact SHA-256 and install verification through Homebrew.
- The current implementation uses `Spectre.Console` for the menu/help/status experience.
- The current Homebrew formula should be generated from a GitHub release asset URL for cross-machine installs.
- A `0.1.1` bugfix release is required to ensure related work items discovered from parent and child records are included reliably.

## Change Log
- 2026-03-30: Initial discovery document created from user request and existing repository scaffold.
- 2026-03-30: Updated discovery with the explicit `ado-wi` command set and expanded retrieval behavior.
- 2026-03-30: Recorded the local Homebrew tap repository path and planned formula location.
- 2026-03-30: Standardized the configuration storage location as `~/.ADO-WI/ADO-WI.config`.
- 2026-03-30: Updated the foundation design to require the STARC splash on every run and to store default project ID plus project name.
- 2026-03-30: Refined retrieval and markdown design to preserve multiple description-style fields and to limit text-reference expansion to one layer.
- 2026-03-30: Expanded the CLI design to include `--version` and `--help`, with splash-first help output that documents both commands and interactive mode.
- 2026-03-30: Added feature `006` for Homebrew packaging and installation, including formula creation, version `0.1.0`, hash generation, and install verification.
- 2026-03-30: Implemented the baseline application, local Homebrew packaging scripts, and verified a local Homebrew install for version `0.1.0`.
- 2026-03-30: Fixed related work item retrieval for parent and child items and prepared the `0.1.1` release.
