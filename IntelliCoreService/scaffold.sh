#!/usr/bin/env bash
# ==========================================================================
# scaffold.sh — Tạo template AiService (Python/FastAPI) cho Boversal backend
#
# Cách dùng:
#   chmod +x scaffold.sh
#   ./scaffold.sh
#
# Chạy tại thư mục gốc repo (ngang hàng với Boversal.Gateway, UtilityService...)
# Script sẽ tạo folder AiService/ với đầy đủ code mẫu, isolated, chỉ phụ
# thuộc vào request đi qua Gateway (không đụng MySQL, không đụng Kafka).
# ==========================================================================

set -e

SERVICE_NAME="AiService"

echo "==> Creating $SERVICE_NAME project structure..."
mkdir -p "$SERVICE_NAME/app/services"

# ---------------------------------------------------------------------
# requirements.txt
# ---------------------------------------------------------------------
cat > "$SERVICE_NAME/requirements.txt" <<'EOF'
fastapi==0.115.0
uvicorn[standard]==0.30.6
pydantic==2.9.2
pyjwt==2.9.0
python-dotenv==1.0.1
EOF

# ---------------------------------------------------------------------
# .env.example
# ---------------------------------------------------------------------
cat > "$SERVICE_NAME/.env.example" <<'EOF'
JWT_KEY=your-super-secret-key-min-32-characters-long-12345
JWT_ISSUER=ProjectManagementAPI
JWT_AUDIENCE=ProjectManagementClient
EOF

# ---------------------------------------------------------------------
# app/config.py
# ---------------------------------------------------------------------
cat > "$SERVICE_NAME/app/config.py" <<'EOF'
import os
from dotenv import load_dotenv

load_dotenv()

JWT_KEY = os.environ.get("JWT_KEY", "your-super-secret-key-min-32-characters-long-12345")
JWT_ISSUER = os.environ.get("JWT_ISSUER", "ProjectManagementAPI")
JWT_AUDIENCE = os.environ.get("JWT_AUDIENCE", "ProjectManagementClient")
EOF

# ---------------------------------------------------------------------
# app/auth.py
# ---------------------------------------------------------------------
cat > "$SERVICE_NAME/app/auth.py" <<'EOF'
import jwt
from fastapi import Header, HTTPException
from app.config import JWT_KEY, JWT_ISSUER, JWT_AUDIENCE


def get_current_user(
    authorization: str = Header(default=None),
    x_forwarded_jwt: str = Header(default=None, alias="X-Forwarded-Jwt"),
):
    """
    Validates the same JWT already issued/validated by ProjectManagementService.
    The Gateway forwards it as Authorization: Bearer <token> and/or X-Forwarded-Jwt.
    """
    token = x_forwarded_jwt or (
        authorization.split(" ")[-1] if authorization else None
    )
    if not token:
        raise HTTPException(status_code=401, detail="Missing token")

    try:
        payload = jwt.decode(
            token,
            JWT_KEY,
            algorithms=["HS256"],
            audience=JWT_AUDIENCE,
            issuer=JWT_ISSUER,
        )
        return payload
    except jwt.PyJWTError as e:
        raise HTTPException(status_code=401, detail=f"Invalid token: {e}")
EOF

# ---------------------------------------------------------------------
# app/schemas.py
# ---------------------------------------------------------------------
cat > "$SERVICE_NAME/app/schemas.py" <<'EOF'
from pydantic import BaseModel
from typing import Optional, Any


class AiRequest(BaseModel):
    prompt: str
    context: Optional[dict] = None


class AiResponse(BaseModel):
    result: Any
    model: str = "placeholder"
EOF

# ---------------------------------------------------------------------
# app/services/ai_engine.py
# ---------------------------------------------------------------------
cat > "$SERVICE_NAME/app/services/ai_engine.py" <<'EOF'
"""
Put your actual AI logic here: model inference, calls to an LLM API,
embeddings, whatever the module ends up doing. Kept separate from
main.py so routes stay thin and the engine is unit-testable on its own.
"""


def run_inference(prompt: str, context: dict | None = None) -> str:
    # TODO: replace with real logic
    return f"Echo: {prompt}"
EOF

# ---------------------------------------------------------------------
# app/main.py
# ---------------------------------------------------------------------
cat > "$SERVICE_NAME/app/main.py" <<'EOF'
from datetime import datetime, timezone
from fastapi import FastAPI, Depends
from app.auth import get_current_user
from app.schemas import AiRequest, AiResponse
from app.services.ai_engine import run_inference

app = FastAPI(title="Boversal AI Service")


@app.get("/health")
def health():
    return {
        "status": "healthy",
        "service": "AiService",
        "timestamp": datetime.now(timezone.utc).isoformat(),
    }


@app.post("/api/ai/infer", response_model=AiResponse)
def infer(payload: AiRequest, user=Depends(get_current_user)):
    result = run_inference(payload.prompt, payload.context)
    return AiResponse(result=result)
EOF

# ---------------------------------------------------------------------
# Dockerfile
# ---------------------------------------------------------------------
cat > "$SERVICE_NAME/Dockerfile" <<'EOF'
FROM python:3.11-slim AS base
WORKDIR /app
EXPOSE 8080

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY app/ app/

HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

CMD ["uvicorn", "app.main:app", "--host", "0.0.0.0", "--port", "8080"]
EOF

# ---------------------------------------------------------------------
# .dockerignore
# ---------------------------------------------------------------------
cat > "$SERVICE_NAME/.dockerignore" <<'EOF'
__pycache__/
*.pyc
.env
.venv/
venv/
EOF

# ---------------------------------------------------------------------
# README
# ---------------------------------------------------------------------
cat > "$SERVICE_NAME/README.md" <<'EOF'
# AiService

Isolated Python/FastAPI service for AI features. No DB, no Kafka —
only depends on HTTP requests routed through Boversal.Gateway.

## Run locally
    python -m venv .venv && source .venv/bin/activate
    pip install -r requirements.txt
    cp .env.example .env
    uvicorn app.main:app --reload --port 8080

## Run with Docker
    docker build -t ai-service .
    docker run -p 8080:8080 --env-file .env ai-service

## Endpoints
- GET  /health
- POST /api/ai/infer   (requires Authorization: Bearer <jwt>)
EOF

echo "==> Done. Structure:"
find "$SERVICE_NAME" -type f | sort
echo ""
echo "Next steps:"
echo "  1. cd $SERVICE_NAME && python -m venv .venv && source .venv/bin/activate && pip install -r requirements.txt"
echo "  2. cp .env.example .env   # fill in real JWT_KEY etc."
echo "  3. uvicorn app.main:app --reload --port 8080"
echo "  4. Add 'ai-service' route/cluster to Boversal.Gateway/appsettings.json"
echo "  5. Add 'ai-service' block to docker-compose.yml"
