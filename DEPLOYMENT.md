# Boversal Backend Deployment Guide

## 🚀 Quick Deploy

### 1. Setup VPS (One-time)

SSH vào VPS:
```bash
ssh root@167.99.68.193
```

Upload và chạy script setup:
```bash
# Copy file setup-vps.sh lên VPS
scp deploy/setup-vps.sh root@167.99.68.193:/root/

# SSH vào VPS và chạy
ssh root@167.99.68.193
chmod +x /root/setup-vps.sh
/root/setup-vps.sh
```

### 2. Setup GitHub Secrets

Vào GitHub repo → Settings → Secrets and variables → Actions → New repository secret

Thêm 3 secrets:

```
VPS_HOST = 167.99.68.193
VPS_USER = root
VPS_PASSWORD = LeHuyVu2692004vps
```

### 3. Deploy

**Tự động:** Push code lên branch `main` → GitHub Actions tự động deploy

**Manual:** Vào Actions tab → Chọn "Deploy to VPS" → Run workflow

---

## 📡 API Endpoints

Sau khi deploy thành công:

### Base URL
```
http://167.99.68.193
```

### API Services

**Gateway** (Port 80):
- Base: `http://167.99.68.193`
- Health: `http://167.99.68.193/health`
- Swagger: `http://167.99.68.193/swagger`

**Project Management Service** (via Gateway):
- Auth Login: `http://167.99.68.193/project-management-service/Auth/login`
- Meetings: `http://167.99.68.193/project-management-service/Meeting`
- Projects: `http://167.99.68.193/project-management-service/Project`
- Tasks: `http://167.99.68.193/project-management-service/Task`

**Utility Service** (Internal only):
- Kafka Consumer: Chạy background
- Email Service: Tự động gửi email khi có event

---

## 🔧 Commands

### Kiểm tra services
```bash
ssh root@167.99.68.193
sudo systemctl status boversal-gateway
sudo systemctl status boversal-projectmanagement
sudo systemctl status boversal-utility
```

### Xem logs
```bash
# Gateway logs
sudo journalctl -u boversal-gateway -f

# ProjectManagement logs
sudo journalctl -u boversal-projectmanagement -f

# Utility logs
sudo journalctl -u boversal-utility -f

# Nginx logs
sudo tail -f /var/log/nginx/access.log
sudo tail -f /var/log/nginx/error.log
```

### Restart services
```bash
sudo systemctl restart boversal-gateway
sudo systemctl restart boversal-projectmanagement
sudo systemctl restart boversal-utility
sudo systemctl restart nginx
```

### Stop services
```bash
sudo systemctl stop boversal-gateway
sudo systemctl stop boversal-projectmanagement
sudo systemctl stop boversal-utility
```

---

## 🌐 Frontend Integration

### API Base URL cho Frontend
```javascript
const API_BASE_URL = 'http://167.99.68.193';

// Example: Login
fetch(`${API_BASE_URL}/project-management-service/Auth/login`, {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  credentials: 'include', // Important for cookies
  body: JSON.stringify({ email, password })
});

// Example: Get Meetings
fetch(`${API_BASE_URL}/project-management-service/Meeting`, {
  credentials: 'include'
});
```

### CORS Configuration
Gateway đã được cấu hình CORS cho phép:
- `http://localhost:3000` (local development)
- `https://yourdomain.com` (production - thêm domain của bạn)

Nếu frontend deploy trên domain khác, cần update CORS trong `Boversal.Gateway/appsettings.json`:
```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "https://your-frontend-domain.com"
    ]
  }
}
```

---

## 📊 Monitoring

### Health Checks
```bash
# Check all services health
curl http://167.99.68.193/health

# Check specific service
curl http://localhost:5001/health  # ProjectManagement
curl http://localhost:5002/health  # Utility
```

### Database Connection
Services kết nối đến MySQL Aiven:
- Host: boversal-bnote-lehuyvu-23a9.k.aivencloud.com:20023
- Database: bnote
- SSL: Required

### Kafka Connection
Services kết nối đến Kafka:
- Bootstrap Server: 167.99.68.193:9092
- Topic: meeting-created

---

## 🔐 Security

### Firewall (Recommended)
```bash
# Allow HTTP, HTTPS, SSH
sudo ufw allow 22/tcp
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw allow 9092/tcp  # Kafka
sudo ufw enable
```

### SSL/HTTPS (Optional - với domain)
Nếu có domain (ví dụ: api.boversal.com):

```bash
# Install Certbot
sudo apt install certbot python3-certbot-nginx

# Get SSL certificate
sudo certbot --nginx -d api.boversal.com

# Nginx sẽ tự động config HTTPS
```

---

## 🐛 Troubleshooting

### Service không start
```bash
# Check logs
sudo journalctl -u boversal-gateway -n 50

# Check file permissions
ls -la /var/www/boversal-backend/

# Ensure www-data owns files
sudo chown -R www-data:www-data /var/www/boversal-backend
```

### Nginx 502 Bad Gateway
```bash
# Check services running
sudo systemctl status boversal-gateway

# Check ports
sudo netstat -tlnp | grep :5000
```

### Database connection error
- Verify connection string trong appsettings.Production.json
- Check SSL certificates
- Test connection từ VPS: `mysql -h boversal-bnote-lehuyvu-23a9.k.aivencloud.com -P 20023 -u avnadmin -p`

---

## 📝 Production Configuration

Các config quan trọng cần set trong VPS:

### /var/www/boversal-backend/gateway/appsettings.Production.json
```json
{
  "Cors": {
    "AllowedOrigins": ["http://your-frontend-domain.com"]
  }
}
```

### /var/www/boversal-backend/projectmanagement/appsettings.Production.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=boversal-bnote-lehuyvu-23a9.k.aivencloud.com;Port=20023;Database=bnote;User=avnadmin;Password=AVNS_u9guVM-XJIxiUH2uJFf;SslMode=Required"
  },
  "Kafka": {
    "BootstrapServers": "167.99.68.193:9092"
  }
}
```

### /var/www/boversal-backend/utility/appsettings.Production.json
```json
{
  "Kafka": {
    "BootstrapServers": "167.99.68.193:9092"
  },
  "Email": {
    "SmtpUser": "evm.system.fpt@gmail.com",
    "SmtpPassword": "wwxoiwuzphtcpfia"
  }
}
```

---

## ✅ Deploy Checklist

- [x] VPS setup complete
- [x] .NET 8 installed
- [x] Nginx configured
- [x] Systemd services created
- [ ] GitHub Secrets added
- [ ] First deployment successful
- [ ] Services running and healthy
- [ ] Frontend can connect to API
- [ ] Database accessible
- [ ] Kafka working
- [ ] Emails sending

---

## 🎯 Final URLs for Frontend

```javascript
const config = {
  API_BASE_URL: 'http://167.99.68.193',
  AUTH_LOGIN: 'http://167.99.68.193/project-management-service/Auth/login',
  AUTH_REGISTER: 'http://167.99.68.193/project-management-service/Auth/register',
  MEETINGS: 'http://167.99.68.193/project-management-service/Meeting',
  PROJECTS: 'http://167.99.68.193/project-management-service/Project',
  TASKS: 'http://167.99.68.193/project-management-service/Task'
};
```

**Nhớ set `credentials: 'include'` trong mọi fetch request để gửi cookies!**
