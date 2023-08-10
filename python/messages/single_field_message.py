import json

from .message import Message


class SingleFieldMessage(Message):
    """
    Utility message wrapping native types.
    Required due to Unity JSON engine unablility to serialize native types alone
    """
    def __init__(self, value) -> None:
        self.value = value
  