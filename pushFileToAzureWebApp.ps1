# The site below shows how to reference the Azure application publish settings
# REF: https://blogs.msdn.microsoft.com/kaushal/2014/08/01/microsoft-azure-web-site-connect-to-your-site-via-ftp-and-uploaddownload-files/

$file="MyDataFile.txt"

# Fill in the Azure information in the 3 variables below
# NOTE: Split the publishUrl for the FTP profile into the first 2 variables

$ftpUrl="your-prod-svr-007.ftp.azurewebsites.windows.net"
$destintaionDir="site/wwwroot/data"
$userName="AzureApp\`$AzureApp"						# Note that the dollar sign is escaped because that is part of the user name from the publish file.
$password="abcdefghijklmnopqrstuvwxyz0123456789"

Clear

Write-Host 
Write-Host "   Sending $file to $ftpUrl"
Write-Host 
Write-Host "   Generating command file $commandFile.."
Write-Host

# Generate the command file which logs in and pushes the file.
# NOTE: You can always use a static file, but this way you can change the way you do things in THIS file.

$commandFile="pushFileToAzureWebApp.ftpcmd.txt"

Write-Output "user $userName $password"	> 	$commandFile		# Logon
Write-Output "cd $destintaionDir" 		>> 	$commandFile		# Change to destination directory
Write-Output "put ""$file""" 			>> 	$commandFile		# Push the local file
Write-Output "quit" 					>> 	$commandFile		# Exit

Write-Host "ftp -n -s:$commandFile $ftpUrl"

# Initiate FTP without logging in and provide the command file.

ftp -n -s:$commandFile $ftpUrl

Write-Host
Write-Host "                  !!!!!! COMPLETE !!!!!!"
Write-Host