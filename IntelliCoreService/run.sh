#!/usr/bin/env bash
## ./run.sh

set -e

cd "$(dirname "$0")"

if [ ! -x ".venv/bin/python" ]; then
  echo "==> Chưa có .venv, đang tạo..."
  python -m venv .venv
fi

PYTHON="$(pwd)/.venv/bin/python"

if ! "$PYTHON" -c 'import uvicorn' >/dev/null 2>&1; then
  echo "==> Đang cài dependencies..."
  "$PYTHON" -m pip install -r requirements.txt
fi

if [ ! -f ".env" ]; then
  cp .env.example .env
fi

echo "==> Starting AiService on http://0.0.0.0:8080 ..."
exec "$PYTHON" -m uvicorn app.main:app --reload --host 0.0.0.0 --port 8080
