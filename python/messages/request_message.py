import json

from .message import Message


class RequestMessage(Message):
    def __init__(self, api, parameter):
        self.api = api
        self.parameter = parameter
