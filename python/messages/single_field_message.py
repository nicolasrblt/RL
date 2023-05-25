import json
from messages.controll_message import ControllMessage
class SingleFieldMessage:
    def __init__(self, x) -> None:
        self.value = x
    def to_json(self):
        return json.dumps(self.__dict__)
  