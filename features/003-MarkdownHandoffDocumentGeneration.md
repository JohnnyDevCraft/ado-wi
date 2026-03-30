# Feature Design: Markdown Handoff Document Generation

## Feature Name
**Markdown Handoff Document Generation**

## Status
Draft

## Overview
This feature generates a markdown document from a retrieved Azure DevOps work item and its immediate hierarchy. The document is intended as a clean starting point for refinement with ChatGPT or another coding agent.

## Goals
- Produce a readable markdown document from the retrieval result
- Preserve essential Azure DevOps metadata
- Present work context in a way that is useful for planning and refinement
- Save the document to the configured output path
- Allow a one-off output path override during direct CLI execution

## Non-Goals
- No direct submission to external AI services in v1
- No rich HTML export
- No opinionated implementation plan generation inside this feature

## Current State (Baseline)
- No export pipeline exists
- No markdown template is defined yet

## Target State
- The app can render a work item hierarchy into markdown
- The generated file is saved to a deterministic path
- The document is ready to be opened and refined manually with another agent

## Proposed Document Sections
- Title and work item identity
- Source metadata
- Summary
- Description / problem statement
- Parent work items
- Child work items
- Comments
- Referenced work items
- Open questions
- Notes for the next agent or refinement session

## File Naming Direction
- `<project>-<workItemId>-<slug>.md`

## Command Surface
- `ado-wi --get <WorkItemId>`
- `ado-wi --get <WorkItemId> --out <OutputPath>`

## Content Rules
- Preserve identifiers and links exactly
- Sanitize or safely render external text
- Avoid lossy shortening of titles or descriptions
- Include enough context to understand what is being requested
- Default export path comes from saved configuration
- `--out` overrides the export destination for the current command only

## Acceptance Criteria
- [ ] Markdown is generated from normalized work item data
- [ ] Output path comes from saved config unless explicitly overridden
- [ ] File naming is deterministic and safe for the filesystem
- [ ] Parent and child work items are included in the document
- [ ] Comments and referenced work items are included in the document
- [ ] Generated markdown is readable without post-processing

## Open Questions
- Should the generated document include YAML front matter?
- Should the template include a standard “implementation planning prompt” footer for downstream agents?
