#!/bin/bash
#################################################
# deploy.sh
# ---------
#
# Deployment script for PurpleFriday software.
#
# User has the choice of 3 options:
#    1) Build
#    2) Deploy
#    3) Restart services on remote host.
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

function check_app_installed()
{
    local l_app=$1
    local l_path="$(which ${l_app})" || true
    local l_version=""
    if  [[ ${l_path} == "" ]] ; then
        echo "    --------------------------------------------------"
        echo "    FAIL: ${l_app} not installed. Please install and re-try."
        echo "    --------------------------------------------------"
        exit
    else
        if  [[ ${l_app} == "docker"* ]] ; then
            l_version="$(${l_app} --version || true)"
        fi
        if  [[ -n "${l_version}" ]] ; then
            echo -e "    PASS: ${l_app} is installed and is using version ${l_version}"
        else
            echo -e "    PASS: ${l_app} is installed, version unavailable"
        fi
    fi
}

function check_local_prereqs_restart()
{
    check_app_installed "ssh"
}

function check_local_prereqs_copy()
{
    check_app_installed "ssh"
    check_app_installed "scp"
}

function check_local_prereqs_build()
{
    check_app_installed "docker"
}

function check_local_prereqs_deploy()
{
    check_local_prereqs_copy
    check_app_installed "docker"
}

function gather_username_host()
{
    read -p "Please enter IP address of remote host:" REMOTE_HOST
    read -p "Please enter username at remote host:" REMOTE_USERNAME  

    SCP_REMOTE_DIR="${REMOTE_USERNAME}@${REMOTE_HOST}:/${REMOTE_USERNAME}/"
    SSH_CONNECT_CMD="${REMOTE_USERNAME}@${REMOTE_HOST}"
}

function create_docker_images()
{
    docker build -t purplefridaytweetlistener:latest -f ./PurpleFridayTweetListener/Dockerfile PurpleFridayTweetListener/
    docker build -t purplefriday_wa:latest -f ./Web.Admin/Dockerfile Web.Admin/
}

function create_docker_tars()
{
    echo "Creating PurpleFridayTweetListener TAR file..."
    docker save -o ./purplefridaytweetlistener.tar purplefridaytweetlistener:latest
    echo "Creating WebAdmin TAR file..."
    docker save -o ./purplefriday_wa.tar purplefriday_wa:latest
}

function copy_tars_to_remote_host()
{
    echo "Copying TAR files to remote host (you will be prompted for your password to the remote host)"
    scp ./docker-compose.yml ./purplefridaytweetlistener.tar ./purplefriday_wa.tar "${SCP_REMOTE_DIR}" 
}

function run_remote_deployment_steps()
{
    echo "Copying remote deployment script to host (you will be prompted for your password to the remote host)"
    scp remote_deploy.sh "${SCP_REMOTE_DIR}"
    
    echo "Running remote deploy script (you will be prompted for your password to the remote host)"
    ssh -tt "${SSH_CONNECT_CMD}" << [EOD]
chmod +x ./remote_deploy.sh
./remote_deploy.sh
exit
[EOD]
}

function restart_remote_docker_services()
{
    echo "Restarting docker containers (you will be prompted for your password to the remote host)"
    ssh -tt "${SSH_CONNECT_CMD}" << [EOD]
docker-compose down
docker-compose up -d
sleep 5s
docker container ls
exit
[EOD]
}

function display_completion()
{
    echo ""
    echo "========================================="
    echo "    $1 completed."
    echo "========================================="
    echo ""
}

function process_build()
{
    check_local_prereqs_build               # Ensure docker is installed locally.
    create_docker_images                    # Create docker images based on GIT repository.
    create_docker_tars                      # Export Docker tars.
    display_completion "Build"              # Confirm to user when finished.
}

function process_deploy()
{
    check_local_prereqs_deploy              # Ensure docker, ssh, scp installed locally.
    gather_username_host                    # Get username and host to connect to.
    copy_tars_to_remote_host                # Copy Docker images to remote host.
    run_remote_deployment_steps             # Run a script copied to remote host to set everything up.
    restart_remote_docker_services          # Stop and start services running on remote host.
    display_completion "Deployment"         # Confirm to user when finished.
}

function process_restart()
{
    check_local_prereqs_restart             # Ensure ssh is installed. Docker is not required locally for this.
    gather_username_host                    # Get username and host to connect to.
    restart_remote_docker_services          # Stop and start services on remote host.
    display_completion "Services Restarted" # Confirm to user when finished.
}

function take_action()
{
    echo " =============================="
    echo " PurpleFriday deployment script"
    echo " =============================="
    echo " Please enter what you would like to do:"
    echo " ------------------------------------------------------------------------------------"
    echo "    1) Build :                  Create Docker images based on the contents"
    echo "                                of the GIT repository, then save them as TAR  "
    echo "                                files ready for deployment."
    echo " ------------------------------------------------------------------------------------"
    echo "    2) Deploy :                 Copy files over to remote host, install any "
    echo "                                required applications (e.g. Docker) and start the "
    echo "                                services."
    echo "                                This will also create the directory structure"
    echo "                                required, but if it is already present it will"
    echo "                                leave it."
    echo " ------------------------------------------------------------------------------------"
    echo "    3) Restart :                Restart Docker containers on remote machine."
    echo " ------------------------------------------------------------------------------------"

    while true ; do
        read -p "Please enter 1-3: " build_or_deploy
        case ${build_or_deploy} in
            1) process_build ; return ;;
            2) process_deploy ; return ;;
            3) process_restart ; return ;;
            *) echo "<${build_or_deploy}> is not a valid input. Please try again or type CTRL-C to exit script" ;;
        esac
    done
}

function main()
{
    set -e -o errtrace       # Set up error trapping
    trap 'error_handler $LINENO' ERR
    set -u                   # Trigger a bad state if any env variables are undefined

    take_action
}

main "$@"