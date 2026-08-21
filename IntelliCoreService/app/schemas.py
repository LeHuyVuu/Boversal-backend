from pydantic import BaseModel
from typing import Optional, Any


class AiRequest(BaseModel):
    prompt: str
    context: Optional[dict] = None


class AiResponse(BaseModel):
    result: Any
    model: str = "placeholder"
