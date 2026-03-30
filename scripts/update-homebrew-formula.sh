#!/usr/bin/env bash

set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PROJECT_FILE="$ROOT_DIR/workitems.csproj"
TAP_DIR="${TAP_DIR:-/Users/john/Source/repos/xelseor/homebrew-ado-wi}"
FORMULA_DIR="$TAP_DIR/Formula"
FORMULA_PATH="$FORMULA_DIR/ado-wi.rb"
ARCHIVE_PATH="${1:-}"

if [[ -z "$ARCHIVE_PATH" ]]; then
  VERSION="$(sed -n 's:.*<Version>\(.*\)</Version>.*:\1:p' "$PROJECT_FILE" | head -n 1)"
  ARCHIVE_PATH="$ROOT_DIR/dist/ado-wi-$VERSION.tar.gz"
fi

if [[ ! -f "$ARCHIVE_PATH" ]]; then
  echo "Archive not found: $ARCHIVE_PATH" >&2
  exit 1
fi

VERSION="$(sed -n 's:.*<Version>\(.*\)</Version>.*:\1:p' "$PROJECT_FILE" | head -n 1)"
SHA256="$(shasum -a 256 "$ARCHIVE_PATH" | awk '{print $1}')"
ARCHIVE_URL="file://$ARCHIVE_PATH"

mkdir -p "$FORMULA_DIR"

cat > "$FORMULA_PATH" <<EOF
class AdoWi < Formula
  desc "STARC Azure DevOps work item export tool"
  homepage "https://github.com/JohnnyDevCraft/ado-wi"
  url "$ARCHIVE_URL"
  sha256 "$SHA256"
  license "MIT"
  version "$VERSION"

  depends_on "dotnet" => :build

  def install
    system "dotnet", "publish", "workitems.csproj", "-c", "Release", "-o", libexec
    bin.install_symlink libexec/"ado-wi"
  end

  test do
    output = shell_output("#{bin}/ado-wi --version")
    assert_match "$VERSION", output
  end
end
EOF

echo "Updated formula: $FORMULA_PATH"
echo "Version: $VERSION"
echo "Archive: $ARCHIVE_PATH"
echo "SHA256: $SHA256"
