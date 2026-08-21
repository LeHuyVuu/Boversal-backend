from datetime import datetime, timezone
from fastapi import Depends, FastAPI
from app.auth import get_current_user
from app.schemas import AiRequest, AiResponse
from app.services.ai_engine import run_inference

app = FastAPI(title="Intelligen AI Service", version="1.0.0")


@app.get("/health")
def health():
    return {
        "status": "healthy",
        "service": "Intelligen",
        "timestamp": datetime.now(timezone.utc).isoformat(),
    }


@app.get("/api/gateway-test")
def gateway_test():
    return {
        "status": "ok",
        "service": "Intelligen",
        "message": "Request reached",
        "timestamp": datetime.now(timezone.utc).isoformat(),
    }


@app.post("/api/ai/infer", response_model=AiResponse)
def infer(payload: AiRequest, user=Depends(get_current_user)):
    result = run_inference(payload.prompt, payload.context)
    return AiResponse(result=result)
