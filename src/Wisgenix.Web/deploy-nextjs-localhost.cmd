@echo off
setlocal

echo.
echo --- Setting variables for Localhost deployment ---
echo.
set NEXTJS_PROJECT_ROOT=%~dp0
set NEXTJS_BUILD_OUTPUT_NAME=build_output
set LOCAL_HOSTING_FOLDER=E:\Workspace\Wisgenix\localhost\Wisgenix.Web
set ENV_FILE=%NEXTJS_PROJECT_ROOT%envs\.env.localhost
set NEXTJS_BUILD_OUTPUT_PATH=%NEXTJS_PROJECT_ROOT%%NEXTJS_BUILD_OUTPUT_NAME%

echo.
echo --- Starting Local QA Deployment for Next.js App ---
echo.

echo 1. Cleaning previous build output folder: %NEXTJS_BUILD_OUTPUT_PATH%
if exist "%NEXTJS_BUILD_OUTPUT_PATH%" (
    call npx rimraf "%NEXTJS_BUILD_OUTPUT_PATH%"
) else (
    echo Build output folder does not exist. Skipping deletion.
)

echo 2. Building Next.js app for QA environment...
pushd "%NEXTJS_PROJECT_ROOT%"
	echo ENV_File used: %ENV_FILE%
	call npx dotenv -e "%ENV_FILE%" -- npm run build
popd

if %errorlevel% neq 0 (
    echo.
    echo ERROR: Next.js build failed. Aborting deployment.
    echo.
    goto :eof
)

echo.
echo 3. Cleaning existing content in local hosting folder: %LOCAL_HOSTING_FOLDER%
if exist "%LOCAL_HOSTING_FOLDER%" (
    call npx rimraf "%LOCAL_HOSTING_FOLDER%"
)
echo %LOCAL_HOSTING_FOLDER%
mkdir "%LOCAL_HOSTING_FOLDER%"

echo 4. Copying built artifacts to local hosting folder...
	xcopy "%NEXTJS_BUILD_OUTPUT_PATH%" "%LOCAL_HOSTING_FOLDER%" /E /I /Y

if %errorlevel% neq 0 (
    echo.
    echo ERROR: Copying artifacts failed. Aborting deployment.
    echo.
    goto :eof
)

echo.
echo --- Deployment to %LOCAL_HOSTING_FOLDER% Complete! ---
echo 5. Starting Webgenix.App
	cd "%LOCAL_HOSTING_FOLDER%" --dir "%LOCAL_HOSTING_FOLDER%\%NEXTJS_BUILD_OUTPUT_NAME%"
	npm run start
echo.

endlocal