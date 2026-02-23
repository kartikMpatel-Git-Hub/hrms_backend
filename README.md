# HRMS – Setup & Configuration Guide

  This document explains how to run the project locally, including seed data, application configuration, and required credentials.

## 1. Database & Seed Data
Database

Database Name: hrms_db

Database Engine: SQL Server (SQLEXPRESS)

Update the connection string in appsettings.json before running the project.
```
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=hrms_db;User Id=YOUR_DB_USER;Password=YOUR_DB_PASSWORD;TrustServerCertificate=True;"
}
```
### Seed Data (Important for Reviewers)

This project uses seed data to make testing and evaluation easier.

Seed data includes:

Default roles (Admin, User, etc.)

Sample users

Sample games

Sample game slots

Initial user-game state records

📌 When seed data runs

Seed data is automatically inserted:

On first application startup

When the database is empty

Implemented using:

DbContext.OnModelCreating() and/or startup logic

📌 Why seed data is required

Ensures the application is usable immediately

Allows reviewers to test:

Slot booking

Waiting logic

Slot allocation cron job

Fairness algorithm (least-played user priority)

## 2. Slot Allocation & Background Processing
Slot Assignment Logic

Slots are assigned automatically using a background service / cron job

Allocation happens X minutes before slot start time

Priority rules:

Users who have played fewer games

FIFO (requested time)

Deterministic tie-breaking using primary key

This prevents unfair slot monopolization and ensures deterministic behavior.

## 3. Application Configuration (appsettings.json)

Below is the required configuration with placeholders.
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "None"
    }
  },
  "AllowedHosts": "*",

  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=hrms_db;User Id=YOUR_DB_USER;Password=YOUR_DB_PASSWORD;TrustServerCertificate=True;"
  },

  "Jwt": {
    "Issuer": "https://localhost:7234",
    "Audience": "https://localhost:7234",
    "Key": "YOUR_JWT_SECRET_KEY"
  },

  "EmailSettings": {
    "Host": "smtp.gmail.com",
    "Port": "587",
    "Username": "YOUR_EMAIL",
    "Password": "YOUR_EMAIL_APP_PASSWORD",
    "EnableSsl": true,
    "From": "YOUR_EMAIL"
  },

  "CloudinarySettings": {
    "CloudName": "YOUR_CLOUDINARY_CLOUD_NAME",
    "ApiKey": "YOUR_CLOUDINARY_API_KEY",
    "ApiSecret": "YOUR_CLOUDINARY_API_SECRET"
  }
}
```
## 4. Passwords & Secrets (Very Important)

⚠️ Security Notice for Reviewers

All sensitive values are intentionally removed from the repository.

Replace the placeholders with your own values before running.
```
Required Secrets
Setting	Description
YOUR_DB_USER	SQL Server username
YOUR_DB_PASSWORD	SQL Server password
YOUR_JWT_SECRET_KEY	Secret key for JWT token generation
YOUR_EMAIL	Gmail address for sending notifications
YOUR_EMAIL_APP_PASSWORD	Gmail App Password (not normal password)
YOUR_CLOUDINARY_*	Cloudinary credentials for media uploads
```
📌 Do NOT use real credentials in Git commits

## 5. Email Configuration Notes

Gmail SMTP is used

You must generate an App Password from Google Account

Normal Gmail password will NOT work

Steps:

Enable 2-Step Verification

Generate App Password

Use it in EmailSettings.Password

## 6. How to Run the Project

Update appsettings.json

Ensure SQL Server is running

Run migrations (if required)

Start the application

Seed data will be created automatically

## 7. Notes for Reviewers

Seed data is required to test booking & allocation logic

Background services handle slot assignment automatically

Ordering logic is deterministic and fairness-based

No manual DB setup is required beyond connection string

## 8. Seed User Accounts

email : hr@gmail.com

password : P@ssw0rd.018
