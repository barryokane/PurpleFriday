# ---------------------
# stop_purplefriday.sh
# ---------------------
# - Stops Docker containers for PurpleFriday and Monitoring.
# ---------------------

SEPARATOR="----------------------------"

function error_handler()
{
    echo "Script has reported an error - $1"
    exit 1
}

function stop_purplefriday()
{
    echo ${SEPARATOR}
    echo "Stopping PurpleFriday"
    docker-compose down
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
