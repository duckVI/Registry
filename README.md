# README

## Introduction
This project is a learning endeavor in .NET and ASP.NET Core. It serves as a practical example for understanding the basics of building, running, and publishing ASP.NET Core applications from the command line.

## Prerequisites
- .NET SDK (ensure you have the correct version installed for your application)
- Command Prompt or PowerShell

## Building the Application
To build the application, use the following steps:
1. Open Command Prompt or PowerShell.
2. Navigate to the directory that contains your project file (.csproj) using the 'cd' command.
3. Execute the command 'dotnet build'. This compiles the application and outputs any build errors.

## Running the Application Locally
To run the application locally on your machine:
1. After building the application, remain in the same directory.
2. Execute the command 'dotnet run'. This will start the application on the default port, usually http://localhost:5000.
3. Open your web browser and go to http://localhost:5000 to view the application.

## Publishing the Application
Publishing the application prepares it for deployment by copying the necessary files to a publish directory.

**Framework-Dependent Deployment:**
1. To publish the application for Framework-Dependent Deployment (requires the .NET runtime to be installed on the target machine), execute:
   `dotnet publish -c Release`
   This will publish the application in the Release configuration.

**Self-Contained Deployment for Windows x64:**
1. To publish the application as a self-contained deployment for Windows x64 (does not require the .NET runtime on the target machine), execute:
   `dotnet publish -c Release -r win-x64 --self-contained true`
   This includes the .NET runtime with your application.

## Output
The published application will be located in your project's bin/Release/netcoreapp<version>/win-x64/publish directory (replace `<version>` with the .NET version of your project).

Copy the contents of this directory to the target machine to deploy your application.

## Notes
- Replace `<version>` in the path with the actual .NET version you are targeting.
- The runtime identifier (win-x64) may need to be adjusted if targeting a platform other than Windows x64.
