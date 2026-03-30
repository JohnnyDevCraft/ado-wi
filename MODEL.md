# MODEL

## Purpose
This document captures the current logical data model for the `workitems` console application. The application is file-backed at this stage; SQL is used here as a stable way to describe entities, fields, and relationships before implementation choices are finalized.

## Storage Direction
- Primary persistence is expected to be JSON files under a user-scoped config/storage root.
- No relational database is planned for the initial release.
- The SQL below represents the conceptual model only.

## Conceptual DDL

```sql
create table app_config (
    id integer primary key,
    storage_root_path text not null,
    output_root_path text not null,
    azure_devops_base_url text null,
    default_organization text null,
    default_project text null,
    auth_mode text not null,
    pat_secret_ref text null,
    console_ui_provider text not null,
    created_utc text not null,
    updated_utc text not null
);

create table azure_devops_project (
    id integer primary key,
    organization_name text not null,
    project_name text not null,
    is_default integer not null default 0,
    created_utc text not null,
    updated_utc text not null
);

create table work_item_snapshot (
    id integer primary key,
    organization_name text not null,
    project_name text not null,
    work_item_id integer not null,
    work_item_type text null,
    title text null,
    state text null,
    assigned_to text null,
    iteration_path text null,
    area_path text null,
    tags text null,
    url text null,
    raw_json text not null,
    retrieved_utc text not null
);

create table work_item_comment (
    id integer primary key,
    work_item_id integer not null,
    comment_id text not null,
    authored_by text null,
    rendered_text text not null,
    raw_json text not null,
    retrieved_utc text not null
);

create table work_item_relation (
    id integer primary key,
    root_work_item_id integer not null,
    related_work_item_id integer not null,
    relation_kind text not null,
    relation_direction text not null,
    retrieved_utc text not null
);

create table work_item_reference (
    id integer primary key,
    source_work_item_id integer not null,
    referenced_work_item_id integer not null,
    source_kind text not null,
    source_comment_id text null,
    reference_text text not null,
    retrieved_utc text not null
);

create table markdown_export (
    id integer primary key,
    organization_name text not null,
    project_name text not null,
    root_work_item_id integer not null,
    output_path text not null,
    template_version text not null,
    exported_utc text not null
);
```

## Entity Notes

### `app_config`
- Stores the local application configuration.
- `output_root_path` is the path the user provides and the app persists.
- `console_ui_provider` exists because the current design needs to reconcile “Subtray” versus Spectre.Console.

### `azure_devops_project`
- Stores known projects for quick selection and future defaults.

### `work_item_snapshot`
- Stores normalized metadata plus the raw Azure DevOps payload for traceability.

### `work_item_relation`
- Stores the parent/child graph captured for a retrieval run.
- `relation_kind` examples:
  - `parent`
  - `child`
  - `related`

### `work_item_comment`
- Stores comments pulled for each retrieved work item.

### `work_item_reference`
- Stores references to other work items discovered in description text or comments.
- `source_kind` examples:
  - `description`
  - `comment`

### `markdown_export`
- Tracks generated markdown handoff documents.

## Expected JSON Equivalents
- `config.json` or `<appname>.config`
- `projects.json`
- `workitems/<org>/<project>/<id>.json`
- `comments/<org>/<project>/<id>.json`
- `exports/<project>/<id>.md`

## Pending Model Decisions
- Whether secret material will be stored directly, referenced indirectly, or delegated to OS keychain storage.
- Whether snapshot history will keep only the latest version or retain multiple retrieval versions.
- Whether relation traversal should remain parent/child plus textual references for v1 or allow optional broader link types.
