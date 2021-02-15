#!/bin/bash
# ---------------------
# start_purplefriday.sh
# ---------------------
# - Checks if PFHOME is defined; if not defaults to $HOME for root (can also be passed in).
# - Checks directory structure, and asks the user if they want to set it up if not there (can be passed in as a flag).
# - Starts Docker containers for PurpleFriday and Monitoring.
# ---------------------
PF_TEMP=${PFHOME:-$1}                # Parameter 1: Home directory for persistent files e.g. _datastore, logs etc.
export PFHOME=${PF_TEMP:-$HOME}      #     Works in order of precednce:
                                     #        1. PFHOME environment variable (eg. an exported variable outside script)
			                         #        2. Parameter 1 inputted to this script
				                     #        3. $HOME for the user running the script.
				                     #
SETUP_DIRS="${2}"                    # Parameter 2: Should we set up the directory structure (Y/[N]). 
                                     #
RUNNING_MODE=${3:-dev}               # Parameter 3: Running mode. dev (default) or prod 
				                     #      - dev = docker-compose.development.yml - No HTTPS, to be used for development.
                                     #      - prod = docker-compose.yml             - Traefik sets up HTTPS, different ports opened.
                                     #
START_MONITORING=${4:-N}             # Parameter 4: Run Monitoring. Default (N)o.

									 #
									 # Examples:
									 #    ./start_purplefriday.sh - If directory structure is set up,
								     #                              The PFHOME environment variable is exported elsewhere
									 #                              The running mode is dev.
									 #                              Does not run monitoring.
									 #
									 #    ./start_purplefriday.sh "${HOME}/PF" "Y" "prod" "Y"
									 #                            - To set up the directory structure against ${HOME}/PF.
									 #                              And then run PurpleFriday with HTTPS. Also runs monitoring.
									 #

SEPARATOR="----------------------------"

function error_handler()
{
    echo "Script has reported an error - $1"
    exit 1
}

function setup_directories()
{
    echo "Setting up folder structure"
    if  [[ -d ${PFHOME}/logs && \
	   -d ${PFHOME}/_datastore && \
	   -d ${PFHOME}/overrides && \
	   -d ${PFHOME}/traefik && \
	   -d ${PFHOME}/Monitoring ]] ; then
        echo ${SEPARATOR}
        echo " Directory structure set up "
        echo ${SEPARATOR}	
    else
        echo ${SEPARATOR}
	echo " In order to run PurpleFriday successfully in a container, the following folder structure is needed under ${PFHOME}:"
        echo " "
	echo " > logs"
	echo " > _datastore"
	echo " > overrides"
	echo " > traefik"
	echo " > Monitoring > data"
	echo " > Monitoring > grafana-storage"
	echo ""
	echo ${SEPARATOR}
	if  [[ ${SETUP_DIRS} == "" ]] ; then
            read -p "These do not currently exist - would you like to create them (Y/[N])" SETUP_DIRS
        fi
        if  [[ ${SETUP_DIRS} != "Y" && ${SETUP_DIRS} != "y" ]] ; then
	    echo ""
	    echo " User has chosen to not setup directory structure - Exiting..."
	    echo ""
	    exit 0
	fi
	mkdir -v -p ${PFHOME}/{logs,_datastore,overrides,traefik,Monitoring}
	mkdir -v -p ${PFHOME}/Monitoring/{grafana-storage,prom-data}
        mkdir -v -p ${PFHOME}/Monitoring/grafana-storage/plugins
	chmod -R 777 ${PFHOME}/Monitoring # Give Docker permission to create folders / files under the Monitoring directory.
	find ${PFHOME} -type d   # List the folder structure under $PFHOME as mkdir -v is unreliable
    fi
}

function start_purplefriday()
{
    echo ${SEPARATOR}
    echo "Starting PurpleFriday"

    if  [[ ${RUNNING_MODE} == "dev" ]] ; then
        docker-compose -f docker-compose.development.yml up -d
    else
        docker-compose -f docker-compose.yml up -d
    fi
}

function start_monitoring()
{
    echo ${SEPARATOR}
    echo "Starting Monitoring"
    docker-compose -f ./Monitoring/docker-compose.yml up -d
    echo ${SEPARATOR}
}

function main()
{
    set -e -o errtrace
    trap 'error_handler $?' ERR

    setup_directories
    start_purplefriday

	if  [[ ${START_MONITORING} != "N" ]] ; then
            start_monitoring
	fi
}

main "$@"
