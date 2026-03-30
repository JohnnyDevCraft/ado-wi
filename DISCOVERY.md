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
  - comments for all retrieved work items
  - additional referenced work items mentioned in comments or descriptions
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
- Supports a nice menu system
- Supports switches and options for direct execution
- Supports:
  - `--set-pat`
  - `--set-org`
  - `--set-project`
  - `--get`
  - `--out`
- Retrieves:
  - target work item
  - parent work items
  - child work items
  - comments for all retrieved work items
  - additional referenced work items found in comments and description text
- Generates a markdown document describing the work item and intended work
- Establish docs and feature specs before coding starts

## Modules
- Configuration
- Console UI
- Azure DevOps retrieval
- Comment and textual reference expansion
- Work item graph assembly
- Markdown export
- Command-line execution

## Planned Tables / Models
- App config
- Known Azure DevOps projects
- Work item snapshot
- Work item relations
- Markdown export record

## Use Cases
- Configure the application’s output path and Azure DevOps defaults
- Retrieve a work item interactively from the main menu
- Retrieve a work item directly from command-line options
- Select a default project interactively from projects visible to the configured PAT
- Export a work item hierarchy as a markdown planning document

## External URLs / Links
- Azure DevOps REST API documentation: to be added during implementation planning
- Console UI package documentation: to be added after package decision is confirmed

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
- `--get` scope now includes comment retrieval and text-based reference discovery, not only formal parent/child relations.

## Change Log
- 2026-03-30: Initial discovery document created from user request and existing repository scaffold.
- 2026-03-30: Updated discovery with the explicit `ado-wi` command set and expanded retrieval behavior.
