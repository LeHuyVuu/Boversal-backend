#!/bin/bash

# Script setup VPS cho .NET 8 application
# Chạy trên VPS với quyền root

echo "========== Boversal Backend VPS Setup =========="

# Update system
apt-get update
apt-get upgrade -y

# Install .NET 8 SDK
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

apt-get update
apt-get install -y dotnet-sdk-8.0

# Install Nginx
apt-get install -y nginx

# Install Docker (optional - nếu muốn dùng Docker)
apt-get install -y apt-transport-https ca-certificates curl software-properties-common
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | apt-key add -
add-apt-repository "deb [arch=amd64] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable"
apt-get update
apt-get install -y docker-ce docker-compose

# Create application directory
mkdir -p /var/www/boversal-backend
chown -R $USER:$USER /var/www/boversal-backend

# Create systemd service for Gateway
cat > /etc/systemd/system/boversal-gateway.service <<EOF
[Unit]
Description=Boversal API Gateway
After=network.target

[Service]
Type=simple
WorkingDirectory=/var/www/boversal-backend/gateway
ExecStart=/usr/bin/dotnet Boversal.Gateway.dll
Restart=always
RestartSec=10
TimeoutStartSec=300
KillSignal=SIGINT
SyslogIdentifier=boversal-gateway
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://0.0.0.0:5000

[Install]
WantedBy=multi-user.target
EOF

# Create systemd service for ProjectManagement
cat > /etc/systemd/system/boversal-projectmanagement.service <<EOF
[Unit]
Description=Boversal Project Management Service
After=network.target

[Service]
Type=simple
WorkingDirectory=/var/www/boversal-backend/projectmanagement
ExecStart=/usr/bin/dotnet ProjectManagementService.API.dll
Restart=always
RestartSec=10
TimeoutStartSec=300
KillSignal=SIGINT
SyslogIdentifier=boversal-projectmanagement
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://0.0.0.0:5001

[Install]
WantedBy=multi-user.target
EOF

# Create systemd service for Utility
cat > /etc/systemd/system/boversal-utility.service <<EOF
[Unit]
Description=Boversal Utility Service
After=network.target

[Service]
Type=simple
WorkingDirectory=/var/www/boversal-backend/utility
ExecStart=/usr/bin/dotnet UtilityService.dll
Restart=always
RestartSec=10
TimeoutStartSec=300
KillSignal=SIGINT
SyslogIdentifier=boversal-utility
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://0.0.0.0:5002

[Install]
WantedBy=multi-user.target
EOF

# Configure Nginx
cat > /etc/nginx/sites-available/boversal-backend <<EOF
server {
    listen 80;
    listen [::]:80;
    server_name 167.99.68.193;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade \$http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host \$host;
        proxy_cache_bypass \$http_upgrade;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
        client_max_body_size 100M;
    }
}
EOF

ln -sf /etc/nginx/sites-available/boversal-backend /etc/nginx/sites-enabled/
rm -f /etc/nginx/sites-enabled/default

# Test Nginx config
nginx -t

# Reload systemd and start services
systemctl daemon-reload
systemctl enable nginx
systemctl restart nginx

echo "========== VPS Setup Complete! =========="
echo "Services created:"
echo "  - boversal-gateway (port 5000)"
echo "  - boversal-projectmanagement (port 5001)"
echo "  - boversal-utility (port 5002)"
echo "  - nginx (port 80 -> gateway)"
echo ""
echo "Next steps:"
echo "1. Deploy application files to /var/www/boversal-backend/"
echo "2. Start services: systemctl start boversal-*"
echo "3. Check status: systemctl status boversal-*"
