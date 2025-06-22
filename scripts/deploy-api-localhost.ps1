# --- Configuration Variables ---
$ApiSourcePath = "E:\Workspace\Wisgenix\src\Wisgenix.API"
$ApiPublishPath = "E:\Workspace\Wisgenix\localhost\Wisgenix.API"
$EnvironmentName = "localhost" # Ensure your appsettings.localhost.json exists for this environment

# Database specific paths
$DbSourceFolder = "E:\Workspace\Wisgenix\db"
$DbDestinationFolder = "E:\Workspace\Wisgenix\localhost\db"

Write-Host "`n--- Starting Local API Deployment ---`n"

# 1. Navigate to API source folder
Write-Host "1. Navigating to API source: $ApiSourcePath"
Set-Location -Path $ApiSourcePath

# 2. Publish website in Release mode to localhost folder
Write-Host "2. Publishing API to: $ApiPublishPath"
# Ensure the publish directory is clean before publishing
if (Test-Path $ApiPublishPath) {
    Write-Host "   - Cleaning existing publish folder: $ApiPublishPath"
    Remove-Item -Path $ApiPublishPath -Recurse -Force
}

# Execute dotnet publish
dotnet publish --configuration Release --output "$ApiPublishPath"

# Check if publish was successful
if ($LASTEXITCODE -ne 0) {
    Write-Error "ERROR: dotnet publish failed. Aborting deployment."
    exit 1 # Exit with a non-zero code indicating failure
}

Write-Host "   - API published successfully."

# 3. Database Management Step
Write-Host "3. Managing Database Folder: $DbDestinationFolder`n"

if (Test-Path $DbDestinationFolder) {
    # Destination folder exists, ask for confirmation
    $Choice = Read-Host "   - Destination database folder '$DbDestinationFolder' already exists. Do you want to [R]etain or [C]opy (replace)? (R/C)"
    $Choice = $Choice.Trim().ToUpper()

    if ($Choice -eq "C") {
        Write-Host "   - User chose to replace. Cleaning existing database folder..."
        Remove-Item -Path $DbDestinationFolder -Recurse -Force
		if (-not (Test-Path -Path $DbDestinationFolder)) {
			Write-Host "Destination folder '$DbDestinationFolder' does not exist. Creating it..."
			New-Item -Path $DbDestinationFolder -ItemType Directory -Force | Out-Null
		} 
        Write-Host "   - Copying database files from '$DbSourceFolder' to '$DbDestinationFolder'..."
        Copy-Item -Path "$DbSourceFolder\*" -Destination $DbDestinationFolder -Recurse -Force
        Write-Host "   - Database files copied successfully."
    } elseif ($Choice -eq "R") {
        Write-Host "   - User chose to retain. Skipping database copy."
    } else {
        Write-Warning "   - Invalid choice. Retaining existing database. Please choose 'R' or 'C' next time."
    }
} else {
    # Destination folder does not exist, create it and copy database
    Write-Host "   - Destination database folder '$DbDestinationFolder' does not exist. Creating and copying..."
    New-Item -Path $DbDestinationFolder -ItemType Directory -Force
    Copy-Item -Path "$DbSourceFolder\*" -Destination $DbDestinationFolder -Recurse -Force
    Write-Host "   - Database files copied successfully."
}

# 4. Set environment to localhost for the *current PowerShell session*
#    Note: This sets the environment variable for the *current session*.
#    When you run the API later, you'll need to set it again in that session.
Write-Host "4. Setting ASPNETCORE_ENVIRONMENT to '$EnvironmentName' (for current session/testing)."
$env:ASPNETCORE_ENVIRONMENT = $EnvironmentName
Write-Host "   - ASPNETCORE_ENVIRONMENT is now: $env:ASPNETCORE_ENVIRONMENT"

# 5. Install and trust certificate
Write-Host "5. Installing and trusting .NET HTTPS development certificate..."
dotnet dev-certs https --trust

# Check if dev-certs trust was successful (can be tricky to check exit code for interactive prompts)
if ($LASTEXITCODE -ne 0) {
    Write-Warning "WARNING: 'dotnet dev-certs https --trust' might require user interaction and may not report simple exit code."
    Write-Warning "         Please ensure you followed any prompts to trust the certificate."
} else {
    Write-Host "   - .NET HTTPS development certificate operations completed."
}

# 6. Run API project
Write-Host "6. Starting Wisgenix.API Service..."
Set-Location -Path $ApiPublishPath
$env:ASPNETCORE_ENVIRONMENT = $EnvironmentName
dotnet Wisgenix.API.dll
Write-Host "...Wisgenix.API Service started..."
