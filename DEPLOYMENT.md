# 🚀 Boversal Backend - Deployment Guide

## 📋 Deployment Options

### 1. Docker Compose (Recommended for VPS)
### 2. Railway
### 3. Render
### 4. Azure App Service
### 5. AWS ECS/Fargate

---

## 🐳 Option 1: Docker Compose (VPS/Cloud VM)

### Prerequisites
- Ubuntu/Debian VPS
- Docker & Docker Compose installed
- Domain name (optional)

### Step 1: Clone Repository
```bash
git clone https://github.com/LeHuyVuu/Boversal-backend.git
cd Boversal-backend
```

### Step 2: Create .env File
```bash
cp .env.example .env
nano .env
```

Fill in your production values:
```env
DB_CONNECTION_STRING=Server=boversal-bnote-lehuyvu-23a9.k.aivencloud.com;Port=20023;Database=bnote;User=avnadmin;Password=YOUR_PASSWORD;SslMode=Required
JWT_KEY=your-production-secret-key-min-32-characters
```

### Step 3: Build & Run
```bash
docker-compose up -d --build
```

### Step 4: Check Status
```bash
docker-compose ps
docker-compose logs -f
```

### Step 5: Setup Nginx Reverse Proxy (Optional)
```nginx
# /etc/nginx/sites-available/boversal-api
server {
    listen 80;
    server_name api.boversal.com;

    location / {
        proxy_pass http://localhost:5268;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }
}
```

Enable site and restart Nginx:
```bash
sudo ln -s /etc/nginx/sites-available/boversal-api /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx
```

### Step 6: Setup SSL with Let's Encrypt
```bash
sudo apt install certbot python3-certbot-nginx
sudo certbot --nginx -d api.boversal.com
```

---

## 🚂 Option 2: Railway

Railway is easiest for beginners - auto-detects Dockerfile and deploys!

### Step 1: Install Railway CLI
```bash
npm i -g @railway/cli
```

### Step 2: Login & Init
```bash
railway login
cd Boversal-backend
railway init
```

### Step 3: Add Environment Variables
```bash
railway variables set DB_CONNECTION_STRING="Server=boversal-bnote-lehuyvu-23a9.k.aivencloud.com;Port=20023;Database=bnote;User=avnadmin;Password=YOUR_PASSWORD;SslMode=Required"
railway variables set JWT_KEY="your-production-secret-key-min-32-characters"
railway variables set JWT_ISSUER="ProjectManagementAPI"
railway variables set JWT_AUDIENCE="ProjectManagementClient"
railway variables set JWT_EXPIRATION_HOURS="24"
```

### Step 4: Deploy
```bash
railway up
```

Railway will:
- Detect `ProjectManagementService.API/Dockerfile`
- Build and deploy automatically
- Provide a public URL: `https://your-app.up.railway.app`

### Step 5: Setup Custom Domain (Optional)
Go to Railway dashboard → Settings → Domains → Add custom domain

---

## 🎨 Option 3: Render

### Step 1: Create New Web Service
1. Go to [render.com](https://render.com)
2. Click "New +" → "Web Service"
3. Connect your GitHub repo: `LeHuyVuu/Boversal-backend`

### Step 2: Configure Service
```yaml
Name: boversal-project-service
Environment: Docker
Docker Context Directory: .
Docker File Path: ProjectManagementService.API/Dockerfile
```

### Step 3: Add Environment Variables
```
DB_CONNECTION_STRING = Server=boversal-bnote-lehuyvu-23a9.k.aivencloud.com;Port=20023;Database=bnote;User=avnadmin;Password=YOUR_PASSWORD;SslMode=Required
JWT_KEY = your-production-secret-key-min-32-characters
JWT_ISSUER = ProjectManagementAPI
JWT_AUDIENCE = ProjectManagementClient
JWT_EXPIRATION_HOURS = 24
ASPNETCORE_ENVIRONMENT = Production
```

### Step 4: Deploy
Click "Create Web Service" - Render will build and deploy automatically!

---

## ☁️ Option 4: Azure App Service

### Step 1: Install Azure CLI
```bash
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash
```

### Step 2: Login & Create Resource Group
```bash
az login
az group create --name boversal-rg --location southeastasia
```

### Step 3: Create App Service Plan
```bash
az appservice plan create \
  --name boversal-plan \
  --resource-group boversal-rg \
  --is-linux \
  --sku B1
```

### Step 4: Create Web App
```bash
az webapp create \
  --resource-group boversal-rg \
  --plan boversal-plan \
  --name boversal-api \
  --deployment-container-image-name mcr.microsoft.com/dotnet/samples:aspnetapp
```

### Step 5: Configure Environment Variables
```bash
az webapp config appsettings set \
  --resource-group boversal-rg \
  --name boversal-api \
  --settings \
    ConnectionStrings__DefaultConnection="Server=boversal-bnote-lehuyvu-23a9.k.aivencloud.com;Port=20023;Database=bnote;User=avnadmin;Password=YOUR_PASSWORD;SslMode=Required" \
    Jwt__Key="your-production-secret-key-min-32-characters" \
    Jwt__Issuer="ProjectManagementAPI" \
    Jwt__Audience="ProjectManagementClient" \
    Jwt__ExpirationHours="24"
```

### Step 6: Deploy with Docker
```bash
# Build and push to Azure Container Registry
az acr create --resource-group boversal-rg --name boversalacr --sku Basic
az acr login --name boversalacr

docker build -f ProjectManagementService.API/Dockerfile -t boversalacr.azurecr.io/boversal-api:latest .
docker push boversalacr.azurecr.io/boversal-api:latest

az webapp config container set \
  --name boversal-api \
  --resource-group boversal-rg \
  --docker-custom-image-name boversalacr.azurecr.io/boversal-api:latest \
  --docker-registry-server-url https://boversalacr.azurecr.io
```

### Step 7: Access Your API
```
https://boversal-api.azurewebsites.net
```

---

## 🚀 Option 5: AWS ECS Fargate

### Step 1: Install AWS CLI
```bash
curl "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o "awscliv2.zip"
unzip awscliv2.zip
sudo ./aws/install
```

### Step 2: Configure AWS
```bash
aws configure
```

### Step 3: Create ECR Repository
```bash
aws ecr create-repository --repository-name boversal-api --region ap-southeast-1
```

### Step 4: Build & Push Docker Image
```bash
# Login to ECR
aws ecr get-login-password --region ap-southeast-1 | docker login --username AWS --password-stdin YOUR_ACCOUNT_ID.dkr.ecr.ap-southeast-1.amazonaws.com

# Build and tag
docker build -f ProjectManagementService.API/Dockerfile -t boversal-api:latest .
docker tag boversal-api:latest YOUR_ACCOUNT_ID.dkr.ecr.ap-southeast-1.amazonaws.com/boversal-api:latest

# Push to ECR
docker push YOUR_ACCOUNT_ID.dkr.ecr.ap-southeast-1.amazonaws.com/boversal-api:latest
```

### Step 5: Create ECS Cluster
```bash
aws ecs create-cluster --cluster-name boversal-cluster --region ap-southeast-1
```

### Step 6: Create Task Definition
Create `ecs-task-definition.json`:
```json
{
  "family": "boversal-api-task",
  "networkMode": "awsvpc",
  "requiresCompatibilities": ["FARGATE"],
  "cpu": "256",
  "memory": "512",
  "containerDefinitions": [
    {
      "name": "boversal-api",
      "image": "YOUR_ACCOUNT_ID.dkr.ecr.ap-southeast-1.amazonaws.com/boversal-api:latest",
      "portMappings": [
        {
          "containerPort": 8080,
          "protocol": "tcp"
        }
      ],
      "environment": [
        {
          "name": "ASPNETCORE_ENVIRONMENT",
          "value": "Production"
        },
        {
          "name": "ConnectionStrings__DefaultConnection",
          "value": "Server=boversal-bnote-lehuyvu-23a9.k.aivencloud.com;Port=20023;Database=bnote;User=avnadmin;Password=YOUR_PASSWORD;SslMode=Required"
        }
      ]
    }
  ]
}
```

Register task:
```bash
aws ecs register-task-definition --cli-input-json file://ecs-task-definition.json
```

### Step 7: Create ECS Service
```bash
aws ecs create-service \
  --cluster boversal-cluster \
  --service-name boversal-api-service \
  --task-definition boversal-api-task \
  --desired-count 1 \
  --launch-type FARGATE \
  --network-configuration "awsvpcConfiguration={subnets=[subnet-xxxxx],securityGroups=[sg-xxxxx],assignPublicIp=ENABLED}"
```

---

## 🔒 Security Checklist

- [ ] Change all default passwords
- [ ] Use strong JWT secret key (32+ characters)
- [ ] Enable HTTPS/SSL
- [ ] Set `ASPNETCORE_ENVIRONMENT=Production`
- [ ] Never commit `.env` or `appsettings.Production.json`
- [ ] Use environment variables for secrets
- [ ] Enable CORS only for trusted domains
- [ ] Setup rate limiting
- [ ] Enable request logging
- [ ] Setup monitoring (Application Insights, CloudWatch, etc.)

---

## 📊 Monitoring & Health Checks

### Health Check Endpoints
```bash
# ProjectManagementService
curl http://localhost:5268/health

# UtilityService
curl http://localhost:5269/health
```

### View Logs
```bash
# Docker Compose
docker-compose logs -f project-management-service

# Railway
railway logs

# Azure
az webapp log tail --name boversal-api --resource-group boversal-rg

# AWS ECS
aws logs tail /ecs/boversal-api-task --follow
```

---

## 🔄 CI/CD with GitHub Actions

Create `.github/workflows/deploy.yml`:
```yaml
name: Deploy to Production

on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Build Docker Image
        run: |
          docker build -f ProjectManagementService.API/Dockerfile -t boversal-api:latest .
      
      - name: Deploy to Railway
        run: |
          npm i -g @railway/cli
          railway login --browserless
          railway up
        env:
          RAILWAY_TOKEN: ${{ secrets.RAILWAY_TOKEN }}
```

---

## 📞 Support

For deployment issues, contact: [your-email@example.com]

## 🎉 Recommended: Railway or Render
- **Railway**: Best for quick deployment, auto-detects Dockerfile
- **Render**: Great free tier, easy to use
- **VPS + Docker Compose**: Best for full control
