import json
import abc

from .message import Message
from .controll_message import ControllMessage
from .observation_message import ObservationMessage

class MultiMessage(Message):
    message_hook = lambda m: m

    def __init__(self, messages):
        self.messages = messages

    @classmethod
    def from_dict(cls, message_dict):
        return cls([cls.message_hook(msg) for msg in message_dict["messages"]])

class MultiControllMessage(MultiMessage):
    message_hook = ControllMessage.from_dict

class MultiObservationMessage(MultiMessage):
    message_hook = ObservationMessage.from_dict
