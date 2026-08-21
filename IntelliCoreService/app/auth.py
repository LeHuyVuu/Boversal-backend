import jwt
from fastapi import Cookie, Header, HTTPException
from app.config import JWT_KEY, JWT_ISSUER, JWT_AUDIENCE


def get_current_user(
    authorization: str = Header(default=None),
    x_forwarded_jwt: str = Header(default=None, alias="X-Forwarded-Jwt"),
    jwt_cookie: str = Cookie(default=None, alias="jwt"),
):
    """
    Accepts the JWT from:
    - Gateway via Authorization: Bearer <token>
    - Gateway via X-Forwarded-Jwt
    - Direct browser call via jwt cookie
    """
    token = x_forwarded_jwt or jwt_cookie

    if not token and authorization:
        token = authorization.split(" ")[-1]

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
