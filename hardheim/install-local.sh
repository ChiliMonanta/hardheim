
#!/usr/bin/env bash
set -euo pipefail

export VALHEIM_INSTALL="/workspace/dependencies/valheim-steam/BepInEx/plugins/HardHeim"

# Install debug build on localhost
echo "Install debug build"
cp bin/Debug/net462/HardHeim.dll $VALHEIM_INSTALL
echo "Done"