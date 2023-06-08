import json

from .message import Message


class MultiMessage(Message):
    def __init__(self, messages):
        self.messages = messages

    @classmethod
    def from_dict(cls, message_dict, message_hook=lambda msg: msg):
        return cls([message_hook(msg) for msg in message_dict["messages"]])
