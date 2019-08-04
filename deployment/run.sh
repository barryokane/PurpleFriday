#!/bin/sh

set -e

user=$1
ip_address=$2
cd $3
root_dir=`pwd`

echo $root_dir

cd $root_dir
git checkout master
git pull

cd $root_dir/Web.Admin

dotnet restore
dotnet publish -c Release -r linux-x64

cd $root_dir/Web.Admin/bin/Release/netcoreapp2.1/linux-x64/publish

zip -r $root_dir/deployment/PurpleFridayWebApp.zip .

cd $root_dir/PurpleFridayTweetListener

dotnet restore
dotnet publish -c Release -r linux-x64

cd PurpleFridayTweetListener/bin/Release/netcoreapp2.2/linux-x64/publish

zip -r $root_dir/deployment/PurpleFridayTweetListener.zip .

cd $root_dir/deployment

scp PurpleFridayWebApp.zip PurpleFridayTweetListener.zip remote-deployment.sh PurpleFridayWebApp.service PurpleFridayTweetListener.service purple-friday-http $user@$ip_address:~/

ssh $user@$ip_address -t './remote-deployment.sh'
