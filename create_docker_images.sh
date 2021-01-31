#!/bin/bash

docker build -t purplefridaytweetlistener:latest -f PurpleFridayTweetListener/Dockerfile PurpleFridayTweetListener/
docker build -t purplefriday_wa:latest -f Web.Admin/Dockerfile Web.Admin/
docker build -t cacheserver:latest -f cacheserver/Dockerfile cacheserver/
