#!/usr/bin/env bash

set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PROJECT_FILE="$ROOT_DIR/workitems.csproj"
DIST_DIR="$ROOT_DIR/dist"

VERSION="$(sed -n 's:.*<Version>\(.*\)</Version>.*:\1:p' "$PROJECT_FILE" | head -n 1)"
if [[ -z "$VERSION" ]]; then
  echo "Could not determine version from $PROJECT_FILE" >&2
  exit 1
fi

ARCHIVE_PATH="$DIST_DIR/ado-wi-$VERSION.tar.gz"

mkdir -p "$DIST_DIR"
rm -f "$ARCHIVE_PATH"

tar \
  --exclude="./.git" \
  --exclude="./bin" \
  --exclude="./obj" \
  --exclude="./dist" \
  -czf "$ARCHIVE_PATH" \
  -C "$ROOT_DIR" \
  .

SHA256="$(shasum -a 256 "$ARCHIVE_PATH" | awk '{print $1}')"

echo "Version: $VERSION"
echo "Archive: $ARCHIVE_PATH"
echo "SHA256: $SHA256"
