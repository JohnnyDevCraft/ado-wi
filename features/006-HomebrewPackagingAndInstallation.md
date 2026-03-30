# Feature Design: Homebrew Packaging And Installation

## Feature Name
**Homebrew Packaging And Installation**

## Status
Implemented

## Overview
This feature packages the `ado-wi` application for Homebrew distribution so the tool can be installed from the project tap. It includes creating and maintaining the Homebrew formula, publishing the first installable version as `0.1.0`, generating the hashes required by the formula, and verifying that the command can actually be installed through Homebrew.

## Goals
- Create the Homebrew formula for `ado-wi`
- Set the first packaged application version to `0.1.0`
- Generate and record the hashes required by the formula
- Ensure the formula points at the correct release artifact
- Verify that `ado-wi` can be installed from Homebrew successfully
- Keep packaging work aligned with the separate tap repository structure

## Non-Goals
- No package manager support beyond Homebrew in this feature
- No auto-update mechanism beyond normal Homebrew upgrade behavior
- No multi-platform installer strategy beyond what Homebrew packaging requires

## Current State (Baseline)
- The application source repository and Homebrew tap repository are separate
- No formula implementation has been created yet
- No packaged version has been released yet

## Target State
- The tap repository contains a working formula at `Formula/ado-wi.rb`
- The formula references version `0.1.0`
- The formula includes the required artifact URL and SHA-256 hash
- Homebrew can install `ado-wi` from the tap
- The installed command can be invoked successfully after installation

## Packaging Scope

### Required Deliverables
- Homebrew formula file: `Formula/ado-wi.rb`
- Initial packaged version: `0.1.0`
- Release artifact URL used by the formula
- SHA-256 hash for the release artifact
- Installation verification steps and results

### Repository Expectations
- Application source remains in the main repo
- Formula changes are made in the Homebrew tap repo
- Packaging docs in the application repo should point to the tap repo and formula path clearly

## Formula Requirements
- Formula name should be `ado-wi`
- Formula should install the executable exposed as `ado-wi`
- Formula should reference version `0.1.0`
- Formula should include the correct `sha256` value for the packaged artifact
- Formula should include the metadata Homebrew requires for installability
- Formula should support a successful `brew install` flow from the project tap

## Verification Requirements
- Validate the formula syntax and Homebrew install flow
- Confirm Homebrew can install the formula from the tap
- Confirm the installed `ado-wi` command starts successfully
- Confirm installed command behavior is testable at least through `ado-wi --version` or `ado-wi --help`

## User Flows

### Packaging Flow
1. Build the release artifact for version `0.1.0`
2. Generate the SHA-256 hash for the artifact
3. Create or update `Formula/ado-wi.rb` in the tap repository
4. Point the formula at the correct artifact URL and version
5. Save the formula with the computed hash

### Installation Verification Flow
1. User adds or uses the tap
2. User runs the Homebrew install command for `ado-wi`
3. Homebrew downloads the artifact and evaluates the formula
4. Homebrew installs the command
5. User runs `ado-wi --version` or `ado-wi --help` to verify the install

## Acceptance Criteria
- [ ] A feature spec exists for Homebrew packaging and installation
- [ ] `Formula/ado-wi.rb` is defined as the target formula path
- [ ] The first packaged version is documented as `0.1.0`
- [ ] The formula includes the required SHA-256 hash
- [ ] The formula is installable through Homebrew
- [ ] The installed `ado-wi` command can be executed successfully
- [ ] The packaging and installation flow is documented in the project context files

## Open Questions
- What release artifact format should the formula consume for the first release?
- Should Homebrew verification include only install plus launch, or also a lightweight smoke test command?
