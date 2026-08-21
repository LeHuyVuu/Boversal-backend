import os
from dotenv import load_dotenv

load_dotenv()

JWT_KEY = os.environ.get("JWT_KEY", "your-super-secret-key-min-32-characters-long-12345")
JWT_ISSUER = os.environ.get("JWT_ISSUER", "ProjectManagementAPI")
JWT_AUDIENCE = os.environ.get("JWT_AUDIENCE", "ProjectManagementClient")
