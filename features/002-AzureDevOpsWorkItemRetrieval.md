# Feature Design: Azure DevOps Work Item Retrieval

## Feature Name
**Azure DevOps Work Item Retrieval**

## Status
Implemented

## Overview
This feature retrieves a target Azure DevOps work item and resolves its parent and child work item relationships so the application can build a structured planning document. Retrieval must include the relevant description-style fields for each retrieved work item and should expand text-based references only one layer deep.

## Goals
- Retrieve a work item by project and work item ID
- Include parent work items
- Include child work items
- Include comments for all retrieved work items
- Retrieve all relevant description-style fields for each retrieved work item
- Expand retrieval to work items referenced in description or comment text
- Limit description/comment reference expansion to one layer deep
- Normalize the retrieved data for downstream export

## Non-Goals
- No broad graph traversal beyond parent and child links in v1
- No recursive text-reference expansion beyond the first referenced layer
- No write-back to Azure DevOps
- No synchronization of unrelated work items

## Current State (Baseline)
- No Azure DevOps API integration exists
- No normalized work item model exists

## Target State
- The app can authenticate to Azure DevOps
- The app can retrieve a target work item
- The app can identify and retrieve immediate parent and child work items
- The app can retrieve comments for the target and related items
- The app can retrieve all configured description-style text fields for the target, parent, child, and related work items
- The app can identify work item references inside descriptions/comments and retrieve those items one layer deep
- The app can assemble the result into a local document model

## Inputs
- Organization
- Project
- Work item ID

## Retrieved Data

### Work Item Payload
- ID
- title
- type
- state
- assigned user
- tags
- description-style fields when available, including primary description and additional long-text fields such as acceptance criteria or reproduction steps
- area path
- iteration path
- Azure DevOps URL

### Related Work Items
- Parent items
- Child items
- Referenced items discovered in description/comment text
- Relationship type and direction

### Comments
- Comment ID
- authored by
- authored date when available
- rendered text

### Textual References
- Referenced work item ID
- Where the reference was found
- Whether the referenced item was successfully retrieved
- Expansion depth limited to the directly referenced items found in the target, parent, or child items' description/comment text

## Behavior Rules
- Retrieval should fail clearly when the work item cannot be found
- Missing related items should not invalidate the primary retrieval
- Missing comments should be surfaced but should not invalidate the main retrieval
- All retrieved work items should expose the same normalized field set, including description-style text fields when available
- Description-style fields may come from multiple Azure DevOps fields and should be preserved distinctly rather than collapsed into a single lossy string
- Referenced work items discovered in text should be retrieved when their IDs can be parsed confidently
- Textual reference expansion should stop after the first referenced layer and should not recurse through references found inside already referenced work items
- External text from Azure DevOps must be treated as plain text in console rendering
- The retrieved payload should be available for markdown generation

## Error Handling
- Invalid auth
- Unknown project
- Missing work item
- Network/API failure
- Partial retrieval where related items fail
- Comment retrieval failure
- Reference parsing ambiguity or false positives

## Acceptance Criteria
- [ ] User can retrieve a work item by project and ID
- [ ] Parent work items are included when present
- [ ] Child work items are included when present
- [ ] Comments are included for all successfully retrieved work items
- [ ] Description-style fields are included for the target, parent, child, and referenced work items when available
- [ ] Referenced work items in descriptions/comments are retrieved when detected
- [ ] Textual references are expanded only one layer deep
- [ ] Retrieval output is normalized into local models
- [ ] Partial failures are surfaced clearly

## Future Enhancements (Out of Scope)
- Related links beyond parent/child
- Linked pull requests or changesets
- Full change history retrieval
