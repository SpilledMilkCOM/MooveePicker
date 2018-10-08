# File: deleteTest.ps1

# Ref: https://docs.microsoft.com/en-us/powershell/azure/overview?view=azurermps-5.6.0

Clear

$resourceGroupName = "MooveePickerLoadTest"

Write-Host "Importing AzureRM..."
Import-Module -Name AzureRM
Write-Host "Imported." -ForegroundColor Green


# Checks to see if there is already a connection.

$azureContext = Get-AzureRmContext

if ($azureContext -eq $null -or $azureContext.Account.Id -eq $null)
{
	# Prompts for Azure credentials.

	$azureAccount = Connect-AzureRmAccount

    $accountId = $azureAccount.Context.Account.Id

    Write-Host "Connected ($accountId)." -ForegroundColor Green
    Write-Host ""
}
else
{
    $accountId = $azureContext.Account.Id

    Write-Host "Already Connected ($accountId)." -ForegroundColor Green
    Write-Host ""
}

$accountId = $azureAccount.Context.Account.Id

Write-Host "Connected ($accountId)." -ForegroundColor Green
Write-Host ""

# Start the timing here since the connection involves the user typing.

$sw = New-Object -TypeName System.Diagnostics.Stopwatch
$sw.Start()

# Check for existing resource

$busResource = Get-AzureRmResourceGroup | Where ResourceGroupName -EQ $resourceGroupName

if ($busResource -eq $null)
{
    Write-Host "Bus resource '$resourceGroupName' does not exist ..." -ForegroundColor Green
}
else
{
    Write-Host "Deleting bus resource '$resourceGroupName' ..." -ForegroundColor Yellow
    $busResource
     
    # If you have multiple subscriptionId in single account
    #$subscriptionId = (Get-AzureRmSubscription | Out-GridView -Title "Select an Azure Subscription …" -PassThru).SubscriptionId
 
    # For single subscriptionId
    #$subscriptionId = (Get-AzureRmSubscription).SubscriptionId
 
    # Remove/Delete Resource Group
    $deleted = Remove-AzureRmResourceGroup -Name $resourceGroupName -Force
 
    if($deleted  -eq 'True')
    {
        Write-Host ""
        Write-Host "Resource group deleted successfully" -ForegroundColor Green
    }
}

$elapsed = $sw.ElapsedMilliseconds.ToString("N0")

Write-Host "Elapsed $elapsed milliseconds."