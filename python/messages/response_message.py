import json

from .message import Message


class ResponseMessage(Message):
    def __init__(self, value):
        self.value = value
