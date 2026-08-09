# Boversal Backend

### Project & Task Management System — Microservices Architecture

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge)
![MySQL](https://img.shields.io/badge/MySQL-Database-4479A1?style=for-the-badge)
![Kafka](https://img.shields.io/badge/Kafka-Event%20Bus-231F20?style=for-the-badge)
![Docker](https://img.shields.io/badge/Docker-Deploy-2496ED?style=for-the-badge)
![AWS S3](https://img.shields.io/badge/AWS%20S3-Storage-FF9900?style=for-the-badge)

---

## What does this project do?

**Boversal** is the backend for a platform similar to a lightweight **Trello + Google Calendar**:

| Feature | Description |
|---|---|
| Project Management | Create projects, add members, track status |
| Task Management | Kanban-style, assignees, deadlines, priority |
| Meetings | Schedule meetings, automatically send invitation emails to attendees |
| Reminders | Automatically send email reminders at the scheduled time |
| Comments & Attachments | Discussion per task/project, file upload to S3 |
| Login/Registration | JWT-based authentication |

---

## Architecture Overview

```mermaid
flowchart TB
    Client["Client / Frontend"]

    Client --> GW

    subgraph GW["Boversal.Gateway"]
        direction TB
        G1["YARP Reverse Proxy\n+ JWT Cookie -> Header"]
    end

    GW --> PMS
    GW --> UTS

    subgraph PMS["ProjectManagementService"]
        direction TB
        P1["Auth - Project - Task\nMeeting - Reminder - Dashboard"]
    end

    subgraph UTS["UtilityService"]
        direction TB
        U1["Send Email - Upload File"]
    end

    PMS <--> DB[("MySQL")]
    PMS -- "Kafka: meeting-created" --> UTS
    UTS --> S3[("AWS S3")]
    UTS --> Mail["SMTP Email"]
```

---

## Role of Each Service

<table>
<tr>
<td width="33%" valign="top">

### Gateway
**Single entry point**

- Routes requests to the correct service
- Converts the `jwt` cookie into an `Authorization` header
- Exposes a `/health` endpoint

</td>
<td width="33%" valign="top">

### ProjectManagement
**Core business logic**

- Auth, User, Project, Task
- Meeting, Reminder, Dashboard
- Publishes a Kafka event when a Meeting is created
- Persists data in MySQL

</td>
<td width="33%" valign="top">

### Utility
**Supporting service**

- Listens to Kafka to send meeting invitation emails
- Sends emails directly via API
- Uploads files/images to AWS S3

</td>
</tr>
</table>

---

## Main Technologies

| Category | Technology |
|---|---|
| Language / Platform | C# / .NET 8 |
| Code Architecture | Clean Architecture, CQRS (MediatR), FluentValidation |
| Database | MySQL with EF Core |
| Asynchronous Communication | Apache Kafka |
| Authentication | JWT |
| File Storage | AWS S3 |
| Containerization | Docker |
| CI/CD | GitHub Actions |

---

## Deployment

<div align="center">

<table>
<tr>
<td></td><td></td><td></td><td></td><td></td><td></td>
<td colspan="5" align="center"><sub><b>AWS Cloud</b></sub></td>
</tr>
<tr>
<td align="center"><img src="https://cdn.simpleicons.org/github/181717" width="42"/></td>
<td align="center">→</td>
<td align="center"><img src="https://cdn.simpleicons.org/githubactions/2088FF" width="42"/></td>
<td align="center">→</td>
<td align="center"><img src="https://cdn.simpleicons.org/docker/2496ED" width="42"/></td>
<td align="center">→</td>
<td align="center"><img src="https://cdn.simpleicons.org/amazonaws/FF9900" width="38"/></td>
<td align="center">→</td>
<td align="center"><img src="https://cdn.simpleicons.org/amazonaws/FF9900" width="38"/></td>
<td align="center">→</td>
<td align="center"><img src="https://cdn.simpleicons.org/amazonec2/FF9900" width="42"/></td>
</tr>
<tr>
<td align="center"><sub><b>Developer</b><br/>git push main</sub></td>
<td></td>
<td align="center"><sub><b>GitHub Actions</b><br/>Build & push image</sub></td>
<td></td>
<td align="center"><sub><b>Docker Hub</b><br/>Registry</sub></td>
<td></td>
<td align="center"><sub><b>ELB</b><br/>Load Balancer</sub></td>
<td></td>
<td align="center"><sub><b>ASG</b><br/>Auto Scaling Group</sub></td>
<td></td>
<td align="center"><sub><b>EC2</b><br/>Instances</sub></td>
</tr>
</table>

<br/>

**— or, alternatively —**

<br/>

<img src="https://cdn.simpleicons.org/render/46E3B7" width="42"/>
<br/>
<sub><b>Render.com</b><br/>deployed via <code>render.yaml</code></sub>

</div>

> Two deployment options:
> - **AWS EC2 + Docker Compose**, fronted by an **Elastic Load Balancer (ELB)** and managed by an **Auto Scaling Group (ASG)**, deployed automatically via GitHub Actions on every push.
> - **Render.com**, deployed using the included `render.yaml` file.

---

### Summary in One Sentence

**Three lightweight services (Gateway -> ProjectManagement -> Utility), communicating over HTTP and Kafka, together powering a project/task/meeting management system with automatic email notifications and file storage.**
