#!/bin/bash
set -euo pipefail

HARDHEIM_VERSION="${1:-0.0.7}"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
DIST_DIR="$SCRIPT_DIR/dist"
PACKAGE_DIR="$DIST_DIR/tmp"
PACKAGE_PATH="$DIST_DIR/HardHeim.zip"

cd "$SCRIPT_DIR"

echo "# Build HardHeim $HARDHEIM_VERSION"
dotnet build "hardheim/HardHeim.csproj" \
  -c Release \
  -p:VersionPrefix="$HARDHEIM_VERSION"

cleanup() {
  rm -rf "$PACKAGE_DIR"
}
trap cleanup EXIT

echo "# Package $PACKAGE_PATH"
mkdir -p "$PACKAGE_DIR"
rm -f "$PACKAGE_PATH"
cp "hardheim/bin/Release/net462/HardHeim.dll" "$PACKAGE_DIR/"
cp hardheim/thunderstore/{CHANGELOG.md,icon.png,manifest.json,README.md} "$PACKAGE_DIR/"
sed -i "s/\$HARDHEIM_VERSION/$HARDHEIM_VERSION/g" "$PACKAGE_DIR/manifest.json"
(cd "$PACKAGE_DIR" && zip -qr "$PACKAGE_PATH" .)

echo "Created $PACKAGE_PATH"