#!/bin/bash
#################################################
# deploy.sh
# ---------
#
# Deployment script on host for PurpleFriday software.
#
#################################################

function error_handler()
{
    local l_save_command=${l_save_command:-$BASH_COMMAND}
    local l_save_line=${l_save_line:-$1}
    local l_save_source=${l_save_source:-${BASH_SOURCE[1]}}

    echo ""
    echo "==================================="
    echo "An error has occurred... Exiting..."
    echo ""
    echo "[Command: ${l_save_command} Function: ${FUNCNAME[1]} Line: ${l_save_line} in ${l_save_source}]"
    echo ""
    echo "---------- Stack Trace ------------"
    for IDX in ${!FUNCNAME[*]} ; do
        echo "[$IDX] Function: ${FUNCNAME[$IDX]} in ${BASH_SOURCE[$IDX]}"
    done
    echo "==================================="    
    exit $1
}

function install_docker()
{
    l_app="docker"
    l_app_path="$(which ${l_app})" || true
    if  [[ -n ${l_app_path// } ]] ; then
        echo "${l_app} already installed..."
        return
    else
        echo "${l_app} not installed..."
    fi

    yes | apt-get update
    yes | apt install docker.io
    docker --version
}

function install_docker_compose()
{
    local l_app="docker-compose"
    local l_app_path="$(which ${l_app})" || true
    if  [[ -n ${l_app_path// } ]] ; then
        echo "${l_app} already installed..."
        return
    fi

    yes | apt purge docker-compose
    yes | curl -L https://github.com/docker/compose/releases/download/1.24.1/docker-compose-`uname -s`-`uname -m` -o /usr/local/bin/docker-compose
    chmod +x /usr/local/bin/docker-compose
    ln -sf /usr/local/bin/docker-compose /usr/bin/docker-compose
    docker-compose --version
}

function set_up_firewall()
{
    local l_app="ufw"
    local l_app_path="$(which ${l_app})" || true
    echo "${l_app} path [${l_app_path}]"
    if  [[ -n ${l_app_path// } ]] ; then
        echo "${l_app} already installed..."
        return
    fi

    yes | apt install ufw
    yes | apt autoremove
    yes | ufw allow ssh
    yes | ufw allow http
    yes | ufw allow https
    yes | apt install fail2ban
}

function take_action()
{
    # Ensure firewall is installed
    set_up_firewall

    # Ensure docker is installed
    install_docker
    install_docker_compose

    # Create required directories (if they don't already exist)
    if  [[ ! -d "./_datastore" ]] ; then mkdir _datastore ; fi
    if  [[ ! -d "./logs" ]] ;       then mkdir logs ; fi
    if  [[ ! -d "./app" ]] ;        then mkdir app ; fi    

    # Load the PurpleFriday tar files into Docker.
    docker load -i purplefriday_wa.tar
    docker load -i purplefridaytweetlistener.tar
}

function main()
{
    set -e -o errtrace       # Set up error trapping
    trap 'error_handler $LINENO' ERR
    set -u                   # Trigger a bad state if any env variables are undefined

    take_action    
}

main "$@"