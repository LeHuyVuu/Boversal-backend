"""
Put your actual AI logic here: model inference, calls to an LLM API,
embeddings, whatever the module ends up doing. Kept separate from
main.py so routes stay thin and the engine is unit-testable on its own.
"""


def run_inference(prompt: str, context: dict | None = None) -> str:
    # TODO: replace with real logic
    return f"Echo: {prompt}"
