# Feature Design: Project Foundation And Configuration

## Feature Name
**Project Foundation And Configuration**

## Status
Draft

## Overview
This feature establishes the application foundation for the `workitems` console tool. It defines the local configuration model, how the user stores a working/output path, and how the app initializes enough state to support both interactive and direct command execution.

## Goals
- Define the application’s foundational configuration workflow
- Persist a user-supplied output path
- Support reusable Azure DevOps defaults
- Support storing a PAT and organization in app configuration
- Support setting the default project directly or by selection
- Keep the local storage model simple and file-based

## Non-Goals
- No Azure DevOps retrieval logic in this feature
- No markdown export implementation in this feature
- No package-level release or installer work

## Current State (Baseline)
- Repository scaffold exists
- No config files or domain models exist yet
- No path persistence exists yet

## Target State
- The application can initialize a local config store
- The user can set and update the output path
- The user can store default organization and project values
- The app can load config for either menu mode or direct CLI mode

## Configuration Scope

### Required Settings
- Output root path
- Console UI provider choice
- Azure DevOps PAT
- Azure DevOps organization

### Expected Initial Settings
- Azure DevOps base URL
- Default organization
- Default project
- Authentication mode

### Optional Future Settings
- Known organizations list
- Known projects list
- Export template selection

## Local Storage Rules
- Config is stored in a user-scoped hidden app directory
- Config format is file-based, preferably JSON
- Missing config should trigger guided setup in menu mode
- Missing required config in direct CLI mode should return a clear error

## User Flows

### First-Run Setup
1. User launches app
2. App detects missing config
3. App prompts for output path and required defaults
4. App validates and saves config
5. App returns user to the main workflow

### Update Config
1. User chooses configuration from menu or CLI
2. User edits one or more values
3. App validates the values
4. App writes updated config to disk

### Direct Config Commands
- `ado-wi --set-pat <PAT>`
- `ado-wi --set-org <OrgName>`
- `ado-wi --set-project <ProjectId || ProjectName>`
- `ado-wi --set-project`

### `--set-project` Selection Behavior
1. User runs `ado-wi --set-project` with no value
2. App uses the current PAT and organization to retrieve visible projects
3. App shows a selectable list
4. User picks a project
5. App stores the selected project as default configuration

## Acceptance Criteria
- [ ] App creates and loads a local config file
- [ ] User can set a path that is persisted between runs
- [ ] User can store a PAT in local configuration
- [ ] User can store an organization in local configuration
- [ ] App supports default organization and project values
- [ ] User can set a project directly or from a selectable list
- [ ] Missing config is handled clearly in both menu and CLI mode
- [ ] Config structure is documented in the codebase

## Open Questions
- Should secrets be stored directly in config, referenced indirectly, or delegated to OS-secure storage?
- Should known projects be discovered dynamically or entered manually in v1?
