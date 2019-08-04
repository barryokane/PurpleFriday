#!/bin/sh

set -e

echo "Setting up defaults"
sudo apt update
sudo apt upgrade -y
sudo apt clean

sudo apt install -y ufw unattended-upgrades fail2ban

sudo ufw allow ssh
sudo ufw allow http
sudo ufw allow https

echo "Installing .Net Core"
sudo apt install -y unzip

wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb

sudo add-apt-repository universe
sudo apt-get install -y apt-transport-https
sudo apt-get update
sudo apt-get install -y dotnet-sdk-2.2

echo "Creating user purple-friday"
id -u purple-friday || sudo useradd -m purple-friday

echo "Installing app specific packages"
sudo apt install -y sqlite

echo "Installing the Web App"
sudo unzip -o -d /opt/PurpleFridayWebApp PurpleFridayWebApp.zip
sudo chown -R purple-friday:nogroup /opt/PurpleFridayWebApp/
sudo cp PurpleFridayWebApp.service /lib/systemd/system/
sudo systemctl enable PurpleFridayWebApp.service
sudo systemctl start PurpleFridayWebApp.service

echo "Installing the Tweet Listener"
sudo unzip -o -d /opt/PurpleFridayTweetListener PurpleFridayTweetListener.zip
sudo chown -R purple-friday:nogroup /opt/PurpleFridayTweetListener/
sudo cp PurpleFridayTweetListener.service /lib/systemd/system/
sudo systemctl enable PurpleFridayTweetListener.service
sudo systemctl start PurpleFridayTweetListener.service

echo "Proxying using nginx"
sudo apt install -y nginx
sudo cp purple-friday-http /etc/nginx/sites-available/
if [ ! -f /etc/nginx/sites-enabled/purple-friday-http ]; then
  sudo ln -s /etc/nginx/sites-available/purple-friday-http /etc/nginx/sites-enabled/purple-friday-http
fi
if [ -f /etc/nginx/sites-enabled/default ]; then
  sudo rm /etc/nginx/sites-enabled/default
fi
sudo nginx -s reload
echo "Complete!"
