import json

class ResponseMessage:
    def __init__(self, value):
        self.value = value

    @staticmethod
    def from_json(json_string):
        response_message_dict = json.loads(json_string)
        return ResponseMessage(response_message_dict["value"])

    def to_json(self):
        return json.dumps(self.__dict__)
