import json
import abc

from .message import Message
from .controll_message import ControllMessage
from .observation_message import ObservationMessage

class MultiMessage(Message):
    """
    Utility class allowing to group several messages into one.
    Useful to control several agents at once
    """
    message_hook = lambda m: m

    def __init__(self, messages):
        self.messages = list(messages)

    @classmethod
    def from_dict(cls, message_dict):
        return cls([cls.message_hook(msg) for msg in message_dict["messages"]])

class MultiControllMessage(MultiMessage):
    """
    Message containing several control messages
    """
    message_hook = ControllMessage.from_dict

class MultiObservationMessage(MultiMessage):
    """
    Message containing several observation messages
    """
    message_hook = ObservationMessage.from_dict
