import json

from .message import Message


class RequestMessage(Message):
    """
    Message wrapping an api call
    """
    def __init__(self, api, parameter):
        self.api = api
        self.parameter = parameter
