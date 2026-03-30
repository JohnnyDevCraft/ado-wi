# Feature Design: Interactive And Command-Line Workflows

## Feature Name
**Interactive And Command-Line Workflows**

## Status
Implemented

## Overview
This feature defines the two primary operating modes of the application: interactive menu-driven workflows and direct command execution via switches and options. It also defines the command-line discovery surfaces used to explain the app, including `--help` and `--version`.

## Goals
- Support a guided menu system for exploratory use
- Support direct execution for scripted or repeatable use
- Keep both modes aligned to the same underlying workflows
- Support a built-in help surface that explains commands, options, and interactive mode
- Support a direct version query for packaging, diagnostics, and support
- Make core tasks reachable in as few steps as possible

## Non-Goals
- No shell completion in v1
- No daemon/background mode
- No TUI complexity beyond menus, prompts, status, and summaries

## Current State (Baseline)
- No UI or CLI behavior has been implemented

## Target State
- App starts in a clear main menu when launched without direct execution options
- App can run directly when required options are supplied
- App can display version information on demand
- App can display a splash-first help screen describing CLI usage and interactive mode
- Both modes can:
  - configure the app
  - retrieve a work item
  - export markdown

## Menu Structure (Proposed)
1. Retrieve Work Item
2. Configure Application
3. View Last Export Result
4. Exit

## Configuration Actions (Proposed)
- Set PAT
- Set Organization
- Set Project
- Set Output Path

## Direct CLI Shape (Proposed)
- `ado-wi --version`
- `ado-wi --help`
- `ado-wi --set-pat <PAT>`
- `ado-wi --set-org <OrgName>`
- `ado-wi --set-project <ProjectId || ProjectName>`
- `ado-wi --set-project`
- `ado-wi --get <WorkItemId>`
- `ado-wi --get <WorkItemId> --out <OutputPath>`

## Help Output Requirements
- `ado-wi --help` should render the STARC splash first
- Help output should explain the purpose of the application
- Help output should list each supported command shape
- Help output should describe what each command and option does
- Help output should explain that running `ado-wi` with no direct command enters interactive menu mode
- Help output should describe the interactive menu system at a high level

## Version Output Requirements
- `ado-wi --version` should render the STARC splash first
- Version output should include the current application version number
- Version output may include additional build metadata if available, but the version number is required

## Behavior Rules
- Direct CLI mode should not require menu interaction
- Menu mode should be discoverable and forgiving
- Validation behavior should be consistent across both modes
- Exit codes should reflect success or failure in direct mode
- Running `--set-project` without a value should open project selection behavior instead of failing
- Running `--help` should not require existing configuration
- Running `--version` should not require existing configuration
- `--help` should document both direct commands and the interactive menu workflow

## Console UI Requirement
- The user requested a “nice menu” experience using `Subtray`
- Existing project precontext recommends `Spectre.Console`
- Final implementation must confirm which library is authoritative before coding proceeds

## Acceptance Criteria
- [ ] App supports an interactive main menu
- [ ] App supports direct execution by switches/options
- [ ] App supports `--version`
- [ ] App supports `--help`
- [ ] Both modes share the same underlying retrieval/export behavior
- [ ] Configuration can be updated from both modes where appropriate
- [ ] Project selection can be launched from CLI when `--set-project` has no explicit value
- [ ] `--help` renders the STARC splash and documents commands, options, and interactive mode
- [ ] `--version` renders the STARC splash and shows the application version
- [ ] Errors are clear and actionable in both modes

## Open Questions
- Should the app auto-enter direct mode when enough arguments are provided, or require an explicit command verb?
- Should “last export result” be tracked in config or derived from export history?
