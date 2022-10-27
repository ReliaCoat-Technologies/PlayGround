#!/bin/bash

# Set the task scheduler timer
echo "$CRON_SCHEDULE echo hello_world >> dotnet /app/MongoBackupDocker.dll" >> /home/cronsettings 
crontab /home/cronsettings 

# Restart thhe task scheduler service
/etc/init.d/cron restart 

# Perform the first backup
dotnet /app/MongoBackupDocker.dll 

# Keep the container running.
tail -f /home/cronsettings 