import json

from .message import Message


class ResponseMessage(Message):
    """
    Message wrapping a response from the Unity side
    """
    def __init__(self, value):
        self.value = value
