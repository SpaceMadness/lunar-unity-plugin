# epic fail

function epic_fail() {
MESSAGE=$1
echo ""
echo "\033[31m"
echo " ####################################### "
echo "#########################################"
echo "#########################################"
echo "############  #############  ############"
echo "#########################################"
echo "#########################################"
echo "#########################################"
echo "####                                 ####"
echo "#### ### ### ### ### ### ### ### ### ####"
echo "#### ### ### ### ### ### ### ### ### ####"
echo "####                                 ####"
echo "#### ### ### ### ### ### ### ### ### ####"
echo "#### ### ### ### ### ### ### ### ### ####"
echo "####                                 ####"
echo "#########################################"
echo " ####################################### "
echo "--------------SCRIPT FAILED--------------"
echo "$MESSAGE"
echo "\033[0m"
echo ""
echo ""
echo ""
echo ""
if [ -z "$2" ] ; then
exit $2
fi

exit 1
}

function exit_if_failed() {
LAST_COMMAND_EXIT_CODE=$?
MESSAGE=$1
if [[ $LAST_COMMAND_EXIT_CODE != 0 ]] ; then
epic_fail "$MESSAGE" $LAST_COMMAND_EXIT_CODE
fi
}

function exit_if_file_not_exists() {
PATHNAME=$1
if [ ! -f "$PATHNAME" ] && [ ! -d "$PATHNAME" ]; then
epic_fail "File not exists: $PATHNAME"
fi
}

function dir_absolute_path() {
    exit_if_file_not_exists "$1"
    echo $(cd "$1"; pwd)
}

# fine print

function print_header() {
CAPTION=$1
echo ""
echo "\033[33m"
echo "$CAPTION"
echo "-----------------------------------------------------"
echo "\033[0m"
}

#filesystem

function makedir() {
dir_target=$1
if [[ ! -d "$dir_target" ]]; then
mkdir -p "$dir_target"
exit_if_failed "Can't make dir: $dir_target"
fi
}

function remove_file() {
filepath=$1
if [[ -d "$filepath" ]]; then
echo "Remove dir: $filepath"
rm -rf "$filepath"
exit_if_failed "Unable to remove directory: $filepath"
elif [[ -f "$filepath" ]]; then
echo "Remove file: $filepath"
rm -f  "$filepath"
exit_if_failed "Unable to remove file: $filepath"
fi
}

function pushdir() {
dir_target="$1"
pushd $dir_target > /dev/null
exit_if_failed "Can't push dir: $dir_target"
}

function popdir() {
popd > /dev/null
exit_if_failed "Can't pop dir"
}

PLUGIN_ASSETS_RUNTIME=$( dir_absolute_path  "../Project/Assets/Plugins/Lunar/")
PLUGIN_ASSETS_IOS=$( dir_absolute_path  "../Project/Assets/Plugins/IOS/")
PLUGIN_ASSETS_ANDROID=$( dir_absolute_path  "../Project/Assets/Plugins/Android/")
PLUGIN_ASSETS_EDITOR=$( dir_absolute_path "../Project/Assets/Editor/Lunar/")

exit_if_file_not_exists $PLUGIN_ASSETS_RUNTIME
exit_if_file_not_exists $PLUGIN_ASSETS_EDITOR

PROJECT_DIR=$1
PROJECT_DIR_ASSETS=$PROJECT_DIR/Assets

print_header "Project dir: $PROJECT_DIR"
exit_if_file_not_exists "$PROJECT_DIR"
exit_if_file_not_exists "$PROJECT_DIR_ASSETS"

makedir "$PROJECT_DIR_ASSETS"
makedir "$PROJECT_DIR_ASSETS/Plugins"
makedir "$PROJECT_DIR_ASSETS/Editor"

ln -s $PLUGIN_ASSETS_RUNTIME "$PROJECT_DIR_ASSETS/Plugins/Lunar"
exit_if_failed "Can't create plugin link"

ln -s $PLUGIN_ASSETS_EDITOR "$PROJECT_DIR_ASSETS/Editor/Lunar"
exit_if_failed "Can't create editor link"



