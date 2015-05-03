#!/bin/sh
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
    if [[ ${LAST_COMMAND_EXIT_CODE} != 0 ]] ; then
        epic_fail "${MESSAGE}" ${LAST_COMMAND_EXIT_CODE}
    fi
}

MODULE_NAME=$1
MODULE_DEST=$2

./gradlew :${MODULE_NAME}:assembleDebug
exit_if_failed “Cant build ${MODULE_NAME} module project”

unzip -p ${MODULE_NAME}/build/outputs/aar/${MODULE_NAME}-debug.aar classes.jar > ${MODULE_DEST}/${MODULE_NAME}.jar
exit_if_failed "Cant update library"