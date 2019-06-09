#!/bin/sh

sudo apt update
sudo apt upgrade
sudo apt clean

sudo apt install unzip

wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb

sudo add-apt-repository universe
sudo apt-get install apt-transport-https
sudo apt-get update
sudo apt-get install dotnet-sdk-2.2

sudo unzip -o -d /usr/local/bin/PurpleFridayWebApp PurpleFridayWebApp.zip
sudo unzip -o -d /usr/local/bin/PurpleFridayTweetListener PurpleFridayTweetListener.zip

sudo cp PurpleFridayWebApp.service /lib/systemd/system/
sudo cp PurpleFridayTweetListener.service /lib/systemd/system/

sudo systemctl enable PurpleFridayWebApp.service
sudo systemctl enable PurpleFridayTweetListener.service
