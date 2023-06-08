import json

from .message import Message


class SingleFieldMessage(Message):
    def __init__(self, x) -> None:
        self.value = x
  