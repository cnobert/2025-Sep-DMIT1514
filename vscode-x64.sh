#!/usr/bin/env bash
# Open VS Code (x64 under Rosetta) in the folder this script is located in

SOURCE="${BASH_SOURCE[0]}"
while [ -h "$SOURCE" ]; do
  DIR="$(cd -P "$(dirname "$SOURCE")" && pwd)"
  SOURCE="$(readlink "$SOURCE")"
  [[ "$SOURCE" != /* ]] && SOURCE="$DIR/$SOURCE"
done
SCRIPT_DIR="$(cd -P "$(dirname "$SOURCE")" && pwd)"

# Ensure x64 .NET is first if you want it in the integrated terminal
export DOTNET_ROOT="/usr/local/share/dotnet/x64"
export PATH="$DOTNET_ROOT:$PATH"

# Launch Code under Rosetta and open the script's folder
/usr/bin/arch -x86_64 "/Applications/Visual Studio Code.app/Contents/MacOS/Electron" "$SCRIPT_DIR"