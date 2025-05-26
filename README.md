# DdotM.EmailClient.Mailgun

A simple .NET 8.0 client for sending email via [Mailgun’s HTTP API](https://documentation.mailgun.com/).  

[![NuGet](https://img.shields.io/nuget/v/DdotM.EmailClient.Mailgun.svg)](https://www.nuget.org/packages/DdotM.EmailClient.Mailgun)

---

## Table of Contents

- [Features](#features)  
- [Prerequisites](#prerequisites)  
- [Installation](#installation)  
- [Getting Started](#getting-started)  
  - [Configure Your DNS](#configure-your-dns)  
  - [Basic Usage](#basic-usage)  

---

## Features

- **.NET 8.0 / C# latest**  
- **No dependencies**: .NET only, no external library dependencies  
- **Built-in config validation** (`MailgunClientConfig`)  
- **Supports:** `To`, `Cc`, `Bcc`, `From`, `Subject`, `Text` & `HTML` bodies, tags, tracking, scheduled delivery  

---

## Prerequisites

1. Mailgun account & API key  
2. Verified sending domain in Mailgun (Mailgun provides DNS record instructions when you set up a Sending Domain)  

---

## Installation

```bash
dotnet add package DdotM.EmailClient.Mailgun
```
Or via Package Manager:
```powershell
Install-Package DdotM.EmailClient.Mailgun
```

## Getting Started

### Obtain Mailgun Credentials

1. **Sign in to Mailgun** and go to the **Sending** section in your dashboard.  
2. **Add or verify** your sending domain (e.g. `mg.yourdomain.com`).  
3. Navigate to the **API Keys** or **Security** tab and **create a new API key**.  
4. **Save your API key** in a secure location (environment variable, secrets store, etc.)—you’ll need it in your application.

### Configure Your DNS

After adding your sending domain in the Mailgun dashboard, Mailgun will display a set of DNS records (SPF, DKIM, DMARC, tracking CNAME, etc.).  

1. **Log in to your DNS provider** (where you registered your domain).  
2. **Create or update** the records exactly as Mailgun specifies for your sending domain (e.g. `mg.yourdomain.com`).  
3. **Save** your changes and wait for propagation (usually < 60 minutes).  
4. **Verify** using a DNS lookup tool (`dig`, `nslookup`, or [MXToolbox](https://mxtoolbox.com)) that each record resolves correctly.

### Basic Usage

```csharp
using DdotM.EmailClient.Mailgun;

// 1. Configure
var config = new MailgunClientConfig
{
    ApiKey        = "YOUR_MAILGUN_API_KEY",
    SendingDomain = "mg.yourdomain.com"
};

// 2. Create client
var mailer = new MailgunClient(config);

// 3. Build message
var msg = new MailgunMessage
{
    From         = new Recipient { Name = "Alice", Address = "alice@yourdomain.com" },
    Subject      = "Hello from Mailgun!",
    TextBody     = "This is a plain-text body.",
    HtmlBody     = "<p>This is an HTML body.</p>",
    Tracking     = true,
    DeliveryTime = DateTime.UtcNow.AddMinutes(10)
};

// 4. Add recipients & tags
msg.ToEmails.Add(new Recipient { Name = "Bob", Address = "bob@example.com" });
msg.CcEmails.Add(new Recipient { Name = "Eve", Address = "eve@example.org" });
msg.Tags.Add("welcome-email");

// 5. Send
MailgunMessage result = await mailer.SendAsync(msg);
Console.WriteLine($"Status: {result.Response.StatusCode}");
```
