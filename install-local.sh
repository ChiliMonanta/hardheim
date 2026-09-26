#!/usr/bin/env bash
set -euo pipefail

# Install selected files locally in the mounted Valheim directory (for testing).
# Use --help for the available arguments.

export VALHEIM_INSTALL="/workspace/dependencies/valheim-steam"
export DIST_PATH="/workspace/dist"

usage() {
	cat <<EOF
Usage: $0 [options]

Install selected packages into: $VALHEIM_INSTALL

Options:
	--bepinex-linux             Install the Linux BepInEx package
	--bepinex-windows           Install the Windows BepInEx package
	--jotunn                    Install the Jotunn plugin
	--configuration-manager     Install the Configuration Manager plugin
	--hardship                  Install the Hardship mod
	--all-windows               Install all packages on windows
	--all-linux                 Install all packages on linux
	-h, --help                  Show this help
EOF
}

install_package() {
	local package_name="$1"
	local package_path="$DIST_PATH/$package_name.zip"
	local install_path="${2:-$VALHEIM_INSTALL}"

	if [[ ! -f "$package_path" ]]; then
		printf 'Missing package: %s\n' "$package_path" >&2
		exit 1
	fi

	printf 'Installing %s\n' "$package_name"
	mkdir -p "$install_path"
	unzip -o "$package_path" -d "$install_path"
}

if (($# == 0)); then
	usage
	exit 1
fi

while (($# > 0)); do
	case "$1" in
		--bepinex-linux)
			install_package "BepInEx-linux"
			;;
		--bepinex-windows)
			install_package "BepInEx-windows"
			;;
		--jotunn)
			install_package "Jotunn"
			;;
		--configuration-manager)
			install_package "ConfigurationManager"
			;;
		--hardship)
			install_package "Hardship" "$VALHEIM_INSTALL/BepInEx/plugins/Hardship"
			;;
		--all-windows)
			install_package "BepInEx-windows"
			install_package "Jotunn"
			install_package "ConfigurationManager"
			install_package "Hardship" "$VALHEIM_INSTALL/BepInEx/plugins/Hardship"
			;;
		--all-linux)
			install_package "BepInEx-linux"
			install_package "Jotunn"
			install_package "ConfigurationManager"
			install_package "Hardship" "$VALHEIM_INSTALL/BepInEx/plugins/Hardship"
			;;
		-h|--help)
			usage
			exit 0
			;;
		*)
			printf 'Unknown argument: %s\n\n' "$1" >&2
			usage >&2
			exit 2
			;;
	esac
	shift
done

