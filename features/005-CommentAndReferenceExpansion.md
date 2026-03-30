# Feature Design: Comment And Reference Expansion

## Feature Name
**Comment And Reference Expansion**

## Status
Draft

## Overview
This feature extends the base Azure DevOps retrieval flow to gather comments for every retrieved work item and to discover additional work item references embedded in comment text or work item descriptions.

## Goals
- Retrieve comments for the root work item and all linked parent/child work items
- Parse text content for work item references
- Retrieve referenced work items when the reference can be identified confidently
- Feed that expanded context into markdown generation

## Non-Goals
- No unbounded recursive graph crawl
- No full work item history retrieval
- No NLP-heavy interpretation of ambiguous text references

## Current State (Baseline)
- This behavior is planned but not implemented

## Target State
- The system includes comment content in the normalized retrieval result
- The system can detect work item references in:
  - descriptions
  - comments
- The system retrieves those referenced work items and marks their source

## Reference Detection Rules (Initial Direction)
- Prefer explicit numeric work item references
- Prefer references that appear with recognizable work item context
- Avoid weak matches that could produce unrelated records
- Mark unresolved or ambiguous references in the export instead of silently discarding them

## Data To Capture
- Source work item
- Source field or comment
- Referenced work item ID
- Retrieval success/failure

## Acceptance Criteria
- [ ] Comments are retrieved for each successfully loaded work item
- [ ] Description text is scanned for references
- [ ] Comment text is scanned for references
- [ ] Confidently detected referenced items are retrieved
- [ ] Ambiguous or failed references are surfaced in the resulting model/export

## Risks
- Comment markup and formatting differences may make parsing inconsistent
- Weak ID parsing rules could retrieve unrelated work items
- Repeated references could cause duplicate retrievals unless deduplicated
