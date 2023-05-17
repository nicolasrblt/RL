import json

class RequestMessage:
    def __init__(self, api, parameter):
        self.api = api
        self.parameter = parameter
        
    @staticmethod
    def from_json(json_string):
        message_dict = json.loads(json_string)
        return RequestMessage(message_dict["api"], message_dict["parameter"])

    def to_json(self):
        return json.dumps(self.__dict__)
