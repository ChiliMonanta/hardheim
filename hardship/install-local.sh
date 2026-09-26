
#!/usr/bin/env bash
set -euo pipefail

export VALHEIM_INSTALL="/workspace/dependencies/valheim-steam/BepInEx/plugins/Hardship"

# Install debug build on localhost
echo "Install debug build"
mkdir -p $VALHEIM_INSTALL
cp bin/Debug/net462/Hardship.dll $VALHEIM_INSTALL
echo "Done"