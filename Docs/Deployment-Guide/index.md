# Deployment Guide for .NET 8 Web Application on IIS

## Introduction
This document provides a step-by-step guide for deploying a .NET 8 web application on IIS and enabling Basic Authentication.

## Prerequisites
- Windows Server with IIS installed.
- .NET 8 Hosting Bundle installed on the server.
- A .NET 8 web application ready for deployment.


## Useful links
- .NET 8 Runtime: https://dotnet.microsoft.com/en-us/download/dotnet/8.0
- .NET 8 Hosting Bundle: https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-8.0.8-windows-hosting-bundle-installer

## Deploying the Web Application

### Publish the Application
1. Open your .NET 8 project in Visual Studio.
2. Right-click on the project in Solution Explorer and select **Publish**:   ![img-1](img-1.png)
3. Select **Folder**, and Specify a folder path where the published files will be saved:   ![img-2](img-2.png)   ![img-3](img-3.png)
5. Click **Publish** to build and publish your application.

### Create a New Site in IIS
1. In IIS Manager, right-click on **Sites** and select **Add Website**:    ![img-4](img-4.png)
2. Fill in the **Site name**, **Physical path** (point to the folder where you published your application), and **Binding** settings:   ![img-5](img-5.png)
3. Click **OK** to create the site.

## Enabling Basic Authentication in Windows Features

To use Basic Authentication, you must ensure the feature is enabled:

1. Open **Control Panel**.
2. Click on **Programs**:  ![img-6](img-6.png)
3. Click on **Turn Windows Features on or off**:  ![img-7](img-7.png)
4. Go to **Internet Information Services-> World Wide Web Services-> Security** and make sure **Basic Authentication** is checked:  ![img-8](img-8.png)

## Adding Basic Authentication

### Enable Basic Authentication
1. In IIS Manager, select your web application.
2. Double-click on **Authentication** in the middle pane:  ![img-9](img-9.png)
3. Disable **Anonymous Authentication**:  ![img-10](img-10.png)
4. Enable **Basic Authentication**:  ![img-11](img-11.png)

### Step 2: Configure Users
1. Open **Computer Management**.
2. In the left menu, expand **Local Users and Groups**:  ![img-13](img-13.png)
3. Right-click on **Users** and select **New User ...**:  ![img-14](img-14.png)
4. Fill-out the user's informations and click **Create**:  ![img-16](img-16.png)

## Testing the Deployment
1. Open a web browser and navigate to your site’s URL.
2. You should see a prompt for a username and password :  ![img-12](img-12.png)
3. Enter the credentials of a user you configured in IIS.
4. Verify that the application loads successfully after authentication.  
- In the case of failed authentication, you will receive a 401 error, as shown below:  ![img-17](img-17.png)
- Otherwise, you will receive a proper response from the API, as shown below:  ![img-18](img-18.png)