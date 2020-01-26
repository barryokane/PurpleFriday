# ---------------------
# stop_purplefriday.sh
# ---------------------
# - Stops Docker containers for PurpleFriday and Monitoring.
# ---------------------

RUNNING_MODE=${1:-dev} 

SEPARATOR="----------------------------"
export PFHOME="$PWD"

function error_handler()
{
    echo "Script has reported an error - $1"
    exit 1
}

function stop_purplefriday()
{
    echo ${SEPARATOR}
    echo "Stopping PurpleFriday"

    if  [[ ${RUNNING_MODE} == "dev" ]] ; then
        docker-compose -f docker-compose.development.yml down
    else
        docker-compose -f docker-compose.yml down
    fi
}

function stop_monitoring()
{
    echo ${SEPARATOR}
    echo "Stopping Monitoring"
    docker-compose -f ./Monitoring/docker-compose.yml down
    echo ${SEPARATOR}
}

function main()
{
    set -e -o errtrace
    trap 'error_handler $?' ERR

    stop_purplefriday
    stop_monitoring
}

main "$@"
