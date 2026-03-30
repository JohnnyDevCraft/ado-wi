# Feature Design: Project Foundation And Configuration

## Feature Name
**Project Foundation And Configuration**

## Status
Implemented

## Overview
This feature establishes the application foundation for the `workitems` console tool. It defines the local configuration model, how the user stores a working/output path, how the app initializes enough state to support both interactive and direct command execution, and how the shared startup splash is shown before any application output.

## Goals
- Define the application’s foundational configuration workflow
- Persist a user-supplied output path
- Support reusable Azure DevOps defaults
- Support storing a PAT and organization in app configuration
- Support setting the default project directly or by selection
- Persist the default project with both project ID and project name
- Always display the STARC splash before application output in both interactive and direct command modes
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
- The user can store the default project as both project ID and project name
- The app can load config for either menu mode or direct CLI mode
- The primary config file lives at `~/.ADO-WI/ADO-WI.config`
- The STARC splash is rendered at the top of every app run before any prompts, status, or command output

## Configuration Scope

### Required Settings
- Output root path
- Console UI provider choice
- Azure DevOps PAT
- Azure DevOps organization

### Expected Initial Settings
- Azure DevOps base URL
- Default organization
- Default project ID
- Default project name
- Authentication mode

### Optional Future Settings
- Known organizations list
- Known projects list
- Export template selection

## Local Storage Rules
- Config is stored in a user-scoped hidden app directory at `~/.ADO-WI/`
- The primary config file is `~/.ADO-WI/ADO-WI.config`
- Config format is file-based, preferably JSON
- Missing config should trigger guided setup in menu mode
- Missing required config in direct CLI mode should return a clear error

## User Flows

### First-Run Setup
1. User launches app
2. App renders the STARC splash at the top of the console
3. App checks whether the output path is configured
4. If the output path is missing, app prompts for it and saves it
5. App checks whether the Azure DevOps organization is configured
6. If the organization is missing, app prompts for it and saves it
7. App checks whether the default project is configured
8. If the default project is missing, app retrieves the visible projects for the configured organization
9. App shows a selectable project list
10. User selects a project
11. App stores both the selected project ID and the selected project name
12. App validates the resulting config and saves it
13. App returns user to the main workflow

### Splash Behavior
1. User launches app in interactive mode or with direct CLI arguments
2. App renders the STARC splash before any other output
3. App then continues with prompts, status messages, command results, or errors
4. Direct config commands and direct action commands follow the same splash-first behavior

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
5. App stores the selected project ID and project name as default configuration

## Acceptance Criteria
- [ ] App creates and loads a local config file
- [ ] App stores configuration in `~/.ADO-WI/ADO-WI.config`
- [ ] App renders the STARC splash at the top of every execution before any other output
- [ ] User can set a path that is persisted between runs
- [ ] User can store a PAT in local configuration
- [ ] User can store an organization in local configuration
- [ ] App supports default organization plus default project ID and default project name values
- [ ] User can set a project directly or from a selectable list
- [ ] First-run setup checks for missing output path, organization, and default project in sequence and prompts only for missing values
- [ ] Missing config is handled clearly in both menu and CLI mode
- [ ] Config structure is documented in the codebase

## Open Questions
- Should secrets be stored directly in config, referenced indirectly, or delegated to OS-secure storage?
- Should known projects be discovered dynamically or entered manually in v1?
