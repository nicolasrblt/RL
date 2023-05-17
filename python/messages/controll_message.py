import json

class ControllMessage:
    def __init__(self, moveInput, turnInput):
        self.moveInput = moveInput
        self.turnInput = turnInput
        
    @staticmethod
    def from_json(json_string):
        message_dict = json.loads(json_string)
        return ControllMessage(message_dict["moveInput"], message_dict["turnInput"])

    def to_json(self):
        return json.dumps(self.__dict__)
