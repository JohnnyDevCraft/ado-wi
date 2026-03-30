# THEME

## Product Theme
- Style: practical developer tool
- Medium: terminal / console
- Primary goal: fast comprehension and low-friction navigation

## Visual Direction
- The app should feel like a purpose-built engineering utility, not a game-like terminal UI.
- Presentation should emphasize structure, status, and readability over decoration.
- Color should be used sparingly to distinguish:
  - primary actions
  - success states
  - warnings/errors
  - section headings

## Console UI Principles
- Start with a readable ASCII splash screen.
- Default to plain-text-safe rendering for any Azure DevOps content.
- Use borders, panels, tables, and rules where they improve scanning.
- Keep menus short and task-oriented.
- Always provide a clear route back to the main menu.

## Typography and Layout
- Font selection is terminal-dependent, so the app should not assume a specific font.
- Favor:
  - short menu labels
  - explicit section titles
  - consistent spacing
  - restrained use of emphasis

## Planned Screen Types
- Startup splash
- Main menu
- Configuration screens
- Work item lookup form
- Retrieval progress/status display
- Work item summary view
- Export confirmation/result view

## Interaction Design
- Menus should support keyboard-first operation.
- Direct command execution should mirror menu behavior as closely as possible.
- Error states should be actionable and specific.
- Long-running calls should show progress/status feedback.

## Theme Assets
- No custom theme assets exist yet.
- No `theme/` folder has been added yet.

## Pending Design Decisions
- Confirm whether the menu system library is actually `Subtray` or if Spectre.Console remains the chosen UI layer.
- Define the final splash screen text and brand treatment during implementation.
